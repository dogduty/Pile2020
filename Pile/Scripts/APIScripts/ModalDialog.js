app.controller('dialogService', ['$scope', '$rootScope', '$modal', function ($scope, $rootScope, $modal) {
    $scope.placeHolder = "";

    $scope.launch = function (templateUrl) {
        var modalInstance = $modal.open({
            templateUrl: 'templateUrl'
        });
    };
}]);

app.controller('popupService', ['$scope', '$modalInstance', function ($scope, $modalInstance) {
    $scope.close = function () {
        $modalInstance.dismiss('cancel');
    };
}]);