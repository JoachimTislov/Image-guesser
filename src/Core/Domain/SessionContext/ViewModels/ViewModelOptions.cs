using Image_guesser.Core.Domain.OracleContext;

namespace Image_guesser.Core.Domain.SessionContext.ViewModels;

public class ViewModelOptions
{
    public ViewModelOptions() { }
    public ViewModelOptions(Options options)
    {
        LobbySize = options.LobbySize;
        GameMode = options.GameMode;
        OracleType = options.OracleType;
        AI_Type = options.AI_Type;
        PictureMode = options.PictureMode;
    }

    // Used temporary to assign values correctly in session's options
    public int LobbySize { get; set; }
    public GameMode GameMode { get; set; }
    public OracleTypes OracleType { get; set; }
    public UserOracleMode UserOracleMode { get; set; }
    public string SelectedUserId { get; set; } = string.Empty;
    public AI_Type AI_Type { get; set; }
    public PictureMode PictureMode { get; set; }

    public bool IsOracleAI()
    {
        return OracleType == OracleTypes.AI;
    }

    public bool IsGameMode(GameMode gameMode)
    {
        return GameMode == gameMode;
    }

}