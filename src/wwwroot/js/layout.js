import { createGroup } from './gameHub.js'

const activeLobbyLink = document.getElementById("ActiveLobby")

if (activeLobbyLink)
{
    activeLobbyLink.addEventListener("click", () =>{
        var sessionId = activeLobbyLink.getAttribute("session-Id");
        createGroup(sessionId)
    })
}