using System.Diagnostics.CodeAnalysis;

namespace Atya.Foundation.Results;

/// <summary>
/// Describes an expected, recoverable error with a stable code.
/// </summary>
[SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "The package models a domain error and exposes the canonical Error type.")]
public sealed record class Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class.
    /// </summary>
    /// <param name="code">Stable machine-readable error code.</param>
    /// <param name="message">Human-readable error message.</param>
    /// <param name="kind">The error classification.</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="code"/> or <paramref name="message"/> is empty.
    /// </exception>
    public Error(string code, string message, ErrorKind kind = ErrorKind.Failure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        Code = code;
        Message = message;
        Kind = kind;
    }

    /// <summary>
    /// Gets the stable machine-readable error code.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the human-readable error message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the error classification.
    /// </summary>
    public ErrorKind Kind { get; }

    /// <inheritdoc />
    public override string ToString() => Code;
}
