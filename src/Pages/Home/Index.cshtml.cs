using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Core.Domain.UserContext.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Image_guesser.Pages.Home;

public class IndexModel(ILogger<IndexModel> logger, ISessionService sessionService, IUserService userService) : PageModel
{
    private readonly ILogger<IndexModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));

    public Guid SessionId { get; private set; } = Guid.NewGuid(); // Creating new guid for potential session here to sync with client side

    public User? User_ { get; set; }

    public bool UserIsAlreadySessionHost { get; set; }

    public async Task OnGet()
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            User_ = await _userService.GetUserByClaimsPrincipal(User);

            // Override the SessionId if the user is already a session host
            var potentialSessionId = await _userService.GetSessionIdForGivenUserWithClaimPrincipal(User);
            if (potentialSessionId != null)
            {
                UserIsAlreadySessionHost = true;
                SessionId = potentialSessionId.Value;
            }
        }
    }

    public async Task<IActionResult> OnPostCreateSession(Guid Id)
    {
        await _sessionService.CreateSession(User, Id);

        _logger.LogInformation("{Name} created a Session", User.Identity!.Name);

        return RedirectToPage("/Lobby/ConfigureSessionOptions", new { text = "Create", Id });
    }

}

