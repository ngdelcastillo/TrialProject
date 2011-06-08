$(document).ready(function () {
    $("li[class='Report']").css('display', 'none');
    $("li[class='LinkedReport']").css('display', 'none');
    $("li[class='DataSource']").css('display', 'none');

    $("li > ul > li[class='Folder']").css('display', 'none');
    $("li > ul > li > ul > li[class='Folder']").css('display', 'none');

    $('<div class="Folder-Contracted"></div>').insertBefore("li[class='Folder']");

    $("li > ul > div[class='Folder-Contracted']").css('display', 'none');
    $("li > ul > li > ul > div[class='Folder']").css('display', 'none');



    $("li[class='Folder']").click(function (event) {
        event.stopPropagation();
        var eventSrc = event.srcElement;
        var link = eventSrc.getElementsByTagName('a')[0];
        window.location = link;
    });

    $("div[class|='Folder']").click(function (event) {
        event.stopPropagation();
        var eventSrc = event.srcElement;
        var next = $(eventSrc).next().get(0);
        var directChildren = next.getElementsByTagName('ul');
        if (directChildren.length == 0) {
            $(eventSrc).removeClass('Folder-Contracted').addClass('Folder-Empty');
            return;
        }
        var directNodes = directChildren[0].childNodes;


        var filteredLI = $(directNodes).filter('li');
        var filteredDIV = $(directNodes).filter('div');
        var display = 'none';
        var itemCheck = filteredLI[0];
        if ($(itemCheck).css('display') == 'none') {
            display = 'list-item';
            $(eventSrc).removeClass('Folder-Contracted').addClass('Folder-Expanded');
        }
        else {
            $(eventSrc).removeClass('Folder-Expanded').addClass('Folder-Contracted');
        }

        for (i = 0; i < filteredLI.length; i++) {
            var item = filteredLI[i];
            $(item).css('display', display);
        }

        for (i = 0; i < filteredDIV.length; i++) {
            var item = filteredDIV[i];
            $(item).css('display', display);
        }


    });

});



/*$("li[class='Folder']").click(function (event) {
event.stopPropagation();
var directChildren = window.event.srcElement.getElementsByTagName('ul');
var directNodes = directChildren[0].childNodes;
var filteredLI = $(directNodes).filter('li');
var filteredDIV = $(directNodes).filter('div');
var display = 'none';
var itemCheck = filteredLI[0];
if ($(itemCheck).css('display') == 'none') {
display = 'list-item';
}

for (i = 0; i < filteredLI.length; i++) {
var item = filteredLI[i];
$(item).css('display', display);
}

for (i = 0; i < filteredDIV.length; i++) {
var item = filteredDIV[i];
$(item).css('display', display);
}


});*/