(function () {
    'use strict';

    angular
        .module('app')
        .controller('createClassController', createClassController);
    createClassController.$inject = ['$scope', 'localizationService'];

    function createClassController($scope, localizationService) {
        var vm = this;
        vm.resources = resources;
        vm.dbRes = resources.dbRes;
        vm.info = null;
        vm.resourceSets = localizationService.resourceSets;
        vm.selectedResourceSets = [];
        vm.classType = "DbRes";
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
            }, 50);
        }

        vm.onCreateClassClick = function () {
            var file = vm.info.StronglyTypedGlobalResource;
            var ns = vm.info.ResourceBaseNamespace;
            var parentView = $scope.$parent.view;
            debugger
            localizationService.createClass(file, ns, vm.selectedResourceSets, vm.classType, vm.info.selectedProject)
                .success(function () {
                    $("#CreateClassDialog").modal('hide');
                    parentView.showMessage(vm.dbRes("StronglyTypedClassCreated"));
                })
                .error(parentView.parseError);
        };





    }
})();;