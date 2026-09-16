using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Obsidian.Infrastructure.Options;
using Obsidian.Infrastructure.Persistence;
using Wolverine.EntityFrameworkCore;

namespace Obsidian.Infrastructure.Extensions
{
    public static class PersistenceServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistence(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            string dbConn = configuration.GetConnectionString("db")
                ?? throw new InvalidOperationException("Connection string 'db' not found.");

            services.AddDbContextWithWolverineManagedConjoinedTenancy<ObsidianDbContext>(
            (builder, connectionString) =>
            {
                builder.UseNpgsql(
                    connectionString.Value,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.UseQuerySplittingBehavior(
                            QuerySplittingBehavior.SplitQuery);
                    });
            });

            services.AddOptions<EvolutionApiOptions>()
                .BindConfiguration(EvolutionApiOptions.SectionName)
                .ValidateOnStart();

            return services;
        }
    }
}