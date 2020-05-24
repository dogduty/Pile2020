app.controller('ExistingCheck', ['$scope', '$parse', 'APIService', '$attrs', function ($scope, $parse, APIService, $attrs) {
    init();

    function init() {
        $scope.email = $attrs.email ?? "";
        $scope.lastName = $attrs.lastname ?? "";
        $scope.address = $attrs.address ?? "";
    }

    $scope.getList = function () {

        if ($scope.email.trim() == "" && $scope.lastName.trim() == "" && $scope.address.trim() == "")
            return;

        $("#LastName").val($scope.lastName.trim());
        $("#Address").val($scope.address.trim());
        $("#Email").val($scope.email.trim());

        id = encodeURI("?email=" + $scope.email.trim() + "&lastName=" + $scope.lastName + "&address=" + $scope.address);

        var servCall = APIService.genGetOne('Customers/DupeCheck', id);
        servCall.then(function (d) {
            $scope.objs = d.data;
        }, function (error) {
            $scope.errors = error.data;
        });
    }

}]);
