angular.module('izendaDashboard').controller('IzendaToolbarController', ['$scope', '$rootScope', '$window', '$location', '$cookies',
	'$izendaRsQuery', '$izendaDashboardToolbarQuery', '$izendaUrl',
	function IzendaToolbarController($scope, $rootScope, $window, $location, $cookies, $izendaRsQuery, $izendaDashboardToolbarQuery, $izendaUrl) {
		'use strict';

		$scope.backgroundColorStyle = {
			'background-color': getCookie('izendaDashboardBackgroundColor') ? getCookie('izendaDashboardBackgroundColor') : '#1c8fd6'
		};
		$scope.izendaBackgroundColor = getCookie('izendaDashboardBackgroundColor') ? getCookie('izendaDashboardBackgroundColor') : '#1c8fd6';
		$scope.izendaBackgroundImageSrc = getImgFromStorage();
		$scope.backgroundFileChangedHandler = function () {
			console.log(arguments);
		};

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

		/**
		 * Create style object for toolbar item
		 */
		$scope.getToolItemStyle = function (dashboard) {
			var parseRgb = function (s) {
				if (!s) return null;
				if (s.indexOf('#') == 0) {
					var r = s.substr(1, 2),
						g = s.substr(3, 2),
						b = s.substr(5, 2);
					return [parseInt(r, 16), parseInt(g, 16), parseInt(b, 16)];
				}
				return null;
			};
			var rgb2hsv = function(colorArray) {
				var rr,
					gg,
					bb,
					r = colorArray[0] / 255,
					g = colorArray[1] / 255,
					b = colorArray[2] / 255,
					h,
					s,
					v = Math.max(r, g, b),
					diff = v - Math.min(r, g, b),
					diffc = function(c) {
						return (v - c) / 6 / diff + 1 / 2;
					};

				if (diff == 0) {
					h = s = 0;
				} else {
					s = diff / v;
					rr = diffc(r);
					gg = diffc(g);
					bb = diffc(b);

					if (r === v) {
						h = bb - gg;
					} else if (g === v) {
						h = (1 / 3) + rr - bb;
					} else if (b === v) {
						h = (2 / 3) + gg - rr;
					}
					if (h < 0) {
						h += 1;
					} else if (h > 1) {
						h -= 1;
					}
				}
				return [Math.round(h * 360), Math.round(s * 100), Math.round(v * 100)];
			};
			var hsv2rgb = function (colorArray)	{
				var h = colorArray[0],
					s = colorArray[1],
					v = colorArray[2];
				var r, g, b;
				var i;
				var f, p, q, t;
				h = Math.max(0, Math.min(360, h));
				s = Math.max(0, Math.min(100, s));
				v = Math.max(0, Math.min(100, v));
				s /= 100;
				v /= 100;
				if (s == 0) {
					r = g = b = v;
					return [Math.round(r * 255), Math.round(g * 255), Math.round(b * 255)];
				}
				h /= 60;
				i = Math.floor(h);
				f = h - i;
				p = v * (1 - s);
				q = v * (1 - s * f);
				t = v * (1 - s * (1 - f));
				switch (i) {
					case 0:
						r = v;g = t;b = p;break;
					case 1:
						r = q;g = v;b = p;break;
					case 2:
						r = p;g = v;b = t;break;
					case 3:
						r = p;g = q;b = v;break;
					case 4:
						r = t;g = p;b = v;break;
					default: // case 5:
						r = v;g = p;b = q;
				}
				return [Math.round(r * 255), Math.round(g * 255), Math.round(b * 255)];
			};
			var getContrastHsvColor = function (hsvArray) {
				var s = hsvArray[1];
				var l = hsvArray[2];
				if (l < 80)
					l = 80;
				if (s < 80)
					s = 80;
				return [(hsvArray[0] + 180) % 360, s, l];
			};
			var ensureColorComp = function(colorInt) {
				var v = colorInt.toString(16);
				if (v.length == 1)
					v = '0' + v;
				return v;
			};

			var isActive = dashboard == $izendaUrl.getReportInfo().fullName;
			var style = {
				'border-bottom': '4px solid transparent'
			};
			if (isActive) {
				var c = parseRgb($scope.izendaBackgroundColor);
				var contrast = hsv2rgb(getContrastHsvColor(rgb2hsv(c)));
				var res = '#' + ensureColorComp(contrast[0]) + ensureColorComp(contrast[1]) + ensureColorComp(contrast[2]);
				style['background-color'] = '#f3f3f3';
				style['border-bottom'] = '4px solid ' + res + '';
				style['font-weight'] = 'bold';
			}
			return style;
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

		function setImgToStorage(img) {
			var imgData = getBase64Image(img);
			localStorage.setItem('izendaDashboardBackgroundImg', imgData);
		}

		function getImgFromStorage() {
			var dataImage = localStorage.getItem('izendaDashboardBackgroundImg');
			return 'data:image/png;base64,' + dataImage;
		}

		function getBase64Image(img) {
			// Create an empty canvas element
			var canvas = document.createElement("canvas");
			canvas.width = img.width;
			canvas.height = img.height;

			// Copy the image contents to the canvas
			var ctx = canvas.getContext("2d");
			ctx.drawImage(img, 0, 0);

			// Get the data-URL formatted image
			// Firefox supports PNG and JPEG. You could check img.src to guess the
			// original format, but be aware the using "image/jpg" will re-encode the image.
			var dataURL = canvas.toDataURL("image/png");

			return dataURL.replace(/^data:image\/(png|jpg);base64,/, "");
		}

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
