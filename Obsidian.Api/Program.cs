using JasperFx;
using Obsidian.Application.Features;
using Obsidian.Infrastructure.Extensions;
using Obsidian.Infrastructure.Persistence;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Http;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureHostOptions(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(30));

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddHttpInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseWolverine(options =>
{
    string dbConn = builder.Configuration.GetConnectionString("db")
        ?? throw new InvalidOperationException("Connection string 'db' not found.");

    string rabbitConn = builder.Configuration.GetConnectionString("rabbitmq")
        ?? throw new InvalidOperationException("Connection string 'rabbitmq' not found.");

    options.UseRabbitMq(rabbitConn).AutoProvision();
    options.PublishMessage<CampaignRequested>()
        .ToRabbitExchange("campaign-requested");

    options.PersistMessagesWithPostgresql(dbConn);
    options.UseEntityFrameworkCoreTransactions();
    options.Policies.AutoApplyTransactions();

    options.Discovery.IncludeAssembly(typeof(CampaignCommand).Assembly);
    options.Discovery.IncludeAssembly(typeof(ObsidianDbContext).Assembly);

});
builder.Services.AddWolverineHttp();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapWolverineEndpoints();

return await app.RunJasperFxCommands(args);