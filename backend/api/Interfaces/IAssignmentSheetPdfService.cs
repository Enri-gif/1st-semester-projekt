using api.Models;

namespace api.Interfaces;

public interface IAssignmentSheetPdfService
{
    byte[] GenerateAssignmentSheetPdf(AssignmentSheet sheet);
}
