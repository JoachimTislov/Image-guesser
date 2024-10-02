using Image_guesser.Core.Domain.GameContext.Responses;
using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.OracleContext.Services;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.SignalRContext.Hubs;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Infrastructure.GenericRepository;
using Microsoft.AspNetCore.SignalR;

namespace Image_guesser.Core.Domain.GameContext.Services;

public class GameService(IOracleService oracleService, IRepository repository, IHubContext<GameHub, IGameClient> hubContext) : IGameService
{
    private readonly IOracleService _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IHubContext<GameHub, IGameClient> _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));

    private async Task<InitializeGameResponse> InitializeGame<T>(Guid sessionId, List<User> users, string gameMode, Oracle<T> Oracle) where T : class
    {
        var game = new Game<T>(sessionId, users, gameMode, Oracle);

        await _repository.Add(game);

        return new InitializeGameResponse(game.Id, game.SessionId);
    }

    private async Task<InitializeGameResponse> SetupGameWithAIAsOracle(Guid sessionId, List<User> users, string gameMode, string ImageIdentifier, AI_Type AI_Type)
    {
        var AIOracle = await _oracleService.CreateAIOracle(ImageIdentifier, AI_Type);

        return await InitializeGame(sessionId, users, gameMode, AIOracle);
    }

    private async Task<InitializeGameResponse> SetupGameWithUserAsOracle(Guid sessionId, List<User> users, string gameMode, string ImageIdentifier, Guid ChosenOracleId)
    {
        User chosenOracle = await _repository.GetById<User, Guid>(ChosenOracleId);
        var UserOracle = _oracleService.CreateOracle(chosenOracle, ImageIdentifier);

        return await InitializeGame(sessionId, users, gameMode, UserOracle);
    }

    public async Task CreateGame(Session session)
    {
        InitializeGameResponse result;

        result = session.Options.IsOracleAI()
        ? await SetupGameWithAIAsOracle(session.Id, session.SessionUsers, session.Options.GameMode.ToString(), session.Options.ImageIdentifier, session.Options.AI_Type)
        : await SetupGameWithUserAsOracle(session.Id, session.SessionUsers, session.Options.GameMode.ToString(), session.Options.ImageIdentifier, session.ChosenOracleId);

        await _hubContext.Clients.Group(result.sessionId.ToString()).RedirectToLink($"/Game/{result.Id}");
    }

    public async Task<Game<T>> GetGameById<T>(Guid Id) where T : class
    {
        return await _repository.GetById<Game<T>, Guid>(Id);
    }

    public async Task<BaseGame> GetBaseGameById(Guid gameId)
    {
        return await _repository.GetById<BaseGame, Guid>(gameId) ?? throw new Exception("Game not found");
    }

    public async Task<Guesser> GetGuesserById(Guid GuesserId)
    {
        return await _repository.GetById<Guesser, Guid>(GuesserId) ?? throw new Exception("Guesser not found");
    }
}