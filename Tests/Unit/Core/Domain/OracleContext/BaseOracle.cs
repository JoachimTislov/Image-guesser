
using Image_guesser.Core.Domain.OracleContext;

namespace Tests.Unit.Core.Domain.OracleContext;

public class BaseOracleTests
{
    [Fact]
    public void EmptyConstructor_ShouldHaveInitializedValues()
    {
        var baseOracle = new BaseOracle();

        Assert.Equal(Guid.Empty, baseOracle.Id);
        Assert.Equal(0, baseOracle.TotalGuesses);
        Assert.Equal(0, baseOracle.NumberOfTilesRevealed);
        Assert.Equal(string.Empty, baseOracle.ImageIdentifier);
    }

    [Fact]
    public void AssignImageId_ShouldAssignAStringToImageIdentifier()
    {
        var baseOracle = new BaseOracle();

        var imageIdentifier = "ImageTest";
        baseOracle.AssignImageId(imageIdentifier);

        Assert.Equal(imageIdentifier, baseOracle.ImageIdentifier);
    }

    [Fact]
    public void Properties_ShouldAllowUpdates()
    {
        var number = 1;
        var baseOracle = new BaseOracle()
        {
            TotalGuesses = number,
            NumberOfTilesRevealed = number,
        };

        Assert.Equal(number, baseOracle.TotalGuesses);
        Assert.Equal(number, baseOracle.NumberOfTilesRevealed);

    }

}