
namespace Image_guesser.Core.Domain.OracleContext;

// Inherit all common properties between the generic_Oracles from BaseOracle

// All of the dynamic data is stored in the generic_Oracles
// Because that is not known at compile time, the user choose which type of oracle to use

public class GenericOracle<T>(T oracle) : BaseOracle where T : class
{
    public T Oracle { get; set; } = oracle;
}