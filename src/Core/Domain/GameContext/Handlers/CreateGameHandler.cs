using MediatR;
using Image_guesser.Core.Domain.GameContext.Services;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.OracleContext.Services;
using Image_guesser.Core.Domain.ImageContext.Services;

namespace Image_guesser.Core.Domain.GameContext.Handlers;

public class CreateGameHandler(IGameService gameService, ISessionService sessionService, IOracleService oracleService, IImageService imageService) : INotificationHandler<CreateGame>
{
    private readonly IGameService _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    private readonly IOracleService _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
    private readonly IImageService _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));


    public async Task Handle(CreateGame notification, CancellationToken cancellationToken)
    {
        var (SessionId, ImageIdentifier) = notification;

        // No reason to check if the picture mode is random or specific, if the image identifier is null, then we choose a random
        var pastIdentifiers = _oracleService.GetImageIdentifierOfAllPreviousPlayedGamesInTheSession(SessionId);
        var ImageId = ImageIdentifier ?? await _imageService.GetDifferentRandomImageIdentifier(pastIdentifiers);

        var session = await _sessionService.GetSessionById(SessionId);

        await _gameService.CreateGame(session, ImageId);
    }
}