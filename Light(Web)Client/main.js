// This variable will contain the list of the stations of the chosen contract.
var stations = [];

// This is the map
var map = new ol.Map({
    target: 'map', // <-- This is the id of the div in which the map will be built.
    layers: [
        new ol.layer.Tile({
            source: new ol.source.OSM()
        })
    ],

    view: new ol.View({
        center: ol.proj.fromLonLat([7.0985774, 43.6365619]), // <-- Those are the GPS coordinates to center the map to.
        zoom: 5 // You can adjust the default zoom.
    })

});

function drawLines(param1, param2, param3, param4) {
    var coords = [[param1[0], param2[0]], [param3[0], param4[0]]];
    var lineString = new ol.geom.LineString(coords);

    // Transform to EPSG:3857
    lineString.transform('EPSG:4326', 'EPSG:3857');

    // Create the feature
    var feature = new ol.Feature({
        geometry: lineString,
        name: 'Line'
    });

    // Configure the style of the line
    var lineStyle = new ol.style.Style({
        stroke: new ol.style.Stroke({
            color: '#ffcc33',
            width: 10
        })
    });

    var source = new ol.source.Vector({
        features: [feature]
    });

    var vector = new ol.layer.Vector({
        source: source,
        style: [lineStyle]
    });
    map.addLayer(vector);
}


function callAPI(url, requestType, params, finishHandler) {
    var fullUrl = url;

    // If there are params, we need to add them to the URL.
    if (params) {
        // Reminder: an URL looks like protocol://host?param1=value1&param2=value2 ...
        fullUrl += "?" + params.join("&");
    }

    // The js class used to call external servers is XMLHttpRequest.
    var caller = new XMLHttpRequest();
    caller.open(requestType, fullUrl, true);
    // The header set below limits the elements we are OK to retrieve from the server.
    //caller.setRequestHeader("", "application/json");
    //caller.setRequestHeader('Access-Control-Allow-Origin');
    //.append('Access-Control-Allow-Origin', 'http://localhost:61616');
    // onload shall contain the function that will be called when the call is finished.
    //WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");
    caller.onload = finishHandler;

    caller.send();
}

function retrieveItinerary() {
    var startingPointLongitude = "w=" + document.getElementById("chosenContract1").value;
    var startingPointLatitude = "&x=" + document.getElementById("chosenContract2").value;
    var endingPointLongitude = "&y=" + document.getElementById("chosenContract3").value;
    var endingPointLatitude = "&z=" + document.getElementById("chosenContract4").value;
    //var targetUrl = "https://api.openrouteservice.org/v2/directions/cycling-regular?api_key=5b3ce3597851110001cf62481b45e6cc4dc346cbae234875d4c2a29e&start=7.239020,43.828603&end=7.5,43.847113";
    var targetUrl = "http://localhost:8733/Design_Time_Addresses/RoutingWithBikes/host/rest/itinerary?" + startingPointLongitude + startingPointLatitude + endingPointLongitude + endingPointLatitude;
    var requestType = "GET";
    var params = null;

    //When the contracts are retrieved, we need to fill the contract list in Step2.
    //This is done in the feedContractList function.
    var onFinish = feedItineraryList;

    callAPI(targetUrl, requestType, params, onFinish);

}

// This function is called when a XML call is finished. In this context, "this" refers to the API response.
function feedItineraryList() {
    // First of all, check that the call went through OK:
    if (this.status !== 200) {
        console.log("Contracts not retrieved. Check the error in the Network or Console tab.");
    } else {
        var response = this.responseText;
        //var responseObject = "[" + JSON.parse(response) + "]";
        var responseObject = JSON.parse(response);
        // Second step: extract the contract names from the list.
        var taille = responseObject.map(function (jsonRouteService) {
            return jsonRouteService.features[0].properties.segments[0].steps.length;
            //return jsonRouteService.features[0].properties.segments[0].steps;
        });
        var listContainer = document.getElementById("ww");
        for (var i = 0; i < taille; i++) {
            var steps = responseObject.map(function (jsonRouteService) {
                return jsonRouteService.features[0].properties.segments[0].steps[i].instruction;
            });
            listContainer.innerHTML += steps + "<br />";
        }
        var tailleCoordonees = responseObject.map(function (jsonRouteService) {
            return jsonRouteService.features[0].geometry.coordinates.length;
            //return jsonRouteService.features[0].properties.segments[0].steps;
        });
        for (var i = 0; i < tailleCoordonees - 1; i++) {
            var steps1 = responseObject.map(function (jsonRouteService) {
                return jsonRouteService.features[0].geometry.coordinates[i][0];
            });
            var steps2 = responseObject.map(function (jsonRouteService) {
                return jsonRouteService.features[0].geometry.coordinates[i][1];
            });
            var steps3 = responseObject.map(function (jsonRouteService) {
                return jsonRouteService.features[0].geometry.coordinates[i + 1][0];
            });
            var steps4 = responseObject.map(function (jsonRouteService) {
                return jsonRouteService.features[0].geometry.coordinates[i + 1][1];
            });
            drawLines(steps1, steps2, steps3, steps4)
        }

        // When the list is filled, display the Step 2:
        //document.getElementById("step2").style.display = "block";
    }
}


/*function feedItineraryList() {
    // First of all, check that the call went through OK:
    if (this.status !== 200) {
        console.log("Contracts not retrieved. Check the error in the Network or Console tab.");
    } else {
        var response = "[" + this.responseText + "]";
        var responseObject = JSON.parse(response);
        // Second step: extract the contract names from the list.
        var taille = responseObject.map(function (jsonRouteService) {
            return jsonRouteService.features[0].properties.segments[0].steps.length;
            //return jsonRouteService.features[0].properties.segments[0].steps;
        });
        var steps = responseObject.map(function (jsonRouteService) {
            return jsonRouteService.features[0].properties.segments[0].steps[1].instruction;
        });
        var listContainer = document.getElementById("ww");
        listContainer.innerHTML += steps;
        for (var i = 0; i < taille; i++) {
            var steps = responseObject.map(function (jsonRouteService) {
                return jsonRouteService.features[0].properties.segments[0].steps[i].instruction;
            });
            listContainer.innerHTML += steps + "<br />";
        }

        // When the list is filled, display the Step 2:
        //document.getElementById("step2").style.display = "block";
    }
}*/



