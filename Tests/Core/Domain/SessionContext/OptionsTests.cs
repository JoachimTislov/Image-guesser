using Image_guesser.Core.Domain.SessionContext;

namespace Tests.Core.Domain.SessionContext;

public class OptionsTests
{
    [Fact]
    public void EmptyConstructor_ShouldHaveInitializedValues()
    {
        var Options = new Options();

        Assert.Equal(1, Options.NumberOfRounds);
        Assert.Equal(1, Options.LobbySize);
        Assert.Equal(GameMode.SinglePlayer, Options.GameMode);
        Assert.True(Options.RandomPictureMode);
        Assert.False(Options.RandomOracle);
        Assert.True(Options.UseAI);

    }

    [Fact]
    public void Properties_ShouldAllowUpdates()
    {
        var number = 1;
        var Options = new Options()
        {
            NumberOfRounds = number,
            LobbySize = number,
            GameMode = GameMode.Duo,
            RandomPictureMode = false,
            RandomOracle = true,
            UseAI = false,
        };

        Assert.Equal(number, Options.NumberOfRounds);
        Assert.Equal(number, Options.LobbySize);
        Assert.Equal(GameMode.Duo, Options.GameMode);
        Assert.False(Options.RandomPictureMode);
        Assert.True(Options.RandomOracle);
        Assert.False(Options.UseAI);

    }
}