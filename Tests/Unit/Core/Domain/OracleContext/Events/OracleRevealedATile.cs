using Image_guesser.Core.Domain.OracleContext.Events;

namespace Tests.Unit.Core.Domain.OracleContext.Events;

public class OracleRevealedATileTests
{
    [Fact]
    public void ConstructorWithOracleId_ShouldAssignTheId()
    {
        var oracleId = Guid.NewGuid();
        var oracleRevealedATile = new OracleRevealedATile(oracleId);

        Assert.Equal(oracleId, oracleRevealedATile.OracleId);
    }

}