using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskBoard.API.Dtos;
using TaskBoard.Core.Data;
using TaskBoard.Core.Models;

namespace TaskBoard.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly TaskBoardDbContext _db;
        private readonly IMapper _mapper;
        public TasksController(TaskBoardDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        /// <summary>List tasks (paged). Most recent first.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> Get(int skip = 0, int take = 50)
        {
            // Clamp paging to reasonable bounds
            take = Math.Clamp(take, 1, 200);

            // Query DB (CreatedUtc DESC) then page
            var items = await _db.Tasks
                .OrderByDescending(t => t.CreatedUtc)
                .Skip(skip).Take(take)
                .ToListAsync();

            // Map entities -> DTOs
            var dtos = _mapper.Map<List<TaskItemDto>>(items);

            return Ok(dtos);
        }

        /// <summary>Get a single task by ID.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskItemDto>> GetById(int id)
        {
            var entity = await _db.Tasks.FindAsync(id);
            if (entity is null) return NotFound();

            return Ok(_mapper.Map<TaskItemDto>(entity));
        }

        /// <summary>Create a new task.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TaskItemDto>> Create([FromBody] TaskItemDto input)
        {
            // ModelState is auto-validated thanks to [ApiController]
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            // Map DTO -> entity; enforce server-owned fields
            var entity = _mapper.Map<TaskItem>(input);
            entity.Id = 0;                        // ensure new
            entity.IsDone = false;                // new tasks start not done
            entity.CreatedUtc = DateTime.UtcNow;  // server time

            _db.Add(entity);
            await _db.SaveChangesAsync();

            var dto = _mapper.Map<TaskItemDto>(entity);

            // Returns 201 with a Location header to GET that resource
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, dto);
        }

        /// <summary>Update an existing task (title + isDone).</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskItemDto>> Update(int id, [FromBody] TaskItemDto input)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var entity = await _db.Tasks.FindAsync(id);
            if (entity is null) return NotFound();

            // Update fields you allow to change
            entity.Title = input.Title!.Trim();
            entity.IsDone = input.IsDone;

            await _db.SaveChangesAsync();

            return Ok(_mapper.Map<TaskItemDto>(entity));
        }

        /// <summary>Delete a task by ID.</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _db.Tasks.FindAsync(id);
            if (entity is null) return NotFound();

            _db.Remove(entity);
            await _db.SaveChangesAsync();

            return NoContent(); // 204
        }
    }
}
