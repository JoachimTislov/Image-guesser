using Image_guesser.Core.Domain.GameContext;
using Image_guesser.Core.Domain.GameContext.Services;
using Image_guesser.Core.Domain.LeaderboardContext;
using Image_guesser.Core.Domain.LeaderboardContext.Services;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Core.Domain.UserContext.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Image_guesser.Pages.Home;

public class IndexModel(ILogger<IndexModel> logger, ISessionService sessionService, IGameService gameService, IUserService userService, ILeaderboardService leaderboardService) : PageModel
{
    private readonly ILogger<IndexModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    private readonly IGameService _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    private readonly ILeaderboardService _leaderboardService = leaderboardService ?? throw new ArgumentNullException(nameof(leaderboardService));

    public Guid SessionId { get; private set; } = Guid.NewGuid(); // Creating new guid for potential session here to sync with client side

    public User? User_ { get; set; }

    public bool UserIsAlreadySessionHost { get; set; }

    public List<LeaderboardEntry> LeaderboardEntries { get; set; } = [];
    public List<BaseGame> RecentGames { get; set; } = [];

    public async Task OnGet()
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            User_ = await _userService.GetUserByClaimsPrincipal(User);
        }

        await LoadLeaderboards();
    }

    public async Task<IActionResult> OnPostCreateSession(Guid Id)
    {
        await _sessionService.CreateSession(User, Id);

        _logger.LogInformation("{Name} created a Session", User.Identity!.Name);

        return RedirectToPage("/Lobby/ConfigureSessionOptions", new { text = "Create", Id });
    }

    private async Task LoadLeaderboards()
    {
        LeaderboardEntries = _leaderboardService.GetLeaderboardEntries();
        RecentGames = await _gameService.GetXAmountOfRecentGames(10);
    }
}

