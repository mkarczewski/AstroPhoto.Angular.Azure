var phonecatApp = angular.module('addPhotoApp', [
  'ngRoute', 'utils'
]);

phonecatApp.config(['$routeProvider',
    function ($routeProvider) {
        $routeProvider
            .when('/index', { templateUrl: '/AppScripts/index.html', controller: 'IndexCtrl' })
            .when('/uploadFile', { templateUrl: '/AppScripts/uploadFile.html', controller: 'UploadFileCtrl' })
            .when('/markPhotoObjects', { templateUrl: '/AppScripts/markPhotoObjects.html', controller: 'MarkPhotoObjects' })
            .otherwise({ redirectTo: '/index' })
    }]);

phonecatApp.controller('IndexCtrl', ['$scope', '$http', '$location', function ($scope, $http, $location) {
    
    $scope.refresh = function () {
        $http.get('/Photos/ListObjects?phrase=' + $scope.filter).then(function (response) {
            $scope.items = response.data;
        });
    };

    $scope.addNewPhoto = function () {
        $location.path('/uploadFile');
    };

    $scope.filter = '';
    $scope.refresh();
}]);

phonecatApp.controller('UploadFileCtrl', ['$scope', '$http', 'fileUpload', '$location', function ($scope, $http, fileUpload, $location) {

    $scope.uploadImage = function () {
        var file = $scope.astroPhotoFile;

        if (file == null || file == undefined)
            return;

        fileUpload.uploadFileToUrl(file, '/Photos/UploadPhoto', function (data) {
            var uid = data.name;
            var url = data.url;
            $location.path('/markPhotoObjects').search({ uid: uid, url: url });
        });
    }
}]);

phonecatApp.controller('MarkPhotoObjects', ['$scope', '$http', '$routeParams', '$location', function ($scope, $http, $routeParams, $location) {

    $scope.photoUid = $routeParams.uid;
    $scope.photoUrl = $routeParams.url;

    $scope.items = [];

    $scope.submit = function () {

        $http.post('/Photos/Submit',
            {
                photoUid: $scope.photoUid,
                photoUrl: $scope.photoUrl,
                items: $scope.items
            }
        ).then(function (response) {
            $location.path('/index').search({});
        });
    };

    $scope.objectClick = function (ev) {
        var nw = ev.target.naturalWidth;
        var nh = ev.target.naturalHeight;

        var cw = ev.target.clientWidth;
        var ch = ev.target.clientHeight;

        var px = ev.offsetX;
        var py = ev.offsetY;

        var rx = Math.round(px / cw * nw);
        var ry = Math.round(py / ch * nh);

        $scope.items.push({
            no: 0,
            x: rx,
            y: ry,
            name: 'unnamed object'
        });

        $scope.reorderItems();
    };

    $scope.removeItem = function (no) {
        $scope.items = $.grep($scope.items, function (el, i) {
            return el.no != no;
        });

        $scope.reorderItems();
    };

    $scope.reorderItems = function () {
        $.each($scope.items, function (i, e) {
            e.no = i + 1;
        });
    };

}]);

