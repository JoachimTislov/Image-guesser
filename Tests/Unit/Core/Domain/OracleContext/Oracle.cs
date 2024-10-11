
using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.UserContext;

namespace Tests.Unit.Core.Domain.OracleContext;

public class OracleTests
{
    [Fact]
    public void OracleAssignment_WhenUsingUser_ShouldBeOfTypeUserOracle()
    {
        var user = new User()
        {
            UserName = "TestUser"
        };
        var imageIdentifier = "TestImageIdentifier";
        var userOracle = new Oracle<User>(user, imageIdentifier);

        Assert.IsType<Oracle<User>>(userOracle);
        Assert.Equal(user, userOracle.Entity);
        Assert.Equal(imageIdentifier, userOracle.ImageIdentifier);
    }

    [Fact]
    public void OracleAssignment_WhenUsingAI_ShouldBeOfTypeOracleAI()
    {
        int[] numbersForImagePieces = [1, 2, 3];
        var AI = new AI(numbersForImagePieces, AI_Type.Random);
        var imageIdentifier = "TestImageIdentifier";
        var AIOracle = new Oracle<AI>(AI, imageIdentifier);

        Assert.IsType<Oracle<AI>>(AIOracle);
        Assert.Equal(AI, AIOracle.Entity);
        Assert.Equal(imageIdentifier, AIOracle.ImageIdentifier);
    }
}