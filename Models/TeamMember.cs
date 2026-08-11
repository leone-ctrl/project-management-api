using System;
using System.Collections.Generic;

namespace ProjectmanagementAPI.Models;

public partial class TeamMember
{
    public int TeamMemberId { get; set; }

    public int TeamId { get; set; }

    public int UserId { get; set; }

    public virtual Team Team { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public bool IsLeader { get; set; }

}
