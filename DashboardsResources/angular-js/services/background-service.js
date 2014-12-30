/**
 * Background image/color storage service
 */
izendaQueryModule.factory('$izendaBackground', ['$cookies', function ($cookies) {
  'use strict';

  // PUBLIC API
  return {
    isStorageAvailable: isStorageAvailable,
    setBackgroundImgToStorage: setBackgroundImgToStorage,
    getBackgroundImgFromStorage: getBackgroundImgFromStorage,
    getBackgroundColor: getBackgroundColor
  };

  /**
   * Get background color from cookie for dashboard.
   */
  function getBackgroundColor() {
    var backColor = getCookie('izendaDashboardBackgroundColor');
    return backColor ? backColor : '#1c8fd6';
  }

  function isStorageAvailable() {
    return typeof (Storage) !== 'undefined';
  }

  function setBackgroundImgToStorage(stringValue) {
    if (!isStorageAvailable())
      return false;
    if (stringValue != null)
      localStorage.setItem('izendaDashboardBackgroundImg', stringValue);
    else
      localStorage.removeItem('izendaDashboardBackgroundImg');
    return true;
  }

  /**
   * Get object from storage
   */
  function getBackgroundImgFromStorage() {
    if (!isStorageAvailable())
      return null;
    var dataImage = localStorage.getItem('izendaDashboardBackgroundImg');
    if (!angular.isString(dataImage))
      return null;
    return dataImage;
  }

  /**
   * Get cookie by name
   */
  function getCookie(name) {
    var nameEq = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
      var c = ca[i];
      while (c.charAt(0) == ' ') c = c.substring(1);
      if (c.indexOf(nameEq) != -1) return c.substring(nameEq.length, c.length);
    }
    return null;
  };
}]);