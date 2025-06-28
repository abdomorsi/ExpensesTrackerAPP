namespace Expenses.API.Controllers;

public class UserRegistrationDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}