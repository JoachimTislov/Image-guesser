namespace Image_guesser.Core.Domain.OracleContext.AI_Repository;

public interface IAI_Repository
{
    AI CreateRandomNumbersAI(int ImagePieceCount);

    AI CreateIncrementalAI(int ImagePieceCount);

    AI CreateDecrementalAI(int ImagePieceCount);

    AI CreateOddEvenOrderAI(int ImagePieceCount);

    AI CreateEvenOddOrderAI(int ImagePieceCount);
}