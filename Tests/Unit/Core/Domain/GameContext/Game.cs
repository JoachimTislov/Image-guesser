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
        Assert.Equal(GameMode.SinglePlayer, game.GameMode);
        Assert.False(game.IsFinished());

        Assert.IsType<Game<object>>(game);

        //Assert.IsType<Oracle<object>>(game.Oracle);
        Assert.Null(game.Oracle);
    }

    [Fact]
    public void ConstructorWithOracleTypeUser_ShouldSetPropertiesCorrectly()
    {
        var sessionId = Guid.NewGuid();

        User user = new()
        {
            UserName = "Peddi"
        };
        var session = new Session(user, sessionId);

        var imageIdentifier = "imageIdentifier";
        var oracle = new Oracle<User>(user, imageIdentifier);

        var game = new Game<User>(session, oracle);

        Assert.Equal(sessionId, game.SessionId);
        Assert.Single(game.Guessers);
        Assert.Equal(session.Options.GameMode, game.GameMode);
        Assert.Equal(user.Id, game.Oracle.Entity.Id);
        Assert.Equal(imageIdentifier, game.Oracle.ImageIdentifier);

        Assert.IsType<Game<User>>(game);
        Assert.IsType<Oracle<User>>(game.Oracle);
    }

    [Fact]
    public void ConstructorWithOracleTypeAI_ShouldSetPropertiesCorrectly()
    {
        var sessionId = Guid.NewGuid();
        User user = new()
        {
            UserName = "Peddi"
        };
        var session = new Session(user, sessionId);

        int[] numbersForImagePieces = [1, 2, 3];
        var AI = new AI(numbersForImagePieces, AI_Type.Random);

        var imageIdentifier = "imageIdentifier";
        var oracle = new Oracle<AI>(AI, imageIdentifier);

        var game = new Game<AI>(session, oracle);

        Assert.Equal(sessionId, game.SessionId);
        Assert.Single(game.Guessers);
        Assert.Equal(user.UserName, game.Guessers[0].Name);
        Assert.Equal(session.Options.GameMode, game.GameMode);
        Assert.Equal(imageIdentifier, game.Oracle.ImageIdentifier);

        // Assert Oracle is created correctly
        Assert.Equal(numbersForImagePieces, game.Oracle.Entity.NumbersForImagePieces);

        Assert.Equal(AI.Id, game.Oracle.Entity.Id);

        Assert.IsType<Game<AI>>(game);
        Assert.IsType<Oracle<AI>>(game.Oracle);
    }
}