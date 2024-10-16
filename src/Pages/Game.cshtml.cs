using Image_guesser.Core.Domain.GameContext;
using Image_guesser.Core.Domain.GameContext.Services;
using Image_guesser.Core.Domain.ImageContext;
using Image_guesser.Core.Domain.ImageContext.Services;
using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.OracleContext.Services;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.SignalRContext.Services.Hub;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Core.Domain.UserContext.Services;
using Image_guesser.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Image_guesser.Pages;

[RequireLogin]
public class GameModel(ILogger<ProfileModel> logger, IOracleService oracleService, IUserService userService, ISessionService sessionService, IGameService gameService, IImageService imageService, IMediator mediator, IHubService hubService) : PageModel
{
    private readonly ILogger<ProfileModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IOracleService _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    private readonly IGameService _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
    private readonly IImageService _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly IHubService _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));

    [BindProperty(SupportsGet = true)]
    public Guid SessionId { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid GameId { get; set; }

    public User Player { get; set; } = null!;
    public BaseGame BaseGame { get; set; } = null!;
    public BaseOracle BaseOracle { get; set; } = null!;
    public Guesser? Guesser { get; set; }

    public Guid SessionHostId { get; set; }

    public Game<User>? GameUser { get; set; }
    public Game<AI>? GameAI { get; set; }

    public int NumberOfTilesRevealed { get; set; }

    public ImageRecord ImageRecord { get; set; } = null!;
    public string ImagePieceList { get; set; } = null!;
    public string? ImageCoordinates { get; set; }
    public string UrlToCorrectImageTilesDirectory { get; set; } = string.Empty;

    /* If statements */
    public bool UserIsOracle { get; set; }
    public bool UserIsSessionHost { get; set; }
    public bool OracleIsAI { get; set; }

    [BindProperty]
    public AI_Type? Selected_AI_Type { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        return await LoadGameData();
    }

    public async Task OnPostReturnToLobby()
    {
        var BaseGame = await _gameService.GetBaseGameById(GameId);
        if (!BaseGame.IsFinished())
        {
            await _mediator.Publish(new GameTerminated(GameId, SessionId));
        }
        await _mediator.Publish(new ReturnToLobby(SessionId));
    }

    public async Task OnPostLeaveGame()
    {
        var user = await _userService.GetUserByClaimsPrincipal(User);
        await _mediator.Publish(new PlayerLeftGame(user.Id.ToString(), Guesser?.Id, GameId, SessionId));
    }

    public async Task OnPostCreateGame()
    {
        await _mediator.Publish(new CreateGame(SessionId));
    }

    public async Task OnPostRestartGameWithNewOracle()
    {
        if (Selected_AI_Type != null)
        {
            await _gameService.RestartGameWithNewOracle(GameId, Selected_AI_Type.Value);
        }
    }

    public async Task OnPostSetImageSize(int imageSize, string imageIdentifier, int imageContainerWidth, int imageContainerHeight)
    {
        _logger.LogInformation("Image container size, width: {width}, and height: {height}. User selected size: {imageSize}", imageContainerWidth, imageContainerHeight, imageSize);

        //await _mediator.Publish(new ChangeImageSizeRequest(imageSize, imageIdentifier, imageContainerWidth, imageContainerHeight));

        Player = await _userService.GetUserByClaimsPrincipal(User);
        _imageService.SetSizeOfImagePieces(imageIdentifier, imageContainerWidth, imageContainerHeight, imageSize / 100.0, Player.UserName!, Player.CustomSizedImageTilesDirectoryId);

        await _hubService.RedirectGroupToPage(SessionId.ToString(), $"/Lobby/{SessionId}/Game/{GameId}");
    }

    private async Task<IActionResult> LoadGameData()
    {
        BaseGame = await _gameService.GetBaseGameById(GameId);

        if (BaseGame.IsFinished() || BaseGame.IsTerminated())
        {
            _logger.LogWarning("Game with id: {Id} is already finished", GameId);
            return RedirectToPage("/Lobby/Session", new { Id = SessionId });
        }

        Player = await _userService.GetUserByClaimsPrincipal(User);
        BaseOracle = await _oracleService.GetBaseOracleById(BaseGame.BaseOracleId);
        await _userService.UpdateCurrentImageIdentifier(Player, BaseOracle.ImageIdentifier);

        var pathToUsersCustomSizedImageTiles = _imageService.CheckIfUserHasCustomSizedImageTiles(Player.UserName!, BaseOracle.ImageIdentifier);

        UrlToCorrectImageTilesDirectory = pathToUsersCustomSizedImageTiles
        ? $"/DataSet/Custom_Sized_Image_Tiles/{Player.UserName!}_{BaseOracle.ImageIdentifier}"
        : $"/DataSet/Scattered_Images/{BaseOracle.ImageIdentifier}";

        SessionHostId = await _sessionService.GetSessionHostIdById(SessionId);
        OracleIsAI = await _sessionService.CheckIfOracleIsAI(SessionId);
        UserIsOracle = await _sessionService.CheckIfUserIsOracle(SessionId, Player.Id);
        UserIsSessionHost = await _sessionService.CheckIfUserIsSessionHost(SessionId, Player.Id);
        NumberOfTilesRevealed = BaseOracle.NumberOfTilesRevealed;
        GameAI = await _gameService.GetGameById<AI>(GameId);
        GameUser = await _gameService.GetGameById<User>(GameId);
        Guesser = BaseGame.Guessers.FirstOrDefault(g => g.Name == Player.UserName);
        Selected_AI_Type = GameAI?.Oracle.Entity.AI_Type;

        if (Guesser != null)
        {
            _logger.LogInformation("Guesser: {Name} entered the game page, game id: {Id}", Guesser.Name, GameId);
        }
        else if (UserIsSessionHost || UserIsOracle)
        {
            _logger.LogInformation("Oracle/Host: {Name} entered the game page, game id: {Id}", Player.UserName, GameId);
        }
        else
        {
            _logger.LogWarning("User: {Name} is not allowed at game with id: {Id}", Player.Id, GameId);
        }

        ImageRecord = await _imageService.GetImageRecordById(BaseOracle.ImageIdentifier);

        var imagePieceList = _imageService.GetFileNameOfImagePieces(BaseOracle.ImageIdentifier, Player.UserName!, Player.CustomSizedImageTilesDirectoryId);
        ImagePieceList = JsonConvert.SerializeObject(imagePieceList.Select(pathName => pathName.Split("wwwroot")[1].Replace("\\", "/")));

        if (UserIsOracle)
        {
            ImageCoordinates = JsonConvert.SerializeObject(_imageService.GetCoordinatesForImagePieces(imagePieceList));
        }

        return Page();
    }
}
