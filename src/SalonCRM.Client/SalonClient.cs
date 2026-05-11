using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using SalonCRM.Models;
using ClientModel = SalonCRM.Models.Client;
using ServiceModel = SalonCRM.Models.Service;
using AppointmentModel = SalonCRM.Models.Appointment;

namespace SalonCRM.Client;

public class SalonClient
{
    private readonly string _host;
    private readonly int _port;
    private readonly int _id;

    public SalonClient(int id, string host = "localhost", int port = 5000)
    {
        _id = id;
        _host = host;
        _port = port;
    }

    public void Run()
    {
        using var tcpClient = new TcpClient(_host, _port);
        using var stream = tcpClient.GetStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream) { AutoFlush = true };

        writer.WriteLine($"CONNECT:{_id}");
        var response = reader.ReadLine();

        if (response == "REFUSED")
        {
            Console.WriteLine($"[Klient #{_id}] Połączenie odrzucone przez serwer (limit klientów)");
            return;
        }

        Console.WriteLine($"[Klient #{_id}] Połączono z serwerem");

        var requests = new[] { "Client", "Service", "Appointment", "Pet" };

        foreach (var className in requests)
        {
            writer.WriteLine($"GET:{className}");
            var dataLine = reader.ReadLine();

            if (dataLine == null || !dataLine.StartsWith("DATA:"))
            {
                Console.WriteLine($"[Klient #{_id}] Nieprawidłowa odpowiedź serwera");
                continue;
            }

            var json = dataLine["DATA:".Length..];
            ProcessResponse(className, json);
        }

        writer.WriteLine("EXIT");
        Console.WriteLine($"[Klient #{_id}] Zakończono sesję");
    }

    private void ProcessResponse(string requestedClass, string json)
    {
        try
        {
            switch (requestedClass)
            {
                case "Client":
                    var clients = JsonSerializer.Deserialize<List<ClientModel>>(json)!;
                    Console.WriteLine($"[Klient #{_id}] Otrzymano {clients.Count} obiektów typu Client");
                    var filtered = clients.Where(c => c.LastName.StartsWith("K")).ToList();
                    Console.WriteLine($"[Klient #{_id}] Klienci z nazwiskiem na 'K': {filtered.Count}");
                    foreach (var c in filtered)
                        Console.WriteLine($"  {c}");
                    break;

                case "Service":
                    var services = JsonSerializer.Deserialize<List<ServiceModel>>(json)!;
                    Console.WriteLine($"[Klient #{_id}] Otrzymano {services.Count} obiektów typu Service");
                    var sorted = services.OrderByDescending(s => s.Price).ToList();
                    Console.WriteLine($"[Klient #{_id}] Usługi wg ceny (malejąco):");
                    foreach (var s in sorted)
                        Console.WriteLine($"  {s}");
                    break;

                case "Appointment":
                    var appointments = JsonSerializer.Deserialize<List<AppointmentModel>>(json)!;
                    Console.WriteLine($"[Klient #{_id}] Otrzymano {appointments.Count} obiektów typu Appointment");
                    var upcoming = appointments.Where(a => a.Date > DateTime.Now).Count();
                    Console.WriteLine($"[Klient #{_id}] Nadchodzące wizyty: {upcoming}");
                    break;

                default:
                    // Serwer zwraca obiekty Service dla nieznanych klas.
                    // JsonRequired na polu PetName powoduje rzucenie JsonException — pole wymagane nie istnieje w JSON Service.
                    var pets = JsonSerializer.Deserialize<List<Pet>>(json)!;
                    Console.WriteLine($"[Klient #{_id}] Otrzymano {pets.Count} obiektów typu {requestedClass}");
                    break;
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[Klient #{_id}] Błąd rzutowania: oczekiwano {requestedClass}, otrzymano inny typ ({ex.Message})");
        }
    }

    // [JsonRequired] na PetName gwarantuje JsonException gdy JSON zawiera obiekty Service (brak pola PetName)
    private class Pet
    {
        [JsonRequired]
        public string PetName { get; set; } = string.Empty;
        public string? Species { get; set; }
    }
}
