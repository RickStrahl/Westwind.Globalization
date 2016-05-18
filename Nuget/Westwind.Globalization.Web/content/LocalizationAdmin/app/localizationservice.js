(function() {
    //'use strict';

    angular
        .module('app')
        .factory('localizationService', localizationService);

    localizationService.$inject = ['$http', '$q', '$timeout'];

    function localizationService($http, $q, $timeout) {
        var service = {
            error: null,
            baseUrl: "./",
            getResourceList: getResourceList,
            resourceList: [],
            getResourceItems: getResourceItems,
            resourceItems: [],
            getResourceGridItems: getResourceGridItems,
            resourceId: null,
            getResourceSets: getResourceSets,
            resourceSets: [],
            getAllLocaleIds: getAllLocaleIds,
            localeIds: [],
            resourceStrings: [],
            localizationInfo: null,
            getResourceStrings: getResourceStrings,
            updateResourceString: updateResourceString,
            updateResource: updateResource,
            updateComment: updateComment,
            deleteResource: deleteResource,
            renameResource: renameResource,
            deleteResourceSet: deleteResourceSet,
            renameResourceSet: renameResourceSet,
            reloadResources: reloadResources,
            isRtl: isRtl,
            backup: backup,
            createTable: createTable,
            createClass: createClass,
            exportResxResources: exportResxResources,
            importResxResources: importResxResources,
            getLocalizationInfo: getLocalizationInfo,
            isLocalizationTable: isLocalizationTable
        };
        return service;

        function getResourceList(resourceSet) {            
            return $http.get("localizationService.ashx?method=GetResourceListHtml&ResourceSet=" + resourceSet)
                .success(function(resourceList) {
                    service.resourceList = resourceList;
                })
                .error(parseHttpError);
        }

        function getResourceSets() {
            return $http.get("localizationService.ashx?method=GetResourceSets")
                .success(function(resourceSets) {
                    service.resourceSets = resourceSets;
                })
                .error(parseHttpError);
        }

        function isLocalizationTable() {
            return $http.get("localizationService.ashx?method=IsLocalizationTable");
        }

        function getAllLocaleIds(resourceSet) {
            return $http.get("localizationService.ashx?method=GetAllLocaleIds&ResourceSet=" + resourceSet)
                .success(function(localeIds) {
                    service.localeIds = localeIds;
                })
                .error(parseHttpError);
        }

        function getResourceItems(resourceId, resourceSet) {
            return $http.post("localizationService.ashx?method=GetResourceItems",
                {
                    ResourceId: resourceId,
                    ResourceSet: resourceSet                    
                })
                .success(function (resourceItems) {
                    service.resourceItems = resourceItems;
                })
                .error(parseHttpError);
        }

        function getResourceGridItems(resourceSet) {
            return $http.get("localizationService.ashx?method=GetAllResourcesForResourceGrid&resourceSet=" + resourceSet)
                .error(parseHttpError);
        }


        function getResourceItem(resourceId, resourceSet, lang) {
            return $http.post("localizationService.ashx?method=GetResourceItem",
                {
                    ResourceId: resourceId,
                    ResourceSet: resourceSet,
                    CultureName: lang
                })
                .success(function(resource) {
                    service.resourceItem = resource;
                })
                .error(parseHttpError);
        }

        function getResourceStrings(resourceId, resourceSet) {
            return $http.get("localizationService.ashx?method=GetResourceStrings&ResourceId=" + resourceId + "&ResourceSet=" + resourceSet)
                .success(function (resourceStrings) {
                    service.resourceStrings = resourceStrings;
                })
                .error(parseHttpError);
        }

        // adds or updates a resource
        function updateResource(resource) {
            return $http.post("localizationService.ashx?method=UpdateResource", resource)
                .error(parseHttpError);
        }

        

        function updateResourceString(value, resourceId, resourceSet, localeId,comment) {
            var parm = {
                value: value,
                resourceId: resourceId,
                resourceSet: resourceSet,
                localeId: localeId,
                comment: comment
            };

            return $http.post("localizationService.ashx?method=UpdateResourceString", parm)
                .error(parseHttpError);
        }

        function updateComment(comment, resourceId, resourceSet, localeId) {
            var parm = {                
                resourceId: resourceId,
                resourceSet: resourceSet,
                localeId: localeId,
                comment: comment
            };

            return $http.post("localizationService.ashx?method=UpdateComment", parm)
                .error(parseHttpError);
        }

        function deleteResource(resourceId, resourceSet, localeId) {
            var parm = {                
                resourceId: resourceId,
                resourceSet: resourceSet,
                localeId: localeId
            };

            return $http.post("localizationService.ashx?method=DeleteResource", parm)
                .error(parseHttpError);
        }

        function renameResource(resourceId, newResourceId, resourceSet) {
            var parm = {
                resourceId: resourceId,
                newResourceId: newResourceId,
                resourceSet: resourceSet                
            };

            return $http.post("localizationService.ashx?method=RenameResource", parm)
                .error(parseHttpError);
        }

        function deleteResourceSet(resourceSet) {
           return $http.get("localizationService.ashx?method=DeleteResourceSet&ResourceSet=" + resourceSet)
                .error(parseHttpError);
        }
        function renameResourceSet(oldResourceSet, newResourceSet) {
            return $http.get("localizationService.ashx?method=RenameResourceSet&oldResourceSet=" + oldResourceSet + "&newResourceSet=" + newResourceSet)
                 .error(parseHttpError);
        }
        function reloadResources() {
            return $http.get("localizationService.ashx?method=ReloadResources")
                 .error(parseHttpError);
        }
        function backup() {
            return $http.get("localizationService.ashx?method=Backup")
                 .error(parseHttpError);
        }
        function createTable() {
            return $http.get("localizationService.ashx?method=CreateTable")
                 .error(parseHttpError);
        }
        function createClass(file, namespace, resourceSets,classType) {
            return $http.post("localizationService.ashx?method=CreateClass",
            { fileName: file, namespace: namespace, resourceSets: resourceSets, classType: classType || "DbRes" })
                    .error(parseHttpError);
        }
        function exportResxResources(path, resourceSets) {
            path = path || "";
            return $http.post("localizationService.ashx?method=ExportResxResources",{  outputBasePath: path, resourceSets: resourceSets})
                    .error(parseHttpError);
        }
        function importResxResources(path) {
            path = path || "";
            return $http.get("localizationService.ashx?method=ImportResxResources&inputBasePath=" + encodeURIComponent(path))
                    .error(parseHttpError);
        }
        function getLocalizationInfo() {
            // cache
            if (service.localizationInfo)
                return ww.angular.$httpPromiseFromValue($q,service.localizationInfo);
            
            return $http.get("localizationService.ashx?method=GetLocalizationInfo")
                .success(function(info) {
                    service.localizationInfo = info;
                })
                .error(parseHttpError);            
        }
        function isRtl(localeId) {
            return $http.get("localizationService.ashx?method=IsRtl&localeId=" + localeId)
                .error(parseHttpError);
        }
        function parseHttpError() {
            service.error = ww.angular.parseHttpError(arguments);
        }
    }
})();
    