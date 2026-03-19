using AuthService.Models;
using AuthService.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AuthService.Helpers;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtTokenGenerator _tokenGenerator;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            JwtTokenGenerator tokenGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenGenerator = tokenGenerator;
        }

        // ================= REGISTER =================
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return BadRequest(new { success = false, message = "User already exists" });

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(new { success = false, errors = result.Errors.Select(e => e.Description) });

            return Ok(new { success = true, message = "User registered successfully" });
        }

        // ================= LOGIN =================
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordValid)
                return Unauthorized("Invalid credentials");

            var token = await _tokenGenerator.GenerateToken(user);

            return Ok(new
            {
                token,
                expiresIn = 7200,
                user = new { user.Id, user.Email, user.FullName }
            });
        }

        // ================= TEMP ASSIGN ROLE =================

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return NotFound("User not found");

            // 🔍 Check roles BEFORE assignment
            var rolesBefore = await _userManager.GetRolesAsync(user);

            if (!await _roleManager.RoleExistsAsync(dto.Role))
                return BadRequest("Role does not exist");

            var result = await _userManager.AddToRoleAsync(user, dto.Role);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // 🔍 Check roles AFTER assignment
            var rolesAfter = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                message = $"Tried assigning role {dto.Role} to {dto.Email}",
                rolesBefore,
                rolesAfter
            });
        }
    }
}
