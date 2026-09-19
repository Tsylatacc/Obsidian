namespace Obsidian.Infrastructure.Options
{
    public sealed class CorsOptions
    {
        public const string SectionName = "Cors";
        public const string PolicyName = "Frontend";

        public string[] Origins { get; init; } = [];
    }
}
