(function () {
    'use strict';

    angular
        .module('app')
        .controller('createClassController', createClassController);
    createClassController.$inject = ['$scope','localizationService'];

    function createClassController($scope,localizationService) {        
        var vm = this;
        vm.resources = resources;
        vm.dbRes = resources.dbRes;        
        vm.info = null;
        
        initialize();

        function initialize() {
            localizationService.getLocalizationInfo()
                .success(function (pi) {
                    vm.info = pi;
                });
        }

        vm.onCreateClassClick = function () {
            var file = vm.info.StronglyTypedGlobalResource;
            var ns = vm.info.ResourceBaseNamespace;
            var parentView = $scope.$parent.view;
            
            localizationService.createClass(file,ns)
                .success(function () {
                    $("#CreateClassDialog").modal('hide');                   
                    parentView.showMessage(vm.dbRes("StronglyTypedClassCreated"));
                })
                .error(parentView.parseError);
        };


      


    }
})();;