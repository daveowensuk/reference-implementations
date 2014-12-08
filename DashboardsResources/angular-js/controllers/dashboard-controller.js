angular.module('izendaDashboard').controller('IzendaDashboardController', ['$rootScope', '$scope', '$window', '$q', '$animate', '$timeout', '$injector', '$izendaUrl', '$izendaDashboardQuery',
function IzendaDashboardController($rootScope, $scope, $window, $q, $animate, $timeout, $injector, $izendaUrl, $izendaDashboardQuery) {
  'use strict';

  var newTileIndex = 1;

  // dashboard options
  $scope.options = {
    dashboardFullName: null,
    dashboardCategory: null,
    dashboardName: null,
    urlSettings: $izendaUrl.urlSettings
  };

  // dashboard tiles container
  $scope.tileContainerStyle = {
    'height': 0
  };

  // is dashboard changing now.
  $scope.isChangingNow = false;

  // dashboard tiles
  $scope.tiles = [];
  $scope.tileWidth = 0;
  $scope.tileHeight = 0;

  ////////////////////////////////////////////////////////
  // event handlers:
  ////////////////////////////////////////////////////////

  /**
   * Dashboard window resized handler
   */
  $scope.$on('dashboardResizeEvent', function () {
    // update dashboard tile sizes
    $scope.updateDashboardSize();
    $scope.updateDashboardHandlers();
  });

  /**
   * Create new dashboard
   */
  $scope.$on('dashboardCreateNewEvent', function () {
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
   * Listen dashboard "save/save as" event
   */
  $scope.$on('dashboardSaveEvent', function (event, args) {
    if (args[0]) {
      $rootScope.$broadcast('openSelectReportNameModalEvent', []);
    } else {
      alert('SAVE DASHBOARD. Use name dialog: ' + args[0]);
    }
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

  ////////////////////////////////////////////////////////
  // scope helper functions:
  ////////////////////////////////////////////////////////

  $scope.getRoot = function () {
    return angular.element('#dashboardsDiv');
  };
  $scope.getTileContainer = function () {
    return angular.element('#dashboardBodyContainer');
  };
  $scope.getTileById = function (tileId) {
    for (var i = 0; i < $scope.tiles.length; i++) {
      if ($scope.tiles[i].id == tileId)
        return $scope.tiles[i];
    }
    return null;
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
    var $el = angular.element(el);
    return angular.element($el.closest('.iz-dash-tile'));
  };
  $scope.isTile$ = function (el) {
    var $el = angular.element(el);
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
  // scope functions:
  ////////////////////////////////////////////////////////

  /**
   * Check is dashboard should have mobile view.
   */
  $scope.isMobile = function () {
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
      return true;
    }
    return false;
  };

  /**
   * Check is dashboard window is too small to fit several columns of tiles.
   */
  $scope.isSmallResolution = function() {
    return angular.element($window).width() <= 1024;
  };

  /**
   * Check if one column view required
   */
  $scope.isOneColumnView = function() {
    return $scope.isMobile() || $scope.isSmallResolution();
  };

  // is dashboard readonly:
  $scope.isReadonly = $scope.isOneColumnView();

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
      $scope.$grid = angular.element('<div></div>')
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
      $shadow = angular.element('<div class="tile-grid-cell shadow"></div>').css({
        'opacity': 0.2,
        'background-color': '#000'
      });
      if (showPlusButton) {
        var $plus = angular.element('<div class="iz-dash-select-report-front-container">' +
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
      var $t = angular.element($tiles[i]);
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
    return $q(function (resolve) {
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
          resolve([$tile1, $tile2]);
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
          resolve([$tile1, $tile2]);
        }
      });
    });
  };

  /**
   * Update dashboard size:
   */
  $scope.updateDashboardSize = function() {
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
    // set options
    $scope.options.reportInfo = $izendaUrl.getReportInfo();

    // remove content from all tiles to speed up "bounce up" animation
    angular.element('.report').empty();

    // load dashboard tiles layout
    loadDashboardLayout();
  };

  ////////////////////////////////////////////////////////
  // dashboard functions:
  ////////////////////////////////////////////////////////
  
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
      $scope.tiles.push($scope.addtile.tile);
      if (!$scope.$$phase)
        $scope.$apply();
    };

    // mouse down
    $scope.getTileContainer().on('mousedown.dashboard', function (e) {
      ensureAddTile();
      var $tileContainer = $scope.getTileContainer();
      var $target = angular.element(e.target);
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
      var $target = angular.element(e.target);
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
      var $target = angular.element(e.target);
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

  function sortTilesByPosition(tilesArray) {
    return tilesArray.sort(function (a, b) {
      if (a.y != b.y)
        return a.y - b.y;
      return a.x - b.x;
    });
  }

  /**
   * Load dashboard layout
   */
  function loadDashboardLayout() {
    // load dashboard layout
    if ($scope.tiles.length > 0) {
      $scope.tiles.length = 0;
    }
    $izendaDashboardQuery.loadDashboardLayout($scope.options.reportInfo.fullName).then(function (data) {
      // collect tiles information:
      var tilesToAdd = [];
      var maxHeight = 0;
      if (data == null || data.Rows == null || data.Rows.length == 0) {
        tilesToAdd.push(angular.extend({}, $injector.get('tileDefaults'), {
          id: 'IzendaDashboardTile0',
          isNew: true,
          width: 12,
          height: 4,
          options: $scope.options
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
            top: cell.RecordsCount,
            options: $scope.options
          });
          if (maxHeight < cell.Y + cell.Height)
            maxHeight = cell.Y + cell.Height;
          tilesToAdd.push(obj);
        }
      }
      tilesToAdd = sortTilesByPosition(tilesToAdd);
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
      var animationSpeed = 200;
      var refreshIntervalId = null;
      $scope.tiles.length = 0;
      var index = 0;
      function nextTile() {
        if (index >= tilesToAdd.length && refreshIntervalId != null) {
          clearInterval(refreshIntervalId);
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
      refreshIntervalId = window.setInterval(nextTile, animationSpeed);
    });
  };

  /**
   * Start preloading report
   */
  function loadTileReport(tileObj) {
    tileObj.preloadStarted = true;
    tileObj.preloadData = null;
    tileObj.preloadDataHandler = $q(function (resolve) {
      $izendaDashboardQuery.loadTileReport(false, $izendaUrl.getReportInfo().fullName, tileObj.reportFullName, null,
            tileObj.top, (tileObj.width * $scope.tileWidth) - 20, (tileObj.height * $scope.tileHeight) - 90)
      .then(function (htmlData) {
        tileObj.preloadStarted = false;
        tileObj.preloadData = htmlData;
        resolve(htmlData);
        tileObj.preloadData = null;
      });
    });
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
    if (!$scope.$$phase)
      $scope.$apply();

    // update height
    var maxHeight = 0;
    $tileContainer.find('.iz-dash-tile').each(function(iTile, tile) {
      var $tile = angular.element(tile);
      if ($tile.position().top + $tile.height() > maxHeight) {
        maxHeight = $tile.position().top + $tile.height();
      }
    });

    // update height of union of tiles and additional box it is set
    if (!angular.isUndefined(additionalBox)) {
      if (additionalBox.top + additionalBox.height > maxHeight) {
        maxHeight = additionalBox.top + additionalBox.height;
      }
    }

    // set height:
    $scope.tileContainerStyle.height = (maxHeight + $scope.tileHeight + 1) + 'px';
  };
}]);
