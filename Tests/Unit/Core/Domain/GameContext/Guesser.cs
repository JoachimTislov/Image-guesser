using Image_guesser.Core.Domain.GameContext;

namespace Tests.Unit.Core.Domain.GameContext;

public class GuesserTests
{
    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {

        var name = "TestUser";
        var gameId = Guid.NewGuid();

        var guesser = new Guesser(name, gameId);

        Assert.Equal(name, guesser.Name);
        Assert.Equal(gameId, guesser.GameId);

        // Guid is generated when added to the database
        Assert.Equal(Guid.Empty, guesser.Id);

        Assert.Equal(0, guesser.Points);
        Assert.Equal(0, guesser.Guesses);
        Assert.Equal(0, guesser.WrongGuessCounter);
        Assert.Equal(TimeSpan.Zero, guesser.TimeSpan);
    }

    [Fact]
    public void Properties_ShouldAllowUpdates()
    {
        var number = 1;
        var name = "TestPlayer";
        var gameId = Guid.NewGuid();
        var guesser = new Guesser(name, gameId)
        {
            Points = number,
            Guesses = number,
            WrongGuessCounter = number,
            TimeSpan = TimeSpan.FromMinutes(number)
        };

        Assert.Equal(number, guesser.Points);
        Assert.Equal(number, guesser.Guesses);
        Assert.Equal(number, guesser.WrongGuessCounter);
        Assert.Equal(TimeSpan.FromMinutes(number), guesser.TimeSpan);

    }
}