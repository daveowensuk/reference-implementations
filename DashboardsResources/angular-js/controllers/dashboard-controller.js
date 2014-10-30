angular.module('izendaDashboard').controller('IzendaDashboardController', ['$scope', '$animate', '$timeout', '$injector', '$izendaUrl', '$izendaDashboardQuery',
function IzendaDashboardController($scope, $animate, $timeout, $injector, $izendaUrl, $izendaDashboardQuery) {
	'use strict';
	
	// dashboard options
	$scope.options = {
		dashboardFullName: null,
		dashboardCategory: null,
		dashboardName: null,
		urlSettings: $izendaUrl.urlSettings
	};

	// dashboard tiles container
	$scope.tileContainerStyle = {
		'height': 0
	};

	// dashboard tiles
	$scope.tiles = [];
	$scope.tileWidth = 0;
	$scope.tileHeight = 0;
	
	// old dashboard instance
	$scope.dashboard = null;

	/**
	 * Dashboard window resized handler
	 */
	$scope.$on('dashboardResizeEvent', function () {
		updateDashboardSize();
	});

	/**
	 * Create new dashboard
	 */
	$scope.$on('dashboardCreateNewEvent', function () {
		if ($scope.dashboard)
			$scope.dashboard.createNewDashboard();
	});

	/**
	 * Listen 'dashboardSetEvent' event. Initialize dashboard when it is fired.
	 */
	$scope.$on('dashboardSetEvent', function (event, args) {
		var options = args[0];
		$scope.initialize(options);
	});

	/**
	 * Listen dashboard refresh event.
	 */
	$scope.$on('dashboardRefreshEvent', function (event, args) {
		refreshAllTiles();
	});

	/**
	 * Listen dashboard "save/save as" event
	 */
	$scope.$on('dashboardSaveEvent', function (event, args) {
		if (!$scope.dashboard)
			return;
		var useSaveAs = args[0];
		if (useSaveAs)
			$scope.dashboard.saveDashboard();
		else
			$scope.dashboard.saveDashboardAs();
	});

	/**
	 * Dashboard tile changes event
	 */
	$scope.$on('dashboardLayoutChanged', function (event, args) {
		if (!angular.isUndefined(args) && !angular.isUndefined(args[0]))
			updateTileContainerSize(args[0]);
		else
			updateTileContainerSize();
	});

	/**
	 * Initialize dashboard
	 */
	$scope.initialize = function (options) {
		// set options
		$scope.options.reportInfo = $izendaUrl.getReportInfo();

		// prepare dashboard parameters
		prepareDashboard();

		// load dashboard tiles layout
		loadDashboardLayout();
	};

	/**
	 * Calculate and set main dashboard parameters
	 */
	function prepareDashboard() {
		// add background
		if (angular.element('body').children('.iz-dash-background').length == 0)
			angular.element('body').prepend(angular.element('<div class="iz-dash-background"></div>'));

		updateDashboardSize();
	}

	////////////////////////////////////////////////////////
	// tiles functions:
	////////////////////////////////////////////////////////

	/**
	 * Load dashboard layout
	 */
	function loadDashboardLayout() {
		// load dashboard layout
		if ($scope.tiles.length > 0) {
			$scope.tiles.length = 0;
		}
		var startTime = (new Date()).getTime();
		$izendaDashboardQuery.loadDashboardLayout($scope.options.reportInfo.fullName).then(function (data) {
			var timeElapsed = (new Date()).getTime() - startTime;
			var timeout = 1000 - timeElapsed;
			if (timeout <= 0) timeout = 1;
			$timeout(function() {
				var maxHeight = 0;
				if (data == null || data.Rows == null || data.Rows.length == 0) {
					// if there is no data - create new empty tile
					$scope.tiles.push(
						angular.extend({}, $injector.get('tileDefaults'), {
							id: 'IzendaDashboardTile0',
							isNew: true,
							width: 12,
							height: 4
						})
					);
					maxHeight = 4;
				} else {
					var cells = data.Rows[0].Cells;
					for (var i = 0; i < cells.length; i++) {
						var cell = cells[i];
						var obj = angular.extend({}, $injector.get('tileDefaults'), $izendaUrl.extractReportPartNames(cell.ReportFullName), {
							id: 'IzendaDashboardTile' + i,
							x: cell.X,
							y: cell.Y,
							width: cell.Width,
							height: cell.Height,
							top: cell.RecordsCount
						});
						if (maxHeight < cell.Y + cell.Height)
							maxHeight = cell.Y + cell.Height;
						$scope.tiles.push(obj);
					}
				}

				// raise dashboard layout change event
				$scope.$broadcast('dashboardLayoutChanged', [{
					top: 0,
					left: 0,
					height: (maxHeight) * $scope.tileHeight,
					width: 12 * $scope.tileWidth
				}]);
			}, timeout);
		});
	};

	////////////////////////////////////////////////////////
	// tile container functions:
	////////////////////////////////////////////////////////

	function getRootContainer() {
		return angular.element('#dashboardsDiv');
	}

	/**
	 * Get tile container
	 */
	function getTileContainer() {
		return angular.element('#dashboardBodyContainer');
	}

	/**
	 * Update dashboard size parameters
	 */
	function updateDashboardSize() {
		updateTileContainerSize();
		console.log($scope.tileWidth, $scope.tileHeight);
	}

	/**
	 * Tile container style
	 */
	function updateTileContainerSize(additionalBox) {
		var $tileContainer = getTileContainer();

		// update width
		var parentWidth = getRootContainer().width();
		var width = Math.floor(parentWidth / 12) * 12;
		$tileContainer.width(width);

		$scope.tileWidth = width / 12;
		$scope.tileHeight = $scope.tileWidth > 100 ? $scope.tileWidth : 100;
		if (!$scope.$$phase)
			$scope.$apply();

		// update height
		var maxHeight = 0;
		$tileContainer.find('.iz-dash-tile').each(function (iTile, tile) {
			var $tile = angular.element(tile);
			if ($tile.position().top + $tile.height() > maxHeight) {
				maxHeight = $tile.position().top + $tile.height();
			}
		});
		if (!angular.isUndefined(additionalBox)) {
			if (additionalBox.top + additionalBox.height > maxHeight) {
				maxHeight = additionalBox.top + additionalBox.height;
			}
		}
		$scope.tileContainerStyle.height = (maxHeight + $scope.tileHeight) + 'px';
	};

	/**
	 * Refresh all tiles
	 */
	function refreshAllTiles() {
		$scope.$broadcast('tileRefreshEvent', [false]);
	}

}]);
