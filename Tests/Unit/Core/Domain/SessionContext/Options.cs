using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.SessionContext.ViewModels;

namespace Tests.Unit.Core.Domain.SessionContext;

public class OptionsTests
{
    [Fact]
    public void EmptyConstructor_ShouldHaveInitializedValues()
    {
        var Options = new Options();

        Assert.Equal(1, Options.LobbySize);
        Assert.Equal(GameMode.SinglePlayer, Options.GameMode);
        Assert.True(Options.IsPictureMode(PictureMode.Random));
        Assert.True(Options.IsOracleAI());

    }

    [Fact]
    public void SetOptionsValues_ShouldUpdateValuesCorrectly_WithViewModelOptionsAsParameter()
    {
        var number = 1;
        var Options = new Options();

        var ViewModelOptions = new ViewModelOptions()
        {
            LobbySize = number,
            GameMode = GameMode.Duo,
            PictureMode = PictureMode.Specific,
            OracleType = OracleTypes.User,
            AI_Type = AI_Type.Random
        };

        Options.SetOptionsValues(ViewModelOptions);

        // Since it duo, should probably test the different scenarios as well
        Assert.Equal(2, Options.LobbySize);

        Assert.Equal(GameMode.Duo, Options.GameMode);

        Assert.True(Options.IsPictureMode(PictureMode.Specific));

        Assert.True(!Options.IsGameMode(GameMode.SinglePlayer));
        Assert.True(!Options.IsOracleAI());
    }
}