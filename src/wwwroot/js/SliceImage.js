const EL = (Id) => document.getElementById(Id);

const canvas = EL('imageCanvas');
const ctx = canvas.getContext('2d');
let slicing = false;
const lines = [];

const img = new Image();
img.style.objectFit = 'cover';
img.src = '@Model.ImagePath';

const height = 600;
const width = 600;

img.onload = () => {
    canvas.width = width;
    canvas.height = height;
    ctx.drawImage(img, 0, 0, width, height);
}

canvas.addEventListener('mouseenter', () => {
    if (slicing) {
        ctx.beginPath();

        ctx.lineTo(e.offsetX, e.offsetY);
        ctx.stroke();
    }
});

canvas.addEventListener('mouseleave', () => {
    if (slicing) {
        ctx.closePath();
        slicing = false;
    }
});

canvas.addEventListener('mousedown', (e) => {
    slicing = true;
    ctx.beginPath();
    lines.push({ x: e.offsetX, y: e.offsetY });
});

canvas.addEventListener('mousemove', (e) => {
    if (!slicing) {
        return;
    }
    ctx.lineWidth = 2;

    ctx.strokeStyle = 'white';

    ctx.lineTo(e.offsetX, e.offsetY);
    ctx.stroke();

    /*for (let point of lines) {
        if (point.x == e.offsetX && point.y == e.offsetY) {
            stopSlicing(e);
            return;
        }
    }*/

    lines.push({ x: e.offsetX, y: e.offsetY });
});

canvas.addEventListener('mouseup', (e) => {
    stopSlicing(e);
});

function stopSlicing(e) {
    slicing = false;
    lines.push({ x: e.offsetX, y: e.offsetY });
    ctx.closePath();
}