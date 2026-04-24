namespace api.DTOs;

public class StatisticsDTO
{
    public List<SubjectStat> BySubject { get; set; } = new();
    public List<TopicStat> ByTopic { get; set; } = new();
}
