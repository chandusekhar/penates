//Este js reemplaza las imagenes del formulario

function GetFileSize(fileid) {
    try {
        var fileSize = 0;
        ////for IE @Deprecated
        //if ($.browser.msie) {
        //    //before making an object of ActiveXObject, 
        //    //please make sure ActiveX is enabled in your IE browser
        //    var objFSO = new ActiveXObject("Scripting.FileSystemObject"); var filePath = $("#" + fileid)[0].value;
        //    var objFile = objFSO.getFile(filePath);
        //    var fileSize = objFile.size; //size in kb
        //    fileSize = fileSize / 1048576; //size in mb 
        //}
        //    //for FF, Safari, Opeara and Others
        //else {
            fileSize = $("#" + fileid)[0].files[0].size //size in kb
            fileSize = fileSize / 1048576; //size in mb 
       // }
        return fileSize;
    }
    catch (e) {
        //FIXME: sacar esta linea al entregar, solo para DEBUG
        alert("Error is :" + e);
    }
}

//get file path from client system
function getNameFromPath(strFilepath) {
    var objRE = new RegExp(/([^\/\\]+)$/);
    var strName = objRE.exec(strFilepath);

    if (strName == null) {
        return null;
    }
    else {
        return strName[0];
    }
}

function checkfile() {
    var file = getNameFromPath($("#Image").val());
    if (file != null) {
        var extension = file.substr((file.lastIndexOf('.') + 1));
        extension = extension.toLowerCase();
        switch (extension) {
            case 'jpg':
            case 'png':
            case 'gif':
            case 'jpeg':
                flag = true;
               break;
            default:
                flag = false;
        }
    }
    if (flag == false) {
        //FIXME: traducir para que use castellano o ingles
        alert("You can upload only jpg,png,gif,jpeg extension file");
        return false;
    }else {
        var size = GetFileSize('Image');
        if (size > 4) {
            //FIXME: traducir para que use castellano o ingles
            alert("You can upload file up to 4 MB");
            return false;
        }
        return true;
    }
}

function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#ProdImg').attr('src', e.target.result);
        }

        reader.readAsDataURL(input.files[0]);
    }
}

$("#Image").change(function () {
    if (checkfile()) {
        readURL(this);
        $('#validImage').prop('checked', true);
    } else {
        $('#ProdImg').attr('src', '/Forms/getProductImg');
        //var input = $('#Image');
        //var clon = input.clone();  // Creamos un clon del elemento original
        //input.replaceWith(clon);   // Y sustituimos el original por el clon
        $('#validImage').prop('checked', false);
    }
});

