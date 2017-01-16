'use strict'

var stc = tradetwitsApp.controller('SearchController', ['$scope', '$window', 'SearchService', function ($scope, $window, SearchService) {
    $scope.getStocks = function (val) {
        return SearchService.getSearchResults(val).$promise
            .then(function (res) {
                var stocks = res;
                return stocks;
            });
    };

    $scope.redirectToItem = function () {
        $window.location.href = '/' + $scope.selectedItem;
    }

    $scope.onSelect = function (item, model, label) {
        $window.location.href = '/' + $scope.selectedItem;
    };
}]
);