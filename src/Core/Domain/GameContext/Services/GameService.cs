using Image_guesser.Core.Domain.GameContext.Responses;
using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.OracleContext.Services;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Infrastructure;
using Image_guesser.Infrastructure.GenericRepository;

namespace Image_guesser.Core.Domain.GameContext.Services;

public class GameService(ImageGameContext db, IOracleService oracleService, IRepository repository) : IGameService
{
    private readonly ImageGameContext _db = db ?? throw new ArgumentNullException(nameof(db));
    private readonly IOracleService _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    private async Task<Guid> AddGame<T>(Game<T> game) where T : class
    {
        await _repository.Add(game);

        return game.Id;
    }

    private async Task<InitializeGameResponse> InitializeGame<T>(Guid sessionId, List<User> users, string gameMode, Oracle<T> Oracle) where T : class
    {
        var game = new Game<T>(sessionId, users, gameMode, Oracle);

        var gameId = await AddGame(game);
        return new InitializeGameResponse(true, gameId, game.SessionId);
    }

    public async Task<InitializeGameResponse> SetupGameWithAIAsOracle(Guid sessionId, List<User> users, string gameMode, string ImageIdentifier, AI_Type AI_Type)
    {
        var AIOracle = await _oracleService.CreateAIOracle(ImageIdentifier, AI_Type);

        return await InitializeGame(sessionId, users, gameMode, AIOracle);
    }

    public async Task<InitializeGameResponse> SetupGameWithUserAsOracle(Guid sessionId, List<User> users, string gameMode, Guid ChosenOracleId)
    {
        User chosenOracle = await _repository.GetById<User>(ChosenOracleId);
        var UserOracle = _oracleService.CreateUserOracle(chosenOracle);

        return await InitializeGame(sessionId, users, gameMode, UserOracle);
    }

    public async Task<Game<T>> GetGameById<T>(Guid Id) where T : class
    {
        return await _repository.GetSingleWhere<Game<T>, Guid>(s => s.Id == Id, Id);
    }

    public async Task<BaseGame> GetBaseGameById(Guid gameId)
    {
        return await _repository.GetById<BaseGame>(gameId) ?? throw new Exception("Game not found");
    }

    public async Task<Guesser> GetGuesserById(Guid GuesserId)
    {
        return await _repository.GetById<Guesser>(GuesserId) ?? throw new Exception("Guesser not found");
    }
}