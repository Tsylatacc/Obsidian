using Microsoft.AspNetCore.Authorization;
using Obsidian.Application.Features;
using Wolverine;
using Wolverine.Http;

namespace Obsidian.Api.Endpoints
{
    public class AuthEndpoints
    {
        [AllowAnonymous]
        [WolverinePost("/api/auth/signup"), NotTenanted]
        public static async Task<SignUpResponse> SignUp(
            SignUpCommand command,
            IMessageBus bus,
            HttpContext httpContext,
            CancellationToken cancellationToken)
        {
            SignUpResult result = await bus.InvokeForTenantAsync<SignUpResult>(
                Guid.NewGuid().ToString(),
                command, 
                cancellationToken);

            httpContext.Response.Cookies.Append(
            "bearer_token",
            result.Token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                IsEssential = true,
                Path = "/api",
                Expires = result.ExpiresAt
            });

            return new SignUpResponse(result.PaymentUrl);
        }

        [AllowAnonymous]
        [WolverinePost("/api/auth/signin"), NotTenanted]
        public static async Task SignIn(
            SignInCommand command,
            IMessageBus bus,
            HttpContext httpContext,
            CancellationToken cancellationToken)
        {
            SignInResult result = await bus.InvokeAsync<SignInResult>(command, cancellationToken);

            if (result is not null)
            {
                httpContext.Response.Cookies.Append(
                "bearer_token",
                result.Token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    IsEssential = true,
                    Path = "/api",
                    Expires = result.ExpiresAt
                });
            }
        }
    }
}
