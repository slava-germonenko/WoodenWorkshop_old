namespace WoodenWorkshop.Common.Exceptions;

public class DuplicateException : Exception
{
    public DuplicateException(string? message) : base(message) { }

    public DuplicateException(string? message, Exception? inner = null) : base(message, inner) { }
}