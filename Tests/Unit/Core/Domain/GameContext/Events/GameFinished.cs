using Image_guesser.Core.Domain.GameContext;
using Image_guesser.Core.Domain.GameContext.Events;

namespace Tests.Unit.Core.Domain.GameContext.Events;

public class GameFinishedTests
{
    [Fact]
    public void ConstructorWithSession_ShouldAssignValues()
    {
        Guid guesserId = Guid.NewGuid();
        BaseGame baseGame = new();
        int points = 1;

        var gameFinished = new GameFinished(baseGame, guesserId, points);

        var now = DateTime.Now;
        Assert.InRange(baseGame.TimeOfCreation, now.AddSeconds(-1), now.AddSeconds(1));

        Assert.Equal(guesserId, gameFinished.GuesserId);
        Assert.Equal(points, gameFinished.Points);
    }
}