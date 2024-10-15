using System.ComponentModel.DataAnnotations;

namespace Image_guesser.Core.Domain.UserContext.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; } = false;

    public string LoginErrorMessage { get; set; } = string.Empty;
}