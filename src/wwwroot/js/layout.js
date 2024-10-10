import { AddToGroup } from './gameHub.js'

const activeLobbyLink = document.getElementById("ActiveLobby")

if (activeLobbyLink)
{
    activeLobbyLink.addEventListener("click", () =>{
        var sessionId = activeLobbyLink.getAttribute("session-Id");
        var userId = activeLobbyLink.getAttribute("user-Id");
        AddToGroup(sessionId, userId)
    })
}