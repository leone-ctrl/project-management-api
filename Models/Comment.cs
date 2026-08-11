using System;
using System.Collections.Generic;

namespace ProjectmanagementAPI.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public int TaskId { get; set; }

    public int UserId { get; set; }

    public string Comment1 { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Task Task { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
