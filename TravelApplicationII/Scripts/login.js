app.controller("loginController", ['$scope', '$location', '$routeParams', '$uibModal', '$rootScope',  
    function ($scope, $location, $routeParams, $uibModal, $rootScope) {

    $scope.userName = "";
    $rootScope.userAuthenticated = false;

    var modalInstance = $uibModal.open({
        templateUrl: 'uitemplates/login.html',
        size: '',
        scope: $scope,
        backdrop: 'static',
        keyboard: false
    });

    modalInstance
        .result
            .then(function () {
                //resolved
            }, function () {
                //rejected
            })
            .catch(angular.noop);    

    $scope.sigin = function (userName) {

        if (!userName) {
            sessionStorage.userAuthenticated = false;
            $rootScope.userAuthenticated = false;
            alert('error');
        }
        else {
            sessionStorage.userAuthenticated = true;
            $rootScope.userAuthenticated = true;
            modalInstance.close(false);
            $("#login").hide();
            $("#logout").show();
            
            $location.path("/role/" + userName);
        }
    };

    $scope.go = function (hash) {
        $location.path(hash);
    }

}]);