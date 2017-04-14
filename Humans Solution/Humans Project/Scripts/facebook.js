$(document).ready(function () {
    var aid = $('.lnkFBLogin').attr('data-aid');

    window.fbAsyncInit = function () {
        FB.init({
            appId: '667708286661950',
            cookie: true,
            xfbml: true,
            version: 'v2.2'
        });
    };

    // Load the SDK asynchronously
    (function (d, s, id) {
        var js, fjs = d.getElementsByTagName(s)[0];
        if (d.getElementById(id)) return;
        js = d.createElement(s); js.id = id;
        js.src = "//connect.facebook.net/en_US/sdk.js";
        fjs.parentNode.insertBefore(js, fjs);
    }(document, 'script', 'facebook-jssdk'));

});

function fb_logout() {

    FB.login(function (response1) {

        console.log('response1');
        console.log(response1);
        
        FB.api('/me/permissions', 'DELETE', function (response2) {

            console.log('response2');
            console.log(response2);

            if (response2.success == true) {
                alert('Successfully revoked access');
            } else {
                alert('Error revoking app');
            }
        });

    }, { scope: 'public_profile,email,user_birthday' });
}

function fb_login() {
    var imgHtml = '<img class="center-block" src="../images/loading2.gif" style="width:50px;" />';
    var btn = $('#lnklogin');

    //Show loading sign
    btn.html(imgHtml);

    FB.login(function (response4) {

        FB.getLoginStatus(function (response5) {
            console.log('getLoginStatus');
            console.log(response5);

            statusChangeCallback(response5);
        });

    }, { scope: 'public_profile,email,user_birthday' });
}

function fb_reset() {
    FB.login(function (response2) {
        console.log('login');
        console.log(response2);

        FB.api('/me/permissions', 'DELETE', function (response3) {

            console.log('response3 permissions');
            console.log(response3);

            if (response3.success == true) {
                console.log('Successfully revoked access');
            } else {
                console.log('Error revoking app');
            }
        });

    });
}

function statusChangeCallback(response) {
    if (response.status === 'connected') {
        console.log('authorized and ready to go!');
        LoginToSite();
    }
}

function LoginToSite() {
    FB.api('/me', function (response) {
        console.log(response);

        $('#fb_Id').val(response.id);
        $('#fb_FirstName').val(response.first_name);
        $('#fb_LastName').val(response.last_name);
        $('#fb_Email').val(response.email);
        $('#fb_Gender').val(response.gender);
        $('#fb_Birthdate').val(response.birthday);

        $('#frmFacebook').submit();
    });
}