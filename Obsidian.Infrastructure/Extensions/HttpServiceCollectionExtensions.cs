using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Obsidian.Infrastructure.Abstractions;
using Obsidian.Infrastructure.ExceptionHandling;
using Obsidian.Infrastructure.Extensions;
using Obsidian.Infrastructure.Security.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Obsidian.Infrastructure.Extensions
{
    public static class HttpServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<IJwtService, JwtService>();

            AddAuthentication(services, configuration);
            AddAuthorization(services);

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            return services;
        }

        private static void AddAuthentication(
            IServiceCollection services,
            IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = configuration["Jwt:Bearer:Issuer"],
                    ValidAudience = configuration["Jwt:Bearer:Audience"],

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            configuration["Jwt:Bearer:Key"]!))
                };
            });
        }

        public static IServiceCollection AddAuthorization(
        this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes("Bearer")
                    .RequireAuthenticatedUser()
                    .Build();
            });

            return services;
        }
    }
}