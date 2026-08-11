using System;
using System.Collections.Generic;

namespace ProjectmanagementAPI.Models;

public partial class Project
{
    public int ProjectId { get; set; }

    public string ProjectName { get; set; } = null!;

    public string? Description { get; set; }

    public int? OwnerId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Status { get; set; }

    public virtual User? Owner { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
