namespace Obsidian.Infrastructure.ExceptionHandling
{
    public sealed class ForbiddenException : Exception
    {
        public ForbiddenException() : base("Access denied for this resource.") { }
    }
}
