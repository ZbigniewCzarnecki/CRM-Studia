namespace SalonCRM.Models;

public class Client
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public Client() { }

    public Client(string firstName, string lastName, string phoneNumber, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Email = email;
    }

    public override string ToString() =>
        $"Client {{ {FirstName}, {LastName}, {PhoneNumber}, {Email} }}";

    public override bool Equals(object? obj) =>
        obj is Client other && Email == other.Email;

    public override int GetHashCode() => Email.GetHashCode();
}
