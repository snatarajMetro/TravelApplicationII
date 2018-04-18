var app = angular.module('travelAppDemo', ['ngRoute', 'ui.grid', 'ui.grid.pagination', 'ui.grid.resizeColumns', 'ui.bootstrap']);

app.config(function ($qProvider) {
    $qProvider.errorOnUnhandledRejections(false);
});

app.config(['$routeProvider', '$locationProvider', function AppConfig($routeProvider, $locationProvider) {

    //$rootScope.userAuthenticated = false;

    $routeProvider
        .when('/', {
            templateUrl: 'loginModalContainer',
            controller: 'loginController',
            css: 'content/login.css'
        })
        .when('/login', {
            templateUrl: 'loginModalContainer',
            controller: 'loginController',
            css: 'content/login.css'
        })
        .when('/role/:userName?', {
            templateUrl: 'roleModalContainer',
            controller: 'roleController',
            css: 'content/login.css'
        })
        .otherwise({
            redirectTo: "/"
        });

    // enable html5Mode for pushstate ('#'-less URLs)
    $locationProvider.html5Mode(true);
    $locationProvider.hashPrefix('!');
}]);
