'use strict'

tradetwitsApp.controller('TwitController', ['$scope', 'TwitService', 'twitHub', 'userName'
    , function ($scope, TwitService, twitHub, userName) {
        $scope.loggedInUserName = userName;
        $scope.likeTwit = function (twit) {
            twit.IsLiked = !twit.IsLiked;
            TwitService.vote(twit.Id, twit.IsLiked, null);
            if (twit.IsLiked)
                twit.LikesCount++;
            else
                twit.LikesCount--;
        };

        $scope.likeComment = function (comment, twitId) {
            comment.IsLiked = !comment.IsLiked;
            TwitService.vote(twitId, comment.IsLiked, comment.Id);
            if (comment.IsLiked)
                comment.LikesCount++;
            else
                comment.LikesCount--;
        };

        $scope.addComment = function (index) {
            var twit = $scope.twits[index];
            if (twit.Comment && !$scope.autocompleteVisible) {
                twitHub.server.addComment(twit.Comment, twit.Id, twit.UserName);
                $scope.twits[index].Comment = "";
            }
        };

        twitHub.client.newMessage = function onNewMessage(twit) {
            var shouldAdd = true
            angular.forEach($scope.twits, function (t) {
                if (t.Id === twit.Id) shouldAdd=false;
            })

            if (shouldAdd) {
                $scope.twits.unshift(twit);
                $scope.$apply();
            }
        };

        twitHub.client.newComment = function onNewMessage(comment, twitId) {
            angular.forEach($scope.twits, function (twit) {
                if (twit.Id === twitId) {
                    var shouldAdd = true;
                    angular.forEach(twit.Comments, function (c) {
                        if (c.Id === comment.Id) shouldAdd = false;
                    });
                    if (shouldAdd) {
                        twit.Comments.push(comment);
                    }
                }
            })
            $scope.$apply();
        };
    }]
);