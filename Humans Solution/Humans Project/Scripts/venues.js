var markers = [];
var map;
var infowindow;
var currPeep;
var mapResults;
var fResults = $('#findResults');
var fmResults = $('#findMapResults');
var remResults = $('#remResults');
var remmResults = $('#remMapResults');

$(document).ready(function () {

    //Get current user's info
    var currPeepDiv = $('#CurrPeep');
    currPeep = { Lat: currPeepDiv.attr('data-lat'), Lng: currPeepDiv.attr('data-lng'), Sport: currPeepDiv.attr('data-sport'), SportId: currPeepDiv.attr('data-sportid') };

    //Distance slider
    $('#sliderDist').slider({
        value: 30,
        min: 1,
        max: 100,
        stop: function (event, ui) {
            GetFindResults();
        },
        slide: function (event, ui) {
            $("#distanceDisplay").text('<' + ui.value + ' mi.');
        }
    });
    $("#distanceDisplay").text('<30 mi.');

    //Switch modes
    $('.headoption').click(function () {
        var link = $(this);
        link.parent().find('a').attr('class', '').addClass('headoption btn greytext');
        link.removeClass('greytext').addClass('orangetext');

        var target = $(this).attr('data-target');
        $('.mode').hide();
        $('#' + target).show();

        if (target == 'addBox') {
            InitializeAddressSearch();
        }

        //Get initial results
        if (target == 'rememberBox') {
            GetRemResults();
        }
        
        //Get initial results
        if (target == 'findBox') {
            GetFindResults();
        }
    });

    ///////// ADD SPOT ////////////////////////////////////////////////////////////////
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

    //Add Spot
    $('.addspot').click(function () {
        var loading = $('.addspot img');
        var oksign = $('.addspot span.ok');
        var errsign = $('.addspot span.err');

        //get venue info
        var sName = $.trim($('#asName').val());
        var sDesc = $.trim($('#asDescription').val());
        var sAddress = $.trim($('#asAddress').val());
        var sAddress2 = $.trim($('#asAddress2').val());
        var sCity = $.trim($('#asCity').val());
        var sState = $.trim($('#asState').val());
        var sZip = $.trim($('#asZip').val());
        var sLat = $.trim($('#asLat').val());
        var sLng = $.trim($('#asLng').val());
        var sPlaceId = $.trim($('#asPlaceId').val());
        var sFile = document.getElementById("fileLogo").files[0];

        var venue = {
            name: sName,
            address: sAddress,
            address2: sAddress2,
            city: sCity,
            state: sState,
            zip: sZip,
            desc: sDesc,
            lat: sLat,
            lng: sLng,
            placeid: sPlaceId
        };
        
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
        setTimeout(function () {

            //Submit
            $.ajax({
                type: "POST",
                url: "/venues/addone",
                data: JSON.stringify({venue: venue, sportids: ids }),
                dataType: 'json',
                traditional: true,
                contentType: 'application/json;'
            })
            .fail(function (response) {
                loading.hide();
                errsign.show();
                
                console.log('ERROR');
                console.log(response);
            })
            .success(function (response) {

                console.log('response for adding venue');
                console.log(response);

                if (response.Success) {
                    if (sFile) {
                        var venueid = response.DynObject;
                        console.log('updating logo for venueid: ' + venueid);

                        var formData = new FormData();
                        formData.append("Logo", sFile);
                        formData.append("VenueId", venueid);

                        console.log('formData');
                        console.log(formData);

                        $.ajax({
                            type: "POST",
                            url: "/venues/updatelogo",
                            data: formData,
                            dataType: 'json',
                            contentType: false,
                            processData: false,
                            success: function (response) {
                                loading.hide();
                                oksign.show();
                            },
                            error: function (error) {
                                console.log("Error");
                                console.log(error);
                            }
                        });
                    }
                    else {
                        loading.hide();
                        oksign.show();
                    }
                }
            });

        }, 2000);

    });

    // This example displays an address form, using the autocomplete feature
    // of the Google Places API to help users fill in the information.
    var placeSearch, autocomplete;
    var componentForm = {
        street_number: 'short_name',
        route: 'long_name',
        locality: 'long_name',
        administrative_area_level_1: 'short_name',
        country: 'long_name',
        postal_code: 'short_name'
    };

    function InitializeAddressSearch() {
        // Create the autocomplete object, restricting the search
        // to geographical location types.
        autocomplete = new google.maps.places.Autocomplete(
            /* type {HTMLInputElement} */(document.getElementById('autocomplete')),
            { types: ['geocode'] });
        // When the user selects an address from the dropdown,
        // populate the address fields in the form.
        google.maps.event.addListener(autocomplete, 'place_changed', function () {
            fillInAddress();
        });
    }

    // [START region_fillform]
    function fillInAddress() {
        // Get the place details from the autocomplete object.
        var place = autocomplete.getPlace();

        console.log('place');
        console.log(place);

        $('#asLat').val(place.geometry.location.A);
        $('#asLng').val(place.geometry.location.F);
        $('#asPlaceId').val(place.place_id);

        for (var component in place.address_components) {
            var comp = place.address_components[component];
            var lname = comp.long_name;
            var sname = comp.short_name;
            var compType = comp.types[0];

            switch (compType) {
                case "street_number":
                    $('#asAddress').val(lname);
                    break;
                case "route":
                    $('#asAddress').val($('#asAddress').val() + ' ' + lname);
                    break;
                case "locality":
                    $('#asCity').val(lname);
                    break;
                case "administrative_area_level_1":
                    $('#asState').val(sname);
                    break;
                case "postal_code":
                    $('#asZip').val(lname);
                    break;
            }
        }
    }
    // [END region_fillform]

    // [END region_geolocation]
    ///////// END OF ADD SPOT ////////////////////////////////////////////////////////////////

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
    $('#showmap, #showlist').click(function () {
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


    //page load
    GetFindResults();
});

function GetRemResults() {
    var sportid = currPeep.SportId;
    var winWd = $(window).width();

    remmResults.hide();
    remResults.html('<img class="center-block" src="../images/loading_animated.gif" style="width:50%;" />');

    //Get results
    $.ajax({
        type: "POST",
        url: "/venues/getremembered",
        data: { sportid: sportid },
        dataType: 'json'
    })
    .fail(function (response) {
        remResults.html(response);
        console.log('Error');
        console.log(response);
    })
    .success(function (response) {

        console.log('Spots');
        console.log(response.Spots);

        if (response.Spots.length > 0) {
            PlotVenuesOnMap(response, 'remResults', 'remMapResults', 2);
            ShowList('remResults', 'remMapResults', 2);
            if (winWd <= widthLimit) { $('#remResults').hide(); } else { $('#remResults').show(); }
            if (winWd <= widthLimit) { $('#spanFilter').hide(); }
        }
        else {
            remResults.html('<div class="text-center greytext"><br/><br/><h3>No venues have been remembered.</h3></div>');
        }
    });
}

function GetFindResults() {
    var dist = $("#sliderDist").slider("value");
    
    fmResults.hide();
    fResults.html('<img class="center-block" src="../images/loading_animated.gif" style="width:50%;" />');

    SearchGoogleForVenues(dist);
}

function PlotVenuesOnMap(response, listBoxName, mapBoxName, searchType) {

    $('#' + mapBoxName).show();

    var centerLoc = new google.maps.LatLng(currPeep.Lat, currPeep.Lng);
    map = new google.maps.Map(document.getElementById(mapBoxName), {
        center: centerLoc,
        zoom: 10
    });

    infowindow = new google.maps.InfoWindow();

    ClearMarkers();
    mapResults = [];

    var v = 0;
    for (var spot in response.Spots) {
        v++;
        var venue = response.Spots[spot];
        CreateMarker(new google.maps.LatLng(venue.Lat, venue.Lng), v, venue, searchType, false);
        mapResults.push(venue);
    }

    //Adding current location pin
    CreateMarker(new google.maps.LatLng(currPeep.Lat, currPeep.Lng), 0, null, searchType, true);
}

function ShowList(listBoxName, mapBoxName, searchType) {
    var listBox = $('#' + listBoxName);
    var mapBox = $('#' + mapBoxName);
    var resultsHtml = '<table class="table table-hover">';
    var htmllinklist = '';
    var winWd = $(window).width();

    //Show list results
    var row = 1;
    for (var r in mapResults) {
        var spot = mapResults[r];
        var sAddress2 = $.trim(spot.Address2) != '' ? '<br/>' + spot.Address2 : '';
        var remlink = searchType == 1 ? '<span title="Remember" data-location="find" style="font-size:18px;" class="remembervenue btn glyphicon glyphicon-plus-sign actionlink" data-id="' + spot.VenueId + '"></span>' : '<span title="Un-Remember" style="font-size:18px;" class="unremembervenue btn glyphicon glyphicon-minus-sign actionlink" data-id="' + spot.VenueId + '"></span>';

        if (Number(spot.Remembered) != 0) {
            remlink = remlink.replace('remembervenue', 'greytext');
        }

        var addy = spot.Address + ', ' + spot.City + ', ' + spot.State + ' ' + spot.Zip;

        resultsHtml += '<tr>' +
                            '<td>' +
                                '<div class="row-fluid">' +
                                    '<div class="col-sm-6">' +
                                        '<div style="font-size:16px;">' +
                                            '<a href="../venues/viewvenue?id=' + spot.VenueId + '"><b>' + row + ') <img src="../logos/' + spot.Logo + '" style="width:35px;" /> ' + spot.Name + '&nbsp;&nbsp;&nbsp;&nbsp;&lt;&nbsp;' + (Number(spot.Distance) + 1) + 'mi.</b></a>' +
                                        '</div>' +
                                        '<div style="font-size:16px;">' + spot.Address + sAddress2 + '<br/>' + 
                                            spot.City + ', ' + spot.State + ' ' + spot.Zip + 
                                        '</div>' +
                                    '</div>' +
                                    '<div class="col-sm-6 text-right">' +
                                        '<span data-marker-indx="' + (row - 1) + '" class="maplink btn glyphicon glyphicon-map-marker actionlink" style="font-size:18px;"></span>' +
                                        '<a href="../venues/viewvenue?id=' + spot.VenueId + '" data-marker-indx="' + (row - 1) + '" class="btn glyphicon glyphicon-eye-open actionlink" style="font-size:18px;"></a>' +
                                        '<a target="_blank" href="../getdirections.aspx?addy=' + addy + '&lat=' + spot.Lat + '&lng=' + spot.Lng + '" class="btn glyphicon glyphicon-road actionlink" style="font-size:18px;"></a>' +
                                        remlink +
                                    '</div>' +
                                '</div>' +
                            '</td>' +
                        '</tr>';
        row++;
    }
    listBox.html(resultsHtml + '</table>');
    
    //Remember Event Handler
    if (searchType == 1) {
        $(".remembervenue").click(function () {
            RememberVenue($(this));
        });
    }

    //Unremember Event Handler
    else if (searchType == 2) {
        $(".unremembervenue").click(function () {
            UnRemember($(this));
        });
    }

    //Find on map event handler
    $('.maplink').click(function () {
        if (winWd <= widthLimit) { $('#showmap').click(); }

        google.maps.event.trigger(markers[$(this).attr('data-marker-indx')], 'click');
        $(window).scrollTop(0);
    });
}

function UnRemember(link) {
    var imgHtml = '<img src="../images/loading2.gif" style="height:20px;" />';

    console.log('test');

    //Show loading sign
    link.attr('class', '').attr('style', '');
    link.html(imgHtml);

    //Wait at least 2 seconds
    setTimeout(function () {

        //Get results
        $.ajax({
            type: "POST",
            url: "/venues/unremember",
            data: { venueid: link.attr('data-id') },
            dataType: 'json'
        })
        .fail(function (response) {
            link.html(response.responseText);

            console.log('Error');
            console.log(response);
        })
        .success(function (response) {
            GetRemResults();
        });
    }, 1000);
}

function ClearMarkers() {
    //Remove markers from map
    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(null);
    }
    markers = [];
}

function CreateMarker(googleLatLng, num, spot, searchType, isCurrLoc) {
    var marker = new google.maps.Marker({
        map: map,
        position: googleLatLng,
        icon: (isCurrLoc ? '../images/icon_pin_black_40_red.png' : '../images/icon_pin_black_40.png')
    });
    markers.push(marker);

    if (!isCurrLoc) {
        google.maps.event.addListener(marker, 'click', function () {
            var sAddress2 = $.trim(spot.Address2) != '' ? '<br/>' + spot.Address2 : '';
            var remlink = searchType == 1 ? '<span title="Remember" data-location="find" style="font-size:18px;" class="remembervenue btn glyphicon glyphicon-plus-sign actionlink" data-id="' + spot.VenueId + '"></span>' : '<span title="Un-Remember" style="font-size:18px;" class="unremembervenue btn glyphicon glyphicon-minus-sign actionlink" data-id="' + spot.VenueId + '"></span>';

            if (Number(spot.Remembered) != 0) {
                remlink = remlink.replace('remembervenue', 'greytext');
            }

            var addy = spot.Address + ', ' + spot.City + ', ' + spot.State + ' ' + spot.Zip;

            var htmlmap =
                    '<table>' +
                        '<tr>' +
                            '<td style="font-size:16px;vertical-align:top;"><a href="../venues/viewvenue?id=' + spot.VenueId + '">' + num + ') <img src="../logos/' + spot.Logo + '" style="width:35px;" /></a></td>' +
                            '<td>' +
                                '<div>' +
                                    '<a href="../venues/viewvenue?id=' + spot.VenueId + '">' + spot.Name + '&nbsp;&nbsp;&nbsp;&nbsp;&lt;&nbsp;' + (Number(spot.Distance) + 1) + 'mi.</a><br/>' +
                                    spot.Address + sAddress2 + '<br/>' + 
                                    spot.City + ', ' + spot.State + ' ' + spot.Zip +
                                '</div>' +
                                '<div>' +
                                    '<a href="../venues/viewvenue?id=' + spot.VenueId + '" data-marker-indx="' + num + '" class="btn glyphicon glyphicon-eye-open actionlink" style="font-size:18px;"></a>' +
                                    '<a target="_blank" href="../getdirections.aspx?addy=' + addy + '&lat=' + spot.Lat + '&lng=' + spot.Lng + '" class="btn glyphicon glyphicon-road actionlink" style="font-size:18px;"></a>' +
                                    remlink +
                                '</div>' +
                            '</td>' +
                        '</tr>' +
                    '</table>';
            
            map.setCenter(marker.getPosition());

            infowindow.setContent(htmlmap);
            infowindow.open(map, this);

            //Remember Event Handler
            $(".remembervenue").click(function () {
                RememberVenue($(this));
            });
            
            //Unremember Event Handler
            $(".unremembervenue").click(function () {
                UnRemember($(this));
            });

        });
    }
}

function SearchGoogleForVenues(distance) {
    
    var centerLoc = new google.maps.LatLng(currPeep.Lat, currPeep.Lng);
    map = new google.maps.Map(document.getElementById('findMapResults'), {
        center: centerLoc,
        zoom: 10
    });

    //Search google for nearby spots
    var radius = distance * 1609.34; if (radius > 50000) { radius = 50000; }
    var request = {
        location: centerLoc,
        radius: radius,
        query: currPeep.Sport
    };

    var service = new google.maps.places.PlacesService(map);
    service.textSearch(request, SearchGoogleForVenues_Callback);
}

function SearchGoogleForVenues_Callback(results, status) {

    //Save results into database
    if (status == google.maps.places.PlacesServiceStatus.OK) {
        
        console.log('results');
        console.log(results);

        var venues = [];
        for (var i = 0; i < results.length; i++) {
            var arrAddy = results[i].formatted_address.split(',');
            var sAddress = $.trim(arrAddy[arrAddy.length - 4]);
            var sCity = $.trim(arrAddy[arrAddy.length - 3]);
            var sState = $.trim(arrAddy[arrAddy.length-2]).split(' ')[0];
            var sZip = $.trim(arrAddy[arrAddy.length - 2]).split(' ')[1];
            var sLat = results[i].geometry.location.A;
            var sLng = results[i].geometry.location.F;
            var sPlaceId = results[i].place_id;
            var winWd = $(window).width();

            if ($('#filterBox').is(':visible') && winWd <= widthLimit) {
                $('#filterBox').hide('slide', { direction: 'right' }, 500);
            }


            venues.push({
                name: results[i].name,
                address: sAddress,
                address2: '',
                city: sCity,
                state: sState,
                zip: sZip,
                desc: '',
                lat: sLat,
                lng: sLng,
                placeid: sPlaceId
            });
        }

        //Send ajax call to save
        $.ajax({
            type: "POST",
            url: "/venues/add",
            data: JSON.stringify({ venues: venues, sportid: currPeep.SportId }),
            dataType: 'json',
            traditional: true,
            contentType: 'application/json;'
        })
        .fail(function (response) {
            console.log('Error /venues/add');
            console.log(response);
        })
        .success(function (response) {

            //Get venues within distance selected
            var dist = $("#sliderDist").slider("value");
            $.ajax({
                type: "POST",
                url: "/venues/find",
                data: { searchType: 1, sportid: currPeep.SportId, distance: dist },
                dataType: 'json'
            })
            .fail(function (response) {
                console.log('Error /venues/find');
                console.log(response);
            })
            .success(function (response) {
                if (response.Spots.length > 0) {
                    PlotVenuesOnMap(response, 'findResults', 'findMapResults', 1);
                    ShowList('findResults', 'findMapResults', 1);
                    if (winWd <= widthLimit) { $('#findResults').hide(); } else { $('#findResults').show(); }
                    if (winWd <= widthLimit) { $('#filterLinks').show(); }
                }
                else {
                    $('#findResults').html('<div class="text-center greytext"><br/><br/><h3>No venues were found near you.</h3></div>');
                }
            });
        });
    }
}