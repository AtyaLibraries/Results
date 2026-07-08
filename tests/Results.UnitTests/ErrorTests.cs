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
    }

    [Fact]
    public void Constructor_DefaultKind_UsesFailure()
    {
        var error = new Error("atya.foundation.results.failure", "The operation failed.");

        error.Kind.Should().Be(ErrorKind.Failure);
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
}
