(function () {
    'use strict';

    angular
        .module('app')
        .controller('translateController', translateController);

    translateController.$inject = ['$scope','$http', '$timeout']; 


    function translateController($scope, $http, $timeout) {
        /* jshint validthis:true */
        var pvm = $scope.$parent.view;        
        var vm = this;
        vm.title = 'translateController';

        vm.inputText = "";
        vm.googleTranslation = "";
        vm.bingTranslation = "";
        vm.fromLang = "en";
        vm.toLang = "de";
        vm.callingId = null;
        vm.callingResource = null;

        vm.error = {
            message: null,
            icon: "info-circle",
            cssClass: "info"
        };

        $scope.$root.$on("startTranslate", function (e, resource, id) {
            vm.callingResource = resource;            
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

        vm.useGoogleClick = function() {
            var $el = $("#ResourceList textarea[data-localeId=" + vm.toLang + "]" );
            if ($el.length < 1)
                $el = $("#ResourceList textarea[data-localeId='']");

            ww.angular.applyBindingValue($el, vm.googleTranslation, $timeout);

            //var el = angular.element($el);
            //el.val(vm.googleTranslation);

            //$timeout(function() {
            //    el.scope().$apply(function(){
            //        el.val(vm.googleTranslation);                    
            //      el.controller('ngModel').$setViewValue(el.val());
            //    });
            //});

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
