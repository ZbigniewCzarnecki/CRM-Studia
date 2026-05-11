using SalonCRM.Models;

namespace SalonCRM.Server;

public class DataStore
{
    private readonly Dictionary<string, object> _store = new();

    public DataStore()
    {
        Initialize();
    }

    private void Initialize()
    {
        _store["client_1"] = new Client("Anna", "Kowalska", "500-100-200", "anna@email.pl");
        _store["client_2"] = new Client("Maria", "Nowak", "600-300-400", "maria@email.pl");
        _store["client_3"] = new Client("Katarzyna", "Wiśniewska", "700-500-600", "kasia@email.pl");
        _store["client_4"] = new Client("Joanna", "Zielińska", "800-700-800", "joanna@email.pl");

        _store["service_1"] = new Service("Manicure hybrydowy", "Lakier hybrydowy UV/LED", 60, 120m);
        _store["service_2"] = new Service("Pedicure klasyczny", "Pielęgnacja stóp i paznokci", 45, 90m);
        _store["service_3"] = new Service("Mikrodermabrazja", "Peeling diamentowy twarzy", 30, 150m);
        _store["service_4"] = new Service("Depilacja laserowa", "Depilacja laserem diodowym", 40, 200m);

        _store["appointment_1"] = new Appointment("Anna Kowalska", "Manicure hybrydowy", new DateTime(2026, 6, 15, 10, 0, 0), 120m);
        _store["appointment_2"] = new Appointment("Maria Nowak", "Pedicure klasyczny", new DateTime(2026, 6, 15, 12, 0, 0), 90m);
        _store["appointment_3"] = new Appointment("Katarzyna Wiśniewska", "Mikrodermabrazja", new DateTime(2026, 6, 16, 9, 0, 0), 150m);
        _store["appointment_4"] = new Appointment("Joanna Zielińska", "Depilacja laserowa", new DateTime(2026, 6, 16, 14, 0, 0), 200m);
    }

    public Dictionary<string, object> GetAll() => _store;

    public List<object> GetByClassName(string className)
    {
        return className switch
        {
            "Client" => _store.Values.OfType<Client>().Cast<object>().ToList(),
            "Service" => _store.Values.OfType<Service>().Cast<object>().ToList(),
            "Appointment" => _store.Values.OfType<Appointment>().Cast<object>().ToList(),
            // Celowo zwraca Service dla nieznanych klas — prowokuje błąd deserializacji po stronie klienta
            _ => _store.Values.OfType<Service>().Cast<object>().ToList()
        };
    }
}
