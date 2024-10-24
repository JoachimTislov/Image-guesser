const EL = (Id) => document.getElementById(Id);

const canvas = EL('imageCanvas');
const ctx = canvas.getContext('2d');
let slicing = false;
const points = [];

const img = new Image();
img.style.objectFit = 'cover';
img.src = imagePath;

const height = 600;
const width = 600;

img.onload = () => {
    canvas.width = width;
    canvas.height = height;
    ctx.drawImage(img, 0, 0, width, height);
}

function AddPoint(x, y)
{
    console.log(x, y);
    console.log(typeof x, typeof y);
    console.log(points);
    points[x] = y;
}

canvas.addEventListener('mouseenter', (e) => {
    if (slicing) {
        ctx.beginPath();

        ctx.lineTo(e.offsetX, e.offsetY);
        ctx.stroke();
    }
});

canvas.addEventListener('mouseleave', (e) => {
    if (slicing) {
        stopSlicing(e)
    }
});

canvas.addEventListener('mousedown', (e) => {
    slicing = true;
    ctx.beginPath();
    AddPoint(e.offsetX, e.offsetY);
});

canvas.addEventListener('mousemove', (e) => {
    if (!slicing) {
        return;
    }
    ctx.lineWidth = 2;

    ctx.strokeStyle = 'white';

    ctx.lineTo(e.offsetX, e.offsetY);
    ctx.stroke();

    AddPoint(e.offsetX, e.offsetY);
});

canvas.addEventListener('mouseup', (e) => {
    stopSlicing(e);
});

function stopSlicing(e) {
    slicing = false;
    AddPoint(e.offsetX, e.offsetY);
    ctx.closePath();

    const pointsInput = EL("points");
    pointsInput.value = JSON.stringify(points);
}