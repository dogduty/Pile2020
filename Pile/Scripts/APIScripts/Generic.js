app.controller('Generic', ['$scope', '$parse', '$location', 'APIService', function ($scope, $parse, $location, APIService) {

    $scope.init = function (webApiController, loadMethod, qualifier) {

        $scope.webApiController = webApiController;
        $scope.loadMethod = loadMethod;
        if (typeof scopeAddon === "function")
            scopeAddon($scope, $parse, $location, APIService);

        if (this[loadMethod]) {
            this[loadMethod](qualifier);
        }

    }

    $scope.new = function () {
        var servCall = APIService.genGetNew($scope.webApiController);
        servCall.then(function (d) {
            var model = $parse($scope.webApiController);
            model.assign($scope, d.data);
        }, function (error) {
            $scope.errors = error.data;
        });
    }

    $scope.getDetailOfType = function (type) {
        var params = new URLSearchParams(window.location.search);
        var servCall = APIService.genGetAll($scope.webApiController + "/" + type, params);
        servCall.then(function (d) {
            var model = $parse($scope.webApiController);
            model.assign($scope, d.data);
        }, function (error) {
            $scope.errors = error.data;
        });
    }

    $scope.getDetailQueryId = function() {
        var params = new URLSearchParams(window.location.search);
        return $scope.getDetails(params.get('id'));
    }

    $scope.getDetails = function (id) {
        var servCall = APIService.genGetDetails($scope.webApiController, id);
        servCall.then(function (d) {
            var model = $parse($scope.webApiController);
            fixDate(d.data);
            model.assign($scope, d.data);
        }, function (error) {
            $scope.errors = error.data;
        });
    }

    $scope.getAll = function (qs) {
        var servCall = APIService.genGetAll($scope.webApiController, qs);
        servCall.then(function (d) {
            var model = $parse($scope.webApiController);
            model.assign($scope, d.data);
            $scope.editing = false;
        }, function (error) {
            $scope.errors = error.data;
        }).catch(function (e) {
            var blah = e;
        });
    }


    $scope.save = function (objOverride) {
        $scope.errors = null;
        var obj = $parse($scope.webApiController);
        var servCall = APIService.genSave($scope.webApiController, (objOverride || obj($scope)));
        servCall.then(function (d) {
            if (d.headers("location") != null)
                window.location.href = d.headers("location");
            else {
                $scope.messages = "Saved.";
                if ($scope.loadMethod) {
                    $scope.init($scope.webApiController, $scope.loadMethod);
                }
            }
        }, function (error) {
            $scope.errors = error.data;
        });
    }

    $scope.saveWithId = function (id, objOverride) {

        $scope.errors = null;
        var obj = $parse($scope.webApiController);
        //I think "action" below is useless. . .. hmmm...
        var servCall = APIService.genSaveWithId($scope.webApiController + action, id, (objOverride || obj($scope)));
        servCall.then(function (d) {
            var model = $parse($scope.webApiController);
            $scope.messages = "Saved Successfully.";
            $scope.editObj = null;
        }, function (error) {
            $scope.errors = error.data;
        });
    }

    $scope.saveWithIdAndAction = function (id, action, objOverride) {
        if (!confirm("Are you sure you want to " + action + "?")) {
            return;
        }
        $scope.errors = null;
        var obj = $parse($scope.webApiController);
        var servCall = APIService.genSaveWithId($scope.webApiController + "/" + action, id, (objOverride || obj($scope)));
        servCall.then(function (d) {
            var model = $parse($scope.webApiController);
            $scope.messages = "Saved Successfully.";
            $scope.editObj = null;
        }, function (error) {
            $scope.errors = error.data;
        });
    }

    $scope.deleteChildObj = function (controller, list, item) {
        if (!confirm("All other changes will be lost.  Are you sure?")) {
            return;
        }
        $scope.errors = null;
        APIService.genDelete(controller, item.Id)
            .then(function (result) {
                var index = list.indexOf(item);
                list.splice(index, 1);
            }, function (error) {
                $scope.errors = error.data;
            });
    }

    $scope.clearError = function () {
        if ($scope.errors !== undefined)
            $scope.errors = null;
        if ($scope.messages !== undefined)
            $scope.messages = null;
    }

    $scope.window = function (url, target) {
        if (url === undefined || url == "")
            return;

        window.open(url, target);
    }

    $scope.isBlankOrNull = function (value) {
        if (!value || value == '')
            return true;

        return false;
    }

    $scope.isNotBlankOrNull = function (value) {
        return !$scope.isBlankOrNull(value);
    }

    $scope.getWeekday = function (dow) {
        var weekday = new Array(7);
        weekday[0] = "Sunday";
        weekday[1] = "Monday";
        weekday[2] = "Tuesday";
        weekday[3] = "Wednesday";
        weekday[4] = "Thursday";
        weekday[5] = "Friday";
        weekday[6] = "Saturday";
        return weekday[dow];
    }

    $scope.daysOfTheWeek = [
        { Id: 0, Day: "Sunday" },
        { Id: 1, Day: "Monday" },
        { Id: 2, Day: "Tuesday" },
        { Id: 3, Day: "Wednesday" },
        { Id: 4, Day: "Thursday" },
        { Id: 5, Day: "Friday" },
        { Id: 6, Day: "Saturday" }     
    ];

    $scope.selectableDays = [
        { Id: 0, Day: "Sun", selected: true },
        { Id: 1, Day: "Mon", selected: true },
        { Id: 2, Day: "Tue", selected: true },
        { Id: 3, Day: "Wed", selected: true },
        { Id: 4, Day: "Thu", selected: true },
        { Id: 5, Day: "Fri", selected: true },
        { Id: 6, Day: "Sat", selected: true }
    ];

    function fixDate(data) {
        //if (angular.isArray(data)) {
        //    fixDate(data);
        //}
        if (typeof data === 'string' && isDate(data))
            data = Date.parse(data);

        for (var prop in data) {
            if (data.hasOwnProperty(prop)) {
                if (angular.isArray(data[prop]) || angular.isObject(data[prop]))
                    fixDate(data[prop])
                else if (typeof data[prop] === 'string' && isDate(data[prop]))
                    data[prop] = new Date(Date.parse(data[prop]));
            }
        }
    }

    function isDate(_date) {
        const _regExp = new RegExp('^(-?(?:[1-9][0-9]*)?[0-9]{4})-(1[0-2]|0[1-9])-(3[01]|0[1-9]|[12][0-9])T(2[0-3]|[01][0-9]):([0-5][0-9]):([0-5][0-9])(.[0-9]+)?(Z)?$');
        return _regExp.test(_date);
    }




    //function getNew() {
    //    var servCall = APIService.genGetNew($scope.webApiController);
    //    servCall.then(function (d) {
    //        var model = $parse($scope.webApiController);
    //        model.assign($scope, d.data);
    //    }, function (error) {
    //        $scope.errors = error.data;
    //    });
    //}

    //function getDetails(qs) {
    //    var servCall = APIService.genGetDetails($scope.webApiController, id);
    //    servCall.then(function (d) {
    //        var model = $parse($scope.webApiController);
    //        model.assign($scope, d.data);
    //    }, function (error) {
    //        $scope.errors = error.data;
    //    });
    //}


    //function getAll(webApiController, qs) {
    //    var servCall = APIService.genGetAll($scope.webApiController, qs);
    //    servCall.then(function (d) {
    //        var model = $parse(webApiController);
    //        model.assign($scope, d.data);
    //        $scope.editing = false;
    //    }, function (error) {
    //        $scope.errors = error.data;
    //    }).catch(function (e) {
    //        var blah = e;
    //    });
    //}


    //$scope.getList = function (webApiController) {
    //    var servCall = APIService.genGetAll(webApiController);
    //    var model = $parse(webApiController);
    //    servCall.then(function (d) {
    //        var model = $parse(webApiController);
    //        model.assign($scope, d.data);
    //    }, function (error) {
    //        $scope.errors = error.data;
    //    });
    //}

    //$scope.getOne = function (id, columnName) {
    //    var data = {};
    //    var servCall = APIService.genGetOne($scope.webApiController, id);
    //    servCall.then(function (d) {
    //        var model = $parse(columnName);
    //        model.assign($scope, d.data[columnName]);
    //    }, function (error) {
    //        $scope.errors = error.data;
    //    });
    //}



}]);
