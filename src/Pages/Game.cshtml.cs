using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Image_guesser.Pages;

public class GameModel(ILogger<ProfileModel> logger) : PageModel
{
    private readonly ILogger<ProfileModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
