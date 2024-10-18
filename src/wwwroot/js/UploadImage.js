const EL = (Id) => document.getElementById(Id);

function readAsDataURL(input) {
    EL("imagePreview").style.display = "block";
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            EL("imagePreview").setAttribute("src", e.target.result);
        }
        reader.readAsDataURL(input.files[0])
    }
}

function showSliceImageModal(image) {
    var fileName = image.split("/").pop()

    EL("selectedImage").value = image;
    EL("routeSliceImageManually").setAttribute("href", `/SliceImage/${fileName}`);
    EL("sliceImageModal_Title").innerText = `Select tile size for ${fileName.split(".")[0]}`;
    EL("imageInModal").setAttribute("src", image);
    EL("sliceImageModal").classList.add("show");

    var sliceImageModal = EL("sliceImageModal")
    if (sliceImageModal) {
        (new bootstrap.Modal(sliceImageModal)).show();
    }
}