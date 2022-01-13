namespace WoodenWorkshop.Common.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(string? message) : base(message) { }

    public ValidationException(string? message, Exception? inner = null) : base(message, inner) { }
}