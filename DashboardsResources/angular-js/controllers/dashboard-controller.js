izendaDashboardModule.controller('IzendaDashboardController', ['$rootScope', '$scope', '$window', '$q', '$location', '$animate', '$injector', '$izendaBackground', '$izendaUrl', '$izendaCompatibility', '$izendaDashboardQuery', '$izendaRsQuery',
function IzendaDashboardController($rootScope, $scope, $window, $q, $location, $animate, $injector, $izendaBackground, $izendaUrl, $izendaCompatibility, $izendaDashboardQuery, $izendaRsQuery) {
  'use strict';

  var _ = angular.element;

  var newTileIndex = 1;

  $scope.izendaUrl = $izendaUrl;

  // dashboard tiles container
  $scope.tileContainerStyle = {
    'height': 0
  };

  $scope.galleryContainerStyle = {
    'height': 0,
    'top': '20px'
  };

  // is dashboard changing now.
  $scope.isChangingNow = false;

  $scope.isGalleryMode = false;
  $scope.isFullscreenMode = false;
  $scope.galleryTileIndex = 0;
  $scope.galleryTile = null;
  $scope.galleryTileTitle = null;

  // dashboard tiles
  $scope.tiles = [];
  $scope.tileWidth = 0;
  $scope.tileHeight = 0;

  // dashboard notifications:
  $scope.notificationsIdCounter = 1;
  $scope.notifications = [];

  ////////////////////////////////////////////////////////
  // scope helper functions:
  ////////////////////////////////////////////////////////

  $scope.getRoot = function () {
    return _('#dashboardsDiv');
  };
  $scope.getTileContainer = function () {
    return _('#dashboardBodyContainer');
  };
  $scope.getGalleryContainer = function () {
    return _('#galleryBodyContainer');
  };
  $scope.getTileById = function (tileId) {
    for (var i = 0; i < $scope.tiles.length; i++) {
      if ($scope.tiles[i].id == tileId)
        return $scope.tiles[i];
    }
    return null;
  };
  $scope.getTileIndex = function(tileid) {
    for (var i = 0; i < $scope.tiles.length; i++) {
      if ($scope.tiles[i].id == tileId)
        return i;
    }
    return -1;
  };
  $scope.getTileByTile$ = function (tile$) {
    return $scope.getTileById($scope.getTile$Id(tile$));
  };
  $scope.getOtherTiles = function (tiles, tile) {
    if (tile == null)
      return tiles;
    var otherTiles = [];
    for (var i = 0; i < tiles.length; i++) {
      if (tiles[i].id != tile.id)
        otherTiles.push(tiles[i]);
    }
    return otherTiles;
  };
  $scope.getTile$ById = function (tileId) {
    return $scope.getTileContainer().find('.iz-dash-tile[tileid="' + tileId + '"]');
  };
  $scope.getTile$Id = function ($tile) {
    return $tile.attr('tileid');
  };
  $scope.getTile$ByInnerEl = function (el) {
    var $el = _(el);
    return _($el.closest('.iz-dash-tile'));
  };
  $scope.isTile$ = function (el) {
    var $el = _(el);
    return $el.closest('.iz-dash-tile').length > 0;
  };
  $scope.getTilePositionIndex = function (tileId) {
    var position = 0;
    var currentTile = $scope.getTileById(tileId);
    for (var i = 0; i < $scope.tiles.length; i++) {
      var tile = $scope.tiles[i];
      if (tile.y < currentTile.y)
        position++;
      if (tile.y == currentTile.y && tile.x < currentTile.x)
        position++;
    }
    return position;
  };

  ////////////////////////////////////////////////////////
  // event handlers:
  ////////////////////////////////////////////////////////

  /**
   * Dashboard window resized handler
   */
  $scope.$on('dashboardResizeEvent', function () {
    // update dashboard tile sizes
    $scope.updateDashboardSize();
    updateGalleryContainer();
    $scope.updateDashboardHandlers();
  });

  /**
   * Listen dashboard "save/save as" event
   */
  $scope.$on('dashboardSaveEvent', function (event, args) {
    if (args[0]) {
      $rootScope.$broadcast('openSelectReportNameModalEvent', []);
    } else {
      var dashboardName = $izendaUrl.getReportInfo().name,
          dashboardCategory = $izendaUrl.getReportInfo().category;
      save(dashboardName, dashboardCategory);
    }
  });

  /**
   * Save dashboard as (create dashboard copy). Fires when user selects name of new dashboard in IzendaSelectReportNameController.
   */
  $scope.$on('selectedReportNameEvent', function (event, args) {
    var dashboardName = args[0],
        dashboardCategory = args[1];
    save(dashboardName, dashboardCategory);
  });

  /**
   * Listen 'dashboardSetEvent' event. Initialize dashboard when it is fired.
   */
  $scope.$on('dashboardSetEvent', function (event, args) {
    var options = args[0];
    $scope.initialize(options);
  });

  /**
   * Listen dashboard refresh event.
   */
  $scope.$on('dashboardRefreshEvent', function (event, args) {
    refreshAllTiles();
  });

  /**
   * Dashboard tile changes event
   */
  $scope.$on('dashboardLayoutLoadedEvent', function (event, args) {
    if (!angular.isUndefined(args) && !angular.isUndefined(args[0]))
      updateTileContainerSize(args[0]);
    else
      updateTileContainerSize();
    turnOnAddTileHandler();
    $scope.updateDashboardHandlers();
  });

  /**
   * Start tile edit event handler
   */
  $scope.$on('startEditTileEvent', function (event, args) {
    var options = args.length > 0 ? args[0] : {};
    $scope.editTileEvent = $scope.editTileEvent || { actionName: null };
    var isMouseMove = options.actionName == 'addtile';
    var isInEdit = $scope.editTileEvent.actionName != null && $scope.editTileEvent.actionName != 'addtile';

    $scope.showTileGrid();
    if (isMouseMove) {
      if (!isInEdit) {
        $scope.showTileGridShadow({
          x: options.shadowX,
          y: options.shadowY,
          width: $scope.tileWidth,
          height: $scope.tileHeight
        }, true);
        $scope.editTileEvent.actionName = options.actionName;
      }
    } else {
      $scope.editTileEvent.actionName = options.actionName;
    }
  });

  /**
   * Tile edit completed event handler
   */
  $scope.$on('stopEditTileEvent', function (event, args) {
    var options = args.length > 0 ? args[0] : {};
    $scope.editTileEvent = $scope.editTileEvent || { actionName: null };
    var isMouseMove = options.actionName == 'addtile';
    var isInEdit = $scope.editTileEvent.actionName != null && $scope.editTileEvent.actionName != 'addtile';

    if (isMouseMove) {
      if (!isInEdit) {
        $scope.hideTileGrid();
        $scope.editTileEvent.actionName = null;
      }
    } else {
      $scope.hideTileGrid();
      updateTileContainerSize();
      $scope.editTileEvent.actionName = null;
    }
  });

  /**
   * Delete tile event handler
   */
  $scope.$on('deleteTileEvent', function (event, args) {
    if (angular.isUndefined(args) || angular.isUndefined(args[0]))
      throw 'Should be 1 argument with object: { tileId: <tileid> }';
    var tileid = args[0].tileId;
    var tile = $scope.getTileById(tileid);
    if (tile == null)
      throw 'Tile "' + tileid + '" not found';
    var idx = -1;
    for (var i = 0; i < $scope.tiles.length; i++)
      if ($scope.tiles[i].id == tileid)
        idx = i;
    $scope.tiles.splice(idx, 1);
  });

  /**
   * Gallery mode activated/deactivated
   */
  $scope.$on('toggleGalleryMode', function (event, args) {
    if (angular.isUndefined(args) || angular.isUndefined(args[0]))
      throw 'Should be 1 argument with boolean parameter';
    var activate = args[0];
    if ((activate && $scope.isGalleryMode) || (!activate && !$scope.isGalleryMode))
      return;
    if (activate) {
      activateGallery();
    } else {
      deactivateGallery();
    }
  });

  /**
   * Open gallery mode in full screen
   */
  $scope.$on('toggleGalleryModeFullscreen', function(event, args) {
    var requestFullScreen = function(htmlElement) {
      var requestMethod = htmlElement.requestFullScreen || htmlElement.webkitRequestFullScreen || htmlElement.mozRequestFullScreen
        || htmlElement.msRequestFullscreen;
      if (requestMethod) {
        requestMethod.call(htmlElement);
      } else if (typeof window.ActiveXObject !== "undefined") {
        var wscript = new ActiveXObject("WScript.Shell");
        if (typeof (wscript.SendKeys) === 'function') {
          wscript.SendKeys("{F11}");
        } else {
          alert('Can\'t run fullscreen mode!');
        }
      }
    };
    var $galleryRoot = $scope.getGalleryContainer();
    if ($galleryRoot.length == 0) {
      alert('Can\'t find gallery root node!');
      return;
    }
    requestFullScreen($galleryRoot.get(0));
  });

  ////////////////////////////////////////////////////////
  // scope functions:
  ////////////////////////////////////////////////////////

  /**
   * Check old IE version
   */
  $scope.checkIsIE8 = function () {
    return $izendaCompatibility.checkIsIe8();
  };

  /**
   * Check if one column view required
   */
  $scope.isOneColumnView = function () {
    return $izendaCompatibility.isOneColumnView();
  };

  /**
   * Close notification
   */
  $scope.closeNotification = function(id) {
    var i = 0;
    while (i < $scope.notifications.length) {
      if ($scope.notifications[i].id == id) {
        $scope.cancelNotificationTimeout(id);
        $scope.notifications.splice(i, 1);
        $scope.$applyAsync();
        return;
      }
      i++;
    }
  };

  /**
   * Open notification
   */
  $scope.showNotification = function (title, text) {
    var nextId = $scope.notificationsIdCounter++;
    var objToShow = {
      id: nextId,
      title: title,
      text: text
    };
    objToShow.timeoutId = setTimeout(function() {
      $scope.closeNotification(objToShow.id);
    }, 5000);
    $scope.notifications.push(objToShow);
    $scope.$applyAsync();
  };

  /**
   * Cancel notification item autohide
   */
  $scope.cancelNotificationTimeout = function(id) {
    var i = 0;
    while (i < $scope.notifications.length) {
      var itm = $scope.notifications[i];
      if (itm.id == id) {
        if (itm.timeoutId >= 0) {
          clearTimeout($scope.notifications[i].timeoutId);
          $scope.notifications[i].timeoutId = -1;
        }
        return;
      }
      i++;
    }
  };

  /**
   * Refresh all tiles.
   */
  $scope.refreshAllTiles = function () {
    refreshAllTiles();
  };

  /**
   * Show editor grid
   */
  $scope.showTileGrid = function () {
    var $gridPlaceholder = $scope.getTileContainer();
    if (angular.isUndefined($scope.$grid) || $scope.$grid == null) {
      $scope.$grid = _('<div></div>')
        .addClass('dashboard-grid')
        .hide();
      $gridPlaceholder.prepend($scope.$grid);
    }
    $scope.$grid.css('background-size', $scope.tileWidth + 'px ' + $scope.tileHeight + 'px, ' +
      $scope.tileWidth + 'px ' + $scope.tileHeight + 'px');
    $scope.$grid.show();
  };

  /**
   * Hide editor grid 
   */
  $scope.hideTileGrid = function () {
    if (angular.isUndefined($scope.$grid) || $scope.$grid == null)
      return;
    $scope.$grid.hide();
    $scope.hideTileGridShadow();
  };

  /**
   * Show tile grid shadow
   */
  $scope.showTileGridShadow = function (shadowBbox, showPlusButton) {
    var $gridPlaceholder = $scope.getTileContainer();
    if (angular.isUndefined($scope.$grid) || $scope.$grid == null) {
      throw 'Can\'t show shadow without grid';
    }
    var $shadow = $gridPlaceholder.find('.tile-grid-cell.shadow');
    if ($shadow.length == 0) {
      $shadow = _('<div class="tile-grid-cell shadow"></div>').css({
        'opacity': 0.2,
        'background-color': '#000'
      });
      if (showPlusButton) {
        var $plus = _('<div class="iz-dash-select-report-front-container">' +
          '<button type="button" class="iz-dash-select-report-front-btn2 btn" title="Select Report">' +
          '<span class="glyphicon glyphicon-plus"></span>' +
          '</button>' +
          '</div>');
        $shadow.append($plus);
      }
      $gridPlaceholder.prepend($shadow);
    }

    // move shadow
    $shadow.css({
      'left': shadowBbox.x,
      'top': shadowBbox.y,
      'width': shadowBbox.width,
      'height': shadowBbox.height
    });
    $shadow.show();
  };

  /**
   * Hide tile grid shadow
   */
  $scope.hideTileGridShadow = function () {
    var $gridPlaceholder = $scope.getTileContainer();
    var $shadow = $gridPlaceholder.find('.tile-grid-cell.shadow');
    $shadow.hide();
  };

  /**
   * Go to selected slide
   */
  $scope.goToSlide = function (tileid) {
    var index = $scope.getTileIndex(tileid);
    if ($scope.galleryTileIndex < index) {
      $scope.$emit('nextSlide');
    } else {
      $scope.$emit('previousSlide');
    }
    $scope.$evalAsync();
  };

  /**
   * Next tile in gallery
   */
  $scope.nextGalleryTile = function () {
    clearInterval($scope.galleryIntervalId);
    $scope.galleryIntervalId = null;
    $scope.galleryTileIndex++;
    if ($scope.galleryTileIndex >= $scope.tiles.length) {
      $scope.galleryTileIndex = 0;
    }
    $scope.galleryTile = $scope.tiles[$scope.galleryTileIndex];
    $scope.galleryTileTitle = createTileTitle($scope.galleryTile);
    $scope.$emit('nextSlide');
    $scope.$evalAsync();
  };

  /**
   * Previous tile in gallery
   */
  $scope.prevGalleryTile = function () {
    clearInterval($scope.galleryIntervalId);
    $scope.galleryIntervalId = null;
    $scope.galleryTileIndex--;
    if ($scope.galleryTileIndex < 0) {
      $scope.galleryTileIndex = $scope.tiles.length - 1;
    }
    $scope.galleryTile = $scope.tiles[$scope.galleryTileIndex];
    $scope.galleryTileTitle = createTileTitle($scope.galleryTile);
    $scope.$emit('previousSlide');
    $scope.$evalAsync();
  };

  /**
   * Check bbox for intersects
   */
  $scope.checkTileIntersectsBbox = function (tile) {
    var hitTest = function (a, b) {
      var aLeft = a.x;
      var aRight = a.x + a.width - 1;
      var aTop = a.y;
      var aBottom = a.y + a.height - 1;

      var bLeft = b.x;
      var bRight = b.x + b.width - 1;
      var bTop = b.y;
      var bBottom = b.y + b.height - 1;
      return !(bLeft > aRight || bRight < aLeft || bTop > aBottom || bBottom < aTop);
    };
    var otherTiles = $scope.getOtherTiles($scope.tiles, tile);
    for (var i = 0; i < otherTiles.length; i++) {
      var oTile = otherTiles[i];
      if (hitTest(tile, oTile)) {
        return true;
      }
    }
    return false;
  };

  /**
   * Check tile intersects to any other tile
   */
  $scope.checkTileIntersects = function (tile, $helper) {
    var hitTest = function (a, b, accuracy) {
      var aPos = a.offset();
      var bPos = b.offset();

      var aLeft = aPos.left;
      var aRight = aPos.left + a.width();
      var aTop = aPos.top;
      var aBottom = aPos.top + a.height();

      var bLeft = bPos.left + accuracy;
      var bRight = bPos.left + b.width() - accuracy;
      var bTop = bPos.top + accuracy;
      var bBottom = bPos.top + b.height() - accuracy;
      return !(bLeft > aRight || bRight < aLeft || bTop > aBottom || bBottom < aTop);
    };
    // check

    var $tile = $scope.getTile$ById(tile.id);
    if (!angular.isUndefined($helper)) {
      $tile = $helper;
    }
    var otherTiles = $scope.getOtherTiles($scope.tiles, tile);
    for (var i = 0; i < otherTiles.length; i++) {
      var oTile = otherTiles[i];
      var $oTile = $scope.getTile$ById(oTile.id);
      if (hitTest($tile, $oTile, 30)) {
        return true;
      }
    }
    return false;
  };

  /**
   * Check tile is outside the dashboard
   */
  $scope.checkTileMovedToOuterSpace = function ($tile, sencitivity) {
    var precision = sencitivity;
    if (typeof (sencitivity) == 'undefined' || sencitivity == null)
      precision = 0;
    var tp = $tile.position();
    if (tp.left + $tile.width() > $scope.tileWidth * 12 + precision || tp.left < -precision || tp.top < -precision)
      return true;
    return false;
  };

  /**
   * Get tile which lie under testing tile.
   */
  $scope.getUnderlyingTile = function (x, y, testingTile) {
    var $target = null;
    var targetTile;
    var $tiles = $scope.getRoot().find('.iz-dash-tile');
    for (var i = 0; i < $tiles.length; i++) {
      var $t = _($tiles[i]);
      if ($t.hasClass('iz-dash-tile-helper'))
        break;
      var tile = $scope.getTileByTile$($t);
      if (tile == null || tile.id != testingTile.id) {
        var tilePosition = $t.offset();
        if (tilePosition.left <= x && tilePosition.left + $t.width() >= x && tilePosition.top <= y && tilePosition.top + $t.height() >= y) {
          targetTile = tile;
          $target = $t;
        }
      }
    }
    return $target;
  };

  /**
   * Swap 2 tiles. Return promise after complete swap.
   */
  $scope.swapTiles = function ($tile1, $tile2) {
    var deferred = $q.defer();
    var t1O = $tile1.position(),
        t2O = $tile2.position(),
        w1 = $tile1.width(),
        h1 = $tile1.height(),
        w2 = $tile2.width(),
        h2 = $tile2.height();

    $tile1.find('.frame').hide();
    $tile2.find('.frame').hide();

    var completeCount = 0;
    $tile1.animate({
      left: t2O.left,
      top: t2O.top,
      width: w2,
      height: h2
    }, 500, function () {
      if (completeCount == 0)
        completeCount++;
      else {
        deferred.resolve([$tile1, $tile2]);
      }
    });
    $tile2.animate({
      left: t1O.left,
      top: t1O.top,
      width: w1,
      height: h1
    }, 500, function () {
      if (completeCount == 0)
        completeCount++;
      else {
        deferred.resolve([$tile1, $tile2]);
      }
    });
    return deferred.promise;
  };

  /**
   * Update dashboard size:
   */
  $scope.updateDashboardSize = function () {
    updateTileContainerSize();
  };

  /**
   * Turn activate/deactivate dashboard handlers after resize
   */
  $scope.updateDashboardHandlers = function () {
    if ($scope.isOneColumnView()) {
      turnOffAddTileHandler();
    } else {
      turnOnAddTileHandler();
    }
  };

  /**
   * Initialize dashboard
   */
  $scope.initialize = function (options) {
    // remove content from all tiles to speed up "bounce up" animation
    document.addEventListener("fullscreenchange", fullscreenChangeHandler);
    document.addEventListener("webkitfullscreenchange", fullscreenChangeHandler);
    document.addEventListener("mozfullscreenchange", fullscreenChangeHandler);
    document.addEventListener("MSFullscreenChange", fullscreenChangeHandler);
    _('.report').empty();

    // load dashboard tiles layout
    $scope.updateDashboardSize();
    loadDashboardLayout();
  };

  ////////////////////////////////////////////////////////
  // dashboard functions:
  ////////////////////////////////////////////////////////

  function fullscreenChangeHandler() {
    if (document.fullscreenElement || document.webkitFullscreenElement || document.mozFullScreenElement ||
      document.msFullscreenElement) {
      $scope.isFullscreenMode = true;

      var backgroundImg = $izendaBackground.getBackgroundImgFromStorage();
      if (backgroundImg != null) {
        $scope.getGalleryContainer().get(0).style.setProperty('background-image', 'url(' + backgroundImg + ')', 'important');
      } else {
        $scope.getGalleryContainer().get(0).style.setProperty('background-image', '');
      }
      $scope.getGalleryContainer().get(0).style.setProperty('background-color', $izendaBackground.getBackgroundColor(), 'important');

      $scope.$applyAsync();
    } else {
      $scope.isFullscreenMode = false;
      $scope.getGalleryContainer().get(0).style.setProperty('background-image', '');
      $scope.getGalleryContainer().get(0).style.setProperty('background-color', '');
      $scope.$applyAsync();
    }
  }

  /**
   * Get addtile context object
   */
  function ensureAddTile() {
    $scope.addtile = $scope.addtile || {
      count: 0,
      started: false,
      startedDraw: false,
      tile: null,
      relativeX: 0,
      relativeY: 0,
      x: 0,
      y: 0
    };
  }

  /**
   * Add tile handler initialize
   */
  function turnOnAddTileHandler() {

    var addNewPixelTile = function (x, y) {
      $scope.addtile.tile = angular.extend({}, $injector.get('tileDefaults'), {
        id: 'IzendaDashboardTileNew' + (newTileIndex++),
        isNew: true,
        width: 1,
        height: 1,
        x: x,
        y: y
      });
      while (!$scope.checkTileIntersectsBbox($scope.addtile.tile) && $scope.addtile.tile.width < 6
            && $scope.addtile.tile.width + $scope.addtile.tile.x < 12) {
        $scope.addtile.tile.width++;
      }
      if ($scope.checkTileIntersectsBbox($scope.addtile.tile)) {
        $scope.addtile.tile.width--;
      }
      while (!$scope.checkTileIntersectsBbox($scope.addtile.tile) && $scope.addtile.tile.height < 3) {
        $scope.addtile.tile.height++;
      }
      if ($scope.checkTileIntersectsBbox($scope.addtile.tile)) {
        $scope.addtile.tile.height--;
      }

      $scope.tiles.push($scope.addtile.tile);
      if (!$scope.$$phase)
        $scope.$apply();
    };

    // mouse down
    $scope.getTileContainer().on('mousedown.dashboard', function (e) {
      ensureAddTile();
      var $tileContainer = $scope.getTileContainer();
      var $target = _(e.target);
      if ($scope.isTile$($target) || $scope.addtile.started)
        return;
      angular.extend($scope.addtile, {
        count: 0,
        started: true,
        startedDraw: false,
        x: e.pageX,
        y: e.pageY,
        relativeX: e.pageX - $tileContainer.offset().left,
        relativeY: e.pageY - $tileContainer.offset().top,
        tile: null
      });
    });

    // move mouse over the dashboard
    $scope.getTileContainer().on('mousemove.dashboard', function (e) {
      ensureAddTile();
      var $tileContainer = $scope.getTileContainer();
      var $target = _(e.target);
      var relativeX = e.pageX - $tileContainer.offset().left;
      var relativeY = e.pageY - $tileContainer.offset().top;
      var x = Math.floor(relativeX / $scope.tileWidth);
      var y = Math.floor(relativeY / $scope.tileHeight);

      if (!$scope.addtile.started) {
        var isTile = $scope.isTile$($target);
        if (isTile) {
          $scope.addtile.count = 0;
          $rootScope.$broadcast('stopEditTileEvent', [{
            tileId: null,
            actionName: 'addtile'
          }]);
        } else {
          $scope.addtile.count++;
          if ($scope.addtile.count > 5) {
            $rootScope.$broadcast('startEditTileEvent', [{
              tileId: $scope.id,
              shadowX: x * $scope.tileWidth,
              shadowY: y * $scope.tileHeight,
              actionName: 'addtile'
            }]);
          }
        }
        return;
      }

      // add new tile if needed
      if ($scope.addtile.count > 5) {
        if ($scope.addtile.tile == null) {
          addNewPixelTile(x, y);
        } else {
          //var tile = $scope.getTileById('IzendaDashboardTileNew');
        }
      }
      $scope.addtile.count++;
    });

    // mouseup
    $scope.getTileContainer().on('mouseup.dashboard', function (e) {
      ensureAddTile();
      var $tileContainer = $scope.getTileContainer();
      var $target = _(e.target);
      var relativeX = e.pageX - $tileContainer.offset().left;
      var relativeY = e.pageY - $tileContainer.offset().top;
      var x = Math.floor(relativeX / $scope.tileWidth);
      var y = Math.floor(relativeY / $scope.tileHeight);

      if (!$scope.addtile.started) {
        return;
      }
      if ($scope.addtile.tile == null) {
        addNewPixelTile(x, y);
      }

      $rootScope.$broadcast('stopEditTileEvent', [{
        tileId: null,
        actionName: 'addtile'
      }]);
    });

    // mouseout
    $scope.getTileContainer().on('mouseout.dashboard', function (e) {
      ensureAddTile();
      var $tileContainer = $scope.getTileContainer();
      $scope.addtile.started = false;
      $scope.addtile.startedDraw = false;
      $scope.addtile.tile = null;
      $rootScope.$broadcast('stopEditTileEvent', [{
        tileId: null,
        actionName: 'addtile'
      }]);
    });
  }

  function turnOffAddTileHandler() {
    $scope.getTileContainer().off('mousedown.dashboard');
    $scope.getTileContainer().off('mousemove.dashboard');
    $scope.getTileContainer().off('mouseup.dashboard');
    $scope.getTileContainer().off('mouseout.dashboard');
  }

  ////////////////////////////////////////////////////////
  // tiles functions:
  ////////////////////////////////////////////////////////

  function createTileTitle(tile) {
    if (tile.title != null && tile.title != '')
      return tile.title;
    var result = '';
    if (tile.reportCategory != null && tile.reportCategory != '')
      result = tile.reportCategory + ' / ';
    result = result + tile.reportName + ' / ' + tile.reportPartName;
    return result;
  }

  function sortTilesByPosition(tilesArray) {
    return tilesArray.sort(function (a, b) {
      if (a.y != b.y)
        return a.y - b.y;
      return a.x - b.x;
    });
  }

  /**
   * Prepare tiles for saving: cleaning, validating and so on...
   */
  function createSaveJson(dashboardName, dashboardCategory) {
    var tiles = $scope.tiles;
    var config = {
      Rows: [{
        Cells: [],
        ColumnsCount: tiles.length
      }],
      RowsCount: 1
    };

    for (var i = 0; i < tiles.length; i++) {
      var tile = tiles[i];
      var saveObject = {
        ReportTitle: angular.isUndefined(tile.title) ? '' : tile.title,
        ReportDescription: angular.isUndefined(tile.description) ? '' : tile.description,
        ReportFullName: tile.reportFullName,
        ReportPartName: tile.reportPartName,
        ReportSetName: tile.reportNameWithCategory,
        RecordsCount: tile.top,
        X: tile.x,
        Y: tile.y,
        Height: tile.height,
        Width: tile.width
      };
      config.Rows[0].Cells[i] = saveObject;
    }
    return config;
  }

  /**
   * Save 
   */
  function save(dashboardName, dashboardCategory) {
    var dashboardFullName = dashboardName;
    if (angular.isString(dashboardCategory) && dashboardCategory != '' && dashboardCategory.toLowerCase() != 'uncategorized') {
      dashboardFullName = dashboardCategory + '\\' + dashboardName;
    }
    var json = createSaveJson(dashboardName, dashboardCategory);
    $izendaDashboardQuery.saveDashboard(dashboardFullName, json).then(function (data) {
      var n = $izendaUrl.getReportInfo().name,
          c = $izendaUrl.getReportInfo().category;
      $scope.showNotification(null, 'Dashboard "' + dashboardName + '" successfully saved.');
      if (n != dashboardName || c != dashboardCategory) {
        $rootScope.$broadcast('selectedNewReportNameEvent', [dashboardName, dashboardCategory]);
      }
    });
  }

  /**
   * Load dashboard layout
   */
  function loadDashboardLayout() {
    // load dashboard layout

    // CLEAR TILES BEFORE LOADING:

    // remove tiles
    $scope.tiles.length = 0;

    // interrupt previous animations
    clearInterval($scope.refreshIntervalId);
    $scope.refreshIntervalId = null;

    // cancel all current queries
    var countCancelled = $izendaRsQuery.cancelAllQueries({
      ignoreList: ['getdashboardcategories']
    });
    if (countCancelled > 0) {
      console.log('>>> Cancelled ' + countCancelled + ' queryes');
    }
    // remove html
    var tiles = _('.iz-dash-tile');
    tiles.remove();

    // start loading dashboard layout
    $izendaDashboardQuery.loadDashboardLayout($izendaUrl.getReportInfo().fullName).then(function (data) {
      // collect tiles information:
      var tilesToAdd = [];
      var maxHeight = 0;
      if (data == null || data.Rows == null || data.Rows.length == 0) {
        tilesToAdd.push(angular.extend({}, $injector.get('tileDefaults'), {
          id: 'IzendaDashboardTile0',
          isNew: true,
          width: 12,
          height: 4
        }));
      } else {
        var cells = data.Rows[0].Cells;
        for (var i = 0; i < cells.length; i++) {
          var cell = cells[i];
          var obj = angular.extend({}, $injector.get('tileDefaults'), $izendaUrl.extractReportPartNames(cell.ReportFullName), {
            id: 'IzendaDashboardTile' + i,
            x: cell.X,
            y: cell.Y,
            width: cell.Width,
            height: cell.Height,
            title: cell.ReportTitle,
            description: cell.ReportDescription,
            top: cell.RecordsCount
          });
          if (maxHeight < cell.Y + cell.Height)
            maxHeight = cell.Y + cell.Height;
          tilesToAdd.push(obj);
        }
      }
      tilesToAdd = sortTilesByPosition(tilesToAdd);

      // start loading tile reports
      for (var i = 0; i < tilesToAdd.length; i++) {
        loadTileReport(tilesToAdd[i]);
      }

      // start loading tiles:
      $scope.$broadcast('dashboardLayoutLoadedEvent', [{
        top: 0,
        left: 0,
        height: (maxHeight) * $scope.tileHeight,
        width: 12 * $scope.tileWidth
      }]);
      var animationSpeed = 400;

      // start adding tiles
      var index = 0;
      function nextTile() {
        if (index >= tilesToAdd.length && $scope.refreshIntervalId != null) {
          clearInterval($scope.refreshIntervalId);
          if (index >= tilesToAdd.length) {
            // update dashboard size when all finished
            $scope.updateDashboardSize();
          }
          return;
        }
        var tile = tilesToAdd[index];
        $scope.tiles.push(tile);
        if (!$scope.$$phase)
          $scope.$apply();
        index++;
      }
      nextTile();
      $scope.refreshIntervalId = window.setInterval(nextTile, animationSpeed);
    });
  };

  /**
   * Start preloading report
   */
  function loadTileReport(tileObj) {
    tileObj.preloadData = null;
    tileObj.preloadDataHandler = null;
    if (!angular.isString(tileObj.reportFullName) || tileObj.reportFullName == '')
      return;
    tileObj.preloadStarted = true;
    tileObj.preloadDataHandler = (function () {
      var deferred = $q.defer();
      var heightDelta = tileObj.description != null && tileObj.description != '' ? 120 : 90;
      $izendaDashboardQuery.loadTileReport(
        false,
        $izendaUrl.getReportInfo().fullName,
        tileObj.reportFullName,
        null,
        tileObj.top,
        (($scope.isOneColumnView() ? 12 : tileObj.width) * $scope.tileWidth) - 40,
        (($scope.isOneColumnView() ? 4 : tileObj.height) * $scope.tileHeight) - heightDelta)
      .then(function (htmlData) {
        tileObj.preloadStarted = false;
        tileObj.preloadData = htmlData;
        deferred.resolve(htmlData);
        tileObj.preloadData = null;
        tileObj.preloadDataHandler = null;
      });
      return deferred.promise;
    })();
  }

  /**
   * Refresh all tiles
   */
  function refreshAllTiles() {
    $scope.$broadcast('tileRefreshEvent', [false]);
  }

  ////////////////////////////////////////////////////////
  // tile container functions:
  ////////////////////////////////////////////////////////

  /**
   * Tile container style
   */
  function updateTileContainerSize(additionalBox) {
    var $tileContainer = $scope.getTileContainer();

    // update width
    var parentWidth = $scope.getRoot().width();
    var width = Math.floor(parentWidth / 12) * 12;
    $tileContainer.width(width);
    $scope.tileWidth = width / 12;
    $scope.tileHeight = $scope.tileWidth > 100 ? $scope.tileWidth : 100;
    $scope.$evalAsync();

    // update height
    var maxHeight = 0;
    _.each($scope.tiles, function(iTile, tile) {
      if (tile.y + tile.height > maxHeight) {
        maxHeight = tile.y + tile.height;
      }
    });
    maxHeight = maxHeight * $scope.tileHeight;

    // update height of union of tiles and additional box it is set
    if (!angular.isUndefined(additionalBox)) {
      if (additionalBox.top + additionalBox.height > maxHeight) {
        maxHeight = additionalBox.top + additionalBox.height;
      }
    }

    // set height:
    $scope.tileContainerStyle.height = (maxHeight + $scope.tileHeight + 1) + 'px';
  }

  ////////////////////////////////////////////////////////
  // gallery
  ////////////////////////////////////////////////////////

  function activateGallery() {
    if ($scope.tiles.length == 0) {
      alert('Cannot run gallery: no tiles found.');
      return;
    }
    $scope.isGalleryMode = true;

    updateGalleryContainer();
    setTimeout(function () {
      angular.element(document.getElementById('impresshook')).scope().$emit('initImpress');
      loadTileToGallery();
     /* $scope.galleryIntervalId = setInterval(function() {
        $scope.$emit('nextSlide');
      }, 10000);*/
      $scope.$evalAsync();
    }, 1);
  }
  
  function deactivateGallery() {
    $scope.isGalleryMode = false;
    clearInterval($scope.galleryIntervalId);
    $scope.galleryIntervalId = null;
    clearGalleryTiles();
    _('body').css('overflow', 'auto');
    updateTileContainerSize();
    $scope.$evalAsync();
  }

  function loadTileToGallery() {
    // load report
    var galleryTiles = _('.slide');
    galleryTiles.each(function (iTile, tile) {
      var $tile = _(tile);
      clearGalleryTileHtml($tile);
      var tileObj = $scope.getTileById($tile.attr('tileId'));
      console.log($tile.width(), $tile.height());
      $izendaDashboardQuery.loadTileReport(false,
        $izendaUrl.getReportInfo().fullName,
        tileObj.reportFullName,
        null,
        tileObj.top,
        $tile.width(), $tile.height())
      .then(function (htmlData) {
        applyGalleryTileHtml($tile, htmlData);
        $scope.$evalAsync();
      });
    });
    $scope.galleryTileIndex = 0;
    $scope.galleryTile = $scope.tiles[0];
    $scope.galleryTileTitle = createTileTitle($scope.galleryTile);
  }

  function updateGalleryContainer() {
    var tileContainerTop = $scope.getRoot().offset().top;
    $scope.galleryContainerStyle['height'] = _($window.top).height() - tileContainerTop - 30;
    $scope.$evalAsync();
  }

  /**
   * Clear tile inner html
   */
  function clearGalleryTileHtml($tile) {
    $tile.empty();
  }

  function clearGalleryTiles() {
    $scope.getGalleryContainer().find('.slide').empty();
  }

  /**
   * Set tile inner html
   */
  function applyGalleryTileHtml($tile, htmlData) {
/*    if ($tile.attr('tileid') == 'IzendaDashboardTile0') {
      debugger;
    }*/
    clearGalleryTileHtml($tile);
    var $b = $tile;
    if (!angular.isUndefined(ReportScripting))
      ReportScripting.loadReportResponse(htmlData, $b);
    if (!angular.isUndefined(AdHoc.Utility) && typeof AdHoc.Utility.DocumentReadyHandler == 'function') {
      AdHoc.Utility.DocumentReadyHandler();
    }
    var divs$ = $b.find('div.DashPartBody, div.DashPartBodyNoScroll');

    divs$.find('span').each(function(iSpan, span) {
      var $span = _(span);
      if ($span.attr('id') && $span.attr('id').indexOf('_outerSpan') >= 0) {
        $span.css('display', 'inline');
      }
    });

    var $zerochartResults = divs$.find('.iz-zero-chart-results');
    if ($zerochartResults.length > 0) {
      $zerochartResults.closest('table').css('height', '100%');
      divs$.css('height', '100%');
    }
    if (!angular.isUndefined(AdHoc) && !angular.isUndefined(AdHoc.Utility) && typeof (AdHoc.Utility.DocumentReady) == 'function') {
      AdHoc.Utility.DocumentReady();
    }
  }
}]);
