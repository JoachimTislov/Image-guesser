using Image_guesser.Core.Domain.UserContext.Services;
using Image_guesser.Core.Domain.UserContext.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Image_guesser.Pages.Auth;

public class RegisterModel(IUserService userService, ILogger<RegisterModel> logger) : PageModel
{
    private readonly ILogger<RegisterModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));

    [BindProperty]
    public RegisterViewModel ViewModel { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            if (ViewModel.Password != ViewModel.Repeat_Password)
            {
                ViewModel.RegisterErrorMessage = "Passwords do not match";
                return Page();
            }

            var (Succeeded, errors) = await _userService.Register(ViewModel.Username, ViewModel.Password);
            if (Succeeded)
            {
                _logger.LogInformation("{Name} registered", ViewModel.Username);
                return RedirectToPage("/Home/Index");
            }
            else
            {
                ViewModel.Errors = errors;
            }
        }

        return Page();
    }
}
