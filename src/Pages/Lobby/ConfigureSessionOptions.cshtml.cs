using Image_guesser.Core.Domain.ImageContext;
using Image_guesser.Core.Domain.ImageContext.Services;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.SessionContext.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Image_guesser.Pages.Lobby;
[Authorize]
public class ConfigureSessionOptionsModel(ILogger<ConfigureSessionOptionsModel> logger, ISessionService sessionService, IImageService imageService) : PageModel
{
    private readonly ILogger<ConfigureSessionOptionsModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    private readonly IImageService _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public List<ImageRecord> ImageRecords { get; private set; } = [];

    [BindProperty]
    public ImageRecord ImageRecord { get; set; } = new();

    [BindProperty]
    public ViewModelOptions Options { get; set; } = null!;

    [BindProperty]
    public int AmountOfPicturesToLoad { get; set; } = 9;

    [BindProperty]
    public bool ShowSelectPictureModal { get; set; }

    public async Task OnGetAsync()
    {
        var session = await _sessionService.GetSessionById(Id);

        Options = new(session.Options);

        if (ImageRecords.Count == 0)
        {
            ImageRecords = await _imageService.GetXAmountOfImageRecords(AmountOfPicturesToLoad);
        }

        if (Options.PictureMode == PictureMode.Specific)
        {
            ImageRecord = await _imageService.GetImageRecordById(Options.ImageIdentifier);
        }

        _logger.LogInformation("{Name} visited the options page for session with Id: {Id}", User.Identity?.Name, Id);
    }

    public async Task OnPostRefreshImagesAsync()
    {
        ShowSelectPictureModal = true;

        ImageRecords = await _imageService.GetXAmountOfImageRecords(AmountOfPicturesToLoad);

        _logger.LogInformation("{Name} refreshed the image records in options page for session with Id: {Id}", User.Identity?.Name, Id);
    }

    public async Task<IActionResult> OnPostModifyAsync()
    {
        await _sessionService.UpdateSessionOptions(Id, Options);

        _logger.LogInformation("{Name} modified options for session with Id: {Id}", User.Identity?.Name, Id);

        return RedirectToPage("/Lobby/Session", new { Id });
    }
}