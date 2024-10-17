using Image_guesser.Core.Domain.UserContext.Services;
using Image_guesser.Core.Domain.UserContext.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Image_guesser.Pages.Auth;

public class LoginModel(IUserService userService, ILogger<LoginModel> logger) : PageModel
{
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    private readonly ILogger<LoginModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    [BindProperty]
    public LoginViewModel ViewModel { get; set; } = new();

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

        var (Succeeded, errorMessage) = await _userService.Login(User, ViewModel.Username, ViewModel.Password, ViewModel.RememberMe);

        if (Succeeded)
        {
            _logger.LogInformation("{Name} logged in", ViewModel.Username);
            return RedirectToPage("/Home/Index");
        }
        else
        {
            ViewModel.LoginErrorMessage = errorMessage;
        }

        return Page();
    }
}
