function scopeAddon($scope, $parse, $location, APIService) {

    $scope.serviceHistoryCustomer = function () {
        $scope.getDetailOfType("customer")
    }

}