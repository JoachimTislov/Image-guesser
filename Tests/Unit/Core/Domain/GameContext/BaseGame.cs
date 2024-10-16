using Image_guesser.Core.Domain.GameContext;
using Image_guesser.Core.Domain.SessionContext;

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
        Assert.Equal(GameMode.SinglePlayer, game.GameMode);
        Assert.False(game.IsFinished());
    }
}