using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Image_guesser.Core.Domain.UserContext;
using Microsoft.AspNetCore.Mvc;

namespace Image_guesser.Pages;

public class ProfileModel(ILogger<ProfileModel> logger, UserManager<User> userManager, SignInManager<User> signInManager) : PageModel
{
    private readonly ILogger<ProfileModel> _logger = logger;

    private readonly UserManager<User> _userManager = userManager;

    private readonly SignInManager<User> _signInManager = signInManager;

    public User? User_;

    public System.Reflection.PropertyInfo[] attributes = [];

    public string[] excludedProperties = [
        "NormalizedEmail",
        "NormalizedUserName",
        "PasswordHash",
         "SecurityStamp",
         "ConcurrencyStamp",
         "AccessFailedCount",
         "LockoutEnabled",
         "LockoutEnd"
    ];

    public async Task<IActionResult> OnGet()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            var username = User.Identity.Name!;

            _logger.LogInformation("User: {User} Viewed their profile", username);

            User_ = await _userManager.FindByNameAsync(username);

            if (User_ == null)
            {
                _logger.LogWarning("User not found: {username}", username);
                return NotFound(new { message = "User not found." });
            }

            attributes = User_?.GetType().GetProperties()!;

            return Page();
        }
        else
        {
            return RedirectToPage("/Home/Index");
        }

    }

    public async Task<IActionResult> OnPostDeleteAccountAsync()
    {
        var username = User.Identity?.Name;

        if (string.IsNullOrEmpty(username))
        {
            _logger.LogWarning("Attempt to delete account failed: no user is logged in.");
            return BadRequest(new { message = "User is not logged in." });
        }

        _logger.LogInformation("User with name: {username} is attempting to delete their account", username);
        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            _logger.LogWarning("User not found: {username}", username);
            return NotFound(new { message = "User not found." });
        }

        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Home/Index");
        }
        else
        {
            _logger.LogError("Deletion of: {username}'s account failed", username);
            return StatusCode(500, new { message = "Failed to delete your account, sorry." });
        }
    }
}
