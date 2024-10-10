
namespace Image_guesser.Core.Domain.SignalRContext.Services.Hub;


public interface IHubService
{
    Task RedirectGroupToPage(string sessionId, string link);
    Task RedirectClientToPage(string userId, string link);
    Task ReloadGroupPage(string sessionId);
    Task ReloadClientPage(string userId);
}