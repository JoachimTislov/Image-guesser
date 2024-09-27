using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext.Events;

public record CreateGame : BaseDomainEvent
{
    public CreateGame(Session session)
    {
        SessionId = session.Id;
        ChosenOracleId = session.ChosenOracleId;
        Users = session.SessionUsers;
        Options = session.Options;
    }

    public Guid SessionId { get; }
    public Guid ChosenOracleId { get; }
    public List<User> Users { get; }
    public Options Options { get; }
}