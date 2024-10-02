using System.Collections.Concurrent;

namespace Image_guesser.Core.Domain.SignalRContext.Services.ConnectionMapping;

public class ConnectionMappingService : IConnectionMappingService
{
    private readonly ConcurrentDictionary<string, string> _connections = new();
    private readonly ConcurrentDictionary<string, string> _groups = new();

    public Task AddConnection(string userId, string connectionId)
    {
        _connections[userId] = connectionId;

        return Task.CompletedTask;
    }

    public Task RemoveConnection(string userId, string connectionId)
    {
        return RemoveAssociation(_connections, userId, connectionId);
    }

    public string GetConnection(string userId)
    {
        return GetValueOrDefault(_connections, userId);
    }

    public Task AddToGroup(string userId, string groupId)
    {
        // This will add the user with the groupId or update the existing one
        _groups.AddOrUpdate(userId, groupId, (key, oldValue) => groupId);

        return Task.CompletedTask;
    }

    public Task RemoveFromGroup(string userId, string groupId)
    {
        return RemoveAssociation(_groups, userId, groupId);
    }

    public string GetGroupId(string userId)
    {
        return GetValueOrDefault(_groups, userId);
    }

    private static string GetValueOrDefault(ConcurrentDictionary<string, string> dictionary, string userId)
    {
        if (dictionary.TryGetValue(userId, out var value))
        {
            return value;
        }
        else
        {
            return string.Empty;
        }
    }

    private static Task RemoveAssociation(ConcurrentDictionary<string, string> dictionary, string userId, string associationId)
    {
        if (dictionary.TryGetValue(userId, out var value) && value == associationId)
        {
            dictionary.TryRemove(userId, out _);
        }

        return Task.CompletedTask;
    }
}