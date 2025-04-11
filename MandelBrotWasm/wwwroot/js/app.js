
function renderImgData(imgData) {
    /*const canvas = document.getElementById("canvas");
    const ctx = canvas.getContext("2d");
    const imageData = new ImageData(new Uint8ClampedArray(imgData), canvas.width, canvas.height);
    ctx.putImageData(imageData, 0, 0);*/
    let renderImg = document.getElementById("renderImg");
    if (renderImg == null) {
        renderImg = document.createElement("img");
        renderImg.id = "renderImg";
        renderImg.src = imgData;
        document.body.appendChild(renderImg);
    } else {
        renderImg.src = imgData;
    }

    renderImg.style.display = "block";
}

function hideImg() {
    const renderImg = document.getElementById("renderImg");
    if (renderImg != null) {
        renderImg.style.display = "none";
    }
}

function getBodySize() {
    return {
        Width: window.innerWidth,
        Height: window.innerHeight
    };
}