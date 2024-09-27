
using Image_guesser.Core.Domain.GameContext.Events;
using Image_guesser.Core.Domain.SessionContext;

namespace Tests.Unit.Core.Domain.GameContext.Events;

public class CreateGameTests
{
    [Fact]
    public void ConstructorWithSession_ShouldAssignValues()
    {
        var session = new Session();
        var createGame = new CreateGame(session);

        Assert.Equal(session.Id, createGame.SessionId);
        Assert.Equal(session.ChosenOracleId, createGame.ChosenOracleId);

        // Assert options, probably unnecessary, since it checked in namespace Tests.Unit.Core.Domain.SessionContext
        Assert.Equal(1, createGame.Options.NumberOfRounds);
        Assert.Equal(1, createGame.Options.LobbySize);
        Assert.Equal(GameMode.SinglePlayer, createGame.Options.GameMode);
        Assert.True(createGame.Options.RandomPictureMode);
        Assert.False(createGame.Options.RandomUserOracle);
        Assert.True(createGame.Options.IsOracleAI());

        Assert.Empty(createGame.Users);

        Assert.Equal(string.Empty, createGame.ImageIdentifier);
    }
}