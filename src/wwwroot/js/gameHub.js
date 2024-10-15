export var isConnected = false;

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/gameHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start().then(()=> {
    console.log("SignalR Connected");
    isConnected = true;
    
    var pathName = sessionStorage.getItem("pathName");

    // Avoid spamming the server
    if(pathName && pathName != window.location.pathname)
    {
        connection.invoke("ClientNavigatedToPage", window.location.pathname).catch(err => console.error(err.toString()));
    }
    sessionStorage.setItem("pathName", window.location.pathname);
}).catch(err => console.error(err.toString()));


export function AddToGroup(sessionId, userId) {
    connection.invoke("AddToGroup", sessionId, userId).catch(err => console.error(err.toString()));
}

export function joinSession(sessionId, userId) {
    connection.invoke("JoinSession", sessionId, userId).catch(err => console.error(err.toString()));
}

export function leaveSession(sessionId, userId) {
    connection.invoke("LeaveSession", sessionId, userId).catch(err => console.error(err.toString()));
}

export function closeSession(sessionId) {
    connection.invoke("CloseSession", sessionId).catch(err => console.error(err.toString()));
}

export function sendGuess(message, userId, sessionId, oracleId, gameId, guesserId, imageIdentifier, timeOfGuess) {
    connection.invoke("SendGuess", message, userId, sessionId, oracleId, gameId, guesserId, imageIdentifier, timeOfGuess).catch(err => console.error(err.toString()));
}

export function oracleRevealedATile(oracleId, imageId) {
    connection.invoke("OracleRevealedATile", oracleId, imageId).catch(err => console.error(err.toString()));
}

export function showThisPiece(pieceId, sessionId) {
    connection.invoke("ShowThisPiece", pieceId, sessionId).catch(err => console.error(err.toString()));
}

export function showPieceForAllPlayers(sessionId) {
    connection.invoke("ShowNextPieceForAll", sessionId).catch(err => console.error(err.toString()));
}

// Temp function to test if the connection works
connection.on("ReceiveMessage", (message) => {
    var chatDiv = document.getElementById("chatDiv");
    var messageDiv = document.createElement("p");
    messageDiv.textContent = message;
    chatDiv.appendChild(messageDiv);
});

// These functions are used to send a user from to the link or force a reload of the current page
// and serve as a way to reload the page for everyone in the SignalR groups
connection.on("RedirectToLink", (link) => {
    window.location.href = link
});

// Pretty much serves the same purpose as above just by reloading the current page instead of redirecting
connection.on("ReloadPage", () => {
    window.location.reload();
});

connection.on("RefreshThisPageIfClientPresent", (pathname) => {
    if(window.location.pathname == pathname)
    {
        window.location.reload();
    }
});

export { connection };