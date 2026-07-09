namespace Atya.Foundation.Results.UnitTests;

public sealed class ErrorTests
{
    [Fact]
    public void Constructor_ValidInput_SetsProperties()
    {
        var error = new Error("atya.foundation.results.failure", "The operation failed.", ErrorKind.Unexpected);

        error.Code.Should().Be("atya.foundation.results.failure");
        error.Message.Should().Be("The operation failed.");
        error.Kind.Should().Be(ErrorKind.Unexpected);
        error.Target.Should().BeNull();
        error.Details.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_DefaultKind_UsesFailure()
    {
        var error = new Error("atya.foundation.results.failure", "The operation failed.");

        error.Kind.Should().Be(ErrorKind.Failure);
    }

    [Fact]
    public void Constructor_TargetAndDetails_SetsProperties()
    {
        var detail = new Error(
            "atya.foundation.results.name-required",
            "A name is required.",
            "Name",
            kind: ErrorKind.Validation);

        var error = new Error(
            "atya.foundation.results.validation",
            "Validation failed.",
            "request",
            [detail],
            ErrorKind.Validation);

        error.Target.Should().Be("request");
        error.Details.Should().ContainSingle().Which.Should().Be(detail);
    }

    [Fact]
    public void Constructor_EmptyTarget_UsesNull()
    {
        var error = new Error("atya.foundation.results.failure", "The operation failed.", " ");

        error.Target.Should().BeNull();
    }

    [Fact]
    public void Constructor_Details_CopiesInput()
    {
        var first = new Error("atya.foundation.results.first", "First.");
        var second = new Error("atya.foundation.results.second", "Second.");
        var details = new List<Error> { first };

        var error = new Error("atya.foundation.results.failure", "The operation failed.", null, details);
        details[0] = second;

        error.Details.Should().ContainSingle().Which.Should().Be(first);
    }

    [Fact]
    public void Constructor_NullDetail_Throws()
    {
        var action = () => new Error(
            "atya.foundation.results.failure",
            "The operation failed.",
            null,
            [null!]);

        action.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_InvalidCode_Throws(string? code)
    {
        var action = () => new Error(code!, "The operation failed.");

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_InvalidMessage_Throws(string? message)
    {
        var action = () => new Error("atya.foundation.results.failure", message!);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ToString_ReturnsCode()
    {
        var error = new Error("atya.foundation.results.failure", "The operation failed.");

        error.ToString().Should().Be("atya.foundation.results.failure");
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        var first = new Error("atya.foundation.results.failure", "The operation failed.", ErrorKind.Failure);
        var second = new Error("atya.foundation.results.failure", "The operation failed.", ErrorKind.Failure);

        first.Should().Be(second);
    }

    [Fact]
    public void Equality_SameDetails_AreEqual()
    {
        var first = new Error(
            "atya.foundation.results.validation",
            "Validation failed.",
            "request",
            [
                new Error(
                    "atya.foundation.results.name-required",
                    "A name is required.",
                    "Name",
                    kind: ErrorKind.Validation),
            ],
            ErrorKind.Validation);
        var second = new Error(
            "atya.foundation.results.validation",
            "Validation failed.",
            "request",
            [
                new Error(
                    "atya.foundation.results.name-required",
                    "A name is required.",
                    "Name",
                    kind: ErrorKind.Validation),
            ],
            ErrorKind.Validation);

        first.Should().Be(second);
        first.GetHashCode().Should().Be(second.GetHashCode());
    }

    [Fact]
    public void Equality_DifferentDetailOrder_AreNotEqual()
    {
        var first = new Error(
            "atya.foundation.results.validation",
            "Validation failed.",
            null,
            [
                new Error("atya.foundation.results.first", "First."),
                new Error("atya.foundation.results.second", "Second."),
            ]);
        var second = new Error(
            "atya.foundation.results.validation",
            "Validation failed.",
            null,
            [
                new Error("atya.foundation.results.second", "Second."),
                new Error("atya.foundation.results.first", "First."),
            ]);

        first.Should().NotBe(second);
    }
}
