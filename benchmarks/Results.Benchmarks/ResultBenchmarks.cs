using BenchmarkDotNet.Attributes;
using System.Globalization;

namespace Atya.Foundation.Results.Benchmarks;

/// <summary>
/// Benchmarks for common result operations.
/// </summary>
[MemoryDiagnoser]
public class ResultBenchmarks
{
    private static readonly Error FailureError =
        new("atya.foundation.results.failure", "The operation failed.");

    private readonly Result<int> _failure = Result.Failure<int>(FailureError);
    private readonly Result<int> _success = Result.Success(42);

    /// <summary>
    /// Creates a successful typed result.
    /// </summary>
    /// <returns>The created result.</returns>
    [Benchmark(Baseline = true)]
    public Result<int> CreateSuccess() => _success;

    /// <summary>
    /// Creates a failed typed result.
    /// </summary>
    /// <returns>The created result.</returns>
    [Benchmark]
    public Result<int> CreateFailure() => _failure;

    /// <summary>
    /// Maps a successful typed result.
    /// </summary>
    /// <returns>The mapped result.</returns>
    [Benchmark]
    public Result<string> MapSuccess() => _success.Map(value => value.ToString(CultureInfo.InvariantCulture));
}
