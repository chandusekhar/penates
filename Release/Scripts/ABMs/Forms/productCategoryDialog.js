//Este js maneja el popup para egregar una nueva categoria

$(function () {

    $('#dialog-form').dialog({
        width: 600,
        autoOpen: false,
        modal: true,
        height: 500,
        draggable: true,
        resizable: true,
        close: function (event, ui) {
            // $(this).dialog('close');
//            allFields.val("").removeClass("ui-state-error");
 //           $(event.target).dialog('destroy');
        },
        open: function (event) {
            event.preventDefault();
            $.ajax({
                url: "/CategoryForm/CategoryForm",
                type: 'GET'
            })
            .done(function (result) {
                $("#dialog-form").empty();
                $("#dialog-form").html(result);
            })
            .fail(function (request, status, error) {
                alert("Error Modal " + request + status + error);
            })
        }
    });

    $("#AddCategory").on("click", function (event) {
        //$("#dialog-form").dialog("destroy");

        $("#dialog-form").dialog("open");
    });
});