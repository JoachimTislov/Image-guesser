using System.Collections.Concurrent;

namespace Image_guesser.Core.Domain.SignalRContext.Services.ConnectionMapping;

public class ConnectionMappingService : IConnectionMappingService
{
    private readonly ConcurrentDictionary<string, string> _connections = new();
    private readonly ConcurrentDictionary<string, HashSet<string>> _groups = new();

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

    public Task AddToGroup(string sessionId, string connectionId)
    {
        if (!_groups.ContainsKey(sessionId))
        {
            _groups[sessionId] = [];
        }

        _groups[sessionId].Add(connectionId);

        return Task.CompletedTask;
    }

    public Task RemoveFromGroup(string sessionId, string connectionId)
    {
        if (_groups.ContainsKey(sessionId))
        {
            _groups[sessionId].Remove(connectionId);
        }

        return Task.CompletedTask;
    }

    public Task DeleteGroup(string sessionId)
    {
        if (_groups.ContainsKey(sessionId))
        {
            _groups.TryRemove(sessionId, out _);
        }

        return Task.CompletedTask;
    }

    public HashSet<string>? GetGroupConnections(string sessionId)
    {
        return _groups.TryGetValue(sessionId, out var value) ? value : null;
    }

}