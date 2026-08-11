using System;
using System.Collections.Generic;

namespace ProjectmanagementAPI.Models;

public partial class Team
{
    public int TeamId { get; set; }

    public string TeamName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}
