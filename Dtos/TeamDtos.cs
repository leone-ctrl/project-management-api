using System.Collections.Generic;

namespace ProjectmanagementAPI.Dtos
{
    public class UserTeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<TeamDto> Teams { get; set; } = new();
    }

    public class TeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsLeader { get; set; }
    }

    public class TeamAssignmentDto
    {
        public int UserId { get; set; }
        public int TeamId { get; set; }
        public bool IsLeader { get; set; }
    }

    public class MoveUserDto
    {
        public int UserId { get; set; }
        public int SourceTeamId { get; set; }
        public int TargetTeamId { get; set; }
    }

    public class UpdateRoleDto
    {
        public int UserId { get; set; }
        public string Role { get; set; } = string.Empty;
    }

    public class TeamLeaderDto
    {
        public int UserId { get; set; }
        public int TeamId { get; set; }
        public bool IsLeader { get; set; }
    }

    public class ToggleStatusDto
    {
        public int UserId { get; set; }
        public bool IsActive { get; set; }
    }
}