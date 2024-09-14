
using Image_guesser.Core.Domain.OracleContext;

namespace Tests.Core.Domain.OracleContext;

public class RandomNumbersAITests
{
    [Fact]
    public void EmptyConstructor_ShouldHaveInitializedValues()
    {
        var randomNumbersAI = new RandomNumbersAI();

        Assert.Empty(randomNumbersAI.NumbersForImagePieces);
    }

    [Fact]
    public void Constructor_WithNumberArray_ShouldAssignValues()
    {
        var randomNumbersAI = new RandomNumbersAI([1, 2, 3]);

        Assert.NotEmpty(randomNumbersAI.NumbersForImagePieces);
    }

}