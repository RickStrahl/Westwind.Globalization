(function () {
    'use strict';

    angular
        .module('app')
        .controller('importExportResxController', importExportResxController);
    importExportResxController.$inject = ['$scope','localizationService'];

    function importExportResxController($scope, localizationService) {        
        var vm = this;
        vm.resources = resources;
        vm.dbRes = resources.dbRes;
        
        vm.info = null;        
        vm.importExportType = "Export";        
        vm.onSubmitClick = function () {
            
            if (vm.importExportType === "Export")
                vm.exportResources();
            if (vm.importExportType === "Import")
                vm.importResources();
        };

        vm.exportResources = function () {
            var parentView = $scope.$parent.view;
            localizationService.exportResxResources(vm.info.ResxBaseFolder)
                .success(function() {
                    $("#ImportExportResxDialog").modal('hide');
                    parentView.showMessage(vm.dbRes("ResxResourcesHaveBeenCreated"));
                });
        };

        vm.importResources = function() {
            var parentView = $scope.$parent.view;
            localizationService.importResxResources(vm.info.ResxBaseFolder)
                .success(function() {
                    $("#ImportExportResxDialog").modal('hide');                    
                    parentView.showMessage(vm.dbRes("ResxResourcesHaveBeenImported"));
                    parentView.getResourceSets();
                })
                .error(parentView.parseError);
        };

        initialize();

        function initialize() {
            localizationService.getLocalizationInfo()
                .success(function(pi) {
                    vm.info = pi;
                });
        }
    }
})();;