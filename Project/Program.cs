using AutoMapper;
using BL.Interfaces;
using BL.Services;
using DAL.Data;
using DAL.Profilies;
using MODELS.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(op =>
        {
            op.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Enter JWT token like this: Bearer {your token}",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
            op.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });

        builder.Services.AddSingleton<IMapper>(provider =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<UserProfile>();
                cfg.AddProfile<DonationProfile>();
            });
            return config.CreateMapper();
        });

        builder.Services.AddDbContext<DBContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDataBase")));

        builder.Services.AddScoped<UserData>();
        builder.Services.AddScoped<DonationData>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IDonationService, DonationService>();
        builder.Services.AddHttpContextAccessor();

        var jwtIssuer = builder.Configuration["Jwt:Issuer"];
        var jwtKey = builder.Configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtKey))
            throw new ArgumentNullException("JWT settings are not configured properly.");

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    RoleClaimType = ClaimTypes.Role,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var userId = context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        var role = context.Principal.FindFirst(ClaimTypes.Role)?.Value;
                        Console.WriteLine($"Token validated successfully. User ID: {userId}, Role: {role}");
                        return Task.CompletedTask;
                    }
                };
            });

        // הגדרת CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin", policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        builder.Services.AddAuthorization();

        var app = builder.Build();

        app.UseCors("AllowSpecificOrigin");

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();
        app.UseAuthentication();

        app.Use(async (context, next) =>
        {
            if (context.User.Identity.IsAuthenticated)
            {
                Console.WriteLine("User authenticated:");
                Console.WriteLine($"User ID: {context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value}");
                Console.WriteLine($"Role: {context.User.FindFirst(ClaimTypes.Role)?.Value}");
            }
            else
            {
                Console.WriteLine("User not authenticated.");
            }
            await next();
        });

        app.UseAuthorization();

        app.MapControllers();
        app.Run();
    }
}
