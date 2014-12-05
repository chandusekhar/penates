// JavaScript Document

function calculateSizeByID() {
    return calculateSizeByParamID("Size", "Width", "Depth", "Height");
}

function calculateSizeByParamID(sizeID, xID, yID, zID) {
    var x = document.getElementById(xID).value;
    var y = document.getElementById(yID).value;
    var z = document.getElementById(zID).value;
    document.getElementById(sizeID).value = calculateSize(x, y, z);
}

function calculateSize(x, y, z) {
    try{
        var error = false;
        x = parseFloat(x);
        y = parseFloat(y);
        z = parseFloat(z);
        if (x < 0 || y < 0 || z < 0) {
            error = true;
        }
        if (error) {
            return "Error";
        }
        var xytotal = x * y * z;
        xytotal = Math.round(xytotal * 100) / 100
        return xytotal;
    } catch (err) {
        console.log(err);
        return "Error";
    }
}