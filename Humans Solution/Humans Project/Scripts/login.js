var stockphotosIndx = 0;
var stockphotos = ["stockphoto1.jpg", "stockphoto2.jpg", "stockphoto3.jpg", "stockphoto4.jpg"];

$(document).ready(function () {

    sp = $('#stockphoto');
    spBox = $('#spBox');

    //fade stock photos
    setInterval(function () {
        stockphotosIndx++;
        if (stockphotosIndx > stockphotos.length - 1) { stockphotosIndx = 0; }
        sp.fadeOut(function () {
            $(this).load(function () { $(this).fadeIn('slow'); });
            $(this).attr('src', '../images/' + stockphotos[stockphotosIndx]);
        });

    }, 6000);

    //Preload stock photos
    for (var i = 0; i < stockphotos.length; i++) {
        $("<img />").attr("src", "../images/" + stockphotos[i]);
    }

    //login btn
    $('#spanLogin').click(function () {
        $('#frmLogin').submit();
    });

    //signup btn
    $('#spanCreateAcct').click(function () {
        $('#frmSignup').submit();
    });

    //Resize front page image
    resizeBox();
});

function resizeBox() {
    var img = $('#spBox img');
    img.css('width', '100%').load(function () {
        if (img.height() < 800) {
            img.css('height', 800);
            img.css('width', 'auto');
        }

        img.css('left', -1*((img.width() - $(window).width()) / 2) + 'px');
    });
}