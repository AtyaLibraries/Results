namespace Atya.Foundation.Results.Samples.ConsoleApp;

/// <summary>
/// Runs the sample console application.
/// </summary>
public static class Program
{
    /// <summary>
    /// Demonstrates matching a result into a process exit code.
    /// </summary>
    public static void Main()
    {
        var result = ValidateName("Atya");
        var exitCode = result.Match(
            value =>
            {
                Console.WriteLine($"Hello, {value}.");
                return 0;
            },
            error =>
            {
                Console.Error.WriteLine($"{error.Code}: {error.Message}");
                return 1;
            });

        Environment.ExitCode = exitCode;
    }

    private static Result<string> ValidateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<string>(
                "atya.foundation.results.name-required",
                "A name is required.",
                ErrorKind.Validation);
        }

        return Result.Success(name);
    }
}
