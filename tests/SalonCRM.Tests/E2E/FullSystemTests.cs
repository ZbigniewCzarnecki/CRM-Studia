using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using SalonCRM.Models;
using SalonCRM.Server;

namespace SalonCRM.Tests.E2E;

public class FullSystemTests : IDisposable
{
    private readonly TcpListener _listener;
    private readonly int _port;
    private readonly Thread _serverThread;
    private readonly SemaphoreSlim _semaphore = new(SalonServer.MaxClients, SalonServer.MaxClients);
    private readonly DataStore _dataStore = new();
    private volatile bool _running = true;
    private int _clientIdCounter = 0;

    public FullSystemTests()
    {
        _listener = new TcpListener(IPAddress.Loopback, 0);
        _listener.Start();
        _port = ((IPEndPoint)_listener.LocalEndpoint).Port;

        _serverThread = new Thread(RunServer) { IsBackground = true };
        _serverThread.Start();
    }

    private void RunServer()
    {
        while (_running)
        {
            try
            {
                var client = _listener.AcceptTcpClient();
                var id = Interlocked.Increment(ref _clientIdCounter);
                var handler = new ClientHandler(client, _dataStore, _semaphore, id);
                new Thread(handler.Handle) { IsBackground = true }.Start();
            }
            catch (SocketException) { }
            catch (ObjectDisposedException) { break; }
        }
    }

    private async Task<(string response, List<string> data)> RunTestClientAsync(int clientId, string[] classRequests)
    {
        using var tcp = new TcpClient("localhost", _port);
        using var stream = tcp.GetStream();
        stream.ReadTimeout = 10000;
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream) { AutoFlush = true };

        await writer.WriteLineAsync($"CONNECT:{clientId}");
        var connectResponse = (await reader.ReadLineAsync())!;

        var dataLines = new List<string>();
        if (connectResponse == "OK")
        {
            foreach (var cls in classRequests)
            {
                await writer.WriteLineAsync($"GET:{cls}");
                dataLines.Add((await reader.ReadLineAsync())!);
            }
            await writer.WriteLineAsync("EXIT");
        }

        return (connectResponse, dataLines);
    }

    [Fact]
    public async Task StartServer_ConnectNClients_AllGetOK()
    {
        var tasks = Enumerable.Range(1, SalonServer.MaxClients)
            .Select(i => RunTestClientAsync(i, ["Client"]))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        Assert.All(results, r => Assert.Equal("OK", r.response));
    }

    [Fact]
    public async Task StartServer_ExceedMaxClients_ExtraClientGetRefused()
    {
        // Otwieramy dokładnie MaxClients połączeń i trzymamy je aktywne (brak EXIT)
        var holding = new List<(TcpClient tcp, StreamReader reader, StreamWriter writer)>();

        for (int i = 0; i < SalonServer.MaxClients; i++)
        {
            var tcp = new TcpClient("localhost", _port);
            var stream = tcp.GetStream();
            stream.ReadTimeout = 5000;
            var r = new StreamReader(stream);
            var w = new StreamWriter(stream) { AutoFlush = true };
            await w.WriteLineAsync($"CONNECT:{i}");
            var resp = await r.ReadLineAsync();
            Assert.Equal("OK", resp);
            holding.Add((tcp, r, w));
        }

        // Próba otwarcia MaxClients+1 połączenia — musi dostać REFUSED
        using var extraTcp = new TcpClient("localhost", _port);
        using var extraStream = extraTcp.GetStream();
        extraStream.ReadTimeout = 5000;
        using var extraReader = new StreamReader(extraStream);
        using var extraWriter = new StreamWriter(extraStream) { AutoFlush = true };
        await extraWriter.WriteLineAsync("CONNECT:99");
        var extraResp = await extraReader.ReadLineAsync();
        Assert.Equal("REFUSED", extraResp);

        // Zamykamy trzymane połączenia
        foreach (var (tcp, _, w) in holding)
        {
            await w.WriteLineAsync("EXIT");
            tcp.Close();
        }
    }

    [Fact]
    public async Task Client_RequestKnownClass_ReceivesCorrectServiceData()
    {
        var (resp, dataLines) = await RunTestClientAsync(200, ["Service"]);

        Assert.Equal("OK", resp);
        Assert.Single(dataLines);
        Assert.StartsWith("DATA:", dataLines[0]);

        var json = dataLines[0]["DATA:".Length..];
        var services = JsonSerializer.Deserialize<List<Service>>(json)!;
        Assert.Equal(4, services.Count);
    }

    [Fact]
    public async Task Client_RequestUnknownClass_ReceivesServiceDataInsteadOfPet()
    {
        var (resp, dataLines) = await RunTestClientAsync(300, ["Pet"]);

        Assert.Equal("OK", resp);
        Assert.Single(dataLines);
        Assert.StartsWith("DATA:", dataLines[0]);

        var json = dataLines[0]["DATA:".Length..];

        // Serwer zwraca obiekty Service zamiast Pet — deserializacja jako Pet rzuci JsonException
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<List<PetE2E>>(json));

        var services = JsonSerializer.Deserialize<List<Service>>(json)!;
        Assert.NotEmpty(services);
    }

    [Fact]
    public async Task Client_RequestAllKnownClasses_ReceivesCorrectData()
    {
        var (resp, dataLines) = await RunTestClientAsync(400, ["Client", "Service", "Appointment"]);

        Assert.Equal("OK", resp);
        Assert.Equal(3, dataLines.Count);

        var clients = JsonSerializer.Deserialize<List<Client>>(dataLines[0]["DATA:".Length..])!;
        var services = JsonSerializer.Deserialize<List<Service>>(dataLines[1]["DATA:".Length..])!;
        var appointments = JsonSerializer.Deserialize<List<Appointment>>(dataLines[2]["DATA:".Length..])!;

        Assert.Equal(4, clients.Count);
        Assert.Equal(4, services.Count);
        Assert.Equal(4, appointments.Count);
    }

    public void Dispose()
    {
        _running = false;
        _listener.Stop();
    }

    private class PetE2E
    {
        [System.Text.Json.Serialization.JsonRequired]
        public string PetName { get; set; } = string.Empty;
    }
}
