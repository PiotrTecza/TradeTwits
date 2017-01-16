'use strict'

tradetwitsApp.controller('StockController', ['$scope', 'UsersService', 'StocksService', 'stockDetailsData', 'TwitService', 'SearchService'
    , function ($scope, UsersService, StocksService, stockDetailsData, TwitService, SearchService, $interval) {
        $scope.isFollowed = stockDetailsData.isFollowed;
        $scope.userName = stockDetailsData.userName;
        $scope.twitsCount = stockDetailsData.twitsCount;
        $scope.followersCount = stockDetailsData.followersCount;
        //$scope.price = stockDetailsData.price;
        //$scope.change = stockDetailsData.change;
        //$scope.changeAmount = stockDetailsData.changeAmount;
        $scope.allTLoaded = false;
        $scope.selectedTab = 0;
        var loading = false;

        $scope.selectTab = function (tab) {
            $scope.allLoaded = true;
            loading = false;
            $scope.selectedTab = tab;
            clearCollections();
            tabsDataLoaders[$scope.selectedTab]();
        }

        var clearCollections = function () {
            $scope.twits = [];
            $scope.followers = [];
        }

        $scope.changeFollowed = function () {
            if ($scope.isFollowed) {
                UsersService.removeFollowed($scope.userName);
                $scope.isFollowed = false;
            }
            else {
                UsersService.addFollowed($scope.userName);
                $scope.isFollowed = true;
            }
        }

        var stocksData = function () {
            StocksService.getStockData($scope.userName).done(function (stocks) {
                $scope.price = stocks.Price;
                $scope.change = stocks.Change;
                $scope.changeAmount = stocks.ChangeAmount;
            });
        }

        $scope.loadNext = function () {
            if (loading)
                return;
            loading = true;
            tabsDataLoaders[$scope.selectedTab]();
        };

        var loadCallback = function (collection, counter) {
            return function (result) {

                if ($scope[collection] === undefined)
                    $scope[collection] = result.data;
                else
                    $scope[collection] = $scope[collection].concat(result.data);

                $scope.allLoaded = result.count == $scope[collection].length;
                $scope[counter] = result.count;
                loading = false;
            }
        }

        var loadTwits = function () {
            var lastTwitDate = ($scope.twits === undefined || $scope.twits.length === 0) ? "" : $scope.twits[$scope.twits.length - 1].CreatedAt;
            var nextTwits = TwitService.getForUser($scope.userName, lastTwitDate).$promise.then(loadCallback('twits', 'twitsCount'));
        }

        var loadFollowers = function () {
            var lastName = ($scope.followers === undefined || $scope.followers.length === 0) ? "" : $scope.followers[$scope.followers.length - 1].UserName
            var nextFollower = SearchService.getFollowers($scope.userName, lastName).$promise.then(loadCallback('followers', 'followersCount'));
        }

        var tabsDataLoaders = [loadTwits, loadFollowers];
        $scope.twits = loadTwits();
        angular.element(document).ready(function () {
            window.hubReady.done(function () { stocksData() });
        });
    }]
);