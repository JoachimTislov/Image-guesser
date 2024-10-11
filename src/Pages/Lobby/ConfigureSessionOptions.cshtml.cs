using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.SessionContext.ViewModels;
using Image_guesser.Core.Domain.SignalRContext.Services.Hub;
using Image_guesser.Core.Domain.UserContext.Services;
using Image_guesser.SharedKernel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Image_guesser.Pages.Lobby;

[RequireLogin]
public class ConfigureSessionOptionsModel(ILogger<ConfigureSessionOptionsModel> logger, ISessionService sessionService, IHubService hubService, IUserService userService) : PageModel
{
    private readonly ILogger<ConfigureSessionOptionsModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    private readonly IHubService _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));


    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public ViewModelOptions Options { get; set; } = null!;

    public List<SelectListItem> Users { get; set; } = [];

    public async Task OnGetAsync()
    {
        var userId = _userService.GetUserIdByClaimsPrincipal(User);
        var session = await _sessionService.GetSessionById(Id);

        if (userId != null && !session.UserIsSessionHost(Guid.Parse(userId)))
        {
            await _hubService.RedirectClientToPage(userId, $"/Lobby/{Id}");
        }

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