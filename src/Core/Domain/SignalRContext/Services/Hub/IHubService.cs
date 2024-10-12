
namespace Image_guesser.Core.Domain.SignalRContext.Services.Hub;


public interface IHubService
{
    Task RedirectGroupToPage(string sessionId, string link);
    Task RedirectOthersInGroupToPage(string sessionId, string userId, string link);
    Task RedirectClientToPage(string userId, string link);
    Task ReloadGroupPage(string sessionId);
    Task ReloadClientPage(string userId);
    Task ReloadConnectionPage(string connectionId);
    Task RemoveFromGroup(string connectionId, string sessionId);
    Task RemoveConnectionsFromGroup(HashSet<string> connectionIds, string sessionId);
    Task RemoveFromGroupAsync(string sessionId, string userId);
}