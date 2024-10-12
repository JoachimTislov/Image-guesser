using System.Text.RegularExpressions;
using Image_guesser.Core.Domain.GameContext.Responses;
using Image_guesser.Core.Domain.ImageContext.Services;
using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.OracleContext.Services;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.SignalRContext.Services.Hub;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Core.Domain.UserContext.Services;
using Image_guesser.Core.Exceptions;
using Image_guesser.Infrastructure.GenericRepository;
using Image_guesser.SharedKernel;
using OneOf;

namespace Image_guesser.Core.Domain.GameContext.Services;

public class GameService(IOracleService oracleService, IRepository repository, IHubService hubService) : IGameService
{
    private readonly IOracleService _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IHubService _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));

    private async Task<InitializeGameResponse> InitializeGame<T>(Session session, Oracle<T> Oracle) where T : class
    {
        var game = new Game<T>(session, Oracle);

        await _repository.Add(game);

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

    public async Task RestartGameWithNewOracle(Guid gameId, AI_Type AI_Type)
    {
        var game = await GetGameById<AI>(gameId) ?? throw new EntityNotFoundException($"Game with Id: {gameId} was not found, cannot restart game with new oracle");

        var imageIdentifier = game.Oracle.ImageIdentifier;

        // Clean up old AI
        await _oracleService.DeleteOracle<AI>(game.Oracle.Id);

        game.Oracle = await _oracleService.CreateAIOracle(imageIdentifier, AI_Type);
        game.BaseOracleId = game.Oracle.Id;

        await UpdateGame(game);

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

        await UpdateGame(game);
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

        await UpdateGame(game);
    }

    public async Task<Game<T>?> GetGameById<T>(Guid Id) where T : class
    {
        return await _repository.WhereAndInclude_SingleOrDefault<Game<T>, T>(g => g.Id == Id, g => g.Oracle.Entity);
    }

    public async Task DeleteGuesserById(Guid Id)
    {
        var guesser = await GetGuesserById(Id);
        await _repository.Delete(guesser);
    }

    public async Task<BaseGame> GetBaseGameById(Guid Id)
    {
        return await _repository.WhereAndInclude_SingleOrDefault<BaseGame, List<Guesser>>(bg => bg.Id == Id, bg => bg.Guessers) ?? throw new EntityNotFoundException($"BaseGame with Id: {Id} was not found");
    }

    public async Task<Guesser> GetGuesserById(Guid GuesserId)
    {
        return await _repository.GetById<Guesser, Guid>(GuesserId);
    }

    public async Task UpdateGame<TGame>(TGame game) where TGame : BaseEntity
    {
        await _repository.Update(game);
    }

    public async Task UpdateGuesser(Guesser guesser)
    {
        await _repository.Update(guesser);
    }
}