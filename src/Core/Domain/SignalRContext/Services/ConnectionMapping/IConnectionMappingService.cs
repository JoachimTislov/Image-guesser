namespace Image_guesser.Core.Domain.SignalRContext.Services.ConnectionMapping;

public interface IConnectionMappingService
{
    Task AddConnection(string userId, string connectionId);

    Task RemoveConnection(string userId, string connectionId);

    string GetConnection(string userId);

    Task AddToGroup(string sessionId, string connectionId);

    Task RemoveFromGroup(string sessionId, string connectionId);

    Task DeleteGroup(string sessionId);

    HashSet<string>? GetGroupConnections(string sessionId);
}