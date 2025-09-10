using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using todo.Entities;
using todo.ViewModels;

namespace todo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly HtmlSanitizer _htmlSanitizer = new();

        public AccountsController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }


        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("RegisterLimiter")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            model.Username = _htmlSanitizer.Sanitize(model.Username);
            model.FirstName = _htmlSanitizer.Sanitize(model.FirstName);
            model.LastName = _htmlSanitizer.Sanitize(model.LastName);
            model.Email = _htmlSanitizer.Sanitize(model.Email);

            var user = new User
            {
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _userManager.AddToRoleAsync(user, "User");

            return Ok(new { Message = "User registered successfully" });
        }


        [HttpPost("login")]
        [EnableRateLimiting("LoginLimiter")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            model.Username = _htmlSanitizer.Sanitize(model.Username);
            model.Password = _htmlSanitizer.Sanitize(model.Password);


            var user = await _userManager.FindByEmailAsync(model.Username)
            ?? await _userManager.FindByNameAsync(model.Username);

            if (user == null)
                return Unauthorized(new { Message = "Felaktigt användarnamn eller email" });

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                model.Password,
                isPersistent: false,
                lockoutOnFailure: true);

            if (!result.Succeeded)
                return Unauthorized(new { Message = "Felaktigt användarnamn eller lösenord" });

            return Ok(new { Message = "Login successful" });
        }


        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { Message = "Logout successful" });
        }
    }
}