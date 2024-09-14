using Image_guesser.Core.Domain.GameContext.Events;

namespace Tests.Core.Domain.GameContext.Events;

public class GameFinishedTests
{
    [Fact]
    public void ConstructorWithSession_ShouldAssignValues()
    {
        var gameId = Guid.NewGuid();
        var gameFinished = new GameFinished(gameId);

        Assert.Equal(gameId, gameFinished.GameId);
    }
}