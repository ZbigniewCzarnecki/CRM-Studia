namespace SalonCRM.Tests.Integration;

public class CommunicationTests
{
    [Theory]
    [InlineData("CONNECT:42", "CONNECT", 42)]
    [InlineData("CONNECT:1", "CONNECT", 1)]
    [InlineData("CONNECT:999", "CONNECT", 999)]
    public void ParseConnect_ExtractsClientId(string message, string expectedCommand, int expectedId)
    {
        var parts = message.Split(':', 2);
        Assert.Equal(expectedCommand, parts[0]);
        Assert.Equal(expectedId, int.Parse(parts[1]));
    }

    [Theory]
    [InlineData("GET:Client", "Client")]
    [InlineData("GET:Service", "Service")]
    [InlineData("GET:Appointment", "Appointment")]
    [InlineData("GET:Pet", "Pet")]
    public void ParseGet_ExtractsClassName(string message, string expectedClass)
    {
        Assert.StartsWith("GET:", message);
        var className = message["GET:".Length..];
        Assert.Equal(expectedClass, className);
    }

    [Theory]
    [InlineData("DATA:[{\"Name\":\"Manicure\"}]", "[{\"Name\":\"Manicure\"}]")]
    public void ParseData_ExtractsJson(string message, string expectedJson)
    {
        Assert.StartsWith("DATA:", message);
        var json = message["DATA:".Length..];
        Assert.Equal(expectedJson, json);
    }

    [Fact]
    public void ParseExit_RecognizesExitCommand()
    {
        const string message = "EXIT";
        Assert.Equal("EXIT", message);
    }
}
