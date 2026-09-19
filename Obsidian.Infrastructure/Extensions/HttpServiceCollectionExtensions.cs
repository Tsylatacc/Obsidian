using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Obsidian.Infrastructure.Abstractions;
using Obsidian.Infrastructure.ExceptionHandling;
using Obsidian.Infrastructure.Extensions;
using Obsidian.Infrastructure.Options;
using Obsidian.Infrastructure.Security.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.RateLimiting;

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

            AddCors(services, configuration);
            AddAuthentication(services, configuration);
            AddAuthorization(services);
            AddRateLimiting(services);

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            return services;
        }

        private static void AddRateLimiting(IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                    httpContext =>
                    {
                        bool isAuthenticated =
                            httpContext.User.Identity?.IsAuthenticated == true;

                        if (isAuthenticated)
                        {
                            string userId = httpContext.User.FindFirst("sub")?.Value
                                ?? httpContext.User.FindFirst(
                                    System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                                ?? httpContext.Connection.RemoteIpAddress?.ToString()
                                ?? "unknown-user";

                            return RateLimitPartition.GetFixedWindowLimiter(
                                $"user:{userId}",
                                _ => new FixedWindowRateLimiterOptions
                                {
                                    PermitLimit = 250,
                                    Window = TimeSpan.FromMinutes(1),
                                    QueueLimit = 0,
                                    AutoReplenishment = true
                                });
                        }

                        string ipAddress = httpContext.Connection.RemoteIpAddress?.ToString()
                            ?? "unknown-ip";

                        return RateLimitPartition.GetFixedWindowLimiter(
                            $"ip:{ipAddress}",
                            _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = 5,
                                Window = TimeSpan.FromMinutes(1),
                                QueueLimit = 0,
                                AutoReplenishment = true
                            });
                    });
            });
        }

        private static void AddCors(
            IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOptions<CorsOptions>()
                .Bind(configuration.GetSection(CorsOptions.SectionName))
                .Validate(options => options.Origins.Length > 0,
                    "At least one CORS origin must be configured.")
                .ValidateOnStart();

            services.AddCors(options =>
            {
                options.AddPolicy(CorsOptions.PolicyName, policy =>
                {
                    policy
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .WithOrigins(configuration
                            .GetSection($"{CorsOptions.SectionName}:Origins")
                            .Get<string[]>() ?? [])
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
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