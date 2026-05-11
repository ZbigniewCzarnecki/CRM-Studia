using System.Text.Json;
using SalonCRM.Models;

namespace SalonCRM.Tests.Integration;

public class SerializationTests
{
    [Fact]
    public void Serialize_ClientList_DeserializesCorrectly()
    {
        var clients = new List<Client>
        {
            new("Anna", "Kowalska", "500-100-200", "anna@email.pl"),
            new("Maria", "Nowak", "600-300-400", "maria@email.pl")
        };

        var json = JsonSerializer.Serialize(clients);
        var result = JsonSerializer.Deserialize<List<Client>>(json)!;

        Assert.Equal(2, result.Count);
        Assert.Equal("Anna", result[0].FirstName);
        Assert.Equal("anna@email.pl", result[0].Email);
    }

    [Fact]
    public void Serialize_ServiceList_DeserializesCorrectly()
    {
        var services = new List<Service>
        {
            new("Manicure hybrydowy", "Opis", 60, 120m),
            new("Pedicure klasyczny", "Opis", 45, 90m)
        };

        var json = JsonSerializer.Serialize(services);
        var result = JsonSerializer.Deserialize<List<Service>>(json)!;

        Assert.Equal(2, result.Count);
        Assert.Equal("Manicure hybrydowy", result[0].Name);
        Assert.Equal(120m, result[0].Price);
    }

    [Fact]
    public void Serialize_AppointmentList_DeserializesCorrectly()
    {
        var date = new DateTime(2026, 6, 15, 10, 0, 0);
        var appointments = new List<Appointment>
        {
            new("Anna Kowalska", "Manicure hybrydowy", date, 120m)
        };

        var json = JsonSerializer.Serialize(appointments);
        var result = JsonSerializer.Deserialize<List<Appointment>>(json)!;

        Assert.Single(result);
        Assert.Equal("Anna Kowalska", result[0].ClientName);
        Assert.Equal(date, result[0].Date);
    }

    [Fact]
    public void Deserialize_WrongType_ThrowsJsonException()
    {
        var services = new List<Service>
        {
            new("Manicure hybrydowy", "Opis", 60, 120m)
        };

        var json = JsonSerializer.Serialize(services);

        // Service JSON deserializowany jako Pet (z wymaganym polem PetName) musi rzucić JsonException
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<List<PetForTest>>(json));
    }

    private class PetForTest
    {
        [System.Text.Json.Serialization.JsonRequired]
        public string PetName { get; set; } = string.Empty;
    }
}
