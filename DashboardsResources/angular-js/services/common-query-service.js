/**
 * Izenda query service which provides dashboard specific queries
 * this is singleton
 */
angular.module('izendaQuery').factory('$izendaCommonQuery', ['$izendaRsQuery', function ($izendaRsQuery) {
  'use strict';

  // PUBLIC API
  return {
    checkReportSetExist: checkReportSetExist,
    getReportSetCategory: getReportSetCategory,
    getReportParts: getReportParts
  };

  /**
   * Check report set is exist.
   * returns promise with 'true' value if exists
   */
  function checkReportSetExist(reportSetFullName) {
    return $izendaRsQuery.query('checkreportsetexists', [reportSetFullName], {
      dataType: 'text'
    });
  }

  function getReportSetCategory(category) {
    return $izendaRsQuery.query('reportlistdatalite', [category.toLowerCase() == 'uncategorized' ? '' : category], {
      dataType: 'json'
    });
  }

  function getReportParts(reportFullName) {
    return $izendaRsQuery.query('reportdata', [reportFullName], {
      dataType: 'json'
    });
  }
}]);