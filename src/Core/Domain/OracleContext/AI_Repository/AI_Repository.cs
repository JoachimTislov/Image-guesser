
namespace Image_guesser.Core.Domain.OracleContext.AI_Repository;

public class AI_Repository : IAI_Repository
{
    private static int[] GetEnumerableArray(int ImagePieceCount) => Enumerable.Range(0, ImagePieceCount).ToArray();

    /*This method generates an array of random numbers,  
    where each number in the row represents a piece of an image.*/
    public AI CreateRandomNumbersAI(int ImagePieceCount)
    {
        // Enumerates the numbers from 0 to the number of image pieces/tiles in the image
        // Creates an array of the numbers
        int[] ArrayOfNumbers = GetEnumerableArray(ImagePieceCount);

        // Shuffles the array of numbers

        //****  Fisher-Yates shuffle algorithm ****\\\\
        int LengthOfArray = ArrayOfNumbers.Length;

        Random random = new();

        while (LengthOfArray > 0)
        {
            int randomNumber = random.Next(LengthOfArray--);

            // Swaps the numbers in the array
            (ArrayOfNumbers[randomNumber], ArrayOfNumbers[LengthOfArray]) = (ArrayOfNumbers[LengthOfArray], ArrayOfNumbers[randomNumber]);
        }
        ////*******************************************\\\\

        return new AI(ArrayOfNumbers, AI_Type.Random);
    }

    public AI CreateIncrementalAI(int ImagePieceCount)
    {
        // Enumerates the numbers from 0 to the number of image pieces/tiles in the image
        int[] ArrayOfNumbers = GetEnumerableArray(ImagePieceCount);

        return new AI(ArrayOfNumbers, AI_Type.Incremental);
    }

    public AI CreateDecrementalAI(int ImagePieceCount)
    {
        // Enumerates the numbers from 0 to the number of image pieces/tiles in the image
        int[] ArrayOfNumbers = GetEnumerableArray(ImagePieceCount).Reverse().ToArray();

        return new AI(ArrayOfNumbers, AI_Type.Decremental);
    }

    public AI CreateOddEvenOrderAI(int ImagePieceCount)
    {
        // Enumerates the numbers from 0 to the number of image pieces/tiles in the image
        int[] ArrayOfNumbers = GetEnumerableArray(ImagePieceCount);

        // Splits the array of numbers into two arrays: one for odd numbers and one for even numbers
        int[] OddNumbers = ArrayOfNumbers.Where(x => x % 2 != 0).ToArray();
        int[] EvenNumbers = ArrayOfNumbers.Where(x => x % 2 == 0).ToArray();

        // Merges the two arrays into one array
        int[] MergedArray = [.. OddNumbers, .. EvenNumbers];

        return new AI(MergedArray, AI_Type.OddEvenOrder);
    }

    public AI CreateEvenOddOrderAI(int ImagePieceCount)
    {
        var OddEvenOrder = CreateOddEvenOrderAI(ImagePieceCount);
        return new AI(OddEvenOrder.NumbersForImagePieces.Reverse().ToArray(), AI_Type.EvenOddOrder);
    }
}