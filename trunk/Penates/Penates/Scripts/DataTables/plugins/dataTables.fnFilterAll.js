﻿$.fn.dataTableExt.oApi.fnFilterAll = function (oSettings, sInput, iColumn, bRegex, bSmart) {
    var settings = $.fn.dataTableSettings;

    for (var i = 0 ; i < settings.length ; i++) {
        settings[i].oInstance.fnFilter(sInput, iColumn, bRegex, bSmart);
    }
};