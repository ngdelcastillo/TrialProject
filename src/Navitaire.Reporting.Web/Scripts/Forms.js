$(document).ready(function () {
    // forms

    // -- add classes to form fields
    $('html body input[type="text"], html body input[type="password"]').addClass("text");
    $('input[type="submit"]').addClass("submit");
    // -- wrap error fields with error div
    $("form .fieldWithErrors").closest("div.field").addClass("error")
    // -- add active class to active elements
    $("form select, form .text, form textarea").focus(function () {
        $(this).closest("div.field").addClass("active");
        $(this).closest("fieldset").addClass("active");
    });
    // -- remove active class from inactive elements
    $("form select, form .text, form textarea").blur(function () {
        $(this).closest("div.field").removeClass("active");
        $(this).closest("fieldset").removeClass("active");
    });
    // -- make error notice the same width as error field
    $("form .fieldWithErrors input, form .fieldWithErrors textarea").each(function (i, field) {
        width = $(field).width();
        $(field).closest('div.field').find('.formError').width(width);
    });
    // make main #flash notice same width as next panel or content
    // width = $("#flash").next().children("div").width();
    // $("#flash").width(width);
});