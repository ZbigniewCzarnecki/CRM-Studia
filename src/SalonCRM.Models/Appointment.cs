namespace SalonCRM.Models;

public class Appointment
{
    public string ClientName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal TotalAmount { get; set; }

    public Appointment() { }

    public Appointment(string clientName, string serviceName, DateTime date, decimal totalAmount)
    {
        ClientName = clientName;
        ServiceName = serviceName;
        Date = date;
        TotalAmount = totalAmount;
    }

    public override string ToString() =>
        $"Appointment {{ {ClientName}, {ServiceName}, {Date:yyyy-MM-dd HH:mm}, {TotalAmount} PLN }}";

    public override bool Equals(object? obj) =>
        obj is Appointment other && ClientName == other.ClientName && Date == other.Date;

    public override int GetHashCode() => HashCode.Combine(ClientName, Date);
}
