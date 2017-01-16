'use strict'

tradetwitsApp.factory('UsersService', ['twitHub',
    function (twitHub) {
        return {
            addFollowed: function (name) {
                twitHub.server.follow(name);
            },
            removeFollowed: function (name) {
                twitHub.server.unfollow(name);
            },
            addBlocked: function (name) {
                twitHub.server.block(name);
            },
            removeBlocked: function (name) {
                twitHub.server.unblock(name);
            }
        }
    }]
);