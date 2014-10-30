/**
 * Izenda query service which provides access to rs.aspx
 * this is singleton
 */
angular.module('izendaQuery').factory('$izendaRsQuery', ['$http', '$q', '$izendaUrl', function ($http, $q, $izendaUrl) {
	'use strict';

	var rsQueryBaseUrl = $izendaUrl.urlSettings.urlRsPage;

	// PUBLIC API:
	return {
		query: query
	};

	/**
	 * Make query to rs.aspx
	 */
	function query(wsCmd, wsArgs, options) {
		if (!angular.isString(wsCmd) || wsCmd.trim() == '')
			throw 'wsCmd parameter should be not empty string.';
		// apply wsCmd query parameter
		var url = rsQueryBaseUrl + '?wscmd=' + encodeURIComponent(wsCmd);

		// apply wsArgs query parameters
		var wsArgsString = '';
		if (!angular.isUndefined(wsArgs)) {
			if (!angular.isArray(wsArgs))
				throw 'wsArgs parameter should be array.';
			for (var i = 0; i < wsArgs.length; i++) {
				wsArgsString += '&wsArg' + i + '=' + encodeURIComponent(wsArgs[i]);
			}
		} else {
			throw 'wsArgs parameter should be defined.';
		}
		url += wsArgsString;

		// apply additional options
		var queryOptions = {
			dataType: 'text'
		};
		if (angular.isObject(options)) {
			angular.extend(queryOptions, options);
		}

		console.log('>>> ' + url);

		// make request
		var request = $http({
			method: 'get',
			url: url,
			responseType: queryOptions.dataType
		});
		return request.then(handleSuccess, handleError);
	}

	// ========================================
	// PRIVATE
	// ========================================

	function handleError(response) {
		if (!angular.isObject(response.data) || !response.data.message) {
			return ($q.reject("An unknown error occurred."));
		}
		return ($q.reject(response.data.message));
	}

	function handleSuccess(response) {
		if (typeof(response.data) == 'string') {
			return response.data;
		}
		return (response.data);
	}
}]);