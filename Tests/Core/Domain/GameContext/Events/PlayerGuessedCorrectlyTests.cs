using Image_guesser.Core.Domain.GameContext.Events;
using Image_guesser.Core.Domain.SessionContext;

namespace Tests.Core.Domain.GameContext.Events;

public class PlayerGuessedCorrectlyPlayerGuessedCorrectlyTests
{
    [Fact]
    public void ConstructorWithSession_ShouldAssignValues()
    {
        var points = 1;
        var gameId = Guid.NewGuid();
        var guesserId = Guid.NewGuid();
        var gameMode = GameMode.SinglePlayer.ToString();

        var playerGuessedCorrectly = new PlayerGuessedCorrectly(points, gameId, guesserId, gameMode);

        Assert.Equal(points, playerGuessedCorrectly.Points);
        Assert.Equal(gameId, playerGuessedCorrectly.GameId);
        Assert.Equal(guesserId, playerGuessedCorrectly.GuesserId);
        Assert.Equal(gameMode, playerGuessedCorrectly.GameMode);
    }
}