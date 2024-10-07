using Image_guesser.Core.Domain.GameContext.Responses;
using Image_guesser.Core.Domain.ImageContext.Services;
using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.OracleContext.Services;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.SignalRContext.Services.Hub;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Core.Exceptions;
using Image_guesser.Infrastructure.GenericRepository;
using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext.Services;

public class GameService(IOracleService oracleService, IRepository repository, IHubService hubService, IImageService imageService) : IGameService
{
    private readonly IOracleService _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IHubService _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
    private readonly IImageService _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));

    private async Task<InitializeGameResponse> InitializeGame<T>(Session session, Oracle<T> Oracle) where T : class
    {
        var game = new Game<T>(session, Oracle);

        await _repository.Add(game);

        return new InitializeGameResponse(game.Id, game.SessionId);
    }

    private async Task<InitializeGameResponse> SetupGameWithAIAsOracle(Session session)
    {
        var AIOracle = await _oracleService.CreateAIOracle(session.Options.ImageIdentifier, session.Options.AI_Type);

        return await InitializeGame(session, AIOracle);
    }

    private async Task<InitializeGameResponse> SetupGameWithUserAsOracle(Session session)
    {
        User chosenOracle = await _repository.GetById<User, Guid>(session.ChosenOracleId);
        var UserOracle = _oracleService.CreateOracle(chosenOracle, session.Options.ImageIdentifier);

        return await InitializeGame(session, UserOracle);
    }

    public async Task CreateGame(Session session)
    {
        var pastIdentifiers = _oracleService.GetImageIdentifierOfAllPreviousPlayedGamesInTheSession(session.Id);
        // Ensure the picture is random each game
        if (session.Options.PictureMode == PictureMode.Random)
        {
            session.Options.ImageIdentifier = await _imageService.GetDifferentRandomImageIdentifier(pastIdentifiers);
        }

        var CreateGameResult = session.Options.IsOracleAI()
        ? await SetupGameWithAIAsOracle(session)
        : await SetupGameWithUserAsOracle(session);

        await _hubService.RedirectGroupToPage(CreateGameResult.SessionId.ToString(), $"/Lobby/{session.Id}/Game/{CreateGameResult.Id}");
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
}