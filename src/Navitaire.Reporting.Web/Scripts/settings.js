function ExpandReportPath() {
    if ($("#ExportTree").css("display") == "none") {
        $("#ExportTree").css("display", "block");
    }
    else {
        $("#ExportTree").css("display", "none");
    }
}

$(document).ready(function () {

    function hideDescription(id) {
        $("#" + id.toString()).css("display", "none");
    }

    //    $("div[id|='info']").mouseenter(function (event) {
    //        event.stopPropagation();
    //        var id = event.srcElement.id.replace("info", "desc");
    //        $("#" + id.toString()).css("display", "block");
    //    });

    //    $("div[id|='info']").mouseleave(function (event) {
    //        event.stopPropagation();
    //        var id = event.srcElement.id.replace("info", "desc");
    //        $("#" + id.toString()).css("display", "none");

    //    });

    $("div[id|='info']").click(function (event) {
        event.stopPropagation();
        var id = event.srcElement.id.replace("info", "desc");
        $("#" + id.toString()).css("display", "block");
        setTimeout(function () { hideDescription(id) }, 2000);
    });

    $("#role-right").click(function (event) {
        event.stopPropagation();
        var element = event.srcElement;
        var selectedIndices = new Array();
        while (deniedRoles.selectedIndex != -1) {
            selectedIndices.push(deniedRoles.getElementsByTagName("option")[deniedRoles.selectedIndex]);
            deniedRoles.getElementsByTagName("option")[deniedRoles.selectedIndex].selected = false;
        }
        for (i = 0; i < selectedIndices.length; i++) {
            $('#allowedRoles').append(selectedIndices[i]);
        }
    });

    $("#role-left").click(function (event) {
        event.stopPropagation();
        var element = event.srcElement;
        var selectedIndices = new Array();
        while (allowedRoles.selectedIndex != -1) {
            selectedIndices.push(allowedRoles.getElementsByTagName("option")[allowedRoles.selectedIndex]);
            allowedRoles.getElementsByTagName("option")[allowedRoles.selectedIndex].selected = false;
        }
        for (i = 0; i < selectedIndices.length; i++) {
            $('#deniedRoles').append(selectedIndices[i]);
        }
    });
});

