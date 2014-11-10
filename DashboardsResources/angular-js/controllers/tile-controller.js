﻿angular.module('izendaDashboard').constant('tileDefaults', {
	id: null,
	reportFullName: null,
	reportPartName: null,
	reportSetName: null,
	reportName: null,
	reportCategory: null,
	reportNameWithCategory: null,
	x: 0,
	y: 0,
	width: 1,
	height: 1,
	top: 100
});

angular.module('izendaDashboard').controller('IzendaTileController', ['$element', '$rootScope', '$scope', '$injector', '$izendaUrl', '$izendaDashboardQuery',
function IzendaTileController($element, $rootScope, $scope, $injector, $izendaUrl, $izendaDashboardQuery) {
	'use strict';

	$scope.isHidden = false;

	$scope.state = {
		resizableHandlerStarted: false
	};

	/**
	 * Tile refresh event handler
	 */
	$scope.$on('tileRefreshEvent', function (event, args) {
		if (args.length > 0 && typeof(args[0]) == 'boolean')
			$scope.refreshTile(args[0]);
		else
			$scope.refreshTile(false);
	});

	/**
	 * Update tile after completing window resize
	 */
	$scope.$on('windowResized', function(event, args) {
		var $tile = $scope.$parent.getTile$ById($scope.id);
		if ($scope.state.resizableHandlerStarted)
			$tile.resizable('option', 'grid', [$scope.$parent.tileWidth, $scope.$parent.tileHeight]);
	});

	/**
	 * Start tile edit event handler
	 */
	$scope.$on('startEditTileEvent', function (event, args) {
		var eventOptions = angular.isArray(args) && args.length > 0 ? args[0] : null;
		if (eventOptions == null)
			throw 'tile controller: startEditTileEvent should have 1 options argument.';
	});

	/**
	 * Tile edit completed event handler
	 */
	$scope.$on('stopEditTileEvent', function (event, args) {
		var eventOptions = angular.isArray(args) && args.length > 0 ? args[0] : null;
		if (eventOptions == null)
			throw 'tile controller: stopEditTileEvent should have 1 options argument.';

		var tileIdArray = angular.isArray(eventOptions.tileId) ? eventOptions.tileId : [eventOptions.tileId];
		if (tileIdArray.indexOf($scope.id) >= 0) {
			$scope.updateTileParameters();
			if (eventOptions.refresh)
				$scope.refreshTile(false);
		}
	});

	////////////////////////////////////////////////////////
	// scope functions:
	////////////////////////////////////////////////////////

	/**
	 * Initialize tile
	 */
	$scope.initialize = function (tile) {
		// extend scope with tile parameters and default parameters:
		var tileDefaults = $injector.get('tileDefaults');
		angular.extend($scope, tileDefaults, tile);
		// set report name
		$scope.reportNameWithCategory = $scope.reportName;
		if ($scope.reportCategory != null)
			$scope.reportNameWithCategory = $scope.reportCategory + '\\' + $scope.reportNameWithCategory;

		initializeDraggable();
		initializeResizable();

		/**
		 * Change size handler
		 */
		$scope.$watch(['width', 'height'], function () {
			changeTileSizeHandler();
		});

	};

	/**
	 * Refresh tile handler
	 */
	$scope.refreshTile = function (updateFromSourceReport) {
		refreshTile(updateFromSourceReport);
	};

	/**
	 * Refresh tile parameters
	 */
	$scope.updateTileParameters = function () {
		var $tile = $scope.$parent.getTile$ById($scope.id);
		var x = Math.round($tile.position().left / $scope.$parent.tileWidth),
			y = Math.round($tile.position().top / $scope.$parent.tileHeight),
			width = Math.round($tile.width() / $scope.$parent.tileWidth),
			height = Math.round($tile.height() / $scope.$parent.tileHeight);
		$scope.x = x > 0 ? x : 0;
		$scope.y = y > 0 ? y : 0;
		$scope.width = width;
		$scope.height = height;
	};

	/**
	 * Flip tile back
	 */
	$scope.flipBack = function() {
		flipTileBack();
	};

	/**
	 * Flip tile front
	 */
	$scope.flipFront = function (update, updateFromSourceReport) {
		flipTileFront(update, updateFromSourceReport);
	};

	/**
	 * Clear tile content
	 */
	$scope.clearContent = function() {
		clearTileContent();
	};

	/**
	 * Set scroll to tile.
	 */
	$scope.setScroll = function() {
		var $tile = angular.element($element);
		$tile.find('.report').css('overflow', 'hidden');
		var $front = $tile.find('.flippy-front .frame .report');
		if ($scope.top != -999) {
			if ($front.hasClass('ps-container')) {
				$front.perfectScrollbar('update');
			} else {
				$front.perfectScrollbar();
			}
		} else {
			if ($front.hasClass('ps-container')) {
				$front.perfectScrollbar('destroy');
			}
		}
		// add back scroll
		var $back = $tile.find('.flippy-back .frame');
		if ($back.hasClass('ps-container')) {
			$back.perfectScrollbar('update');
		} else {
			$back.perfectScrollbar();
		}
	};

	////////////////////////////////////////////////////////
	// tile functions:
	////////////////////////////////////////////////////////

	/**
	 * initialize draggable for tile
	 */
	function initializeDraggable() {
		$element.draggable({
			stack: '.iz-dash-tile',
			handle: '.title-container',
			helper: function (event, ui) {
				var $target = angular.element(event.currentTarget);
				var helperStr =
					'<div class="iz-dash-tile iz-dash-tile-helper" style="top: 0px; height: ' + $target.height() + 'px; left: 0px; width: ' + $target.width() + 'px; opacity: 1; transform: matrix(1, 0, 0, 1, 0, 0); z-index: 1000;">' +
					'<div class="animate-flip">' +
					'<div class="flippy-front animated fast">' +
						'<div class="title-container" style="height: 35px; overflow: hidden;"><div class="title"><span class="title-text">' +
						'</span></div></div>' +
					'</div></div></div>';
				return angular.element(helperStr);
			},
			distance: 10,
			start: function (event, ui) {
				$rootScope.$broadcast('startEditTileEvent', [{
					tileId: $scope.id,
					actionName: 'drag'
				}]);

				var $target = angular.element(event.target),
					$helper = ui.helper,
					targetPos = $target.position(),
					helperPos = $helper.position(),
					targetWidth = $target.width(),
					targetHeight = $target.height();
				$helper.find('.flippy-front, .flippy-back').removeClass('flipInY');
				$helper.find('.flippy-front, .flippy-back').css('background-color', 'rgba(50,205,50, 0.3)');
				$helper.find('.frame').remove();
				$helper.css('z-index', 1000);
				$helper.css('opacity', 1);
			},
			drag: function (event, ui) {
				var $flippies = $scope.$parent.getTileContainer().find('.animate-flip > .flippy-front, .animate-flip > .flippy-back');
				var $helper = ui.helper;
				var helperPos = $helper.position();

				// move tile shadow
				var x = Math.round(helperPos.left / $scope.$parent.tileWidth) * $scope.$parent.tileWidth;
				var y = Math.round(helperPos.top / $scope.$parent.tileHeight) * $scope.$parent.tileHeight;
				$scope.$parent.showTileGridShadow({
					x: x,
					y: y,
					width: $helper.width(),
					height: $helper.height()
				}, false);

				// check underlying tile
				$flippies.css('background-color', '#fff');
				$helper.find('.flippy-front, .flippy-back').css('background-color', 'rgba(50,205,50, 0.3)');
				var $target = $scope.$parent.getUnderlyingTile(event.pageX, event.pageY, $scope);
				if ($target != null) {
					$scope.$parent.hideTileGridShadow();
					$target.find('.flippy-front, .flippy-back').css('background-color', 'rgba(50,205,50, 1)');
				} else {
					if ($scope.$parent.checkTileIntersects($scope, $helper) || $scope.$parent.checkTileMovedToOuterSpace($helper, 10)) {
						$helper.find('.flippy-front, .flippy-back').css('background-color', 'rgba(220,20,60,0.2)');
					}
				}
			},
			stop: function (event, ui) {
				var $flippies = $scope.$parent.getTileContainer().find('.animate-flip > .flippy-front, .animate-flip > .flippy-back');
				var $helper = ui.helper;
				var $source = angular.element(event.target);
				var $target = $scope.$parent.getUnderlyingTile(event.pageX, event.pageY, $scope);
				$flippies.css('background-color', '#fff');

				// swap tile:
				if ($target != null) {
					$scope.$parent.swapTiles($source, $target).then(function (swappedTiles) {
						var $swappedTile1 = swappedTiles[0],
						    $swappedTile2 = swappedTiles[1];
						$swappedTile1.find('.frame').show();
						$swappedTile2.find('.frame').show();
						var tileSizeChanged = Math.abs($swappedTile1.width() - $swappedTile2.width()) > 5
									|| Math.abs($swappedTile1.height() - $swappedTile2.height()) > 5;
						if (tileSizeChanged) {
							var id1 = $scope.$parent.getTile$Id($swappedTile1),
							    id2 = $scope.$parent.getTile$Id($swappedTile2);
							$rootScope.$broadcast('stopEditTileEvent', [{
								tileId: [id1, id2],
								refresh: true,
								actionName: 'drag'
							}]);
						}
					});
					return;
				}

				// cancel drag if have intersections or tile is out of dashboard space:
				if ($scope.$parent.checkTileIntersects($scope, $helper) || $scope.$parent.checkTileMovedToOuterSpace($helper, 10)) {
					$rootScope.$broadcast('stopEditTileEvent', [{
						tileId: $scope.id,
						refresh: false,
						actionName: 'drag'
					}]);
					return;
				}
				
				// move tile to new location:
				var pos = $helper.position();
				var $t = $scope.$parent.getTile$ByInnerEl($source);
				$t.animate({
				    left: Math.round(pos.left / $scope.$parent.tileWidth) * $scope.$parent.tileWidth,
				    top: Math.round(pos.top / $scope.$parent.tileHeight) * $scope.$parent.tileHeight
				}, 500, function () {
					$rootScope.$broadcast('stopEditTileEvent', [{
						tileId: $scope.id,
						refresh: false,
						actionName: 'drag'
					}]);
				});
			}
		});
	}

	/**
	 * Initialize resizable handler
	 */
	function initializeResizable() {
		var $animates = $scope.$parent.getTileContainer().find('.animate-flip');
		$element.resizable({
			grid: [$scope.$parent.tileWidth, $scope.$parent.tileHeight],
			containment: 'parent',
			handles: 'n, e, s, w, se',
			start: function (event, ui) {
				$rootScope.$broadcast('startEditTileEvent', [{
					tileId: $scope.id,
					actionName: 'resize'
				}]);
				$animates = $scope.$parent.getTileContainer().find('.animate-flip');
				var $target = angular.element(event.target);
				$target.find('.flippy-front, .flippy-back').removeClass('flipInY');
				$target.find('.flippy-front, .flippy-back').css('background-color', 'rgba(50,205,50, 0.3)');
				$target.find('.frame').addClass('hidden');
				$target.css('z-index', 1000);
				$target.css('opacity', 1);
			},
			resize: function(event, ui) {
				var $currentTileUi = ui.element;
				var $t = $scope.$parent.getTile$ByInnerEl($currentTileUi);
				var tile = $scope.$parent.getTileByTile$($t);

				$animates.find('.flippy-front,.flippy-back').css('background-color', '#fff');
				$t.find('.flippy-front,.flippy-back').css('background-color', 'rgba(50,205,50, 0.5)');
				if ($scope.$parent.checkTileIntersects(tile)) {
					$t.find('.flippy-front,.flippy-back').css('background-color', 'rgba(220,20,60,0.5)');
				}
			},
			stop: function (event, ui) {
				var $currentTileUi = ui.element;
				var $t = $scope.$parent.getTile$ByInnerEl($currentTileUi);
				var tile = $scope.$parent.getTileByTile$($t);
				$t.css('z-index', 1);
				$t.find('.frame').removeClass('hidden');
				$t.find('.flippy-front, .flippy-back').addClass('flipInY');
				$animates.find('.flippy-front,.flippy-back').css('background-color', '#fff');
				if ($scope.$parent.checkTileIntersects(tile) || $scope.$parent.checkTileMovedToOuterSpace($t)) {
					// revert if intersects
					$currentTileUi.animate({
						left: ui.originalPosition.left,
						top: ui.originalPosition.top,
						width: ui.originalSize.width,
						height: ui.originalSize.height
					}, 200, function () {
						// no need to update tile:
						$rootScope.$broadcast('stopEditTileEvent', [{
							actionName: 'resize'
						}]);
					});
				} else {
					$rootScope.$broadcast('stopEditTileEvent', [{
						tileId: $scope.id,
						actionName: 'resize',
						refresh: true
					}]);
				}
				$t.find('.flippy-front .report, .flippy-back .report').removeClass('hidden');
				$t.css('opacity', 1);
			}
		});
		$scope.state.resizableHandlerStarted = true;
	}

    /**
	 * Tile changed handler
	 */
	function changeTileSizeHandler() {
		refreshTile(false);
	}

	/**
	 * Flip tile to the front side and refresh if needed.
	 */
	function flipTileFront(update, updateFromSourceReport) {
		var $tile = angular.element($element);
		var showClass = 'animated fast flipInY';
		var hideClass = 'animated fast flipOutY';
		var $front = $tile.find('.flippy-front');
		var $back = $front.parent().find('.flippy-back');
		$back.addClass(hideClass);
		$front.removeClass(showClass);
		//$tile.find('.frame').css('overflow-y', 'hidden');
		//$tile.find('.ui-icon-gripsmall-diagonal-se').hide();
		setTimeout(function () {
			$front.css('display', 'block').addClass(showClass);
			$back.css('display', 'none').removeClass(hideClass);
		}, 1);

		if (update) {
			refreshTile(updateFromSourceReport);
		}

		/*setTimeout(function () {
			$tile.find('.ui-icon-gripsmall-diagonal-se').show();
			if (typeof additionalCallback != 'undefined')
				additionalCallback.apply(_this, [tile]);
		}, 200);*/
	}

	/**
	 * Flip tile to back side
	 */
	function flipTileBack() {
		var $tile = angular.element($element);
		//this.removeScroll($tile);
		var showClass = 'animated fast flipInY';
		var hideClass = 'animated fast flipOutY';
		var $front = $tile.find('.flippy-front');
		var $back = $tile.find('.flippy-back');
		$front.addClass(hideClass);
		$back.removeClass(showClass);
		//$tile.find('.frame').css('overflow-y', 'hidden');
		//$tile.find('.ui-icon-gripsmall-diagonal-se').hide();
		setTimeout(function () {
			$back.css('display', 'block').addClass(showClass);
			$front.css('display', 'none').removeClass(hideClass);
		}, 1);
		//setTimeout(function () {
		//	$tile.find('.ui-icon-gripsmall-diagonal-se').show();
		//	$tile.find('.frame').css('overflow-y', 'auto');
		//}, 200);
	}

	/**
	 * Clear tile content
	 */
	function clearTileContent() {
		var $body = angular.element($element).find('.report');
		$body.empty();
	}

	/**
	 * Refresh tile content
	 */
	function refreshTile(updateFromSourceReport) {
		var loadingHtml = '<div class="iz-dash-tile-vcentered-container">' +
			'<div class="iz-dash-tile-vcentered-item">' +
			'<img class="img-responsive" src="' + $izendaUrl.urlSettings.urlRsPage + '?image=ModernImages.loading-grid.gif" alt="Loading..." />' +
			'</div>' +
			'</div>';
		var $body = angular.element($element).find('.report');
		$body.html(loadingHtml);

		if ($scope.preloadStarted) {
			$scope.preloadDataHandler.then(function(htmlData) {
				applyTileHtml(htmlData);
			});
		} else {
			if ($scope.preloadData !== null) {
				console.log('!!! preloadData used');
				applyTileHtml($scope.preloadData);
			} else {
				$izendaDashboardQuery.loadTileReport(updateFromSourceReport, $izendaUrl.getReportInfo().fullName, $scope.reportFullName, $scope.top,
							($scope.width * $scope.$parent.tileWidth) - 20, ($scope.height * $scope.$parent.tileHeight) - 80)
				.then(function (htmlData) {
					applyTileHtml(htmlData);
				});
			}
		}
	}

	/**
	 * Set tile inner html
	 */
	function applyTileHtml(htmlData) {
		clearTileContent();
		var $b = angular.element($element).find('.report');
		if (!angular.isUndefined(ReportScripting))
			ReportScripting.loadReportResponse(htmlData, $b);
		var divs$ = $b.find('div.DashPartBody, div.DashPartBodyNoScroll');
		var $zerochartResults = divs$.find('.iz-zero-chart-results');
		if ($zerochartResults.length > 0) {
			$zerochartResults.closest('table').css('height', '100%');
			divs$.css('height', '100%');
		}
		if (!angular.isUndefined(AdHoc) && !angular.isUndefined(AdHoc.Utility) && typeof (AdHoc.Utility.DocumentReady) == 'function') {
			AdHoc.Utility.DocumentReady();
		}
		$scope.setScroll();
	}
}]);