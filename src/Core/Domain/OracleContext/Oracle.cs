using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.OracleContext;

// Values common for all generic Oracles
public class BaseOracle() : BaseEntity
{
    public Guid Id { get; set; }
    public int TotalGuesses { get; set; }
    public int NumberOfTilesRevealed { get; set; }
    public string ImageIdentifier { get; set; } = null!;
    public void AssignImageId(string imageIdentifier)
    {
        ImageIdentifier = imageIdentifier;
    }
}
