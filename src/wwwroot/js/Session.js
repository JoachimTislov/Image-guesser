import { leaveSession, closeSession } from './gameHub.js'

var closeSessionButton = document.getElementById("CloseSession");

if(closeSessionButton)
{
    closeSessionButton.addEventListener("click", () => {
        closeSession(sessionId);
    });
}

document.querySelectorAll('.removeUserButton').forEach(button => {
    button.addEventListener('click', function () {
        const userId = button.getAttribute("user-Id");
        leaveSession(userId, sessionId);
    });
});