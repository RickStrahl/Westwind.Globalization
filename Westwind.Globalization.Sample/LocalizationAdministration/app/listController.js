/// <reference path="localizationservice.js" />
/// <reference path="../bower_components/lodash/lodash.js" />
(function (undefined) {
    'use strict';
    
    var app = angular
        .module('app')
        .controller('listController', listController);

   listController.$inject = [ '$scope','$timeout','localizationService'];

   function listController( $scope,$timeout,localizationService) {
       console.log('list controller');

        var vm = this;
        vm.resourceSet = null;
        vm.resourceSets = [];
        vm.resourceList = [];
        vm.resourceId = null;
        vm.localeIds = [];
        vm.localeId = null;

        vm.resourceItems = [];
        vm.resourceItemIndex = 0;

        vm.editedResource = null,
        vm.error = {
            message:null,
            icon: "info-circle",
            cssClass: "info"
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
        vm.updateResourceString = function (value, localeId) {            
            localizationService.updateResourceString(value, vm.resourceId, vm.resourceSet, localeId)
                .success(function() {                               
                    vm.getResourceItems();
                    showMessage("Resource saved.");
                })
                .error(handleErrorResult);
        };

        vm.onResourceSetChange = function onResourceSetChange() {
            vm.getAllLocaleIds();
            vm.getResourceList();

        };
        vm.onResourceIdChange = function onResourceIdChange() {
            vm.getResourceItems();
        };
        vm.onLocaleIdChanged = function onLocaleIdChanged(localeId) {
            if (localeId !== undefined)
                vm.localeId = localeId;            
        };
        vm.onStringUpdate = function onStringUpdate(resource) {                
            vm.localeId = resource.LocaleId;
            vm.editedResource = resource.Value;
            vm.updateResourceString(resource.Value, resource.LocaleId);
        };
       vm.onResourceKeyDown = function onResourceKeyDown(ev,resource,form) {
           // Ctrl-Enter - save and next field
           if (ev.ctrlKey && ev.keyCode === 13) {
               vm.onStringUpdate(resource);
               $timeout(function () {
                   // set focus to next field
                   var el = $(ev.target);
                   var id = el.prop("id").replace("value_", "") * 1;
                   var $el = $("#value_" + (id + 1));                   
                   if ($el.length < 1) 
                       $el = $("#value_0"); // loop around
                   $el.focus();
               }, 100);
               $scope.resourceForm.$setPristine();

           }
       };
       vm.onTranslateClick = function (ev, resource) {               
           vm.localeId = resource.LocaleId;
           vm.editedResource = resource.Value;
           var id = $(ev.target).parent().find("textarea").prop("id");

           // notify Translate Dialog of active resource and source element id
           $scope.$emit("startTranslate", resource, id);
           $("#TranslateDialog").modal();
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

        vm.getResourceItems = function getResourceItems() {

            localizationService.getResourceItems(vm.resourceId, vm.resourceSet)
                .success(function(resourceItems) {
                    vm.resourceItems = resourceItems;                   
                })
                .error(handleErrorResult);
        };

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
                                if (localeId.LocaleId === oldId)
                                    return true;

                                return false;
                            });
                            idx = idx > -1 ? idx : 0;
                            vm.localeId = vm.localeIds[1].LocaleId;
                        }
                    }
                })
                .error(handleErrorResult);
        };
   


        function showMessage(msg, icon, cssClass) {
            
            if (!vm.error)
                vm.error = {};
            if (msg)
                vm.error.message = msg;

            if (icon)
                vm.error.icon = icon;
            else
                vm.error.icon = "info-circle";

            if (cssClass)
                vm.error.cssClass = cssClass;
            else
                vm.error.cssClass = "info";            

            $timeout(function() {
                if (msg === vm.error.message)
                    vm.error.message = null;
            }, 5000);            
        }

        function handleErrorResult(msg) {
            msg = msg || vm.error.message;

            showMessage(msg, "warning", "warning");            
        };

       $(document.body).keydown(function(ev) {
           if (ev.keyCode == 76 && ev.altKey) {
               $("#ResourceIdList").focus();
           }
       });
       

        // initialize
        vm.getResourceSets();        
    }
})();

