using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebStoreApp.Services.AuthenticationService;
using WebStoreApp.DTOs;
using WebStoreApp.Models;

namespace WebStoreApp.Controllers
{
    /// <summary>
    /// Manages user-related operations.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/users")]
    [ApiVersion("1.0")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenService _tokenService;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;

        public UserController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            TokenService tokenService,
            ILogger<UserController> logger,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a user by username.
        /// </summary>
        [HttpGet("username/{username}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Gets a user by username")]
        [SwaggerResponse(200, "Success", typeof(UserDTO))]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "User not found")]
        public async Task<ActionResult<UserDTO>> GetUserByUsername(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }
            return Ok(new { message = "User retrieved successfully.", user = _mapper.Map<UserDTO>(user) });
        }

        /// <summary>
        /// Retrieves a user by ID.
        /// </summary>
        [HttpGet("{userId}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Gets a user by ID")]
        [SwaggerResponse(200, "Success", typeof(UserDTO))]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "User not found")]
        public async Task<ActionResult<UserDTO>> GetUserById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }
            return Ok(new { message = "User retrieved successfully.", user = _mapper.Map<UserDTO>(user) });
        }

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Gets all users")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<UserDTO>))]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
        {
            var users = await Task.Run(() => _userManager.Users.ToList());
            return Ok(new { message = "Users retrieved successfully.", users = _mapper.Map<IEnumerable<UserDTO>>(users) });
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        [HttpPut("{userId}")]
        [Authorize]
        [SwaggerOperation(Summary = "Updates a user")]
        [SwaggerResponse(200, "User updated successfully")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "User not found")]
        public async Task<ActionResult> UpdateUser([FromBody] UserDTO userRequest, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            user.FirstName = userRequest.FirstName;
            user.LastName = userRequest.LastName;
            user.UserName = userRequest.FirstName + userRequest.LastName;
            user.PhoneNumber = userRequest.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Update failed.", errors = result.Errors });
            }

            return Ok(new { message = "User updated successfully." });
        }

        /// <summary>
        /// Resets a user's password.
        /// </summary>
        [HttpPost("reset-password")]
        [Authorize]
        [SwaggerOperation(Summary = "Resets a user's password")]
        [SwaggerResponse(200, "Password reset successfully")]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "User not found")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Password reset failed.", errors = result.Errors });
            }

            return Ok(new { message = "Password reset successfully." });
        }

        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        [HttpDelete("{userId}")]
        [Authorize]
        [SwaggerOperation(Summary = "Deletes a user by ID")]
        [SwaggerResponse(200, "User deleted successfully")]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "User not found")]
        public async Task<ActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "User deletion failed.", errors = result.Errors });
            }

            return Ok(new { message = "User deleted successfully." });
        }

        /// <summary>
        /// Retrieves users by role.
        /// </summary>
        [HttpGet("role/{roleName}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Gets users by role")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<UserDTO>))]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "Role not found")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsersByRole(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return NotFound(new { message = "Role not found." });
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            return Ok(new { message = "Users retrieved successfully.", users = _mapper.Map<IEnumerable<UserDTO>>(usersInRole) });
        }
    }
}
