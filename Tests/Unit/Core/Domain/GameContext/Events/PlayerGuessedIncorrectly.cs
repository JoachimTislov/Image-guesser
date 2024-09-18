using Image_guesser.Core.Domain.GameContext.Events;

namespace Tests.Unit.Core.Domain.GameContext.Events;

public class PlayerGuessedIncorrectlyTests
{
    [Fact]
    public void ConstructorWithSession_ShouldAssignValues()
    {
        var gameId = Guid.NewGuid();
        var guesserId = Guid.NewGuid();

        var playerGuessedIncorrectly = new PlayerGuessedIncorrectly(guesserId, gameId);

        Assert.Equal(guesserId, playerGuessedIncorrectly.GuesserId);
        Assert.Equal(gameId, playerGuessedIncorrectly.GameId);
    }
}