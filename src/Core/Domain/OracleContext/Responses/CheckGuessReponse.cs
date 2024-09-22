namespace Image_guesser.Core.Domain.OracleContext.Responses;

public record Check_Guess_Response(bool IsGuessCorrect, string WinnerText);