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
        vm.resourceSets = null;
        vm.selectedResourceSets = [];
        
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
            if (vm.selectedResourceSets && vm.selectedResourceSets.length == 1 && !vm.selectedResourceSets[0])
                vm.selectedResourceSets = null;
            localizationService.exportResxResources(vm.info.ResxBaseFolder, vm.selectedResourceSets)
                .success(function() {
                    $("#ImportExportResxDialog").modal('hide');
                    parentView.showMessage(vm.dbRes("ResxResourcesHaveBeenCreated"));
                })
                .error(parentView.parseError);
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

            setTimeout(function () {
                vm.resourceSets = localizationService.resourceSets;
                vm.selectedResourceSets = [""];
            }, 20);
        }
    }
})();;