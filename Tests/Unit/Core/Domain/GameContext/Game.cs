using Image_guesser.Core.Domain.GameContext;
using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.UserContext;

namespace Tests.Unit.Core.Domain.GameContext;

public class GameTests
{
    [Fact]
    public void EmptyConstructorWithOracleTypeObject_ShouldHaveDefaultValues()
    {
        var game = new Game<object>();

        Assert.Equal(Guid.Empty, game.Id);
        Assert.Equal(Guid.Empty, game.SessionId);
        Assert.Empty(game.Guessers);
        Assert.Equal(string.Empty, game.GameMode);
        Assert.False(game.IsFinished);

        Assert.IsType<Game<object>>(game);

        //Assert.IsType<Oracle<object>>(game.Oracle);
        Assert.Null(game.Oracle);

        var now = DateTime.Now;
        Assert.InRange(game.TimeOfCreation, now.AddSeconds(-1), now.AddSeconds(1));
    }

    [Fact]
    public void ConstructorWithOracleTypeUser_ShouldSetPropertiesCorrectly()
    {
        var sessionId = Guid.NewGuid();
        var gameMode = GameMode.SinglePlayer.ToString();

        User user = new()
        {
            UserName = "Peddi"
        };
        List<User> users = [user];

        var oracle = new Oracle<User>(user);

        var game = new Game<User>(sessionId, users, gameMode, oracle);

        Assert.Equal(sessionId, game.SessionId);
        Assert.Single(game.Guessers);
        Assert.Equal(game.Id, game.Guessers[0].GameId);
        Assert.Equal(gameMode, game.GameMode);
        Assert.Equal(user.Id, game.Oracle.Id);

        Assert.IsType<Game<User>>(game);
        Assert.IsType<Oracle<User>>(game.Oracle);
    }

    [Fact]
    public void ConstructorWithOracleTypeAI_ShouldSetPropertiesCorrectly()
    {
        var sessionId = Guid.NewGuid();
        var gameMode = GameMode.SinglePlayer.ToString();

        User user = new()
        {
            UserName = "Peddi"
        };
        List<User> users = [user];

        int[] numbersForImagePieces = [1, 2, 3];
        var AI = new AI(numbersForImagePieces, AI_Type.Random);

        var oracle = new Oracle<AI>(AI);

        var game = new Game<AI>(sessionId, users, gameMode, oracle);

        Assert.Equal(sessionId, game.SessionId);
        Assert.Single(game.Guessers);
        Assert.Equal(user.UserName, game.Guessers[0].Name);
        Assert.Equal(gameMode, game.GameMode);

        // Assert Oracle is created correctly
        Assert.Equal(numbersForImagePieces, game.Oracle.Entity.NumbersForImagePieces);

        // ??
        Assert.Equal(game.Oracle.Id, game.Oracle.Entity.Id);

        Assert.Equal(AI.Id, game.Oracle.Entity.Id);

        Assert.IsType<Game<AI>>(game);
        Assert.IsType<Oracle<AI>>(game.Oracle);
    }
}