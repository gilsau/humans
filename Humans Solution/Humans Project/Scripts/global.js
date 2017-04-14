var widthLimit = 760;
var notify_timer;

$(document).ready(function () {
    $('#btnLogoff').click(function () {
        $('#frmLogoff').submit();
    });

    $('.rememberhuman').click(function () {
        RememberHuman($(this));
    });

    $('.remembervenue').click(function () {
        RememberVenue($(this));
    });

    //enter key to submit form
    $('.enterkey').keypress(function (e) {
        var code = e.keyCode || e.which;
        if (code == 13) {
            $($(this).attr('data-form')).submit();
        }
    });

    //Slide in left menu
    $('.glyphicon-modal-window').click(function () {
        $('#leftPanel').toggle('slide');
    });

    //Check for NEW messages in header
    /*notify_msg_timer = setInterval(function () {
        GetNotifications();
    }, 10000);*/
    GetNotifications();
});

function RememberHuman(link) {
    var imgHtml = '<img src="../images/loading2.gif" style="height:20px;" />';
    var location = link.attr('data-location');

    if (location == 'find') {
        link.attr('class', '').attr('style', '');
    }
    link.html(imgHtml);

    //Wait at least 2 seconds
    setTimeout(function () {

        //Get results
        $.ajax({
            type: "POST",
            url: "/humans/remember",
            data: { userid: link.attr('data-id') },
            dataType: 'json'
        })
        .fail(function (response) {
            link.html(response.responseText);

            console.log('Error');
            console.log(response);
        })
        .success(function (response) {
            if (location == 'find') {
                GetFindResults();
            }
            else if (location == 'viewprofile') {
                link.text('remembered');
            }

            //console.log('Success');
            //console.log(response);
        });
    }, 500);
}

function RememberVenue(link) {
    var imgHtml = '<img src="../images/loading2.gif" style="height:20px;" />';
    var location = link.attr('data-location');

    if (location == 'find') {
        link.attr('class', '').attr('style', '');
    }
    link.html(imgHtml);

    //Wait at least 2 seconds
    setTimeout(function () {

        //Get results
        $.ajax({
            type: "POST",
            url: "/venues/remember",
            data: { venueid: link.attr('data-id') },
            dataType: 'json'
        })
        .fail(function (response) {
            link.html(response.responseText);

            console.log('Error');
            console.log(response);
        })
        .success(function (response) {
            if (location == 'find') {
                GetFindResults();
            }
            else if (location == 'viewvenue') {
                link.text('remembered');
            }

            //console.log('Success');
            //console.log(response);
        });
    }, 500);
}

function GetNotifications() {
    $.ajax({
        type: "GET",
        url: "/messages/getnotifications",
        dataType: 'json'
    })
    .fail(function (response) {
        console.log('Error');
        console.log(response);
    })
    .success(function (response) {
        console.log('Msg Notifications');
        console.log(response);

        if (response.Success) {
            var venueCount = response.DynObject.resultV.DynObject.Data;
            var humanCount = response.DynObject.resultH.DynObject.Data;
            var convoCount = response.DynObject.resultC.DynObject.Data;
            var cHtml = '<span style="position:relative;top:-10px;font-size:16px;padding:2px;">{0}</span>';
            
            $('#mapicon').html(cHtml.replace('{0}', venueCount));

            if ($(window).width() <= widthLimit) {
                $('#humanicon2').html(cHtml.replace('{0}', humanCount));
            }
            else {
                $('#humanicon1').html(cHtml.replace('{0}', humanCount));
            }

            $('#msgicon').html(cHtml.replace('{0}', convoCount));
        }
    });
}