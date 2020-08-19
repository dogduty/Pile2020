var map;

function initMap() {
    map = new google.maps.Map(
        document.getElementById('map_canvas'), {
            center: new google.maps.LatLng(30.40491, -97.768267),
            zoom: 10,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        });
    
}

function createMarker(lat, lng, crew, day, stop, customerId, highlightedCustId) {
    //var rect = {
    //    path: "M 0 0 L -50 -50 L -50 -100 L 0 -100 z",
    //    fillColor: color,
    //    fillOpacity: 0.8,
    //    scale: 0.3,
    //    strokeColor: "black",
    //    strokeWeight: 1,
    //    labelOrigin: { x: -27.5, y: -60 }
    //};
    var iconImage = "/images/mapicons/route" + day + crew + "/number_" + parseInt(stop) + ".png";

    if (customerId == highlightedCustId)
        iconImage = "/images/mapicons/target.png";

    var marker = new google.maps.Marker({
        position: { lat: lat, lng: lng },
        icon: { url: iconImage, scaledSize: new google.maps.Size(20, 34) }, //rect
        //label: { color: textColor, text: text }, /*new google.maps.MarkerLabel({ color: "black", text: "1" })*/
        map: map
    });

    if (customerId == highlightedCustId) {
        marker.setZIndex(google.maps.Marker.MAX_ZINDEX);
        marker.setAnimation(google.maps.Animation.DROP);
    }


    return marker;
}

function getInfoHtml(data) {
    html = "<font size='2'><table>" +
        "<tr><td>Customer Name:</td> <td><b>" + data.CustomerName + "</b></td></tr>" +
        "<tr><td>Address:</td><td>" + data.Address + "</td></tr>" +
        "<tr><td>Crew:</td><td>" + data.CrewId + "</td></tr>" +
        "<tr><td>Day:</td><td>" + data.Day + "</td></tr>" +
        "<tr><td>Stop:</td><td>" + data.Stop + "</td></tr>" +
        "<tr><td>Services:</td><td>" + data.Services + "</td></tr>" +
        //"<tr><td><input type='hidden' id='oldcustid' value='" + customerid + "'/>" +
        //"<input type='hidden' id='oldcrew' value='" + crew + "'/>" +
        //"<input type='hidden' id='oldday' value='" + day + "'/>" +
        //"<input type='hidden' id='oldestnum' value='" + estnum + "'/>" +
        //"<input type='hidden' id='oldcount' value='" + count.toString() + "'/>" +
        //"<input type='hidden' id='oldcustname' value='" + firstname + " " + lastname + "'/>" +
        //"<input type='hidden' id='oldaddress' value='" + address + "'/>" +
        //"<input type='hidden' id='oldservices' value='" + services + "'/>" +
        "</td></tr><table></font>";

    return html;
}