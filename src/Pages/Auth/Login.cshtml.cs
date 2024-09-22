using System.ComponentModel.DataAnnotations;
using Image_guesser.Core.Domain.UserContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Image_guesser.Pages.Auth;

public class LoginModel(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<LoginModel> logger) : PageModel
{
    private readonly ILogger<LoginModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly SignInManager<User> _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));

    [Required(ErrorMessage = "Username is required")]
    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public bool RememberMe { get; set; } = false;

    public string LoginErrorMessage { get; set; } = string.Empty;

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            foreach (var modelStateEntry in ModelState)
            {
                var key = modelStateEntry.Key;
                var errors = modelStateEntry.Value.Errors;

                foreach (var error in errors)
                {
                    _logger.LogError("ModelState error: {error.ErrorMessage}", error.ErrorMessage);
                }
            }
            return Page();
        }

        var signInResult = await _signInManager.PasswordSignInAsync(Username, Password, RememberMe, lockoutOnFailure: false);

        if (signInResult.Succeeded)
        {
            _logger.LogInformation("User with username: {username} successfully signed in", Username);
            return RedirectToPage("/Home/Index");
        }
        else
        {
            var user = await _userManager.FindByNameAsync(Username);

            if (user != null)
            {
                LoginErrorMessage = "Wrong password";
            }
            else
            {
                LoginErrorMessage = "Invalid username";
            }
        }

        return Page();
    }
}
