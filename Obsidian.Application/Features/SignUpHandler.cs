using Microsoft.EntityFrameworkCore;
using Obsidian.Domain.Entities;
using Obsidian.Domain.Enums;
using Obsidian.Infrastructure.Abstractions;
using Obsidian.Infrastructure.ExceptionHandling;
using Obsidian.Infrastructure.Persistence;
using Obsidian.Infrastructure.Security;

namespace Obsidian.Application.Features
{
    public class SignUpHandler
    {
        public static async Task<SignUpResult> Handle(
             SignUpCommand command,
             ObsidianDbContext db,
             IJwtService jwtService,
             CancellationToken cancellationToken)
        {
            if (await db.Users
                .IgnoreQueryFilters()
                .AnyAsync(x => x.Email == command.Email, cancellationToken))
                throw new ConflictException("A user with this email already exists.");

            SubscriptionPlan subscriptionPlan = await db.SubscriptionPlans
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(x => x.Id == command.SubscriptionPlanId, cancellationToken) ??
                throw new KeyNotFoundException($"Subscription plan {command.SubscriptionPlanId} not found.");

            Subscription subscription = Subscription.Create(subscriptionPlan);
            subscription.AddUsage();

            if (subscriptionPlan.Type == SubscriptionPlanType.Paid)
            {
                Payment payment = Payment.Create(
                    subscription.Id,
                    subscriptionPlan.Price);

                await db.Payments.AddAsync(payment, cancellationToken);
            }
            else
            {
                subscription.Activate();
            }

            string passwordHash = SecurityService.HashPassword(command.Password);
            User user = User.Create(command.Email, passwordHash, subscription.Id);

            await db.Users.AddAsync(user, cancellationToken);
            await db.Subscriptions.AddAsync(subscription, cancellationToken);

            JwtDto jwtDto = jwtService.GenerateBearerToken(user.Id, user.Email);
            return new SignUpResult(
                jwtDto.Token,
                jwtDto.ExpiresAt,
                null);
        }
    }

    public sealed record SignUpCommand(
       string Email,
       string Password,
       Guid SubscriptionPlanId
   );
    public sealed record SignUpResult(
        string Token,
        DateTimeOffset ExpiresAt,
        string? PaymentUrl
    );
    public sealed record SignUpResponse(
        string? PaymentUrl
    );
}
