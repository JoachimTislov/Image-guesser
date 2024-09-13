namespace Image_guesser.Core.Domain.GameContext;

public class Guesser(string name, Guid gameId)
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; } = gameId;
    public string Name { get; set; } = name;
    public int Points { get; set; }
    public TimeSpan TimeSpan { get; set; }
    public int Guesses { get; set; }
    public int WrongGuessCounter { get; set; }
}
