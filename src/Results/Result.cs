namespace Atya.Foundation.Results;

/// <summary>
/// Represents the outcome of an operation that either succeeds or fails with an <see cref="Error"/>.
/// </summary>
public sealed record class Result
{
    private static readonly Result SuccessInstance = new(isSuccess: true, error: null);

    private readonly Error? _error;

    private Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
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
    /// Gets the failure error.
    /// </summary>
    /// <exception cref="InvalidOperationException">The result is successful.</exception>
    public Error Error =>
        IsFailure
            ? _error!
            : throw new InvalidOperationException("A successful result does not have an error.");

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A successful result.</returns>
    public static Result Success() => SuccessInstance;

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">The failure error.</param>
    /// <returns>A failed result.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="error"/> is <see langword="null"/>.</exception>
    public static Result Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return new Result(isSuccess: false, error);
    }

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="code">Stable machine-readable error code.</param>
    /// <param name="message">Human-readable error message.</param>
    /// <param name="kind">The error classification.</param>
    /// <returns>A failed result.</returns>
    public static Result Failure(string code, string message, ErrorKind kind = ErrorKind.Failure) =>
        Failure(new Error(code, message, kind));

    /// <summary>
    /// Creates a successful typed result.
    /// </summary>
    /// <typeparam name="TValue">The success value type.</typeparam>
    /// <param name="value">The success value.</param>
    /// <returns>A successful typed result.</returns>
    public static Result<TValue> Success<TValue>(TValue value) =>
        Result<TValue>.CreateSuccess(value);

    /// <summary>
    /// Creates a failed typed result.
    /// </summary>
    /// <typeparam name="TValue">The success value type.</typeparam>
    /// <param name="error">The failure error.</param>
    /// <returns>A failed typed result.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="error"/> is <see langword="null"/>.</exception>
    public static Result<TValue> Failure<TValue>(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return Result<TValue>.CreateFailure(error);
    }

    /// <summary>
    /// Creates a failed typed result.
    /// </summary>
    /// <typeparam name="TValue">The success value type.</typeparam>
    /// <param name="code">Stable machine-readable error code.</param>
    /// <param name="message">Human-readable error message.</param>
    /// <param name="kind">The error classification.</param>
    /// <returns>A failed typed result.</returns>
    public static Result<TValue> Failure<TValue>(string code, string message, ErrorKind kind = ErrorKind.Failure) =>
        Failure<TValue>(new Error(code, message, kind));

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
    public TResult Match<TResult>(Func<TResult> onSuccess, Func<Error, TResult> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        return IsSuccess ? onSuccess() : onFailure(Error);
    }

    /// <summary>
    /// Maps a successful result to a successful typed result.
    /// </summary>
    /// <typeparam name="TValue">The mapped value type.</typeparam>
    /// <param name="mapper">Delegate that creates the success value.</param>
    /// <returns>A typed result with either the mapped value or the current error.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="mapper"/> is <see langword="null"/>.</exception>
    public Result<TValue> Map<TValue>(Func<TValue> mapper)
    {
        ArgumentNullException.ThrowIfNull(mapper);

        return IsSuccess ? Success(mapper()) : Failure<TValue>(Error);
    }

    /// <summary>
    /// Binds a successful result to another typed result.
    /// </summary>
    /// <typeparam name="TValue">The bound value type.</typeparam>
    /// <param name="binder">Delegate that creates the next result.</param>
    /// <returns>The bound result, or a typed failed result with the current error.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="binder"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="binder"/> returns <see langword="null"/>.</exception>
    public Result<TValue> Bind<TValue>(Func<Result<TValue>> binder)
    {
        ArgumentNullException.ThrowIfNull(binder);

        if (IsFailure)
        {
            return Failure<TValue>(Error);
        }

        return binder() ?? throw new InvalidOperationException("A result binder must return a result.");
    }
}
