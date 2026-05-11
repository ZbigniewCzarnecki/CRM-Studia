using SalonCRM.Models;

namespace SalonCRM.Tests.Unit;

public class ClientModelTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var client = new Client("Anna", "Kowalska", "500-100-200", "anna@email.pl");

        Assert.Equal("Anna", client.FirstName);
        Assert.Equal("Kowalska", client.LastName);
        Assert.Equal("500-100-200", client.PhoneNumber);
        Assert.Equal("anna@email.pl", client.Email);
    }

    [Fact]
    public void Equals_SameEmail_ReturnsTrue()
    {
        var a = new Client("Anna", "Kowalska", "500-100-200", "anna@email.pl");
        var b = new Client("Anna", "Inna", "999-999-999", "anna@email.pl");

        Assert.Equal(a, b);
    }

    [Fact]
    public void Equals_DifferentEmail_ReturnsFalse()
    {
        var a = new Client("Anna", "Kowalska", "500-100-200", "anna@email.pl");
        var b = new Client("Anna", "Kowalska", "500-100-200", "other@email.pl");

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void GetHashCode_SameEmail_SameHash()
    {
        var a = new Client("X", "Y", "1", "same@email.pl");
        var b = new Client("A", "B", "2", "same@email.pl");

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        var client = new Client("Anna", "Kowalska", "500-100-200", "anna@email.pl");
        var result = client.ToString();

        Assert.Contains("Anna", result);
        Assert.Contains("Kowalska", result);
        Assert.Contains("500-100-200", result);
        Assert.Contains("anna@email.pl", result);
    }
}
