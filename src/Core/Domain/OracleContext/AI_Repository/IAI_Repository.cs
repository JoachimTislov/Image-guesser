namespace Image_guesser.Core.Domain.OracleContext.AI_Repository;

public interface IAI_Repository
{
    AI CreateRandomNumbersAI(int ImagePieceCount);
}