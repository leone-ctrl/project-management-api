using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectmanagementAPI.Models;
using TaskModel = ProjectmanagementAPI.Models.Task;

namespace ProjectmanagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ProjectManagementContext _context;

        public TasksController(ProjectManagementContext context)
        {
            _context = context;
        }

        // GET: api/Tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskModel>>> GetTasks()
        {
            return await _context.Tasks.AsNoTracking().ToListAsync();
        }

        // GET: api/Tasks/details/5
        [HttpGet("details/{id}")]
        public async Task<ActionResult<TaskModel>> GetTaskDetails(int id)
        {
            var task = await _context.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TaskId == id);

            if (task == null)
            {
                return NotFound(new { Message = $"Task with ID {id} not found." });
            }

            return Ok(task);
        }

        // POST: api/Tasks
        [HttpPost]
        public async Task<ActionResult<TaskModel>> CreateTask(TaskModel task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaskDetails), new { id = task.TaskId }, task);
        }

        // PUT: api/Tasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskModel task)
        {
            if (id != task.TaskId)
            {
                return BadRequest(new { Message = "Task ID mismatch." });
            }

            _context.Entry(task).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                {
                    return NotFound(new { Message = $"Task with ID {id} not found." });
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound(new { Message = $"Task with ID {id} not found." });
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Task with ID {id} successfully deleted." });
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.TaskId == id);
        }
    }
}