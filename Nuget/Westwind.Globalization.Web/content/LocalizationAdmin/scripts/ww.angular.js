/// <reference path="jquery.js" />
/// <reference path="ww.jquery.js" />
/*
ww.jQuery.js  
Version 1.11 - 1/2/2014
West Wind jQuery plug-ins and utilities

(c) 2008-2014 Rick Strahl, West Wind Technologies 
www.west-wind.com

Licensed under MIT License
http://en.wikipedia.org/wiki/MIT_License
*/
(function(undefined) {
    ww = {};
    var self;
    ww.angular = {
        parseHttpError: function (args) {
            // error/message object passed
            if (args.hasOwnProperty("message"))
                return args;
            if (args.hasOwnProperty("Message")) {
                args.message = args.Message;
                return args;
            }

            var data = args[0];
            var status = args[1];
            var msg = args[2];
            var errorMsg = "";
            if (data) 
            {                      
                if(data.hasOwnProperty("message"))
                   return data;
                if (msg.hasOwnProperty("Message")) {
                    data.message = data.Message;
                    return data;
                }

                // assume JSON   
                try {
                    var msg = JSON.parse(data);

                    if (msg && msg.hasOwnProperty("message") || msg.hasOwnProperty("Message"))
                        return msg;
                } catch (exception) {
                    return new CallbackException("Unknown error.");
                }
            }

            return new CallbackException(errorMsg);
        },
        // extends deferred with $http compatible .success and .error functions
        $httpDeferredExtender: function(deferred) {
            deferred.promise.success = function(fn) {
                deferred.promise.then(fn, null);
                return deferred.promise;
            }
            deferred.promise.error = function(fn) {
                deferred.promise.then(null, fn);
                return deferred.promise;
            }
            return deferred;
        },
        // creates a resolved/rejected promise from a value
        $httpPromiseFromValue: function($q, val, reject) {
            var def = $q.defer();
            if (reject)
                def.reject(val);
            else
                def.resolve(val);
            self.$httpDeferredExtender(def);
            return def.promise;
        },
        promiseFrom$http: function($q, $http) {
            var d = $q.defer();
            d.then($http.success, $http.error);
            return d.promise;
        },
        // Assigns a value to DOM element explicitly and forces
        // an Angular binding to be updated. Use this function
        // when you need to update values 'manually' via DOM
        // manipulation
        // element parameter can be DOM or jq element
        applyBindingValue: function(element, value) {                                    
            // must use timeout to avoid recursive $apply errors
            setTimeout(function () {                
                var el = angular.element(element);
                el.scope().$apply(function () {
                    el.val(value); // force value to be set                    
                    el.controller('ngModel').$setViewValue(value);                    
                });
            });
        }

    };
    self = ww.angular;
})();