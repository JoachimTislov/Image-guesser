using System.ComponentModel.DataAnnotations;
using Image_guesser.Core.Domain.UserContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Image_guesser.Pages.Auth;

public class RegisterModel(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<RegisterModel> logger) : PageModel
{
    private readonly ILogger<RegisterModel> _logger = logger;

    private readonly UserManager<User> _userManager = userManager;
    private readonly SignInManager<User> _signInManager = signInManager;

    [BindProperty]
    [Required(ErrorMessage = "Username is required")]
    [StringLength(15, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 5)]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Password is required")]
    [StringLength(30, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Repeat Password is required")]
    [StringLength(30, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 8)]
    public string Repeat_Password { get; set; } = string.Empty;

    public string RegisterErrorMessage { get; set; } = string.Empty;

    public IdentityError[] Errors { get; private set; } = [];

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            if (Password != Repeat_Password)
            {
                RegisterErrorMessage = "Passwords does not match";
                return Page();
            }

            var user = new User
            {
                UserName = Username
            };

            var createUserResult = await _userManager.CreateAsync(user, Password);

            if (createUserResult.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToPage("/Home/Index");
            }
            else
            {
                RegisterErrorMessage = "Missing requirements:";
                Errors = createUserResult.Errors.ToArray();
            }
        }

        return Page();
    }
}
