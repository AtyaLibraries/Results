using System.Globalization;

namespace Atya.Foundation.Results.UnitTests;

public sealed class ResultOfTTests
{
    private static readonly Error FailureError =
        new("atya.foundation.results.failure", "The operation failed.");

    [Fact]
    public void Success_CreatesSuccessfulResult()
    {
        var result = Result.Success("value");

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be("value");
    }

    [Fact]
    public void Success_NullReferenceValue_IsAllowed()
    {
        var result = Result.Success<string?>(null);

        result.Value.Should().BeNull();
    }

    [Fact]
    public void Success_Error_Throws()
    {
        var result = Result.Success(42);

        var action = () => result.Error;

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Failure_Error_CreatesFailedResult()
    {
        var result = Result.Failure<int>(FailureError);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FailureError);
    }

    [Fact]
    public void Failure_Value_Throws()
    {
        var result = Result.Failure<int>(FailureError);

        var action = () => result.Value;

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Failure_NullError_Throws()
    {
        var action = () => Result.Failure<int>(error: null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Failure_CodeAndMessage_CreatesError()
    {
        var result = Result.Failure<int>("atya.foundation.results.conflict", "Conflict.", ErrorKind.Conflict);

        result.Error.Code.Should().Be("atya.foundation.results.conflict");
        result.Error.Message.Should().Be("Conflict.");
        result.Error.Kind.Should().Be(ErrorKind.Conflict);
    }

    [Fact]
    public void Match_Success_UsesSuccessDelegate()
    {
        var result = Result.Success(42);

        var value = result.Match(value => value.ToString(CultureInfo.InvariantCulture), error => error.Code);

        value.Should().Be("42");
    }

    [Fact]
    public void Match_Failure_UsesFailureDelegate()
    {
        var result = Result.Failure<int>(FailureError);

        var value = result.Match(value => value.ToString(CultureInfo.InvariantCulture), error => error.Code);

        value.Should().Be(FailureError.Code);
    }

    [Fact]
    public void Match_NullSuccessDelegate_Throws()
    {
        var result = Result.Success(42);

        var action = () => result.Match<string>(onSuccess: null!, error => error.Code);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Match_NullFailureDelegate_Throws()
    {
        var result = Result.Success(42);

        var action = () => result.Match(value => value.ToString(CultureInfo.InvariantCulture), onFailure: null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Map_Success_CreatesMappedSuccess()
    {
        var result = Result.Success(42);

        var mapped = result.Map(value => value.ToString(CultureInfo.InvariantCulture));

        mapped.Value.Should().Be("42");
    }

    [Fact]
    public void Map_Failure_PropagatesError()
    {
        var result = Result.Failure<int>(FailureError);

        var mapped = result.Map(value => value.ToString(CultureInfo.InvariantCulture));

        mapped.Error.Should().Be(FailureError);
    }

    [Fact]
    public void Map_NullMapper_Throws()
    {
        var result = Result.Success(42);

        var action = () => result.Map<string>(mapper: null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Bind_Success_ReturnsBoundResult()
    {
        var result = Result.Success(42);

        var bound = result.Bind(value => Result.Success(value.ToString(CultureInfo.InvariantCulture)));

        bound.Value.Should().Be("42");
    }

    [Fact]
    public void Bind_Failure_PropagatesError()
    {
        var result = Result.Failure<int>(FailureError);

        var bound = result.Bind(value => Result.Success(value.ToString(CultureInfo.InvariantCulture)));

        bound.Error.Should().Be(FailureError);
    }

    [Fact]
    public void Bind_NullBinder_Throws()
    {
        var result = Result.Success(42);

        var action = () => result.Bind<string>(binder: null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Bind_NullResult_Throws()
    {
        var result = Result.Success(42);

        var action = () => result.Bind<string>(_ => null!);

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ToResult_Success_ReturnsUntypedSuccess()
    {
        var result = Result.Success(42);

        var untyped = result.ToResult();

        untyped.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ToResult_Failure_ReturnsUntypedFailure()
    {
        var result = Result.Failure<int>(FailureError);

        var untyped = result.ToResult();

        untyped.Error.Should().Be(FailureError);
    }
}
