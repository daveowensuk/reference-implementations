/**
 * Izenda query service which provides access to rs.aspx
 * this is singleton
 */
izendaQueryModule.factory('$izendaRsQuery', ['$http', '$q', '$izendaUrl', function ($http, $q, $izendaUrl) {
  'use strict';

  var rsQueryBaseUrl = $izendaUrl.urlSettings.urlRsPage;

  var rsQueryLog = {};

  var requestList = [];

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
    var request = angular.element.ajax({
      method: 'get',
      url: url,
      context: {
        url: url
      },
      responseType: queryOptions.dataType
    });
    requestList.push({
      url: url,
      request: request
    });
    return request.then(handleSuccess, handleError);
  }

  /**
   * Remove request from array
   */
  function removeRequest(url) {
    var foundIndex = -1;
    var i = 0;
    while (foundIndex < 0 && i < requestList.length) {
      if (requestList[i].url === url) {
        foundIndex = i;
      }
      i++;
    }
    if (foundIndex >= 0) {
      requestList.splice(foundIndex, 1);
    }
  }

  /**
   * Cancel all running queries
   */
  function cancelAllQueries(options) {
    var opts = options || {};
    var count = requestList.length;
    var i = 0;
    while (i < requestList.length) {
      var request = requestList[0];
      var cancel = true;
      if (opts.hasOwnProperty('ignoreList')) {
        var ignoreList = opts['ignoreList'];
        for (var j = 0; j < ignoreList.length; j++) {
          if (request.url.indexOf(ignoreList[j]) >= 0) {
            cancel = false;
          }
        }
      }
      if (cancel) {
        console.log('<<< (cancelled!) ' + ((new Date()).getTime() - rsQueryLog[request.url].getTime()) + 'ms: ' + request.url);
        request.request.abort();
        requestList.splice(0, 1);
      } else {
        i++;
      }
    }
    return count - i;
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

  function handleSuccess(response, state, params) {
    var currentUrl = this.url;
    console.log('<<< ' + ((new Date()).getTime() - rsQueryLog[currentUrl].getTime()) + 'ms: ' + currentUrl);
    removeRequest(currentUrl);
    if (typeof (response.data) == 'string') {
      return response.data;
    }
    return (response.data);
  }
}]);