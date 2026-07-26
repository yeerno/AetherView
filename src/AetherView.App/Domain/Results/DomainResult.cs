namespace AetherView.App.Domain.Results;

public readonly record struct DomainResult
{
    private DomainResult(bool isSuccess, DomainError error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public DomainError Error { get; }

    public static DomainResult Success()
    {
        return new DomainResult(true, DomainError.None);
    }

    public static DomainResult Failure(DomainError error)
    {
        if (error is DomainError.None)
        {
            throw new ArgumentOutOfRangeException(nameof(error));
        }

        return new DomainResult(false, error);
    }
}

public readonly record struct DomainResult<T>
    where T : class
{
    private DomainResult(bool isSuccess, T? value, DomainError error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public bool IsSuccess { get; }

    public T? Value { get; }

    public DomainError Error { get; }

    public static DomainResult<T> Success(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return new DomainResult<T>(true, value, DomainError.None);
    }

    public static DomainResult<T> Failure(DomainError error)
    {
        if (error is DomainError.None)
        {
            throw new ArgumentOutOfRangeException(nameof(error));
        }

        return new DomainResult<T>(false, null, error);
    }
}
