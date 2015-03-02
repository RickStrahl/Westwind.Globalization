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
       vm.listVisible = true;
        vm.searchText = null;
        vm.resourceSet = null;
        vm.resourceSets = [];
        vm.resourceList = [];
        vm.resourceId = null;
        vm.activeResource = null;
        vm.localeIds = [];        
        vm.resourceItems = [];
        vm.resourceItemIndex = 0;
       vm.newResourceId = null;
       vm.editedResource = null,
           vm.error = {
               message: null,
               icon: "info-circle",
               cssClass: "info"
           };

       vm.newResource = function() {
           return {
               "ResourceId": null,
               "Value": null,
               "Comment": null,
               "Type": "",
               "LocaleId": "",
               "ResourceSet": "",
               "TextFile": null,
               "BinFile": null,
               "FileName": ""
           };
       };


       vm.collapseList = function () {           
           vm.listVisible = !vm.listVisible;
           console.log(vm.listVisible);
       };

        vm.getResourceSets = function getResourceSets() {
            return localizationService.getResourceSets()
                .success(function(resourceSets) {
                    vm.resourceSets = resourceSets;
                    if (!vm.resourceSet && resourceSets && resourceSets.length > 0)
                        vm.resourceSet = vm.resourceSets[0];
                    vm.onResourceSetChange();
                })
                .error(parseError);
        };


       vm.updateResource = function(resource) {
           return localizationService.updateResource(resource)
                    .success(function () {
                        vm.getResourceItems();
                        showMessage("Resource saved.");
           })
           .error(parseError);
       };

        vm.updateResourceString = function (value, localeId) {            
            return localizationService.updateResourceString(value, vm.resourceId, vm.resourceSet, localeId)
                .success(function() {                               
                    vm.getResourceItems();
                    showMessage("Resource saved.");
                })
                .error(parseError);
        };

        

        vm.getResourceList = function getResourceList() {
            return localizationService.getResourceList(vm.resourceSet)
                .success(function(resourceList) {
                    vm.resourceList = resourceList;
                    if (resourceList.length > 0) {
                        vm.resourceId = vm.resourceList[0].ResourceId;
                        setTimeout(function() { vm.onResourceIdChange(); }, 10);
                    }
                })
                .error(parseError);
        };

        vm.getResourceItems = function getResourceItems() {

            localizationService.getResourceItems(vm.resourceId, vm.resourceSet)
                .success(function(resourceItems) {
                    vm.resourceItems = resourceItems;
                    if (vm.resourceItems.length > 0)
                        vm.activeResource = vm.resourceItems[0];
                })
                .error(parseError);
        };

   

        /// *** Event handlers *** 

        vm.onResourceSetChange = function onResourceSetChange() {            
            vm.getResourceList();
        };
        vm.onResourceIdChange = function onResourceIdChange() {
            vm.getResourceItems();
        };
       
        vm.onLocaleIdChanged = function onLocaleIdChanged(resource) {
            if (resource !== undefined) {                
                vm.activeResource = resource;
            }
        };
        vm.onStringUpdate = function onStringUpdate(resource) {            
            vm.activeResource = resource;
            vm.editedResource = resource.Value;
            vm.updateResourceString(resource.Value, resource.LocaleId);
        };
        vm.onResourceKeyDown = function onResourceKeyDown(ev, resource, form) {
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
       vm.onTranslateClick = function(ev, resource) {           
           vm.editedResource = resource.Value;
           var id = $(ev.target).parent().find("textarea").prop("id");

           // notify Translate Dialog of active resource and source element id
           $scope.$emit("startTranslate", resource, id);
           $("#TranslateDialog").modal();
       };

       // call back from translate controller
       $scope.$root.$on("translateComplete", function (e, lang, value) {
           var res = null;
           var index = -1;
           for (var i = 0; i < vm.resourceItems.length; i++) {
               res = vm.resourceItems[i];
               if (res.LocaleId === lang) {
                   index = i;
                   break;
               }                                  
               res = null;
           }

           if (res == null) {
               res = vm.newResource();
               res.LocaleId = lang;
               //res.Value = value;
               res.ResourceId = vm.resourceId;
               res.ResourceSet = vm.resourceSet;
               vm.resourceItems.push(res);
           }
           //else
           //    vm.resourceItems[index].Value = value;

           if (index == -1)
               index = vm.resourceItems.length - 1;
           
           ww.angular.applyBindingValue("#value_" + index, value);

           // assign the value directly to field
           // to force to $dirty state and show green check
           $timeout(function() {
               angular.element('#value_' + index)
                   .val(value)
                   .controller('ngModel')                   
                   .$setViewValue(value);
           }, 100);

       });

       vm.onResourceIdBlur = function() {
           if (!vm.activeResource.Value)
               vm.activeResource.Value = vm.activeResource.ResourceId;

       }
       vm.onAddResourceClick = function() {
           
           var res = vm.newResource();           
           res.ResourceSet = vm.activeResource.ResourceSet;
           res.LocaleId = vm.activeResource.LocaleId;
           res.ResourceId = vm.activeResource.ResourceId;
           vm.activeResource = res;

           $("#AddResourceDialog").modal();
       };

       vm.onSaveResourceClick = function () {
           vm.updateResource(vm.activeResource)
               .success(function () {
                   var id = vm.activeResource.ResourceId;
                   var resourceSet = vm.activeResource.ResourceSet;

                   // check to see if resourceset exists
                   var i = _.findIndex(vm.resourceSets, function (set) {
                       return set.ResourceSet === resourceSet;
                   });
                   if (i < 0) {
                       vm.resourceSets.unshift(vm.activeResource.ResourceSet);
                       vm.resourceSet = vm.activeResource.ResourceSet;
                       vm.onResourceSetChange();
                   }

                   // check if resourceId exists
                   var i = _.findIndex(vm.resourceList,function(res) {
                       return res.ResourceId === id;
                   });                   
                   if (i < 0)
                       vm.resourceList.unshift(vm.activeResource);

                   vm.resourceId = id;
                   vm.onResourceIdChange();

                   

                   $("#AddResourceDialog").modal('hide');
               })
               .error(function() {
                   var err = ww.angular.parseHttpError(arguments);
                   alert(err.message);
               });
       };
       vm.onDeleteResourceClick = function() {
           var id = vm.activeResource.ResourceId;

           if (!confirm(
               id +
               "\n\nAre you sure you want to delete this resource?"))
               return;

           localizationService.deleteResource(id, vm.activeResource.ResourceSet)
               .success(function() {
                   var i = _.findIndex(vm.resourceList, function(res) {
                       return res.ResourceId === id;
                   });

                   vm.resourceList.splice(i, 1);

                   if (i > 0)
                       vm.resourceId = vm.resourceList[i - 1].ResourceId;
                   else
                       vm.resourceId = vm.resourceList[0].ResourceId;
                   vm.onResourceIdChange();

                   showMessage(id + " resource deleted.");
               })
               .error(function() {
                   showMessage(id + " was not deleted deleted.");

               });
       };
       vm.onRenameResourceClick = function () {
           vm.newResourceId = null;
           $("#RenameResourceDialog").modal();
           $timeout(function() {
               $("#NewResourceId").focus();
           },1000);
       };
       vm.onRenameResourceDialogClick = function () {
           localizationService.renameResource(vm.activeResource.ResourceId, vm.newResourceId, vm.activeResource.ResourceSet)
               .success(function () {                                      
                   for (var i = 0; i < vm.resourceList.length; i++) {
                       var res = vm.resourceList[i];                       
                       if (res.ResourceId == vm.activeResource.ResourceId) {                               
                           vm.resourceList[i].ResourceId = vm.newResourceId;
                           break;
                       }
                   }
                   vm.activeResource.ResourceId = vm.newResourceId;
                   showMessage("Resource was renamed to '"+ vm.newResourceId + "" +"'.");
                   $("#RenameResourceDialog").modal("hide");
               })
               .error(parseError);
       }

       vm.onDeleteResourceSetClick = function () {
           if (!confirm("You are about to delete this resource set:\n\n     " + 
                        vm.resourceSet + "\n\n" +
               "Are you sure you want to do this?"))
               return;

           localizationService.deleteResourceSet(vm.resourceSet)
               .success(function () {
                   vm.getResourceSets();
                   showMessage("Resource set deleted.");
                   vm.resourceSet = vm.resourceSets[0];
                   vm.onResourceSetChange();
               })
               .error(parseError);
       }

       vm.onRenameResourceSetClick = function () {
           var newResourceSet = prompt("Rename resource set " + vm.resourceSet + " to:" , "");
           if (!newResourceSet)
               return;


           localizationService.renameResourceSet(vm.resourceSet, newResourceSet)
               .success(function() {
                   vm.getResourceSets()
                       .success(function() {
                           vm.resourceSets.every(function(rs) {
                               if (rs == newResourceSet) {
                                   vm.resourceSet = rs;
                                   vm.getResourceList();
                                   return false;
                               }
                               return true;
                           });
                       });


                   showMessage("Resource set renamed.");
               })
               .error(parseError);
       }
       vm.onReloadResourcesClick = function() {
           localizationService.reloadResources()
               .success(function() {
                   showMessage("Resources have been reloaded.");
               })
               .error(parseError);           
       };
       vm.onBackupClick = function () {
           localizationService.backup()
               .success(function () {
                   showMessage("Resources have been backed up.");
               })
               .error(parseError);
       };
       vm.onCreateTableClick = function () {
           localizationService.createTable()
               .success(function () {
                   vm.getResourceSets();
                   showMessage("Localization table has been created.");
               })
               .error(parseError);
       };
       vm.onCreateClassClick = function () {
           localizationService.createClass()
               .success(function () {
                   showMessage("Strongly typed class has been created. You'll have to re-compile your application to use any added resources.");
               })
               .error(parseError);
       };
       vm.onExportResxResourcesClick = function () {
           localizationService.exportResxResources()
               .success(function () {
                   showMessage("Resx Resources have been created in your Web project.");
               })
               .error(parseError);
       };
       vm.onImportResxResourcesClick = function () {
           localizationService.importResxResources()
               .success(function() {
                   vm.getResourceSets();
                   showMessage("Resx Resources have been imported in your Web project. Make sure to recompile your application and turn on code generation on any Resx resources for stronly typed Resx resources.");
               })
               .error(function() {
                   vm.getResourceSets();
                   parseError(arguments);
               });
       };


       function parseError(args) {           
           var err = ww.angular.parseHttpError(args || arguments);           
           showMessage(err.message,"warning","warning");
       }
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


        $(document.body).keydown(function (ev) {
           if (ev.keyCode == 76 && ev.altKey) {
               $("#ResourceIdList").focus();
           }
       });
       

        // initialize
        vm.getResourceSets();        
    }
})();

