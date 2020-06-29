app.controller('Generic', ['$scope', '$parse', 'APIService', function ($scope, $parse, APIService) {

    $scope.init = function (webApiController, loadMethod) {

        $scope.webApiController = webApiController;
        if (typeof scopeAddon === "function")
            scopeAddon($scope, APIService);

        if (loadMethod == "getAll")
            getAll(webApiController);
        if (loadMethod == "getNew")
            getNew();
    }

    function getNew() {
        var servCall = APIService.genGetNew($scope.webApiController);
        servCall.then(function (d) {
            var model = $parse($scope.webApiController);
            model.assign($scope, d.data);
        }, function (error) {
            $scope.errors = error.data;
        });
    }

    $scope.new = function () {
        getNew();
    }

    $scope.save = function (id) {
        $scope.errors = null;
        var obj = $parse($scope.webApiController);
        var servCall = APIService.genSave($scope.webApiController, obj($scope));
        servCall.then(function (d) {
            window.location.href = d.headers("location");
        }, function (error) {
            $scope.errors = error.data;
        });
    }

    function getAll(webApiController) {
        var servCall = APIService.genGetAll($scope.webApiController);
        servCall.then(function (d) {
            $scope.objs = d.data;
            $scope.editing = false;
        }, function (error) {
            $scope.errors = error.data;
        });
    }

    $scope.getAll = function () {
        getAll();
    }

    $scope.getList = function (webApiController) {
        var servCall = APIService.genGetAll(webApiController);
        var model = $parse(webApiController);
        servCall.then(function (d) {
            var model = $parse(webApiController);
            model.assign($scope, d.data);
        }, function (error) {
            $scope.errors = error.data;
        });
    }

    $scope.getOne = function (id, columnName) {
        var data = {};
        var servCall = APIService.genGetOne($scope.webApiController, id);
        servCall.then(function (d) {
            var model = $parse(columnName);
            model.assign($scope, d.data[columnName]);
        }, function (error) {
            $scope.errors = error.data;
        });
    }

    $scope.delete = function (id, index) {
        $scope.errors = null;
        APIService.genDelete($scope.webApiController, id)
            .then(function (result) {
                $scope.objs.splice(index, 1);
                $scope.edit(null);
            }, function (error) {
                $scope.errors = error.data;
            });
    }


}]);
