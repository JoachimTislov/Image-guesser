using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.OracleContext;

public class BaseOracle() : BaseEntity
{
    public Guid Id { get; set; }
    public int TotalGuesses { get; set; }
    public int NumberOfTilesRevealed { get; set; }
    public string ImageIdentifier { get; private set; } = string.Empty;
    public void AssignImageId(string imageIdentifier)
    {
        ImageIdentifier = imageIdentifier;
    }
}
