using Image_guesser.Core.Domain.GameContext;
using Image_guesser.Core.Domain.ImageContext;
using Image_guesser.Core.Domain.ImageContext.Services;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Core.Domain.UserContext.Services;
using Image_guesser.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Image_guesser.Pages.Lobby;

[RequireLogin]
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
    public bool ShowImageModal { get; set; }

    public User Player { get; set; } = null!;

    public async Task OnGet()
    {
        await LoadSessionData();

        _logger.LogInformation("{Name} entered the session page with Id: {Id}", User.Identity?.Name, Id);
    }

    public async Task OnPostStartGame()
    {
        await LoadSessionData();

        if (!Session.Options.IsGameMode(GameMode.SinglePlayer) && Session.SessionUsers.Count < 2)
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
        else if (ImageRecord == null && Session.Options.IsPictureMode(PictureMode.Specific))
        {
            ModelState.AddModelError(string.Empty, "Please select an image to start the game");
        }
        else
        {
            _logger.LogInformation("{Name} started a game in a session with Id: {Id}", User.Identity?.Name, Id);
            await _mediator.Publish(new CreateGame(Id, ImageRecord?.Identifier));
        }
    }

    public async Task OnPostOracleSelectedAnImage(string imageIdentifier)
    {
        await _sessionService.SetImageIdentifier(Id, imageIdentifier);

        await LoadSessionData();

        _logger.LogInformation("User Oracle - {Name} selected an image in sessions with Id: {Id}", User.Identity?.Name, Id);
    }

    public async Task OnPostRefreshImagesAsync()
    {
        ShowImageModal = true;

        await LoadSessionData();

        _logger.LogInformation("User Oracle - {Name} refreshed the image records in sessions with Id: {Id}", User.Identity?.Name, Id);
    }

    private async Task LoadImageRecords() => ImageRecords = await _imageService.GetXAmountOfImageRecords(AmountOfPicturesToLoad);

    private async Task LoadSessionData()
    {
        Session = await _sessionService.GetSessionById(Id);
        SessionHost = await _userService.GetUserById(Session.SessionHostId.ToString());
        ChosenOracle = await _userService.GetUserById(Session.ChosenOracleId.ToString());

        var ImageIdentifier = Session.Options.ImageIdentifier;
        ImageRecord = !string.IsNullOrEmpty(ImageIdentifier) ? await _imageService.GetImageRecordById(ImageIdentifier) : null;

        await LoadImageRecords();

        Player = await _userService.GetUserByClaimsPrincipal(User);
        UserIsSessionHost = Session.UserIsSessionHost(Player.Id);
        UserIsOracle = Session.UserIsOracle(Player.Id);
    }
}