using Image_guesser.Core.Domain.GameContext;

namespace Tests.Unit.Core.Domain.GameContext.Events;

public class GameFinishedTests
{
    [Fact]
    public void ConstructorWithSession_ShouldAssignValues()
    {
        var GameId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();

        var gameFinished = new GameFinished(GameId, sessionId);

        Assert.Equal(GameId, gameFinished.GameId);
        Assert.Equal(sessionId, gameFinished.SessionId);
    }
}