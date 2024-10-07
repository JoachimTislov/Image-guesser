
namespace Image_guesser.Core.Domain.SignalRContext.Services.Hub;


public interface IHubService
{
    Task LeaveGroup(string userId, string SessionId);
    Task CloseGroup(string SessionId);
    Task RedirectGroupToPage(string sessionId, string link);
    Task RedirectClientToPage(string userConnection, string link);
}