import { AddToGroup } from "./gameHub.js";

const createSessionButton = document.getElementById("createSession")
if(createSessionButton)
{
    createSessionButton.addEventListener("click", () => {
        var sessionId = createSessionButton.getAttribute("session-Id");
        var userId = createSessionButton.getAttribute("user-Id");
        AddToGroup(sessionId, userId)
    })
}
