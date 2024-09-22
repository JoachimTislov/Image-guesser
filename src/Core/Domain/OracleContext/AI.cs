using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.OracleContext;

/* The AI class holds an array of integers and is used 
to determine the sequence in which tiles are revealed. */

public class AI : BaseEntity
{
    public AI() { }

    public AI(int[] numbersForImagePieces, AI_Type type)
    {
        NumbersForImagePieces = numbersForImagePieces;
        AI_Type = type;
    }
    public Guid Id { get; set; }
    public int[] NumbersForImagePieces { get; private set; } = [];
    public AI_Type AI_Type { get; set; }
}

