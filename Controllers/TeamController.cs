using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectmanagementAPI.Models;

namespace ProjectmanagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ProjectManagementContext _context;

        public TeamController(ProjectManagementContext context)
        {
            _context = context;
        }

        // GET: api/Team
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            return await _context.Teams.AsNoTracking().ToListAsync();
        }

        // GET: api/Team/details/5
        [HttpGet("details/{id}")]
        public async Task<ActionResult<Team>> GetTeamDetails(int id)
        {
            var team = await _context.Teams
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TeamId == id);

            if (team == null)
            {
                return NotFound(new { Message = $"Team with ID {id} not found." });
            }

            return Ok(team);
        }

        // POST: api/Team
        [HttpPost]
        public async Task<ActionResult<Team>> CreateTeam(Team team)
        {
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeamDetails), new { id = team.TeamId }, team);
        }

        // PUT: api/Team/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(int id, Team team)
        {
            if (id != team.TeamId)
            {
                return BadRequest(new { Message = "Team ID mismatch." });
            }

            _context.Entry(team).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamExists(id))
                {
                    return NotFound(new { Message = $"Team with ID {id} not found." });
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Team/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound(new { Message = $"Team with ID {id} not found." });
            }

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Team with ID {id} successfully deleted." });
        }

        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.TeamId == id);
        }
    }
}