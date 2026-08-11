using System;

namespace ProjectmanagementAPI.Models
{
    public class TaskItem
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public int AssignedTo { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
    }
}