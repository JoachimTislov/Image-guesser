import {
    sendGuess, oracleRevealedATile, showThisPiece, 
    connection, isConnected, showPieceForAllPlayers
} from "./gameHub.js";


//************************* Image size logic ***************************************/
window.addEventListener('resize', function () {
   // console.log('The window was resized', 'width: ' + window.innerWidth, 'height: ', window.innerHeight);

    updateImageContainerSizeValues()
});

function updateImageContainerSizeValues()
{
    var imageContainerHeightEL = document.getElementById("imageContainerHeight");
    var imageContainerWidthEL = document.getElementById("imageContainerWidth");

    var imageList = document.getElementById("imageList");
    if(imageList && imageContainerHeightEL && imageContainerWidthEL)
    {
        imageContainerHeightEL.value = imageList.offsetHeight;
        imageContainerWidthEL.value = imageList.offsetWidth;

        console.log('ImageContainer; height: ' + imageContainerHeightEL.value, 'width: ' + imageContainerWidthEL.value);
    }
}

updateImageContainerSizeValues()
/****************************************************/

const imageContainer = document.getElementById('imageList');

const UserIsOracle = oracleEntityId == userId;
const userIsAGuesser = guesserId != null; 

if(userIsAGuesser) {
    var answerInput = document.getElementById("answerInput");
    var guessButton = document.getElementById("guessButton");
     if(guessButton && answerInput) {
        guessButton.addEventListener("click", () => {
            var message = answerInput.value;
            sendGuess(message, userId, sessionId, oracleId, gameId, guesserId, imageIdentifier, getTime());
        })
        answerInput.addEventListener("keypress", function(event) {
            if (event.key === "Enter") {
                guessButton.click();
            }
        });
    }
};

connection.on("ShowPiece", (pieceId) => {
  //const chosenPieceId = availablePiecesOfImage.find(p => p.split("/").pop() == piece);

  const chosenPiecePath = `${urlToCorrectImageTilesDirectory}/${pieceId}`;

  console.log("Showing piece: " + chosenPiecePath);

  AddNextImageTile(createImgElement(chosenPiecePath))
});

// Handles functions for receiving and displaying guesses
connection.on("ReceiveGuess", (guess, playerName) => {
    AddGuessToChat(guess, playerName, getTime());
})

function AddGuessToChat(guess, playerName, timeOfGuess) {
  var guessingDiv = document.getElementById("guessingDiv");

  var guessContainer = CreateElement("div");
  guessContainer.classList.add("d-flex");

  guessContainer.appendChild(InitPlayerPElement(playerName, timeOfGuess));
  guessContainer.appendChild(InitGuessPElement(guess));
  guessingDiv.appendChild(guessContainer);

  guessingDiv.scrollTop = guessingDiv.scrollHeight;
}

function InitPlayerPElement(playerName, timeOfGuess)
{
  var playerNameP = CreateElement("p");
  playerNameP.classList.add("me-auto");

  playerNameP.textContent = `[ ${timeOfGuess} ] ${playerName}:`;

  return playerNameP;
}

function InitGuessPElement(guess)
{
  var guessP = CreateElement("p");
  guessP.classList.add("ms-auto");
  guessP.textContent = guess;

  return guessP;
}

const CreateElement = (elementName) => document.createElement(elementName);

function getTime()
{
    const now = new Date();
    const hours = now.getHours(); 
    const minutes = now.getMinutes(); 
    const seconds = now.getSeconds(); 

    const AddAZeroIfUnderTen = (value) => {
        if (value < 10) {
            return "0" + value;
        }
        return value;
    }

    return `${AddAZeroIfUnderTen(hours)}:${AddAZeroIfUnderTen(minutes)}:${AddAZeroIfUnderTen(seconds)}`;
}

connection.on("CorrectGuess", (winnerText, guess, nameOfImage) => {
    document.getElementById("winningPlayer").textContent = winnerText;
    document.getElementById("modalAnswer").textContent = `Answer: ${nameOfImage}`;
    document.getElementById("modalGuess").textContent = `Guess: ${guess}`;

    var myModal = document.getElementById("victoryModal")
    if(myModal)
    {
      var victoryModal = new bootstrap.Modal(myModal, {
        backdrop: 'static',
        keyboard: false
      });
      victoryModal.show();
    }
});

connection.on("ShowNextPieceForAll", () => {    
    ExecuteOracleRevealTile()
});

