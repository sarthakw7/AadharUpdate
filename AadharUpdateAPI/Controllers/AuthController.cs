using Microsoft.AspNetCore.Mvc;
using AadharUpdateAPI.Models;
using AadharUpdateAPI.Services;
using System.Security.Authentication;

namespace AadharUpdateAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ISecurityService _securityService;
        private readonly ICacheService _cacheService;
        // In a real application, you would inject your user repository/database context here
        
        public AuthController(
            ISecurityService securityService,
            ICacheService cacheService)
        {
            _securityService = securityService;
            _cacheService = cacheService;
        }

        [HttpPost("login")]
        public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
        {
            try
            {
                // For demo purposes - in real application, get this from database
                var demoUser = new User
                {
                    UserId = 1,
                    Username = "demo",
                    PasswordHash = _securityService.HashPassword("demo123"),
                    Email = "demo@example.com"
                };

                if (request.Username != demoUser.Username)
                {
                    LoggingService.LogWarning($"Login attempt failed for username: {request.Username}");
                    throw new AuthenticationException("Invalid username or password");
                }

                if (!_securityService.VerifyPassword(request.Password, demoUser.PasswordHash))
                {
                    LoggingService.LogWarning($"Invalid password for user: {request.Username}");
                    throw new AuthenticationException("Invalid username or password");
                }

                // Generate a simple token (in real application, use JWT)
                string token = Guid.NewGuid().ToString();

                // Cache user session
                _cacheService.Set($"user_session_{token}", demoUser);

                // Log successful login
                LoggingService.LogInformation($"User {request.Username} logged in successfully");

                return Ok(new LoginResponse
                {
                    Success = true,
                    Message = "Login successful",
                    Token = token
                });
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                LoggingService.LogError(ex, $"Unexpected error during login for user: {request.Username}");
                return StatusCode(500, new LoginResponse
                {
                    Success = false,
                    Message = "An unexpected error occurred"
                });
            }
        }

        [HttpPost("logout")]
        public ActionResult Logout()
        {
            try
            {
                string? token = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
                if (string.IsNullOrEmpty(token))
                {
                    throw new ArgumentException("No authorization token provided");
                }

                // Remove user session from cache
                _cacheService.Remove($"user_session_{token}");
                LoggingService.LogInformation("User logged out successfully");

                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                LoggingService.LogError(ex, "Error during logout");
                return StatusCode(500, new { message = "An error occurred during logout" });
            }
        }
    }
} 