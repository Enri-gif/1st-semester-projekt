using api.Models;

namespace api.DTOs;

// Response DTOs project entity data onto the wire contract so that:
//   - EF navigation properties can't accidentally serialize (lazy-load or infinite loops)
//   - Concurrency tokens (RowVersion) don't leak to clients
//   - Schema changes to entities don't silently change the API response shape

public record AssignmentResponseDto(
    Guid Id,
    string Answer,
    string Education,
    string Subject,
    string Level,
    int Year,
    DateTime Date,
    string Subquestion,
    int Subtest,
    string Topic,
    string Owner,
    int Number,
    int Points,
    List<string> Tags,
    Guid? AssignmentSheetId);

public record AssignmentSheetResponseDto(
    Guid Id,
    string Title,
    string Subject,
    string Level,
    int Year,
    string Owner);

public record StudentResponseDto(
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

public static class ResponseMapping
{
    public static AssignmentResponseDto ToResponse(this Assignment a)
        => new(
            a.Id, a.Answer, a.Education, a.Subject, a.Level, a.Year, a.Date,
            a.Subquestion, a.Subtest, a.Topic, a.Owner, a.Number, a.Points,
            a.Tags ?? new List<string>(), a.AssignmentSheetId);

    public static AssignmentSheetResponseDto ToResponse(this AssignmentSheet s)
        => new(s.Id, s.Title, s.Subject, s.Level, s.Year, s.Owner);

    public static StudentResponseDto ToResponse(this Student s)
        => new(
            s.Id, s.FirstName, s.LastName, s.Email ?? "",
            s.Phone, s.Address, s.City, s.CustomerNumber, s.CustomerId,
            s.Note, s.StudentClass, s.BirthDate, s.Enrolled, s.Pause, s.Inactive);
}
