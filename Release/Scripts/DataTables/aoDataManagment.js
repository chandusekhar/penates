function addGlobalParameters(array, globalParams) {
    try {
        if (typeof globalParams != 'undefined' && globalParams instanceof Array) {
            $.each(globalParams, function (index, value) {
                var ind = findInAoData(array, value);
                if (ind >= 0) {
                    array[index] = value;
                } else {
                    array.push({ name: value.name, value: value.value });
                }
            });
        }
        return array;
    } catch (err) {
        console.log(err);
        return array;
    }
}

function findInAoData(array, param) {
    for (var i = 0; i < array.length; i++) {
        if (array[i].name == param.name) return i;
    }
    return -1;
}