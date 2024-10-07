import {
    sendGuess, oracleRevealedATile, showThisPiece, 
    connection, isConnected, showPieceForAllPlayers
} from "./gameHub.js";

const imageContainer = document.getElementById('imageList');
const displayedImages = [];

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
  const imgIndex = availablePiecesOfImage.indexOf(chosenImage);

  if (imgIndex == -1) return;

  availablePiecesOfImage.splice(imgIndex, 1);

  displayedImages.push(chosenImage);

  if (userIsAGuesser) {
    renderImages();
  } else {
    renderUserOracleImage();
  }

});

connection.on("CorrectGuess", (winnerText, answer) => {
  document.getElementById("winningPlayer").textContent = winnerText;
  document.getElementById("modalAnswer").textContent = answer;

  var myModal = document.getElementById('victoryModal')

  var victoryModal = new bootstrap.Modal(myModal);
  victoryModal.show();
});


connection.on("ShowNextPieceForAll", () => {    
    ExecuteOracleRevealTile()
});

// Renders guessers image
function renderImages() {
  imageContainer.innerHTML = '';

  displayedImages.forEach(image => {
    imageContainer.appendChild(createImgElement(image));
  });

  return imageContainer
}

const createImgElement = (image, opacity = 1) => {
    const imageElement = document.createElement('img');
    const relativeImagePiecePath = image.replace('wwwroot', '');
    
    imageElement.src = relativeImagePiecePath;
    imageElement.style.position = 'absolute';
    imageElement.style.opacity = opacity;

    imageElement.style.objectFit = "cover";

    return imageElement
}

function renderUserOracleImage() {
  const image_container = renderImages()

  availablePiecesOfImage.forEach(image => {
    image_container.appendChild(createImgElement(image, 0.1));
  });
}

function InitUserAsOracle() { // imageInteractionInitializer
  const isPointInNonTransparentArea = (x, y, nonTransparentPixels) => {
      return nonTransparentPixels.some(pixel => pixel.Item1 == x && pixel.Item2 == y);
  }

  imageContainer.addEventListener('click', function(event) {
      let x = event.offsetX;
      let y = event.offsetY;

      for (let piece of imageCoordinates) {
          if (isPointInNonTransparentArea(x, y, piece.Item2)) {

              showThisPiece(piece.Item1, sessionId);

              oracleRevealedATile(oracleId);
          }
      }
  })

  renderUserOracleImage()
}

function ExecuteOracleRevealTile() {
  const result = AddNextImageTile()
  if(result)
  {
    oracleRevealedATile(oracleId);

    renderImages();
  }
}

function AddNextImageTile()
{
  const newImagePiece = getRandomImage();

  if (newImagePiece != null) {
    displayedImages.push(newImagePiece);
    return true
  }
  else
  {
    return false
  }
}

function getRandomImage() {
    if (availablePiecesOfImage.length == 0) {
        console.warn("No more images to show");
        return null;
    }

    if (Array.isArray(oracleAI_Array_NumbersForImagePieces))
    {
        const randomIndex = oracleAI_Array_NumbersForImagePieces.shift();
        return availablePiecesOfImage[randomIndex];
    } else {
        console.error('AI_Array_NumbersForImagePieces is not an array: ', oracleAI_Array_NumbersForImagePieces);
        return null;
    }   
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

window.onload = function() {
  function waitForConnection() {
      if (!isConnected) {
          setTimeout(waitForConnection, 100);
      }
  }
  waitForConnection();

  if(!userIsAGuesser) {
    InitUserAsOracle();
    return;
  }

  if (numberOfTilesRevealed == 0)
  {
      // initial load 
      ExecuteOracleRevealTile();
  }
  else
  {
    // Loads the correct amount of tiles
    for(var i = 0; i < numberOfTilesRevealed; i++)
    {
        AddNextImageTile()
    }
    renderImages();
  }
}