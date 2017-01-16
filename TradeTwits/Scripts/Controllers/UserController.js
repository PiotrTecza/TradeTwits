'use strict'

tradetwitsApp.controller('UserController', ['$scope', 'UsersService', 'userDetailesData', 'TwitService', 'SearchService'
    , function ($scope, UsersService, userDetailesData, TwitService, SearchService) {
        $scope.isFollowed = userDetailesData.isFollowed;
        $scope.isBlocked = userDetailesData.isBlocked;
        $scope.userName = userDetailesData.userName;
        $scope.twitsCount = userDetailesData.twitsCount;
        $scope.followersCount = userDetailesData.followersCount;
        $scope.followedUsersCount = userDetailesData.followedUsersCount;
        $scope.followedStocksCount = userDetailesData.followedStocksCount;
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
            $scope.followedUsers = [];
            $scope.followedStocks = [];
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

        $scope.changeBlocked = function () {
            if ($scope.isBlocked) {
                UsersService.removeBlocked($scope.userName);
                $scope.isBlocked = false;
            }
            else {
                UsersService.addBlocked($scope.userName);
                $scope.isBlocked = true;
            }
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

        var loadFollowedUsers = function () {
            var lastName = ($scope.followedUsers === undefined || $scope.followedUsers.length === 0) ? "" : $scope.followedUsers[$scope.followedUsers.length - 1].UserName
            var nextFollowed = SearchService.getFollowedUsers($scope.userName, lastName).$promise.then(loadCallback('followedUsers', 'followedUsersCount'));
        }

        var loadFollowedStocks = function () {
            var lastName = ($scope.followedStocks === undefined || $scope.followedStocks.length === 0) ? "" : $scope.followedStocks[$scope.followedStocks.length - 1].UserName
            var nextFollowed = SearchService.getFollowedStocks($scope.userName, lastName).$promise.then(loadCallback('followedStocks', 'followedStocksCount'));
        }

        var loadFollowers = function () {
            var lastName = ($scope.followers === undefined || $scope.followers.length === 0) ? "" : $scope.followers[$scope.followers.length - 1].UserName
            var nextFollower = SearchService.getFollowers($scope.userName, lastName).$promise.then(loadCallback('followers', 'followersCount'));
        }

        var tabsDataLoaders = [loadTwits, loadFollowedUsers, loadFollowedStocks, loadFollowers];
        $scope.twits = loadTwits();

    }]
);