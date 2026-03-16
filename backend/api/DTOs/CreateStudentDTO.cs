using System.ComponentModel.DataAnnotations;

namespace api.DTOs;

public class CreateStudentDTO
{
    [Required]
    public string FirstName { get; set; } = default!;

    [Required]
    public string LastName { get; set; } = default!;
}
