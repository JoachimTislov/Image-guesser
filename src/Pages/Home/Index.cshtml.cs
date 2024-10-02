using Image_guesser.Core.Domain.SessionContext.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Image_guesser.Pages.Home;

public class IndexModel(ILogger<IndexModel> logger, ISessionService sessionService) : PageModel
{
    private readonly ILogger<IndexModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));

    public Guid SessionId { get; private set; } = Guid.NewGuid();

    public async Task<IActionResult> OnPostCreateSession(Guid Id)
    {
        await _sessionService.CreateSession(User, Id);

        _logger.LogInformation("{Name} created a Session", User.Identity!.Name);

        return RedirectToPage("/Lobby/ConfigureSessionOptions", new { text = "Create", Id });
    }

}

