function scopeAddon($scope, $parse, $location, APIService) {


    $scope.phoneTypes = ["Mobile", "Home", "Work", "Other"];

    $scope.flags = [{ Id: null, Name: "" }, { Id: "L", Name: "Late" }, { Id: "C", Name: "Call" }, { Id: "F", Name: "Final" }];
    $scope.custTypes = [{ Type: "R", Name: "Residential" }, { Type: "C", Name: "Commercial" }];
    $scope.actions = [
        { Id: "/CustomerTasks?CustomerId=", Name: "Tasks" },
        { Id: "/CustomerTasks?CustomerId=", Name: "New Contact" },
        { Id: "/CustomerTasks?CustomerId={0}", Name: "Tasks" },
        { Id: "/CustomerContactLog.aspx?CustID={0}", Name: "New Contact" },
        { Id: "/CustomerChangeLog.aspx?CustID={0}", Name: "New Change" },
        { Id: "/CustomerComplaintLog.aspx?CustID={0}", Name: "Complaints" },
        { Id: "/CustomerInvoiceListing.aspx?CustID={0}", Name: "Invoice Detail" },
        { Id: "/CustomerContactListing.aspx?CustID={0}", Name: "Contact Log" },
        { Id: "/CustomerHistoryListing.aspx?CustID={0}", Name: "Change Log" },
        { Id: "/CustomerServiceListing.aspx?CustID={0}", Name: "Service History" },
        { Id: "/QBAccountHistory.aspx?CustID={0}", Name: "Account History" }
    ];

    $scope.addPhone = function (phoneType) {
        if (phoneType == null)
            return;

        $scope.phoneTypes = $scope.phoneTypes.filter(i => i !== phoneType);
        $scope.getNewObjectOfType($scope.Customers.Customer.Phones, "Phones", phoneType);
    }

    $scope.refilterPhone = function () {
        if (!$scope.Customers) { return; }
        $scope.Customers.Customer.Phones.forEach(phone => {
            $scope.phoneTypes = $scope.phoneTypes.filter(i => i !== phone.PhoneType);
        });
    }

    $scope.addEmail = function () {
        $scope.getNewObjectOfType($scope.Customers.Customer.EmailAddresses, "EmailAddresses", "/?isPrimary=false");
    }

    $scope.addAddress = function () {
        $scope.getNewObjectOfType($scope.Customers.Customer.Addresses, "Addresses", "Mailing");
    }

    $scope.addNote = function () {
        $scope.getNewObjectOfType($scope.Customers.Customer.Notes, "Notes");
    }

    $scope.getDropDown = function (controller) {
        var servCall = APIService.genGetNew(controller);
        servCall.then(function (d) {
            var model = $parse(controller);
            model.assign($scope, d.data);
        }, function (error) {
            $scope.errors = error.data;
        });
    }

    $scope.getNewObjectOfType = function (list, controller, objType) {
        var servCall = APIService.genGetNew(controller, objType);
        servCall.then(function (d) {
            if (angular.isArray(list))
                list.push(d.data);
            else
                list = d.data;
        }, function (error) {
            $scope.errors = error.data;
        });
    }


    $scope.refilterPhone();

    $scope.getDropDown('HowFounds');
    $scope.getDropDown('PaymentMethods');
    $scope.getDropDown('InvoiceMethods');
    
}