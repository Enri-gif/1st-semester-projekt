namespace api.DTOs;

// Response DTO: decouples the wire contract from the EF entity so that
// navigation properties, concurrency tokens and internal fields don't leak
// onto the API.
public record AssignmentResponseDTO(
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
