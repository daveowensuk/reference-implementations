izendaCompatibilityModule.factory('$izendaCompatibility', ['$window', function ($window) {
  'use strict';

  var checkIsIe8 = function () {
    var isSpecificIeVersion = function (version, comparison) {
      var cc = 'IE',
          b = document.createElement('B'),
          docElem = document.documentElement,
          isIeResult;
      if (version) {
        cc += ' ' + version;
        if (comparison) {
          cc = comparison + ' ' + cc;
        }
      }
      b.innerHTML = '<!--[if ' + cc + ']><b id="iecctest"></b><![endif]-->';
      docElem.appendChild(b);
      isIeResult = !!document.getElementById('iecctest');
      docElem.removeChild(b);
      var isCompatibilityMode = (typeof (document.documentMode) !== 'undefined') &&
      ((comparison === 'lte' && document.documentMode <= version)
        || (comparison === 'gte' && document.documentMode >= version)
        || (comparison === 'lt' && document.documentMode < version)
        || (comparison === 'gt' && document.documentMode > version)
        || (comparison === 'eq' && document.documentMode == version));
      return isIeResult || isCompatibilityMode;
    };
    return isSpecificIeVersion(8, 'lte');
  };

  /**
   * Check is dashboard should have mobile view.
   */
  var isMobile = function () {
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
      return true;
    }
    return false;
  };

  /**
   * Check is dashboard window is too small to fit several columns of tiles.
   */
  var isSmallResolution = function () {
    return angular.element($window).width() <= 1024;
  };

  /**
   * Check if one column view required
   */
  var isOneColumnView = function () {
    return isMobile() || isSmallResolution();
  };

  return {
    checkIsIe8: checkIsIe8,
    isMobile: isMobile,
    isSmallResolution: isSmallResolution,
    isOneColumnView: isOneColumnView
  };
}]);