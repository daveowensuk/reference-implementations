////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Izenda Filters
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
var jq = (typeof jq$ == 'undefined' ? jQuery : jq$);
var IzendaFilters;
(function ($) {
	IzendaFilters = function () { };

	IzendaFilters.prototype = {

		/**
		* Initialize filters
		*/
		initialize: function (options) {
			this.options = $.extend({
				urlSettings: new UrlSettings()
			}, options);
			this.$root = options.$root;
			this.dashboard = options.dashboard;

			// create query
			this.query = new IzendaQuery();
			this.query.initialize({
				trace: true,
				urlSettings: this.options.urlSettings
			});
			// initialize filters ui
			this.initializeFiltersUi();
		},

		/**
		* initialize filters ui
		*/
		initializeFiltersUi: function (handler) {
			//var reportFullName = this.options.urlSettings.reportInfo.fullName;
			this.query.loadFiltersData(this, function (filtersData) {
				this.refresh(filtersData);
				if (typeof handler == 'function') {
					handler.apply(this, []);
				}
			});
		},

		/**
		* Commit filter changes
		*/
		commitFiltersData: function (updateReportSet) {
			var _this = this;
			if (this.filtersData == null || this.filtersData.length <= 0)
				return;
			var dataToCommit = new Array();
			for (var index = 0; index < this.filtersData.length; index++) {
				var filterObj = new Object();
				var fData = this.getFilterValues(index);
				filterObj.Values = fData.slice(1);
				filterObj.Uid = fData[0];
				dataToCommit[index] = filterObj;
			}
			// update report set
			if (updateReportSet) {
				this.query.setFiltersData(this, JSON.stringify(dataToCommit), function (data) {
					this.initializeFiltersUi(function () {
						var tiles = _this.dashboard.options.dashboardLayout.tiles;
						for (var i = 0; i < tiles.length; i++) {
							var tile = tiles[i];
							var $t = _this.dashboard.getTile$ById(tile.id);
							_this.dashboard.removeScroll($t);
							_this.dashboard.updateTile(tile, true, false, function ($tt) {
								_this.dashboard.setScrollNew($tt);
							});
						}
					});
				});
			} else {
				this.query.refreshCascadingFilters(this, JSON.stringify(dataToCommit), function (data) {
					this.refresh(data);
				});
			}
		},

		/**
		* Returns value(s) for filter with number (index)
		*/
		getFilterValues: function (index) {
			var result = new Array();
			result[0] = '';
			switch (this.filtersData[index].ControlType) {
				case 1:
				case 3:
				case 4:
				case 7:
				case 9:
					result[0] = $('#ndbfc' + index).closest('.iz-dash-filter-uid').attr('id');
					result[1] = $('#ndbfc' + index).val();
					break;
				case 2:
					result[0] = $('#ndbfc' + index + '_l').closest('.iz-dash-filter-uid').attr('id');
					result[1] = $('#ndbfc' + index + '_l').val();
					result[2] = $('#ndbfc' + index + '_r').val();
					break;
				case 5:
					result[0] = $('#ndbfc' + index + '_1').closest('.iz-dash-filter-uid').attr('id');
					result[1] = $('#ndbfc' + index + '_1').val();
					result[2] = $('#ndbfc' + index + '_2').val();
					break;
				case 6:
					break;
				case 8:
					result[0] = $('#ndbfc' + index).closest('.iz-dash-filter-uid').attr('id');
					var combinedRes8 = '';
					var cnt8 = 0;
					var cb = document.getElementById('ndbfc' + index + '_cb' + cnt8);
					while (cb != null) {
						if (cb.checked) {
							if (combinedRes8.length > 1)
								combinedRes8 += ',';
							combinedRes8 += cb.value;
						}
						cnt8++;
						cb = document.getElementById('ndbfc' + index + '_cb' + cnt8);
					}
					result[1] = combinedRes8;
					break;
				case 10:
					result[0] = $('#ndbfc' + index).closest('.iz-dash-filter-uid').attr('id');
					var combinedRes = '';
					var selCtl = document.getElementById('ndbfc' + index);
					for (var cnt10 = 0; cnt10 < selCtl.options.length; cnt10++) {
						if (selCtl.options[cnt10].selected) {
							if (combinedRes.length > 1)
								combinedRes += ',';
							combinedRes += selCtl.options[cnt10].value;
						}
					}
					result[1] = combinedRes;
					break;
				default:
			}
			return result;
		},

		/**
		* Reload and render filters
		*/
		refresh: function (returnObj) {
			var _this = this;
			if (returnObj.Filters == null || returnObj.Filters.length <= 0) {
				this.$root.fadeOut('normal', function () {
					_this.$root.empty();
				});
				return;
			}

			this.filtersData = returnObj.Filters;
			var calendars = this.calendars = new Array();

			// render filter control
			var fHtml = $('#filterTemplate').render().trim();
			this.$root.html(fHtml);

			var $filterItemsContainer = this.$root.find('.iz-dash-filter-items');
			var $row = $filterItemsContainer.children('div.row:last');
			var index = 0;
			while (index < returnObj.Filters.length) {
				var filter = returnObj.Filters[index];
				var filterOuterHtml = '<div id="' + filter.Uid + '" class="col-lg-3 col-md-4 col-sm-6 col-xs-12 iz-dash-filter-uid">';
				filterOuterHtml += '<div style="background-color:#CCEEFF;padding:2px;margin-bottom:2px;margin-right:15px;">' + filter.Description + '</div>';
				filterOuterHtml += this.generateFilterControl(index, filter.ControlType, filter.Value, filter.Values, filter.ExistingLabels, filter.ExistingValues);
				filterOuterHtml += '</div></div>';
				$row.append($(filterOuterHtml));
				index++;
			}

			// create filters
			var $filterItems = this.$root.find('#iz-dash-filter-items');
			$filterItems.empty();

			// show/hide filters
			this.$root.find('.iz-dash-toggle-filter').click(function (e) {
				e.preventDefault();
				_this.dashboard.hideAllScrollbars();
				var $this = $(this);
				var $glyphicon = $this.children('span.glyphicon');
				$row = $filterItemsContainer;
				if ($glyphicon.hasClass('glyphicon-chevron-up')) {
					$glyphicon.removeClass('glyphicon-chevron-up');
					$glyphicon.addClass('glyphicon-chevron-down');
					$row.slideUp(400, function () {
						_this.dashboard.updateAllScrollbars();
					});
				} else {
					$glyphicon.removeClass('glyphicon-chevron-down');
					$glyphicon.addClass('glyphicon-chevron-up');
					$row.slideDown(400, function () {
						_this.dashboard.updateAllScrollbars();
					});
				}
			});

			// update filters btn
			this.$root.find('#dashboardUpdateFiltersBtn').click(function (e) {
				e.preventDefault();
				_this.commitFiltersData(true);
			});

			_this.dashboard.updateAllScrollbars();

			setTimeout(function () {
				for (var cnt = 0; cnt < calendars.length; cnt++) {
					var eqInput = document.getElementById(calendars[cnt]);
					jq$(eqInput).datepicker();
				}
			}, 100);
		},

		/**
		* Generate filter inner elements
		*/
		generateFilterControl: function (index, cType, value, values, existingLabels, existingValues) {
			var onChangeCmd = 'onchange="dashboard.filters.commitFiltersData(false);"';
			//var onKeyUpCmd = 'onkeyup="CommitFiltersData(false);"';
			var result = '';
			var onKeyUpCmd = '';
			result += '<form role="form">';
			result += '<div class="form-group">';
			switch (cType) {
				case 1:
					result += '<input style="width:99%;" type="text" id="ndbfc' + index + '" value="' + value + '" ' + onKeyUpCmd + ' />';
					break;
				case 2:
					result += '<input style="width:99%;" type="text" id="ndbfc' + index + '_l" value="' + values[0] + '" ' + onKeyUpCmd + ' />';
					result += '<input style="width:99%;" type="text" id="ndbfc' + index + '_r" value="' + values[1] + '" ' + onKeyUpCmd + ' />';
					break;
				case 3:
					result += '<select style="width:100%;" id="ndbfc' + index + '" ' + onChangeCmd + '>';
					for (var cnt3 = 0; cnt3 < existingValues.length; cnt3++) {
						var selected3 = '';
						if (existingValues[cnt3] == value)
							selected3 = 'selected="selected"';
						result += '<option value="' + existingValues[cnt3] + '" ' + selected3 + '>' + existingLabels[cnt3] + '</option>';
					}
					result += '</select>';
					break;
				case 4:
					result += '<select style="width:100%" id="ndbfc' + index + '" ' + onChangeCmd + '>';
					for (var cnt4 = 0; cnt4 < existingValues.length; cnt4++) {
						var selected4 = '';
						if (existingValues[cnt4] == value)
							selected4 = 'selected="selected"';
						result += '<option value="' + existingValues[cnt4] + '" ' + selected4 + '>' + existingLabels[cnt4] + '</option>';
					}
					result += '</select>';
					break;
				case 5:
					result += '<input type="text" ' + onChangeCmd + ' value="' + values[0] + '" style="width:248px" id="ndbfc' + index + '_1" />';
					this.calendars[this.calendars.length] = 'ndbfc' + index + '_1';
					result += '<img onclick="javascript: if (showingC) {return;} showingC = true; setTimeout(function() {document.getElementById(\'ndbfc' + index + '_1\').focus(); setTimeout(function(){showingC = false;}, 500);}, 500); " style="cursor:pointer;position:relative;top:0px;margin-left: 5px;" src="' + urlSettings.urlRsPage + '?image=calendar_icon.png">';
					result += '<input type="text" ' + onChangeCmd + ' value="' + values[1] + '" style="width:248px" id="ndbfc' + index + '_2" />';
					this.calendars[this.calendars.length] = 'ndbfc' + index + '_2';
					result += '<img onclick="javascript: if (showingC) {return;} showingC = true; setTimeout(function() {document.getElementById(\'ndbfc' + index + '_2\').focus(); setTimeout(function(){showingC = false;}, 500);}, 500); " style="cursor:pointer;position:relative;top:0px;margin-left: 5px;" src="' + urlSettings.urlRsPage + '?image=calendar_icon.png">';
					break;
				case 6:
					result += 'equals popup - not implemented yet';
					break;
				case 7:
					result += '<textarea style="width:99%;" rows="2" id="ndbfc' + index + '" ' + onKeyUpCmd + '>' + value + '</textarea>';
					break;
				case 8:
					result += '<div id="ndbfc' + index + '" style="padding-left:8px; width:96%; overflow: auto; max-height: 100px;background-color: white;border: 1px solid #A5A5A5;">';
					var valuesSet8 = value.split(',');
					for (var cnt8 = 0; cnt8 < existingValues.length; cnt8++) {
						var checked8 = '';
						for (var subCnt8 = 0; subCnt8 < valuesSet8.length; subCnt8++) {
							if (existingValues[cnt8] == valuesSet8[subCnt8]) {
								checked8 = 'checked="checked"';
								break;
							}
						}
						result += '<nobr><input type="checkbox" id="ndbfc' + index + '_cb' + cnt8 + '" ' + onChangeCmd + ' value="' + existingValues[cnt8] + '" ' + checked8 + '" />' + existingLabels[cnt8] + '</nobr><br>';
					}
					result += '</div>';
					break;
				case 9:
					result += '<input type="text" ' + onChangeCmd + ' value="' + value + '" style="width:248px" id="ndbfc' + index + '" />';
					this.calendars[this.calendars.length] = 'ndbfc' + index;
					result += '<img onclick="javascript: if (showingC) {return;} showingC = true; setTimeout(function() {document.getElementById(\'ndbfc' + index + '\').focus(); setTimeout(function(){showingC = false;}, 500);}, 500); " style="cursor:pointer;position:relative;top:4px;" src="rs.aspx?image=calendar_icon.png">';
					break;
				case 10:
					result += '<select style="width:100%" size="5" multiple="" id="ndbfc' + index + '" ' + onChangeCmd + '>';
					var valuesSet10 = value.split(',');
					for (var cnt10 = 0; cnt10 < existingValues.length; cnt10++) {
						var selected10 = '';
						for (var subCnt = 0; subCnt < valuesSet10.length; subCnt++) {
							if (existingValues[cnt10] == valuesSet10[subCnt]) {
								selected10 = 'selected="selected"';
								break;
							}
						}
						result += '<option value="' + existingValues[cnt10] + '" ' + selected10 + '>' + existingLabels[cnt10] + '</option>';
					}
					result += '</select>';
					break;
				default:
					result = '';
			}
			result += '</div>';
			result += '</form>';
			return result;
		}
	};
})(jq);