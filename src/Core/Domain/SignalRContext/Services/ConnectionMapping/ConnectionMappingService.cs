using System.Collections.Concurrent;

namespace Image_guesser.Core.Domain.SignalRContext.Services.ConnectionMapping;

public class ConnectionMappingService : IConnectionMappingService
{
    private readonly ConcurrentDictionary<string, string> _connections = new();

    public Task AddConnection(string userId, string connectionId)
    {
        _connections[userId] = connectionId;

        return Task.CompletedTask;
    }

    public Task RemoveConnection(string userId, string connectionId)
    {
        if (_connections.TryGetValue(userId, out var value) && value == connectionId)
        {
            _connections.TryRemove(userId, out _);
        }

        return Task.CompletedTask;
    }

    public string GetConnection(string userId)
    {
        if (_connections.TryGetValue(userId, out var value))
        {
            return value;
        }
        else
        {
            return string.Empty;
        }
    }
}