function scopeAddon($scope, $parse, $location, APIService) {

    if ($scope.editObj === undefined)
        $scope.editObj = null;

    $scope.edit = function (obj, create = false) {
        if (obj == null) {
            $scope.editObj = null;
            return;
        }
    }

    $scope.getRouteData = function () {
        $scope.markers = [];
        var servCall = APIService.genGetAll($scope.webApiController);
        var params = new URLSearchParams(window.location.search);
        var highlightedCustId = params.get('CustID');
        servCall.then(function (d) {
            $scope.days = [];
            $scope.crews = [];
            $scope.inactiveCrews = [];
            $scope.inaciveDays = [];
            d.data.Days.forEach(function (item) {
                $scope.days.push({ selected: true, item: item })
            });
            d.data.Crews.forEach(function (item) {
                $scope.crews.push({ selected: true, item: item })
            });
            d.data.Infos.forEach(function (item) {
                createMarkers(item, highlightedCustId);
            });
        }, function (error) {
            $scope.errors = error.data;
        }).catch(function (e) {
            var blah = e;
        });
    }

    function createMarkers(d, highlightedCustId) {

        var marker = createMarker(d.Lat, d.Lng, d.CrewId, d.Day, d.Stop, d.CustomerId, highlightedCustId);
        marker.infowindow = new google.maps.InfoWindow({
            content: getInfoHtml(d)
        });

        marker.addListener("click", () => {
            marker.infowindow.open(map, marker);
        });

        $scope.markers.push({ day: d.Day, crew: d.CrewId, marker });
    }

    $scope.toggleMarker = function () {

        var inactiveDays = $scope.days.filter(function (day) {
            return day.selected == false;
        });
        var inactiveCrews = $scope.crews.filter(function (crew) {
            return crew.selected == false;
        });
        for (var i = 0; i < $scope.markers.length; i++) {
            var isVisible = !(inactiveDays.some(x => x.item.Day == $scope.markers[i].day) || inactiveCrews.some(x => x.item.Id == $scope.markers[i].crew));
            if ($scope.markers[i].marker.visible != isVisible)
                $scope.markers[i].marker.setVisible(isVisible);
        }
    };

    
}