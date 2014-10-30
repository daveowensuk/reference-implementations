angular.module('izendaDashboard').constant('tileDefaults', {
	id: null,
	reportFullName: null,
	reportPartName: null,
	reportSetName: null,
	reportName: null,
	reportCategory: null,
	x: 0,
	y: 0,
	width: 1,
	height: 1,
	top: 100
});

angular.module('izendaDashboard').controller('IzendaTileController', ['$element', '$scope', '$injector', '$izendaUrl', '$izendaDashboardQuery',
function IzendaTileController($element, $scope, $injector, $izendaUrl, $izendaDashboardQuery) {
	'use strict';

	$scope.isHidden = false;

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
	 * Initialize tile
	 */
	$scope.initialize = function (tile) {
		// extend scope with tile parameters and default parameters:
		var tileDefaults = $injector.get('tileDefaults');
		angular.extend($scope, tileDefaults, tile);

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

	////////////////////////////////////////////////////////
	// tile handlers:
	////////////////////////////////////////////////////////

	/**
	 * Tile changed handler
	 */
	function changeTileSizeHandler() {
		refreshTile(false);
	}

	////////////////////////////////////////////////////////
	// tile functions:
	////////////////////////////////////////////////////////

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
		var $body = angular.element($element).find('.report');
		$body.empty();
		var loadingHtml = '<div class="iz-dash-tile-vcentered-container">' +
			'<div class="iz-dash-tile-vcentered-item">' +
			'<img class="img-responsive" src="' + $izendaUrl.urlSettings.urlRsPage + '?image=ModernImages.loading-grid.gif" alt="Loading..." />' +
			'</div>' +
			'</div>';
		$body.html(loadingHtml);
		$izendaDashboardQuery
			.loadTileReport(updateFromSourceReport, $izendaUrl.getReportInfo().fullName, $scope.reportFullName, $scope.top,
						$scope.width * $scope.$parent.tileWidth - 20, $scope.height * $scope.$parent.tileHeight - 65)
			.then(function (htmlData) {
				clearTileContent();

				var $body = angular.element($element).find('.report');
				ReportScripting.loadReportResponse(htmlData, $body);

				var divs$ = $body.find('div.DashPartBody, div.DashPartBodyNoScroll');
				var $zerochartResults = divs$.find('.iz-zero-chart-results');
				if ($zerochartResults.length > 0) {
					$zerochartResults.closest('table').css('height', '100%');
					divs$.css('height', '100%');
				}
				if (!angular.isUndefined(AdHoc) && !angular.isUndefined(AdHoc.Utility)
					&& typeof (AdHoc.Utility.DocumentReady) == 'function') {
					AdHoc.Utility.DocumentReady();
				}
		});
	}
}]);