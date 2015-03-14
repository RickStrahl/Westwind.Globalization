(function () {
    'use strict';

    angular
        .module('app')
        .controller('importExportResxController', importExportResxController);
    importExportResxController.$inject = ['localizationService'];

    function importExportResxController(localizationService) {        
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

        vm.exportResources = function() {
            localizationService.exportResxResources(vm.info.ResxBaseFolder)
                .success(function() {
                    $("#ImportExportResxDialog").modal('hide');
                });
        };

        vm.importResources = function() {
            localizationService.importResxResources(vm.info.ResxBaseFolder)
                .success(function() {
                    $("#ImportExportResxDialog").modal('hide');
                });
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