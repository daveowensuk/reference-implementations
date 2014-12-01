angular.module('izendaDashboard').controller('IzendaToolbarController', ['$scope', '$rootScope', '$compile', '$window', '$location', '$cookies', '$izendaRsQuery', '$izendaDashboardToolbarQuery', '$izendaUrl',
function IzendaToolbarController($scope, $rootScope, $compile, $window, $location, $cookies, $izendaRsQuery, $izendaDashboardToolbarQuery, $izendaUrl) {
  'use strict';

  $scope.backgroundColorStyle = {
    'background-color': getCookie('izendaDashboardBackgroundColor') ? getCookie('izendaDashboardBackgroundColor') : '#1c8fd6'
  };
  $scope.izendaBackgroundColor = getCookie('izendaDashboardBackgroundColor') ? getCookie('izendaDashboardBackgroundColor') : '#1c8fd6';
  $scope.izendaBackgroundImageSrc = getImgFromStorage();
  $scope.backgroundFileChangedHandler = function () {
    console.log(arguments);
  };

  $scope.$izendaUrl = $izendaUrl;
  $scope.dashboardCategoriesLoading = true;
  $scope.dashboardCategories = [];
  $scope.dashboardsInCurrentCategory = [];
  $scope.previousDashboardCategory = null;
  $scope.leftDashboards = [];
  $scope.rightDashboards = [];
  $scope.liItems = null;

  // triple bar button styles:
  $scope.buttonbarClass = 'nav navbar-nav iz-dash-toolbtn-panel left-transition';
  $scope.buttonbarCollapsedClass = 'nav navbar-nav iz-dash-collapsed-toolbtn-panel left-transition opened';
  $scope.showButtonBar = function () {
    $scope.buttonbarClass = 'nav navbar-nav iz-dash-toolbtn-panel left-transition opened';
    $scope.buttonbarCollapsedClass = 'nav navbar-nav iz-dash-collapsed-toolbtn-panel left-transition';
    angular.element('#izendaDashboardLinksPanel').fadeOut(200);
  };
  $scope.hideButtonBar = function () {
    $scope.buttonbarClass = 'nav navbar-nav iz-dash-toolbtn-panel left-transition';
    $scope.buttonbarCollapsedClass = 'nav navbar-nav iz-dash-collapsed-toolbtn-panel left-transition opened';
    angular.element('#izendaDashboardLinksPanel').fadeIn(200, function () {
      $scope.updateToolbarItems(true);
    });
  };

  /**
   * Create style object for toolbar item
   */
  $scope.getToolItemStyle = function (dashboard) {
    var parseRgb = function (s) {
      if (!s) return null;
      if (s.indexOf('#') == 0) {
        var r = s.substr(1, 2),
          g = s.substr(3, 2),
          b = s.substr(5, 2);
        return [parseInt(r, 16), parseInt(g, 16), parseInt(b, 16)];
      }
      return null;
    };
    var rgb2hsv = function (colorArray) {
      var rr,
        gg,
        bb,
        r = colorArray[0] / 255,
        g = colorArray[1] / 255,
        b = colorArray[2] / 255,
        h,
        s,
        v = Math.max(r, g, b),
        diff = v - Math.min(r, g, b),
        diffc = function (c) {
          return (v - c) / 6 / diff + 1 / 2;
        };

      if (diff == 0) {
        h = s = 0;
      } else {
        s = diff / v;
        rr = diffc(r);
        gg = diffc(g);
        bb = diffc(b);

        if (r === v) {
          h = bb - gg;
        } else if (g === v) {
          h = (1 / 3) + rr - bb;
        } else if (b === v) {
          h = (2 / 3) + gg - rr;
        }
        if (h < 0) {
          h += 1;
        } else if (h > 1) {
          h -= 1;
        }
      }
      return [Math.round(h * 360), Math.round(s * 100), Math.round(v * 100)];
    };
    var hsv2rgb = function (colorArray) {
      var h = colorArray[0],
        s = colorArray[1],
        v = colorArray[2];
      var r, g, b;
      var i;
      var f, p, q, t;
      h = Math.max(0, Math.min(360, h));
      s = Math.max(0, Math.min(100, s));
      v = Math.max(0, Math.min(100, v));
      s /= 100;
      v /= 100;
      if (s == 0) {
        r = g = b = v;
        return [Math.round(r * 255), Math.round(g * 255), Math.round(b * 255)];
      }
      h /= 60;
      i = Math.floor(h);
      f = h - i;
      p = v * (1 - s);
      q = v * (1 - s * f);
      t = v * (1 - s * (1 - f));
      switch (i) {
        case 0:
          r = v; g = t; b = p; break;
        case 1:
          r = q; g = v; b = p; break;
        case 2:
          r = p; g = v; b = t; break;
        case 3:
          r = p; g = q; b = v; break;
        case 4:
          r = t; g = p; b = v; break;
        default: // case 5:
          r = v; g = p; b = q;
      }
      return [Math.round(r * 255), Math.round(g * 255), Math.round(b * 255)];
    };
    var getContrastHsvColor = function (hsvArray) {
      var s = hsvArray[1];
      var l = hsvArray[2];
      if (l < 80)
        l = 80;
      if (s < 80)
        s = 80;
      return [(hsvArray[0] + 180) % 360, s, l];
    };
    var ensureColorComp = function (colorInt) {
      var v = colorInt.toString(16);
      if (v.length == 1)
        v = '0' + v;
      return v;
    };

    var isActive = dashboard == $izendaUrl.getReportInfo().fullName;
    var style = {
      'border-bottom': '4px solid transparent',
      'background-color': ''
    };
    if (isActive) {
      var c = parseRgb($scope.izendaBackgroundColor);
      var contrast = hsv2rgb(getContrastHsvColor(rgb2hsv(c)));
      var res = '#' + ensureColorComp(contrast[0]) + ensureColorComp(contrast[1]) + ensureColorComp(contrast[2]);
      style['background-color'] = '#f3f3f3';
      style['border-bottom'] = '4px solid ' + res + '';
    }
    return style;
  };

  //////////////////////////////////////////////////////
  // EVENT HANDLERS
  //////////////////////////////////////////////////////

  /**
   * Start initialize dashboard when location was changed or page was loaded
   */
  $scope.$on('$locationChangeSuccess', function () {
    if ($izendaUrl.getReportInfo().fullName == null)
      return;
    setDashboard($izendaUrl.getReportInfo().fullName);
  });

  /**
   * Start tile edit event handler
   */
  $scope.$on('startEditTileEvent', function () {
    $scope.turnOffWindowResizeHandler();
  });

  /**
   * Tile edit completed event handler
   */
  $scope.$on('stopEditTileEvent', function () {
    $scope.turnOnWindowResizeHandler();
  });

  /**
   * Check toggleHueRotateEnabled
   */
  $scope.isToggleHueRotateEnabled = function () {
    var isChrome = /Chrome/.test(navigator.userAgent) && /Google Inc/.test(navigator.vendor);
    var isSafari = /Safari/.test(navigator.userAgent) && /Apple Computer/.test(navigator.vendor);
    return isChrome || isSafari;
  };

  /**
   * Create new dashboard button handler.
   */
  $scope.createNewDashboardHandler = function () {
    $rootScope.$broadcast('dashboardCreateNewEvent', []);
  };

  /**
   * Refresh dashboard button handler.
   */
  $scope.refreshDashboardHandler = function () {
    $rootScope.$broadcast('dashboardRefreshEvent', []);
  };

  /**
   * Save/SaveAS dialog
   */
  $scope.saveDashboardHandler = function (showSaveAsDialog) {
    $rootScope.$broadcast('dashboardSaveEvent', [showSaveAsDialog]);
  };

  /**
   * Turn on window resize handler
   */
  $scope.turnOnWindowResizeHandler = function () {
    angular.element($window).on('resize.dashboard', function () {
      $scope.updateToolbarItems();
      $rootScope.$broadcast('dashboardResizeEvent', [{}]);
    });
  };

  /**
   * Turn off window resize handler
   */
  $scope.turnOffWindowResizeHandler = function () {
    angular.element($window).off('resize.dashboard');
  };

  /**
   * Toggle hue rotate switcher control handler.
   */
  $scope.toggleHueRotateHandler = function () {
    var $hueRotateControl = angular.element('#izendaDashboardHueRotateSwitcher');
    var turnedOn = $hueRotateControl.hasClass('on');
    var text = turnedOn ? 'OFF' : 'ON';
    $hueRotateControl.find('.iz-dash-switcher-text').text(text);
    $hueRotateControl.toggleClass('on');
    resetRotate();
    toggleHueRotate();
  };

  /**
   * Initialize background color picker control.
   */
  $scope.initializeColorPicker = function () {
    var $colorPickerInput = angular.element('#izendaDashboardColorPicker');
    $colorPickerInput.minicolors({
      inline: true,
      control: 'hue',
      change: function (hex) {
        angular.element('.hue-rotate-btn, .iz-dash-background').css('background-color', hex);
        $cookies.izendaBackgroundColor = hex;
        document.cookie = "izendaDashboardBackgroundColor=" + hex;
        $scope.backgroundColorStyle = {
          'background-color': hex
        };
        $scope.izendaBackgroundColor = hex;
      }
    });
    $colorPickerInput.minicolors('value', [$scope.izendaBackgroundColor]);

    // prevent closing dropdown menu:
    angular.element('.dropdown-no-close-on-click.dropdown-menu .minicolors-grid, .dropdown-no-close-on-click.dropdown-menu .minicolors-slider, #izendaDashboardHueRotateSwitcher').click(function (e) {
      e.stopPropagation();
    });
  };

  /**
   * Is tabs shift button hidden
   */
  $scope.hiddenShiftTabs = function (direction) {
    var $linksPanel = angular.element('#izendaDashboardLinksPanel');
    var $ul = $linksPanel.find('ul.iz-dash-nav-tabs');
    if ($ul.children().length == 0)
      return true;

    var isHidden = $scope.isAllToolbarItemsFit();
    if (isHidden)
      return true;

    if (direction > 0) {
      var $first = $ul.children().first();
      return $first.postion().left > 0;
    }
    var $last = $ul.children().last();
    var space = $scope.getToolbarItemsAvailableSpace();
    return space < $last.position().left + $last.width();
  };

  /**
   * Move tabs
   */
  $scope.shiftTabs = function (direction) {
    var $linksPanel = angular.element('#izendaDashboardLinksPanel');
    var $liItems = $linksPanel.find('ul.iz-dash-nav-tabs > li');
    $liItems.each(function (il, l) {
      var $l = angular.element(l);
      var d = $l.data('dashboard');
      var oldRight = $scope.liItems == null ? 0 : $scope.liItems[d];
      var newRight = oldRight + direction * 150;
      if ($scope.liItems == null) $scope.liItems = {};
      $scope.liItems[d] = newRight;
      $l.css('right', newRight);
    });
    $scope.alignToolbarItems($scope.liItems);
  };

  /**
   * Calculate available space for toolbar tabs
   */
  $scope.getToolbarItemsAvailableSpace = function() {
    var $tool = angular.element('#izendaDashboardToolbar');
    return $tool.find('.iz-dash-nav-tabs').width() - $tool.find('.iz-dash-nav-tabs-left').width() - $tool.find('.iz-dash-nav-tabs-right').width();
  };

  /**
   * Calculate overal width of toolbar items.
   */
  $scope.getToolbarItemsWidth = function() {
    var itemsWidth = 0;
    var $linksPanel = angular.element('#izendaDashboardLinksPanel');
    var $liItems = $linksPanel.find('ul.iz-dash-nav-tabs > li');
    $liItems.each(function (iLi, li) {
      var $li = angular.element(li);
      itemsWidth += $li.width();
    });
  };

  /**
   * Is toolbar items requires scrollbar
   */
  $scope.isAllToolbarItemsFit = function() {
    return $scope.getToolbarItemsAvailableSpace() >= $scope.getToolbarItemsWidth();
  };

  /**
   * Align toobar item to right or left if free space is found
   */
  $scope.alignToolbarItems = function(li2right) {
    var $linksPanel = angular.element('#izendaDashboardLinksPanel');
    var $liItems = $linksPanel.find('ul.iz-dash-nav-tabs > li');
    var space = $scope.getToolbarItemsAvailableSpace();

    // check if there is some free space at left:
    var leftDelta = 100000;
    $liItems.each(function (il, l) {
      var $l = angular.element(l);
      var d = $l.data('dashboard');
      var right = li2right[d];
      var left = space - right - $l.width();
      leftDelta = Math.min(leftDelta, left);
    });
    if (leftDelta > 0) {
      // shift left:
      $liItems.each(function (il, l) {
        var $l = angular.element(l);
        var d = $l.data('dashboard');
        var newRight = li2right[d] + leftDelta + 42;
        li2right[d] = newRight;
        $l.css('right', newRight);
      });
    }

    // check if there is some free space at right:
    var rightDelta = 100000;
    $liItems.each(function (il, l) {
      var $l = angular.element(l);
      var d = $l.data('dashboard');
      var right = li2right[d];
      rightDelta = Math.min(rightDelta, right);
    });
    if (rightDelta > 0) {
      // shift right:
      $liItems.each(function (il, l) {
        var $l = angular.element(l);
        var d = $l.data('dashboard');
        var newRight = li2right[d] - rightDelta + 42;
        li2right[d] = newRight;
        $l.css('right', newRight);
      });
    }
  };

  /**
   * Update toolbar dashboard tabs.
   */
  $scope.updateToolbarItems = function (updateActiveDashboard) {

    // update tab style
    var updateItemsUi = function () {
      var $lp = angular.element('#izendaDashboardLinksPanel');
      var $liItems = $lp.find('ul.iz-dash-nav-tabs > li');
      $liItems.each(function (iLi, li) {
        var $li = angular.element(li);
        var d = $li.data('dashboard');
        $li.css($scope.getToolItemStyle(d));
      });
    };

    // move tab to it's place
    var moveItemsToItsPlace = function () {
      var li2right = {};
      var $lp = angular.element('#izendaDashboardLinksPanel');
      var $liItems = $lp.find('ul.iz-dash-nav-tabs > li');

      // move items to initial place
      var currentRight = 42;
      for (var iLi = $liItems.length - 1; iLi >= 0; iLi--) {
        var $li = angular.element($liItems[iLi]);
        var dash = $li.data('dashboard');
        $li.css('right', currentRight);
        li2right[dash] = currentRight;
        currentRight += $li.width();
      };

      var hasHiddenItems = !$scope.isAllToolbarItemsFit();
      // if scroll needed:
      if (hasHiddenItems) {
        var activeIndex = 0;
        $liItems.each(function (il, l) {
          var $l = angular.element(l);
          if ($l.data('dashboard') == $izendaUrl.getReportInfo().fullName)
            activeIndex = il;
        });

        var space = $scope.getToolbarItemsAvailableSpace();
        var center = space / 2;
        var $activeLi = angular.element($liItems[activeIndex]);
        if ($activeLi.length == 0)
          return li2right;
        var activeLiRight = li2right[$activeLi.data('dashboard')];
        var delta = activeLiRight - (center - $activeLi.width() / 2);
        $liItems.each(function (il, l) {
          var $l = angular.element(l);
          var d = $l.data('dashboard');
          var newRight = li2right[d] - delta;
          li2right[d] = newRight;
          $l.css('right', newRight);
        });

        // align items to left or right
        $scope.alignToolbarItems(li2right);
      }
      return li2right;
    };
    
    // set current dashboard menu items
    var $linksPanel = angular.element('#izendaDashboardLinksPanel');
    var $ul = $linksPanel.find('ul.iz-dash-nav-tabs');
    if (updateActiveDashboard) {
      $ul.empty();
    }
    // create items if empty
    if ($ul.children('li').length == 0) {
      for (var i = 0; i < $scope.dashboardCategories.length; i++) {
        if ($scope.dashboardCategories[i].name == $izendaUrl.getReportInfo().category) {
          var dashboards = $scope.dashboardCategories[i].dashboards;
          $ul.empty();
          for (var j = 0; j < dashboards.length; j++) {
            var dashboard = dashboards[j];
            var $dashboardLi = angular.element('<li class="iz-dash-menu-item">' +
                '<a href="#' + dashboard + '">' + $izendaUrl.extractReportName(dashboard) + '</a>' +
                '</li>')
              .css($scope.getToolItemStyle(dashboard))
              .css('position', 'absolute')
              .data('dashboard', dashboard);
            $ul.append($dashboardLi);
          }
        }
      }
    }
    $scope.liItems = moveItemsToItsPlace($linksPanel);
    updateItemsUi();
  };

  //////////////////////////////////////////////////////
  // CONSTRUCTOR
  //////////////////////////////////////////////////////

  // initialize dashboard method
  initialize();

  //////////////////////////////////////////////////////
  // PRIVATE
  //////////////////////////////////////////////////////

  function setImgToStorage(img) {
    var imgData = getBase64Image(img);
    localStorage.setItem('izendaDashboardBackgroundImg', imgData);
  }

  function getImgFromStorage() {
    var dataImage = localStorage.getItem('izendaDashboardBackgroundImg');
    return 'data:image/png;base64,' + dataImage;
  }

  function getBase64Image(img) {
    // Create an empty canvas element
    var canvas = document.createElement("canvas");
    canvas.width = img.width;
    canvas.height = img.height;

    // Copy the image contents to the canvas
    var ctx = canvas.getContext("2d");
    ctx.drawImage(img, 0, 0);

    // Get the data-URL formatted image
    // Firefox supports PNG and JPEG. You could check img.src to guess the
    // original format, but be aware the using "image/jpg" will re-encode the image.
    var dataURL = canvas.toDataURL("image/png");

    return dataURL.replace(/^data:image\/(png|jpg);base64,/, "");
  }

  function getCookie(name) {
    var nameEq = name + "=";
    //alert(document.cookie);
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
      var c = ca[i];
      while (c.charAt(0) == ' ') c = c.substring(1);
      if (c.indexOf(nameEq) != -1) return c.substring(nameEq.length, c.length);
    }
    return null;
  }

  function resetRotate() {
    var e = angular.element('.iz-dash-background');
    e.css({ 'filter': 'hue-rotate(' + '0' + 'deg)' });
    e.css({ '-webkit-filter': 'hue-rotate(' + '0' + 'deg)' });
    e.css({ '-moz-filter': 'hue-rotate(' + '0' + 'deg)' });
    e.css({ '-o-filter': 'hue-rotate(' + '0' + 'deg)' });
    e.css({ '-ms-filter': 'hue-rotate(' + '0' + 'deg)' });
  }

  /**
   * Turn off/on hue rotate effect handler
   */
  function toggleHueRotate() {
    if (window.hueRotateTimeOut == null) {
      angular.element('.hue-rotate-btn').children('img').attr('src', 'DashboardsResources/images/color.png');
      var e = angular.element('.iz-dash-background');
      if (window.chrome) {
        rotate(e);
      }
    } else {
      angular.element('.hue-rotate-btn').children('img').attr('src', 'DashboardsResources/images/color-bw.png');
      clearTimeout(hueRotateTimeOut);
      window.hueRotateTimeOut = null;
    }
  }

  /**
   * Initialize dashboard navigation
   */
  function initialize() {
    if (angular.element('body > .iz-dash-background').length == 0) {
      angular.element('body').prepend(angular.element('<div class="iz-dash-background" style="background-color: ' +
        $scope.backgroundColorStyle['background-color'] + '"></div>'));
    }


    // start window resize handler
    $scope.turnOnWindowResizeHandler();

    // load dashboard navigation
    $izendaDashboardToolbarQuery.loadDashboardNavigation().then(function (data) {
      dashboardNavigationLoaded(data);
    });

    $scope.initializeColorPicker();
  }

  /**
   * Dashboard categories loaded. Now we have to update it.
   */
  function dashboardNavigationLoaded(data) {
    $scope.dashboardCategoriesLoading = false;
    $scope.dashboardCategories.length = 0;
    if (angular.isObject(data)) {
      for (var category in data) {
        var dashboards = data[category];
        if (dashboards.length > 0) {
          var item = {
            id: (new Date()).getTime(),
            name: category,
            dashboards: data[category]
          };
          $scope.dashboardCategories.push(item);
        }
      }
      $scope.updateToolbarItems();
    }

    // check dashboard parameter is defined
    var fullName = $scope.$izendaUrl.getReportInfo().fullName;
    if (angular.isUndefined(fullName) || fullName == null) {
      // go to the first found dashboard, if parameter isn't defined
      if ($scope.dashboardCategories.length > 0) {
        // update url
        fullName = $scope.dashboardCategories[0].dashboards[0];
        $izendaUrl.setReportFullName(fullName);
      } else {
        throw 'There is no dashboards to load.';
      }
    }
  }

  /**
   * Set current dashboard
   */
  function setDashboard(dashboardFullName) {
    console.log(' ');
    console.log('>>>>> Set current dashboard: "' + dashboardFullName + '"');
    console.log(' ');

    // check if category was changed
    var newCat = $izendaUrl.extractReportCategory(dashboardFullName);
    var prevCat = $scope.previousDashboardCategory;
    if (prevCat == null)
      prevCat = newCat;
    var isCategoryChanged = prevCat !== newCat;
    $scope.previousDashboardCategory = newCat;

    // cancel all current queries
    $izendaRsQuery.cancelAllQueries('Starting load next dashboard.');

    angular.element('.iz-dash-tile').css('display', 'none');

    // notify dashboard to start
    $rootScope.$broadcast('dashboardSetEvent', [{}]);

    // update toolbar items
    $scope.updateToolbarItems(isCategoryChanged);
  }
}]);
