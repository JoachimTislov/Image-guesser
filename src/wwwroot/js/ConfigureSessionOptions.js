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

function gameModeChanged() {
    const selectedGameMode = EL("selectedGameMode")

    const pickAI = EL("pickAI")
    const lobbySize = EL("lobbyPlayerLimit")
    const RandomUserOracle = EL("RandomUserOracle")
    const RandomPictureMode = EL("RandomPictureMode")
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
            hide([RandomUserOracle, RandomPictureMode, lobbySize, ImageContainer, selectOracleTypeContainer])
            break

        case 1: //Duo
            // Oracle can only be a User in Duo
            infoOracleInput.value = "User"
            selectOracleType.value = "1" // Index for user in OracleTypes

            show([infoOracleType, RandomUserOracle, RandomPictureMode])
            hide([pickAI, lobbySize, selectOracleTypeContainer])
            break

        case 2: //FreeForAll
            show([selectOracleTypeContainer, RandomPictureMode, lobbySize])
            hide([infoOracleType])

            // Adjust visibility of elements related to a 'parent'
            OracleTypeChange()
            break
        default:
            console.warn('GameMode invalid', selectedGameMode.value)
    }

    // Adjust visibility of elements related to a 'parent'
    ToggleRandomPictureMode()
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

function ToggleRandomPictureMode() {
    const RandomPictureMode = EL("randomPictureCheckBox")
    const ImageContainer = EL("ImageContainer")

    const selectedGameMode = EL("selectedGameMode")

    if(RandomPictureMode.checked || selectedGameMode.value == "0") {
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

function numberOfRoundsChange() {
    var input = EL("numberOfRounds")
    var visibleNumber = EL("numberOfRoundsValue")
    
    visibleNumber.innerHTML = input.value
}

function lobbySizeChange() {
    var input = EL("lobbySize")
    var visibleNumber = EL("lobbySizeValue")
    
    visibleNumber.innerHTML = input.value
}