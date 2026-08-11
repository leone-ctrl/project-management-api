using System;
using System.Collections.Generic;

namespace ProjectmanagementAPI.Models;

public partial class Task
{
    public int TaskId { get; set; }

    public string TaskName { get; set; } = null!;

    public string? Description { get; set; }

    public int ProjectId { get; set; }

    public int? AssignedTo { get; set; }

    public string? Status { get; set; }

    public DateTime? DueDate { get; set; }

    public virtual User? AssignedToNavigation { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Project Project { get; set; } = null!;
}
