/* Script for Datepicker */
$(function () {
    $(".datepicker").datepicker({
        showAnim: "slide",
        showOn: "button",
        buttonImage: "../Content/Themes/SkySales/images/calendar-view-month-icon.png",
        buttonImageOnly: true        
    });
});

/* Script for Delete Confirmation */
function deleteConfirm() {
    modifyMessage();
    showModalConfirm();
}

function modifyMessage() {
    var msg = $(".deleteButton").attr("title");
    var messagebox = $(".dialog-confirm").find(".confirmMessage");
    var content = messagebox.text();

    msg = "Delete " + msg + "?";
    if (content == "") {
        messagebox.append(msg);
    }
}

function showModalConfirm() {
    $(".dialog-confirm").dialog({
        modal: true,
        resizable: false,
        width: 500,
        buttons: {
            "Delete": function () {
                $(this).dialog("close");
                $(".hiddenDelete").click();
            },
            Cancel: function () {                
                $(this).dialog("close");
            }
        }        
    });
}

/* Script for Add Items */
function resetDisplay() {
    $(".additemDatasource").attr("style", "Display: none;");
    $(".additemsreport").attr("style", "Display: none;");
    $(".additemsfolder").attr("style", "Display: none;");
    $(".additemslinkedreport").attr("style", "Display: none;");
}

function addDataSource() {
    resetDisplay();
    $(".additemDatasource").attr("style", "Display: block;");    
}

function addReport() {
    resetDisplay();
    $(".additemsreport").attr("style", "Display: block;");
}

function addFolder() {
    resetDisplay();
    $(".additemsfolder").attr("style", "Display: block;");
}

function addLinkedReport() {
    resetDisplay();
    $(".additemslinkedreport").attr("style", "Display: block;");
}