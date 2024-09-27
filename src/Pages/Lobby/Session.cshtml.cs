using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Image_guesser.Pages.Lobby;

public class SessionModel(ILogger<SessionModel> logger) : PageModel
{
    private readonly ILogger<SessionModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}