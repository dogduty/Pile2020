function scopeAddon($scope, APIService) {

    $scope.edit = function (obj) {
        $scope.errors = null;
        $scope.Id = obj == null ? 0 : obj.Id;
        $scope.Name = obj == null ? null : obj.Name;
        $scope.Value = obj == null ? null : obj.Value;
    }

    
    $scope.save = function () {
        $scope.errors = null;
        var obj = {
            Id: $scope.Id,
            Name: $scope.Name,
            Value: $scope.Value,
        };
        var returned = APIService.genSave($scope.webApiController, obj);
        returned.then(function (result) {
            $scope.getAll();
            $scope.edit(null);
        }, function (error) {
            $scope.errors = error.data;
            getAll();
        });
    };

}

