using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebStoreApp.DTOs;
using WebStoreApp.Services.AuthenticationService;
using WebStoreApp.Models;

namespace WebStoreApp.Controllers
{
    /// <summary>
    /// Authentication API for user login, registration, and role management.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/auth")]
    [ApiVersion("1.0")]
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

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">User registration details.</param>
        /// <returns>Success or error message.</returns>
        [HttpPost("register")]
        [SwaggerOperation(Summary = "Registers a new user", Description = "Creates a new user account.")]
        [SwaggerResponse(201, "User created successfully.")]
        [SwaggerResponse(400, "Invalid request or user already exists.")]
        public async Task<ActionResult> Register([FromBody] RegistrationRequestDTO request)
        {
            if (request == null)
            {
                return BadRequest("Invalid client request.");
            }

            var userExists = await _userManager.FindByEmailAsync(request.Email);
            if (userExists != null)
            {
                return BadRequest("User already exists.");
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
                return BadRequest(result.Errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "user");
            if (!roleResult.Succeeded)
            {
                return BadRequest("User registered, but failed to assign default role.");
            }

            _logger.LogInformation($"User {user.UserName} registered successfully with default role 'user'.");
            return CreatedAtAction(nameof(Register), new { user.Email }, "User created successfully.");
        }


        /// <summary>
        /// Authenticates a user and returns an access token.
        /// </summary>
        /// <param name="request">User login credentials.</param>
        /// <returns>JWT token and user details.</returns>
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Logs in a user", Description = "Authenticates a user and returns a JWT token.")]
        [SwaggerResponse(200, "Successful login.", typeof(AuthResponseDTO))]
        [SwaggerResponse(400, "Invalid request or missing credentials.")]
        [SwaggerResponse(401, "Invalid credentials.")]
        public async Task<ActionResult<AuthResponseDTO>> Authenticate([FromBody] AuthRequestDTO request)
        {
            if (request == null)
            {
                return BadRequest("Request body is empty.");
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return NotFound("No user found.");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
            {
                return Unauthorized("Invalid credentials.");
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

        /// <summary>
        /// Creates a new role in the system.
        /// </summary>
        /// <param name="roleName">Role name to create.</param>
        /// <returns>Success or error message.</returns>
        [HttpPost("role")]
        [SwaggerOperation(Summary = "Creates a new role", Description = "Adds a new role to the system.")]
        [SwaggerResponse(201, "Role created successfully.")]
        [SwaggerResponse(400, "Invalid request or role already exists.")]
        public async Task<ActionResult> CreateRoles([FromBody] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Role name cannot be empty.");
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
            return CreatedAtAction(nameof(CreateRoles), new { roleName }, $"Role {roleName} created successfully.");
        }

        /// <summary>
        /// Assigns a role to a user.
        /// </summary>
        /// <param name="username">Username of the user.</param>
        /// <param name="roleName">Role to assign.</param>
        /// <returns>Success or error message.</returns>
        [HttpPost("assign")]
        [SwaggerOperation(Summary = "Assigns a role to a user", Description = "Assigns an existing role to a user, overriding any previous roles.")]
        [SwaggerResponse(200, "Role assigned successfully.")]
        [SwaggerResponse(400, "Invalid request.")]
        [SwaggerResponse(404, "User or role not found.")]
        public async Task<ActionResult> AssignRoleToUser([FromBody] string username, string roleName)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Username and role name cannot be empty.");
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

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                return BadRequest("Failed to remove existing roles.");
            }

            var addResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!addResult.Succeeded)
            {
                return BadRequest("Failed to assign new role.");
            }

            _logger.LogInformation($"User {username} had roles {string.Join(", ", currentRoles)} removed and was assigned the new role {roleName}.");
            return Ok($"User {username} was assigned the role '{roleName}', replacing previous roles.");
        }

    }
}
