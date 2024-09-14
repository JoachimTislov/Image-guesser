
using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.UserContext;

namespace Tests.Core.Domain.OracleContext;

public class GenericOracleTests
{
    [Fact]
    public void OracleAssignment_WhenUsingUser_ShouldBeOfTypeGenericOracle()
    {
        var username = "TestUser";
        var user = new User()
        {
            UserName = username
        };
        var userOracle = new GenericOracle<User>(user);

        Assert.IsType<GenericOracle<User>>(userOracle);
        Assert.Equal(username, userOracle.Oracle.UserName);
    }

    [Fact]
    public void OracleAssignment_WhenUsingRandomNumbersAI_ShouldBeOfTypeGenericOracle()
    {
        var randomNumbersAI = new RandomNumbersAI();
        var randomNumbersAIOracle = new GenericOracle<RandomNumbersAI>(randomNumbersAI);

        Assert.IsType<GenericOracle<RandomNumbersAI>>(randomNumbersAIOracle);
        Assert.Empty(randomNumbersAIOracle.Oracle.NumbersForImagePieces);
    }
}