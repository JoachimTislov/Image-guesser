using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext;

// Games lifecycle 
public record CreateGame(Guid SessionId, string? ImageIdentifier = null) : BaseDomainEvent;
public record GameTerminated(Guid GameId, Guid SessionId) : BaseDomainEvent;
public record GameFinished(Guid GameId, Guid SessionId) : BaseDomainEvent;


// Player Actions
public record PlayerGuessed(string ImageIdentifier, Guid OracleId, string Guess, Guid GuesserId, Guid GameId, Guid SessionId, string Username, string TimeOfGuess) : BaseDomainEvent;
public record PlayerGuessedCorrectly(Guid GuesserId, int Points, Guid GameId, Guid SessionId) : BaseDomainEvent;
public record PlayerGuessedIncorrectly(Guid GuesserId, Guid GameId) : BaseDomainEvent;
public record PlayerLeftGame(string UserId, Guid? GuesserId, Guid GameId, Guid SessionId) : BaseDomainEvent;