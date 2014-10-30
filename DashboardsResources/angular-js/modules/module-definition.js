angular.module('izendaUrl', []);
angular.module('izendaQuery', ['izendaUrl']);
angular.module('izendaDashboard', ['ngRoute', 'ngFx', 'izendaQuery']);
angular.module('izendaDashboard').directive('cancelOutAnimation', function($animate){
	function link(scope, element){
		scope.$on('zoom-down:leave', function () {
			angular.element(element).find('.report').empty();
			/*$animate.leave(element);*/
		});
	}
	return {
		restrict: 'A',
		link: link
	};
})