using System.ComponentModel.DataAnnotations;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.SharedKernel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Image_guesser.Pages.Auth;

public class RegisterModel(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<RegisterModel> logger) : PageModel
{
    private readonly ILogger<RegisterModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly SignInManager<User> _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));

    [BindProperty]
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Repeat Password is required")]
    public string Repeat_Password { get; set; } = string.Empty;

    public string RegisterErrorMessage { get; set; } = string.Empty;

    public IdentityError[] Errors { get; private set; } = [];

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            if (Password != Repeat_Password)
            {
                RegisterErrorMessage = "Passwords do not match";
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
