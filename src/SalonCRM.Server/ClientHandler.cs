using System.Net.Sockets;
using System.Text.Json;
using SalonCRM.Models;

namespace SalonCRM.Server;

public class ClientHandler
{
    private readonly TcpClient _tcpClient;
    private readonly DataStore _dataStore;
    private readonly SemaphoreSlim _semaphore;
    private readonly int _clientId;
    private readonly Random _random = new();

    public ClientHandler(TcpClient tcpClient, DataStore dataStore, SemaphoreSlim semaphore, int clientId)
    {
        _tcpClient = tcpClient;
        _dataStore = dataStore;
        _semaphore = semaphore;
        _clientId = clientId;
    }

    public void Handle()
    {
        using var stream = _tcpClient.GetStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream) { AutoFlush = true };

        try
        {
            var connectLine = reader.ReadLine();
            if (connectLine == null || !connectLine.StartsWith("CONNECT:"))
            {
                writer.WriteLine("REFUSED");
                return;
            }

            if (!_semaphore.Wait(0))
            {
                writer.WriteLine("REFUSED");
                Console.WriteLine($"[Serwer] Klient #{_clientId} odrzucony — przekroczono limit połączeń");
                return;
            }

            try
            {
                writer.WriteLine("OK");
                Console.WriteLine($"[Serwer] Klient #{_clientId} połączony");

                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line == "EXIT")
                        break;

                    if (line.StartsWith("GET:"))
                    {
                        var className = line["GET:".Length..];
                        var objects = _dataStore.GetByClassName(className);

                        var json = className switch
                        {
                            "Client" => JsonSerializer.Serialize(objects.Cast<Client>().ToList()),
                            "Service" => JsonSerializer.Serialize(objects.Cast<Service>().ToList()),
                            "Appointment" => JsonSerializer.Serialize(objects.Cast<Appointment>().ToList()),
                            _ => JsonSerializer.Serialize(objects.Cast<Service>().ToList())
                        };

                        Thread.Sleep(_random.Next(500, 2001));
                        writer.WriteLine($"DATA:{json}");
                        Console.WriteLine($"[Serwer] Wysłano {objects.Count}x {className} do klienta #{_clientId}");
                    }
                }
            }
            finally
            {
                _semaphore.Release();
                Console.WriteLine($"[Serwer] Klient #{_clientId} rozłączony");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Serwer] Błąd obsługi klienta #{_clientId}: {ex.Message}");
        }
        finally
        {
            _tcpClient.Close();
        }
    }
}
