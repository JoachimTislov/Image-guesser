namespace Image_guesser.Core.Domain.SignalRContext.Hubs;
public interface IGameServer
{
    //--------------- Client To Server ---------------//

    // Services invoked from client 
    Task ClientNavigatedToPage(string pathName);
    Task AddToGroup(string sessionId, string userId);
    Task JoinSession(string sessionId, string userId);
    Task LeaveSession(string userId, string sessionId);
    Task CloseSession(string sessionId);
    Task SendGuess(string guess, string userId, string sessionId, string oracleId,
            string gameId, string guesserId, string imageIdentifier, string timeOfGuess);
    Task OracleRevealedATile(string oracleId, string imageId);
    Task ShowThisPiece(string pieceId, string sessionId);
    Task ShowNextPieceForAll(string sessionId);

}