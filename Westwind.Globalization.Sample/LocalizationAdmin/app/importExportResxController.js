(function () {
    'use strict';

    angular
        .module('app')
        .controller('importExportResxController', importExportResxController);
    importExportResxController.$inject = ['localizationService'];

    function importExportResxController(localizationService) {        
        var vm = this;
        vm.info = null;        
        vm.importExportType = "Export";
        
        vm.onSubmitClick = function () {
            
            if (vm.importExportType === "Export")
                vm.exportResources();
            if (vm.importExportType === "Import")
                vm.importResources();
        };

        vm.exportResources = function () {
            console.log('exportresources');
            localizationService.exportResxResources(vm.info.ResxBaseFolder)
                .success(function () {
                    console.log('done');
                    $("#ImportExportResxDialog").modal('hide')
                });
        };

        vm.importResources = function () {

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