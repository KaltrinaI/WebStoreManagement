using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PartyUp.Services.AuthenticationService;
using WebStoreApp.DTOs;
using WebStoreApp.Models;

namespace WebStoreApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpGet("username/{username}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDTO>> GetUserByUsername(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(_mapper.Map<UserDTO>(user));
        }

        [HttpGet("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDTO>> GetUserById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(_mapper.Map<UserDTO>(user));
        }

        [HttpPut("{userId}")]
        [Authorize]
        public async Task<ActionResult> UpdateUser([FromBody] UserDTO userRequest, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            user.FirstName = userRequest.FirstName;
            user.LastName = userRequest.LastName;
            user.UserName = userRequest.FirstName + userRequest.LastName;
            user.PhoneNumber = userRequest.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User updated successfully");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            return Ok(_mapper.Map<IEnumerable<UserDTO>>(users));
        }

        [HttpPost("reset-password")]
        [Authorize]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Password reset successfully");
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User deleted successfully");
        }

        [HttpGet("role/{roleName}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsersByRole(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return NotFound("Role not found");
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            return Ok(_mapper.Map<IEnumerable<UserDTO>>(usersInRole));
        }
    }
}
