/**
 * Izenda query service which provides toolbar specific queries
 * this is singleton
 */
izendaQueryModule.factory('$izendaDashboardToolbarQuery', ['$izendaRsQuery', function ($izendaRsQuery) {
  'use strict';

  // ========================================
  // PUBLIC API
  // ========================================
  return {
    loadDashboardNavigation: loadDashboardNavigation
  };

  function loadDashboardNavigation() {
    return $izendaRsQuery.query('getdashboardcategories', [], {
      dataType: 'json'
    });
  }

}]);