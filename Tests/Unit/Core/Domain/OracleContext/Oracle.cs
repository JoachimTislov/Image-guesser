
using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.UserContext;

namespace Tests.Unit.Core.Domain.OracleContext;

public class OracleTests
{
    [Fact]
    public void OracleAssignment_WhenUsingUser_ShouldBeOfTypeUserOracle()
    {
        var username = "TestUser";
        var user = new User()
        {
            UserName = username
        };
        var userOracle = new Oracle<User>(user);

        Assert.IsType<Oracle<User>>(userOracle);
        Assert.Equal(username, userOracle.Entity.UserName);
    }

    [Fact]
    public void OracleAssignment_WhenUsingAI_ShouldBeOfTypeOracleAI()
    {
        int[] numbersForImagePieces = [1, 2, 3];
        var AI = new AI(numbersForImagePieces, AI_Type.Random);
        var AIOracle = new Oracle<AI>(AI);

        Assert.IsType<Oracle<AI>>(AIOracle);
        Assert.Equal(numbersForImagePieces, AIOracle.Entity.NumbersForImagePieces);
    }
}