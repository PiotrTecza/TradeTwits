'use strict'

tradetwitsApp.factory('SearchService', ['$resource',
    function ($resource) {
        return {
            get: function (name) {
                return $resource('/api/Search?q=' + name).query();
            },
            getUsers: function (name) {
                return $resource('/api/Search?q=' + name + '&$filter=IsUser eq true').query();
            },
            getStocks: function (name) {
                return $resource('/api/Search?q=' + name + '&$filter=IsUser eq false').query();
            },
            getSearchResults: function (name) {
                return $resource('/api/Search/GetSearchResults?name=' + name).query();
            },
            getFollowedUsers: function (userName, lastName) {
                return $resource('/api/Search/GetFollowedUsers?id=' + userName + '&lastName=' + lastName).get();
            },
            getFollowedStocks: function (userName, lastName) {
                return $resource('/api/Search/GetFollowedStocks?id=' + userName + '&lastName=' + lastName).get();
            },
            getFollowers: function (userName, lastName) {
                return $resource('/api/Search/GetFollowers?id=' + userName + '&lastName=' + lastName).get();
            }
        }
    }]
);
