using api.Models;

namespace api.DTOs;

// Entity -> wire DTO projection. Kept as extensions so controllers can write
// `entity.ToResponse()` without pulling in a mapping service.
public static class ResponseMapping
{
    public static AssignmentResponseDTO ToResponse(this Assignment a)
        => new(
            a.Id, a.Answer, a.Education, a.Subject, a.Level, a.Year, a.Date,
            a.Subquestion, a.Subtest, a.Topic, a.Owner, a.Number, a.Points,
            a.Tags ?? new List<string>(), a.AssignmentSheetId);

    public static AssignmentSheetResponseDTO ToResponse(this AssignmentSheet s)
        => new(s.Id, s.Title, s.Subject, s.Level, s.Year, s.Owner, s.Type);

    public static StudentResponseDTO ToResponse(this Student s)
        => new(
            s.Id, s.FirstName, s.LastName, s.Email ?? "",
            s.Phone, s.Address, s.City, s.CustomerNumber, s.CustomerId,
            s.Note, s.StudentClass, s.BirthDate, s.Enrolled, s.Pause, s.Inactive);
}
