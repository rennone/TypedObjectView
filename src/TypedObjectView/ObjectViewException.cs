namespace ObjectViews;

/// <summary>Represents an invalid selector or data that cannot be read by an object view.</summary>
public sealed class ObjectViewException : Exception
{
    /// <summary>Initializes an exception with a description of the failed access.</summary>
    public ObjectViewException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes an exception with a description and its underlying cause.</summary>
    public ObjectViewException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
