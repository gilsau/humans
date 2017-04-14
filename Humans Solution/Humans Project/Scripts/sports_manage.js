$(document).ready(function () {

    //Save location
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(SavePosition);
    }

    //Toggle sports selected
    $('.sportslist span').click(function () {

        var span = $(this);

        if (span.hasClass('sportslistselected')) {
            span.removeClass('sportslistselected').css('color', '');
        }
        else {
            span.addClass('sportslistselected').css('color', '#fff');
        }
    });

    //Save changes
    $('.done').click(function () {

        var loading = $('.done img');
        var oksign = $('.done span.ok');
        var errsign = $('.done span.err');

        //collect all sports
        var ids = '';
        $('.sportslist span.sportslistselected').each(function () {
            ids += ',' + $(this).attr('data-id');
        });
        ids = ids.substr(1);

        //Show dialog for at least 2 seconds
        loading.show();
        oksign.hide();
        errsign.hide();
        
        //Submit
        $.ajax({
            type: "POST",
            url: "/sports/add",
            data: { sportIds: ids },
            dataType: 'json'
        })
        .fail(function (response) {
            loading.hide();
            errsign.show();
            errsign.text(response);
        })
        .success(function (response) {
            loading.hide();
            oksign.show();

            $('.leftmenulist').html(response);
        });
    });
});

function SavePosition(position) {
    var lat = position.coords.latitude;
    var lng = position.coords.longitude;

    //Get results
    $.ajax({
        type: "POST",
        url: "/account/savelocation",
        data: { lat: lat, lng: lng },
        dataType: 'json'
    })
    .fail(function (response) {
        console.log('Error');
        console.log(response);
    })
    .success(function (response) {
        //console.log('Success');
        //console.log(response);
    });
}
