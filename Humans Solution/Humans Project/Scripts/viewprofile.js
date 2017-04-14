$(document).ready(function () {
    //Send msg
    $('.msg').click(function () {
        window.location = "/messages/manage?add=1&id=" + $(this).attr('data-id');
    });
});

