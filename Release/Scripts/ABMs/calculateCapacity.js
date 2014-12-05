// JavaScript Document

function calculateCapacity(error) {
    return calculateCapacityByID("Size", "Capacity", "UsedSpace", error);
}

function calculateCapacityByID(sizeID, capacityID, usedSpaceID, error) {
    var newCapacity = document.getElementById(sizeID).value;
    var usedspace = document.getElementById(usedSpaceID).value;
    newCapacity = Math.round(newCapacity * 100) / 100
    document.getElementById(capacityID).value = newCapacity;
    freespace = (newCapacity * 1) - usedspace; //lo multiplico por 1 para que no lo tome como String
    if (error != null && error != "" && freespace < 0) {
        alert(error);
    }
}