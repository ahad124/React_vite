using EventBoard.Api.Models;
using EventBoard.Api.Repositories;

namespace EventBoard.Api.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public async Task<Guid> RegisterAsync(string email, string password)
    {
        // Check if email already exists
        if (await _userRepository.UserExistsAsync(email))
        {
            _logger.LogWarning("Registration failed: email {Email} already exists", email);
            throw new InvalidOperationException($"A user with email '{email}' already exists.");
        }

        // Hash the password with BCrypt
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        // Create a new user with default role "User"
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            Role = "User"
        };

        await _userRepository.AddUserAsync(user);

        _logger.LogInformation("User registered successfully with ID: {UserId}", user.Id);
        return user.Id;
    }

    public async Task<AuthResponseDto?> LoginAsync(string email, string password)
    {
        // Look up the user by email
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            _logger.LogWarning("Login failed: no user found with email {Email}", email);
            return null;
        }

        // Verify the BCrypt password hash
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed: invalid password for email {Email}", email);
            return null;
        }

        // Generate JWT token
        var (token, expiresAt) = _jwtTokenService.GenerateToken(user);

        _logger.LogInformation("User logged in successfully: {UserId}", user.Id);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt
        };
    }
}
