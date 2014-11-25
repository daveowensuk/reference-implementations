angular.module('izendaDashboard').controller('IzendaToolbarController', ['$scope', '$rootScope', '$window', '$location', '$cookies',
	'$izendaRsQuery', '$izendaDashboardToolbarQuery', '$izendaUrl',
	function IzendaToolbarController($scope, $rootScope, $window, $location, $cookies, $izendaRsQuery, $izendaDashboardToolbarQuery, $izendaUrl) {
		'use strict';

		$scope.backgroundColorStyle = {
			'background-color': getCookie('izendaDashboardBackgroundColor') ? getCookie('izendaDashboardBackgroundColor') : '#1c8fd6'
		};
		$scope.izendaBackgroundColor = getCookie('izendaDashboardBackgroundColor') ? getCookie('izendaDashboardBackgroundColor') : '#1c8fd6';

		$scope.$izendaUrl = $izendaUrl;
		$scope.dashboardCategoriesLoading = true;
		$scope.dashboardCategories = [];
		$scope.dashboardsInCurrentCategory = [];
		$scope.leftDashboards = [];
		$scope.rightDashboards = [];

		$scope.buttonbarClass = 'nav navbar-nav iz-dash-toolbtn-panel left-transition';
		$scope.buttonbarCollapsedClass = 'nav navbar-nav iz-dash-collapsed-toolbtn-panel left-transition opened';
		$scope.showButtonBar = function() {
			$scope.buttonbarClass = 'nav navbar-nav iz-dash-toolbtn-panel left-transition opened';
			$scope.buttonbarCollapsedClass = 'nav navbar-nav iz-dash-collapsed-toolbtn-panel left-transition';
		};
		$scope.hideButtonBar = function() {
			$scope.buttonbarClass = 'nav navbar-nav iz-dash-toolbtn-panel left-transition';
			$scope.buttonbarCollapsedClass = 'nav navbar-nav iz-dash-collapsed-toolbtn-panel left-transition opened';
		};

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

		/**
		 * Toggle hue rotate switcher control handler.
		 */
		$scope.toggleHueRotateHandler = function() {
			var $hueRotateControl = angular.element('#izendaDashboardHueRotateSwitcher');
			var turnedOn = $hueRotateControl.hasClass('on');
			var text = turnedOn ? 'OFF' : 'ON';
			$hueRotateControl.find('.iz-dash-switcher-text').text(text);
			$hueRotateControl.toggleClass('on');
			resetRotate();
			toggleHueRotate();
		};

		/**
		 * Initialize background color picker control.
		 */
		$scope.initializeColorPicker = function () {
			var $colorPickerInput = angular.element('#izendaDashboardColorPicker');
			$colorPickerInput.minicolors({
				inline: true,
				control: 'hue',
				change: function (hex) {
					angular.element('.hue-rotate-btn, .iz-dash-background').css('background-color', hex);
					$cookies.izendaBackgroundColor = hex;
					document.cookie = "izendaDashboardBackgroundColor=" + hex;
					$scope.backgroundColorStyle = {
						'background-color': hex
					};
					$scope.izendaBackgroundColor = hex;
				}
			});
			$colorPickerInput.minicolors('value', [$scope.izendaBackgroundColor]);

			// prevent closing dropdown menu:
			angular.element('.dropdown-no-close-on-click.dropdown-menu .minicolors-grid, .dropdown-no-close-on-click.dropdown-menu .minicolors-slider, #izendaDashboardHueRotateSwitcher').click(function (e) {
				e.stopPropagation();
			});
		};

		// initialize dashboard method
		initialize();

		//////////////////////////////////////////////////////
		// PRIVATE
		//////////////////////////////////////////////////////

		function getCookie(name) {
			var nameEq = name + "=";
			//alert(document.cookie);
			var ca = document.cookie.split(';');
			for (var i = 0; i < ca.length; i++) {
				var c = ca[i];
				while (c.charAt(0) == ' ') c = c.substring(1);
				if (c.indexOf(nameEq) != -1) return c.substring(nameEq.length, c.length);
			}
			return null;
		}

		function resetRotate() {
			var e = angular.element('.iz-dash-background');
			e.css({ 'filter': 'hue-rotate(' + '0' + 'deg)' });
			e.css({ '-webkit-filter': 'hue-rotate(' + '0' + 'deg)' });
			e.css({ '-moz-filter': 'hue-rotate(' + '0' + 'deg)' });
			e.css({ '-o-filter': 'hue-rotate(' + '0' + 'deg)' });
			e.css({ '-ms-filter': 'hue-rotate(' + '0' + 'deg)' });
		}

		/**
		 * Turn off/on hue rotate effect handler
		 */
		function toggleHueRotate() {
			if (window.hueRotateTimeOut == null) {
				angular.element('.hue-rotate-btn').children('img').attr('src', 'DashboardsResources/images/color.png');
				var e = angular.element('.iz-dash-background');
				if (window.chrome) {
					rotate(e);
				}
			} else {
				angular.element('.hue-rotate-btn').children('img').attr('src', 'DashboardsResources/images/color-bw.png');
				clearTimeout(hueRotateTimeOut);
				window.hueRotateTimeOut = null;
			}
		}

		/**
		 * Initialize dashboard navigation
		 */
		function initialize() {
			if (angular.element('body > .iz-dash-background').length == 0) {
				angular.element('body').prepend(angular.element('<div class="iz-dash-background" style="background-color: ' +
					$scope.backgroundColorStyle['background-color'] + '"></div>'));
			}


			// start window resize handler
			$scope.turnOnWindowResizeHandler();

			// load dashboard navigation
			$izendaDashboardToolbarQuery.loadDashboardNavigation().then(function (data) {
				dashboardNavigationLoaded(data);
			});

			$scope.initializeColorPicker();
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

			// cancel all current queries
			$izendaRsQuery.cancelAllQueries('Starting load next dashboard.');

			angular.element('.iz-dash-tile').css('display', 'none');

			// notify dashboard to start
			$rootScope.$broadcast('dashboardSetEvent', [{}]);

			// update toolbar items
			updateToolbarItems();
		}
	}]);
