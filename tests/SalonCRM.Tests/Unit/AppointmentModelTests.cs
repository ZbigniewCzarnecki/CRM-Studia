using SalonCRM.Models;

namespace SalonCRM.Tests.Unit;

public class AppointmentModelTests
{
    private static readonly DateTime TestDate = new(2026, 6, 15, 10, 0, 0);

    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var appt = new Appointment("Anna Kowalska", "Manicure hybrydowy", TestDate, 120m);

        Assert.Equal("Anna Kowalska", appt.ClientName);
        Assert.Equal("Manicure hybrydowy", appt.ServiceName);
        Assert.Equal(TestDate, appt.Date);
        Assert.Equal(120m, appt.TotalAmount);
    }

    [Fact]
    public void Equals_SameClientAndDate_ReturnsTrue()
    {
        var a = new Appointment("Anna Kowalska", "Manicure hybrydowy", TestDate, 120m);
        var b = new Appointment("Anna Kowalska", "Inny serwis", TestDate, 999m);

        Assert.Equal(a, b);
    }

    [Fact]
    public void Equals_DifferentClient_ReturnsFalse()
    {
        var a = new Appointment("Anna Kowalska", "Manicure", TestDate, 120m);
        var b = new Appointment("Maria Nowak", "Manicure", TestDate, 120m);

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Equals_DifferentDate_ReturnsFalse()
    {
        var a = new Appointment("Anna Kowalska", "Manicure", TestDate, 120m);
        var b = new Appointment("Anna Kowalska", "Manicure", TestDate.AddHours(1), 120m);

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void ToString_ContainsAllInfo()
    {
        var appt = new Appointment("Anna Kowalska", "Manicure hybrydowy", TestDate, 120m);
        var result = appt.ToString();

        Assert.Contains("Anna Kowalska", result);
        Assert.Contains("Manicure hybrydowy", result);
        Assert.Contains("2026-06-15", result);
        Assert.Contains("120", result);
        Assert.Contains("PLN", result);
    }
}
