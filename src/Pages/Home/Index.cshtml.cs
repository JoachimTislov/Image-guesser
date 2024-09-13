using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Image_guesser.Pages.Home;

public class IndexModel(ILogger<IndexModel> logger) : PageModel
{
    private readonly ILogger<IndexModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
