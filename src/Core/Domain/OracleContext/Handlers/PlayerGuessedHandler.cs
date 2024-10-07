using Image_guesser.Core.Domain.GameContext.Events;
using Image_guesser.Core.Domain.ImageContext.Services;
using Image_guesser.Core.Domain.OracleContext.Services;
using Image_guesser.Infrastructure.GenericRepository;
using MediatR;

namespace Image_guesser.Core.Domain.OracleContext.Handlers;

public class PlayerGuessedHandler(IRepository repository, IImageService imageService, IOracleService oracleService, IMediator mediator) : INotificationHandler<PlayerGuessed>
{
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IImageService _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
    private readonly IOracleService _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));


    public async Task Handle(PlayerGuessed notification, CancellationToken cancellationToken)
    {
        var oracle = await _oracleService.GetBaseOracleById(notification.OracleId);
        oracle.IncrementGuesses();
        await _repository.Update(oracle);

        var ImageRecord = await _imageService.GetImageRecordById(oracle.ImageIdentifier);

        if (_oracleService.CheckGuess(notification.Guess, ImageRecord.Name))
        {
            //Calculating amount of unrevealed tiles
            var points = _oracleService.CalculatePoints(ImageRecord.PieceCount, oracle.NumberOfTilesRevealed);

            await _mediator.Publish(new PlayerGuessedCorrectly(notification.GuesserId, points, notification.GameId), cancellationToken);
            await _mediator.Publish(new GameFinished(notification.GameId, notification.SessionId));
        }
        else
        {
            await _mediator.Publish(new PlayerGuessedIncorrectly(notification.GuesserId, notification.GameId), cancellationToken);
        }
    }
}