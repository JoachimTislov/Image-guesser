namespace Image_guesser.Core.Domain.SignalRContext.Hubs;
public interface IGameServer
{
    //--------------- Client To Server ---------------//

    // Services invoked from client 

    Task AddToGroup(string sessionId);
    Task JoinSession(string sessionId, string userId);
    Task SendGuess(string guess, string userId, string sessionId, string oracleId,
            string gameId, string guesserId, string imageIdentifier);
    Task OracleRevealedATile(string oracleId);
    Task ShowThisPiece(string pieceId, string sessionId);
    Task ShowNextPieceForAll(string sessionId);
}