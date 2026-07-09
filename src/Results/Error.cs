using System.Diagnostics.CodeAnalysis;

namespace Atya.Foundation.Results;

/// <summary>
/// Describes an expected, recoverable error with a stable code.
/// </summary>
/// <remarks>
/// Equality is structural: <see cref="Code"/>, <see cref="Message"/>, <see cref="Kind"/>,
/// <see cref="Target"/>, and the ordered contents of <see cref="Details"/> all participate.
/// </remarks>
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
        : this(code, message, target: null, details: null, kind)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class.
    /// </summary>
    /// <param name="code">Stable machine-readable error code.</param>
    /// <param name="message">Human-readable error message.</param>
    /// <param name="target">Optional member, field, or resource the error applies to.</param>
    /// <param name="details">Optional child errors. The stored value is never <see langword="null"/>.</param>
    /// <param name="kind">The error classification.</param>
    /// <remarks>
    /// Equality is structural: <see cref="Code"/>, <see cref="Message"/>, <see cref="Kind"/>,
    /// <see cref="Target"/>, and the ordered contents of <see cref="Details"/> all participate.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// <paramref name="code"/> or <paramref name="message"/> is empty.
    /// </exception>
    /// <exception cref="ArgumentNullException"><paramref name="details"/> contains a <see langword="null"/> item.</exception>
    public Error(
        string code,
        string message,
        string? target,
        IReadOnlyList<Error>? details = null,
        ErrorKind kind = ErrorKind.Failure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        Code = code;
        Message = message;
        Kind = kind;
        Target = string.IsNullOrWhiteSpace(target) ? null : target;
        Details = CopyDetails(details);
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

    /// <summary>
    /// Gets the optional member, field, or resource the error applies to.
    /// </summary>
    public string? Target { get; }

    /// <summary>
    /// Gets the child errors. The list is never <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// Equality is structural and includes the ordered child errors in this list.
    /// </remarks>
    public IReadOnlyList<Error> Details { get; }

    /// <inheritdoc />
    public override string ToString() => Code;

    /// <inheritdoc />
    public bool Equals(Error? other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return other is not null
            && string.Equals(Code, other.Code, StringComparison.Ordinal)
            && string.Equals(Message, other.Message, StringComparison.Ordinal)
            && Kind == other.Kind
            && string.Equals(Target, other.Target, StringComparison.Ordinal)
            && Details.SequenceEqual(other.Details);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Code, StringComparer.Ordinal);
        hash.Add(Message, StringComparer.Ordinal);
        hash.Add(Kind);
        hash.Add(Target, StringComparer.Ordinal);

        foreach (var detail in Details)
        {
            hash.Add(detail);
        }

        return hash.ToHashCode();
    }

    private static Error[] CopyDetails(IReadOnlyList<Error>? details)
    {
        if (details is null || details.Count == 0)
        {
            return Array.Empty<Error>();
        }

        var copy = new Error[details.Count];
        for (var index = 0; index < details.Count; index++)
        {
            copy[index] = details[index] ?? throw new ArgumentNullException(nameof(details));
        }

        return copy;
    }
}
