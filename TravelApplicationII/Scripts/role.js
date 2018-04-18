app.controller("roleController", ['$scope', '$location', '$routeParams', '$uibModal', function ($scope, $location, $routeParams, $uibModal) {

    $scope.userName2 = $routeParams.userName;

    $uibModal.open(
        {
            templateUrl: 'uitemplates/role.html',
            size: '',
            scope: $scope,
            backdrop: 'static',
            keyboard: false
        });
}]);