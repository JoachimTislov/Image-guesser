using Image_guesser.Core.Domain.GameContext;
using Image_guesser.Core.Domain.GameContext.Events;
using Image_guesser.Core.Domain.GameContext.Services;
using Image_guesser.Core.Domain.ImageContext;
using Image_guesser.Core.Domain.ImageContext.Services;
using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.OracleContext.Services;
using Image_guesser.Core.Domain.SessionContext.Events;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Core.Domain.UserContext.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Image_guesser.Pages;

[Authorize]
public class GameModel(ILogger<ProfileModel> logger, IOracleService oracleService, IUserService userService, ISessionService sessionService, IGameService gameService, IImageService imageService, IMediator mediator) : PageModel
{
    private readonly ILogger<ProfileModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IOracleService _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    private readonly IGameService _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
    private readonly IImageService _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [BindProperty(SupportsGet = true)]
    public Guid SessionId { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid GameId { get; set; }

    public User Player { get; set; } = null!;
    public BaseGame BaseGame { get; set; } = null!;
    public BaseOracle BaseOracle { get; set; } = null!;
    public Guesser? Guesser { get; set; } = null!;

    public Guid SessionHostId { get; set; }

    public Game<User>? GameUser { get; set; }
    public Game<AI>? GameAI { get; set; }

    public int NumberOfTilesRevealed { get; set; }

    public ImageRecord ImageRecord { get; set; } = null!;
    public string ImagePieceList { get; set; } = null!;
    public string? ImageCoordinates { get; set; }

    /* If statements */
    public bool UserIsOracle { get; set; }
    public bool UserIsSessionHost { get; set; }
    public bool OracleIsAI { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!User.Identity?.IsAuthenticated ?? false)
        {
            return RedirectToPage("/Home/Index");
        }
        else
        {
            Player = await _userService.GetUserByClaimsPrincipal(User);

            BaseGame = await _gameService.GetBaseGameById(GameId);

            BaseOracle = await _oracleService.GetBaseOracleById(BaseGame.BaseOracleId);

            SessionHostId = await _sessionService.GetSessionHostIdById(SessionId);

            OracleIsAI = await _sessionService.CheckIfOracleIsAI(SessionId);

            UserIsOracle = await _sessionService.CheckIfUserIsOracle(SessionId, Player.Id);

            UserIsSessionHost = await _sessionService.CheckIfUserIsSessionHost(SessionId, Player.Id);

            NumberOfTilesRevealed = BaseOracle.NumberOfTilesRevealed;

            GameAI = await _gameService.GetGameById<AI>(GameId);

            GameUser = await _gameService.GetGameById<User>(GameId);

            Guesser = BaseGame.Guessers.FirstOrDefault(g => g.Name == Player.UserName);

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

            var ImageIdentifier = BaseOracle.ImageIdentifier;

            ImageRecord = await _imageService.GetImageRecordById(ImageIdentifier);

            var imagePieceList = _imageService.GetFileNameOfImagePieces(ImageIdentifier);
            ImagePieceList = JsonConvert.SerializeObject(imagePieceList);

            if (UserIsOracle)
            {
                ImageCoordinates = JsonConvert.SerializeObject(_imageService.GetCoordinatesForImagePieces(imagePieceList));
            }
        }

        return Page();
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
        await _mediator.Publish(new UserLeftGame(user.Id.ToString(), Guesser?.Id, GameId, SessionId));
    }

    public async Task OnPostCreateGame()
    {
        await _mediator.Publish(new CreateGame(SessionId));
    }

    /* public IActionResult OnPostSetImageSize(int width, int height, List<string> imagePieceList)
     {
         _logger.LogInformation("Change the image size to; width: {width}, and height: {height}", width, height);

         // _imageService.ChangeSizeOfImagePiece(imagePieceList, width, height);

         return new JsonResult(new { success = true, width = 30, height });
     }*/
}
