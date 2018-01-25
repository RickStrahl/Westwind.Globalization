/// <reference path="localizationservice.js" />
/// <reference path="../bower_components/lodash/lodash.js" />
/// <reference path="../scripts/ww.resourceEditor.js" />
(function(undefined) {
    'use strict';

    var app = angular
        .module('app')
        .controller('listController', listController);

    listController.$inject = ['$scope', '$timeout', '$upload', 'localizationService'];

    function listController($scope, $timeout, $upload,  localizationService) {
        console.log('list controller');

        var vm = this;

        vm.resources = resources; // global generated resources
        vm.dbRes = resources.dbRes;

        vm.listVisible = true;
        vm.searchText = "";
        vm.resourceSet = null;
        vm.resourceSets = [];
        vm.resourceList = [];
        vm.isLocalizationTable = true;
        vm.resourceGridResources = [];
        vm.resourceId = null;
        vm.activeResource = null;
        vm.localeIds = [];
        vm.resourceItems = [];
        vm.resourceItemIndex = 0;
        vm.newResourceId = null;
        vm.uploadProgress = null;
        vm.resourceEditMode = false;
        vm.editedResource = null;
        vm.error = {
            message: null,
            icon: "info-circle",
            cssClass: "info"
        }

        vm.newResource = function() {
            return {
                "ResourceId": null,
                "Value": null,
                "Comment": null,
                "Type": "",
                "LocaleId": "",
                "IsRtl": false,
                "ResourceSet": "",
                "TextFile": null,
                "BinFile": null,
                "ValueType": 0,
                "FileName": ""
            };
        };


        vm.collapseList = function () {
            if ($("#ListPanel").width() < 80) {
                vm.listVisible = true;
                $("#ListPanel").width(310);
            } else {
                vm.listVisible = false;
                $("#ListPanel").width(0);
            }
        };

        vm.getResourceSets = function getResourceSets() {
            return localizationService.getResourceSets()
                .success(function(resourceSets) {
                    vm.resourceSets = resourceSets;
                    if (!vm.resourceSet && resourceSets && resourceSets.length > 0) {
                        vm.resourceSet = vm.resourceSets[0];
                        vm.isLocalizationTable = true;
                    } else {
                        localizationService.isLocalizationTable()
                            .success(function(exists) {
                                vm.isLocalizationTable = exists;
                            })
                            .error(parseError);
                    }

                    vm.onResourceSetChange();
                })
                .error(parseError);
        };


        vm.updateResource = function (resource) {            
            return localizationService.updateResource(resource)
                .success(function () {
                     // reset the items and rebind
                     vm.getResourceItems();
                     showMessage(vm.dbRes('ResourceSaved') + " [" +
                        resource.ResourceId +
                        " (" + (resource.LocaleId ? resource.LocaleId : "invariant") + ")]");                 
                })
                .error(parseError);
        };

        vm.updateResourceString = function (value, localeId) {
            return localizationService.updateResourceString(value, vm.resourceId, vm.resourceSet, localeId, vm.activeResource.Comment)
                .success(function () {
                    // reset the items and rebind                     

                    showMessage(vm.dbRes('ResourceSaved') +
                        " [" + vm.resourceId +
                        " (" + (localeId ? localeId : "invariant") + ")]");

                })
                .error(parseError);
        };


        var firstListLoad = true;
        vm.getResourceList = function getResourceList() {
            // ignore first auto load from render - 
            // let our explict render get the list so we load only once           
            if (firstListLoad) {
                firstListLoad = false;
                return;
            }
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
                .success(function (resourceItems) {
                    //setTimeout(function() {
                        vm.resourceItems = resourceItems;

                        if (vm.resourceItems.length > 0) {
                            vm.activeResource = vm.resourceItems[0];

                            for (var i = 0; i < vm.resourceItems.length; i++) {
                                var resource = vm.resourceItems[i];
                                if (!resource.Value) {
                                    resource.Value = !resource.Type
                                        ? resource.Value
                                        : 'binary: ' + resource.Type + ':' + resource.FileName;
                                }
                            }
                        }
                    //},2000);
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
        vm.onStringUpdate = function onStringUpdate(resource, event) {            
            if (event) {                
                var el = event.target;
                // update only if empty                
                if (el.className.indexOf("ng-dirty") < 0)
                    return;
            }

            vm.activeResource = resource;
            vm.editedResource = resource.Value;

            vm.updateResourceString(resource.Value, resource.LocaleId);
        };
        vm.onResourceFullscreenEdit = function(ev, resource) {
            $("#resource-editor").fullScreenEditor('show', {
                value: resource.Value,
                rtl: resource.IsRtl,
                onSave: function(value) {
                    var $el = $("textarea[data-localeid='" + resource.LocaleId + "'");
                    var id = $el.prop("id");
                    vm.activeResource.Value = value;
                    ww.angular.applyBindingValue("#" + id, value);
                }
            });
        }
        vm.onTranslateClick = function(ev, resource) {
            vm.editedResource = resource.Value;
            var id = $(ev.target).parent().find("textarea").prop("id");

            // notify Translate Dialog of active resource and source element id
            $scope.$emit("startTranslate", resource, id);
            $("#TranslateDialog").modal();
        };

        // call back from translate controller
        $scope.$root.$on("translateComplete", function(e, lang, value) {
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
                //res.ResourceId = vm.resourceId;
                res.ResourceSet = vm.resourceSet;
                vm.resourceItems.push(res);
            }
            // always set the resource id
            res.ResourceId = vm.resourceId;

            if (index == -1)
                index = vm.resourceItems.length - 1;

            ww.angular.applyBindingValue("#value_" + index, value);
            
            
            // assign the value directly to field
            // to force to $dirty state and show green check
            $timeout(function() {
                    var $el = angular.element('#value_' + index);
                    $el.val(value)
                        .controller('ngModel')
                        .$setViewValue(value);

                    $el.focus();
                    $timeout(function() { vm.onSaveResourceClick(); });
            },100);
        });

        vm.onResourceIdBlur = function() {
            if (!vm.activeResource.Value) {                
                localizationService.fromCamelCase(vm.activeResource.ResourceId)
                    .success(function(text) {
                        vm.activeResource.Value = text;
                    })
                    .error(function() {
                        vm.activeResource.Value = vm.activeResource.ResourceId;
                    });                
            }

            },
            vm.onLocaleIdBlur = function(localeId) {
                if (!localeId)
                    localeId = vm.activeResource.LocaleId;

                localizationService.isRtl(localeId)
                    .success(function(isRtl) {
                        vm.activeResource.IsRtl = isRtl;
                    });
            },
            vm.onAddResourceClick = function(resourceId, resourceSet, content) {

                if (!resourceId) {
                    if (vm.activeResource)
                        resourceId = vm.activeResource.ResourceId;
                    else
                        resourceId = "";
                }
                if (!resourceSet) {
                    if (vm.activeResource)
                        resourceSet = vm.activeResource.ResourceSet;
                    else
                        resourceSet = "";
                }
                var localeId = "";
                if (vm.activeResource)
                    localeId = vm.activeResource.LocaleId;

                var res = vm.newResource();
                res.ResourceSet = resourceSet;
                res.LocaleId = localeId;
                res.ResourceId = resourceId;
                res.Value = content;
                vm.activeResource = res;
                
                $("#AddResourceDialog")
                    .modal()
                    .on("shown.bs.modal",
                        function() { $("#ResourceId").focus(); });
            };
        vm.onEditResourceClick = function() {
            $("#AddResourceDialog").modal();            
        };

        vm.onCommentClick = function() {
            $("#CommentDialog").modal();            
        }
        vm.onSaveResourceClick = function() {
            vm.updateResource(vm.activeResource)
                .success(function() {
                    var id = vm.activeResource.ResourceId;
                    var resourceSet = vm.activeResource.ResourceSet;

                    // check to see if resourceset exists
                    var i = _.findIndex(vm.resourceSets, function(set) {
                        return set === resourceSet;
                    });
                    if (i < 0) {
                        vm.resourceSets.unshift(vm.activeResource.ResourceSet);
                        vm.resourceSet = vm.resourceSets[0];
                        vm.onResourceSetChange();
                    }

                    // check if resourceId exists
                    var i = _.findIndex(vm.resourceList, function(res) {
                        return res.ResourceId === id;
                    });
                    if (i < 0) {
                        vm.resourceList.unshift(vm.activeResource);
                        vm.activeResource = vm.resourceList[0];
                    }

                    setTimeout(function() {
                        vm.resourceId = id;
                        vm.onResourceIdChange();
                    },10);

                    $("#AddResourceDialog").modal('hide');
                })
                .error(function () {                    
                    var err = ww.angular.parseHttpError(arguments);
                    alert(err.message);
                });
        };
        vm.onCommentUpdateClick = function () {
            localizationService.updateComment(vm.activeResource.Comment,
                    vm.activeResource.ResourceId,
                    vm.activeResource.ResourceSet,
                    vm.activeResource.LocaleId)
                .success(function() {
                    $('#CommentDialog').modal('hide');
                })
                .error(parseError);
        }
        vm.onResourceUpload = function(files) {
            if (files && files.length) {
                for (var i = 0; i < files.length; i++) {
                    var file = files[i];
                    $upload.upload({
                            url: 'LocalizationService.ashx?method=UploadResource',
                            fields: { 'resourceset': vm.activeResource.ResourceSet, 'resourceid': vm.activeResource.ResourceId, "localeid": vm.activeResource.LocaleId },
                            file: file
                        }).progress(function(evt) {
                            var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                            vm.uploadProgress = progressPercentage + '% ' + evt.config.file.name;
                        }).success(function(data, status, headers, config) {
                            $("#AddResourceDialog").modal('hide');
                            var id = vm.activeResource.ResourceId;

                            // check if resourceId exists
                            var i = _.findIndex(vm.resourceList, function(res) {
                                return res.ResourceId === id;
                            });
                            if (i < 0)
                                vm.resourceList.unshift(vm.activeResource);

                            vm.resourceId = id;
                            vm.onResourceIdChange();

                            showMessage(vm.dbRes('ResourceSaved') +
                                " [" + vm.activeResource.ResourceId +
                                "(" + (vm.activeResource.localeId ? vm.activeResource.localeId : "invariant") + ")]");
                            vm.uploadProgress = null;
                        })
                        .error(function() {
                            parseError(arguments);
                            vm.uploadProgress = null;
                        });
                }
            }
        };
        vm.onDeleteResourceClick = function() {
            var id = vm.activeResource.ResourceId;

            if (!confirm(
                id +
                "\n\n" +
                vm.dbRes('AreYouSureYouWantToDeleteThisResource')))
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

                    showMessage(String.format(vm.dbRes('ResourceDeleted'), id));
                })
                .error(function() {
                    showMessage(String.Format(vm.dbRes('ResourceNotDeleted'), id));
                });
        };
        vm.onRenameResourceClick = function() {
            vm.newResourceId = null;
            $("#RenameResourceDialog").modal();
            $timeout(function() {
                $("#NewResourceId").focus();
            }, 1000);
        };
        vm.onRenameResourceDialogClick = function() {
            localizationService.renameResource(vm.activeResource.ResourceId, vm.newResourceId, vm.activeResource.ResourceSet)
                .success(function() {
                    for (var i = 0; i < vm.resourceList.length; i++) {
                        var res = vm.resourceList[i];
                        if (res.ResourceId == vm.activeResource.ResourceId) {
                            vm.resourceList[i].ResourceId = vm.newResourceId;
                            break;
                        }
                    }
                    vm.activeResource.ResourceId = vm.newResourceId;
                    showMessage(String.format(vm.dbRes('ResourceSetWasRenamedTo,vm.newResourceId')));
                    $("#RenameResourceDialog").modal("hide");
                })
                .error(parseError);
        }

        vm.onDeleteResourceSetClick = function() {
            if (!confirm(vm.dbRes('YouAreAboutToDeleteThisResourceSet') + ":\n\n     " +
                vm.resourceSet + "\n\n" +
                vm.dbRes('AreYouSureYouWantToDoThis')))
                return;

            localizationService.deleteResourceSet(vm.resourceSet)
                .success(function() {
                    vm.getResourceSets();
                    showMessage(vm.resoures.ResourceSetDeleted);
                    vm.resourceSet = vm.resourceSets[0];
                    vm.onResourceSetChange();
                })
                .error(parseError);
        }

        vm.onRenameResourceSetClick = function() {
            var newResourceSet = prompt(String.format(vm.dbRes('RenameResourceSetTo'), vm.resourceSet), "");
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
                    showMessage(vm.dbRes('ResourceSetRenamed'));
                })
                .error(parseError);
        };
        vm.onGridMenuClick = function () {
            var resourceSet = vm.resourceSet;
            localizationService.getResourceGridItems(resourceSet)
                .success(function (resources) {
                    vm.resourceGridResources = resources;
                });
        
            $("#ResourceGrid").show();
        };
        vm.saveGridResource = function(resource) {
            localizationService.updateResource(resource)
                .success(function() { 
                    showMessage(vm.dbRes('ResourceSaved') +
                     " [" + resource.ResourceId + 
                     " (" + (resource.LocaleId ? resource.LocaleId : "invariant") + ")]");                     
                })
                .error(parseError);
        };
        vm.onGridClose = function() {
            // refresh resource display in case we change the value 
            // of the active item
            vm.onResourceIdChange();

            $("#ResourceGrid").hide();

            // release resources and clear bindings
            vm.resourceGridResources = null;            
        };
        vm.onReloadResourcesClick = function() {
            localizationService.reloadResources()
                .success(function() {
                    showMessage(vm.dbRes('ResourcesHaveBeenReloaded'));
                })
                .error(parseError);
        };
        vm.onBackupClick = function() {
            localizationService.backup()
                .success(function() {
                    showMessage(vm.dbRes('ResourcesHaveBeenBackedUp'));
                })
                .error(parseError);
        };
        vm.onCreateTableClick = function() {
            localizationService.createTable()
                .success(function() {
                    vm.getResourceSets();
                    showMessage(vm.dbRes('LocalizationTableHasBeenCreated'));
                })
                .error(parseError);
        };
        vm.showResourceIcons = function() {
            vm.resourceEditMode = !vm.resourceEditMode;
            if (vm.resourceEditMode)
                ww.resourceEditor.showResourceIcons({
                    adminUrl: "./",
                    editorWindowOpenOptions: "height=600, width=900, left=30, top=30"
                });
            else
                ww.resourceEditor.removeResourceIcons();
        };


        vm.showMessage = showMessage;
        vm.parseError = parseError;

        function parseError(args) {
            var err = ww.angular.parseHttpError(args || arguments);
            showMessage(err.message, "warning", "warning");
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

        function parseQueryString() {
            var query = window.location.search;
            var res = {
                isEmpty: !query,
                query: query,
                resourceId: getUrlEncodedKey("ResourceId", query),
                resourceSet: getUrlEncodedKey("ResourceSet", query),
                content: getUrlEncodedKey("Content", query)
            }

            return res;
        }

        function selectResourceSet(query) {
            if (!query.resourceSet)
                return;

            for (var i = 0; i < vm.resourceSets.length; i++) {
                if (vm.resourceSets[i] == query.resourceSet) {
                    vm.resourceSet = vm.resourceSets[i];
                    selectResourceIdWithQuery(query);
                    break;
                }
            }

            function selectResourceIdWithQuery(query) {
                vm.getResourceList()
                    .success(function() {
                        var found = false;
                        for (var i = 0; i < vm.resourceList.length; i++) {
                            if (vm.resourceList[i].ResourceId === query.resourceId) {
                                vm.resourceId = vm.resourceList[i].ResourceId;
                                vm.onResourceIdChange();
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                            $timeout(function() {
                                vm.onAddResourceClick(query.resourceId, query.resourceSet, query.content);
                            }, 100);
                    });
            }

        }

        function selectResourceId(resourceId) {

            for (var i = 0; i < vm.resourceList.length; i++) {
                if (resourceSet.ResourceId == resourceSetId) {
                    vm.resourceSet == resourceSet;
                    break;
                }
            }
        }


        $(document.body).keydown(function(ev) {
            if (ev.keyCode == 76 && ev.altKey) {
                $("#ResourceIdList").focus();
            }
        });


        // initialize
        vm.getResourceSets()
            .success(function() {
                var query = parseQueryString();
                if (query.isEmpty) {
                    // just load resource sets
                    vm.getResourceSets();
                    return;
                }                
                selectResourceSet(query);
            });
    }
})();

