function scopeAddon($scope, $parse, $location, APIService) {

    $scope.empEdit = function (id) {
        window.location.href = "/Employees/" + id;
    }

    $scope.getAllEmployees = function () {
        $scope.getAll("showall=true");
    }

}