namespace api.DTOs;

public class AssignmentImagesDTO
{
    public AssignmentResponseDTO Assignment { get; set; } = default!;
    public List<string> ImageUrls { get; set; } = new();
}
