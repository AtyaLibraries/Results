namespace Atya.Foundation.Results.UnitTests;

public sealed class ResultTests
{
    private static readonly Error FailureError =
        new("atya.foundation.results.failure", "The operation failed.");

    [Fact]
    public void Success_CreatesSuccessfulResult()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void Success_ReusesSingleton()
    {
        var first = Result.Success();
        var second = Result.Success();

        first.Should().BeSameAs(second);
    }

    [Fact]
    public void Success_Error_Throws()
    {
        var result = Result.Success();

        var action = () => result.Error;

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Failure_Error_CreatesFailedResult()
    {
        var result = Result.Failure(FailureError);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FailureError);
    }

    [Fact]
    public void Failure_NullError_Throws()
    {
        var action = () => Result.Failure(error: null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Failure_CodeAndMessage_CreatesError()
    {
        var result = Result.Failure("atya.foundation.results.validation", "Invalid.", ErrorKind.Validation);

        result.Error.Code.Should().Be("atya.foundation.results.validation");
        result.Error.Message.Should().Be("Invalid.");
        result.Error.Kind.Should().Be(ErrorKind.Validation);
    }

    [Fact]
    public void Match_Success_UsesSuccessDelegate()
    {
        var result = Result.Success();

        var value = result.Match(() => "ok", error => error.Code);

        value.Should().Be("ok");
    }

    [Fact]
    public void Match_Failure_UsesFailureDelegate()
    {
        var result = Result.Failure(FailureError);

        var value = result.Match(() => "ok", error => error.Code);

        value.Should().Be(FailureError.Code);
    }

    [Fact]
    public void Match_NullSuccessDelegate_Throws()
    {
        var result = Result.Success();

        var action = () => result.Match<string>(onSuccess: null!, error => error.Code);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Match_NullFailureDelegate_Throws()
    {
        var result = Result.Success();

        var action = () => result.Match(() => "ok", onFailure: null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Map_Success_CreatesTypedSuccess()
    {
        var result = Result.Success();

        var mapped = result.Map(() => 42);

        mapped.Value.Should().Be(42);
    }

    [Fact]
    public void Map_Failure_PropagatesError()
    {
        var result = Result.Failure(FailureError);

        var mapped = result.Map(() => 42);

        mapped.Error.Should().Be(FailureError);
    }

    [Fact]
    public void Map_NullMapper_Throws()
    {
        var result = Result.Success();

        var action = () => result.Map<int>(mapper: null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Bind_Success_ReturnsBoundResult()
    {
        var result = Result.Success();

        var bound = result.Bind(() => Result.Success(42));

        bound.Value.Should().Be(42);
    }

    [Fact]
    public void Bind_Failure_PropagatesError()
    {
        var result = Result.Failure(FailureError);

        var bound = result.Bind(() => Result.Success(42));

        bound.Error.Should().Be(FailureError);
    }

    [Fact]
    public void Bind_NullBinder_Throws()
    {
        var result = Result.Success();

        var action = () => result.Bind<int>(binder: null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Bind_NullResult_Throws()
    {
        var result = Result.Success();

        var action = () => result.Bind<int>(() => null!);

        action.Should().Throw<InvalidOperationException>();
    }
}
