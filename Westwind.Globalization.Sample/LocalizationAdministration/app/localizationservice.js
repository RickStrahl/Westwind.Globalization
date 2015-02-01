(function() {
    //'use strict';

    angular
        .module('app')
        .factory('localizationService', localizationService);

    localizationService.$inject = ['$http', '$q'];

    function localizationService($http, $q) {

        var service = {
            error: null,
            baseUrl: "./",
            getResourceList: getResourceList,
            resourceList: [],
            getResourceItems: getResourceItems,
            resourceItems: [],
            resourceId: null,
            getResourceSets: getResourceSets,
            resourceSets: [],
            getAllLocaleIds: getAllLocaleIds,
            localeIds: [],
            resourceStrings: [],
            getResourceStrings: getResourceStrings,
            updateResourceString: updateResourceString,
            updateResource: updateResource
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

        function updateResourceString(value, resourceId, resourceSet, localeId) {
            var parm = {
                value: value,
                resourceId: resourceId,
                resourceSet: resourceSet,
                localeId: localeId
            };

            return $http.post("localizationService.ashx?method=UpdateResourceString", parm)
                .error(parseHttpError);
        }

        function parseHttpError() {
            service.error = ww.angular.parseHttpError(arguments);
        }


    }
})();
    