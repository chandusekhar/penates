// Plugin definition.

function autocompletarMultipleSplit(val) {
    return val.split(/,\s*/);
}
function autocompletarMultipleExtractLast(term) {
    return split(term).pop();
}

$.fn.autocompletarMultiple = function (options) {

    // Extend our default options with those provided.
    // Note that the first argument to extend is an empty
    // object – this is to keep from overriding our "defaults" object.
    var opts = $.extend({}, $.fn.autocompletarMultiple.defaults, options);
    
    this.each(function () {
        $(this).bind("keydown", function (event) {
            if (event.keyCode === $.ui.keyCode.TAB &&
                $(this).autocomplete("instance").menu.active) {
                event.preventDefault();
            }
        });

        $(this).select2({
            placeholder: opts.placeholder,
            minimumInputLength: 1,
            multiple: true,
            ajax: {
                url: opts.url,
                data: function (term, page) {
                    var parametros = opts.params;
                    var funcParams = opts.paramsFunc();
                    if (funcParams != undefined && funcParams != null) {
                        for (var i in funcParams) {
                            nam = i;
                            parametros[i] = funcParams[i];
                        }
                    }
                    parametros["term"] = term;
                    console.log(parametros);
                    return parametros;
                },
                dataType: 'json',
                delay: 500,
                beforeSend: function () {
                    return opts.beforeSend();
                },
                results: function (data, page) {
                    return { results: opts.bind(data, page) };
                },
                error: function (xhr, textStatus, error) {
                    var er = xhr.status;
                    er = er + " - " + error;
                    er = er + ": \"" + xhr.responseText + "\"";
                    alert(er);
                }
            },
            formatResult: function (items) {
                return opts.formatResult(items);
            },
            formatSelection: function (item) {
                return opts.formatSelection(item);
            },
            initSelection: function (element, callback) {
                return opts.initSelection(element, callback);
            },
            dropdownCssClass: "bigdrop", // apply css that makes the dropdown taller
            escapeMarkup: function (m) { return m; },
            focus: function (e, ui) {
                return opts.focus(e, ui);
            },
            select: function (e, ui) {
                return opts.seleccion(e, ui);
            },
            change: function (e, ui) {
                return opts.change(e, ui);
            },
            minLength: 1,
            appendTo: opts.append
        });
    });
    this.bind("keyup.myPlugin", function (e) {
        opts.keyup(e);
    });
};

// Plugin defaults – added as a property on our plugin function.
$.fn.autocompletarMultiple.defaults = {
    placeholder: "Search...",
    url: "",
    params: {},
    paramsFunc: function () {
        return null;
    },
    idField: $(this).attr("id"),
    seleccion: function(event, ui){
        alert(ui.item.label);
    },
    bind: function (data, page) {
        var a = $.map(data, function (item) {
            return {
                id: item.ID,
                label: item.Label,
                desc: item.Description
            }
        });
        return a;
    },
    append: null,
    change: function (event, ui) { },
    keyup: function (event, ui) { },
    focus: function (event, ui) {
        // prevent value inserted on focus
        return false;
    },
    formatResult: function (items) {
        var markup = "<table class='movie-result'><tr>";
        markup += "<td class='item-info'><div class='item-title'>" + items.label + "</div>";
        if (items.desc != undefined && items.desc != null && items.desc != "") {
            markup += "<div class='movie-synopsis'>" + items.desc + "</div>";
        }
        markup += "</td></tr></table>";
        return markup;
    },
    formatSelection: function (item) {
        return item.label;
    },
    initSelection: function (element, callback) {
        callback(element);
    }
};