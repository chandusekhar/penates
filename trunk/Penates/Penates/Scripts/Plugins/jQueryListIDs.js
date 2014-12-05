
function getListIndex(s) {
    var x = s.lastIndexOf("_");
    var y = s.lastIndexOf("_", (x-1));
    return s.substring(y+1,x);
}