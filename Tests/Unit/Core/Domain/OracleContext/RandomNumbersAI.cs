
using Image_guesser.Core.Domain.OracleContext;

namespace Tests.Unit.Core.Domain.OracleContext;

public class RandomNumbersAITests
{
    [Fact]
    public void Constructor_WithNumberArray_ShouldAssignValues()
    {
        int[] numbersForImagePieces = [1, 2, 3];
        AI_Type AIrandomType = AI_Type.Random;
        var randomNumbersAI = new AI(numbersForImagePieces, AIrandomType);

        Assert.Equal(numbersForImagePieces, randomNumbersAI.NumbersForImagePieces);
        Assert.Equal(AIrandomType, randomNumbersAI.AI_Type);
    }

}