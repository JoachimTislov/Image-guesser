using Image_guesser.Core.Domain.GameContext;
using Image_guesser.Core.Domain.ImageContext.Services;
using Image_guesser.Core.Domain.OracleContext.AI_Repository;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Core.Domain.UserContext.Services;
using Image_guesser.Core.Exceptions;
using Image_guesser.Infrastructure.GenericRepository;
using OneOf;

namespace Image_guesser.Core.Domain.OracleContext.Services;

public class OracleService(IAI_Repository AI_Repository, IImageService imageService, IRepository repository, IUserService userService) : IOracleService
{
    private readonly IAI_Repository _AI_Repository = AI_Repository ?? throw new ArgumentNullException(nameof(AI_Repository));
    private readonly IImageService _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));

    private Oracle<T> CreateGenericOracle<T>(T Oracle, string imageIdentifier) where T : class
    {
        var oracle = new Oracle<T>(Oracle, imageIdentifier);

        _repository.Add(oracle);

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

    public async Task<Oracle<T>> GetOracleById<T>(Guid Id) where T : class
    {
        return await _repository.GetSingleWhere<Oracle<T>>(o => o.Id == Id) ?? throw new EntityNotFoundException($"Oracle was not found by id: {Id}");
    }

    public async Task<BaseOracle> GetBaseOracleById(Guid Id)
    {
        return await _repository.GetSingleWhere<BaseOracle>(o => o.Id == Id) ?? throw new EntityNotFoundException($"Base Oracle was not found by id: {Id}");
    }

    public List<string> GetImageIdentifierOfAllPreviousPlayedGamesInTheSession(Guid sessionId)
    {
        IQueryable<Guid> BaseOracleIds = _repository.WhereAndSelect<BaseGame, Guid>(g => g.SessionId == sessionId, g => g.BaseOracleId);

        return [.. _repository.WhereAndSelect<BaseOracle, string>(b => BaseOracleIds.Contains(b.Id), b => b.ImageIdentifier)];
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
                User ChosenOracle = await _repository.GetById<User, Guid>(ChosenOracleId);
                winnerText += $" and {ChosenOracle.UserName}";
            }

            return (true, winnerText, ImageRecord.Name);
        }

        return (false, string.Empty, string.Empty);
    }

    public async Task DeleteOracle<T>(Guid Id) where T : class
    {
        var oracle = await GetOracleById<T>(Id);
        await _repository.Delete(oracle);
    }

    public async Task UpdateBaseOracle(BaseOracle baseOracle)
    {
        await _repository.Update(baseOracle);
    }
}
