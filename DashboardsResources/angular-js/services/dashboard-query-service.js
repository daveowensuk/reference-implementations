﻿/**
 * Izenda query service which provides dashboard specific queries
 * this is singleton
 */
angular.module('izendaQuery').factory('$izendaDashboardQuery', ['$izendaRsQuery', function ($izendaRsQuery) {
	'use strict';

	// PUBLIC API
	return {
		loadDashboardLayout: loadDashboardLayout,
		loadTileReport: loadTileReport
	};

	/**
	 * Load dashboard tiles
	 */
	function loadDashboardLayout(dashboardFullName) {
		return $izendaRsQuery.query('getReportDashboardConfig', [dashboardFullName], {
			dataType: 'json'
		});
	}

	/**
	 * Load tile report html
	 */
	function loadTileReport(updateFromSourceReport, dashboardFullName, reportFullName, top, contentWidth, contentHeight) {
		return $izendaRsQuery.query(updateFromSourceReport ? 'updateandgetreportpartpreview' : 'getreportpartpreview',
			[dashboardFullName, reportFullName, 1, top, contentWidth, contentHeight, 'forceSize'], {
				dataType: 'text',
				headers: {
					'Content-Type': 'text/html'
				}
		});
	}
}]);