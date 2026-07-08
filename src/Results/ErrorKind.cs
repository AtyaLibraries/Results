namespace Atya.Foundation.Results;

/// <summary>
/// Classifies an expected, recoverable error.
/// </summary>
public enum ErrorKind
{
    /// <summary>
    /// A general operation failure.
    /// </summary>
    Failure = 0,

    /// <summary>
    /// Input did not satisfy validation rules.
    /// </summary>
    Validation = 1,

    /// <summary>
    /// The requested resource was not found.
    /// </summary>
    NotFound = 2,

    /// <summary>
    /// The operation conflicted with current state.
    /// </summary>
    Conflict = 3,

    /// <summary>
    /// The caller is not authenticated for the operation.
    /// </summary>
    Unauthorized = 4,

    /// <summary>
    /// The caller is authenticated but not allowed to perform the operation.
    /// </summary>
    Forbidden = 5,

    /// <summary>
    /// The operation failed in an unexpected but reportable way.
    /// </summary>
    Unexpected = 6,
}
