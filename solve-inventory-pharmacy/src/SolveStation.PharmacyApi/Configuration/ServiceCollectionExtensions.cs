using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Security.Claims;
using FluentValidation;
using SolveStation.PharmacyApi.Services;
using SolveStation.PrescriptionManagement.Extensions;
using SolveStation.UserManagement.Services;

namespace SolveStation.PharmacyApi.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");

        services.AddAuthentication(options =>
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
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = "role", // Explicitly map the role claim
                NameClaimType = ClaimTypes.NameIdentifier
            };


        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequirePharmacistRole", policy =>
                policy.RequireAssertion(context => 
                    context.User.HasClaim("role", "Pharmacist") || 
                    context.User.HasClaim("role", "Admin") ||
                    context.User.HasClaim(ClaimTypes.Role, "Pharmacist") || 
                    context.User.HasClaim(ClaimTypes.Role, "Admin")));

            options.AddPolicy("RequireDoctorRole", policy =>
                policy.RequireAssertion(context => 
                    context.User.HasClaim("role", "Doctor") || 
                    context.User.HasClaim("role", "Admin") ||
                    context.User.HasClaim(ClaimTypes.Role, "Doctor") || 
                    context.User.HasClaim(ClaimTypes.Role, "Admin")));

            options.AddPolicy("RequireAdminRole", policy =>
                policy.RequireAssertion(context => 
                    context.User.HasClaim("role", "Admin") ||
                    context.User.HasClaim(ClaimTypes.Role, "Admin")));

            options.AddPolicy("RequirePatientRole", policy =>
                policy.RequireAssertion(context => 
                    context.User.HasClaim("role", "Patient") || 
                    context.User.HasClaim("role", "Admin") ||
                    context.User.HasClaim(ClaimTypes.Role, "Patient") || 
                    context.User.HasClaim(ClaimTypes.Role, "Admin")));

            options.AddPolicy("RequirePatientOrDoctor", policy =>
                policy.RequireAssertion(context => 
                    context.User.HasClaim("role", "Patient") || 
                    context.User.HasClaim("role", "Doctor") || 
                    context.User.HasClaim("role", "Admin") ||
                    context.User.HasClaim(ClaimTypes.Role, "Patient") || 
                    context.User.HasClaim(ClaimTypes.Role, "Doctor") || 
                    context.User.HasClaim(ClaimTypes.Role, "Admin")));
        });

        // Register authentication services
        services.AddScoped<SolveStation.Authentication.Services.IAuthService, SolveStation.Authentication.Services.AuthService>();
        services.AddScoped<SolveStation.Authentication.Services.ITokenService, SolveStation.Authentication.Services.TokenService>();
        services.AddScoped<SolveStation.Common.Interfaces.IPasswordService, SolveStation.Authentication.Services.PasswordService>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add MediatR for CQRS pattern
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

        // Add FluentValidation
        services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        // Add custom services
        services.AddScoped<IApiResponseService, ApiResponseService>();
        services.AddScoped<IDrugService, DrugService>();

        // Add user management services
        services.AddScoped<IUserService, UserService>();

        // Add prescription management services
        services.AddPrescriptionManagement();

        return services;
    }

    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Pharmacy System API",
                Version = "v1",
                Description = "A comprehensive pharmacy management system API",
                Contact = new OpenApiContact
                {
                    Name = "Pharmacy System Team",
                    Email = "support@pharmacysystem.com"
                }
            });

            // Add JWT Authentication to Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    new string[] {}
                }
            });

            // Only include XML documentation if the file exists
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }
}
