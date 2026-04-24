using System.ComponentModel.DataAnnotations;

namespace api.Models;

public class Student
{
    [Key]
    public Guid Id { get; set; }

    public string? CustomerNumber { get; set; }
    public string? CustomerId { get; set; }

    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;

    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Note { get; set; }
    public string? StudentClass { get; set; }

    public DateTime? BirthDate { get; set; }
    public DateTime? Enrolled { get; set; }
    public DateTime? Inactive { get; set; }
    public DateTime? Pause { get; set; }

    [Timestamp]
    public byte[]? RowVersion { get; set; }
}
