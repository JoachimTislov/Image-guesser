
using Microsoft.EntityFrameworkCore;

namespace Image_guesser.Core.Domain.OracleContext;

// OracleAI is a class that stores an array of integers,
// which are used to determine which image pieces/tiles the AI should reveal

[Keyless]
public class RandomNumbersAI
{
    public RandomNumbersAI() { }
    public RandomNumbersAI(int[] numbersForImagePieces)
    {
        NumbersForImagePieces = numbersForImagePieces;
    }
    public int[]? NumbersForImagePieces { get; set; }
}

