using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.UserContext;

namespace Image_guesser.Core.Domain.GameContext;

public class Game<TOracle> : BaseGame where TOracle : class
{
    public Oracle<TOracle> Oracle { get; private set; } = default!;

    public Game() { }

    public Game(
        Session session,
        Oracle<TOracle> oracle
    )
    {
        SessionId = session.Id;
        BaseOracleId = oracle.Id;
        GameMode = session.Options.GameMode.ToString();
        Oracle = oracle;

        Guessers.AddRange(CreateGuesserList(Id, session.Options.IsOracleAI(), session.SessionUsers, session.ChosenOracleId));
    }

    private static IEnumerable<Guesser> CreateGuesserList(Guid gameId, bool gameIsAI, List<User> users, Guid chosenOracleId)
    {
        var filteredUsers = !gameIsAI
            ? users.Where(u => u.Id != chosenOracleId)
            : users;

        return filteredUsers.Select(user => CreateGuesser(user.UserName!, gameId));
    }
}