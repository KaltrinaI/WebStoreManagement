using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PartyUp.DTOs;
using PartyUp.Services.AuthenticationService;
using WebStoreApp.Models;

namespace WebStoreApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, TokenService tokenService, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegistrationRequestDTO request)
        {
            if (request == null)
            {
                return BadRequest("Invalid client request");
            }

            var userExists = await _userManager.FindByEmailAsync(request.Email);
            if (userExists != null)
            {
                return BadRequest("User already exists");
            }

            var user = new User
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            _logger.LogInformation($"User {user.UserName} registered successfully.");
            return Ok("User created successfully");
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Authenticate([FromBody] AuthRequestDTO request)
        {
            if (request == null)
            {
                return BadRequest("Request body is empty");
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest("No user found");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
            {
                return BadRequest("Bad credentials");
            }

            var accessToken = await _tokenService.CreateToken(user);
            _logger.LogInformation($"User {user.UserName} authenticated successfully.");

            return Ok(new AuthResponseDTO
            {
                Email = user.Email,
                Token = accessToken,
                UserId = user.Id
            });
        }

        [HttpPost("role")]
        public async Task<ActionResult> CreateRoles(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Role name cannot be empty");
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
            }

            _logger.LogInformation($"Role {roleName} created successfully.");
            return Ok($"Role {roleName} created successfully.");
        }

        [HttpPost("assign")]
        public async Task<ActionResult> AssignRoleToUser(string username, string roleName)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Username and role name cannot be empty");
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound($"User with username '{username}' not found.");
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return NotFound($"Role '{roleName}' does not exist.");
            }

            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                var result = await _userManager.AddToRoleAsync(user, roleName);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
            }

            _logger.LogInformation($"Role {roleName} assigned to user {username} successfully.");
            return Ok($"Role {roleName} assigned to user {username} successfully.");
        }
    }
}
