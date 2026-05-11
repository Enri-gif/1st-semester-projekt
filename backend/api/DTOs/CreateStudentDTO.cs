using System.ComponentModel.DataAnnotations;

namespace api.DTOs;

public class CreateStudentDTO
{
    [Required]
    public string FirstName { get; set; } = default!;

    [Required]
    public string LastName { get; set; } = default!;

    [EmailAddress]
    public string? Email { get; set; }

    public string? CustomerNumber { get; set; }
    public string? CustomerId { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Note { get; set; }
    public string? StudentClass { get; set; }

    public DateTime? BirthDate { get; set; }
    public DateTime? Enrolled { get; set; }
    public DateTime? Inactive { get; set; }
    public DateTime? Pause { get; set; }
}
