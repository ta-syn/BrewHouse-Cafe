using Microsoft.AspNetCore.Mvc;
using CafeManagement.Models.ViewModels;
using CafeManagement.Services;
using CafeManagement.Models.Enums;
using CafeManagement.Exceptions;

namespace CafeManagement.Controllers
{
    public class AuthController : Controller {
        private readonly AuthService _authService;
        public AuthController(AuthService authService) { _authService = authService; }

        [HttpGet] public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model) {
            if (!ModelState.IsValid) return View(model);
            try {
                var user = await _authService.LoginAsync(model.Email, model.Password);
                if (user == null) {
                    ModelState.AddModelError("", "Invalid email or password.");
                    return View(model);
                }
                
                // 🛑 Check if Email is Verified
                if (!user.IsEmailVerified) {
                    ModelState.AddModelError("", "Your email is not verified. Please check your inbox.");
                    return View(model);
                }

                // Set session
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("UserRole", user.Role.ToString());

                // Redirect based on role
                return user.Role switch {
                    UserRole.Admin => RedirectToAction("Dashboard", "Admin"),
                    UserRole.Staff => RedirectToAction("Dashboard", "Staff"),
                    _ => RedirectToAction("Menu", "Home")
                };
            } catch (Exception ex) {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpGet] public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model) {
            if (!ModelState.IsValid) return View(model);
            try {
                await _authService.RegisterAsync(model.Name, model.Email, model.Password);
                TempData["SuccessMessage"] = "Registration successful! Please check your email to verify your account before logging in.";
                return RedirectToAction("Login");
            } catch (DuplicateEmailException ex) {
                ModelState.AddModelError("Email", ex.Message);
                return View(model);
            } catch (Exception ex) {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public IActionResult Logout() {
            HttpContext.Session.Clear();  // FIX: clears cart + user session
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string token) {
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login");
            var result = await _authService.VerifyEmailAsync(token);
            if (result) {
                TempData["SuccessMessage"] = "Email verified successfully! You can now login.";
            } else {
                TempData["ErrorMessage"] = "Invalid or expired verification token.";
            }
            return RedirectToAction("Login");
        }
    }
}
