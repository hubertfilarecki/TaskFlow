using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskFlow.Models
{
    public class WorkTask
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [Required]
        [JsonPropertyName("status")]
        public WorkTaskStatus Status { get; set; } = WorkTaskStatus.Todo;

        [Required]
        [JsonPropertyName("priority")]
        public WorkTaskPriority Priority { get; set; } = WorkTaskPriority.Medium;

        [StringLength(100)]
        [JsonPropertyName("assignee")]
        public string? Assignee { get; set; }

        [Range(0, int.MaxValue)]
        [JsonPropertyName("timeSpentMinutes")]
        public int TimeSpentMinutes { get; set; }

        [DataType(DataType.Date)]
        [JsonPropertyName("dueDate")]
        public DateTime? DueDate { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }
}
