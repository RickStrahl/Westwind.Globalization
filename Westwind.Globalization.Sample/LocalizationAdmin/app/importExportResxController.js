(function () {
    'use strict';

    angular
        .module('app')
        .controller('importExportResxController', importExportResxController);
    importExportResxController.$inject = ['$scope', 'localizationService'];

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
            debugger
            var parentView = $scope.$parent.view;
            if (vm.selectedResourceSets && vm.selectedResourceSets.length == 1 && !vm.selectedResourceSets[0])
                vm.selectedResourceSets = null;
            localizationService.exportResxResources(vm.info.ResxBaseFolder, vm.selectedResourceSets, vm.info.selectedProject)
                .success(function () {
                    $("#ImportExportResxDialog").modal('hide');
                    parentView.showMessage(vm.dbRes("ResxResourcesHaveBeenCreated"));
                })
                .error(parentView.parseError);
        };

        vm.importResources = function () {

            var parentView = $scope.$parent.view;
            localizationService.importResxResources(vm.info.ResxBaseFolder, vm.info.ProjectName)
                .success(function () {
                    $("#ImportExportResxDialog").modal('hide');
                    parentView.showMessage(vm.dbRes("ResxResourcesHaveBeenImported"));
                    parentView.getProjectNames().success(function () {

                    });
                })
                .error(parentView.parseError);
        };

        vm.getProjectNames = function getProjectNames() {
            var parentView = $scope.$parent.view;
            return localizationService.getProjectNames()
                .success(function (projectNames) {
                    vm.projectNames = projectNames;
                })
                .error(parentView.parseError);
        };

        vm.onProjectNameChange = function onProjectNameChange() {
            debugger
            localizationService.getResourceSets(vm.info.selectedProject).success(function (resourcesets) {
                debugger
                vm.resourceSets = resourcesets;
            });
            vm.selectedResourceSets = [""];
            //setTimeout(function () {
            //    localizationService.getResourceSets(vm.info.selectedProject);
            //    vm.selectedResourceSets = [""];
            //}, 20);

            //vm.getResourceSets(vm.projectName).success(function () {

            //});

        };

        initialize();

        function initialize() {
            localizationService.getLocalizationInfo()
                .success(function (pi) {
                    vm.info = pi;
                });
            vm.getProjectNames();
            setTimeout(function () {
                vm.resourceSets = localizationService.resourceSets;
                vm.selectedResourceSets = [""];
            }, 20);
        }
    }
})();;