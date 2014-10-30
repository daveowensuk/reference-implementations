/**
 * Izenda dashboard jQuery plugin.
 */
var jq = typeof jq$ != 'undefined' ? jq$ : jQuery;
(function ($) {
	var tileIdCounter = 0;
	var rtime = new Date(1, 1, 2000, 12, 00, 00);
	var timeout = false;
	var delta = 200;
	var previousWidth;
	var isEditActionPerformsNow = false;

	function parseReportName(reportFullName) {
		if (reportFullName == null)
			throw 'full name is null';
		var parseReportSetName = function (reportSetName) {
			if (reportSetName.indexOf('\\') > 0) {
				var p = reportSetName.split('\\');
				return {
					reportCategory: p[0],
					reportName: p[1]
				};
			}
			return {
				reportCategory: null,
				reportName: reportSetName
			};
		};

		var result = {
			reportFullName: reportFullName
		};
		var reportSetName = reportFullName;
		if (reportFullName.indexOf('@') >= 0) {
			var parts = reportFullName.split('@');
			result.reportPartName = parts[0];
			reportSetName = parts[1];
		}

		var reportNameObj = parseReportSetName(reportSetName);
		result.reportSetName = reportSetName;
		result.reportName = reportNameObj.reportName;
		result.reportCategory = reportNameObj.reportCategory;
		return result;
	}


	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Izenda Dashboard
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/**
	* Izenda dashboard class
	*/
	var IzendaDashboard = function () { };
	IzendaDashboard.prototype = {

		/**
		 * Initialize dashboard.
		 */
		initialize: function (element, options, defaults) {
			this.options = $.extend({}, defaults, options);
			this.temp = {
				addtile: null
			};
			this.query = new IzendaQuery();
			this.query.initialize({
				trace: true,
				urlSettings: this.options.urlSettings
			});

			this.$root = element;
			this.$root.empty();

			this.deinitializeWindowResize();

			if (this.options.urlSettings.reportInfo.fullName) {
				this.query.setDashboardToCrs(this, this.options.urlSettings.reportInfo.fullName, function () {
					this.initializeUi();
				});
			} else {
				this.initializeUi();
			}
		},

		initializeUi: function () {

			this.initializeToolbar();

			// initialize filters
			this.initializeFilters();

			// initialize modal
			this.initializeModal();

			// initialize confirmation modal
			this.initializeModalConfirm();

			// initialize tiles
			this.initializeTiles();

			// initialize resize
			this.initializeWindowResize();
		},

		/**
		 * Initialize dashboard filters
		 */
		initializeFilters: function () {
			$('#htmlFiltersTd').remove();
			this.$root.children('nav').after('<div id="htmlFiltersTd" style="position: relative;"></div>');
			this.filters = new IzendaFilters();
			this.filters.initialize({
				dashboard: this,
				$root: $('#htmlFiltersTd')
			});
		},

		/**
		 * initialize dashboard toolbar
		 */
		initializeToolbar: function () {
			// dashboard modal events.
			var $nameInput = $('#izendaDashboardNameModalName');
			var $categoryNameInput = $('#izendaDashboardNameModalCategoryName');
			var $categoryNameSelect = $('#izendaDashboardNameModalCategory');
			var $okBtn = $('#izendaDashboardNameModalOk');
			$categoryNameSelect.change(function () {
				var $select = $(this);
				var $selectedOption = $select.children('option:selected');
				var attr = $selectedOption.attr('create-new');
				if (typeof attr !== typeof undefined && attr !== false) {
					$categoryNameInput.parent().toggleClass('hidden');
					$categoryNameSelect.parent().toggleClass('hidden');
					$categoryNameInput.focus();
				}
			});
			$nameInput.keyup(function (e) {
				if ($(e.target).val()) {
					$okBtn.removeClass('disabled');
				} else {
					$okBtn.addClass('disabled');
				}
			});
			return;
			var _this = this;
			var $root = this.$root;
			var $toolbar = $($("#toolbarTemplate").render({
				dashboardOptions: this.options
			}).trim());
			$root.append($toolbar);

			

			// save button
			$('#izendaDashboardSaveDash').click(function (e) {
				e.preventDefault();
				_this.updateTilesSizes();
				_this.query.saveDashboard(_this, _this.options.urlSettings.reportInfo.fullName, _this.options.dashboardLayout.tiles, function (data) {
					alert('Dashboard saved');
				});
			});

			// save as button
			$('#izendaDashboardSaveDashAs').click(function (e) {
				e.preventDefault();
				_this.openNewDashboardModal(function (reportName, categoryName) {
					var newDashboardName = reportName;
					if (categoryName != null && categoryName.trim() != '' && categoryName.toLowerCase() != 'uncategorized') {
						newDashboardName = categoryName + '\\' + reportName;
					}
					_this.query.checkReportSetExists(_this, newDashboardName, function (exist) {
						if (!exist) {
							_this.updateTilesSizes();
							_this.query.saveDashboard(_this, newDashboardName, _this.options.dashboardLayout.tiles, function (data) {
								var url = _this.options.urlSettings.urlDashboardsPage + '?rn=' + newDashboardName;
								url = url.replace('Dashboards.aspx', 'Dashboards-New.aspx');
								window.location.href = url;
							});
						} else {
							alert("Can't create new dashboard. Report or dashboard with this name already exists.");
						}
					});
				});
			});

			// initialize create button
			$('#izendaDashboardCreateDash').click(function (e) {
				e.preventDefault();
				// open dashboard
				_this.openNewDashboardModal(function (reportName, categoryName) {
					var url = reportName;
					if (categoryName != null && categoryName.trim() != '' && categoryName.toLowerCase() != 'uncategorized') {
						url = categoryName + '\\' + reportName;
					}
					_this.query.checkReportSetExists(_this, url, function (exist) {
						if (!exist) {
							url = _this.options.urlSettings.urlDashboardsPage + '?isNew=1&rn=' + url;
							url = url.replace('Dashboards.aspx', 'Dashboards-New.aspx');
							window.location.href = url;
						} else {
							alert("Can't create new dashboard. Report or dashboard with this name already exists.");
						}
					});
				});
			});

			$('#izendaDashboardRefreshDash').click(function(e) {
				e.preventDefault();
				_this.updateAllTiles();
			});

			// initialize hue rotate button
			var isChrome = /Chrome/.test(navigator.userAgent) && /Google Inc/.test(navigator.vendor);
			var isSafari = /Safari/.test(navigator.userAgent) && /Apple Computer/.test(navigator.vendor);
			if (isChrome || isSafari) {
				$('.hue-rotate-btn').click(function (e) {
					e.preventDefault();
					if (hueRotateTimeOut == null) {
						$('.hue-rotate-btn').children('img').attr('src', 'Resources/images/hue-rotate.png');
						var e = $(".iz-dash-background");
						if (window.chrome) {
							rotate(e);
						}
					} else {
						$('.hue-rotate-btn').children('img').attr('src', 'Resources/images/hue-rotate-inactive.png');
						clearTimeout(hueRotateTimeOut);
						hueRotateTimeOut = null;
					}
				});
			} else {
				$('.hue-rotate-btn').hide();
			}

			// load and initialize navigation
			this.query.loadDashboardNavigation(this, function (dashboardNavigation) {
				// create dashboard selector
				_this.dashboardNavigation = dashboardNavigation;
				_this.createDashboardNavigationMenu();
			});
		},

		/**
		 * Show create new dashboard dialog
		 */
		createNewDashboard: function () {
			var _this = this;
			this.openNewDashboardModal(function (reportName, categoryName) {
				var url = reportName;
				if (categoryName != null && categoryName.trim() != '' && categoryName.toLowerCase() != 'uncategorized') {
					url = categoryName + '\\' + reportName;
				}
				_this.query.checkReportSetExists(_this, url, function (exist) {
					if (!exist) {
						url = _this.options.urlSettings.urlDashboardsPage + '?rn=' + url;
						url = url.replace('Dashboards.aspx', 'Dash.aspx#');
						url += '&isNew=1';
						window.location.href = url;
					} else {
						alert("Can't create new dashboard. Report or dashboard with this name already exists.");
					}
				});
			});
		},

		/**
		 * Save current dashboard.
		 */
		saveDashboard: function () {
			this.updateTilesSizes();
			this.query.saveDashboard(this, this.options.urlSettings.reportInfo.fullName, this.options.dashboardLayout.tiles, function (data) {
				alert('Dashboard saved');
			});
		},

		/**
		 * Save current dashboard with given name and category.
		 */
		saveDashboardAs: function () {
			var _this = this;
			_this.openNewDashboardModal(function (reportName, categoryName) {
				var newDashboardName = reportName;
				if (categoryName != null && categoryName.trim() != '' && categoryName.toLowerCase() != 'uncategorized') {
					newDashboardName = categoryName + '\\' + reportName;
				}
				_this.query.checkReportSetExists(_this, newDashboardName, function (exist) {
					if (!exist) {
						_this.updateTilesSizes();
						_this.query.saveDashboard(_this, newDashboardName, _this.options.dashboardLayout.tiles, function (data) {
							var url = _this.options.urlSettings.urlDashboardsPage + '?rn=' + newDashboardName;
							url = url.replace('Dashboards.aspx', 'Dashboards-New.aspx');
							window.location.href = url;
						});
					} else {
						alert("Can't create new dashboard. Report or dashboard with this name already exists.");
					}
				});
			});
		},

		/**w
		 * Create dashboards navigation links in toolbar
		 */
		createDashboardNavigationMenu: function () {
			return;
			var _this = this;
			var currentDashboardCategory = this.options.urlSettings.reportInfo.category;
			if (currentDashboardCategory == null)
				currentDashboardCategory = 'Uncategorized';
			var currentDashboardShortName = this.options.urlSettings.reportInfo.name;
			var $toolbarPanel = $('#izendaDashboardToolbar');
			var $tool = $('#izendaDashboardLinksPanel');
			var $menuDropdown = $('#izendaDashboardDropdownPanel');
			var $buttons = $('#izendaDashboardButtonsPanel');
			var availableWidth = $toolbarPanel.width() - $menuDropdown.width() - $buttons.width() - 120;
			var currentWidth = 0;
			var dashboardsInCurrentCategory = [];
			var activeIndex = -1;
			var $menu = $('.dashboard-links-btn');

			$menu.empty();
			$.each(_this.dashboardNavigation, function (iCat, cat) {
				var categoryName = cat.name;
				var dashboards = cat.dashboards;
				$menu.append('<li class="iz-dash-menu-catergory"><a>' + categoryName + '</a></li>');
				$.each(dashboards, function (iDash, dash) {
					var href = _this.options.urlSettings.urlDashboardsPage + '?rn=' + dash;
					href = href.replace('Dashboards.aspx', 'Dashboards-New.aspx');
					var parts = dash.split('\\');
					var name = dash;
					if (parts.length > 1) {
						name = parts[1];
					}
					$menu.append('<li><a href="' + href + '">' + name + '</a></li>');
					//store current category dashboards:
					if (currentDashboardCategory == categoryName) {
						var isCurrentDashboard = currentDashboardShortName.toLowerCase() == name.toLowerCase();
						dashboardsInCurrentCategory.push(dash);
						if (isCurrentDashboard) {
							activeIndex = dashboardsInCurrentCategory.length - 1;
						}
					}
				});
			});

			// draw current dashboard items:
			$tool.empty();
			$tool.show();
			if (dashboardsInCurrentCategory.length > 0) {
				var i = activeIndex;
				var dI = 0;
				var cnt = 0;
				var $rightDashboards = null,
					$leftDashboards = null;

				while (cnt < dashboardsInCurrentCategory.length) {
					// 
					var dash = null;
					if (dI < 0) {
						if (i + dI >= 0) {
							dash = dashboardsInCurrentCategory[i + dI];
							dI = -dI;
						} else {
							dI = -dI;
						}
					} else {
						if (i + dI < dashboardsInCurrentCategory.length) {
							dash = dashboardsInCurrentCategory[i + dI];
							dI = -dI - 1;
						} else {
							dI = -dI - 1;
						}
					}
					// draw dash
					if (dash != null) {
						var href = _this.options.urlSettings.urlDashboardsPage + '?rn=' + dash;
						href = href.replace('Dashboards.aspx', 'Dashboards-New.aspx');
						var parts = dash.split('\\');
						var name = dash;
						if (parts.length > 1) {
							name = parts[1];
						}
						var $itm = $('<li class="iz-dash-menu-item ' + (currentDashboardShortName.toLowerCase() == name.toLowerCase() ? ' active' : '') + '"><a href="' + href + '">' + name + '</a></li>');
						if (dI < 0) {
							// if shifted right (delta already changed sign)
							$tool.append($itm);
							currentWidth += $itm.width();
							if (currentWidth >= availableWidth) {
								if ($rightDashboards == null)
									$rightDashboards = $('<li class="dropdown"><a class="dropdown-toggle" data-toggle="dropdown" title="Show next dashboards"><b style="font-size: 12px;" class="glyphicon glyphicon-chevron-right"></b></a><ul class="dropdown-menu pull-right" role="menu"></ul></li>');
								$rightDashboards.find('.dropdown-menu').append($itm.clone());
								$itm.addClass('hidden');
							}
						} else {
							// if shifted left (delta already changed sign)
							$tool.prepend($itm);
							currentWidth += $itm.width();
							if (currentWidth >= availableWidth) {
								if ($leftDashboards == null) {
									$leftDashboards = $('<li class="dropdown"><a class="dropdown-toggle" data-toggle="dropdown" title="Show previous dashboards"><b style="font-size: 12px;" class="glyphicon glyphicon-chevron-left"></b></a><ul class="dropdown-menu pull-right" role="menu"></ul></li>');
								}
								$leftDashboards.find('.dropdown-menu').append($itm.clone());
								$itm.addClass('hidden');
							}
						}
						cnt++;
					}
				}
			}
			if ($rightDashboards != null)
				$tool.append($rightDashboards);
			if ($leftDashboards != null)
				$tool.prepend($leftDashboards);
		},

		/**
		 * Open new dashboard name modal dialog
		 */
		openNewDashboardModal: function (doneHandler) {
			// load navigation
			var _this = this;
			this.query.loadDashboardNavigation(this, function (dashboardNavigation) {
				if (dashboardNavigation == null)
					return;
				// dashboard modal events.
				var $nameInput = $('#izendaDashboardNameModalName');
				var $categoryNameInput = $('#izendaDashboardNameModalCategoryName');
				var $categoryNameSelect = $('#izendaDashboardNameModalCategory');
				var $okBtn = $('#izendaDashboardNameModalOk');
				$okBtn.addClass('disabled');
				$nameInput.val('');
				$categoryNameInput.parent().addClass('hidden');
				$categoryNameInput.val('');
				$categoryNameSelect.parent().removeClass('hidden');
				$categoryNameSelect.empty();
				$categoryNameSelect.append($('<option create-new>(Create New)</option>'));
				$.each(dashboardNavigation, function (iCat, cat) {
					var categoryName = cat.name;
					var isUndefined = categoryName && categoryName.toLowerCase() == 'uncategorized';
					$categoryNameSelect.append($('<option' + (isUndefined ? ' selected' : '') + '>' + categoryName + '</option>'));
				});
				$okBtn.one('click', function () {
					var reportName = $nameInput.val();
					var categoryName = $categoryNameSelect.val();
					if ($categoryNameSelect.parent().hasClass('hidden')) {
						categoryName = $categoryNameInput.val();
					}
					$('#izendaDashboardNameModal').modal('hide');
					if (typeof doneHandler == 'function') {
						doneHandler.apply(_this, [reportName, categoryName]);
					}
				});
				// open modal
				$('#izendaDashboardNameModal').modal();
			});
		},

		/**
		 * initialize add part modal dialog
		 */
		initializeModal: function () {
			var $root = this.$root;
			var $modal = $($("#modalTemplate").render().trim());
			$root.append($modal);
		},

		initializeModalConfirm: function () {
			var $root = this.$root;
			var $modal = $($("#modalConfirmTemplate").render().trim());
			$root.append($modal);
		},

		/**
		 * initialize tiles
		 */
		initializeTiles: function () {
			var reportFullName = this.options.urlSettings.reportInfo.fullName;
			// Load tiles from server:
			this.query.loadDashboardLayout(this, reportFullName, function (dashboardLayout) {
				// Prepare tiles:
				this.options.dashboardLayout = dashboardLayout;
				for (var i = 0; i < this.options.dashboardLayout.tiles.length; i++) {
					this.options.dashboardLayout.tiles[i] = this.initializeTileObject(this.options.dashboardLayout.tiles[i]);
				}
				// Check tiles for intersections:
				if (!this.checkTilesForIntersections(this.options.dashboardLayout.tiles)) {
					alert('Cannot render dashboard: found tile intersections!');
					return;
				}

				// Dashboard initialized! Now we can start drawing.
				this.drawDashboard();
			});
		},

		/**
		 * Initialize tile
		 */
		initializeTileObject: function (tile) {
			if (tile == null)
				throw 'Tile is null';
			tile = $.extend({}, $.fn.izendaDashboard.tileDefaults, tile);
			tile.id = 'IzendaDashboardTile' + (++tileIdCounter);
			return tile;
		},

		/**
		 * initialize grid parameters
		 */
		initializeGrid: function () {
			this.$containerDiv.css('width', '100%');
			var width = Math.floor(this.$containerDiv.width() / 12) * 12;
			this.tileWidth = width / 12;
			this.tileHeight = this.tileWidth > 100 ? this.tileWidth : 100;
			this.$containerDiv.width(width);
		},

		/**
		 * Remove windows resize handler
		 */
		deinitializeWindowResize: function () {
			$(window).off('resize.dashboard');
		},

		/**
		 * Initialize window resize handler.
		 */
		initializeWindowResize: function () {
			var _this = this;
			var resizeEnd = function () {
				if (new Date() - rtime < delta) {
					setTimeout(function () {
						resizeEnd.apply(_this);
					}, delta);
				} else {
					timeout = false;
					_this.$root.find('.iz-dash-tile .report').addClass('hidden');
					_this.updateDashboardSize();
					_this.updateDashboardContainerSize();
					_this.createDashboardNavigationMenu();
				}
			};
			previousWidth = $(window).width();
			this.deinitializeWindowResize();
			$(window).on('resize.dashboard', null, this, function (e) {
				/*$('#izendaDashboardLinksPanel').hide();*/
				rtime = new Date();
				if (timeout === false) {
					timeout = true;
					setTimeout(function () {
						resizeEnd.apply(e.data);
					}, delta);
				}
			});
		},

		/**
		 * Update dashboard size
		 */
		updateDashboardSize: function () {
			var _this = this;
			// initialize grid
			this.initializeGrid();
			// draw tile
			var tiles = this.options.dashboardLayout.tiles;
			for (var i = 0; i < tiles.length; i++) {
				var tile = tiles[i];
				var $tile = this.getTile$ById(tile.id);
				$tile.resizable('option', 'grid', [this.tileWidth, this.tileHeight]);
				this.updateTile$Size(tile, $tile);
				this.removeScroll($tile);
				this.updateTile(this.getTileByTile$($tile), true, false, function ($tt) {
					_this.setScrollNew($tt);
				});
				$tile.find('.report').removeClass('hidden');
			}
		},

		updateTile$Size: function (tile, $tile) {
			if (!this.isDashboardMobile()) {
				$tile.css('top', tile.y * this.tileHeight);
				$tile.css('left', tile.x * this.tileWidth);
				$tile.width(tile.width * this.tileWidth);
				$tile.height(tile.height * this.tileHeight);
			} else {
				$tile.css('top', this.getTileRelativePosition(tile) * 4 * this.tileHeight);
				$tile.css('left', 0);
				$tile.width(12 * this.tileWidth);
				$tile.height(4 * this.tileHeight);
			}
		},

		/**
		 * Update dashboard tile sizes
		 */
		updateTilesSizes: function () {
			if (this.isDashboardMobile())
				return;
			var tiles = this.options.dashboardLayout.tiles;
			for (var i = 0; i < tiles.length; i++) {
				var tile = tiles[i];
				var $tile = this.getTile$ById(tile.id);
				var x = Math.round($tile.position().left / this.tileWidth),
					y = Math.round($tile.position().top / this.tileHeight),
					width = Math.round($tile.width() / this.tileWidth),
					height = Math.round($tile.height() / this.tileHeight);
				tile.x = x;
				tile.y = y;
				tile.width = width;
				tile.height = height;
			}
		},

		/**
		 * drawDashboard:
		 */
		drawDashboard: function () {
			var _this = this;
			this.$root.find('#dashboardBodyContainer').remove();
			this.$containerDiv = $('<div id="dashboardBodyContainer" class="iz-dash-body-container"></div>');
			this.$root.append(this.$containerDiv);
			this.$containerDiv.width(this.$root.width());

			// initialize grid
			this.initializeGrid();

			// draw tile
			var tiles = this.options.dashboardLayout.tiles;
			for (var i = 0; i < tiles.length; i++) {
				var tile = tiles[i];
				var $tile = $($("#tileTemplate").render({
					tile: tile,
					dashboardOptions: this.options
				}).trim());
				$tile.attr('tileid', tile.id);

				if (!this.isDashboardMobile()) {
					$tile.css('top', tile.y * this.tileHeight);
					$tile.height(tile.height * this.tileHeight);
					$tile.css('left', tile.x * this.tileWidth);
					$tile.width(tile.width * this.tileWidth);
				} else {
					$tile.css('top', this.getTileRelativePosition(tile) * 4 * this.tileHeight);
					$tile.css('left', 0);
					$tile.width(12 * this.tileWidth);
					$tile.height(4 * this.tileHeight);
				}
				this.$containerDiv.append($tile);
				this.setTileHandlers($tile);
				this.updateTile(tile, true, false, function ($tt) {
					_this.setScrollNew($tt);
				});
			}

			// initialize add tile handler
			this.updateDashboardContainerSize();

			this.setAddTileHandler();

			// initialize auto refresh function
			this.initializeAutoRefresh();
		},

		/**
		 * Initialize autorefresh every 60 minutes
		 */
		initializeAutoRefresh: function () {
			var _this = this;
			_this.autoRefresh = {
				intervalsCount: 0,
				intervalHandle: null
			};
			_this.autoRefresh.intervalHandle = setInterval(function () {
				if (_this.autoRefresh.intervalsCount > 0) {
					_this.updateAllTiles();
				}
				_this.autoRefresh.intervalsCount = _this.autoRefresh.intervalsCount + 1;
			}, 60 * 60 * 1000);
		},

		/**
		 * Update dashboard container height
		 */
		updateDashboardContainerSize: function () {
			var $container = this.$containerDiv;
			var maxHeight = 0;
			$('.iz-dash-tile').each(function (iTile, tile) {
				var $tile = $(tile);
				if ($tile.position().top + $tile.height() > maxHeight) {
					maxHeight = $tile.position().top + $tile.height();
				}
			});
			$container.height(maxHeight + this.tileHeight);
		},

		/**
		 * Update all tiles
		 */
		updateAllTiles: function () {
			var _this = this;
			var tiles = _this.options.dashboardLayout.tiles;
			for (var i = 0; i < tiles.length; i++) {
				var tile = tiles[i];
				_this.updateTile(tile, true, true, function ($tt) {
					_this.setScrollNew($tt);
				});
			}
		},

		/**
		 * Update tile coordinates.
		 */
		updateTile: function (tile, reload, updateTileFromSource, doneHandler) {
			var $tile = this.getTile$ById(tile.id);
			$tile.find('.frame').css('overflow', 'hidden');
			// update tile content size
			this.fitTileUiContent($tile);
			// update coordinates
			if (!this.isDashboardMobile()) {
				var newX = Math.round($tile.position().left / this.tileWidth);
				var newY = Math.round($tile.position().top / this.tileHeight);
				var newWidth = Math.round($tile.width() / this.tileWidth);
				var newHeight = Math.round($tile.height() / this.tileHeight);
				tile.x = newX;
				tile.y = newY;
				tile.width = newWidth;
				tile.height = newHeight;
			}

			if (reload && tile.reportFullName != null) {
				// if need to reload
				var $frame = $tile.find('.frame');

				// add loading... splash
				$tile.find('.report')
					.empty()
					.append(this.createLoadingSplash());

				// select query function and run
				var queryFunction = updateTileFromSource
				  ? this.query.loadReportAndUpdate
				  : this.query.loadReport;
				queryFunction.apply(this.query, [{
					_this: this,
					$tile: $tile,
					tile: tile
				}, this.options.urlSettings, tile, $frame.width(), $frame.height(), function (htmlData) {
					// update tile links
					this._this.updateTileLinks(this.tile, this.$tile);

					// draw tile report
					var $body = this.$tile.find('.report');
					$body.empty();
					ReportScripting.loadReportResponse(htmlData, $body);

					var divs$ = $body.find('div.DashPartBody, div.DashPartBodyNoScroll');
					var $zerochartResults = divs$.find('.iz-zero-chart-results');
					if ($zerochartResults.length > 0) {
						$zerochartResults.closest('table').css('height', '100%');
						divs$.css('height', '100%');
					}
					if (typeof (AdHoc) != 'undefined' && typeof (AdHoc.Utility) != 'undefined'
					  && typeof (AdHoc.Utility.DocumentReady) == 'function') {
						AdHoc.Utility.DocumentReady();
					}
					if (typeof doneHandler == 'function') {
						doneHandler.apply(this._this, [this.$tile]);
					}
				}]);
				//this.query.loadReport();
			} else {
				if (typeof doneHandler == 'function') {
					doneHandler.apply(this, [$tile]);
				}
			}
		},

		/**
		 * Update tile links to designer, viewer and etc.
		 */
		updateTileLinks: function (tile, $tile) {
			var rsPage = this.options.urlSettings.urlRsPage;
			var viewerPage = this.options.urlSettings.urlReportViewer;
			var designerPage = this.options.urlSettings.urlReportDesigner;
			var rn = tile.reportName;
			if (tile.reportCategory != null && tile.reportCategory != '') {
				rn = tile.reportCategory + '\\' + rn;
			}
			$tile.find('.dd-tile-button-search').attr('href', viewerPage + '?rn=' + rn);
			$tile.find('.dd-tile-button-options').attr('href', designerPage + '?rn=' + rn);
			$tile.find('.dd-tile-button-export-html').attr('href', rsPage + '?rn=' + rn + '&p=htmlreport&print=1');
			$tile.find('.dd-tile-button-export-pdf').attr('href', rsPage + '?rn=' + rn + '&output=pdf');
			$tile.find('.dd-tile-button-export-xls').attr('href', rsPage + '?rn=' + rn + '&output=xls');
		},

		/**
		 * Set tile handlers
		 */
		setTileHandlers: function ($tile) {
			/*this.setTileTitleHandlers($tile);
			this.setTileFlipHandlers($tile);
			this.setTileRefreshHandler($tile);
			this.setTileRemoveHandler($tile);
			this.setSelectTopHandler($tile);
			this.setSelectPartHandler($tile);
			this.setEditHandlers($tile);*/
		},

		/**
		 * Apply handlers for edit dashboard tile
		 */
		setEditHandlers: function ($tile) {
			this.setResizeHandlers($tile);
			this.setDragHandlers($tile);
		},

		setTileTitleHandlers: function ($tile) {
			var _this = this;
			if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
				$tile.find('.title-container-background').css('display', 'block !important');
				$tile.find('.title-container').css('display', 'block !important');
			}

			$tile.find('.title-container-background, .title-container').click(function () {
				var $t = $(this).closest('.iz-dash-tile');
				$t.toggleClass('iz-dash-show-menu');
				$t.find('.flip-button').each(function (iBtn, btn) {
					var $titleBtn = $(btn);
					if (_this.getTile$ByInnerEl(btn).hasClass('iz-dash-show-menu')) {
						$titleBtn.addClass('hidden');
					} else {
						$titleBtn.removeClass('hidden');
					};
				});

				/*if ($title.height() == 35) {
				  $frame.css('border-radius', '3px');
				  $title.stop().animate({
					height: 0
				  }, 300, function() {
					$t.css('overflow', 'hidden');
					_this.setScrollNew($t);
				  });
				  $frame.stop().animate({
					'top': '5px',
					'height': $t.find('.animate-flip').height() - 10
				  }, 300);
				} else {
				  $frame.css('border-radius', '0 0 3px 3px');
				  $title.stop().animate({
					height: '35px'
				  }, 300, function () {
					$t.css('overflow', 'inherit');
					_this.setScrollNew($t);
				  });
				  $frame.stop().animate({
					'top': '40px',
					'height': $t.find('.animate-flip').height() - 45
				  }, 300);
				}*/
				return true;
			});
		},

		/**
		 * Apply handlers for add dashboard tile
		 */
		setAddTileHandler: function () {
			var _this = this;
			var $rootContraint = _this.$root.find('#dashboardBodyContainer');

			// helper functions:
			var normWidth = function (val) {
				return Math.floor(val / _this.tileWidth) * _this.tileWidth;
			};
			var normHeight = function (val) {
				return Math.floor(val / _this.tileHeight) * _this.tileHeight;
			};
			var getNormalizedPosition = function (ev) {
				return {
					x: normWidth(ev.pageX - $rootContraint.offset().left),
					y: normHeight(ev.pageY - $rootContraint.offset().top)
				};
			};
			var addNewTile = function () {
				var addTile = _this.temp.addtile;
				if (addTile.$addTile == null) {
					var $tile = $($("#tileTemplate").render({
						tile: $.fn.izendaDashboard.tileDefaults,
						dashboardOptions: _this.options
					}).trim());
					$tile.attr('tileid', 0);
					_this.temp.addtile.prevX = normWidth(addTile.relativeX);
					_this.temp.addtile.prevY = normHeight(addTile.relativeY);
					_this.temp.addtile.prevW = _this.tileWidth;
					_this.temp.addtile.prevH = _this.tileHeight;
					$tile.css('left', addTile.prevX);
					$tile.css('top', addTile.prevY);
					$tile.width(addTile.prevW);
					$tile.height(addTile.prevH);
					$tile.find('.frame').addClass('hidden');
					$tile.find('.flippy-front, .flippy-back').removeClass('flipInY');
					$tile.find('.flippy-front,.flippy-back').css('background-color', 'rgba(50,205,50, 0.5)');
					$rootContraint.append($tile);
					_this.temp.addtile.$addTile = $tile;
					_this.temp.addtile.cnt = 21;
					return true;
				}
				return false;
			};

			// ===============================================================
			// MOUSE HANDLERS:
			// ===============================================================

			// start draw new tile
			$rootContraint.mousedown(function (e) {
				// if it is 
				var $target = $(e.target);
				// if we click not the background - return.
				if (!$target.hasClass('iz-dash-body-container')
					&& !$target.hasClass('dashboard-grid')
					&& !$target.hasClass('iz-dash-body-overlay')
					&& !$target.hasClass('tile-grid-cell')
					&& !$target.closest('.tile-grid-cell').length > 0
				  )
					return;
				// if we click after started new tile - return.
				if (_this.temp.addtile != null && _this.temp.addtile.started) {
					return;
				}
				// start creating new tile.
				isEditActionPerformsNow = true;
				_this.temp.addtile = {
					cnt: 0,
					started: true,
					startedDraw: false,
					relativeX: e.pageX - $rootContraint.offset().left,
					relativeY: e.pageY - $rootContraint.offset().top,
					x: e.pageX,
					y: e.pageY,
					$addTile: null
				};
				_this.showTileGrid();
				_this.deinitializeWindowResize();
			});

			// select tile grid item highlight
			$rootContraint.mousemove(function (e) {
				e.preventDefault();
				var addTile = _this.temp.addtile;

				// show grid if not started
				if (addTile == null || !addTile.started) {
					if (!isEditActionPerformsNow) {
						var $target = typeof (e.target) != 'undefined' ? $(e.target) : null;
						var isOverEmptySpace = $target != null
						  && ($target.attr('id') == 'dashboardBodyContainer'
							|| $target.hasClass('dashboard-grid')
							|| $target.hasClass('iz-dash-body-overlay')
							|| $target.hasClass('tile-grid-cell')
							|| $target.closest('.tile-grid-cell').length > 0);
						if (isOverEmptySpace) {
							var tileXy = getNormalizedPosition(e);
							_this.showTileGridShadow({
								x: tileXy.x,
								y: tileXy.y,
								width: _this.tileWidth,
								height: _this.tileHeight
							}, '#000', true);
						} else {
							_this.hideTileGridShadow();
						}
					}
					return;
				}

				addNewTile();
				if (addTile.cnt > 20) {
					var l, t, w, h;
					_this.hideTileGridShadow();
					if (addTile.x > e.pageX || addTile.y > e.pageY) {
						l = normWidth(addTile.relativeX);
						t = normHeight(addTile.relativeY);
						w = _this.tileWidth;
						h = _this.tileHeight;
					} else {
						l = normWidth(addTile.relativeX);
						w = normWidth(e.pageX - addTile.x) + _this.tileWidth;
						if (w <= _this.tileWidth) w = _this.tileWidth;
						t = normHeight(addTile.relativeY);
						h = normHeight(e.pageY - addTile.y) + _this.tileHeight;
						if (h <= _this.tileHeight) h = _this.tileHeight;
					}
					var hasIntersection = false;
					for (var i = 0; i < _this.options.dashboardLayout.tiles.length; i++) {
						var tt = _this.options.dashboardLayout.tiles[i];
						var $tt = _this.getTile$ById(tt.id);
						var bbox1 = {
							left: $tt.position().left,
							top: $tt.position().top,
							width: $tt.width(),
							height: $tt.height()
						};
						var bbox2 = {
							left: l,
							top: t,
							width: w,
							height: h
						};
						if (_this.checkBboxIntersects(bbox1, bbox2)) {
							hasIntersection = true;
						}
					}
					if (!hasIntersection) {
						addTile.$addTile.css('left', l + 'px');
						addTile.$addTile.css('top', t + 'px');
						addTile.$addTile.width(w);
						addTile.$addTile.height(h);
						_this.fitTileUiContent(addTile.$addTile);
					}

					// update tile grid.
					var p = addTile.$addTile.position();
					var y = Math.round(p.top / _this.tileHeight);
					var x = Math.round(p.left / _this.tileWidth);
					_this.updateTileGrid({
						minY: y,
						maxY: y + Math.round(addTile.$addTile.height() / _this.tileHeight),
						minX: x,
						maxX: x + Math.round(addTile.$addTile.width() / _this.tileWidth)
					});
					addTile.cnt = 0;
				} else {
					addTile.cnt++;
				}
				//}
			});

			// mouse button released: create tile.
			$rootContraint.mouseup(function () {
				if (_this.temp.addtile == null)
					return;
				_this.hideTileGrid();
				_this.temp.addtile.started = false;
				/*if (!_this.temp.addtile.startedDraw) {
				  _this.temp.addtile = null;
				  return;
				}*/
				// create tile if not exist
				addNewTile();
				_this.fitTileUiContent(_this.temp.addtile.$addTile);
				var $newTile = _this.temp.addtile.$addTile;

				_this.temp.addtile = null;
				isEditActionPerformsNow = false;
				// add tile
				var tile = $.extend({}, $.fn.izendaDashboard.tileDefaults);
				tile.id = 'IzendaDashboardTile' + (++tileIdCounter);
				_this.options.dashboardLayout.tiles.push(tile);
				$newTile.attr('tileid', tile.id);
				$newTile.find('.frame').removeClass('hidden');
				$newTile.find('.flippy-front, .flippy-back').addClass('flipInY');
				$newTile.find('.flippy-front,.flippy-back').css('background-color', '#fff');
				// add tile btn
				var $selectTileFrontBtn = $($("#tileAddButtonTemplate").render($.fn.izendaDashboard.tileDefaults).trim());
				$newTile.find('.report').append($selectTileFrontBtn);
				_this.setTileHandlers($newTile);
				_this.updateDashboardContainerSize();
				_this.updateTilesSizes();
				_this.initializeWindowResize();
			});
		},

		/**
		 * Set resize and drag handlers
		 */
		setResizeHandlers: function ($tile) {
			var _this = this;
			var $animates = this.$root.find('.animate-flip');
			$tile.resizable({
				grid: [this.tileWidth, this.tileHeight],
				handles: 'n, e, s, w, se',
				start: function (event, ui) {
					_this.deinitializeWindowResize();
					var $currentTileUi = ui.element;
					var $t = _this.getTile$ByInnerEl($currentTileUi);
					$t.css('z-index', 10);
					$t.find('.frame').addClass('hidden');
					$t.find('.flippy-front, .flippy-back').removeClass('flipInY');
					$t.css('opacity', 0.8);
					_this.removeScroll($t);
					_this.showTileGrid();
					isEditActionPerformsNow = true;
				},
				resize: function (event, ui) {
					var $currentTileUi = ui.element;
					var $t = _this.getTile$ByInnerEl($currentTileUi);
					var tile = _this.getTileByTile$($t);
					_this.fitTileUiContent($t);
					$animates.find('.flippy-front,.flippy-back').css('background-color', '#fff');
					$t.find('.flippy-front,.flippy-back').css('background-color', 'rgba(50,205,50, 0.5)');
					if (_this.checkTileIntersects($t, tile.id) || _this.checkTileMovedToOuterSpace($t)) {
						$t.find('.flippy-front,.flippy-back').css('background-color', 'rgba(220,20,60,0.5)');
					}
					// update tile grid.
					var p = $t.position();
					var y = Math.round(p.top / _this.tileHeight);
					var x = Math.round(p.left / _this.tileWidth);
					_this.updateTileGrid({
						minY: y,
						maxY: y + Math.round($t.height() / _this.tileHeight),
						minX: x,
						maxX: x + Math.round($t.width() / _this.tileWidth)
					});
				},
				stop: function (event, ui) {
					_this.hideTileGrid();
					var $currentTileUi = ui.element;
					var $t = _this.getTile$ByInnerEl($currentTileUi);
					var tile = _this.getTileByTile$($t);
					$t.css('z-index', 1);
					$t.find('.frame').removeClass('hidden');
					$t.find('.flippy-front, .flippy-back').addClass('flipInY');
					$animates.find('.flippy-front,.flippy-back').css('background-color', '#fff');
					if (_this.checkTileIntersects($t, tile.id) || _this.checkTileMovedToOuterSpace($t)) {
						// revert if intersects
						$currentTileUi.animate({
							left: ui.originalPosition.left,
							top: ui.originalPosition.top,
							width: ui.originalSize.width,
							height: ui.originalSize.height
						}, 200, function () {
							var $ct = $(this);
							_this.removeScroll($ct);
							_this.updateTile(_this.getTileByTile$($ct), false, false, function ($tt) {
								_this.setScrollNew($tt);
							});
						});
					} else {
						_this.removeScroll($t);
						_this.updateTile(_this.getTileByTile$($t), true, false, function ($tt) {
							_this.setScrollNew($tt);
						});
					}
					$t.find('.flippy-front .report, .flippy-back .report').removeClass('hidden');
					$t.css('opacity', 1);
					_this.updateDashboardContainerSize();
					_this.updateTilesSizes();
					_this.initializeWindowResize();
					isEditActionPerformsNow = false;
				}
			});
		},

		/**
		 * Apply drag-n-drop handler for tile
		 */
		setDragHandlers: function ($tile) {
			// draggable
			var _this = this;

			$tile.draggable({
				//grid: [this.tileWidth, this.tileHeight],
				stack: '.iz-dash-tile',
				handle: '.title-container',
				helper: 'clone',
				distance: 10,
				start: function (event, ui) {
					_this.deinitializeWindowResize();
					var $target = $(event.target),
						$helper = ui.helper,
						targetPos = $target.position(),
						helperPos = $helper.position(),
						targetWidth = $target.width(),
						targetHeight = $target.height();
					_this.temp.drag = {
						helperX: helperPos.left,
						helperY: helperPos.top,
						targetX: targetPos.left,
						targetY: targetPos.top,
						targetWidth: targetWidth,
						targetHeight: targetHeight
					};

					$helper.find('.flippy-front, .flippy-back').removeClass('flipInY');
					$helper.find('.frame').remove();
					$helper.css('z-index', 1000);
					$helper.css('opacity', 1);

					_this.removeScroll($target);
					_this.showTileGrid();
					_this.showTileGridShadow({
						x: targetPos.left,
						y: targetPos.top,
						width: targetWidth,
						height: targetHeight
					});
					isEditActionPerformsNow = true;
				},

				drag: function (event, ui) {
					var $flippies = _this.$containerDiv.find('.animate-flip > .flippy-front, .animate-flip > .flippy-back');
					var $helper = ui.helper;
					var helperPos = $helper.position();
					var $t = _this.getTile$ByInnerEl($helper);
					var tile = _this.getTileByTile$($t);

					// update tile grid.
					var y = Math.round(helperPos.top / _this.tileHeight);
					var x = Math.round(helperPos.left / _this.tileWidth);
					_this.updateTileGridWithShadow({
						minY: y,
						maxY: y + tile.height,
						minX: x,
						maxX: x + tile.width
					}, {
						x: helperPos.left,
						y: helperPos.top,
						width: $helper.width(),
						height: $helper.height()
					});

					//var $target = _this.getUnderlyingTile(shadowPos.left + $shadow.width() / 2, shadowPos.top + $shadow.height() / 2, tile);
					var $target = _this.getUnderlyingTile(event.pageX, event.pageY, tile);

					$flippies.css('background-color', '#fff');
					$helper.find('.flippy-front, .flippy-back').css('background-color', 'rgba(50,205,50, 0.3)');
					if ($target != null) {
						$target.find('.flippy-front, .flippy-back').css('background-color', 'rgba(50,205,50, 1)');
					} else {
						if (_this.checkTileIntersects($t, tile.id) || _this.checkTileMovedToOuterSpace($helper, 10)) {
							$helper.find('.flippy-front, .flippy-back').css('background-color', 'rgba(220,20,60,0.2)');
						}
					}

					_this.temp.drag.helperX = helperPos.left;
					_this.temp.drag.helperY = helperPos.top;
				},

				stop: function (event, ui) {
					var $flippies = _this.$containerDiv.find('.animate-flip > .flippy-front, .animate-flip > .flippy-back');
					var $helper = ui.helper;
					var $source = $(event.target);
					var $t = _this.getTile$ByInnerEl($source);
					var tile = _this.getTileByTile$($t);

					//var $target = _this.getUnderlyingTile(shadowPos.left + $shadow.width() / 2, shadowPos.top + $shadow.height() / 2, tile);
					var $target = _this.getUnderlyingTile(event.pageX, event.pageY, tile);
					isEditActionPerformsNow = false;
					$helper.css('opacity', 1);
					$flippies.css('background-color', '#fff');
					if ($target != null) {
						_this.hideTileGrid();
						$helper.remove();
						_this.swapTiles($source, $target, function ($swappedTile1, $swappedTile2) {
							$swappedTile1.find('.frame').show();
							$swappedTile2.find('.frame').show();
							$swappedTile1.css('z-index', 0);
							$swappedTile2.css('z-index', 0);
							var tileSizeChanged = Math.abs($swappedTile1.width() - $swappedTile2.width()) > 5
							  || Math.abs($swappedTile1.height() - $swappedTile2.height()) > 5;
							_this.removeScroll($swappedTile1);
							_this.removeScroll($swappedTile2);
							_this.updateTile(_this.getTileByTile$($swappedTile1), tileSizeChanged, false, function ($tt) {
								_this.setScrollNew($tt);
							});
							_this.updateTile(_this.getTileByTile$($swappedTile2), tileSizeChanged, false, function ($tt) {
								_this.setScrollNew($tt);
							});
						});
						_this.$root.find('.animate-flip').css('overflow', 'inherit');
						_this.updateDashboardContainerSize();
						_this.updateTilesSizes();
						_this.initializeWindowResize();
						return;
					}
					// if intersects or out of dashboard space
					if (_this.checkTileIntersects($helper, tile.id) || _this.checkTileMovedToOuterSpace($helper, 10)) {
						_this.hideTileGrid();
						_this.$root.find('.animate-flip').css('overflow', 'inherit');
						_this.updateDashboardContainerSize();
						_this.updateTilesSizes();
						_this.initializeWindowResize();
						return;
					}

					// 
					var pos = $helper.position();
					$helper.remove();
					$t.animate({
						left: Math.round(pos.left / _this.tileWidth) * _this.tileWidth,
						top: Math.round(pos.top / _this.tileHeight) * _this.tileHeight
					}, 500, function () {
						var $tt1 = $(this);
						$tt1.css('z-index', 0);
						_this.hideTileGrid();
						_this.removeScroll($tt1);
						_this.updateTile(_this.getTileByTile$($tt1), false, false, function ($tt) {
							_this.setScrollNew($tt);
						});
						_this.$root.find('.animate-flip').css('overflow', 'inherit');
						_this.updateDashboardContainerSize();
						_this.updateTilesSizes();
						_this.initializeWindowResize();
					});
				}
			});
		},

		/**
		 * Swap tiles
		 */
		swapTiles: function ($tile1, $tile2, swapCompleted) {
			var _this = this;
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
				else if (typeof (swapCompleted) == 'function') {
					$tile1.find('.frame').show();
					$tile2.find('.frame').show();
					swapCompleted.apply(_this, [$tile1, $tile2]);
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
				else if (typeof (swapCompleted) == 'function') {
					$tile1.find('.frame').show();
					$tile2.find('.frame').show();
					swapCompleted.apply(_this, [$tile1, $tile2]);
				}
			});
		},

		/**
		 * Find underlying tile
		 */
		getUnderlyingTile: function (x, y, testingTile) {
			var _this = this;
			var $target = null;
			var targetTile;
			this.$root.find('.iz-dash-tile').each(function (iT, t) {
				var $t = $(t);
				var tile = _this.getTileByTile$($t);
				if (tile == null || tile.id != testingTile.id) {
					var tilePosition = $t.offset();
					if (tilePosition.left <= x && tilePosition.left + $t.width() >= x
						  && tilePosition.top <= y && tilePosition.top + $t.height() >= y) {
						targetTile = tile;
						$target = $t;
					}
				}
			});
			return $target;
		},

		/**
		 * Select report part
		 */
		setSelectPartHandler: function ($tile) {
			var _this = this;
			$tile.find('.dd-tile-button-select-part, .iz-dash-select-report-front-btn').click(function (e) {
				e.preventDefault();
				var $currentTile = _this.getTile$ByInnerEl(this);
				_this.temp.$selectReportPartTile = $currentTile;

				// load report sets:
				var $modal = $('#selectPartModal');
				$modal.modal();
				_this.addSplashScreenToModal();
				_this.query.loadReportSetCategory(_this, 'Uncategorized', function (reportSets) {
					_this.addReportsToModal(reportSets);
					_this.addCategoriesToModal(reportSets);
				});
			});
		},

		/**
		 * Tile refresh button
		 */
		setTileRefreshHandler: function ($tile) {
			var _this = this;
			$tile.find('.flip-trigger-refresh, .dd-tile-button-refresh').click(function (e) {
				e.preventDefault();
				var $t = _this.getTile$ByInnerEl(this);
				_this.removeScroll($t);
				_this.flipTileFront($t, function (currentTile) {
					this.updateTile(currentTile, true, true, function ($tt) {
						_this.setScrollNew($tt);
					});
				});
				/*_this.updateTile(_this.getTileByTile$($t), true, false, function ($tt) {
				  _this.setScrollNew($tt);
				});*/
				return false;
			});
		},

		/**
		 * Tile remove button
		 */
		setTileRemoveHandler: function ($tile) {
			var _this = this;
			$tile.find('.flip-trigger-remove').off('click.dashboard');
			$tile.find('.flip-trigger-remove').on('click.dashboard', function (e) {
				e.preventDefault();
				var $this = $(this);
				var $t = _this.getTile$ByInnerEl(this);
				var $confirmBtn = $t.find('.flip-trigger-confirm-delete');
				$confirmBtn.width(0);
				$confirmBtn.show();
				$confirmBtn.animate({
					width: 255
				}, 200);

				return false;
				/*var $t = _this.getTile$ByInnerEl(this);
				var $confirm = _this.$root.find('#izDashConfirmModal');
				$confirm.find('.modal-header').html('<b>Tile Remove Confirmation Dialog</b>');
				$confirm.find('.modal-body').html('<h4>Please confirm tile remove operation.</h4>');
				var $okBtn = $confirm.find('#izDashConfirmModalOk');
				$okBtn.off('click.dashboard');
				$okBtn.on('click.dashboard', $t, function (e1) {
					// tile remove confirmed
					var $t1 = e1.data;
				});
				$confirm.modal('show');
				return false;*/
			});

			$tile.find('.flip-trigger-confirm-delete > .confirm').click(function (e) {
				e.preventDefault();
				var $t = _this.getTile$ByInnerEl(this);
				var t = _this.getTileByTile$($t);
				_this.query.removeReportPart({
					_this: _this,
					tile: t,
					$tile: $t
				}, t.reportFullName, function () {
					// remove tile from collection
					var _this2 = this._this;
					_this2.options.dashboardLayout.tiles = $.grep(_this2.options.dashboardLayout.tiles, function (value) {
						return value.id != t.id;
					});

					this.$tile.fadeOut(200, function () {
						_this2.removeScroll($(this));
						$(this).remove();
						_this2.initializeFilters();
						// if it was last tile: create new empty tile.
						if (_this2.options.dashboardLayout.tiles.length == 0) {

							// create tile object
							var addTile = $.extend({}, $.fn.izendaDashboard.tileDefaults);
							addTile.id = 'IzendaDashboardTile' + (++tileIdCounter);
							addTile.width = 12;
							addTile.height = 3;
							_this2.options.dashboardLayout.tiles.push(addTile);

							// create tile html
							var $addTile = $($("#tileTemplate").render({
								tile: $.fn.izendaDashboard.tileDefaults,
								dashboardOptions: _this2.options
							}).trim());
							_this2.$root.find('#dashboardBodyContainer').append($addTile);
							$addTile.attr('tileid', addTile.id);
							_this2.updateTile$Size(addTile, $addTile);

							// append select report button
							var $selectTileFrontBtn = $($("#tileAddButtonTemplate").render($.fn.izendaDashboard.tileDefaults).trim());
							$addTile.find('.report').append($selectTileFrontBtn);
							// prepare tile for use
							_this2.setTileHandlers($addTile);
							_this2.fitTileUiContent($addTile);
							_this2.updateDashboardContainerSize();
							_this2.updateTilesSizes();
						}
					});
				});
				return false;
			});
			$tile.find('.flip-trigger-confirm-delete > .cancel').click(function (e) {
				e.preventDefault();
				var $this = $(this);
				var $t = _this.getTile$ByInnerEl(this);
				var $ft = $t.find('.flip-trigger-confirm-delete');
				$ft.animate({
					width: 0
				}, 200, function() {
					$(this).width(0);
					$(this).hide();
				});
				return false;
			});
		},

		/**
		 * Select top handler
		 */
		setSelectTopHandler: function ($tile) {
			// helper functions
			var setTopValue = function ($currentTile, value) {
				var $currentValSlider = $currentTile.find('.iz-dash-tile-top-slider-value');
				if (value == 101)
					$currentValSlider.text('All');
				else
					$currentValSlider.text(value);
			};
			var updateTopValue = function ($t, v) {
				var t = _this.getTileByTile$($t);
				if (t == null)
					return;

				t.top = typeof v == 'string' ? parseInt(v) : v;
				if (t.top == 101)
					t.top = -1;
				_this.query.updateReportTop({
					_this: _this,
					$tile: $t
				}, t.reportFullName, v, function () {
					this._this.removeScroll(this.$tile);
					this._this.flipTileFront(this.$tile, function (currentTile) {
						this.updateTile(currentTile, true, false, function ($tt) {
							_this.setScrollNew($tt);
						});
					});
				});
			};

			// calculate current top
			var tile = this.getTileByTile$($tile);
			if (tile == null)
				return;
			var _this = this;
			var currentTopFixed = tile.top;
			if (currentTopFixed == -999) {
				currentTopFixed = -1;
			}
			if (currentTopFixed == -1) {
				currentTopFixed = 101;
			}
			setTopValue($tile, currentTopFixed);
			var $topInput = $tile.find('.slider');
			$topInput.attr('value', currentTopFixed);
			$topInput.attr('data-slider-value', currentTopFixed);

			// new ----------->

			var $slider = $tile.find('.slider');
			$slider.bootstrapSlider().on('slide', function (ev) {
				setTopValue(_this.getTile$ByInnerEl(this), ev.value);
			}).on('slideStop', function (ev) {
				var $t = _this.getTile$ByInnerEl(this);
				setTopValue($t, ev.value);
				updateTopValue($t, ev.value);
			});
			return;

			/*// old ----------->
			var $select = $tile.find('.iz-dash-tile-select-record-count');
			var $input = $tile.find('.iz-dash-tile-input-record-count');
			// fill records count select
			var records = '';
			if (currentTop == -999) {
			  records += '<option value="-999" selected="selected">All (no scrollbar)</option>';
			} else {
			  records += '<option value="-999">All (no scrollbar)</option>';
			}
			if (currentTop == -1) {
			  records += '<option value="-1" selected="selected">All</option>';
			} else {
			  records += '<option value="-1">All</option>';
			}
			for (var i = 1; i <= 100; i++) {
			  if (currentTop == i) {
				records = records + '<option value="' + i + '" selected="selected">' + i + '</option>';
			  } else {
				records = records + '<option value="' + i + '">' + i + '</option>';
			  }
			}
			$select.html(records);
	  
			// top changed handler
			var handler = function ($t, v) {
			  var t = _this.getTileByTile$($t);
			  if (t == null)
				return;
	  
			  t.top = typeof v == 'string' ? parseInt(v) : v;
			  _this.query.updateReportTop({
				_this: _this,
				$tile: $t
			  }, t.reportFullName, v, function () {
				this._this.removeScroll(this.$tile);
				this._this.flipTileFront(this.$tile, function (currentTile) {
				  this.updateTile(currentTile, true, false, function ($tt) {
					_this.setScrollNew($tt);
				  });
				});
			  });
			};
	  
			// change select value
			$select.change(function () {
			  var val = $select.val();
			  if (val == -999) {
				$input.val('All (no scrollbar)');
			  } else if (val == -1) {
				$input.val('All');
			  } else {
				$input.val($select.val());
			  }
			  var $currentTile = _this.getTile$ByInnerEl(this);
			  handler($currentTile, val);
			});
			// change input value
			$input.change(function () {
			  $select.val($input.val());
			  var val = $select.val();
			  var $currentTile = _this.getTile$ByInnerEl(this);
			  handler($currentTile, val);
			});
			$input.val(currentTopText);*/
		},

		/**
		 * Add basic tile handlers
		 */
		setTileFlipHandlers: function ($tile) {
			var _this = this;
			// set handlers
			$tile.find('.animate-flip .flippy-front .flip-trigger').click(function (e) {
				var $t = _this.getTile$ByInnerEl($(this));
				_this.flipTileBack($t);
				return false;
			});
			$tile.find('.animate-flip .flippy-back .flip-trigger').click(function (e) {
				var $t = _this.getTile$ByInnerEl($(this));
				_this.removeScroll($t);
				_this.flipTileFront($t, function (t) {
					var $tt = _this.getTile$ById(t.id);
					_this.setScrollNew($tt);
				});
				return false;
			});
		},

		/**
		 * Flip tile to the front side
		 */
		flipTileFront: function ($tile, additionalCallback) {
			var _this = this;
			var tile = this.getTileByTile$($tile);
			var showClass = 'animated fast flipInY';
			var hideClass = 'animated fast flipOutY';
			var $front = $tile.find('.flippy-front');
			var $back = $front.parent().find('.flippy-back');
			$back.addClass(hideClass);
			$front.removeClass(showClass);
			//$tile.find('.frame').css('overflow-y', 'hidden');
			$tile.find('.ui-icon-gripsmall-diagonal-se').hide();
			setTimeout(function () {
				$front.css('display', 'block').addClass(showClass);
				$back.css('display', 'none').removeClass(hideClass);
				_this.fitTileUiContent($tile);
			}, 1);
			setTimeout(function () {
				$tile.find('.ui-icon-gripsmall-diagonal-se').show();
				if (typeof additionalCallback != 'undefined')
					additionalCallback.apply(_this, [tile]);
			}, 200);
		},

		/**
		 * Create loading splash screen
		 */
		createLoadingSplash: function() {
			var $loadingSplashScreen = $($("#loadingSplashTemplate").render({
				dashboardOptions: this.options
			}).trim());
			return $loadingSplashScreen;
		},

		/**
		 * Flip tile to the front side
		 */
		flipTileBack: function ($tile, additionalCallback) {
			var _this = this;
			this.removeScroll($tile);
			var showClass = 'animated fast flipInY';
			var hideClass = 'animated fast flipOutY';
			var $front = $tile.find('.flippy-front');
			var $back = $tile.find('.flippy-back');
			$front.addClass(hideClass);
			$back.removeClass(showClass);
			//$tile.find('.frame').css('overflow-y', 'hidden');
			$tile.find('.ui-icon-gripsmall-diagonal-se').hide();
			setTimeout(function () {
				$back.css('display', 'block').addClass(showClass);
				$front.css('display', 'none').removeClass(hideClass);
				_this.fitTileUiContent($tile);
			}, 1);
			setTimeout(function () {
				$tile.find('.ui-icon-gripsmall-diagonal-se').show();
				//$tile.find('.frame').css('overflow-y', 'auto');
				_this.setScrollNew($tile);
				if (typeof additionalCallback != 'undefined')
					additionalCallback.apply(_this, [tile]);
			}, 200);
		},

		/**
		 * update tile grid with shadow
		 */
		updateTileGrid: function (additionalBbox) {
			var bbox = this.getTilesBBox(this.options.dashboardLayout.tiles);
			this.$containerDiv.height((Math.max(additionalBbox.maxY, bbox.maxY) + 3) * this.tileHeight);

			var $grid = this.$root.find('.dashboard-grid');
			$grid.css('background-size', this.tileWidth + 'px ' + this.tileHeight + 'px, ' + this.tileWidth + 'px ' + this.tileHeight + 'px');
		},

		/**
		 * update tile grid without shadow
		 */
		updateTileGridWithShadow: function (additionalBbox, helperPosition) {
			if (typeof helperPosition == 'undefined' || helperPosition == null)
				throw 'helper position cannot be null';
			if (typeof this.temp.drag == 'undefined' || typeof this.temp.drag.helperX == 'undefined' || typeof this.temp.drag.helperY == 'undefined')
				throw 'this.temp.drag.helperX & this.temp.drag.helperY should be defined';

			var bbox = this.getTilesBBox(this.options.dashboardLayout.tiles);
			this.$containerDiv.height((Math.max(additionalBbox.maxY, bbox.maxY) + 3) * this.tileHeight);

			var previousX = this.temp.drag.helperX,
				previousY = this.temp.drag.helperY;
			// update only when drag helper cross the grid cell
			if (Math.round(helperPosition.x / this.tileWidth) != Math.round(previousX / this.tileWidth) ||
				  Math.round(helperPosition.y / this.tileHeight) != Math.round(previousY / this.tileHeight)) {
				var $grid = this.$root.find('.dashboard-grid');
				$grid.css('background-size', this.tileWidth + 'px ' + this.tileHeight + 'px, ' + this.tileWidth + 'px ' + this.tileHeight + 'px');

				// move shadow
				this.showTileGridShadow({
					x: additionalBbox.minX * this.tileWidth,
					y: additionalBbox.minY * this.tileHeight,
					width: helperPosition.width,
					height: helperPosition.height
				});
			}
		},

		/**
		 * Hide tile grid shadow
		 */
		hideTileGridShadow: function () {
			var $gridPlaceholder = this.$root.find('#dashboardBodyContainer');
			var $shadow = $gridPlaceholder.find('.tile-grid-cell.shadow');
			$shadow.hide();
		},

		/**
		 * Show grid shadow
		 */
		showTileGridShadow: function (shadowBox, backgroundColor, showPlusIcon) {
			var $gridPlaceholder = this.$root.find('#dashboardBodyContainer');
			var $shadow = $gridPlaceholder.find('.tile-grid-cell.shadow');

			// create shadow if doesn't exists
			if ($shadow.length == 0) {
				$shadow = $('<div class="tile-grid-cell shadow"></div>').css({
					'opacity': 0.2,
					'background-color': (typeof (backgroundColor) == 'string' ? backgroundColor : '#cddbf6')
				});
				if (typeof (showPlusIcon) == 'boolean' && showPlusIcon) {
					var $plus = $('<div class="iz-dash-select-report-front-container">' +
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
				'left': shadowBox.x,
				'top': shadowBox.y,
				'width': shadowBox.width,
				'height': shadowBox.height
			});
			$shadow.show();
		},

		/**
		 * Show the grid under tile
		 */
		showTileGrid: function () {
			var _this = this;
			var $gridPlaceholder = _this.$root.find('#dashboardBodyContainer');

			var $grid = _this.getGrid();
			if ($grid == null) {
				_this.$grid = $('<div></div>')
				  .addClass('dashboard-grid')
				  .hide();
				_this.$containerDiv.prepend(_this.$grid);
			}
			_this.$grid.css('background-size', _this.tileWidth + 'px ' + _this.tileHeight + 'px, ' + _this.tileWidth + 'px ' + _this.tileHeight + 'px');

			// append grid
			$gridPlaceholder.prepend($grid);

			if (typeof _this.$overlay == 'undefined') {
				_this.$overlay = $('<div></div>')
				  .addClass('iz-dash-body-overlay')
				  .css('display', 'none');
				_this.$root.prepend(_this.$overlay);
			}
			_this.$overlay.show();

			_this.$grid.show();
		},

		/**
		 * Hide tile grid
		 */
		hideTileGrid: function () {
			var _this = this;
			this.$root.find('.tile-grid-cell').remove();
			if (typeof _this.$overlay != 'undefined') {
				_this.$overlay.hide();
			}
			var $grid = _this.getGrid();
			if ($grid != null) {
				$grid.remove();
				_this.$grid = null;
			}
		},

		/**
		 * Other tiles
		 */
		getOtherTiles: function (ts, t) {
			if (t == null)
				return ts;
			var otherTiles = [];
			for (var i = 0; i < ts.length; i++) {
				if (ts[i].id != t.id)
					otherTiles.push(ts[i]);
			}
			return otherTiles;
		},

		/**
		 * Check tile is outside the dashboard
		 */
		checkTileMovedToOuterSpace: function ($tile, sencitivity) {
			var precision = sencitivity;
			if (typeof (sencitivity) == 'undefined' || sencitivity == null)
				precision = 0;
			var tp = $tile.position();
			if (tp.left + $tile.width() > this.tileWidth * 12 + precision || tp.left < -precision || tp.top < -precision)
				return true;
			return false;
		},

		/**
		 * Check 2 tiles for intersects
		 */
		checkTileIntersects: function ($tile, tileId, $tileShadow) {
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
				return !(bLeft > aRight
					|| bRight < aLeft
					|| bTop > aBottom
					|| bBottom < aTop
				);
			};
			// check
			var tile = this.getTileById(tileId);
			var $tileToTest = (typeof ($tileShadow) != 'undefined' && $tileShadow != null) ? $tileShadow : $tile;
			var otherTiles = this.getOtherTiles(this.options.dashboardLayout.tiles, tile);
			for (var i = 0; i < otherTiles.length; i++) {
				var oTile = otherTiles[i];
				var $oTile = this.getTile$ById(oTile.id);
				if (hitTest($tileToTest, $oTile, 30)) {
					var o1 = $oTile.offset();
					var o2 = $tileToTest.offset();
					return true;
				}
			}
			return false;
		},

		/**
		 * Check item intersects with bbox
		 */
		checkItem$Intersects: function ($item, bbox) {
			var aPos = $item.offset();

			var aLeft = aPos.left;
			var aRight = aPos.left + $item.width();
			var aTop = aPos.top;
			var aBottom = aPos.top + $item.height();

			var bLeft = bbox.left;
			var bRight = bbox.left + bbox.width;
			var bTop = bbox.top;
			var bBottom = bbox.top + bbox.height;
			return !(bLeft >= aRight
				|| bRight <= aLeft
				|| bTop >= aBottom
				|| bBottom <= aTop
			);
		},

		checkBboxIntersects: function (bbox1, bbox2) {
			var aLeft = bbox1.left;
			var aRight = bbox1.left + bbox1.width;
			var aTop = bbox1.top;
			var aBottom = bbox1.top + bbox1.height;

			var bLeft = bbox2.left;
			var bRight = bbox2.left + bbox2.width;
			var bTop = bbox2.top;
			var bBottom = bbox2.top + bbox2.height;
			return !(bLeft >= aRight
				|| bRight <= aLeft
				|| bTop >= aBottom
				|| bBottom <= aTop
			);
		},

		/**
		 * update all scrollbars
		 */
		updateAllScrollbars: function () {
			var _this = this;
			this.$root.find('.iz-dash-tile').each(function (iTile, tile) {
				var $tile = $(tile);
				_this.setScrollNew($tile);
			});
		},

		/**
		 * Hide all scrollbars
		 */
		hideAllScrollbars: function () {
			var _this = this;
			this.$root.find('.iz-dash-tile').each(function (iTile, tile) {
				var $tile = $(tile);
				_this.removeScroll($tile);
			});
		},

		/**
		 * Set scrollbar for tile
		 */
		setScrollNew: function ($tile) {
			var _this = this;
			var tile = _this.getTileByTile$($tile);
			// set front scroll
			$tile.find('.frame').css('overflow', 'hidden');
			var $front = $tile.find('.flippy-front .frame');
			if (tile.top != -999) {
				if ($front.hasClass('ps-container')) {
					$front.perfectScrollbar('update');
				} else {
					$front.perfectScrollbar();
				}
			} else {
				if ($front.hasClass('ps-container')) {
					$front.perfectScrollbar('destroy');
				}
			}
			// add back scroll
			var $back = $tile.find('.flippy-back .frame');
			if ($back.hasClass('ps-container')) {
				$back.perfectScrollbar('update');
			} else {
				$back.perfectScrollbar();
			}
		},

		/**
		 * Remove scroll
		 */
		removeScroll: function ($tile) {
			/*setTimeout(function() {
			  $tile.find('.frame').css('overflow', 'hidden');
			  var scrollFront = $tile.find('.flippy-front .frame').getNiceScroll();
			  if (scrollFront != null) {
				scrollFront.remove();
			  }
			  var scrollBack = $tile.find('.flippy-back .frame').getNiceScroll();
			  if (scrollBack != null) {
				scrollBack.remove();
			  }
			}, 2);*/
		},

		/**
		 * Set tile ui content width
		 */
		fitTileUiContent: function ($tile) {
			$tile.find('.frame').width($tile.find('.animate-flip').width() - 10);
			$tile.find('.frame').height($tile.find('.animate-flip').height() - 45);
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
					(tile.y + tile.height <= tile2.y || tile2.y + tile2.height <= tile.y)) {
					} else {
						return false;
					}
				}
			}
			return true;
		},

		/**
		 * Return tile jquery placeholder object.
		 */
		getTile$ById: function (tileId) {
			return this.$root.find('.iz-dash-tile[tileid="' + tileId + '"]');
		},

		/**
		 * Get tile visual position
		 */
		getTileRelativePosition: function (tile) {
			var pos = 0;
			var tiles = this.options.dashboardLayout.tiles;
			$.each(tiles, function (iT, t) {
				if (tile.y > t.y || (t.y == tile.y && tile.x > t.x)) {
					pos++;
				}
			});
			return pos;
		},

		/**
		 * Is dashboard can change
		 */
		isDashboardMobile: function () {
			var windowWidth = $(window).width();
			return windowWidth <= 992;
		},

		/**
		 * Find tile options by given id
		 */
		getTileById: function (tileId) {
			var dashboardLayout = this.options.dashboardLayout;
			var tiles = $.grep(dashboardLayout.tiles, function (tileDescriptor) {
				return tileDescriptor.id == tileId;
			});
			return tiles.length > 0 ? tiles[0] : null;
		},

		/**
		 * Get tile by given ui element
		 */
		getTileByTile$: function (tile$) {
			return this.getTileById(tile$.attr('tileid'));
		},

		/**
		 * Get tile jquery object by inner element
		 */
		getTile$ByInnerEl: function (el) {
			var $el = $(el);
			return $($el.closest('.iz-dash-tile'));
		},

		getGrid: function () {
			if (typeof this.$grid == 'undefined') {
				this.$grid = null;
			}
			return this.$grid;
		},

		/**
		 * Calculate tiles bbox
		 */
		getTilesBBox: function (tiles) {
			var minX = Number.MAX_VALUE,
				minY = Number.MAX_VALUE,
				maxX = 0,
				maxY = 0;
			$.each(tiles, function (iTile, tile) {
				if (tile.x < minX) minX = tile.x;
				if (tile.y < minY) minY = tile.y;
				if (tile.x + tile.width - 1 > maxX) maxX = tile.x + tile.width - 1;
				if (tile.y + tile.height - 1 > maxY) maxY = tile.y + tile.height - 1;
			});
			return {
				minX: minX,
				minY: minY,
				maxX: maxX,
				maxY: maxY
			};
		},

		/**
		 * Calculate items bbox
		 */
		getItems$BBox: function (tiles$) {
			var gridsSize = {
				left: Number.MAX_VALUE,
				top: Number.MAX_VALUE,
				right: 0,
				bottom: 0
			};
			if (tiles$ == null || tiles$.length == 0) {
				gridsSize.left = 0;
				gridsSize.top = 0;
			}
			$.each(tiles$, function (iTile, tile) {
				var tile$ = $(tile);
				var currentOffset = tile$.offset();
				gridsSize.left = currentOffset.left < gridsSize.left
				  ? currentOffset.left
				  : gridsSize.left;
				gridsSize.top = currentOffset.top < gridsSize.top
				  ? currentOffset.top
				  : gridsSize.top;
				gridsSize.right = currentOffset.left + tile$.width() > gridsSize.right
				  ? currentOffset.left + tile$.width()
				  : gridsSize.right;
				gridsSize.bottom = currentOffset.top + tile$.height() > gridsSize.bottom
				  ? currentOffset.top + tile$.height()
				  : gridsSize.bottom;
			});
			return {
				left: gridsSize.left,
				top: gridsSize.top,
				width: gridsSize.right - gridsSize.left,
				height: gridsSize.bottom - gridsSize.top
			};
		},

		/**
		 * getTiles height
		 */
		getTiles$Height: function () {
			var $tiles = this.$root.find('.iz-dash-tile');
			var minTop = Number.MAX_VALUE;
			var maxHeight = 0;
			for (var i = 0; i < $tiles.length; i++) {
				var $tile = $($tiles[i]);
				minTop = Math.min(minTop, $tile.offset().top);
				maxHeight = Math.max(maxHeight, $tile.offset().top + $tile.height());
			}
			return maxHeight - minTop;
		},

		/**
		 * Update tile title
		 */
		updateTileTitle: function ($tile) {
			var tile = this.getTileByTile$($tile);
			var $title = $tile.find('.title-text');
			$title.remove();
			$title = $($("#tileTitleTemplate").render({
				tile: tile,
				dashboardOptions: this.options
			}).trim());
			$tile.find('.title').prepend($title);
		},

		////////////////////////////////////////////////////////////////////////////////////////////////////
		// Modal
		////////////////////////////////////////////////////////////////////////////////////////////////////

		/**
		 * Show splash screen in modal
		 */
		addSplashScreenToModal: function () {
			var urlSettings = this.options.urlSettings;
			var $modal = $('#selectPartModal');
			var $modalBody = $modal.find('.modal-body');
			$modalBody.empty();
			var $splash = $('<div id="selectPartModalSplash"><img src="' + urlSettings.urlRsPage + '?image=loading.gif" alt="" /></div>');
			$modalBody.append($splash);
			$splash.width($modalBody.width());
			$splash.height($modalBody.height());
		},

		/**
		 * Remove splash screen
		 */
		removeSplashScreenFromModal: function () {
			$('#selectPartModalSplash').remove();
		},

		/**
		 * Add categories to modal
		 */
		addCategoriesToModal: function (reportSets) {
			var _this = this;
			// collect categories and create category filter
			var categories = [];
			for (var i = 0; i < reportSets.length; i++) {
				var report = reportSets[i];
				if (report.Dashboard)
					continue;
				var category = report.Category;
				if (category == null || category == '')
					category = 'Uncategorized';
				if (categories.indexOf(category) < 0) {
					categories.push(category);
				}
			}

			var categorySelector$ = $('#reportListCategorySelector');
			categorySelector$.empty();
			$.each(categories, function (idx, item) {
				categorySelector$.append($('<option value="' + item + '">' + item + '</option>'));
			});
			categorySelector$.change(function () {
				var value = $("#reportListCategorySelector option:selected").attr('value');
				_this.query.loadReportSetCategory(_this, value, function (rsets) {
					_this.addReportsToModal(rsets);
				});
			});
		},

		/**
		 * Add report parts to modal
		 */
		addReportPartsToModal: function (reportParts) {
			var _this = this;
			var $modalItemTemplate = $("#modalItemTemplate");
			var $modal = $('#selectPartModal');
			var $modalBody = $modal.find('.modal-body');
			$modalBody.empty();
			var $row = $('<div class="row"></div>');
			$.each(reportParts, function (iReportPart, reportPart) {
				if (iReportPart > 0 && iReportPart % 4 == 0) {
					$modalBody.append($row);
					$row = $('<div class="row"></div>');
				}
				var $reportSet = $($modalItemTemplate.render(reportPart).trim());
				// add handlers
				var $thumb = $reportSet.find('.thumb');
				$thumb.attr('fullName', reportPart.FullName);

				// report part selected:
				$thumb.bind('click', {
					reportPart: reportPart,
					_this: _this
				}, function (e) {
					$('#selectPartModal').modal('hide');
					var part = e.data.reportPart;
					var _t = e.data._this;
					if (_t.temp.$selectReportPartTile != null) {
						_t.removeScroll(_t.temp.$selectReportPartTile);
						_t.flipTileFront(_t.temp.$selectReportPartTile, function (currentTile) {
							currentTile.reportFullName = part.FullName;
							currentTile.reportCategory = part.Category;
							currentTile.reportName = part.Name;
							if (part.Category == null || part.Category == '' || part.Category.toLowerCase() == 'uncategorized')
								currentTile.reportSetName = part.Category + '\\' + part.Name;
							else
								currentTile.reportSetName = part.Name;

							var $t = _this.getTile$ById(currentTile.id);
							// add report part to dashboard
							_this.query.addReportPart({
								_this: _this,
								tile: currentTile,
								$tile: $t
							}, currentTile.reportFullName, currentTile, function (newName) {
								$.extend(this.tile, parseReportName(newName));
								_t.initializeFilters();
								this._this.updateTile(this.tile, true, false, function ($tt) {
									_t.setScrollNew($tt);
								});
								this._this.updateTileTitle(this.$tile);
							});
						});
					}
				});
				$row.append($reportSet);
			});
			$modalBody.append($row);
		},

		/**
		 * Add reports to modal dialog
		 */
		addReportsToModal: function (reportSets) {
			var _this = this;
			var $modalItemTemplate = $("#modalItemTemplate");
			var $modal = $('#selectPartModal');
			var $modalBody = $modal.find('.modal-body');
			$modalBody.empty();
			var reportSetsToShow = $.grep(reportSets, function (reportSet) {
				return !reportSet.Dashboard && reportSet.Name;
			});
			if (reportSetsToShow == null || reportSetsToShow.length == 0) {
				$modalBody.html('No reports found!');
				return;
			}
			// render report sets
			var $row = $('<div class="row"></div>');
			$.each(reportSetsToShow, function (iReportSet, reportSet) {
				// add row if needed
				if (iReportSet > 0 && iReportSet % 4 == 0) {
					$modalBody.append($row);
					$row = $('<div class="row"></div>');
				}
				// add handlers
				var $reportSet = $($modalItemTemplate.render(reportSet).trim());
				$reportSet.find('.thumb').bind('click', {
					reportSet: reportSet
				}, function (e) {
					var rs = e.data.reportSet;
					var reportFullName = rs.Category;
					if (reportFullName == null)
						reportFullName = '';
					reportFullName = reportFullName.trim();
					if (reportFullName != '')
						reportFullName += '\\';
					reportFullName += rs.Name;
					_this.query.loadReportParts(_this, reportFullName, function (rsets) {
						this.addReportPartsToModal(rsets);
					});
				});

				// add report set item to row
				$row.append($reportSet);
			});
			$modalBody.append($row);
		}
	};

	////////////////////////////////////////////////////////////////////////////////////////////////////
	// Plugin functions
	////////////////////////////////////////////////////////////////////////////////////////////////////

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
		reportSetName: null,
		reportSetCategory: null,
		dashboardLayout: null, // dashboard sctructure (rows and cells description)
		urlSettings: null, // url 
		onFlipTileFront: function (dashboard, tile) {
			//if (console && console.log) console.log('>> flip front ');
		},
		onFlipTileBack: function (dashboard, tile) {
			//if (console && console.log) console.log('>> flip back ');
		},
		onChangeTileTop: function (dashboard, tile) {
			//if (console && console.log) console.log('>> change top ');
		},
		onTileLoad: function (dashboard, tile, success) {
			//if (console && console.log) console.log('>> load ');
		},
		onGridInitialized: function (dashboard) {
			//if (console && console.log) console.log('>> grid initialized ');
		},
		onDashboardLayoutLoaded: function (dashboard) {
			//if (console && console.log) console.log('>> layout loaded ');
		}
	};

	/**
	 * Defaults
	 */
	$.fn.izendaDashboard.tileDefaults = {
		$tile: null,
		id: null,
		reportFullName: null,
		reportPartName: null,
		reportSetName: null,
		reportName: null,
		reportCategory: null,
		x: 0,
		y: 0,
		width: 1,
		height: 1,
		top: 100
	};
})(jq);