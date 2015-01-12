﻿/**
 * Izenda query service which provides dashboard specific queries
 * this is singleton
 */
izendaQueryModule.factory('$izendaDashboardQuery', ['$izendaRsQuery', function ($izendaRsQuery) {
  'use strict';

  // PUBLIC API
  return {
    loadDashboardLayout: loadDashboardLayout,
    loadTileReport: loadTileReport,
    saveDashboard: saveDashboard
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
  function loadTileReport(updateFromSourceReport, dashboardFullName, reportFullName, reportPreviousFullName, top, contentWidth, contentHeight) {
    return $izendaRsQuery.query(
          updateFromSourceReport ? 'getReportPartPreviewAndUpdate' : 'getReportPartPreview',
          [dashboardFullName, reportFullName, reportPreviousFullName, 1, top, contentWidth, contentHeight, 'forceSize'],
          {
            dataType: 'text',
            headers: {
               'Content-Type': 'text/html'
            }
          }
    );
  }

  /**
   * Save dashboard
   */
  function saveDashboard(dashboardName, dashboardConfigObject) {
    return $izendaRsQuery.query('savecrsdashboard', [JSON.stringify(dashboardConfigObject), dashboardName], {
      dataType: 'text'
    });
  }
}]);