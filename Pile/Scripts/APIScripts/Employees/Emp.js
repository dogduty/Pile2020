function scopeAddon($scope, $parse, $location, APIService) {

    $scope.showInactive = false;
    $scope.activating = false;

    $scope.empEdit = function (id) {
        window.location.href = "/Employees/edit?id=" + id;
    }

    $scope.deactivate = function (id) {
        window.location.href = "/Employees/deactivate?id=" + id;
    }

    $scope.empActivate = function () {
        $scope.Employees.User = {
            UserName: "",
            Email: ""
        };
        $scope.activating = true;
        document.querySelector('#UserName').focus();
    }

    $scope.isUserNull = function (obj) {
        var res = obj['User'] === null;
        return res;
    }

    $scope.getAllEmployees = function () {
        $scope.getAll("showall=true");
    }

}