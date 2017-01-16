'use strict'

var tradetwitsApp = angular.module('tradetwitsApp', ['ngResource', 'ui.bootstrap', 'angularFileUpload']);
tradetwitsApp.run(['$rootScope', 'SearchService', function ($rootScope, SearchService) {
    $rootScope.parseDate = function (dateToParse) {
        return moment(dateToParse).format('YYYY-MM-DD HH:mm:ss');
    };

    $rootScope.autocompleteVisible = false;
    $rootScope.textcomplete = function (target) {
        $(target).textcomplete([
       {
           match: /\B@([\-+\w]*)$/,
           search: function (term, callback) {
               if (term === "") callback([]);
               else {
                   SearchService.getUsers(term).$promise
                        .then(function (res) {
                            var stocks = [];
                            angular.forEach(res, function (item) {
                                stocks.push({
                                    username: item.UserName,
                                    fullname: item.FullName
                                });
                            });
                            callback(stocks);
                        });
               }
           },
           index: 1,
           template: function (item) {
               return item.username;
           },
           replace: function (item) {
               return '@' + item.username + ' ';
           }
       },
       {
           match: /\B\$([\-+\w]*)$/,
           search: function (term, callback) {
               if (term === "") callback([]);
               else {
                   SearchService.getStocks(term).$promise
                        .then(function (res) {
                            var stocks = [];
                            angular.forEach(res, function (item) {
                                stocks.push({
                                    username: item.UserName,
                                    fullname: item.FullName
                                });
                            });
                            callback(stocks);
                        });
               }
           },
           index: 1,
           template: function (item) {
               return item.username + '(' + item.fullname + ')';
           },
           replace: function (item) {
               return '$' + item.username + ' ';
           }
       }
        ]).on({
            'textComplete:show': function (e) {
                $rootScope.autocompleteVisible = true;
            },
            'textComplete:hide': function (e) {
                $rootScope.autocompleteVisible = false;
            }
        });
    }


}])

$(function () {
    $.connection.hub.logging = true;
    window.hubReady = $.connection.hub.start();
    window.hubReady.done(function () {
        console.log('Now connected, connection ID=' + $.connection.hub.id);
    }).fail(function () { console.log('Could not connect'); });
});

$.connection.hub.error(function (err) {
    console.log("SignalR error : " + err);
});

tradetwitsApp.value('twitHub', $.connection.twitHub);

tradetwitsApp.directive('ngEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    scope.$eval(attrs.ngEnter);
                });

                event.preventDefault();
            }
        });
    };
});

tradetwitsApp.directive('removeBlocked', ['UsersService',function (UsersService) {
    return function (scope, element, attrs) {
        element.bind("click", function (event) {
            UsersService.removeBlocked(attrs.removeBlocked);
            var target = element.closest('.blockedUser');
            target.animate({height:'0px'},'slow', function () { target.remove(); });
        });
    };
}]);

tradetwitsApp.directive('ngAutocomplete', ['$parse',function ($parse) {
    return function (scope, element, attrs) {
        var invoker = $parse(attrs.ngAutocomplete);
        invoker(scope, { target: element });
    };
}]);


var uri_pattern = /\b((?:[a-z][\w-]+:(?:\/{1,3}|[a-z0-9%])|www\d{0,3}[.]|[a-z0-9.\-]+[.][a-z]{2,4}\/)(?:[^\s()<>]+|\(([^\s()<>]+|(\([^\s()<>]+\)))*\))+(?:\(([^\s()<>]+|(\([^\s()<>]+\)))*\)|[^\s`!()\[{};:'".,<>?«»“”‘’]|\]|\?))/ig;
tradetwitsApp.directive('twitInput' ,function () {
    function link(scope, element, attrs) {
        var innerHTML = scope.$eval(attrs.twitInput);
        innerHTML = innerHTML.replace(/@([a-zA-Z0-9_]+)/g, '<a href="/$1">@$1</a>');
        innerHTML = innerHTML.replace(/\$([a-zA-Z0-9_]+)/g, '<a href="/$1">$$$1</a>');
        innerHTML = innerHTML.replace(uri_pattern, '<a href="$1" target="_blank">$1</a>');
        element.html(innerHTML);
    };

    return {
        link: link,
    };
});

