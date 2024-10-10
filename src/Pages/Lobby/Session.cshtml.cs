using Image_guesser.Core.Domain.GameContext;
using Image_guesser.Core.Domain.ImageContext;
using Image_guesser.Core.Domain.ImageContext.Services;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Core.Domain.UserContext.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Image_guesser.Pages.Lobby;

[Authorize]
public class SessionModel(ILogger<SessionModel> logger, ISessionService sessionService, IUserService userService, IImageService imageService, IMediator mediator) : PageModel
{
    private readonly ILogger<SessionModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    private readonly IImageService _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public List<string> SessionErrorMessages { get; set; } = [];

    public Session Session { get; set; } = null!;
    public User SessionHost { get; set; } = null!;
    public bool UserIsSessionHost { get; set; }
    public bool UserIsOracle { get; set; }
    public User ChosenOracle { get; set; } = null!;

    [BindProperty]
    public List<ImageRecord> ImageRecords { get; set; } = [];

    [BindProperty]
    public int AmountOfPicturesToLoad { get; set; } = 9;

    public ImageRecord? ImageRecord { get; set; }
    public string? ImageIdentifier { get; set; }

    public User Player { get; set; } = null!;

    public async Task OnGet()
    {
        await LoadSessionData();

        await LoadImageRecords();

        _logger.LogInformation("{Name} entered the session page with Id: {Id}", Player.UserName, Id);
    }

    public async Task<IActionResult> OnPostStartGame(string imageIdentifier)
    {
        ImageIdentifier = imageIdentifier;

        await LoadSessionData();

        var session = await _sessionService.GetSessionById(Id);

        if (!session.Options.IsGameMode(GameMode.SinglePlayer) && session.SessionUsers.Count < 2)
        {
            string[] missingRequirements = [
                "Need more players to start a game",
                "Duo requires two players",
                "Free for all requires at least two players",
            ];

            foreach (var requirement in missingRequirements)
            {
                ModelState.AddModelError(string.Empty, requirement);
            }
        }
        else if (ImageRecord == null && session.Options.PictureMode == PictureMode.Specific)
        {
            ModelState.AddModelError(string.Empty, "Please select an image to start the game");
        }
        else
        {
            _logger.LogInformation("{Name} started a game in a session with Id: {Id}", User.Identity?.Name, Id);
            await _mediator.Publish(new CreateGame(Id, ImageRecord?.Identifier));
        }

        return Page();
    }

    public async Task<IActionResult> OnPostOracleSelectedAnImage(string imageIdentifier)
    {
        ImageIdentifier = imageIdentifier;

        _logger.LogInformation("User Oracle - {Name} selected an image in sessions with Id: {Id}", User.Identity?.Name, Id);

        await LoadSessionData();

        return Page();
    }

    public async Task<IActionResult> OnPostRefreshImagesAsync()
    {
        await LoadSessionData();

        await LoadImageRecords();

        _logger.LogInformation("User Oracle - {Name} refreshed the image records in sessions with Id: {Id}", User.Identity?.Name, Id);

        return Page();
    }

    private async Task LoadImageRecords() => ImageRecords = await _imageService.GetXAmountOfImageRecords(AmountOfPicturesToLoad);

    private async Task LoadSessionData()
    {
        Session = await _sessionService.GetSessionById(Id);
        SessionHost = await _userService.GetUserById(Session.SessionHostId.ToString());
        ChosenOracle = await _userService.GetUserById(Session.ChosenOracleId.ToString());

        if (ImageIdentifier != null) ImageRecord = await _imageService.GetImageRecordById(ImageIdentifier);

        Player = await _userService.GetUserByClaimsPrincipal(User);
        UserIsSessionHost = Session.UserIsSessionHost(Player.Id);
        UserIsOracle = Session.UserIsOracle(Player.Id);
    }
}