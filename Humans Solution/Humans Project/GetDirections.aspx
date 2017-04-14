<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetDirections.aspx.cs" Inherits="Humans.Web.GetDirections" %>

<!DOCTYPE html>
<html>
  <head>
    <meta charset="UTF-8" />
    <title>Get Directions</title>
    <script src="http://maps.google.com/maps/api/js?sensor=true"></script>
    <script>
        var userLatLng, map;
        function geolocationSuccess(position) {
            userLatLng = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);

            initializeMap();

            // Draw a circle around the user position to have an idea of the current localization accuracy
            /*var circle = new google.maps.Circle({
                center: userLatLng,
                radius: position.coords.accuracy,
                map: mapObject,
                fillColor: '#0000FF',
                fillOpacity: 0.5,
                strokeColor: '#0000FF',
                strokeOpacity: 1.0
            });
            mapObject.fitBounds(circle.getBounds());*/
        }

        function geolocationError(positionError) {
            document.getElementById("error").innerHTML += "Error: " + positionError.message + "<br />";
        }

        function geolocateUser() {

            // If the browser supports the Geolocation API
            if (navigator.geolocation) {
                var positionOptions = {
                    enableHighAccuracy: true,
                    timeout: 10 * 1000 // 10 seconds
                };
                navigator.geolocation.getCurrentPosition(geolocationSuccess, geolocationError, positionOptions);
            }
            else
                document.getElementById("error").innerHTML += "Your browser doesn't support the Geolocation API";
        }

        var directionsDisplay;
        var directionsService = new google.maps.DirectionsService();
        function initializeMap() {
            directionsDisplay = new google.maps.DirectionsRenderer();
            var mapOptions = {
                zoom: 7,
                center: userLatLng
            };
            map = new google.maps.Map(document.getElementById('map'), mapOptions);

            directionsDisplay.setMap(map);
            directionsDisplay.setPanel(document.getElementById('directions-panel'));

            calcRoute();
        }

        function calcRoute() {
            var start = userLatLng;
            var end = new google.maps.LatLng(<% = Request.QueryString["lat"].ToString() %>, <% = Request.QueryString["lng"].ToString() %>);
            var request = {
                origin: start,
                destination: end,
                travelMode: google.maps.TravelMode.DRIVING
            };
            directionsService.route(request, function (response, status) {
                if (status == google.maps.DirectionsStatus.OK) {
                    directionsDisplay.setDirections(response);
                }
            });
        }

        google.maps.event.addDomListener(window, 'load', geolocateUser);
    </script>
    <style type="text/css">
      #map, #directions-panel {
        width: 100%;
        height: 500px;
      }
    </style>
  </head>
  <body>
    <div id="map"></div>
    <div id="directions-panel"></div>
    <p id="error"></p>
  </body>
</html>