using SalonCRM.Models;
using SalonCRM.Server;

namespace SalonCRM.Tests.Unit;

public class DataStoreTests
{
    private readonly DataStore _store = new();

    [Fact]
    public void InitializeStore_Contains12Objects()
    {
        Assert.Equal(12, _store.GetAll().Count);
    }

    [Fact]
    public void InitializeStore_Contains4Clients()
    {
        var clients = _store.GetByClassName("Client");
        Assert.Equal(4, clients.Count);
        Assert.All(clients, obj => Assert.IsType<Client>(obj));
    }

    [Fact]
    public void InitializeStore_Contains4Services()
    {
        var services = _store.GetByClassName("Service");
        Assert.Equal(4, services.Count);
        Assert.All(services, obj => Assert.IsType<Service>(obj));
    }

    [Fact]
    public void InitializeStore_Contains4Appointments()
    {
        var appointments = _store.GetByClassName("Appointment");
        Assert.Equal(4, appointments.Count);
        Assert.All(appointments, obj => Assert.IsType<Appointment>(obj));
    }

    [Fact]
    public void GetObjectsByClass_UnknownClass_ReturnsFallbackObjects()
    {
        var result = _store.GetByClassName("Pet");
        Assert.NotEmpty(result);
        Assert.All(result, obj => Assert.IsType<Service>(obj));
    }

    [Fact]
    public void KeyFormat_IsCorrect()
    {
        var all = _store.GetAll();
        Assert.True(all.ContainsKey("client_1"));
        Assert.True(all.ContainsKey("service_2"));
        Assert.True(all.ContainsKey("appointment_3"));
    }
}
