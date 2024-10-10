const EL = (Id) => {
    return document.getElementById(Id);
}

const show = (divs) => {
    loop(divs, true)
}

const hide = (divs) => {
    loop(divs, false)
}

const loop = (arr, display) => {
    for (const div of arr) {
        if(div)
        {
            if(display) {
                div.classList.remove("hide")
            } else {
                div.classList.add("hide")
            }
        }
    }
}

function gameModeChanged() 
{
    const selectedGameMode = EL("selectedGameMode")

    const pickAI = EL("pickAI")
    const lobbySize = EL("lobbyPlayerLimit")
    const PictureMode = EL("PictureMode")
    const UserOracleMode = EL("UserOracleMode")

    const infoOracleType = EL("infoOracleType")
    const infoOracleInput = EL("infoOracleInput")

    const selectOracleTypeContainer = EL("selectOracleTypeContainer")
    const selectOracleType = EL("selectOracleType")
    
    const selectPictureMode = EL("selectPictureMode")

    switch (parseInt(selectedGameMode.value)) {
        case 0: // SinglePayer
            // Oracle can only be AI in SinglePlayer
            infoOracleInput.value = "AI"
            selectOracleType.value = "0"  // Index for AI in OracleTypes
            selectPictureMode.value = "0" // Index for random picture mode

            show([infoOracleType, pickAI])
            hide([UserOracleMode, PictureMode, lobbySize, selectOracleTypeContainer])
            break

        case 1: //Duo
            // Oracle can only be a User in Duo
            infoOracleInput.value = "User"
            selectOracleType.value = "1" // Index for user in OracleTypes

            show([infoOracleType, UserOracleMode, PictureMode])
            hide([pickAI, lobbySize, selectOracleTypeContainer])
            break

        case 2: //FreeForAll
            show([selectOracleTypeContainer, PictureMode, lobbySize])
            hide([infoOracleType])

            // Adjust visibility of elements related to a 'parent'
            OracleTypeChange()
            break
        default:
            console.warn('GameMode invalid', selectedGameMode.value)
    }
}

function OracleTypeChange()
{
    const selectOracleType = EL("selectOracleType")

    const UserOracleMode = EL("UserOracleMode")
    const pickAI = EL("pickAI")

    if (selectOracleType.value == "0")  // Index for AI in OracleTypes
    {
        show([pickAI])
        hide([UserOracleMode])
    }

    if (selectOracleType.value == "1")  // Index for user in OracleTypes
    {
        show([UserOracleMode])
        hide([pickAI])
    }
}

function UserOracleModeChange()
{
    const selectUserOracleMode = EL("selectUserOracleMode")

    const ListOfUsersInSession = EL("ListOfUsersInSession")

    if(selectUserOracleMode.value == "0") // Index for random
    {
        hide([ListOfUsersInSession])
    }
    
    if (selectUserOracleMode.value == "1") // Index for chosen
    {
        show([ListOfUsersInSession])
    }
}

function lobbySizeChange() {
    var input = EL("lobbySize")
    var visibleNumber = EL("lobbySizeValue")
    
    visibleNumber.innerHTML = input.value
}