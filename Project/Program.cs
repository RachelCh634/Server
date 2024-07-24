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
using System.Text;
using System.Security.Claims;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // הוספת שירותים למיכל ההזרקות
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        // הוספת AutoMapper באופן ידני
        builder.Services.AddSingleton<IMapper>(provider =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                // הוספת פרופילים כאן
                cfg.AddProfile<UserProfile>();
                cfg.AddProfile<DonationProfile>();
            });
            return config.CreateMapper();
        });

        // הוספת DbContext
        builder.Services.AddDbContext<DBContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDataBase")));

        // הוספת UserData ו-DonationData למיכל ההזרקות
        builder.Services.AddScoped<UserData>();
        builder.Services.AddScoped<DonationData>();

        // הוספת IUserService עם ההטמעה שלו UserService
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IDonationService, DonationService>();

        //jwt
        var jwtIssuer = builder.Configuration["Jwt:Issuer"];
        var jwtKey = builder.Configuration["Jwt:Key"];

        // בדיקת ערכים שהוגדרו
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
        //jwt for swagger
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

        var app = builder.Build();

        // הגדרת הצנרת של בקשות HTTP
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