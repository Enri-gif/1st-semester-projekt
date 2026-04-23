using api.Models;

namespace api.Services;

public interface IAssignmentSheetPdfService
{
    byte[] GenerateAssignmentSheetPdf(AssignmentSheet sheet);
}
