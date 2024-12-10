using Microsoft.AspNetCore.Mvc;
using Models;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Options; // Til IOptions<>
using UserService.Configurations; // Til MongoDbSettings
using UserService.Repositories;

namespace UserService.Controllers;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{

        private readonly MongoRepository<User> _repository;
        private readonly ILogger<UserController> _logger;

        public UserController(MongoRepository<User> repository, ILogger<UserController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _repository.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<User>> GetUser(string id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
public async Task<IActionResult> CreateUser(User newUser)
{
    await _repository.CreateAsync(newUser);
    return CreatedAtAction(nameof(GetUser), new { id = newUser.id }, newUser);
}
}