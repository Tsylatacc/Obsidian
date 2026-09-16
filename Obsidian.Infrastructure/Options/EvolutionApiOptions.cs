namespace Obsidian.Infrastructure.Options
{
    public sealed class EvolutionApiOptions
    {
        public const string SectionName = "EvolutionApi";

        public string ApiUrl { get; init; } = default!;
        public string ApiKey { get; init; } = default!;
    }
}
