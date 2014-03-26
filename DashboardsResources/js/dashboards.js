/**
 * Izenda dashboard jQuery plugin.
 */
(function ($) {
  var tileIdCounter = 0;

  /**
	 * Izenda dashboard class
	 */
  var IzendaDashboard = function () {

  };

  IzendaDashboard.prototype = {
    setConfig: function (options) {
      this.options = null;
      this.initialize(this.$root, options, $.fn.izendaDashboard.defaults);
      return this;
    },

    initialize: function (element, options, defaults) {
      this.$root = element;
      this.options = $.extend({}, defaults, options);

      // prepare dashboard layout
      if (this.options.dashboardLayout == null) {
        throw 'Dashboard option "dashboardLayout" cannot be null';
      }
      var dashboardLayout = this.options.dashboardLayout;

      // prepare tiles
      for (var i = 0; i < dashboardLayout.tiles.length; i++) {
        dashboardLayout.tiles[i] = $.extend({}, $.fn.izendaDashboard.tileDefaults, dashboardLayout.tiles[i]);
        dashboardLayout.tiles[i].id = 'IzendaDashboardTile' + (++tileIdCounter);
      }

      // check tiles for intersections
      if (!this.checkTilesForIntersections(dashboardLayout.tiles)) {
        alert('Cannot render dashboard: found tile intersections!');
        return this;
      }

      this.draw();
      return this;
    },

    /**
     * Check tiles for intersections
     */
    checkTilesForIntersections: function (tiles) {
      for (var i = 0; i < tiles.length; i++) {
        var tile = tiles[i];
        for (var j = i + 1; j < tiles.length; j++) {
          var tile2 = tiles[j];
          if ((tile.x + tile.width <= tile2.x || tile2.x + tile2.width <= tile.x) ||
          (tile.y + tile.height <= tile2.y || tile2.y + tile2.height <= tile.y)) { } else {
            console.log('Intersections found: ', tile, tile2);
            return false;
          }
        }
      }
      return true;
    },

    /**
     * Draw dashboard
     */
    draw: function () {
      this.$root.empty();
      this.$containerDiv = $('<div class="container-fluid"></div>');
      this.$root.append(this.$containerDiv);

      // draw tile
      var tiles = this.options.dashboardLayout.tiles;
      this.drawCluster(0, 0, tiles);
      this.options.onGridInitialized();
    },

    /**
     * Draw cluster
     */
    drawCluster: function (clusterOffsetX, clusterOffsetY, tiles) {
      var _this = this;
      var subclusters = this.createRowClusters(clusterOffsetX, clusterOffsetY, tiles);
      console.log('clusters:', subclusters);
      if (subclusters.length > 0) {
        for (var i = 0; i < subclusters.length; i++) {
          _this.drawRow(_this.$containerDiv, subclusters[i], true);
        }
      }
    },

    /**
     * Draw row
     */
    drawRow: function ($container, rowClusters, first) {
      var _this = this;
      var tileWidth = this.$containerDiv.width() / 12;
      var clusterBbox = _this.getClusterBBox(rowClusters);
      var mdValue = 12;
      if (first) {
        mdValue = clusterBbox.maxX - clusterBbox.minX + 1;
      }

      var $row = $('<div class="row"></div>');
      $row.height(tileWidth * (clusterBbox.maxY - clusterBbox.minY + 1));
      if (!$.isArray(rowClusters)) {
        // row cluster is one single cell
        var $cell = $('<div></div>');
        $cell.height(tileWidth * rowClusters.height);
        $cell.addClass('col-md-' + mdValue);
        if (!rowClusters.isStub) {
          //$cell.css('background-color', '#' + (Math.floor(255 * 255 * 255 - Math.random() * (255 * 255))).toString(16));
          $cell.attr('tileId', rowClusters.id);
          $cell.addClass('dd-tile-placeholder');
        }
        $row.append($cell);
      } else {
        var getLeftClustersMaxX = function (allClusters, testCluster) {
          var clusterBbox1 = _this.getClusterBBox(testCluster);
          var clusterMinX = clusterBbox1.minX;
          var maxX = 0;
          for (var i = 0; i < allClusters.length; i++) {
            var cluster = allClusters[i];
            clusterBbox1 = _this.getClusterBBox(cluster);
            if (clusterBbox1.maxX < clusterMinX && maxX < clusterBbox1.maxX + 1) {
              maxX = clusterBbox1.maxX + 1;
            }
          }
          return maxX;
        };

        // run over row clusters
        for (var i = 0; i < rowClusters.length; i++) {
          var cluster = rowClusters[i];
          var $cell = $('<div></div>');
          if ($.isArray(cluster)) {
            // current row cluster contains a lot of elements
            var clusterBbox2 = _this.getClusterBBox(cluster);
            for (var j = 0; j < cluster.length; j++) {
              var columnCluster = cluster[j];
              _this.drawRow($cell, columnCluster, false);
            }
            $cell.addClass('col-md-' + (clusterBbox2.maxX - clusterBbox2.minX + 1));
            var leftMaxX = getLeftClustersMaxX(rowClusters, cluster);
            if (leftMaxX > 0 && clusterBbox2.minX - leftMaxX - 1 > 0) {
              $cell.addClass('col-md-offset-' + (clusterBbox2.minX - leftMaxX));
            }
          } else {
            // current row cluster contains on element
            $cell.height(tileWidth * cluster.height);
            if (first) {
              $cell.addClass('col-md-' + cluster.width);
            } else {
              $cell.addClass('col-md-12');
            }
            if (cluster.x > 0 && first) {
              var leftMaxX2 = getLeftClustersMaxX(rowClusters, cluster);
              var offsetClass = 'col-md-offset-' + (cluster.x - leftMaxX2);
              $cell.addClass(offsetClass);
            }
            if (!cluster.isStub) {
              $cell.attr('tileId', cluster.id);
              $cell.addClass('dd-tile-placeholder');
              //$cell.css('background-color', '#' + (Math.floor(255 * 255 * 255 - Math.random() * (255 * 255))).toString(16));
            }
          }
          $row.append($cell);
        }
      }
      $container.append($row);
      return $row;
    },

    /**
    * Create column cluster
    */
    createColumnClusters: function (clusterOffsetX, clusterOffsetY, tiles) {
      var _this = this;
      var transposedTiles = this.transposeCoordinates(tiles);
      var columnClusters = this.createRowClusters(clusterOffsetY, clusterOffsetX, transposedTiles);
      var clusters = [];
      for (var iCluster = 0; iCluster < columnClusters.length; iCluster++) {
        var cluster = columnClusters[iCluster];
        var tCluster = _this.transposeCoordinates(cluster);
        clusters.push(tCluster);
      }
      var result = [];
      for (var iCluster = 0; iCluster < clusters.length; iCluster++) {
        var cluster = clusters[iCluster];
        var bbox = _this.getTilesBBox(cluster);
        if (cluster.length > 1 && bbox.maxX - bbox.minX > 0) {
          columnClusters = _this.createColumnClusters(bbox.minX, bbox.minY, cluster);
          result.push(columnClusters);
        } else {
          result.push(cluster);
        }
      }
      return result;
    },

    /**
     * Create row cluster of which contains tiles, not intersecting with tiles from other rows
     */
    createRowClusters: function (clusterOffsetX, clusterOffsetY, tiles) {
      var _this = this;
      var clusters = [];

      // check free space before cluster
      var rowClusterBbox = this.getTilesBBox(tiles);
      var currentCluster = [];
      var currentClusterHeight = 0;
      var rowsInCluster = 0;
      var currentClusterIndex = 0;
      var i = 0;
      var currentEmptyCluster = null;
      var clusterCompleted = false;
      for (var y = clusterOffsetY; y <= rowClusterBbox.maxY; y++) {
        var currentRow = this.getTilesAtRow(y, tiles);
        if (currentRow.length == 0 && i >= currentClusterIndex + currentClusterHeight) {
          // if row is empty
          clusterCompleted = true;
          if (currentEmptyCluster == null) {
            currentEmptyCluster = {
              x: rowClusterBbox.minX,
              y: y,
              width: rowClusterBbox.maxX - rowClusterBbox.minX + 1,
              height: 1,
              isStub: true
            };
          } else {
            currentEmptyCluster.height += 1;
          }
        } else {
          if (currentEmptyCluster != null) {
            // save cluster
            clusters.push(currentEmptyCluster);
            currentEmptyCluster = null;
            currentClusterIndex = i;
          }

          // process real row
          var rowHeight = this.calculateTilesHeight(currentRow);
          if (rowsInCluster + rowHeight > currentClusterHeight) {
            currentClusterHeight = rowsInCluster + rowHeight;
          }
          currentCluster = $.merge(currentCluster, currentRow);
          rowsInCluster++;
          if (i >= currentClusterIndex + currentClusterHeight - 1) {
            clusterCompleted = true;
          } else {
            clusterCompleted = false;
          }
        }
        // if cluster completed: save it
        if (clusterCompleted && currentCluster.length > 0) {
          var currCluster = $.merge([], currentCluster);
          if (currCluster.length > 0)
            clusters.push(currCluster);
          currentCluster = [];
          currentClusterHeight = 0;
          currentClusterIndex = i + 1;
          rowsInCluster = 0;
        }
        i++;
      }
      var result = [];
      for (var iCluster = 0; iCluster < clusters.length; iCluster++) {
        var cluster = clusters[iCluster];
        var bbox = _this.getTilesBBox(cluster);
        if ($.isArray(cluster) && cluster.length > 1 && bbox.maxY - bbox.minY > 0) {
          var columnClusters = _this.createColumnClusters(bbox.minX, bbox.minY, cluster);
          result.push(columnClusters);
        } else {
          result.push(cluster);
        }
      }
      return result;
    },

    /**
     * Sort tiles by x and y values;
     */
    sortTilesByYX: function (tiles) {
      return tiles.sort(function (a, b) {
        if (a.x !== b.x)
          return a.x - b.x;
        return a.y - b.y;
      });
    },

    /**
     * Sort tiles by y and x values;
     */
    sortTilesByXY: function (tiles) {
      return tiles.sort(function (a, b) {
        if (a.y !== b.y)
          return a.y - b.y;
        return a.x - b.x;
      });
    },

    /**
     * Get tiles placed on selected row
     */
    getTilesAtRow: function (rowNumber, tiles) {
      return $.grep(tiles, function (tile) {
        return tile.y == rowNumber;
      });
    },

    /**
		 * Calculate tiles cluster bbox
		 */
    getClusterBBox: function (cluster) {
      var _this = this;
      if (!$.isArray(cluster)) {
        return _this.getTilesBBox([cluster]);
      }
      var minX = Number.MAX_VALUE,
          minY = Number.MAX_VALUE,
          maxX = 0,
          maxY = 0;
      for (var iItem = 0; iItem < cluster.length; iItem++) {
        var virtualTile = cluster[iItem];
        if ($.isArray(virtualTile)) {
          var bbox = _this.getClusterBBox(virtualTile);
          virtualTile = {
            x: bbox.minX,
            y: bbox.minY,
            width: bbox.maxX - bbox.minX + 1,
            height: bbox.maxY - bbox.minY + 1
          };
        }
        if (virtualTile.x < minX) {
          minX = virtualTile.x;
        }
        if (virtualTile.y < minY) {
          minY = virtualTile.y;
        }
        if (virtualTile.x + virtualTile.width - 1 > maxX) {
          maxX = virtualTile.x + virtualTile.width - 1;
        }
        if (virtualTile.y + virtualTile.height - 1 > maxY) {
          maxY = virtualTile.y + virtualTile.height - 1;
        }
      }
      return {
        minX: minX,
        minY: minY,
        maxX: maxX,
        maxY: maxY
      };
    },

    /**
		 * Calculate tiles bbox
		 */
    getTilesBBox: function (tiles) {
      var minX = Number.MAX_VALUE, minY = Number.MAX_VALUE, maxX = 0, maxY = 0;
      $.each(tiles, function (iTile, tile) {
        if (tile.x < minX) {
          minX = tile.x;
        }
        if (tile.y < minY) {
          minY = tile.y;
        }
        if (tile.x + tile.width - 1 > maxX) {
          maxX = tile.x + tile.width - 1;
        }
        if (tile.y + tile.height - 1 > maxY) {
          maxY = tile.y + tile.height - 1;
        }
      });
      return {
        minX: minX,
        minY: minY,
        maxX: maxX,
        maxY: maxY
      };
    },

    transposeTile: function (tile) {
      var tTile = $.extend({}, tile);
      var x = tTile.x;
      var y = tTile.y;
      var width = tTile.width;
      var height = tTile.height;
      tTile.x = y;
      tTile.y = x;
      tTile.width = height;
      tTile.height = width;
      return tTile;
    },

    /**
		 * Change coordinates
		 */
    transposeCoordinates: function (tiles) {
      var trasposedTiles = [];
      var transposed;
      if (!$.isArray(tiles)) {
        return this.transposeTile(tiles);
      }
      for (var iTile = 0; iTile < tiles.length; iTile++) {
        var tile = tiles[iTile];
        if ($.isArray(tile)) {
          var result = [];
          for (var iTile2 = 0; iTile2 < tile.length; iTile2++) {
            transposed = this.transposeCoordinates(tile[iTile2]);
            result.push(transposed);
          }
          trasposedTiles.push(result);
        } else {
          transposed = this.transposeTile(tile);
          trasposedTiles.push(transposed);
        }
      }
      return trasposedTiles;
    },

    /**
		 * Calculate count of rows needed for drawing dashboard
		 */
    calculateTilesHeight: function (tiles) {
      if (tiles.length == 0)
        return 0;
      // find minimum y
      var minY = Number.MAX_VALUE;
      for (var iTile = 0; iTile < tiles.length; iTile++) {
        var tile = tiles[iTile];
        if (tile.y < minY) {
          minY = tile.y;
        }
      }

      // find max height
      var maxHeight = 0;
      for (var iTile = 0; iTile < tiles.length; iTile++) {
        var tile = tiles[iTile];
        if (tile.y - minY + tile.height > maxHeight) {
          maxHeight = tile.y - minY + tile.height;
        }
      }
      return maxHeight;
    }
  };

  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  /**
	 * Plugin definition. Possible options you can see at $.fn.izendaDashboard.defaults object.
	 */
  $.fn.izendaDashboard = function (dashboardOptions) {
    var element = $(this);
    var dashboard = new IzendaDashboard();
    dashboard.initialize(element, dashboardOptions, $.fn.izendaDashboard.defaults);
    return dashboard;
  };

  /**
	 * Default dashboard options. Can be overriden in external code if needed.
	 */
  $.fn.izendaDashboard.defaults = {
    dashboardLayout: null, // dashboard sctructure (rows and cells description)
    onGridInitialized: function() {}
  };

  /**
	 * Default 
	 */
  $.fn.izendaDashboard.tileDefaults = {
    id: null,
    title: null,
    x: 0,
    y: 0,
    width: 1,
    height: 1
  };
}(jQuery));