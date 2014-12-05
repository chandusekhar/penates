// Plugin definition.
$.fn.enviar = function (options) {

    // Extend our default options with those provided.
    // Note that the first argument to extend is an empty
    // object – this is to keep from overriding our "defaults" object.
    var opts = $.extend({}, $.fn.enviar.defaults, options);
    
    if (opts.params == null) {
        var paramID = $(this).attr('id');
        opts.params = $("#" + paramID + " :input").serialize();
    }

    this.each(function () {
       $.ajax({
        type: "POST",
        url: opts.url,
        data: opts.params,
        beforeSend : function(){
            return opts.beforeSend();
        },
        success: function (response) {
            return opts.success(response);
        },
        error: function (obj, textStatus, errorThrown) {
            return opts.error(obj, textStatus, errorThrown);
         }
       });
    });
};

// Plugin defaults – added as a property on our plugin function.
$.fn.enviar.defaults = {
    url: "",
    params: null,
    success: function (response) {
        var message = response.title;
        message = message + ": " + response.message;
        alert(message);
    },
    error: function (obj, textStatus, errorThrown) {
        var err = obj.status;
        err = err + ": " + obj.statusText;
        err = err + ". " + obj.responseText;
        alert(err);
    },
    beforeSend: function () {}
};