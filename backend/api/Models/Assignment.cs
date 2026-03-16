using System.ComponentModel.DataAnnotations;

namespace api.Models;

public class Assignment
{
    [Key]
    public Guid Id { get; init; }
    public required string Answer { get; set; }
    public required string Topic { get; set; }
    public required string Subject { get; set; }
    public int Points { get; set; }
    public required string Level { get; set; }
    public int Subtest { get; set; }
    public int Number { get; set; }
    public string Subquestion { get; set; } = "a";
}
