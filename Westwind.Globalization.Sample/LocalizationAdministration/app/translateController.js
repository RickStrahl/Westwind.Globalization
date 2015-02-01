(function () {
    'use strict';

    angular
        .module('app')
        .controller('translateController', translateController);

    translateController.$inject = ['$scope','$http', '$timeout']; 


    function translateController($scope, $http, $timeout) {
        /* jshint validthis:true */          
        var vm = this;
        vm.title = 'translateController';
        vm.invariantLanguage = "en";

        vm.inputText = "";
        vm.googleTranslation = "";
        vm.bingTranslation = "";
        vm.fromLang = "en";
        vm.toLang = "de";
        vm.callingId = null;
        vm.resource = null;

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
            vm.googleTranslate();
            vm.bingTranslate();
        };

        vm.useGoogleClick = function () {
            var lang = vm.toLang;  // language to update

            // treat default language language as invariant
            if (lang === vm.invariantLanguage)
                lang = ""; // invariant

            //var resourceList = $scope.$parent.view.resourceItems;
            //// treat invariant language as invariant entry
            
            //// find index to update
            //var idx = _.findIndex(resourceList, function(res) {                
            //    return res.LocaleId == lang;
            //});
            
            //// no match - nothing we can do...
            //if (idx == -1) 
            //    // TODO: Add new resource
            //    return;
            
            //$timeout(function() {                
            //    $scope.$parent.view.resourceItems[idx].Value = vm.googleTranslation;
            //    $scope.$parent.$apply();
            //},100);            
            
            // explicitly update the element so we can show changed status
            var $el = $("#ResourceList textarea[data-localeId=" + lang + "]" );
            if ($el.length < 1)
                // TODO: Add new resource
                return; 

            ww.angular.applyBindingValue($el, vm.googleTranslation, $timeout);

            
            $("#TranslateDialog").modal("hide");
        };

        vm.googleTranslate = function () {
            var data = {
                from: vm.fromLang,
                to: vm.toLang,
                text: vm.inputText,
                service: "google"
            };
            $http.post("localizationService.ashx?method=Translate", data)
                .success(function(result) {
                    vm.googleTranslation = result;
                })
                .error(function() {
                    var err = ww.angular.parseHttpError(arguments);
                    alert(err.message);
            });

        };
        vm.bingTranslate = function() {

        };
    }
})();
