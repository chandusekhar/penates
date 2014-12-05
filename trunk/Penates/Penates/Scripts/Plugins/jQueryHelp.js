$.fn.redraw = function () {
    this.each(function () {
        var sty = this.style.display;
        this.style.display = "none";
        var redraw = this.offsetHeight;
        this.style.display = sty;
    });
};

$.fn.forceRedraw = function() {

    this.each(function () {
        var n = document.createTextNode(' ');
        var disp = this.style.display;  // don't worry about previous display style

        this.appendChild(n);
        this.style.display = 'none';

        setTimeout(function () {
            this.style.display = disp;
            n.parentNode.removeChild(n);
        }, 20); // you can play with this timeout to make it as short as possible
    });
}