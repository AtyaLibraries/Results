namespace Atya.Foundation.Results;

/// <summary>
/// Represents the outcome of an operation that either succeeds with a value or fails with an <see cref="Error"/>.
/// </summary>
/// <typeparam name="TValue">The success value type.</typeparam>
public sealed record class Result<TValue>
{
    private readonly Error? _error;
    private readonly TValue? _value;

    private Result(bool isSuccess, TValue? value, Error? error)
    {
        IsSuccess = isSuccess;
        _value = value;
        _error = error;
    }

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the success value.
    /// </summary>
    /// <exception cref="InvalidOperationException">The result is failed.</exception>
    public TValue Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException("A failed result does not have a value.");

    /// <summary>
    /// Gets the failure error.
    /// </summary>
    /// <exception cref="InvalidOperationException">The result is successful.</exception>
    public Error Error =>
        IsFailure
            ? _error!
            : throw new InvalidOperationException("A successful result does not have an error.");

    internal static Result<TValue> CreateSuccess(TValue value) => new(isSuccess: true, value, error: null);

    internal static Result<TValue> CreateFailure(Error error) =>
        new(isSuccess: false, value: default, error);

    /// <summary>
    /// Executes one of two delegates based on the result state.
    /// </summary>
    /// <typeparam name="TResult">The value returned by the selected delegate.</typeparam>
    /// <param name="onSuccess">Delegate executed when the result is successful.</param>
    /// <param name="onFailure">Delegate executed when the result is failed.</param>
    /// <returns>The value returned by the selected delegate.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="onSuccess"/> or <paramref name="onFailure"/> is <see langword="null"/>.
    /// </exception>
    public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<Error, TResult> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        return IsSuccess ? onSuccess(Value) : onFailure(Error);
    }

    /// <summary>
    /// Maps a successful result value to a new successful typed result.
    /// </summary>
    /// <typeparam name="TNext">The mapped value type.</typeparam>
    /// <param name="mapper">Delegate that maps the success value.</param>
    /// <returns>A typed result with either the mapped value or the current error.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="mapper"/> is <see langword="null"/>.</exception>
    public Result<TNext> Map<TNext>(Func<TValue, TNext> mapper)
    {
        ArgumentNullException.ThrowIfNull(mapper);

        return IsSuccess ? Result.Success(mapper(Value)) : Result.Failure<TNext>(Error);
    }

    /// <summary>
    /// Binds a successful result value to another typed result.
    /// </summary>
    /// <typeparam name="TNext">The bound value type.</typeparam>
    /// <param name="binder">Delegate that creates the next result.</param>
    /// <returns>The bound result, or a typed failed result with the current error.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="binder"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="binder"/> returns <see langword="null"/>.</exception>
    public Result<TNext> Bind<TNext>(Func<TValue, Result<TNext>> binder)
    {
        ArgumentNullException.ThrowIfNull(binder);

        if (IsFailure)
        {
            return Result.Failure<TNext>(Error);
        }

        return binder(Value) ?? throw new InvalidOperationException("A result binder must return a result.");
    }

    /// <summary>
    /// Converts this result to an untyped result, discarding any success value.
    /// </summary>
    /// <returns>An untyped result with the same state.</returns>
    public Result ToResult() => IsSuccess ? Result.Success() : Result.Failure(Error);
}
