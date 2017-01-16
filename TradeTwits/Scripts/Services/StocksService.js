'use strict'

tradetwitsApp.factory('StocksService', ['twitHub',
    function (twitHub) {
        return {
            getStockData: function (name) {
                return twitHub.server.getStockData(name);
            }
        }
    }]
);