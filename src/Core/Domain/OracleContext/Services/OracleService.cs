using Image_guesser.Core.Domain.ImageContext.Services;
using Image_guesser.Core.Domain.OracleContext.Repositories;
using Image_guesser.Core.Domain.OracleContext.Responses;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Infrastructure;
using Image_guesser.Infrastructure.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace Image_guesser.Core.Domain.OracleContext.Services;

public class OracleService(IAI_Repository AI_Repository, IImageService imageService, ImageGameContext db, IRepository repository) : IOracleService
{
    private readonly IAI_Repository _AI_Repository = AI_Repository ?? throw new ArgumentNullException(nameof(AI_Repository));
    private readonly IImageService _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
    private readonly ImageGameContext _db = db ?? throw new ArgumentNullException(nameof(db));
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<Oracle<AI>> CreateAIOracle(string imageIdentifier, AI_Type AI_Type)
    {
        var ImagePieceCount = await _imageService.GetImagePieceCountById(imageIdentifier);

        AI AI = AI_Type switch
        {
            AI_Type.Random => _AI_Repository.CreateRandomNumbersAI(ImagePieceCount),
            _ => _AI_Repository.CreateRandomNumbersAI(ImagePieceCount)
        };

        return new Oracle<AI>(AI);
    }

    public Oracle<User> CreateUserOracle(User ChosenOracle)
    {
        return new Oracle<User>(ChosenOracle);
    }

    public async Task<Oracle<T>> GetOracleById<T>(Guid Id) where T : class
    {
        return await _db.Oracles
            .Where(o => o.Id == Id)
            .OfType<Oracle<T>>()
            .SingleOrDefaultAsync() ?? throw new Exception($"Oracle, with type: {typeof(T)} and Id: {Id} not found");
    }

    public async Task<BaseOracle> GetBaseOracleById(Guid Id)
    {
        return await _db.Oracles.Where(o => o.Id == Id).SingleOrDefaultAsync() ?? throw new Exception("Oracle not found");
    }

    public async Task<Check_Guess_Response> CheckGuess(string Guess, string ImageIdentifier, string Username, Guid ChosenOracleId, GameMode gameMode)
    {
        var ImageRecord = await _imageService.GetImageRecordById(ImageIdentifier);

        var winnerText = $"Congratulations: {Username}";

        if (Guess.Equals(ImageRecord.Name, StringComparison.CurrentCultureIgnoreCase))
        {
            if (gameMode == GameMode.Duo)
            {
                User ChosenOracle = await _repository.GetById<User>(ChosenOracleId);
                winnerText += $" and {ChosenOracle.UserName}";
            }

            return new Check_Guess_Response(true, winnerText);
        }

        return new Check_Guess_Response(false, string.Empty);
    }
}
