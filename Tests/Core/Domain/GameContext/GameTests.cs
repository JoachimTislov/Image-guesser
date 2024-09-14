using Image_guesser.Core.Domain.GameContext;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.UserContext;

namespace Tests.Core.Domain.GameContext;

public class GameTests
{
    [Fact]
    public void EmptyConstructor_ShouldHaveDefaultValues()
    {
        var game = new Game();

        Assert.Equal(Guid.Empty, game.Id);
        Assert.Equal(Guid.Empty, game.SessionId);
        Assert.Equal(Guid.Empty, game.OracleId);
        Assert.Empty(game.Guessers);
        Assert.Equal(string.Empty, game.GameMode);
        Assert.False(game.OracleIsAI);
        Assert.False(game.IsGameOver());

        var now = DateTime.Now;
        Assert.InRange(game.TimeOfCreation, now.AddSeconds(-1), now.AddSeconds(1));
    }
    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        var sessionId = Guid.NewGuid();
        var oracleId = Guid.NewGuid();
        var oracleIsAI = false;
        var gameMode = GameMode.SinglePlayer.ToString();

        User user = new()
        {
            UserName = "Peddi"
        };
        List<User> users = [user];

        var game = new Game(sessionId, users, gameMode, oracleId, oracleIsAI);

        Assert.Equal(sessionId, game.SessionId);
        Assert.Single(game.Guessers);
        Assert.Equal(game.Id, game.Guessers[0].GameId);
        Assert.Equal(gameMode, game.GameMode);
        Assert.Equal(oracleId, game.OracleId);
        Assert.Equal(oracleIsAI, game.OracleIsAI);
    }
}