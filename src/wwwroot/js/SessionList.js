import { joinSession } from "./gameHub.js";

document.querySelectorAll('.joinSessionButton').forEach(button => {
    button.addEventListener('click', function () {
        const sessionId = button.getAttribute("session-Id")
        const userId = button.getAttribute("user-Id")
        joinSession(sessionId, userId);
    });
});