import { createGroup } from "./gameHub.js";

const createSessionButton = document.getElementById("createSession")
if(createSessionButton)
{
    createSessionButton.addEventListener("click", () => {
        var sessionId = createSessionButton.getAttribute("session-Id")
        createGroup(sessionId)
    })
}