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
        if(display) {
            div.classList.remove("hide")
        } else {
            div.classList.add("hide")
        }
    }
}

function gameModeChanged() 
{
    const selectedGameMode = EL("selectedGameMode")

    const pickAI = EL("pickAI")
    const lobbySize = EL("lobbyPlayerLimit")
    const RandomUserOracle = EL("RandomUserOracle")
    const PictureMode = EL("PictureMode")
    const ImageContainer = EL("ImageContainer") 

    const infoOracleType = EL("infoOracleType")
    const infoOracleInput = EL("infoOracleInput")

    const selectOracleTypeContainer = EL("selectOracleTypeContainer")
    const selectOracleType = EL("selectOracleType")

    switch (parseInt(selectedGameMode.value)) {
        case 0: // SinglePayer
            // Oracle can only be AI in SinglePlayer
            infoOracleInput.value = "AI"
            selectOracleType.value = "0"  // Index for AI in OracleTypes

            show([infoOracleType, pickAI])
            hide([RandomUserOracle, PictureMode, lobbySize, ImageContainer, selectOracleTypeContainer])
            break

        case 1: //Duo
            // Oracle can only be a User in Duo
            infoOracleInput.value = "User"
            selectOracleType.value = "1" // Index for user in OracleTypes

            show([infoOracleType, RandomUserOracle, PictureMode])
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

    // Adjust visibility of elements related to a 'parent'
    TogglePictureMode()
}

function OracleTypeChange()
{
    const selectOracleType = EL("selectOracleType")

    const RandomUserOracle = EL("RandomUserOracle")
    const pickAI = EL("pickAI")

    if (selectOracleType.value == "0")  // Index for AI in OracleTypes
    {
        show([pickAI])
        hide([RandomUserOracle])
    }

    if (selectOracleType.value == "1")  // Index for user in OracleTypes
    {
        show([RandomUserOracle])
        hide([pickAI])
    }
}

function TogglePictureMode()
{
    const selectPictureMode = EL("selectPictureMode")
    const ImageContainer = EL("ImageContainer")

    const selectedGameMode = EL("selectedGameMode")

    // Checks if its Random picture mode or its singleplayer, 
    //selectPicture should be random if the game mode is singleplayer
    if(selectPictureMode.value == "0" || selectedGameMode.value == "0") {
        hide([ImageContainer])
    } else {
        show([ImageContainer])
    }
}

function UserSelectedImage(Link, Name, Identifier)
{
    const selectedImageEL = EL("selectedImageEL")
    const selectedImageName = EL("selectedImageName")
    const selectedImageContainer = EL("selectedImageContainer")
 
    show([selectedImageContainer])

    selectedImageEL.src = `/${Link}`
    selectedImageName.value = Name

    const imageLink = EL("imageLink")
    const imageIdentifier = EL("imageIdentifier")

    imageLink.value = Link
    imageIdentifier.value = Identifier
}

function numberOfGamesChange() {
    var input = EL("numberOfGames")
    var visibleNumber = EL("numberOfGamesValue")
    
    visibleNumber.innerHTML = input.value
}

function lobbySizeChange() {
    var input = EL("lobbySize")
    var visibleNumber = EL("lobbySizeValue")
    
    visibleNumber.innerHTML = input.value
}