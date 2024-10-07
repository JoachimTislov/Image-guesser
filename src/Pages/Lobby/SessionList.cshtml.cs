using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.SessionContext.Events;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.SignalRContext.Services.Hub;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Core.Domain.UserContext.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Image_guesser.Pages.Lobby;

[Authorize]
public class SessionListModel(ILogger<SessionListModel> logger, ISessionService sessionService, IUserService userService, IHubService hubService, IMediator mediator) : PageModel
{
    private readonly ILogger<SessionListModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    private readonly IHubService _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public Dictionary<Session, (User Host, User ChosenOracle)> SessionHosts { get; set; } = [];

    public User Player { get; set; } = null!;

    public async void OnGet()
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