using Image_guesser.Core.Domain.GameContext.Events;
using Image_guesser.Core.Domain.GameContext.Pipelines;
using MediatR;

namespace Image_guesser.Core.Domain.GameContext.Handlers;

public class PlayerGuessedIncorrectlyHandler(IMediator mediator) : INotificationHandler<PlayerGuessedIncorrectly>
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public async Task Handle(PlayerGuessedIncorrectly notification, CancellationToken cancellationToken)
    {
        var guesser = await _mediator.Send(new GetGuesserById.Request(notification.GuesserId), cancellationToken);

        guesser.Guesses++;

        if (guesser.WrongGuessCounter == 3)
        {
            // Oracles Turn
            // await _mediator.Publish(new OraclesTurn(notification.GameId), cancellationToken);

            // This might be written too soon
            guesser.WrongGuessCounter = 0;
        }
        else
        {
            guesser.WrongGuessCounter++;
        }

        await _mediator.Send(new UpdateGuesserStats.Request(
        notification.GameId, guesser, guesser.TimeSpan),
        cancellationToken);
    }
}
