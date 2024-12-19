using Microsoft.AspNetCore.Mvc;
using UserService.Models;
using UserService.Repositories;

namespace UserService.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{

    private readonly MongoRepository<User> _repository;
    private readonly ILogger<UserController> _logger;

    public UserController(MongoRepository<User> repository, ILogger<UserController> logger, IConfiguration config)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        _logger.LogInformation("Fetching all users from the database.");

        try
        {
            var users = await _repository.GetAllAsync();
            _logger.LogInformation($"Successfully retrieved {users.Count()} users.");
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while fetching users: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("GetByUsername/{username?}")]
    public async Task<ActionResult<Credentials>> GetUser(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            _logger.LogWarning("Attempted to fetch user with an invalid or empty username.");
            return BadRequest("Invalid ID format.");
        }

        _logger.LogInformation($"Fetching user with username: {username}");

        try
        {
            var user = await _repository.GetByUsernameAsync(username);
            if (user == null)
            {
                _logger.LogWarning($"User with username {username} not found.");
                return NotFound();
            }

            var UserCredentials = new Credentials()
            {
                Username = user.Username,
                Password = user.Password
            };

            _logger.LogInformation($"Successfully retrieved credentials for user {username}.");
            return Ok(UserCredentials);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while fetching user {username}: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(User newUser)
    {
        _logger.LogInformation("Attempting to create a new user.");

        try
        {
            await _repository.CreateAsync(newUser);
            _logger.LogInformation($"Successfully created a new user with username: {newUser.Username}");

            return CreatedAtAction(nameof(GetUser), new { id = newUser.id }, newUser);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while creating the user with username {newUser.Username}: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}