namespace Image_guesser.Core.Domain.GameContext.Responses;

public record InitializeGameResponse(bool Success, Guid Id, Guid SessionId);
