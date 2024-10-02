import { joinGroup } from "./gameHub.js";

document.querySelectorAll('.joinSessionButton').forEach(button => {
    button.addEventListener('click', function () {
        const sessionId = button.getAttribute("session-Id")
        joinGroup(sessionId);
    });
});