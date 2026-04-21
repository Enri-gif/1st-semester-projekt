using api.Models;

namespace api.DTOs;

public class AssignmentImagesDTO{
    public Assignment Assignment { get; set; }
    public List<string> ImageUrls { get; set; }
}
