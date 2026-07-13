using EventBoard.Api.Data;
using EventBoard.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBoard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<UsersController> _logger;

    public UsersController(AppDbContext context, ILogger<UsersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        _logger.LogInformation("Retrieving all users");
        var users = await _context.Users
            .AsNoTracking()
            .ToListAsync();

        return Ok(users.Select(u => MapToUserDto(u)));
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        _logger.LogInformation("Retrieving user with ID: {UserId}", id);

        if (id <= 0)
        {
            _logger.LogWarning("Invalid user ID provided: {UserId}", id);
            return BadRequest("User ID must be greater than 0");
        }

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            _logger.LogWarning("User not found with ID: {UserId}", id);
            return NotFound($"User with ID {id} not found");
        }

        return Ok(MapToUserDto(user));
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request)
    {
        _logger.LogInformation("Creating new user with email: {Email}", request.Email);

        // Validate input
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for user creation");
            return BadRequest(ModelState);
        }

        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email))
        {
            _logger.LogWarning("Name or Email is empty");
            return BadRequest("Name and Email are required");
        }

        // Check if email already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (existingUser != null)
        {
            _logger.LogWarning("User with email {Email} already exists", request.Email);
            return Conflict($"A user with email {request.Email} already exists");
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim()
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User created successfully with ID: {UserId}", user.Id);
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, MapToUserDto(user));
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        _logger.LogInformation("Updating user with ID: {UserId}", id);

        if (id <= 0)
        {
            _logger.LogWarning("Invalid user ID provided: {UserId}", id);
            return BadRequest("User ID must be greater than 0");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for user update");
            return BadRequest(ModelState);
        }

        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            _logger.LogWarning("User not found with ID: {UserId}", id);
            return NotFound($"User with ID {id} not found");
        }

        // Check if new email already exists (and is different from current)
        if (!user.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == request.Email && u.Id != id);

            if (emailExists)
            {
                _logger.LogWarning("Email {Email} already exists for another user", request.Email);
                return Conflict($"Email {request.Email} is already in use by another user");
            }
        }

        user.Name = request.Name?.Trim() ?? user.Name;
        user.Email = request.Email?.Trim() ?? user.Email;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User updated successfully with ID: {UserId}", id);
        return Ok(MapToUserDto(user));
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        _logger.LogInformation("Deleting user with ID: {UserId}", id);

        if (id <= 0)
        {
            _logger.LogWarning("Invalid user ID provided: {UserId}", id);
            return BadRequest("User ID must be greater than 0");
        }

        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            _logger.LogWarning("User not found with ID: {UserId}", id);
            return NotFound($"User with ID {id} not found");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User deleted successfully with ID: {UserId}", id);
        return NoContent();
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }
}

/// <summary>
/// DTO for creating a user
/// </summary>
public class CreateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// DTO for updating a user
/// </summary>
public class UpdateUserRequest
{
    public string? Name { get; set; }
    public string? Email { get; set; }
}

/// <summary>
/// DTO for user response
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
