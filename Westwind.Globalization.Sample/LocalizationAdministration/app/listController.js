/// <reference path="../bower_components/lodash/lodash.js" />
(function (undefined) {
    'use strict';

    var app = angular
        .module('app')
        .controller('listController', listController);

   listController.$inject = [ 'localizationService'];

    function listController(localizationService) {
        console.log('list controller');
        var vm = this;
        vm.resourceSet = null;
        vm.resourceSets = [];
        vm.resourceList = [];
        vm.resourceId = null;
        vm.localeIds = [];
        vm.localeId = null;
        vm.resourceStrings = [];
        vm.error = {};



        function handleErrorResult() {
            vm.error = vm.error;
            console.log(vm.error,arguments);
        };

        vm.getResourceSets = function getResourceSets() {
            localizationService.getResourceSets()
                .success(function(resourceSets) {
                    vm.resourceSets = resourceSets;
                    if (!vm.resourceSet && resourceSets.length > 0)
                        vm.resourceSet = vm.resourceSets[0];
                    vm.onResourceSetChange();
                })
                .error(handleErrorResult);
        };

        vm.onResourceSetChange = function onResourceSetChange() {
            vm.getAllLocaleIds();
            vm.getResourceList();

        };
        vm.onResourceIdChange = function onResourceIdChange() {
            vm.getResourceItem();
        }
        vm.onLocaleIdChanged = function onLocaleIdChanged(localeId) {            
            if (localeId !== undefined)
                vm.localeId = localeId;
            vm.getResourceItem(localeId);
        }

        vm.getResourceList = function getResourceList() {
            localizationService.getResourceList(vm.resourceSet)
                .success(function(resourceList) {
                    vm.resourceList = resourceList;
                    if (resourceList.length > 0) {
                        vm.resourceId = vm.resourceList[0].ResourceId;
                        setTimeout(function() { vm.onResourceIdChange(); }, 10);
                    }
                })
                .error(handleErrorResult);
        };

        vm.getResourceItem = function getResourceItem() {
            vm.getResourceStrings();

            localizationService.getResourceItem(vm.resourceId, vm.resourceSet, vm.localeId)
                .success(function(resourceItem) {
                    vm.resourceItem = resourceItem;
                    vm.resourceItemId = vm.resourceItem.ResourceId;
                })
                .error(handleErrorResult);
        };

        vm.getResourceStrings = function getResourceStrings() {
            localizationService.getResourceStrings(vm.resourceId, vm.resourceSet)
                .success(function(resourceStrings) {
                    vm.resourceStrings = resourceStrings;
                })
                .error(handleErrorResult);
        }

        vm.getAllLocaleIds = function getAllLocaleIds() {
            var oldId = vm.localeId;
            localizationService.getAllLocaleIds(vm.resourceSet)
                .success(function(localeIds) {
                    vm.localeIds = localeIds;
                    if (localeIds.length > 0) {
                        if (vm.localeId == null)
                            vm.localeId = vm.localeId = vm.localeIds[0].LocaleId;
                        else {
                            var idx = _.findIndex(vm.localeIds, function(localeId) {
                                console.log(localeId.LocaleId, oldId);
                                if (localeId.LocaleId === oldId)
                                    return true;
                                return false;
                            });
                            idx = idx > -1 ? idx : 0;
                            vm.localeId = vm.localeIds[1].LocaleId;
                            console.log("Setting Locale Id: " + vm.localeId + " " + idx);
                        }
                    } else
                        console.log("not setting Locale Id: " + vm.localeId);

                })
                .error(handleErrorResult);
        };

       

        // initialize
        vm.getResourceSets();        
    }
})();

