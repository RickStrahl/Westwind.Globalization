/**
 * Created by Rick on 1/28/2015.
 */

(function () {
    'use strict';

    var app = angular
        .module('app')
        .controller('resourceController', resourceController);

   resourceController.$inject = [ 'localizationService'];

    function resourceController(localizationService) {
        var vm = this;

    }
})();

