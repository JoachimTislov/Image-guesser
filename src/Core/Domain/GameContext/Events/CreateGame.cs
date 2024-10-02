using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext.Events;

public record CreateGame : BaseDomainEvent
{
    public CreateGame(Session session)
    {
        Session = session;
    }

    public Session Session { get; }
}