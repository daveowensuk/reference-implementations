var izendaUrlModule = angular.module('izendaUrl', []);
var izendaCompatibilityModule = angular.module('izendaCompatibility', []);
var izendaQueryModule = angular.module('izendaQuery', ['izendaUrl']);
var izendaDashboardModule;
if (angular.version.major >= 1 && angular.version.minor >= 3)
  izendaDashboardModule = angular.module('izendaDashboard', ['ngRoute', 'ngCookies', 'ngFx', 'izendaUrl', 'izendaCompatibility', 'izendaQuery']);
else
  izendaDashboardModule = angular.module('izendaDashboard', ['ngRoute', 'ngCookies', 'izendaUrl', 'izendaCompatibility', 'izendaQuery']);
