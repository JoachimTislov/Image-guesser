using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext.Events;

public record PlayerGuessed : BaseDomainEvent
{
    public PlayerGuessed(Guid sessionId, string guess, string guesserId, Guid gameId)
    {
        SessionId = sessionId;
        Guess = guess;
        GuesserId = guesserId;
        GameId = gameId;
    }
    public Guid SessionId { get; }
    public string Guess { get; }
    public string GuesserId { get; }
    public Guid GameId { get; }
}