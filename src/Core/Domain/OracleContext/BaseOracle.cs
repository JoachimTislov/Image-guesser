using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.OracleContext;

public class BaseOracle : BaseEntity
{
    public BaseOracle() { }

    public Guid Id { get; private set; }
    public int TotalGuesses { get; private set; }
    public int NumberOfTilesRevealed { get; private set; }
    public string ImageIdentifier { get; protected set; } = string.Empty;

    public void IncrementTiles()
    {
        NumberOfTilesRevealed++;
    }
    public void IncrementGuesses()
    {
        TotalGuesses++;
    }
}
