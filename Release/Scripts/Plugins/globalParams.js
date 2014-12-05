function ParametrosGlobales () {
    this.array = new Array();
}

ParametrosGlobales.prototype.indexOfParam = function (key) {
    for (var i = 0; i < this.array.length; i++) {
        if (this.array[i].name == key) return i;
    }
    return -1;
};

ParametrosGlobales.prototype.getParam = function (key) {
    for (var i = 0; i < this.array.length; i++) {
        if (this.array[i].name == key) return array[i].value;
    }
    return null;
};

ParametrosGlobales.prototype.addParam = function (name, value) {
    try {
        var ind = this.indexOfParam(name);
        if (ind >= 0) {
            this.array[ind].value = value;
        } else {
            this.array.push({ name: name, value: value });
        }
        return true;
    } catch (err) {
        console.log(err);
        return false;
    }
};

ParametrosGlobales.prototype.deleteParam = function (name) {
    try {
        var ind = this.indexOfParam(name);
        if (ind >= 0) {
            this.array.splice(ind, 1);
        }
        return true;
    } catch (err) {
        console.log(err);
        return false;
    }
};