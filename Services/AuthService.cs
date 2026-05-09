using Microsoft.EntityFrameworkCore;
using CafeManagement.Data;
using CafeManagement.Models;
using CafeManagement.Models.Enums;
using CafeManagement.Exceptions;

namespace CafeManagement.Services
{
    public class AuthService {
        private readonly CafeDbContext _context;
        private readonly EmailService _emailService;
        public AuthService(CafeDbContext context, EmailService emailService) { 
            _context = context; 
            _emailService = emailService;
        }

        public async Task<User?> LoginAsync(string identifier, string password) {
            // ═══ OOP CONCEPT: EXCEPTION HANDLING ═══
            try {
                // Check both Email and Name (Case-insensitive)
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == identifier.ToLower() || 
                                              u.Name.ToLower() == identifier.ToLower());
                if (user == null) return null;
                bool valid = BCrypt.Net.BCrypt.Verify(password, user.Password);
                
                if (valid) {
                    // Send login notification
                    await _emailService.SendEmailAsync(user.Email, "Login Notification - BrewHouse Cafe", 
                        $"<h3>Hello {user.Name},</h3><p>You have just logged into your BrewHouse Cafe account.</p><p>If this wasn't you, please change your password immediately.</p>");
                }
                
                return valid ? user : null;
            } catch (Exception ex) {
                Console.WriteLine($"[{DateTime.Now}] LoginAsync Error: {ex.Message}");
                throw new AuthException("Login failed due to a system error.");
            }
        }

        public async Task<bool> RegisterAsync(string name, string email, string password) {
            // ═══ OOP CONCEPT: EXCEPTION HANDLING ═══
            try {
                bool exists = await _context.Users
                    .AnyAsync(u => u.Email.ToLower() == email.ToLower());
                if (exists) throw new DuplicateEmailException("This email is already registered.");

                var user = new User {
                    Name = name,
                    Email = email,
                    Password = BCrypt.Net.BCrypt.HashPassword(password),
                    Role = UserRole.Customer,
                    CreatedAt = DateTime.UtcNow,
                    IsEmailVerified = false,
                    EmailVerificationToken = Guid.NewGuid().ToString()
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Send verification email
                var verificationLink = $"http://localhost:5129/Auth/VerifyEmail?token={user.EmailVerificationToken}";
                await _emailService.SendEmailAsync(user.Email, "Verify Your Email - BrewHouse Cafe", 
                    $"<h3>Welcome to BrewHouse Cafe!</h3><p>Please click the link below to verify your email address:</p><a href='{verificationLink}'>Verify Email</a>");

                return true;
            } catch (DuplicateEmailException) {
                throw;
            } catch (Exception ex) {
                Console.WriteLine($"[{DateTime.Now}] RegisterAsync Error: {ex.Message}");
                throw new AuthException("Registration failed.");
            }
        }

        public async Task<User?> GetUserByIdAsync(int id) {
            // ═══ OOP CONCEPT: EXCEPTION HANDLING ═══
            try {
                return await _context.Users.FindAsync(id);
            } catch (Exception ex) {
                Console.WriteLine($"[{DateTime.Now}] GetUserById Error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateProfileAsync(int userId, string name, string email) {
            // ═══ OOP CONCEPT: EXCEPTION HANDLING ═══
            try {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) throw new ItemNotFoundException("User not found.");
                bool emailTaken = await _context.Users
                    .AnyAsync(u => u.Email.ToLower() == email.ToLower() && u.Id != userId);
                if (emailTaken) throw new DuplicateEmailException("Email already in use.");
                user.Name = name.Trim();
                user.Email = email.Trim().ToLower();
                await _context.SaveChangesAsync();
                return true;
            } catch (CafeException) { throw; }
            catch (Exception ex) {
                Console.WriteLine($"[{DateTime.Now}] UpdateProfile Error: {ex.Message}");
                throw new AuthException("Profile update failed.");
            }
        }

        public async Task<List<User>> GetAllStaffAsync() =>
            await _context.Users.Where(u => u.Role == UserRole.Staff).ToListAsync();

        public async Task DeleteUserAsync(int id) {
            // ═══ OOP CONCEPT: EXCEPTION HANDLING ═══
            try {
                var user = await _context.Users.FindAsync(id);
                if (user == null) throw new ItemNotFoundException("User not found.");
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            } catch (CafeException) { throw; }
            catch (Exception ex) {
                Console.WriteLine($"[{DateTime.Now}] DeleteUser Error: {ex.Message}");
                throw;
            }
        }
        public async Task<bool> VerifyEmailAsync(string token) {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailVerificationToken == token);
            if (user == null) return false;
            user.IsEmailVerified = true;
            user.EmailVerificationToken = null;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
