namespace WoodenWorkshop.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string? message) : base(message) { }

    public NotFoundException(string? message, Exception? inner = null) : base(message, inner) { }
}