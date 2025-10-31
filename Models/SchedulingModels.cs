using System.Text.Json.Serialization;


public class ScheduleRequest
{
    [JsonPropertyName("tasks")]
    public List<TaskItem> Tasks { get; set; }
}

public class TaskItem
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("estimatedHours")]
    public int EstimatedHours { get; set; }

    [JsonPropertyName("dueDate")]
    public string DueDate { get; set; }

    [JsonPropertyName("dependencies")]
    public List<string> Dependencies { get; set; }
}


public class ScheduleResponse
{
    [JsonPropertyName("recommendedOrder")]
    public List<string> RecommendedOrder { get; set; }
}