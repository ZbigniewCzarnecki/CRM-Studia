using SalonCRM.Models;

namespace SalonCRM.Tests.Unit;

public class ServiceModelTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var service = new Service("Manicure hybrydowy", "Lakier UV", 60, 120m);

        Assert.Equal("Manicure hybrydowy", service.Name);
        Assert.Equal("Lakier UV", service.Description);
        Assert.Equal(60, service.DurationMinutes);
        Assert.Equal(120m, service.Price);
    }

    [Fact]
    public void Equals_SameName_ReturnsTrue()
    {
        var a = new Service("Manicure hybrydowy", "Opis A", 60, 120m);
        var b = new Service("Manicure hybrydowy", "Opis B", 90, 150m);

        Assert.Equal(a, b);
    }

    [Fact]
    public void Equals_DifferentName_ReturnsFalse()
    {
        var a = new Service("Manicure hybrydowy", "Opis", 60, 120m);
        var b = new Service("Pedicure klasyczny", "Opis", 60, 120m);

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void ToString_ContainsPriceAndDuration()
    {
        var service = new Service("Manicure hybrydowy", "Opis", 60, 120m);
        var result = service.ToString();

        Assert.Contains("60", result);
        Assert.Contains("120", result);
        Assert.Contains("PLN", result);
        Assert.Contains("min", result);
    }
}
