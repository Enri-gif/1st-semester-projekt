using api.Models;

namespace api.DTOs;

public record AssignmentSheetResponseDTO(
    Guid Id,
    string Title,
    string Subject,
    string Level,
    int Year,
    string Owner,
    AssignmentSheetType Type,
    string Grade,
    string Feedback,
    string CorrectionNotes);
