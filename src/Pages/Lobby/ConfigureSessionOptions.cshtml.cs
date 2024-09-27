using Image_guesser.Core.Domain.ImageContext;
using Image_guesser.Core.Domain.ImageContext.Services;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.SessionContext.ViewModels;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Core.Domain.UserContext.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Image_guesser.Pages.Lobby;

public class ConfigureSessionOptionsModel(ILogger<ConfigureSessionOptionsModel> logger, ISessionService sessionService, IImageService imageService, IUserService userService) : PageModel
{
    private readonly ILogger<ConfigureSessionOptionsModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    private readonly IImageService _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public List<ImageRecord> ImageRecords { get; private set; } = [];

    [BindProperty]
    public ImageRecord ImageRecord { get; set; } = new();

    [BindProperty]
    public ViewModelOptions Options { get; set; } = null!;

    [BindProperty]
    public int AmountOfPicturesToLoad { get; set; } = 9;

    public async Task<IActionResult> OnGetAsync()
    {
        Session session;
        if (Id == Guid.Empty)
        {
            User user = await _userService.GetUserByClaimsPrincipal(User);
            session = await _sessionService.CreateSession(user);

            return RedirectToPage("/Lobby/ConfigureSessionOptions", new { session.Id });
        }
        else
        {
            session = await _sessionService.GetSessionById(Id);
        }

        // Update global options
        Options = new(session.Options);

        if (ImageRecords.Count == 0)
        {
            ImageRecords = await _imageService.GetXAmountOfImageRecords(AmountOfPicturesToLoad);
        }

        if (Options.ImageIdentifier != string.Empty && !Options.RandomPictureMode)
        {
            ImageRecord = await _imageService.GetImageRecordById(Options.ImageIdentifier);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostRefreshImagesAsync()
    {
        ImageRecords = await _imageService.GetXAmountOfImageRecords(AmountOfPicturesToLoad);

        return Page();
    }

    public async Task<IActionResult> OnPostModifyAsync()
    {
        if (Id == Guid.Empty)
        {
            Id = (await _userService.GetSessionIdForGivenUserWithClaimPrincipal(User)).Value;
        }

        await _sessionService.UpdateSession(Id, Options);

        return RedirectToPage("/Lobby/Session", new { Id });
    }
}