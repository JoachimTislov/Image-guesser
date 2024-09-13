using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext.Events;

public record PlayerGuessed : BaseDomainEvent
{
    public PlayerGuessed(string guess, string guesserId, string gameId)
    {
        Guess = guess;
        GuesserId = guesserId;
        GameId = gameId;
    }
    public string Guess { get; }
    public string GuesserId { get; }
    public string GameId { get; }
}