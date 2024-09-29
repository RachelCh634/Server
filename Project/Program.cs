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
        builder.Services.AddHttpContextAccessor(); // הכנס כאן את ה-HttpContextAccessor

        var jwtIssuer = builder.Configuration["Jwt:Issuer"];
        var jwtKey = builder.Configuration["Jwt:Key"];

        if (string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtKey))
        {
            throw new ArgumentNullException("JWT settings are not configured properly.");
        }

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
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
                RoleClaimType = ClaimTypes.Role
            };
        });

        builder.Services.AddAuthorization();

        builder.Services.AddSwaggerGen(op =>
        {
            op.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
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
                        Reference=new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                             Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
        });

        var app = builder.Build();

        app.UseCors("AllowSpecificOrigin");

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
