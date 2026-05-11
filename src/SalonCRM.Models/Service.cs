namespace SalonCRM.Models;

public class Service
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }

    public Service() { }

    public Service(string name, string description, int durationMinutes, decimal price)
    {
        Name = name;
        Description = description;
        DurationMinutes = durationMinutes;
        Price = price;
    }

    public override string ToString() =>
        $"Service {{ {Name}, {DurationMinutes} min, {Price} PLN }}";

    public override bool Equals(object? obj) =>
        obj is Service other && Name == other.Name;

    public override int GetHashCode() => Name.GetHashCode();
}
