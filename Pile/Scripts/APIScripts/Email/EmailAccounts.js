app.controller('EmailAccounts', function ($scope, APIService) {
    getAccounts();

    //$scope.getAccounts = function() {
    function getAccounts() {
        var servCall = APIService.getEmailSetup("EmailAccounts");
        servCall.then(function (d) {
            $scope.emailaccounts = d.data;
            $scope.editing = false;
        }, function (error) {
            $scope.errors = error.data;
        });
        var servCall = APIService.getEmailSetup("EmailDomains");
        servCall.then(function (d) {
            $scope.domains = d.data
        });
    }

    $scope.edit = function (account) {
        $scope.errors = null;
        $scope.Id = account == null ? 0 : account.Id;
        $scope.LocalPart = account == null ? null : account.LocalPart;
        $scope.Domain = account == null ? null : account.Domain;
        $scope.DisplayName = account == null ? null : account.DisplayName;
        $scope.Password = null;
        $scope.editing = true;
    }

    $scope.save = function () {
        $scope.errors = null;
        var account = {
            Id: $scope.Id,
            LocalPart: $scope.LocalPart,
            Domain: $scope.Domain,
            DisplayName: $scope.DisplayName,
            Password: $scope.Password
        };
        var returned = APIService.saveEmailSetup("EmailAccounts", account);
        returned.then(function (result) {
            getAccounts();
            $scope.edit(null);
        });
    }

    $scope.delete = function (id, index) {
        $scope.errors = null;
        APIService.deleteEmailSetup("EmailAccounts", id)
            .then(function (result) {
                $scope.emailaccounts.splice(index, 1);
            }, function (error) {
                $scope.errors = error.data;
                getAccounts();
            });
    }

});