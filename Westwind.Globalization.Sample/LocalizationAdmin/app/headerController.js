(function () {
    'use strict';

    var app = angular
        .module('app')
        .controller('headerController', headerController);

   headerController.$inject = [ 'localizationService'];

    function headerController(localizationService) {
        var vm = this;


    }
})();

