(function () {
    'use strict';

    angular
        .module('app')
        .controller('translateController', translateController);

    translateController.$inject = ['$scope','$http', '$timeout', 'localizationService']; 


    function translateController($scope, $http, $timeout) {
        /* jshint validthis:true */          
        var vm = this;

        vm.resources = resources;
        vm.dbRes = resources.dbRes;

        vm.title = 'translateController';
        vm.invariantLanguage = "en";

        vm.inputText = "";
        vm.googleTranslation = "";
        vm.bingTranslation = "";
        vm.fromLang = "en";
        vm.toLang = "de";
        vm.callingId = null;
        vm.resource = null;
        vm.baseUrl = "../api/LocalizationAdministration/";

        vm.error = {
            message: null,
            icon: "info-circle",
            cssClass: "info"
        };

        $scope.$root.$on("startTranslate", function (e, resource, id) {
            // emitted by Resource controller
            vm.resource = resource;            
            vm.fromLang = resource.LocaleId;

            if (vm.fromLang === "")
                vm.fromLang = "en";
            vm.inputText = resource.Value;
            vm.callingId = id;

            vm.googleTranslation = null;
            vm.bingTranslation = null;
        });

        vm.onTranslateClick = function () {            
            vm.translate("google");
            vm.translate("bing");
        };


       vm.useTranslateClick = function (type) {
            var lang = vm.toLang;  // language to update

            // treat default language language as invariant
            if (lang === vm.invariantLanguage)
                lang = ""; // invariant

            var val = vm.googleTranslation;
            if (type == "bing") 
                val = vm.bingTranslation;
            
            // notify caller
            $scope.$root.$emit("translateComplete", lang, val);
            $("#TranslateDialog").modal("hide");
        };

        vm.translate = function (type) {
            var data = {
                from: vm.fromLang,
                to: vm.toLang,
                text: vm.inputText,
                service: type
            };
            
            $http.post(vm.baseUrl + "Translate", data)
                .success(function (result) {
                    if (type == "google")
                        vm.googleTranslation = result;
                    else
                        vm.bingTranslation = result;
                })
                .error(function() {
                    var err = ww.angular.parseHttpError(arguments);
                    alert(err.message);
            });

        };
    }
})();
