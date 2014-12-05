// Plugin definition.
$.fn.autocompletar = function (options) {

    // Extend our default options with those provided.
    // Note that the first argument to extend is an empty
    // object – this is to keep from overriding our "defaults" object.
    var opts = $.extend({}, $.fn.autocompletar.defaults, options);
    
    this.each(function () {
        $(this).autocomplete({
            source: function (request, response) {
                var parametros = {};
                if (opts.params instanceof Array) {
                    for (var p in opts.params) {
                        parametros[opts.params[p].name] = opts.params[p].value;
                    }
                } else {
                    parametros = opts.params;
                }
                parametros["term"] = request.term;
                $.ajax({
                    url: opts.url,
                    data: parametros,
                    dataType: 'json',
                    type: 'GET',
                    delay: 500,
                    success: function (data) {
                        return opts.bind(response, data);
                    },
                    error: function (xhr, textStatus, error) {
                        var er = xhr.status;
                        er = er + " - " + error;
                        er = er + ": \"" + xhr.responseText + "\"";
                        alert(er);
                    }
                })
            },
            select: function (e, ui) {
                return opts.seleccion(e, ui);
            },
            change: function (e, ui) {
                return opts.change(e, ui);
            },
            minLength: 1,
            appendTo: opts.append  
        }).data("ui-autocomplete")._renderItem = function (ul, item) {
            if (item.desc == null || item.desc == "" || item.desc == "null") {
                return $("<li>")
                .append("<a>" + "<p>" + item.label + "</p>\n" + "</a>")
                .appendTo(ul);
            } else {
                return $("<li>")
                    .append("<a>" + "<p>" + item.label + "</p>" + "<p   style=\"font-size: 10px\">" + item.desc + "</p>\n" + "</a>")
                    .appendTo(ul);
            }
        };
    });

    this.bind("keyup.myPlugin", function (e) {
        opts.keyup(e);
    });
};

// Plugin defaults – added as a property on our plugin function.
$.fn.autocompletar.defaults = {
    url: "",
    params: {},
    seleccion: function (event, ui) {
        $(event.target).val(ui.item.label);
        $(event.target).next('input[type=hidden]').val(ui.item.id);
    },
    bind: function (response, data) {
        return response($.map(data, function (item) {
            return {
                id: item.ID,
                label: item.Label,
                desc: item.Description,
                aux: item.aux
            }
        }));
    },
    append: null,
    change: function (event, ui) { },
    keyup: function (event, ui) { },
    
};