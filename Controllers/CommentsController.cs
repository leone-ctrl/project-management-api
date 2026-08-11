using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectmanagementAPI.Models;

namespace ProjectmanagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ProjectManagementContext _context;

        public CommentsController(ProjectManagementContext context)
        {
            _context = context;
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<IActionResult> GetComments()
        {
            var comments = await _context.Comments
                .AsNoTracking()
                .Select(c => new
                {
                    CommentId = c.CommentId,
                    TaskId = c.TaskId,
                    UserId = c.UserId,
                    CommentText = c.Comment1, // Fixed: uses Comment1 instead of Comment
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(comments);
        }

        // GET: api/Comments/details/5
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetCommentDetails(int id)
        {
            var comment = await _context.Comments
                .AsNoTracking()
                .Where(c => c.CommentId == id)
                .Select(c => new
                {
                    CommentId = c.CommentId,
                    TaskId = c.TaskId,
                    UserId = c.UserId,
                    CommentText = c.Comment1,
                    CreatedAt = c.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (comment == null)
            {
                return NotFound(new { Message = $"Comment with ID {id} not found." });
            }

            return Ok(comment);
        }

        // POST: api/Comments
        [HttpPost]
        public async Task<ActionResult<Comment>> CreateComment(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCommentDetails), new { id = comment.CommentId }, comment);
        }

        // PUT: api/Comments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, Comment comment)
        {
            if (id != comment.CommentId)
            {
                return BadRequest(new { Message = "Comment ID mismatch." });
            }

            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                {
                    return NotFound(new { Message = $"Comment with ID {id} not found." });
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound(new { Message = $"Comment with ID {id} not found." });
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Comment with ID {id} successfully deleted." });
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.CommentId == id);
        }
    }
}