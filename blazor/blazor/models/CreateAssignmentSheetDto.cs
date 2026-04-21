namespace blazor.models;

public class CreateAssignmentSheetDto
{
    public string Title { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Level { get; set; } = "";
    public int Year { get; set; } = System.DateTime.Today.Year;
    public string Owner { get; set; } = "";
}
