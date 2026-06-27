using System.Text.Json;

namespace Domain.Entities
{
    public class JobExecution : BaseEntity
    {
        public string JobName { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? DurationMs { get; set; }
        public string Status { get; set; } = null!;
        public bool HasError { get; set; }
        public string? ErrorMessage { get; set; }
        public string? MetricsJson { get; set; }
    }
}
