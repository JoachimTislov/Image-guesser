using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Core.Domain.UserContext.Services;
using Image_guesser.SharedKernel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Image_guesser.Pages.Lobby;

[RequireLogin]
public class SessionListModel(ILogger<SessionListModel> logger, ISessionService sessionService, IUserService userService) : PageModel
{
    private readonly ILogger<SessionListModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));

    public Dictionary<Session, (User Host, User ChosenOracle)> SessionHosts { get; set; } = [];

    public User Player { get; set; } = null!;

    public async Task OnGet()
    {
        var Sessions = _sessionService.GetAllOpenSessions();

        foreach (var session in Sessions)
        {
            User Host = await _userService.GetUserById(session.SessionHostId.ToString());
            User ChosenOracle = await _userService.GetUserById(session.ChosenOracleId.ToString());

            if (!SessionHosts.ContainsKey(session))
            {
                SessionHosts.Add(session, (Host, ChosenOracle));
            }
        }

        Player = await _userService.GetUserByClaimsPrincipal(User);

        _logger.LogInformation("{Name} viewed the session list", Player.UserName);
    }
}