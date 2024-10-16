using Image_guesser.Core.Domain.GameContext.Repository;
using Image_guesser.Core.Domain.GameContext.Responses;
using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.OracleContext.Services;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.SignalRContext.Services.Hub;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Core.Exceptions;
using Image_guesser.SharedKernel;
using OneOf;

namespace Image_guesser.Core.Domain.GameContext.Services;

public class GameService(IOracleService oracleService, IHubService hubService, IGameRepository gameRepository, ISessionService sessionService) : IGameService
{
    private readonly IOracleService _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
    private readonly IHubService _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
    private readonly IGameRepository _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));

    private async Task<InitializeGameResponse> InitializeGame<T>(Session session, Oracle<T> Oracle) where T : class
    {
        var game = new Game<T>(session, Oracle);

        await _sessionService.AddGameToSession(session, game);

        return new InitializeGameResponse(game.Id, game.SessionId);
    }

    public async Task CreateGame(Session session, string imageIdentifier)
    {
        OneOf<Oracle<User>, Oracle<AI>> Oracle = await _oracleService.CreateOracle(session.ChosenOracleId, imageIdentifier, session.Options.AI_Type, session.Options.IsOracleAI());

        var InitializeGameResult = await Oracle.Match(
            async userOracle => await InitializeGame(session, userOracle),
            async aiOracle => await InitializeGame(session, aiOracle)
        );

        await _hubService.RedirectGroupToPage(InitializeGameResult.SessionId.ToString(), $"/Lobby/{session.Id}/Game/{InitializeGameResult.Id}");
    }

    public async Task CreateAndAddGuess(string guess, string nameOfGuesser, string timeOfGuess, Guid gameId)
    {
        var game = await GetBaseGameById(gameId);
        game.CreateAndAddGuess(guess, nameOfGuesser, timeOfGuess);

        await UpdateGameOrGuesser(game);
    }

    public async Task RestartGameWithNewOracle(Guid gameId, AI_Type AI_Type)
    {
        var game = await GetGameById<AI>(gameId) ?? throw new EntityNotFoundException($"Game with Id: {gameId} was not found, cannot restart game with new oracle");

        var imageIdentifier = game.Oracle.ImageIdentifier;

        // Clean up old AI
        await _oracleService.DeleteOracle<AI>(game.Oracle.Id);

        game.Oracle = await _oracleService.CreateAIOracle(imageIdentifier, AI_Type);
        game.BaseOracleId = game.Oracle.Id;

        await UpdateGameOrGuesser(game);

        await _hubService.RedirectGroupToPage(game.SessionId.ToString(), $"/Lobby/{game.SessionId}/Game/{gameId}");
    }

    public async Task AddGuesserFromGame(Guid guesserId, Guid gameId)
    {
        var game = await GetBaseGameById(gameId);
        var guesser = await GetGuesserById(guesserId);

        if (!game.AddGuesser(guesser))
        {
            // Guesser is already in game..
            return;
        }

        await UpdateGameOrGuesser(game);
    }

    public async Task RemoveGuesserFromGame(Guid guesserId, Guid gameId)
    {
        var game = await GetBaseGameById(gameId);
        var guesser = await GetGuesserById(guesserId);

        if (!game.RemoveGuesser(guesser))
        {
            // No guesser to remove..
            return;
        }

        await UpdateGameOrGuesser(game);
    }

    public async Task<Game<T>?> GetGameById<T>(Guid Id) where T : class
    {
        return await _gameRepository.GetGameById<T>(Id);
    }

    public async Task<List<BaseGame>> GetXAmountOfRecentGames(int amount)
    {
        return await _gameRepository.GetXAmountOfRecentGames(amount);
    }

    public async Task DeleteGuesserById(Guid Id)
    {
        await _gameRepository.DeleteGuesserById(Id);
    }

    public async Task<BaseGame> GetBaseGameById(Guid Id)
    {
        return await _gameRepository.GetBaseGameById(Id);
    }

    public async Task<Guesser> GetGuesserById(Guid GuesserId)
    {
        return await _gameRepository.GetGuesserById(GuesserId);
    }

    public List<BaseGame> GetGames()
    {
        return _gameRepository.GetGames();
    }

    public async Task UpdateGameOrGuesser<TGameOrGuesser>(TGameOrGuesser game) where TGameOrGuesser : BaseEntity
    {
        await _gameRepository.UpdateGameOrGuesser(game);
    }
}