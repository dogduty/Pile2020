app.controller('TaskProgress', ['$scope', '$interval', 'APIService', '$attrs', function ($scope, $interval, APIService, $attrs) {
    $scope.webApiController = 'TaskProgress';
    get();
    var timer = $interval(function () {
        get();
    }, 3000);

    function get() {
        var servCall = APIService.genGetOne($scope.webApiController, $attrs.model);
        servCall.then(function (d) {
            $scope.objs = d.data;
            $scope.curval = 100 * d.data.Current / d.data.Target;
            if (d.data.Completed == true && angular.isDefined(timer)) {
                $interval.cancel(timer);
            }
        }, function (error) {
            $scope.errors = error.data;
        });
    }

    $scope.get = function () {
        get();
    }



}]);


