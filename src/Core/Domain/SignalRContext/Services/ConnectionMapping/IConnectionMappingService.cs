namespace Image_guesser.Core.Domain.SignalRContext.Services.ConnectionMapping;

public interface IConnectionMappingService
{
    Task AddConnection(string userId, string connectionId);

    Task RemoveConnection(string userId, string connectionId);

    string GetConnection(string userId);

    Task AddToGroup(string userId, string groupId);

    Task RemoveFromGroup(string userId, string groupId);

    string GetGroupId(string userId);

}