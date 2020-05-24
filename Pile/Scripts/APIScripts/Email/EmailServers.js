app.controller('EmailServers', function ($scope, APIService) {
    getServers();

    function getServers() {
        var servCall = APIService.getEmailSetup("EmailServers");
        servCall.then(function (d) {
            $scope.emailservers = d.data;
        }, function (error) {
            $log.error('Oops! Something went wrong while fetching the data.')
        });
    }

});

