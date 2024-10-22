using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SqlBasedBookAPI;
using SqlBasedBookAPI.Services;


namespace SqlBasedBookAPI.Controllers
    {

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
       
        public AuthController(IAuthService authService, IEmailService emailService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _emailService = emailService;
           
            }

        [HttpPost("signup")]
        public async Task<IActionResult> Register(UserRegistrationDto userRegistrationDto)
        {
            // Check if the model is valid
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { message = "Validation failed", errors });
            }

            // Call the AuthService to register the user
            var result = await _authService.RegisterAsync(userRegistrationDto);

            // If registration failed, return the reason
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            // Send the welcome email
            try
            {
                await _emailService.SendWelcomeEmail(userRegistrationDto.Email, userRegistrationDto.Username);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Registration successful, but failed to send welcome email", error = ex.Message });
            }

            // On success, return a success message
            return Ok(new { message = result.Message });
        }


        /*
        [HttpPost("signup")]
        public async Task<IActionResult> Register(UserRegistrationDto userDto)
        {
            // Check if the model is valid
            if (!ModelState.IsValid)
            {
                // Return validation errors in the response
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { message = "Validation failed", errors });
            }

            // Call the AuthService to register the user
            var result = await _authService.RegisterAsync(userDto);

            // If registration failed, return the reason
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            // On success, return a success message
            return Ok(new { message = result.Message });
        }*/

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("Invalid login data.");
            }

            // Delegate user validation and token generation to AuthService
            var result = await _authService.AuthenticateAsync(loginDto.Username, loginDto.Password);

            if (!result.Success)
            {
                return Unauthorized(new { message = result.Message });
            }

            return Ok(new { token = result.Token });
        }
    }
}
