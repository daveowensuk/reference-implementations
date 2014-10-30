/**
 * Izenda query service which provides dashboard specific queries
 * this is singleton
 */
angular.module('izendaQuery').factory('$izendaCommonQuery', ['$izendaRsQuery', function ($izendaRsQuery) {
	'use strict';

	// PUBLIC API
	return {
		checkReportSetExist: checkReportSetExist
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

}]);