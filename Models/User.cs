using System;
using System.Collections.Generic;

namespace ProjectmanagementAPI.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Role { get; set; }

    public string Password { get; set; } = null!; // <-- Added Password Property

    public bool IsActive { get; set; } 

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}