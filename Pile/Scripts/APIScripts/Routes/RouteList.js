function scopeAddon($scope, $parse, $location, APIService) {

    $scope.thisWeek = true;

    
    $scope.getRouteList = function (nextWeek) {
        if (nextWeek === undefined)
            weekOffset = false;

        var servCall = APIService.genGetAll('Routes/RouteList');
        servCall.then(function (d) {
            $scope.routeList = d.data;
            $scope.thisWeek = !nextWeek;
            $scope.editing = false;
        }, function (error) {
            $scope.errors = error.data;
        }).catch(function (e) {
            var blah = e;
        });
    }

    $scope.filterHammer = function (showAll) {
        $scope.routeList.employees.forEach(x => x.selected = showAll);
        $scope.selectableDays.forEach(x => x.selected = showAll);
        $scope.showHide();
    }

    $scope.selectAllChange = function () {
        var routes = $scope.routeList.routeInfos;
        for (var i = 0; i < routes.length; i++) {
            routes[i].Selected = $scope.routeList.selectAll && routes[i].Visible;
        }
    }

    $scope.transfer = function () {
        var selectedRoutes = $scope.routeList.routeInfos.filter(x => x.Selected);
        if (selectedRoutes.length == 0) {
            alert("Nothing Selected!")
            return;
        }

        if ($scope.routeList.transferTo == 0) {
            alert("Transfer to option not selected!")
            return;
        }

        var transferIds = [];
        selectedRoutes.forEach(x => transferIds.push(x.Id));

        var servCall = APIService.genSaveWithId($scope.webApiController + "/transfer", $scope.routeList.transferTo, transferIds);
        servCall.then(function (d) {
            var model = $parse($scope.webApiController);
            $scope.messages = "Saved Successfully.";
            $scope.getRouteList();
        }, function (error) {
            $scope.errors = error.data;
        });

        //$scope.saveWithId($scope.routeList.transferTo, transferIds, "transfer").then($scope.getRouteList();)
        //$scope.getRouteList();

    }

    $scope.showHide = function () {
        var routes = $scope.routeList.routeInfos;
        const employees = {};
        $scope.selectAll = false;
        $scope.routeList.employees.forEach(x => employees[x.id] = x);
        $scope.routeList.employees;
        var days = $scope.selectableDays;
        for (var i = 0; i < routes.length; i++) {
            if (employees[routes[i].EmployeeId] === undefined)
                routes[i].Visible = false;
            else 
                routes[i].Visible = days[routes[i].Day].selected && employees[routes[i].EmployeeId].selected;

            if (!routes[i].Visible)
                routes[i].Selected = false;  // if you hide it, unselected it.
        }
    }

}