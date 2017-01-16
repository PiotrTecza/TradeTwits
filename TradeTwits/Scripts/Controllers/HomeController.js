'use strict'

tradetwitsApp.controller('HomeController', ['$scope', 'TwitService', 'twitHub', 'FileUploader'
    , function ($scope, TwitService, twitHub, FileUploader) {
        $scope.twits = TwitService.get();
        $scope.twit;
        $scope.allTwitsLoaded = false;
        $scope.isLoading = false;
       
        var uploader = $scope.uploader = new FileUploader({
            url: window.location.protocol + '//' + window.location.host + window.location.pathname + '/api/Upload/Send',
            queueLimit: 1
        });

        uploader.onBeforeUploadItem = function (item) {
            item.formData.push({ message: $scope.twit });
        };

        uploader.onSuccessItem = function (fileItem, response, status, headers) {
            $scope.twit = ""
            $scope.uploader.queue = [];
            $scope.uploader.progress = 0;
        };

        uploader.filters.push({
            name: 'extensionFilter',
            fn: function (item, options) {
                var filename = item.name;
                var extension = filename.substring(filename.lastIndexOf('.') + 1).toLowerCase();
                if (extension == "jpg" || extension == "jpeg" || extension == "png" ||
                    extension == "gif")
                    return true;
                else {
                    alert('Invalid file format. Please select a file with jpg/jpeg/png or gif format and try again.');
                    return false;
                }
            }
        });

        uploader.filters.push({
            name: 'sizeFilter',
            fn: function (item, options) {
                var fileSize = item.size;
                fileSize = parseInt(fileSize) / (1024 * 1024);
                if (fileSize <= 5)
                    return true;
                else {
                    alert('Selected file exceeds the 5MB file size limit. Please choose a new file and try again.');
                    return false;
                }
            }
        });


        $scope.sendMessage = function () {
            
            if ($scope.twit && !$scope.autocompleteVisible) {
                twitHub.server.send($scope.twit);
                $scope.twit = "";
            }
        };

        $(window).scroll(function () {
            if (!$scope.isLoading && !$scope.allTwitsLoaded && $(window).scrollTop() >= $(document).height() - $(window).height() - 10) {
                $scope.isLoading = true;
                var lastTwitDate = $scope.twits[$scope.twits.length - 1].CreatedAt
                var nextTwits = TwitService.getNext(lastTwitDate).$promise.then(function (result) {
                    $scope.isLoading = false;
                    if (result.length > 0) {
                        $scope.twits = $scope.twits.concat(result);
                    }
                    else {
                        $scope.allTwitsLoaded = true;
                    }
                });
            }
        });

    }]
);