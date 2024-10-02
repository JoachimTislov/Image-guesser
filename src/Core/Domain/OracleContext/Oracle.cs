namespace Image_guesser.Core.Domain.OracleContext;

public class Oracle<TOracle> : BaseOracle where TOracle : class
{
    public Oracle() { }

    public Oracle(TOracle oracle, string imageIdentifier)
    {
        Entity = oracle;
        ImageIdentifier = imageIdentifier;
    }

    public TOracle Entity { get; private set; } = default!;
}