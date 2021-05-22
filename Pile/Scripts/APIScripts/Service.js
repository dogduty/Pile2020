app.service("APIService", function ($http) {

    this.genGetAll = function (controller, qs) {
        if (qs === undefined)
            return $http.get("/api/" + controller);
           
        return $http.get("/api/" + controller + "?" + qs);
    }

    this.genGetDetails = function (controller, id) {
        return $http.get("/api/" + controller + "/details/" + id)
    }

    this.genGetOne = function (controller, id) {
        return $http.get("/api/" + controller + "/" + id);
    }

    this.genDelete = function (controller, id) {
        return $http.delete("/api/" + controller + "/" + id);
    }

    this.genGetNew = function (controller, objType) {
        if (objType === undefined)
            return $http.get("/api/" + controller + "/new")

        return $http.get("/api/" + controller + "/new/" + objType);
    }

    this.genSave = function (controller, obj) {
        var method = "post";
        var url = "/api/" + controller;

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
        var url = "/api/" + controller + '/' + id;
        var request = $http({
            method: method,
            url: url,
            data: obj
        });
        return request;
    }

    this.getEmailSetup = function (objType) {
        return $http.get("api/" + objType);
    }

    this.deleteEmailSetup = function (objType, id) {
        return $http.delete("api/" + objType + "/" + id);
    }

    this.saveEmailSetup = function (objType, obj) {

        var method = "post";
        var url = "/api/" + objType;

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

