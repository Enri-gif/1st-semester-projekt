namespace api.DTOs;

public record StudentResponseDTO(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? Address,
    string? City,
    string? CustomerNumber,
    string? CustomerId,
    string? Note,
    string? StudentClass,
    DateTime? BirthDate,
    DateTime? Enrolled,
    DateTime? Pause,
    DateTime? Inactive);
