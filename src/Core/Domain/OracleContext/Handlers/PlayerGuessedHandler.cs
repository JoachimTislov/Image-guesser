using Image_guesser.Core.Domain.GameContext;
using Image_guesser.Core.Domain.ImageContext.Services;
using Image_guesser.Core.Domain.OracleContext.Repositories.Repository;
using Image_guesser.Core.Domain.OracleContext.Services;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.SignalRContext.Services.Hub;
using MediatR;

namespace Image_guesser.Core.Domain.OracleContext.Handlers;

public class PlayerGuessedHandler(ISessionService sessionService, IOracleRepository oracleRepository, IImageService imageService, IOracleService oracleService, IMediator mediator, IHubService hubService) : INotificationHandler<PlayerGuessed>
{
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    private readonly IOracleRepository _oracleRepository = oracleRepository ?? throw new ArgumentNullException(nameof(oracleRepository));
    private readonly IImageService _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
    private readonly IOracleService _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly IHubService _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));

    public async Task Handle(PlayerGuessed notification, CancellationToken cancellationToken)
    {
        var (imageIdentifier, oracleId, guess, _, _, sessionId, userName, _) = notification;

        var oracle = await _oracleService.GetBaseOracleById(oracleId);
        oracle.IncrementGuesses();
        await _oracleService.UpdateOracle(oracle);

        var ImageRecord = await _imageService.GetImageRecordById(oracle.ImageIdentifier);

        var session = await _sessionService.GetSessionById(sessionId);
        var (IsGuessCorrect, WinnerText, nameOfImage) = await _oracleService.HandleGuess(guess, imageIdentifier, userName, session.ChosenOracleId, session.Options.GameMode);
        if (IsGuessCorrect)
        {
            await _hubService.AlertGroupCorrectGuess(sessionId.ToString(), WinnerText, guess, nameOfImage);

            //Calculating amount of unrevealed tiles
            var points = _oracleService.CalculatePoints(ImageRecord.PieceCount, oracle.NumberOfTilesRevealed);

            await _mediator.Publish(new PlayerGuessedCorrectly(notification.GuesserId, points, notification.GameId, notification.SessionId), cancellationToken);
        }
        else
        {
            await _mediator.Publish(new PlayerGuessedIncorrectly(notification.GuesserId, notification.GameId), cancellationToken);
        }
    }
}