using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Expenses.API.Data;
using Expenses.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Expenses.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(AppDbContext appDbContext, PasswordHasher<User> passwordHasher) : Controller
{
    [HttpPost("Login")]
    public IActionResult Login([FromBody] UserLoginDto loginDto)
    {
        if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
        {
            return BadRequest(new { Message = "Email and password are required." });
        }
        // Check if the user exists
        var user = appDbContext.Users.FirstOrDefault(u => u.Email == loginDto.Email);
        if (user == null)
        {
            return Unauthorized(new { Message = "Invalid email or password." });
        }
        // In a real application, you would typically verify the password using a password hasher
        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password);
        if (passwordVerificationResult != PasswordVerificationResult.Success)
        {
            return Unauthorized(new { Message = "Invalid email or password." });
        }
        // Generate JWT token
        var token = GenerateJwtToken(user);
        return Ok(new { Token = token });
    }
    
    
    [HttpPost("Register")]
    public IActionResult Register([FromBody] UserRegistrationDto registrationDto)
    {
        if (registrationDto == null || string.IsNullOrEmpty(registrationDto.Email) || string.IsNullOrEmpty(registrationDto.Password))
        {
            return BadRequest(new { Message = "Email and password are required." });
        }
        // Check if the user already exists
        var existingUser = appDbContext.Users.FirstOrDefault(u => u.Email == registrationDto.Email);
        if (existingUser != null)
        {
            return Conflict(new { Message = "User with this email already exists." });
        }
        // Validate the password (e.g., length, complexity) - this is a simple example
        if (registrationDto.Password.Length < 6)
        {
            return BadRequest(new { Message = "Password must be at least 6 characters long." });
        }
        // In a real application, you would also want to validate the email format
        if (!System.Text.RegularExpressions.Regex.IsMatch(registrationDto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            return BadRequest(new { Message = "Invalid email format." });
        }
        
        // Here you would typically hash the password and save the user to the database
        var hashedPassword = passwordHasher.HashPassword(null, registrationDto.Password);
        var user = new User
        {
            Email = registrationDto.Email,
            Password = hashedPassword,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        appDbContext.Users.Add(user);
        appDbContext.SaveChanges();
        var token = GenerateJwtToken(user);
        return Ok(new { Token = token });
    }
    
    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        };
        var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey("YourSuperSecretKey123456789012345678901234"u8.ToArray());
        var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: "dev.morsy.net",
            audience: "dev.morsy.net",
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}