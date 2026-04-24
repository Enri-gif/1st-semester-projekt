namespace api.DTOs;

public class TopicStat
{
    public string Topic { get; set; } = "";
    public int Count { get; set; }
    public int TotalPoints { get; set; }
    public double AveragePoints { get; set; }
}
