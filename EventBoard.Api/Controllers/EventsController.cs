using EventBoard.Api.Data;
using EventBoard.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBoard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<EventsController> _logger;

    public EventsController(AppDbContext context, ILogger<EventsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all events
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetAllEvents()
    {
        _logger.LogInformation("Retrieving all events");
        var events = await _context.Events
            .Include(e => e.User)
            .AsNoTracking()
            .ToListAsync();

        return Ok(events.Select(e => MapToEventDto(e)));
    }

    /// <summary>
    /// Get event by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventDto>> GetEventById(int id)
    {
        _logger.LogInformation("Retrieving event with ID: {EventId}", id);

        if (id <= 0)
        {
            _logger.LogWarning("Invalid event ID provided: {EventId}", id);
            return BadRequest("Event ID must be greater than 0");
        }

        var evt = await _context.Events
            .Include(e => e.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (evt == null)
        {
            _logger.LogWarning("Event not found with ID: {EventId}", id);
            return NotFound($"Event with ID {id} not found");
        }

        return Ok(MapToEventDto(evt));
    }

    /// <summary>
    /// Get events by user ID
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetEventsByUserId(int userId)
    {
        _logger.LogInformation("Retrieving events for user ID: {UserId}", userId);

        var events = await _context.Events
            .Where(e => e.UserId == userId)
            .Include(e => e.User)
            .AsNoTracking()
            .ToListAsync();

        return Ok(events.Select(e => MapToEventDto(e)));
    }

    /// <summary>
    /// Create a new event
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventDto>> CreateEvent([FromBody] CreateEventRequest request)
    {
        _logger.LogInformation("Creating new event: {Title}", request.Title);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for event creation");
            return BadRequest(ModelState);
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            _logger.LogWarning("Event title is empty");
            return BadRequest("Event title is required");
        }

        // Verify user exists
        var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
        if (!userExists)
        {
            _logger.LogWarning("User not found with ID: {UserId}", request.UserId);
            return NotFound($"User with ID {request.UserId} not found");
        }

        var evt = new Event
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Date = request.Date,
            UserId = request.UserId
        };

        _context.Events.Add(evt);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Event created successfully with ID: {EventId}", evt.Id);
        return CreatedAtAction(nameof(GetEventById), new { id = evt.Id }, MapToEventDto(evt));
    }

    /// <summary>
    /// Update an existing event
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventDto>> UpdateEvent(int id, [FromBody] UpdateEventRequest request)
    {
        _logger.LogInformation("Updating event with ID: {EventId}", id);

        if (id <= 0)
        {
            _logger.LogWarning("Invalid event ID provided: {EventId}", id);
            return BadRequest("Event ID must be greater than 0");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for event update");
            return BadRequest(ModelState);
        }

        var evt = await _context.Events.FindAsync(id);

        if (evt == null)
        {
            _logger.LogWarning("Event not found with ID: {EventId}", id);
            return NotFound($"Event with ID {id} not found");
        }

        evt.Title = request.Title?.Trim() ?? evt.Title;
        evt.Description = request.Description?.Trim() ?? evt.Description;
        evt.Date = request.Date ?? evt.Date;

        _context.Events.Update(evt);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Event updated successfully with ID: {EventId}", id);
        return Ok(MapToEventDto(evt));
    }

    /// <summary>
    /// Delete an event
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        _logger.LogInformation("Deleting event with ID: {EventId}", id);

        if (id <= 0)
        {
            _logger.LogWarning("Invalid event ID provided: {EventId}", id);
            return BadRequest("Event ID must be greater than 0");
        }

        var evt = await _context.Events.FindAsync(id);

        if (evt == null)
        {
            _logger.LogWarning("Event not found with ID: {EventId}", id);
            return NotFound($"Event with ID {id} not found");
        }

        _context.Events.Remove(evt);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Event deleted successfully with ID: {EventId}", id);
        return NoContent();
    }

    private static EventDto MapToEventDto(Event evt)
    {
        return new EventDto
        {
            Id = evt.Id,
            Title = evt.Title,
            Description = evt.Description,
            Date = evt.Date,
            UserId = evt.UserId,
            UserName = evt.User?.Name ?? "Unknown User"
        };
    }
}

/// <summary>
/// DTO for creating an event
/// </summary>
public class CreateEventRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public int UserId { get; set; }
}

/// <summary>
/// DTO for updating an event
/// </summary>
public class UpdateEventRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? Date { get; set; }
}

/// <summary>
/// DTO for event response
/// </summary>
public class EventDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
}
