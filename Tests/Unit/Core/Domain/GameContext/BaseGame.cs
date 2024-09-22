using Image_guesser.Core.Domain.GameContext;

namespace Tests.Unit.Core.Domain.GameContext;

public class BaseGameTests
{
    [Fact]
    public void EmptyConstructor_ShouldHaveDefaultValues()
    {
        var game = new BaseGame();

        Assert.Equal(Guid.Empty, game.Id);
        Assert.Equal(Guid.Empty, game.SessionId);
        Assert.Empty(game.Guessers);
        Assert.Equal(string.Empty, game.GameMode);
        Assert.False(game.IsGameOver());

        var now = DateTime.Now;
        Assert.InRange(game.TimeOfCreation, now.AddSeconds(-1), now.AddSeconds(1));
    }
}