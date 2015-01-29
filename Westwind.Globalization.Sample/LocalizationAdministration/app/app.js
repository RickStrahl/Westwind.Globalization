(function() {
    'use strict';

    var app = angular.module('app', [
        // Angular modules 
        // ngAnimate',
        //'ngRoute',
        'ngSanitize'        


        // Custom modules 

        // 3rd Party Modules
        
    ]);

    // config settings
    app.configuration = {
    };

    app.config([
            function() {
                console.log('app.coifig');
            }
        ])
        .filter('linebreakFilter', function () {        
            return function(text) {
                if (text !== undefined)
                    return text.replace(/\n/g, '<br />');
                return text;
            };
        });
    
})();