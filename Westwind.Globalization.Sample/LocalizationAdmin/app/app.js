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
            function() {
                resizeControls();
                $(window).resize(resizeControls);
            }
        ])
        .filter('linebreakFilter', function() {
            return function(text) {
                if (text !== undefined)
                    return text.replace(/\n/g, '<br />');
                return text;
            };
        });

    function resizeControls() {
        var cHeight = $(window).height() - $(".banner").outerHeight() - $(".menubar").outerHeight();
        console.log(cHeight);
        $("#ContentContainer").height(cHeight);
    }

})();