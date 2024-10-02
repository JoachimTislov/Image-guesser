using System.ComponentModel.DataAnnotations;
using Image_guesser.Core.Domain.OracleContext;

namespace Image_guesser.Core.Domain.SessionContext.ViewModels;

public class ViewModelOptions
{
    public ViewModelOptions() { }
    public ViewModelOptions(Options options)
    {
        NumberOfRounds = options.NumberOfRounds;
        LobbySize = options.LobbySize;
        GameMode = options.GameMode;
        RandomUserOracle = options.RandomUserOracle;
        Oracle = options.Oracle;
        AI_Type = options.AI_Type;
        RandomPictureMode = options.RandomPictureMode;
        ImageIdentifier = options.ImageIdentifier;
    }
    public int NumberOfRounds { get; set; }
    public int LobbySize { get; set; }
    public GameMode GameMode { get; set; }
    public bool RandomUserOracle { get; set; }
    public OracleTypes Oracle { get; set; }
    public AI_Type AI_Type { get; set; }
    public bool RandomPictureMode { get; set; }
    public string ImageIdentifier { get; set; } = string.Empty;

    public bool IsOracleAI()
    {
        return Oracle == OracleTypes.AI;
    }

    public bool IsGameMode(GameMode gameMode)
    {
        return GameMode == gameMode;
    }

}