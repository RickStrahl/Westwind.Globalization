(function() {
    'use strict';

    var app = angular.module('app', [
        // Angular modules 
        // ngAnimate',
        //'ngRoute',
        'ngSanitize'
        // Custom modules 

        // 3rd Party Modules    
        ,'angularFileUpload'
    ]);

    // config settings
    app.configuration = {
    
    };

    app.config([
        function () {
            $(window).resize(resizeControls);            
            setTimeout(resizeControls, 90);
        }])
        .filter('linebreakFilter', function() {
            return function(text) {
                if (text !== undefined)
                    return text.replace(/\n/g, '<br />');
                return text;
            };
        });

        app.directive('convertToNumber', function() {
            return {
                require: 'ngModel',
                link: function (scope, element, attrs, ngModel) {                
                    ngModel.$parsers.push(function(val) {                    
                        return parseInt(val, 10);
                    });
                    ngModel.$formatters.push(function (val) {                    
                        return '' + val;
                    });
                }
            };
        });

    function resizeControls() {
        var cHeight = $(window).height() - $(".banner").outerHeight() - $(".menubar").outerHeight();
        console.log(cHeight);
        $("#ContentContainer").height(cHeight);
    }

})();