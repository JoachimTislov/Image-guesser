using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Image_guesser.Pages.Lobby;

public class SessionListModel(ILogger<SessionListModel> logger) : PageModel
{
    private readonly ILogger<SessionListModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}