export var isConnected = false;

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/gameHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start().then(()=> {
    console.log("SignalR Connected");
    isConnected = true;
}).catch(err => console.error(err.toString()));

export function AddToGroup(sessionId, userId) {
    connection.invoke("AddToGroup", sessionId, userId).catch(err => console.error(err.toString()));
}

export function joinSession(sessionId, userId) {
    connection.invoke("JoinSession", sessionId, userId).catch(err => console.error(err.toString()));
}

export function updateSessionSettings(options, sessionId) {
    connection.invoke("UpdateSessionSettings", options, sessionId).catch(err => console.error(err.toString()));
}

export function leaveSession(sessionId, userId) {
    connection.invoke("LeaveSession", sessionId, userId).catch(err => console.error(err.toString()));
}

export function closeSession(sessionId) {
    connection.invoke("CloseSession", sessionId).catch(err => console.error(err.toString()));
}

export function sendGuess(message, userId, sessionId, oracleId, gameId, guesserId, imageIdentifier) {
    console.log("Sending guess: " + message + " to game: " + gameId + " from guesser: " + guesserId);
    connection.invoke("SendGuess", message, userId, sessionId, oracleId, gameId, guesserId, imageIdentifier).catch(err => console.error(err.toString()));
}

export function oracleRevealedATile(oracleId, imageId) {
    console.log("Oracle with Id: " + oracleId + " revealed a tile", "ImageId: " + imageId);
    connection.invoke("OracleRevealedATile", oracleId, imageId).catch(err => console.error(err.toString()));
}

export function showThisPiece(pieceId, sessionId) {
    console.log("We are trying to show this piece")
    connection.invoke("ShowThisPiece", pieceId, sessionId).catch(err => console.error(err.toString()));
}

export function showPieceForAllPlayers(sessionId) {
    console.log("We are trying to show a piece to everyone")
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
    console.log(`Redirected to: ${link}`);
    window.location.href = link
});

// Pretty much serves the same purpose as above just by reloading the current page instead of redirecting
connection.on("ReloadPage", () => {
    window.location.reload();

    console.log("Reloaded the page.. ")
});

// Handles functions for receiving and displaying guesses
connection.on("ReceiveGuess", (guess, playerName) => {
    var guessingDiv = document.getElementById("guessingDiv");

    var guessContainer = document.createElement("div");
    guessContainer.classList.add("d-flex");

    var playerNameP = document.createElement("p");
    playerNameP.classList.add("me-auto");
    playerNameP.textContent = `[ ${getTime()} ] ${playerName}:`;
    guessContainer.appendChild(playerNameP);

    var guessP = document.createElement("p");
    guessP.classList.add("ms-auto");
    guessP.textContent = guess;
    guessContainer.appendChild(guessP);

    guessingDiv.appendChild(guessContainer);

    guessingDiv.scrollTop = guessingDiv.scrollHeight;
})

function getTime()
{
    const now = new Date();
    const hours = now.getHours(); 
    const minutes = now.getMinutes(); 
    const seconds = now.getSeconds(); 

    return `${AddAZeroIfUnderTen(hours)}:${AddAZeroIfUnderTen(minutes)}:${AddAZeroIfUnderTen(seconds)}`;
}

function AddAZeroIfUnderTen(value) {
    if (value < 10) {
        return "0" + value;
    }
    return value;
}

export { connection };