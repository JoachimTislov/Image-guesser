
using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.SessionContext.ViewModels;

namespace Tests.Unit.Core.Domain.SessionContext.ViewModels;

public class ViewModelOptionsTests
{
    [Fact]
    public void ConstructorWithOptions_ShouldHaveTheSameInitializedValues()
    {

        var Options = new Options();
        var ViewModelOptions = new ViewModelOptions(Options);

        var number = 1;

        Assert.Equal(number, ViewModelOptions.LobbySize);
        Assert.True(ViewModelOptions.IsGameMode(GameMode.SinglePlayer));

        Assert.True(ViewModelOptions.IsOracleAI());
        Assert.Equal(AI_Type.Random, ViewModelOptions.AI_Type);

        Assert.Equal(PictureMode.Random, ViewModelOptions.PictureMode);
    }
}