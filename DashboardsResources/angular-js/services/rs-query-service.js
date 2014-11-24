/**
 * Izenda query service which provides access to rs.aspx
 * this is singleton
 */
angular.module('izendaQuery').factory('$izendaRsQuery', ['$http', '$q', '$izendaUrl', function ($http, $q, $izendaUrl) {
	'use strict';

	var rsQueryBaseUrl = $izendaUrl.urlSettings.urlRsPage;

	var rsQueryLog = {};

	var requestList = {};

	// PUBLIC API:
	return {
		cancelAllQueries: cancelAllQueries,
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
				if (wsArgs[i] != null)
					wsArgsString += '&wsArg' + i + '=' + encodeURIComponent(wsArgs[i]);
				else
					wsArgsString += '&wsArg' + i + '=';
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

		// uncomment if want to trace queries
		//console.log('>>> ' + url);
		rsQueryLog[url] = new Date();

		// make request
		var request = $http({
			method: 'get',
			url: url,
			responseType: queryOptions.dataType
		});
		requestList[url] = request;
		return request.then(handleSuccess, handleError);
	}

	/**
	 * Cancel all running queries
	 */
	function cancelAllQueries(message) {
		/*for (var requestId in requestList) {
			if (requestList.hasOwnProperty(requestId)) {
				if (requestList[requestId] != null)
					requestList[requestId].cancel(message);
			}
		}*/
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
		delete requestList[response.config.url];
		console.log('<<< ' + ((new Date()).getTime() - rsQueryLog[response.config.url].getTime()) + 'ms: ' + response.config.url);
		if (typeof(response.data) == 'string') {
			return response.data;
		}
		return (response.data);
	}
}]);