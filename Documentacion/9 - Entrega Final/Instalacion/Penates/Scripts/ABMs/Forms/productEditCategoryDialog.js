//Este js maneja el popup para editar una categoria existente

$(function () {

    $('#category-form-edit').dialog({
        width: 600,
        autoOpen: false,
        modal: true,
        height: 500,
        draggable: true,
        resizable: true,
        close: function (event, ui) {

        },
        open: function (event) {
            event.preventDefault();
            var id = $('#Category').val();
            $.ajax({
                url: "/CategoryForm/EditCategoryForm/" + id,
                type: 'GET'
            })
            .done(function (result) {
                $("#category-form-edit").empty();
                $("#category-form-edit").html(result);
            })
            .fail(function (request, status, error) {
                alert("Error Modal " + request + status + error);
            })
        }
    });

    $("#ManageCategory").on("click", function (event) {
        $("#category-form-edit").dialog("open");
    });
});