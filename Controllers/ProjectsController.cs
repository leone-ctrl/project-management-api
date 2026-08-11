using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectmanagementAPI.Models;

namespace ProjectmanagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectManagementContext _context;

        public ProjectsController(ProjectManagementContext context)
        {
            _context = context;
        }

        // 1. GET: api/Projects (Read All)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _context.Projects.AsNoTracking().ToListAsync();
        }

        // 2. GET: api/Projects/details/5 (Read One)
        [HttpGet("details/{id}")]
        public async Task<ActionResult<Project>> GetProjectDetails(int id)
        {
            var projectDetails = await _context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProjectId == id);

            if (projectDetails == null)
            {
                return NotFound(new { Message = $"Project with ID {id} not found." });
            }

            return Ok(projectDetails);
        }

        // 3. POST: api/Projects (Create)
        [HttpPost]
        public async Task<ActionResult<Project>> CreateProject(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            // Returns a 201 Created status and automatically links to the details page of the new project
            return CreatedAtAction(nameof(GetProjectDetails), new { id = project.ProjectId }, project);
        }

        // 4. PUT: api/Projects/5 (Update)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, Project project)
        {
            if (id != project.ProjectId)
            {
                return BadRequest(new { Message = "Project ID mismatch." });
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                {
                    return NotFound(new { Message = $"Project with ID {id} not found." });
                }
                throw;
            }

            return NoContent(); // 204 No Content (Standard successful update response)
        }

        // 5. DELETE: api/Projects/5 (Delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound(new { Message = $"Project with ID {id} not found." });
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Project with ID {id} successfully deleted." });
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }
    }
}