const createImgElement = (imagePiecePath, opacity = 1) => {
    const imageElement = document.createElement('img');

    imageElement.src = `${imagePiecePath}?v=${new Date().getTime()}`; // to prevent caching, lets user change size of image and get new images
    imageElement.style.position = 'absolute';
    imageElement.style.opacity = opacity;

    imageElement.style.objectFit = "cover";

    return imageElement
}

function AddNextImageTile(imgElement) {
  imageContainer.appendChild(imgElement);
}

function InitUserAsOracle() { // imageInteractionInitializer
  const isPointInNonTransparentArea = (x, y, nonTransparentPixels) => {
      return nonTransparentPixels.some(pixel => pixel.Item1 == x && pixel.Item2 == y);
  }

  imageContainer.addEventListener('click', function(event) {
      let x = event.offsetX;
      let y = event.offsetY;

      for (let i = 0; i < imageCoordinates.length; i++) {
          let piece = imageCoordinates[i];

          if (isPointInNonTransparentArea(x, y, piece.Item2)) {

              showThisPiece(piece.Item1, sessionId);

              oracleRevealedATile(oracleId, piece.Item1);
              
              imageCoordinates.splice(i, 1);
              break;
          }
      }
  })

  availablePiecesOfImage.forEach(imagePath => {
    AddNextImageTile(createImgElement(imagePath, 0.1));
  });
}

function ExecuteOracleRevealTile() {
  var imageId = OracleRevealTile()
  oracleRevealedATile(oracleId, imageId);
}

function OracleRevealTile() {
  const newImagePiece = getRandomImage();
  if(newImagePiece)
  {
    AddNextImageTile(createImgElement(newImagePiece));
  }
  
  return newImagePiece;
} 

function getRandomImage() {
    if (Array.isArray(oracleAI_Array_NumbersForImagePieces))
    {
        const randomIndex = oracleAI_Array_NumbersForImagePieces.shift();
        return availablePiecesOfImage[randomIndex];
    }
    if (availablePiecesOfImage.length == 0) {
        console.warn("No more images to show");
    }
    return null;
}

var showNextPieceForAllButton = document.getElementById("ShowNextPieceForAll");
if (showNextPieceForAllButton && gameMode == "FreeForAll" && oracleIsAI) {
  showNextPieceForAllButton.addEventListener("click", () => {
      showPieceForAllPlayers(sessionId);
  });
}

var showOneMoreButton = document.getElementById("showOneMore"); 
if (oracleIsAI && showOneMoreButton) {
  showOneMoreButton.addEventListener("click", () => {
      ExecuteOracleRevealTile();
  });
}

function InitImageLog()
{
  if(imageTileOrderLog != null)
  {
    // Load tile-log from server
    for(var i = 0; i < imageTileOrderLog.length; i++)
    { 
      var pathToImage = `${urlToCorrectImageTilesDirectory}/${imageTileOrderLog[i]}`;

      var newImagePiece = createImgElement(pathToImage);
      AddNextImageTile(newImagePiece)

      // Remove the image from the available pieces
      var pieceIndex = availablePiecesOfImage.indexOf(pathToImage)
      if(pieceIndex != -1)
      {
        // Remove the image tile indexes from the AI array
        if(oracleIsAI && oracleAI_Array_NumbersForImagePieces)
        {
          var indexToRemove = oracleAI_Array_NumbersForImagePieces.indexOf(availablePiecesOfImage.length - 1);
          oracleAI_Array_NumbersForImagePieces.splice(indexToRemove, 1);
        }

        availablePiecesOfImage.splice(pieceIndex, 1);
      }

      // Remove the correlating coordinates
      if(imageCoordinates != "ImageCoordinates not found" && imageCoordinates != null)
      { 
        for(var j = 0; j < imageCoordinates.length; j++)
        { 
          if(imageCoordinates[j].Item1 == imageTileOrderLog[i])
          {
            imageCoordinates.splice(j, 1);
          }
        }
      }
    }
  }
}

function InitGuessLog()
{
  for(var i = 0; i < guessLog.length; i++)
  {
    AddGuessToChat(guessLog[i].GuessMessage, guessLog[i].NameOfGuesser, guessLog[i].TimeOfGuess);
  }
}

function InitGamePage()
{
  InitImageLog()
  InitGuessLog()

  // User is Oracle
  if(UserIsOracle) {
    InitUserAsOracle();
    return;
  }
  else if(oracleIsAI && numberOfTilesRevealed == 0) {
    ExecuteOracleRevealTile();
  }

  console.log("Game initialized")
}

window.onload = function() {
  function waitForConnection() {
      if (isConnected) {
        InitGamePage()
      }
      else
      {
        setTimeout(waitForConnection, 100);
      }
  }
  waitForConnection();
}