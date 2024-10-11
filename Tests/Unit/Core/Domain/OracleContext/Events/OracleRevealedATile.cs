
using Image_guesser.Core.Domain.OracleContext;

namespace Tests.Unit.Core.Domain.OracleContext.Events;

public class OracleRevealedATileTests
{
    [Fact]
    public void ConstructorWithOracleId_ShouldAssignTheId()
    {
        var oracleId = Guid.NewGuid();
        var imageIdentifier = "imageIdentifier";
        var oracleRevealedATile = new OracleRevealedATile(oracleId, imageIdentifier);

        Assert.Equal(oracleId, oracleRevealedATile.OracleId);
        Assert.Equal(imageIdentifier, oracleRevealedATile.ImageId);
    }

}