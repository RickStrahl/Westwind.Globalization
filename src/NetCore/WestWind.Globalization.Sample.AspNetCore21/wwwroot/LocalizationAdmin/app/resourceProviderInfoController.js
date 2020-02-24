(function () {
    'use strict';

    angular
        .module('app')
        .controller('resourceProviderInfoController', resourceProviderInfoController);

    resourceProviderInfoController.$inject = ['$scope','$http', '$timeout', 'localizationService']; 


    function resourceProviderInfoController($scope, $http, $timeout, localizationService) {        
        var vm = this;
        vm.title = 'translateController';
        vm.resources = resources;
        vm.dbRes = resources.dbRes;
        vm.info = null;

        vm.error = {
            message: null,
            icon: "info-circle",
            cssClass: "info"
        };
        
        console.log("resourceProviderInfoController");
        localizationService.getLocalizationInfo()
            .success(function(info) {
            vm.info = info;
        });

    }
})();
