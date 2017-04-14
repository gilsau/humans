var map;
var markersInfo = [];
var markersOnMap = [];

$(document).ready(function () {

    //Gender selected
    $('[name="gender"]').click(function () {
        GetFindResults();
    });

    //Distance slider
    $('#sliderDist').slider({
        value: 100,
        min: 1,
        max: 500,
        stop: function(event, ui){
            GetFindResults();
        },
        slide: function (event, ui) {
            $("#distanceDisplay").text('<' + ui.value + ' mi.');
        }
    });
    $("#distanceDisplay").text('< 100 mi.');
    
    //Age slider
    $("#sliderAge").slider({
        range: true,
        min: 10,
        max: 60,
        values: [18, 40],
        stop: function(event, ui){
            GetFindResults();
        },
        slide: function (event, ui) {
            $("#ageDisplay").text(ui.values[0] + " - " + ui.values[1]);
        }
    });
    $("#ageDisplay").text($("#sliderAge").slider("values", 0) + " - " + $("#sliderAge").slider("values", 1));
    
    //Skill slider
    $("#sliderSkill").slider({
        value: 1,
        min: 1,
        max: 4,
        step: 1,
        stop: function(event, ui){
            GetFindResults();
        },
        slide: function (event, ui) {
            var txt = '';
            switch (ui.value) {
                case 1: txt = 'fresh'; break;
                case 2: txt = 'intermediate'; break;
                case 3: txt = 'advanced'; break;
                case 4: txt = 'expert'; break;
            }

            $("#skillDisplay").text(txt);
        }
    });
    $("#skillDisplay").text('fresh');

    //Switch modes
    $('.headoption').click(function () {
        var link = $(this);
        link.parent().find('a').attr('class', '').addClass('headoption btn greytext');
        link.removeClass('greytext').addClass('orangetext');

        var target = $(this).attr('data-target');
        $('.mode').hide();
        $('#' + target).show();

        if (target == 'youBox') {
            $('#filterLinks').hide();
        }

        //Get initial results
        if (target == 'rememberBox') {
            var remResults = $('#remResults');
            GetRemResults();
        }

        //Get initial results
        if (target == 'findBox') {
            var fResults = $('#findResults');
            GetFindResults();
        }
    });

    //Toggle findable link
    $('#spanFindable').click(function () {
        var txt = $.trim($(this).text()) == 'YEP.' ? 'NO.' : 'YEP.';
        var val = $.trim($(this).text()) == 'YEP.' ? false : true;

        $(this).text(txt);
        $(this).attr('data-value', val);
    });

    //Toggle Skill level
    $('.skilllisted, .skillselected').on('click', function () {
        $('.skillselected').attr('class', '').addClass('btn skilllisted');
        $(this).attr('class', '').addClass('btn skillselected');
    });

    //Filter click handler
    $('#spanFilter').click(function () {

        if ($('#filterBox').is(':visible')) {
            $('#filterBox').hide('slide', { direction: 'right' }, 500);
        }
        else {
            $('#filterBox').show('slide', { direction: 'right' }, 500);
        }
    });

    //Toggle map/list
    $('#showmap, #showlist').click(function(){
        var id = $(this).attr('id');
        var inFind = $('#findBox').is(':visible');

        console.log('inFind: ' + inFind);
        
        if (id == 'showmap') {
            if (inFind) {
                $('#findMapResults').show();
                $('#findResults').hide();
            }
            else {
                $('#remMapResults').show();
                $('#remResults').hide();
            }
        }
        else if (id == 'showlist') {
            if (inFind) {
                $('#findMapResults').hide();
                $('#findResults').show();
            }
            else {
                $('#remMapResults').hide();
                $('#remResults').show();
            }
        }
    });

    //Save changes
    $('.save').click(function () {

        var loading = $('.save img');
        var oksign = $('.save span.ok');
        var errsign = $('.save span.err');

        //collect all values
        var sportid = $('.save').attr('data-sportid');
        var findable = $('#spanFindable').attr('data-value');
        var note = $('#txtNote').val();
        var skilllevel = $('.skillselected').attr('data-skilllevel');

        //Show dialog for at least 2 seconds
        loading.show();
        oksign.hide();
        errsign.hide();
        setTimeout(function () {

            //Submit
            $.ajax({
                type: "POST",
                url: "/sports/you",
                data: { sportid: sportid, findable: findable, note: note, skilllevel: skilllevel},
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
            });

        }, 2000);

    });

    //page load
    GetFindResults();
});

