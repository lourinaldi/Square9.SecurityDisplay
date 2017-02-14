var app = angular.module('app', ['ngRoute']);

app.config(function ($routeProvider) {
    $routeProvider
.when('/home', {
    templateUrl: 'partials/home.view.html'
})
.otherwise({
    redirectTo: '/home'
})

});
