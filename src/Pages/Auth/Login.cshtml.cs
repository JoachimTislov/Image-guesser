using System.ComponentModel.DataAnnotations;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Core.Domain.UserContext.Pipelines;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Image_guesser.Pages.Auth;

public class LoginModel(IMediator mediator, ILogger<LoginModel> logger, SignInManager<User> signInManager) : PageModel
{
    private readonly ILogger<LoginModel> _logger = logger;

    private readonly IMediator _mediator = mediator;

    private readonly SignInManager<User> _signInManager = signInManager;

    [Required(ErrorMessage = "Username is required")]
    [BindProperty]
    [StringLength(15, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 5)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [BindProperty]
    [StringLength(30, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 8)]
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
                    _logger.LogError("ModelState error for key '{key}': {error.ErrorMessage}", key, error.ErrorMessage);
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
            _logger.LogInformation("Sign in issues: {sigInResult}", signInResult);

            var user = await _mediator.Send(new GetUserByUsername.Request(Username));


            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    LoginErrorMessage = "Please confirm your email";
                }
                else
                {
                    LoginErrorMessage = "Wrong password";
                }
            }
            else
            {
                LoginErrorMessage = "Invalid username";
            }
        }

        return Page();
    }
}
