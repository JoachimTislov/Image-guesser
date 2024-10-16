using Image_guesser.Core.Domain.ImageContext.Services;
using Image_guesser.Core.Domain.OracleContext.AI_Repository;
using Image_guesser.Core.Domain.OracleContext.Repositories.Repository;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Core.Domain.UserContext.Services;
using Image_guesser.SharedKernel;
using OneOf;

namespace Image_guesser.Core.Domain.OracleContext.Services;

public class OracleService(IAI_Repository AI_Repository, IImageService imageService, IOracleRepository oracleRepository, IUserService userService) : IOracleService
{
    private readonly IAI_Repository _AI_Repository = AI_Repository ?? throw new ArgumentNullException(nameof(AI_Repository));
    private readonly IImageService _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
    private readonly IOracleRepository _oracleRepository = oracleRepository ?? throw new ArgumentNullException(nameof(oracleRepository));
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));

    private Oracle<T> CreateGenericOracle<T>(T Oracle, string imageIdentifier) where T : class
    {
        var oracle = new Oracle<T>(Oracle, imageIdentifier);

        _oracleRepository.AddOracle(oracle);

        return oracle;
    }

    private AI CreateAI(AI_Type AI_Type, int pieceCount)
    {
        // Creating AI - which can easily be extended with more types
        return AI_Type switch
        {
            AI_Type.Incremental => _AI_Repository.CreateIncrementalAI(pieceCount),
            AI_Type.Decremental => _AI_Repository.CreateDecrementalAI(pieceCount),
            AI_Type.OddEvenOrder => _AI_Repository.CreateOddEvenOrderAI(pieceCount),
            AI_Type.EvenOddOrder => _AI_Repository.CreateEvenOddOrderAI(pieceCount),
            AI_Type.Random => _AI_Repository.CreateRandomNumbersAI(pieceCount),
            _ => _AI_Repository.CreateRandomNumbersAI(pieceCount)
        };
    }

    public async Task<OneOf<Oracle<User>, Oracle<AI>>> CreateOracle(Guid ChosenOracleId, string imageIdentifier, AI_Type AI_Type, bool OracleIsAI)
    {
        return OracleIsAI
        ? await CreateAIOracle(imageIdentifier, AI_Type)
        : CreateGenericOracle(await _userService.GetUserById(ChosenOracleId.ToString()), imageIdentifier);
    }

    public async Task<Oracle<AI>> CreateAIOracle(string imageIdentifier, AI_Type AI_Type)
    {
        var AI = CreateAI(AI_Type, await _imageService.GetImagePieceCountById(imageIdentifier));
        return CreateGenericOracle(AI, imageIdentifier);
    }

    public async Task<Oracle<T>?> GetOracleById<T>(Guid Id) where T : class
    {
        return await _oracleRepository.GetOracleById<T>(Id);
    }

    public async Task<string> GetNameOfOracleById(Guid Id)
    {
        var userOracle = await GetOracleById<User>(Id);
        if (userOracle != null)
        {
            return $"User: {userOracle.Entity.UserName}";
        }

        var aiOracle = await GetOracleById<AI>(Id);
        if (aiOracle != null)
        {
            return $"AI: {aiOracle.Entity.AI_Type}";
        }

        return "Unknown";
    }

    public async Task<BaseOracle> GetBaseOracleById(Guid Id)
    {
        return await _oracleRepository.GetBaseOracleById(Id);
    }

    public List<string> GetImageIdentifierOfAllPreviousPlayedGamesInTheSession(Guid sessionId)
    {
        return _oracleRepository.GetImageIdentifierOfAllPreviousPlayedGamesInTheSession(sessionId);
    }

    public int CalculatePoints(int pieceCount, int numberOfTilesRevealed)
    {
        return pieceCount - numberOfTilesRevealed;
    }

    public bool CheckGuess(string guess, string imageName)
    {
        var trimmedGuess = guess.Trim();

        // Comparing the guess with the image name
        if (trimmedGuess.Equals(imageName, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        // Comparing the guess with the image name words
        foreach (var word in imageName.Split(" "))
        {
            if (word.Trim().Equals(trimmedGuess, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public async Task<(bool IsGuessCorrect, string WinnerText, string ImageName)> HandleGuess(string Guess, string ImageIdentifier, string Username, Guid ChosenOracleId, GameMode gameMode)
    {
        var ImageRecord = await _imageService.GetImageRecordById(ImageIdentifier);

        var winnerText = $"Congratulations: {Username}";

        if (CheckGuess(Guess, ImageRecord.Name))
        {
            if (gameMode == GameMode.Duo)
            {
                User ChosenOracle = await _userService.GetUserById(ChosenOracleId.ToString());
                winnerText += $" and {ChosenOracle.UserName}";
            }

            return (true, winnerText, ImageRecord.Name);
        }

        return (false, string.Empty, string.Empty);
    }

    public async Task DeleteOracle<T>(Guid Id) where T : BaseEntity
    {
        await _oracleRepository.DeleteOracle<T>(Id);
    }

    public async Task UpdateOracle<T>(T baseOracle) where T : BaseEntity
    {
        await _oracleRepository.UpdateOracle(baseOracle);
    }
}
