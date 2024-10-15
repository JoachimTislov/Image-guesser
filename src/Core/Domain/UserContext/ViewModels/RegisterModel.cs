using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Image_guesser.Core.Domain.UserContext.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Repeat Password is required")]
    public string Repeat_Password { get; set; } = string.Empty;

    public string RegisterErrorMessage { get; set; } = string.Empty;

    public IdentityError[] Errors { get; set; } = [];
}