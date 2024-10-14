using Image_guesser.Core.Domain.LeaderboardContext.Services;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.SessionContext.Services;
using MediatR;

namespace Image_guesser.Core.Domain.LeaderboardContext.Handlers;

public class SessionClosedHandler(ILeaderboardService leaderboardService, ISessionService sessionService) : INotificationHandler<SessionClosed>
{
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    private readonly ILeaderboardService _leaderboardService = leaderboardService ?? throw new ArgumentNullException(nameof(leaderboardService));

    public async Task Handle(SessionClosed notification, CancellationToken cancellationToken)
    {
        var baseGames = _sessionService.GetGamesInSessionById(notification.SessionId);
        await _leaderboardService.EvaluateSessionGamesForLeaderboard(baseGames);
    }
}