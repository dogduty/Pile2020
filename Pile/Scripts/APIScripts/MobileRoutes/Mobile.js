function scopeAddon($scope, $parse, $location, APIService) {

    $scope.markComplete = function (thisStop) {
        for (i = 0; i < thisStop.Details.length; i++)
        {
            if (!thisStop.Details[i].Completed) {
                alert("Must complete all services!");
                return false;
            }
        }

        var servCall = APIService.genSave($scope.webApiController, thisStop);
        servCall.then(function (d) {
            thisStop.Completed = true;
            $scope.expandFirst();
        }, function (error) {
            $scope.errors = error.data;
        });

    }

    $scope.isNotCompleted = function (obj) {
        return !obj.Completed;
    }

    $scope.isCompleted = function (obj) {
        return obj.Completed;
    }

    $scope.expandFirst = function () {
        firstUnfinished = {};
        filtered = $scope.MobileRoutes.stopInfos.filter($scope.isNotCompleted);
        if (filtered && filtered.length > 0) {
            firstUnfinished = filtered[0];
        }
        $scope.expand(firstUnfinished);
    }

    $scope.showConditions = function (obj) {
        obj.ShowConditions = true;
    }

    $scope.expand = function (thisStop) {
        angular.forEach($scope.MobileRoutes.stopInfos, function (stop) {
            stop.Expand = false;
        });

        thisStop.Expand = true;
    }


}