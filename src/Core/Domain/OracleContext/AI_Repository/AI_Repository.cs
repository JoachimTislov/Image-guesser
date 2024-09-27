namespace Image_guesser.Core.Domain.OracleContext.AI_Repository;

public class AI_Repository : IAI_Repository
{
    /*This method generates an array of random numbers,  
    where each number in the row represents a piece of that image.*/
    public AI CreateRandomNumbersAI(int ImagePieceCount)
    {
        // Enumerates the numbers from 1 to the number of image pieces/tiles in the image
        // Creates an array of the numbers
        int[] ArrayOfNumbers = Enumerable.Range(1, ImagePieceCount).ToArray();

        // Shuffles the array of numbers

        //****  Fisher-Yates shuffle algorithm ****\\\\
        int LengthOfArray = ArrayOfNumbers.Length;

        // Creates a random object
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

}