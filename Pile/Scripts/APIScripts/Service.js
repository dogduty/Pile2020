app.service("APIService", function ($http) {

    this.genGetAll = function (controller, qs) {
        if (qs === undefined)
            return $http.get(appApiPath + controller);
           
        return $http.get(appApiPath + controller + "?" + qs);
    }

    this.genGetDetails = function (controller, id) {
        return $http.get(appApiPath + controller + "/details/" + id)
    }

    this.genGetOne = function (controller, id) {
        return $http.get(appApiPath + controller + "/" + id);
    }

    this.genDelete = function (controller, id) {
        return $http.delete(appApiPath + controller + "/" + id);
    }

    this.genGetNew = function (controller, objType) {
        if (objType === undefined)
            return $http.get(appApiPath + controller + "/new")

        return $http.get(appApiPath + controller + "/new/" + objType);
    }

    this.genSave = function (controller, obj) {
        var method = "post";
        var url = appApiPath + controller;

        if (obj.Id != null && obj.Id != 0) {
            method = "put";
            url += "/" + obj.Id
        }

        var request = $http({
            method: method,
            url: url,
            data: obj
        });
        return request;
    };

    this.genSaveWithId = function (controller, id, obj) {
        var method = "post";
        var url = appApiPath + controller + '/' + id;
        var request = $http({
            method: method,
            url: url,
            data: obj
        });
        return request;
    }

    this.getEmailSetup = function (objType) {
        return $http.get(appApiPath + objType);
    }

    this.deleteEmailSetup = function (objType, id) {
        return $http.delete(appApiPath + objType + "/" + id);
    }

    this.saveEmailSetup = function (objType, obj) {

        var method = "post";
        var url = appApiPath + objType;

        if (obj.Id != null && obj.Id != 0) {
            method = "put";
        }

        var request = $http({
            method: method,
            url: url,
            data: obj
        });
        return request;

    };


});

