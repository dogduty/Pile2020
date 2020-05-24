app.controller('EmailDomains', function ($scope, APIService) {
    getDomains();

    function getDomains() {
        $scope.dataLoading = true;
        var servCall = APIService.getEmailSetup("EmailDomains");
        servCall.then(function (d) {
            $scope.emaildomains = d.data;
        }, function (error) {
            $log.error('Oops! Something went wrong while fetching the data.')
        });
        var servCall = APIService.getEmailSetup("EmailServers");
        servCall.then(function (d) {
            $scope.servers = d.data
        });
    };

    $scope.edit = function (domain) {
        $scope.errors = null;
        $scope.Id = domain == null ? 0 : domain.Id;
        $scope.Domain = domain == null ? null : domain.Domain;
        $scope.Server = domain == null ? null : domain.Sever;
        $scope.EmailServerId = domain == null ? null : domain.EmailServerId;
        $scope.editing = true;
    };

    $scope.save = function () {
        $scope.errors = null;
        var domain = {
            Id: $scope.Id,
            Domain: $scope.Domain,
            EmailServerId: $scope.EmailServerId,
        };
        var returned = APIService.saveEmailSetup("EmailDomains", domain);
        returned.then(function (result) {
            window.location.reload();
        }, function (error) {
            $scope.errors = error.data;
        });
    }

    $scope.delete = function (id, index) {
        $scope.errors = null;
        APIService.deleteEmailSetup("EmailDomains", id)
            .then(function (result) {
                $scope.emaildomains.splice(index, 1);
            }, function (error) {
                $scope.errors = error.data;
                getDomains();
            });
    };

});