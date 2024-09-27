using Image_guesser.Core.Domain.SessionContext.Pipelines;
using Image_guesser.Core.Domain.GameContext.Events;
using MediatR;

namespace Image_guesser.Core.Domain.SessionContext.Handlers;

public class CreateGameHandler(IMediator mediator) : INotificationHandler<CreateGame>
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public async Task Handle(CreateGame notification, CancellationToken cancellationToken)
    {
        var session = await _mediator.Send(new GetSessionById.Request(notification.SessionId), cancellationToken);

        if (session != null)
        {
            session.SessionStatus = SessionStatus.InGame;
            session.Options.DecrementNumberOfRounds();
        }
    }
}