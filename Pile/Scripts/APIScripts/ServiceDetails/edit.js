function scopeAddon($scope, $parse, $location, APIService) {

    if ($scope.editObj === undefined)
        $scope.editObj = null;

    $scope.edit = function (obj, create = false) {
        if (obj == null) {
            $scope.editObj = null;
            return;
        }

        //Make sure we aren't double-firing on new record
        if ($scope.editObj != null && $scope.editObj.Id == obj.Id)
            return;

        if ($scope.editObj == null && obj.Id == 0 && create == true) {
            if ($scope.ServiceDetails.ServiceDays.filter(x => x.Id == 0).length > 0) {
                $scope.errors = "You can only create one new record at a time without first saving."
                return;
            }
        }

        $scope.editObj = {};
        $scope.errors = null;
        $scope.editObj.Id = obj.Id;
        $scope.editObj.CustomerId = $scope.ServiceDetails.Customer.Id;
        $scope.editObj.Day = obj.Day;
        $scope.editObj.CrewId = obj.CrewId;
        $scope.editObj.NumDogs = obj.NumDogs;
        $scope.editObj.EstNum = obj.EstNum;
        $scope.editObj.NonTaxable = obj.NonTaxable;
        $scope.editObj.ServiceDetails = create ? [] : obj.ServiceDetails;

        if ($scope.editObj.Id == 0) 
            if ($scope.ServiceDetails.ServiceDays.filter(x => x.Id == 0).length == 0)
                $scope.ServiceDetails.ServiceDays.push(obj);
        
    };

    $scope.filterUnusedServices = function (service) {
        var serviceIds = [];
        if (service == null || $scope.editObj == null || !$scope.editObj.ServiceDetails)
            return true;
        $scope.editObj.ServiceDetails.forEach(x => serviceIds.push(x.ServiceId));
        return !serviceIds.includes(service.Id);
    }

    $scope.deleteServiceDay = function (obj) {
        $scope.ServiceDetails.ServiceDays = $scope.ServiceDetails.ServiceDays.filter(x => x != obj);
        $scope.edit(null);
    }

    $scope.updateServiceDay = function () {
        var serviceDay = $scope.ServiceDetails.ServiceDays.filter(x => x.Id == $scope.editObj.Id)[0];
        serviceDay.Day = $scope.editObj.Day;
        serviceDay.CrewId = $scope.editObj.CrewId;
        serviceDay.NumDogs = $scope.editObj.NumDogs;
        serviceDay.EstNum = $scope.editObj.EstNum;
        serviceDay.NonTaxable = $scope.editObj.NonTaxable;
    }

    $scope.cloneServiceDay = function (serviceDay) {
        if ($scope.ServiceDetails.ServiceDays.filter(x => x.Id == 0).length > 0) {
            $scope.errors = "You can only create one new record at a time without first saving."
            return;
        }
        var clone = {};
        angular.copy(serviceDay, clone)
        clone.Id = 0;
        clone.ServiceDetails.forEach(x => x.ServiceDayId = 0); //don't want to clone that!
        $scope.ServiceDetails.ServiceDays.push(clone);
        $scope.edit(clone);
    }

    $scope.addServiceDetail = function (service) {
        if (service == null)
            return;
        //var serviceDay = $scope.ServiceDetails.ServiceDays.filter(x => x.Id == $scope.editObj.Id)[0];
        //serviceDay.ServiceDetails.push({
        $scope.editObj.ServiceDetails.push({
            ServiceId: service.Id,
            Description: service.Description,
            Price: service.Price,
            QtyPrice: service.QtyPrice,
            Discount: 0,
            Qty: service.AutoPop ? $scope.editObj.NumDogs : 0,
            AdditAmount: 0,
            EmpPayAdj: 0,
            CountDown: service.CountDown
        });
        //$scope.edit(serviceDay);
    }

    $scope.editSave = function () {
        $scope.saveWithId($scope.ServiceDetails.Customer.Id, $scope.ServiceDetails.ServiceDays, false);
        //var returned = $scope.saveWithId($scope.ServiceDetails.Customer.CustomerId, $scope.ServiceDetails.ServiceDays, false);
        //returned.then(function (result) {
        //    $scope.error = null;
        //    window.location.href = $scope.redirectTo;
        //}, function (error) {
        //    $scope.error = error.data;
        //});
    }

}