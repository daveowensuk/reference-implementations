angular.module('izendaDashboard').controller('IzendaDashboardController', ['$rootScope', '$scope', '$q', '$animate', '$timeout', '$injector', '$izendaUrl', '$izendaDashboardQuery',
function IzendaDashboardController($rootScope, $scope, $q, $animate, $timeout, $injector, $izendaUrl, $izendaDashboardQuery) {
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
	// is dashboard changing now.
	$scope.isChangingNow = false;

	// dashboard tiles
	$scope.tiles = [];
	$scope.tileWidth = 0;
	$scope.tileHeight = 0;

	// dashboard resize
	$scope.windowResizeOptions = {
		timeout: false
	};

	////////////////////////////////////////////////////////
	// event handlers:
	////////////////////////////////////////////////////////

	/**
	 * Dashboard window resized handler
	 */
	$scope.$on('dashboardResizeEvent', function () {
		$scope.isChangingNow = true;
		if (!$scope.$$phase)
			$scope.$apply();
		// update dashboard tile sizes
		updateDashboardSize();

		// update all tiles
		var resizeEnd = function () {
			if (new Date() - $scope.windowResizeOptions.rtime < 500) {
				setTimeout(function () {
					resizeEnd.apply();
				}, 500);
			} else {
				$scope.windowResizeOptions.timeout = false;
				$scope.isChangingNow = false;
				$rootScope.$broadcast('windowResized', []);
				$scope.refreshAllTiles();
			}
		};
		$scope.windowResizeOptions.rtime = new Date();
		if ($scope.windowResizeOptions.timeout === false) {
			$scope.windowResizeOptions.timeout = true;
			setTimeout(function () {
				resizeEnd.apply();
			}, 500);
		}
	});

	/**
	 * Create new dashboard
	 */
	$scope.$on('dashboardCreateNewEvent', function () {
		/*if ($scope.dashboard)
			$scope.dashboard.createNewDashboard();*/
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
		/*if (!$scope.dashboard)
			return;
		var useSaveAs = args[0];
		if (useSaveAs)
			$scope.dashboard.saveDashboard();
		else
			$scope.dashboard.saveDashboardAs();*/
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
	 * Start tile edit event handler
	 */
	$scope.$on('startEditTileEvent', function (event, args) {
		$scope.showTileGrid();
	});

	/**
	 * Tile edit completed event handler
	 */
	$scope.$on('stopEditTileEvent', function (event, args) {
		$scope.hideTileGrid();
	});

	////////////////////////////////////////////////////////
	// scope helper functions:
	////////////////////////////////////////////////////////

	$scope.getRoot = function() {
		return angular.element('#dashboardsDiv');
	};
	$scope.getTileContainer = function() {
		return angular.element('#dashboardBodyContainer');
	};
	$scope.getTileById = function(tileId) {
		for (var i = 0; i < $scope.tiles.length; i++) {
			if ($scope.tiles[i].id == tileId)
				return $scope.tiles[i];
		}
		return null;
	};
	$scope.getTileByTile$ = function(tile$) {
		return $scope.getTileById($scope.getTile$Id(tile$));
	};
	$scope.getOtherTiles = function(tiles, tile) {
		if (tile == null)
			return tiles;
		var otherTiles = [];
		for (var i = 0; i < tiles.length; i++) {
			if (tiles[i].id != tile.id)
				otherTiles.push(tiles[i]);
		}
		return otherTiles;
	};
	$scope.getTile$ById = function(tileId) {
		return $scope.getTileContainer().find('.iz-dash-tile[tileid="' + tileId + '"]');
	};
	$scope.getTile$Id = function($tile) {
		return $tile.attr('tileid');
	};
	$scope.getTile$ByInnerEl = function(el) {
		var $el = angular.element(el);
		return angular.element($el.closest('.iz-dash-tile'));
	};
	
	////////////////////////////////////////////////////////
	// scope functions:
	////////////////////////////////////////////////////////

	/**
	 * Initialize dashboard
	 */
	$scope.initialize = function (options) {
		// set options
		$scope.options.reportInfo = $izendaUrl.getReportInfo();

		// remove content from all tiles to speed up "bounce up" animation
		angular.element('.report').empty();

		// prepare dashboard parameters
		prepareDashboard();

		// load dashboard tiles layout
		loadDashboardLayout();
	};

	/**
	 * Refresh all tiles.
	 */
	$scope.refreshAllTiles = function () {
		refreshAllTiles();
	};

	/**
	 * Show editor grid
	 */
	$scope.showTileGrid = function () {
		var $gridPlaceholder = $scope.getTileContainer();
		if (angular.isUndefined($scope.$grid) || $scope.$grid == null) {
			$scope.$grid = angular.element('<div></div>')
				  .addClass('dashboard-grid')
				  .hide();
			$gridPlaceholder.prepend($scope.$grid);
		}
		$scope.$grid.css('background-size', $scope.tileWidth + 'px ' + $scope.tileHeight + 'px, ' +
					$scope.tileWidth + 'px ' + $scope.tileHeight + 'px');
		$scope.$grid.show();
	};

	/**
	 * Hide editor grid 
	 */
	$scope.hideTileGrid = function () {
		if (angular.isUndefined($scope.$grid) || $scope.$grid == null)
			return;
		$scope.$grid.hide();
		$scope.hideTileGridShadow();
	};

	/**
	 * Show tile grid shadow
	 */
	$scope.showTileGridShadow = function(shadowBbox, showPlusButton) {
		var $gridPlaceholder = $scope.getTileContainer();
		if (angular.isUndefined($scope.$grid) || $scope.$grid == null) {
			throw 'Can\'t show shadow without grid';
		}
		var $shadow = $gridPlaceholder.find('.tile-grid-cell.shadow');
		if ($shadow.length == 0) {
			$shadow = angular.element('<div class="tile-grid-cell shadow"></div>').css({
				'opacity': 0.2,
				'background-color': '#000'
			});
			if (showPlusButton) {
				var $plus = $('<div class="iz-dash-select-report-front-container">' +
				  '<button type="button" class="iz-dash-select-report-front-btn2 btn" title="Select Report">' +
				  '<span class="glyphicon glyphicon-plus"></span>' +
				  '</button>' +
				  '</div>');
				$shadow.append($plus);
			}
			$gridPlaceholder.prepend($shadow);
		}

		// move shadow
		$shadow.css({
			'left': shadowBbox.x,
			'top': shadowBbox.y,
			'width': shadowBbox.width,
			'height': shadowBbox.height
		});
		$shadow.show();
	};

	/**
	 * Hide tile grid shadow
	 */
	$scope.hideTileGridShadow = function() {
		var $gridPlaceholder = $scope.getTileContainer();
		var $shadow = $gridPlaceholder.find('.tile-grid-cell.shadow');
		$shadow.hide();
	};

	/**
	 * Check tile intersects to any other tile
	 */
	$scope.checkTileIntersects = function (tile, $helper) {
		var hitTest = function (a, b, accuracy) {
			var aPos = a.offset();
			var bPos = b.offset();

			var aLeft = aPos.left;
			var aRight = aPos.left + a.width();
			var aTop = aPos.top;
			var aBottom = aPos.top + a.height();

			var bLeft = bPos.left + accuracy;
			var bRight = bPos.left + b.width() - accuracy;
			var bTop = bPos.top + accuracy;
			var bBottom = bPos.top + b.height() - accuracy;
			return !(bLeft > aRight
				|| bRight < aLeft
				|| bTop > aBottom
				|| bBottom < aTop
			);
		};
		// check

		var $tile = $scope.getTile$ById(tile.id);
		if (!angular.isUndefined($helper)) {
			$tile = $helper;
		}
		var otherTiles = $scope.getOtherTiles($scope.tiles, tile);
		for (var i = 0; i < otherTiles.length; i++) {
			var oTile = otherTiles[i];
			var $oTile = $scope.getTile$ById(oTile.id);
			if (hitTest($tile, $oTile, 30)) {
				return true;
			}
		}
		return false;
	};

	/**
	 * Check tile is outside the dashboard
	 */
	$scope.checkTileMovedToOuterSpace = function ($tile, sencitivity) {
		var precision = sencitivity;
		if (typeof (sencitivity) == 'undefined' || sencitivity == null)
			precision = 0;
		var tp = $tile.position();
		if (tp.left + $tile.width() > $scope.tileWidth * 12 + precision || tp.left < -precision || tp.top < -precision)
			return true;
		return false;
	};

	/**
	 * Get tile which lie under testing tile.
	 */
	$scope.getUnderlyingTile = function(x, y, testingTile) {
		var $target = null;
		var targetTile;
		var $tiles = $scope.getRoot().find('.iz-dash-tile');
		for (var i = 0; i < $tiles.length; i++) {
			var $t = angular.element($tiles[i]);
			if ($t.hasClass('iz-dash-tile-helper'))
				break;
			var tile = $scope.getTileByTile$($t);
			if (tile == null || tile.id != testingTile.id) {
				var tilePosition = $t.offset();
				if (tilePosition.left <= x && tilePosition.left + $t.width() >= x
					&& tilePosition.top <= y && tilePosition.top + $t.height() >= y) {
					targetTile = tile;
					$target = $t;
				}
			}
		}
		return $target;
	};

	/**
	 * Swap 2 tiles. Return promise after complete swap.
	 */
	$scope.swapTiles = function ($tile1, $tile2) {
		return $q(function (resolve) {
			var t1O = $tile1.position(),
				t2O = $tile2.position(),
				w1 = $tile1.width(),
				h1 = $tile1.height(),
				w2 = $tile2.width(),
				h2 = $tile2.height();

			$tile1.find('.frame').hide();
			$tile2.find('.frame').hide();

			var completeCount = 0;
			$tile1.animate({
				left: t2O.left,
				top: t2O.top,
				width: w2,
				height: h2
			}, 500, function () {
				if (completeCount == 0)
					completeCount++;
				else {
					resolve([$tile1, $tile2]);
				}
			});
			$tile2.animate({
				left: t1O.left,
				top: t1O.top,
				width: w1,
				height: h1
			}, 500, function () {
				if (completeCount == 0)
					completeCount++;
				else {
					resolve([$tile1, $tile2]);
				}
			});
		});
	};
	
	////////////////////////////////////////////////////////
	// dashboard functions:
	////////////////////////////////////////////////////////

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
			$timeout(function () {
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

	/**
	 * Refresh all tiles
	 */
	function refreshAllTiles() {
		$scope.$broadcast('tileRefreshEvent', [false]);
	}

	////////////////////////////////////////////////////////
	// tile container functions:
	////////////////////////////////////////////////////////

	/**
	 * Update dashboard size parameters
	 */
	function updateDashboardSize() {
		updateTileContainerSize();
	}

	/**
	 * Tile container style
	 */
	function updateTileContainerSize(additionalBox) {
		var $tileContainer = $scope.getTileContainer();

		// update width
		var parentWidth = $scope.getRoot().width();
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
}]);
