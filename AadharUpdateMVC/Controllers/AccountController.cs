using Microsoft.AspNetCore.Mvc;
using AadharUpdateMVC.Models;
using System.Net.Http.Json;

namespace AadharUpdateMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001";
                
                var response = await client.PostAsJsonAsync($"{apiBaseUrl}/api/auth/login", new
                {
                    Username = model.Username,
                    Password = model.Password
                });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (result?.Success == true)
                    {
                        // Store the token in session or cookie based on RememberMe
                        if (model.RememberMe)
                        {
                            Response.Cookies.Append("AuthToken", result.Token, new CookieOptions
                            {
                                Expires = DateTime.Now.AddDays(30),
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Strict
                            });
                        }
                        else
                        {
                            HttpContext.Session.SetString("AuthToken", result.Token);
                        }

                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing your request. Please try again later.");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001";
                
                // Get token from cookie or session
                var token = Request.Cookies["AuthToken"] ?? HttpContext.Session.GetString("AuthToken");
                
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    await client.PostAsync($"{apiBaseUrl}/api/auth/logout", null);
                    
                    // Clear local storage
                    Response.Cookies.Delete("AuthToken");
                    HttpContext.Session.Remove("AuthToken");
                }
            }
            catch
            {
                // Log the error but proceed with local logout
            }

            return RedirectToAction("Login");
        }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
} 