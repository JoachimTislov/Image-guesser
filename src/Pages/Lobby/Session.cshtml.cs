using Image_guesser.Core.Domain.GameContext.Events;
using Image_guesser.Core.Domain.ImageContext;
using Image_guesser.Core.Domain.ImageContext.Services;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.SessionContext.Events;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.SignalRContext.Services.Hub;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Core.Domain.UserContext.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Image_guesser.Pages.Lobby;

[Authorize]
public class SessionModel(ILogger<SessionModel> logger, ISessionService sessionService, IUserService userService, IImageService imageService, IMediator mediator) : PageModel
{
    private readonly ILogger<SessionModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    private readonly IImageService _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public List<string> SessionErrorMessages { get; set; } = [];

    public Session Session { get; set; } = null!;
    public User SessionHost { get; set; } = null!;
    public User ChosenOracle { get; set; } = null!;

    public ImageRecord? ImageRecord { get; set; }

    public User Player { get; set; } = null!;

    public async Task OnGet()
    {
        Session = await _sessionService.GetSessionById(Id);
        SessionHost = await _userService.GetUserById(Session.SessionHostId.ToString());
        ChosenOracle = await _userService.GetUserById(Session.ChosenOracleId.ToString());

        var identifier = Session.Options.ImageIdentifier;
        if (identifier != string.Empty)
        {
            ImageRecord = await _imageService.GetImageRecordById(identifier);
        }

        Player = await _userService.GetUserByClaimsPrincipal(User);
        _logger.LogInformation("{Name} entered the session page with Id: {Id}", Player.UserName, Id);
    }

    public async Task OnPostStartGame()
    {
        var session = await _sessionService.GetSessionById(Id);

        if (!session.Options.IsGameMode(GameMode.SinglePlayer) && session.SessionUsers.Count < 2)
        {
            SessionErrorMessages.Add("Need more players to start a game");
        }
        else if (session.HasPlayedSetAmountGames)
        {
            SessionErrorMessages.Add("Reached set games limit, increase it to start a new game");
        }
        else
        {
            _logger.LogInformation("{Name} started a game in a session with Id: {Id}", User.Identity!.Name, Id);
            await _mediator.Publish(new CreateGame(Id));
        }
    }

    public async Task OnPostCloseSession()
    {
        await _mediator.Publish(new SessionClosed(Id));
    }

    public async Task OnPostRemoveUserFromSession(string userId)
    {
        await _mediator.Publish(new UserLeftSessionOrWasKicked(userId, Id));
    }
}