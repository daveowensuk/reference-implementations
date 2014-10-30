////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Izenda Dashboard Data Loader
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
var jq = (typeof jq$ == 'undefined' ? jQuery : jq$);
var IzendaQuery;
(function ($) {

	/**
	* Izenda dashboard data loader
	*/
	IzendaQuery = function () {
	};

	IzendaQuery.prototype = {

		/**
		* Initialize query
		*/
		initialize: function (options) {
			this.options = $.extend({
				trace: false,
				urlSettings: new UrlSettings()
			}, options);
			this.tracer = new IzendaTrace();
		},

		/**
		* Make query: rs.aspx?wscmd=<command>&wsarg0=<arg1>&...
		*/
		rsQuery: function (options) {
			// extend default options:
			var queryOptions = $.extend({
				dataType: 'text',
				contextObject: this,
				wsCmd: null,
				wsArgs: [],
				done: function () { },
				fail: function (jqXhr, textStatus, errorThrown) {
					if (console && console.log)
						console.log(jqXhr, textStatus, errorThrown);
				}
			}, options);

			// create url:
			var urlRsPage = this.options.urlSettings.urlRsPage;
			var url = urlRsPage + '?wscmd=' + encodeURIComponent(queryOptions.wsCmd);
			if ($.isArray(queryOptions.wsArgs)) {
				$.each(queryOptions.wsArgs, function (iArg, arg) {
					url += '&wsarg' + iArg + '=' + encodeURIComponent(arg);
				});
			}

			// perform request:
			if (this.options.trace) {
				this.tracer.startTrace(url);
			}
			$.ajax({
				url: url,
				context: {
					queryObject: this,
					queryOptions: queryOptions
				},
				dataType: queryOptions.dataType
			}).done(function (data) {
				if (this.queryObject.options.trace) {
					if (['text', 'html'].indexOf(this.queryOptions.dataType) >= 0)
						this.queryObject.tracer.showTrace(url, ' Loaded ' + (data != null ? data.length : 0) + ' bytes.');
					else
						this.queryObject.tracer.showTrace(url);
				}
				if (typeof this.queryOptions.done == 'function')
					this.queryOptions.done.apply(this.queryOptions.contextObject, [data]);
				else
					throw 'Done handler should be function';
			}).fail(function (jqXhr, textStatus, errorThrown) {
				if (this.queryObject.options.trace) {
					this.queryObject.tracer.showTrace(url, ' Failed: ' + textStatus + ', ' + errorThrown);
				}
				if (typeof this.queryOptions.fail == 'function')
					this.queryOptions.fail.apply(this.queryOptions.contextObject, [jqXhr, textStatus, errorThrown]);
				else
					throw 'Fail handler should be function';
			});
		},

		/**
		* Load dashboard navigation:
		*/
		loadDashboardNavigation: function (context, doneHandler) {
			this.rsQuery({
				dataType: 'json',
				contextObject: context,
				wsCmd: 'getdashboardcategories',
				wsArgs: [],
				done: function (data) {
					var categories = [];
					if (data != null) {
						for (var category in data) {
							var categoryInfo = {
								name: category
							};
							categoryInfo.dashboards = data[category];
							if (categoryInfo.dashboards.length > 0)
								categories.push(categoryInfo);
						}
						if (typeof doneHandler == 'function') {
							doneHandler.apply(this, [categories]);
						}
					}
				}
			});
		},

		/**
		* Check report set already exists
		*/
		checkReportSetExists: function (context, reportFullName, doneHandler) {
			this.rsQuery({
				dataType: 'text',
				contextObject: context,
				wsCmd: 'checkreportsetexists',
				wsArgs: [reportFullName],
				done: function (data) {
					if (typeof doneHandler == 'function') {
						var reportExists = data != null && data.toLowerCase() == 'true';
						doneHandler.apply(this, [reportExists]);
					}
				}
			});
		},

		/**
		* Load dashboard layout
		*/
		loadDashboardLayout: function (context, dashboardFullName, doneHandler) {
			var _this = this;
			this.rsQuery({
				dataType: 'json',
				contextObject: context,
				wsCmd: 'getReportDashboardConfig',
				wsArgs: [dashboardFullName],
				done: function (data) {
					var dashboardLayout;
					if (data == null || data.Rows == null || data.Rows.length == 0) {
						dashboardLayout = {
							tiles: [
                $.extend({}, $.fn.izendaDashboard.tileDefaults, {
                	isNew: true,
                	width: 12,
                	height: 4
                })
							]
						};
					} else {
						var cells = data.Rows[0].Cells;
						dashboardLayout = {
							tiles: []
						};
						for (var i = 0; i < cells.length; i++) {
							var cell = cells[i];
							var obj = $.extend({}, $.fn.izendaDashboard.tileDefaults, _this.parseReportName(cell.ReportFullName), {
								x: cell.X,
								y: cell.Y,
								width: cell.Width,
								height: cell.Height,
								top: cell.RecordsCount
							});
							dashboardLayout.tiles.push(obj);
						}
					}
					if (typeof doneHandler == 'function') {
						doneHandler.apply(this, [dashboardLayout]);
					}
				}
			});
		},

		/**
		* Load report
		*/
		loadReport: function (context, tile, width, height, doneHandler) {
			this.rsQuery({
				dataType: 'html',
				contextObject: context,
				wsCmd: 'getcrsreportpartpreview',
				wsArgs: [tile.reportFullName, 1, tile.top, width, height - 35, 'forceSize'],
				done: function (htmlData) {
					if (typeof doneHandler == 'function') {
						doneHandler.apply(this, [htmlData]);
					}
				}
			});
		},

		/**
		* Update report from source and load
		*/
		loadReportAndUpdate: function (context, tile, width, height, doneHandler) {
			this.rsQuery({
				dataType: 'html',
				contextObject: context,
				wsCmd: 'updateandgetcrsreportpartpreview',
				wsArgs: [tile.reportFullName, 1, tile.top, width, height - 35, 'forceSize'],
				done: function (htmlData) {
					if (typeof doneHandler == 'function') {
						doneHandler.apply(this, [htmlData]);
					}
				}
			});
		},

		/**
		* Update report top
		*/
		updateReportTop: function (context, reportFullName, topValue, doneHandler) {
			this.rsQuery({
				contextObject: context,
				wsCmd: 'settoptoreportpartincrs',
				wsArgs: [reportFullName, topValue],
				done: function (data) {
					if (typeof doneHandler == 'function') {
						doneHandler.apply(this, [data]);
					}
				}
			});
		},

		/**
		* Save dashboard
		*/
		saveDashboard: function (context, dashboardFullName, tiles, doneHandler) {
			var config = {
				Rows: [{
					Cells: [],
					ColumnsCount: tiles.length
				}],
				RowsCount: 1
			};
			for (var i = 0; i < tiles.length; i++) {
				var tile = tiles[i];
				config.Rows[0].Cells[i] = {
					ReportFullName: tile.reportFullName,
					ReportPartName: tile.reportPartName,
					ReportSetName: tile.reportSetName,
					RecordsCount: tile.top,
					X: tile.x,
					Y: tile.y,
					Width: tile.width,
					Height: tile.height
				};
			}
			this.rsQuery({
				contextObject: context,
				wsCmd: 'savecrsdashboard',
				wsArgs: [JSON.stringify(config), dashboardFullName],
				done: function (data) {
					if (typeof doneHandler == 'function') {
						doneHandler.apply(this, [data]);
					}
				}
			});
		},

		/**
		* Load categories
		*/
		loadReportSetCategory: function (context, category, doneHandler) {
			this.rsQuery({
				dataType: 'json',
				contextObject: context,
				wsCmd: 'reportlistdatalite',
				wsArgs: [category.toLowerCase() == 'uncategorized' ? '' : category],
				done: function (data) {
					if (typeof doneHandler == 'function') {
						doneHandler.apply(this, [data.ReportSets]);
					}
				}
			});
		},

		/**
		* Load report parts
		*/
		loadReportParts: function (context, reportFullName, doneHandler) {
			this.rsQuery({
				dataType: 'json',
				contextObject: context,
				wsCmd: 'reportdata',
				wsArgs: [reportFullName],
				done: function (data) {
					if (typeof doneHandler == 'function') {
						doneHandler.apply(this, [data.Reports]);
					}
				}
			});
		},

		/**
		* Add report part to CRS
		*/
		addReportPart: function (context, reportFullName, tile, doneHandler) {
			this.rsQuery({
				dataType: 'json',
				contextObject: context,
				wsCmd: 'addreportparttocrs',
				wsArgs: [reportFullName, tile.x, tile.y],
				done: function (data) {
					if (typeof doneHandler == 'function') {
						doneHandler.apply(this, [data]);
					}
				}
			});
		},

		/**
		* remove tile from CRS
		*/
		removeReportPart: function (context, reportFullName, doneHandler) {
			this.rsQuery({
				contextObject: context,
				wsCmd: 'removereportpartfromcrs',
				wsArgs: [reportFullName],
				done: function (data) {
					if (typeof doneHandler == 'function') {
						doneHandler.apply(this, [data]);
					}
				}
			});
		},

		/**
		* Load filters for dashboard
		*/
		loadFiltersData: function (context, doneHandler) {
			this.rsQuery({
				dataType: 'json',
				contextObject: context,
				wsCmd: 'getdashboardfiltersdata',
				done: function (data) {
					if (typeof doneHandler == 'function') {
						doneHandler.apply(this, [data]);
					}
				}
			});
		},

		/**
		* Update filters for dashboard
		*/
		setFiltersData: function (context, filtersData, doneHandler) {
			this.rsQuery({
				dataType: 'json',
				contextObject: context,
				wsCmd: 'setfiltersdata',
				wsArgs: [filtersData],
				done: function (data) {
					if (typeof doneHandler == 'function') {
						doneHandler.apply(this, [data]);
					}
				}
			});
		},

		/**
		* Update filters for dashboard (used to cascade update filters after for example checking one item in list)
		*/
		refreshCascadingFilters: function (context, filtersData, doneHandler) {
			this.rsQuery({
				dataType: 'json',
				contextObject: context,
				wsCmd: 'refreshcascadingfilters',
				wsArgs: [filtersData],
				done: function (data) {
					if (typeof doneHandler == 'function') {
						doneHandler.apply(this, [data]);
					}
				}
			});
		},

		/**
		* Parse report name into 3 parts:
		* result.reportSetName, result.reportName, result.reportCategory
		*/
		parseReportName: function (reportFullName) {
			if (reportFullName == null)
				throw 'full name is null';
			var parseReportSetName = function (rsName) {
				if (rsName.indexOf('\\') > 0) {
					var p = rsName.split('\\');
					return {
						reportCategory: p[0],
						reportName: p[1]
					};
				}
				return {
					reportCategory: null,
					reportName: rsName
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
	};
})(jq);