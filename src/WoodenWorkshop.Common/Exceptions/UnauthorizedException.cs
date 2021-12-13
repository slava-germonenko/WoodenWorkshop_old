namespace WoodenWorkshop.Common.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string? message) : base(message) { }

    public UnauthorizedException(string? message, Exception? inner = null) : base(message, inner) { }
}