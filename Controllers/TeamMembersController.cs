using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectmanagementAPI.Data;
using ProjectmanagementAPI.Dtos;
using ProjectmanagementAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectmanagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamMembersController : ControllerBase
    {
        private readonly ProjectManagementContext _context;

        public TeamMembersController(ProjectManagementContext context)
        {
            _context = context;
        }

        // GET: api/TeamMembers (Retrieves raw team member rows matching the database table)
        [HttpGet]
        public async Task<IActionResult> GetTeamMembers()
        {
            var members = await _context.TeamMembers
                .Select(tm => new
                {
                    TeamMemberId = tm.TeamMemberId,
                    TeamId = tm.TeamId,
                    UserId = tm.UserId,
                    IsLeader = tm.IsLeader
                })
                .ToListAsync();

            return Ok(members);
        }

        // POST: api/TeamMembers/assign
        [HttpPost("assign")]
        public async Task<IActionResult> AssignUserToTeam([FromBody] TeamAssignmentDto dto)
        {
            var existing = await _context.TeamMembers
                .FirstOrDefaultAsync(tm => tm.UserId == dto.UserId && tm.TeamId == dto.TeamId);

            if (existing != null) return BadRequest(new { message = "User is already in this team." });

            _context.TeamMembers.Add(new TeamMember
            {
                UserId = dto.UserId,
                TeamId = dto.TeamId,
                IsLeader = dto.IsLeader
            });
            await _context.SaveChangesAsync();
            return Ok(new { message = "User assigned to team successfully." });
        }

        // DELETE: api/TeamMembers/remove
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveUserFromTeam([FromQuery] int userId, [FromQuery] int teamId)
        {
            var membership = await _context.TeamMembers
                .FirstOrDefaultAsync(tm => tm.UserId == userId && tm.TeamId == teamId);

            if (membership == null) return NotFound(new { message = "Team membership not found." });

            _context.TeamMembers.Remove(membership);
            await _context.SaveChangesAsync();
            return Ok(new { message = "User removed from team successfully." });
        }

        // PUT: api/TeamMembers/move
        [HttpPut("move")]
        public async Task<IActionResult> MoveUserBetweenTeams([FromBody] MoveUserDto dto)
        {
            var membership = await _context.TeamMembers
                .FirstOrDefaultAsync(tm => tm.UserId == dto.UserId && tm.TeamId == dto.SourceTeamId);

            if (membership == null) return NotFound(new { message = "Source team membership not found." });

            membership.TeamId = dto.TargetTeamId;
            await _context.SaveChangesAsync();
            return Ok(new { message = "User moved successfully between teams." });
        }

        // PUT: api/TeamMembers/role
        [HttpPut("role")]
        public async Task<IActionResult> AssignTeamRole([FromBody] UpdateRoleDto dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null) return NotFound(new { message = "User not found." });

            user.Role = dto.Role;
            await _context.SaveChangesAsync();
            return Ok(new { message = "User role updated successfully." });
        }

        // PUT: api/TeamMembers/leader
        [HttpPut("leader")]
        public async Task<IActionResult> AssignTeamLeader([FromBody] TeamLeaderDto dto)
        {
            var membership = await _context.TeamMembers
                .FirstOrDefaultAsync(tm => tm.UserId == dto.UserId && tm.TeamId == dto.TeamId);

            if (membership == null) return NotFound(new { message = "Team membership not found." });

            membership.IsLeader = dto.IsLeader;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Team leader status updated successfully." });
        }

        // PUT: api/TeamMembers/status
        [HttpPut("status")]
        public async Task<IActionResult> ToggleStatus([FromBody] ToggleStatusDto dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null) return NotFound(new { message = "User not found." });

            user.IsActive = dto.IsActive;
            await _context.SaveChangesAsync();
            return Ok(new { message = "User membership status updated successfully." });
        }
    }
}