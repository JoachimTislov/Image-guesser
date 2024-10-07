using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext.Events;

public record PlayerGuessed : BaseDomainEvent
{
    public PlayerGuessed(Guid oracleId, string guess, Guid guesserId, Guid gameId, Guid sessionId)
    {
        OracleId = oracleId;
        Guess = guess;
        GuesserId = guesserId;
        GameId = gameId;
        SessionId = sessionId;
    }
    public Guid OracleId { get; }
    public string Guess { get; }
    public Guid GuesserId { get; }
    public Guid GameId { get; }
    public Guid SessionId { get; }
}