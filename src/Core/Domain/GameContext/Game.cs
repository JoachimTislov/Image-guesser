using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.UserContext;

namespace Image_guesser.Core.Domain.GameContext;

public class Game<TOracle> : BaseGame where TOracle : class
{
    public Game() { }

    public Game(
        Guid sessionId,
        List<User> users,
        string gameMode,
        Oracle<TOracle> oracle
    )
    {
        SessionId = sessionId;
        GameMode = gameMode;
        Oracle = oracle;

        foreach (var user in users)
        {
            var guesser = new Guesser(user.UserName!, Id);
            Guessers.Add(guesser);
        }
    }

    public Oracle<TOracle> Oracle { get; set; } = default!;
}