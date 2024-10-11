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
            sendGuess(message, userId, sessionId, oracleId, gameId, guesserId, imageIdentifier);
        })
        answerInput.addEventListener("keypress", function(event) {
            if (event.key === "Enter") {
                guessButton.click();
            }
        });
    }
};

connection.on("ShowPiece", (piece) => {
  const chosenImage = availablePiecesOfImage.find(p => p == piece);

  AddNextImageTile(createImgElement(chosenImage))
});

connection.on("CorrectGuess", (winnerText, answer) => {
    document.getElementById("winningPlayer").textContent = winnerText;
    document.getElementById("modalAnswer").textContent = answer;

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

const createImgElement = (image, opacity = 1) => {
    const imageElement = document.createElement('img');
    const relativeImagePiecePath = image.replace('wwwroot', '');
    
    imageElement.src = `${relativeImagePiecePath}?v=${new Date().getTime()}`; // to prevent caching, lets user change size of image and get new images
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

  availablePiecesOfImage.forEach(image => {
    AddNextImageTile(createImgElement(image, 0.1));
  });
}

function AIGetImageTile()
{
  const newImagePiece = getRandomImage();
  if(newImagePiece)
  {
    return newImagePiece;
  }
}

function ExecuteOracleRevealTile() {
  var imageId = OracleRevealTile()
  oracleRevealedATile(oracleId, imageId);
}

function OracleRevealTile() {
  var newImagePiece = AIGetImageTile();
  AddNextImageTile(createImgElement(newImagePiece));

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

function InitGamePage()
{
  // Load tile-log from server
  for(var i = 0; i < imageTileOrderLog.length; i++)
  { 
    var newImagePiece = createImgElement(imageTileOrderLog[i]);
    AddNextImageTile(newImagePiece)

    // Remove the image from the available pieces
    var pieceIndex = availablePiecesOfImage.indexOf(imageTileOrderLog[i])
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

    if(imageCoordinates != "ImageCoordinates not found")
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