(function () {
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
            getProjectNames: getProjectNames,
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
            deleteProject: deleteProject,
            renameResourceSet: renameResourceSet,
            renameProjectName: renameProjectName,
            reloadResources: reloadResources,
            isRtl: isRtl,
            backup: backup,
            createTable: createTable,
            createClass: createClass,
            exportResxResources: exportResxResources,
            importResxResources: importResxResources,
            getLocalizationInfo: getLocalizationInfo,
            isLocalizationTable: isLocalizationTable,
            projectNames: [],
            projectName: null
        };
        return service;

        function getResourceList(resourceSet, projectName) {
            return $http.get("localizationService.ashx?method=GetResourceListHtml&ResourceSet=" + resourceSet + (projectName != null ? "&projectName=" + projectName : ""))
                .success(function (resourceList) {
                    service.resourceList = resourceList;
                })
                .error(parseHttpError);
        }

        function getResourceSets(projectName) {
            return $http.get("localizationService.ashx?method=GetResourceSets" + (projectName != null ? "&projectName=" + projectName : ""))
                .success(function (resourceSets) {
                    service.resourceSets = resourceSets;
                })
                .error(parseHttpError);
        }

        function getProjectNames() {
            return $http.get("localizationService.ashx?method=GetProjectNames")
                .success(function (projectNames) {
                    service.projectNames = projectNames;
                })
                .error(parseHttpError);
        }

        function isLocalizationTable() {
            return $http.get("localizationService.ashx?method=IsLocalizationTable");
        }

        function getAllLocaleIds(resourceSet) {
            return $http.get("localizationService.ashx?method=GetAllLocaleIds&ResourceSet=" + resourceSet)
                .success(function (localeIds) {
                    service.localeIds = localeIds;
                })
                .error(parseHttpError);
        }

        function getResourceItems(resourceId, resourceSet, projectName) {
            return $http.post("localizationService.ashx?method=GetResourceItems",
                {
                    ResourceId: resourceId,
                    ResourceSet: resourceSet,
                    projectName: projectName
                })
                .success(function (resourceItems) {
                    service.resourceItems = resourceItems;
                })
                .error(parseHttpError);
        }

        function getResourceGridItems(resourceSet, projectName) {
            debugger
            return $http.get("localizationService.ashx?method=GetAllResourcesForResourceGrid&resourceSet=" + resourceSet + (projectName != null ? "&projectName=" + projectName : ""))
                .error(parseHttpError);
        }


        function getResourceItem(resourceId, resourceSet, lang) {
            return $http.post("localizationService.ashx?method=GetResourceItem",
                {
                    ResourceId: resourceId,
                    ResourceSet: resourceSet,
                    CultureName: lang
                })
                .success(function (resource) {
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



        function updateResourceString(value, resourceId, resourceSet, localeId, comment, projectName) {
            var parm = {
                value: value,
                resourceId: resourceId,
                resourceSet: resourceSet,
                localeId: localeId,
                comment: comment,
                projectName: projectName
            };

            return $http.post("localizationService.ashx?method=UpdateResourceString", parm)
                .error(parseHttpError);
        }

        function updateComment(comment, resourceId, resourceSet, localeId, projectName) {
            var parm = {
                resourceId: resourceId,
                resourceSet: resourceSet,
                localeId: localeId,
                comment: comment,
                projectName: projectName
            };

            return $http.post("localizationService.ashx?method=UpdateComment", parm)
                .error(parseHttpError);
        }

        function deleteResource(resourceId, resourceSet, localeId, projectName) {
            var parm = {
                resourceId: resourceId,
                resourceSet: resourceSet,
                localeId: localeId,
                projectName: projectName
            };

            return $http.post("localizationService.ashx?method=DeleteResource", parm)
                .error(parseHttpError);
        }

        function renameResource(resourceId, newResourceId, resourceSet, projectName) {
            var parm = {
                resourceId: resourceId,
                newResourceId: newResourceId,
                resourceSet: resourceSet,
                projectName: projectName
            };

            return $http.post("localizationService.ashx?method=RenameResource", parm)
                .error(parseHttpError);
        }

        function deleteResourceSet(resourceSet, projectName) {
            debugger
            return $http.get("localizationService.ashx?method=DeleteResourceSet&ResourceSet=" + resourceSet + (projectName != null ? "&projectName=" + projectName : ""))
                 .error(parseHttpError);
        }

        function deleteProject(projectName) {
            return $http.get("localizationService.ashx?method=DeleteProject" + (projectName != null ? "&projectName=" + projectName : ""))
                 .error(parseHttpError);
        }
        function renameResourceSet(oldResourceSet, newResourceSet, projectName) {
            return $http.get("localizationService.ashx?method=RenameResourceSet&oldResourceSet=" + oldResourceSet + "&newResourceSet=" + newResourceSet + (projectName != null ? "&projectName=" + projectName : ""))
                 .error(parseHttpError);
        }

        function renameProjectName(oldProjectName, newProjectName) {
            return $http.get("localizationService.ashx?method=RenameProjectName&oldProjectName=" + oldProjectName + "&newProjectName=" + newProjectName)
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
        function createClass(file, namespace, resourceSets, classType, projectName) {
            return $http.post("localizationService.ashx?method=CreateClass",
            { fileName: file, namespace: namespace, resourceSets: resourceSets, classType: classType || "DbRes", projectName: projectName })
                    .error(parseHttpError);
        }
        function exportResxResources(path, resourceSets, projectName) {
            debugger
            path = path || "";
            return $http.post("localizationService.ashx?method=ExportResxResources", { outputBasePath: path, resourceSets: resourceSets, projectName: projectName })
                    .error(parseHttpError);
        }
        function importResxResources(path, projectName) {

            path = path || "";
            return $http.get("localizationService.ashx?method=ImportResxResources&inputBasePath=" + encodeURIComponent(path) + (projectName != null ? "&projectName=" + projectName : ""))
                    .error(parseHttpError);
        }
        function getLocalizationInfo() {
            // cache
            if (service.localizationInfo)
                return ww.angular.$httpPromiseFromValue($q, service.localizationInfo);

            return $http.get("localizationService.ashx?method=GetLocalizationInfo")
                .success(function (info) {
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
