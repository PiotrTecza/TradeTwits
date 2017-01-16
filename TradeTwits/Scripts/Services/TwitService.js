'use strict'

tradetwitsApp.factory('TwitService',['$resource',
    function ($resource) {
        return {
            get: function () {
                return $resource('/api/Twits').query();
            },
            getNext: function (lastTwitDate) {
                return $resource('/api/Twits?date=' + lastTwitDate).query();
            },
            getForUser: function (userName, lastTwitDate) {
                return $resource('/api/Twits/GetForUser?userName=' + userName + '&date=' + lastTwitDate).get();
            },
            vote: function (twitId, up, commentId) {
                var Vote = $resource('/api/Twits/Vote');
                var vote1 = new Vote();
                vote1.TwitId = twitId;
                vote1.Up = up;
                vote1.CommentId = commentId;
                vote1.$save();
            }
        }
    }]
);