function UnRemember(link) {
    var imgHtml = '<img src="../images/loading2.gif" style="height:20px;" />';

    //Show loading sign
    link.attr('class', '').attr('style', '');
    link.html(imgHtml);
    
    //Wait at least 2 seconds
    setTimeout(function () {

        //Get results
        $.ajax({
            type: "POST",
            url: "/humans/unremember",
            data: { userid: link.attr('data-id') },
            dataType: 'json'
        })
        .fail(function (response) {
            link.html(response.responseText);

            console.log('Error');
            console.log(response);
        })
        .success(function (response) {
            GetRemResults();

            //console.log('Success');
            //console.log(response);
        });
    }, 1000);
}

function GetFindResults() {
    var filterBox = $('#filterBox');
    var fmResults = $('#findMapResults');
    var fResults = $('#findResults');
    var dist = $("#sliderDist").slider("value");
    var agemin = $("#sliderAge").slider("values", 0);
    var agemax = $("#sliderAge").slider("values", 1);
    var gender = $('[name="gender"]:checked').val();
    var skill = $("#sliderSkill").slider("value");
    var sportid = $('.save').attr('data-sportid');
    var winWd = $(window).width();

    if ($('#filterBox').is(':visible') && winWd <= widthLimit) {
        $('#filterBox').hide('slide', { direction: 'right' }, 500);
    }

    fmResults.hide();
    fResults.html('<img class="center-block" src="../images/loading_animated.gif" style="width:50%;" />');

    //Get results
    $.ajax({
        type: "POST",
        url: "/humans/find",
        data: { sportid: sportid, distance: dist, agemin: agemin, agemax: agemax, gender: gender, skill: skill },
        dataType: 'json'
    })
    .fail(function (response) {
        fResults.html(response);
        console.log('Error');
        console.log(response);
    })
    .success(function (response) {
        if (response.Peeps.length > 0) {
            markersInfo = [];
            markersOnMap = [];

            if (winWd <= widthLimit) { fResults.hide(); }
            ShowList('findResults', 'findMapResults', response.Peeps, 1);
            ShowMap('findMapResults', 1, response.CurrPeep);
            if (winWd <= widthLimit) { $('#filterLinks').show(); }
        }
        else {
            $('#findResults').html('<div class="text-center greytext"><br/><br/><h3>No humans were found near you.</h3></div>');
        }
    });
}

function GetRemResults() {
    var remmResults = $('#remMapResults');
    var remResults = $('#remResults');
    var sportid = $('.save').attr('data-sportid');
    
    remmResults.hide();
    remResults.html('<img class="center-block" src="../images/loading_animated.gif" style="width:50%;" />');

    //Get results
    $.ajax({
        type: "POST",
        url: "/humans/getremembered",
        data: { sportid: sportid },
        dataType: 'json'
    })
    .fail(function (response) {
        remResults.html(response);
        console.log('Error');
        console.log(response);
    })
    .success(function (response) {
        if (response.Peeps.length > 0) {
            markersInfo = [];
            markersOnMap = [];

            var winWd = $(window).width();

            if (winWd <= widthLimit) { remResults.hide(); }
            ShowList('remResults', 'remMapResults', response.Peeps, 2);
            ShowMap('remMapResults', 2, response.CurrPeep);
            if (winWd <= widthLimit) { $('#spanFilter').hide(); }
        }
        else {
            $('#remResults').html('<div class="text-center greytext"><br/><br/><h3>No humans have been remembered.</h3></div>');
        }
    });
}

