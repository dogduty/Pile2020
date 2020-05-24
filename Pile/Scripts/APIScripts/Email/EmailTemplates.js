function scopeAddon($scope, APIService) {


    $scope.edit = function (obj) {
        

        $scope.errors = null;
        $scope.Id = obj == null ? 0 : obj.Id;
        $scope.EmailTypeId = obj == null ? 0 : obj.EmailTypeId;
        $scope.Title = obj == null ? null : obj.Title;
        $scope.EmailAccountId = obj == null ? null : obj.EmailAccountId;
        $scope.BodyTemplate = obj == null ? null : obj.BodyTemplate;
    };

    $scope.save = function () {
        $scope.errors = null;
        var obj = {
            Id: $scope.Id,
            Title: $scope.Title,
            EmailTypeId: $scope.EmailTypeId,
            EmailAccountId: $scope.EmailAccountId,
            BodyTemplate: $scope.BodyTemplate,
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

    $scope.sendTest = function () {
        $scope.errors = null;
        var obj = {
            Id: 0, //force this to be a put vs. a post.
            Title: $scope.Title,
            EmailTypeId: $scope.EmailTypeId,
            EmailAccountId: $scope.EmailAccountId,
            BodyTemplate: $scope.BodyTemplate,
        };
        var returned = APIService.genSave('EmailTemplates/SendTest/6527', obj);
        returned.then(function (result) {
            $scope.errors = result.data;
        }, function (error) {
            $scope.errors = error.data;
        });
    }

}