angular.module('izendaDashboard').controller('IzendaToolbarController', ['$scope', '$rootScope', '$window', '$location',
	'$izendaDashboardToolbarQuery', '$izendaUrl',
	function IzendaToolbarController($scope, $rootScope, $window, $location, $izendaDashboardToolbarQuery, $izendaUrl) {
		'use strict';

		$scope.$izendaUrl = $izendaUrl;

		$scope.dashboardCategoriesLoading = true;
		$scope.dashboardCategories = [];
		$scope.dashboardsInCurrentCategory = [];
		$scope.leftDashboards = [];
		$scope.rightDashboards = [];

		//////////////////////////////////////////////////////
		// EVENT HANDLERS
		//////////////////////////////////////////////////////

		/**
		 * Start initialize dashboard when location was changed or page was loaded
		 */
		$scope.$on('$locationChangeSuccess', function () {
			if ($izendaUrl.getReportInfo().fullName == null)
				return;
			setDashboard($izendaUrl.getReportInfo().fullName);
		});

		/**
		 * Start tile edit event handler
		 */
		$scope.$on('startEditTileEvent', function () {
			$scope.turnOffWindowResizeHandler();
		});

		/**
		 * Tile edit completed event handler
		 */
		$scope.$on('stopEditTileEvent', function () {
			$scope.turnOnWindowResizeHandler();
		});

		/**
		 * Check toggleHueRotateEnabled
		 */
		$scope.isToggleHueRotateEnabled = function () {
			var isChrome = /Chrome/.test(navigator.userAgent) && /Google Inc/.test(navigator.vendor);
			var isSafari = /Safari/.test(navigator.userAgent) && /Apple Computer/.test(navigator.vendor);
			return isChrome || isSafari;
		};

		/**
		 * Turn off/on hue rotate effect handler
		 */
		$scope.toggleHueRotateHandler = function () {
			if (window.hueRotateTimeOut == null) {
				angular.element('.hue-rotate-btn').children('img').attr('src', 'Resources/images/hue-rotate.png');
				var e = angular.element('.iz-dash-background');
				if (window.chrome) {
					rotate(e);
				}
			} else {
				angular.element('.hue-rotate-btn').children('img').attr('src', 'Resources/images/hue-rotate-inactive.png');
				clearTimeout(hueRotateTimeOut);
				window.hueRotateTimeOut = null;
			}
		};

		/**
		 * Create new dashboard button handler.
		 */
		$scope.createNewDashboardHandler = function () {
			$rootScope.$broadcast('dashboardCreateNewEvent', []);
		};

		/**
		 * Refresh dashboard button handler.
		 */
		$scope.refreshDashboardHandler = function () {
			$rootScope.$broadcast('dashboardRefreshEvent', []);
		};

		/**
		 * Save/SaveAS dialog
		 */
		$scope.saveDashboardHandler = function (showSaveAsDialog) {
			$rootScope.$broadcast('dashboardSaveEvent', [showSaveAsDialog]);
		};

		/**
		 * Turn on window resize handler
		 */
		$scope.turnOnWindowResizeHandler = function () {
			angular.element($window).on('resize.dashboard', function () {
				updateToolbarItems();
				$rootScope.$broadcast('dashboardResizeEvent', [{}]);
			});
		};

		/**
		 * Turn off window resize handler
		 */
		$scope.turnOffWindowResizeHandler = function () {
			angular.element($window).off('resize.dashboard');
		};

		// initialize dashboard method
		initialize();

		//////////////////////////////////////////////////////
		// PRIVATE
		//////////////////////////////////////////////////////

		/**
		 * Initialize dashboard navigation
		 */
		function initialize() {
			// start window resize handler
			$scope.turnOnWindowResizeHandler();

			// load dashboard navigation
			$izendaDashboardToolbarQuery.loadDashboardNavigation().then(function (data) {
				dashboardNavigationLoaded(data);
			});
		}

		/**
		 * Update toolbar dashboard tabs.
		 */
		function updateToolbarItems() {
			// set current dashboard menu items
			$scope.dashboardsInCurrentCategory.length = 0;
			$scope.leftDashboards.length = 0;
			$scope.rightDashboards.length = 0;
			for (var i = 0; i < $scope.dashboardCategories.length; i++) {
				if ($scope.dashboardCategories[i].name == $izendaUrl.getReportInfo().category) {
					var dashboards = $scope.dashboardCategories[i].dashboards;
					var activeIndex = dashboards.indexOf($izendaUrl.getReportInfo().fullName);
					var countAdded = 0;
					var leftIndex = activeIndex - 1;
					var rightIndex = activeIndex + 1;

					var $w = angular.element($window);
					var othersMaximumCount = 4;
					if ($w.width() < 1280) {
						othersMaximumCount = 0;
					} else if ($w.width() >= 1280 && $w.width < 1680) {
						othersMaximumCount = 2;
					} else if ($w.width() >= 1680 && $w.width < 1920) {
						othersMaximumCount = 6;
					} else if ($w.width() >= 1920) {
						othersMaximumCount = 8;
					}
					if (activeIndex >= 0) {
						var result = [];
						result.push(dashboards[activeIndex]);
						while (countAdded < othersMaximumCount && countAdded < dashboards.length) {
							if (leftIndex >= 0) {
								result.splice(0, 0, dashboards[leftIndex]);
								leftIndex--;
								countAdded++;
							}
							if (rightIndex < dashboards.length) {
								result.push(dashboards[rightIndex]);
								rightIndex++;
								countAdded++;
							}
							if (leftIndex < 0 && rightIndex >= dashboards.length)
								break;
						}
						if (leftIndex >= -1)
							$scope.leftDashboards.push.apply($scope.leftDashboards, dashboards.slice(0, leftIndex + 1).reverse());
						$scope.dashboardsInCurrentCategory.push.apply($scope.dashboardsInCurrentCategory, result);
						if (rightIndex < dashboards.length)
							$scope.rightDashboards.push.apply($scope.rightDashboards, dashboards.slice(rightIndex));
					} else if ($izendaUrl.getReportInfo().isNew || !$izendaUrl.getReportInfo().fullName) {
						// keep toolbar tabs unchanged when isNew
					} else {
						throw $izendaUrl.getReportInfo().fullName + ' not found in ' + dashboards;
					}
				}
			}
		}

		/**
		 * Dashboard categories loaded. Now we have to update it.
		 */
		function dashboardNavigationLoaded(data) {
			$scope.dashboardCategoriesLoading = false;
			$scope.dashboardCategories.length = 0;
			if (angular.isObject(data)) {
				for (var category in data) {
					var dashboards = data[category];
					if (dashboards.length > 0) {
						var item = {
							id: (new Date()).getTime(),
							name: category,
							dashboards: data[category]
						};
						$scope.dashboardCategories.push(item);
					}
				}
				updateToolbarItems();
			}

			// check dashboard parameter is defined
			var fullName = $scope.$izendaUrl.getReportInfo().fullName;
			if (angular.isUndefined(fullName) || fullName == null) {
				// go to the first found dashboard, if parameter isn't defined
				if ($scope.dashboardCategories.length > 0) {
					// update url
					fullName = $scope.dashboardCategories[0].dashboards[0];
					$izendaUrl.setReportFullName(fullName);
				} else {
					throw 'There is no dashboards to load.';
				}
			}
		}

		/**
		 * Set current dashboard
		 */
		function setDashboard(dashboardFullName) {
			console.log(' ');
			console.log('>>>>> Set current dashboard: "' + dashboardFullName + '"');
			console.log(' ');

			// notify dashboard to start
			$rootScope.$broadcast('dashboardSetEvent', [{}]);

			// update toolbar items
			updateToolbarItems();
		}
	}]);
