using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Obsidian.Infrastructure.Extensions;
using Obsidian.Infrastructure.Persistence;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;
using Wolverine.ErrorHandling;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddPersistence(builder.Configuration);

builder.UseWolverine(options =>
{
    string dbConn = builder.Configuration.GetConnectionString("db")
        ?? throw new InvalidOperationException("Connection string 'db' not found.");

    string rabbitConn = builder.Configuration.GetConnectionString("rabbitmq")
        ?? throw new InvalidOperationException("Connection string 'rabbitmq' not found.");

    options.PersistMessagesWithPostgresql(dbConn);
    options.UseEntityFrameworkCoreTransactions();
    options.Policies.AutoApplyTransactions();

    options.UseRabbitMq(rabbitConn)
        .AutoProvision()
        .BindExchange("campaign-requested")
        .ToQueue("campaign-requested-queue");

    options.ListenToRabbitQueue("campaign-requested-queue")
        .UseDurableInbox();

    options.Discovery.IncludeAssembly(typeof(ObsidianDbContext).Assembly);

    options.OnException<Exception>()
        .RetryWithCooldown(TimeSpan.FromMilliseconds(150))
        .Then.MoveToErrorQueue();
});

var host = builder.Build();
host.Run();