function scopeAddon($scope, $parse, $location, APIService) {

    $scope.showInactive = false;

    $scope.empEdit = function (id) {
        window.location.href = "/Employees/edit?id=" + id;
    }

    $scope.empActivate = function (id) {
        
    }

    $scope.getAllEmployees = function () {
        $scope.getAll("showall=true");
    }

}