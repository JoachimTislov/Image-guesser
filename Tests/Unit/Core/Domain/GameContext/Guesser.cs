using Image_guesser.Core.Domain.GameContext;

namespace Tests.Unit.Core.Domain.GameContext;

public class GuesserTests
{
    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        var name = "TestUser";

        var guesser = new Guesser(name);

        Assert.Equal(name, guesser.Name);
        Assert.Equal(Guid.Empty, guesser.Id);

        Assert.Equal(0, guesser.Points);
        Assert.Equal(0, guesser.Guesses);
        Assert.Equal(0, guesser.WrongGuessCounter);
    }

    [Fact]
    public void Properties_ShouldAllowUpdates()
    {
        var number = 1;
        var name = "TestPlayer";

        var guesser = new Guesser(name)
        {
            Points = number,
        };

        guesser.IncrementGuesses();
        guesser.IncrementWrongGuessCounter();

        Assert.Equal(number, guesser.Points);
        Assert.Equal(number, guesser.Guesses);
        Assert.Equal(number, guesser.WrongGuessCounter);

    }
}