function ShowList(listBoxName, mapBoxName, peeps, searchType) {
    var listBox = $('#' + listBoxName);
    var mapBox = $('#' + mapBoxName);
    var resultsHtml = '<table class="table table-hover">';
    var htmllinklist = '';
    var htmllinkmap = '';

    //Show list results
    var row = 1;
    for (var person in peeps) {
        var peep = peeps[person];
        var title = $.trim(peep.Title) != '' ? '<span class="pull-right orangeback whitetext" style="padding:5px;">' + peep.Title + '</span>' : '';
        var sDistance = Number(peep.Distance) + 1;
        var remlink = searchType == 1 ? '<span title="Remember" data-location="find" style="font-size:20px;" class="rememberhuman btn glyphicon glyphicon-plus-sign actionlink" data-id="' + peep.UserId + '"></span>' : '<span title="Un-Remember" style="font-size:20px;" class="unrememberhuman btn glyphicon glyphicon-minus-sign actionlink" data-id="' + peep.UserId + '"></span>';

        if (Number(peep.Remembered) != 0) {
            remlink = remlink.replace('rememberhuman', 'greytext');
        }

        htmllinklist = '<tr>' +
                            '<th style="text-align:left;width:5%;"><a style="color:#000;" href="../account/viewprofile?id=' + peep.UserSportId + '">' + row + ') <img class="roundImg3" src="../profilepics/' + peep.ProfilePic + '" /></a></th>' +
                            '<td style="text-align:left;width:95%;">' +
                                '<div class="row-fluid">' +
                                    '<div class="col-sm-5">' +
                                        '<div style="font-weight:bold;padding:5px;vertical-align:top;">' + peep.FirstName + ', ' + peep.Age + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;' + sDistance + 'mi. ' + title + '</div>' +
                                        '<div style="word-wrap:break-word;vertical-align:top;">' + peep.Note + '</div>' +
                                    '</div>' +
                                    '<div class="col-sm-7 text-right">' +
                                            '<span data-marker-indx="' + row + '" class="maplink btn glyphicon glyphicon-map-marker actionlink" style="font-size:18px;"></span>' +
                                            '<a href="../account/viewprofile?id=' + peep.UserSportId + '" data-marker-indx="' + (row - 1) + '" class="btn glyphicon glyphicon-eye-open actionlink" style="font-size:18px;"></a>' +
                                            '<span title="Send Message" style="font-size:20px;" class="msg btn glyphicon glyphicon-comment actionlink" data-id="' + peep.UserId + '"></span>' +
                                            remlink +
                                    '</div>' +
                                '</div>' +
                            '<td>' +
                        '</tr>';

        htmllinkmap = '<table>' +
                        '<tr>' +
                            '<td><a style="color:#000;" href="../account/viewprofile?id=' + peep.UserSportId + '"><b>' + row + ') <img class="roundImg3" src="../profilepics/' + peep.ProfilePic + '" /></b></a></td>' +
                            '<td>' +
                                '<div class="row-fluid">' +
                                    '<div class="col-sm-5">' +
                                        '<b>' + peep.FirstName + ', ' + peep.Age + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;' + sDistance + 'mi. ' + title + '</b>' +
                                    '</div>' +
                                    '<div class="col-sm-7">' +
                                        '<a href="../account/viewprofile?id=' + peep.UserSportId + '" data-marker-indx="' + (row - 1) + '" class="btn glyphicon glyphicon-eye-open actionlink" style="font-size:18px;"></a>' +
                                        '<span title="Send Message" style="font-size:20px;" class="msg btn glyphicon glyphicon-comment" data-id="' + peep.UserId + '"></span>' +
                                        remlink +
                                    '</div>' +
                                '</div>' +
                            '</td>' +
                        '</tr>' +
                    '</table>' +
                    '<div style="padding:10px;">' + peep.Note + '</div>';

        resultsHtml += htmllinklist;

        markersInfo.push({
            "title": peep.FirstName,
            "lat": peep.Lat,
            "lng": peep.Lng,
            "description": htmllinkmap
        });

        row++;
    }
    listBox.html(resultsHtml + '</table>');
}

function ShowMap(mapBoxName, searchType, currPeep) {
    var mapBox = document.getElementById(mapBoxName);
    $(mapBox).show();

    markersInfo.unshift({
        "title": currPeep.FirstName,
        "lat": currPeep.Lat,
        "lng": currPeep.Lng,
        "description": ''
    });
    
    var mapOptions = {
        center: new google.maps.LatLng(currPeep.Lat, currPeep.Lng),
        zoom: 8,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    var infoWindow = new google.maps.InfoWindow();
    map = new google.maps.Map(mapBox, mapOptions);

    for (i = 0; i < markersInfo.length; i++) {
        var data = markersInfo[i];
        var myLatlng = new google.maps.LatLng(data.lat, data.lng);
        var marker = new google.maps.Marker({
            position: myLatlng,
            map: map,
            title: data.title,
            icon: (i == 0 ? '../images/icon_pin_black_40_red.png' : '../images/icon_pin_black_40.png')
        });
        
        //Save marker in array, to trigger click event later
        markersOnMap.push(marker);

        (function (marker, data) {

            //No window for current user
            if (i > 0) {

                google.maps.event.addListener(marker, "click", function (e) {

                    infoWindow.setContent(data.description);
                    infoWindow.open(map, marker);
                    map.setCenter(marker.getPosition());

                    //Remember Event Handler
                    if (searchType == 1) {
                        $(".rememberhuman").click(function () {
                            RememberHuman($(this));
                        });
                    }

                        //Unremember Event Handler
                    else if (searchType == 2) {
                        $(".unrememberhuman").click(function () {
                            UnRemember($(this));
                        });
                    }

                    //Send msg
                    $('.msg').click(function () {
                        window.location = "/messages/manage?add=1&id=" + $(this).attr('data-id');
                    });


                });
            }

        })(marker, data);
    }

    //Find on map event handler
    $('.maplink').click(function () {
        google.maps.event.trigger(markersOnMap[$(this).attr('data-marker-indx')], 'click');
        $(window).scrollTop(0);
        $('#showmap').click();
    });

    //Send msg
    $('.msg').click(function () {
        window.location = "/messages/manage?add=1&id=" + $(this).attr('data-id');
    });
}