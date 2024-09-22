using Image_guesser.Core.Domain.GameContext.Events;
using Image_guesser.Core.Domain.GameContext.Services;
using Image_guesser.Core.Domain.ImageContext.Services;
using Image_guesser.Core.Domain.OracleContext.Services;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Infrastructure;
using MediatR;

namespace Image_guesser.Core.Domain.OracleContext.Handlers;

public class PlayerGuessedHandler(ImageGameContext db, IImageService imageService, IGameService gameService, ISessionService sessionService, IOracleService oracleService) : INotificationHandler<PlayerGuessed>
{
    private readonly ImageGameContext _db = db ?? throw new ArgumentNullException(nameof(db));
    private readonly IImageService _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
    private readonly IGameService _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    private readonly IOracleService _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));

    public async Task Handle(PlayerGuessed notification, CancellationToken cancellationToken)
    {
        var OracleIsAI = await _sessionService.CheckIfOracleIsAI(notification.SessionId);

        Guid OracleId;

        if (OracleIsAI)
        {
            OracleId = (await _gameService.GetGameById<AI>(notification.GameId)).Oracle.Entity.Id;
        }
        else
        {
            OracleId = (await _gameService.GetGameById<User>(notification.GameId)).Oracle.Entity.Id;
        }

        var Oracle = await _oracleService.GetBaseOracleById(OracleId);

        Oracle.IncrementGuesses();
        _db.Oracles.Update(Oracle);

        var ImageRecord = await _imageService.GetImageRecordById(Oracle.ImageIdentifier);
        var game = await _gameService.GetBaseGameById(notification.GameId);
        if (ImageRecord.Name == notification.Guess)
        {
            //Calculating amount of unrevealed tiles
            var points = ImageRecord.PieceCount - Oracle.NumberOfTilesRevealed;

            game.Events.Add(new GameFinished(game, Guid.Parse(notification.GuesserId), points));
        }
        else
        {
            game.Events.Add(new PlayerGuessedIncorrectly(Guid.Parse(notification.GuesserId), notification.GameId));
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}