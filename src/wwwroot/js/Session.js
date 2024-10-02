import { closeGroup, leaveGroup } from "./gameHub.js";

const closeLobbyButton = document.getElementById("closeLobbyButton");

if (closeLobbyButton)
{
    closeLobbyButton.addEventListener("click", function () {
        closeGroup(sessionId);
    });
}

const leaveLobbyButton = document.getElementById("leaveLobbyButton");

if(leaveLobbyButton)
{
    leaveLobbyButton.addEventListener("click", function () {
        leaveGroup(userId, sessionId);
    });
}

document.querySelectorAll('.kickButton').forEach(button => {
    button.addEventListener('click', function () {
        const userId = button.getAttribute("user-Id")
        leaveGroup(userId, sessionId);
    });
});