using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.SessionContext.ViewModels;
using Image_guesser.Core.Domain.SignalRContext.Services.Hub;
using Image_guesser.Core.Domain.UserContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Image_guesser.Pages.Lobby;
[Authorize]
public class ConfigureSessionOptionsModel(ILogger<ConfigureSessionOptionsModel> logger, ISessionService sessionService, IHubService hubService) : PageModel
{
    private readonly ILogger<ConfigureSessionOptionsModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    private readonly IHubService _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));


    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public ViewModelOptions Options { get; set; } = null!;

    public List<SelectListItem> Users { get; set; } = [];

    public async Task OnGetAsync()
    {
        var session = await _sessionService.GetSessionById(Id);

        Options = new(session.Options);

        Users = await _sessionService.GetSelectListOfUsersById(Id);

        _logger.LogInformation("{Name} visited the options page for session with Id: {Id}", User.Identity?.Name, Id);
    }

    public async Task OnPostModifyAsync()
    {
        await _sessionService.UpdateSessionOptions(Id, Options);

        _logger.LogInformation("{Name} modified options for session with Id: {Id}", User.Identity?.Name, Id);

        await _hubService.RedirectGroupToPage(Id.ToString(), $"/Lobby/{Id}");
    }
}