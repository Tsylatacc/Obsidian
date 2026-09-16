namespace Obsidian.Infrastructure.ExceptionHandling
{
    public sealed class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}
