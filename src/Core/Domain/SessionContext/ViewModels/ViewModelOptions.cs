using Image_guesser.Core.Domain.OracleContext;

namespace Image_guesser.Core.Domain.SessionContext.ViewModels;

public class ViewModelOptions
{
    public ViewModelOptions() { }
    public ViewModelOptions(Options options)
    {
        LobbySize = options.LobbySize;
        GameMode = options.GameMode;
        RandomUserOracle = options.RandomUserOracle;
        Oracle = options.Oracle;
        AI_Type = options.AI_Type;
        PictureMode = options.PictureMode;
        ImageIdentifier = options.ImageIdentifier;
    }
    public int LobbySize { get; set; }
    public GameMode GameMode { get; set; }
    public bool RandomUserOracle { get; set; }
    public OracleTypes Oracle { get; set; }
    public AI_Type AI_Type { get; set; }
    public PictureMode PictureMode { get; set; }
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