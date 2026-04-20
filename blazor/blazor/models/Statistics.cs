namespace blazor.models;

public class StatisticsDto
{
    public List<SubjectStat> BySubject { get; set; } = new();
    public List<TopicStat> ByTopic { get; set; } = new();
}

public class SubjectStat
{
    public string Subject { get; set; } = "";
    public int Count { get; set; }
    public int TotalPoints { get; set; }
    public double AveragePoints { get; set; }
}

public class TopicStat
{
    public string Topic { get; set; } = "";
    public int Count { get; set; }
    public int TotalPoints { get; set; }
    public double AveragePoints { get; set; }
}
