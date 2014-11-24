angular.module('izendaUrl', []);
angular.module('izendaQuery', ['izendaUrl']);
angular.module('izendaDashboard', ['ngRoute', 'ngCookies', 'ngFx', 'izendaQuery']);