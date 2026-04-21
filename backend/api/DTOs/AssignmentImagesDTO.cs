namespace api.DTOs;

public class AssignmentImagesDTO
{
    public AssignmentResponseDto Assignment { get; set; } = default!;
    public List<string> ImageUrls { get; set; } = new();
}
