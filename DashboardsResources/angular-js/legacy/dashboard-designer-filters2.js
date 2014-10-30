var filtersData;
var showingC = false;
var calendars;

function ToggleFilters() {
	var filtersBodyDiv$ = $('#filtersBodyDiv');
	if (filtersBodyDiv$.css('visibility') == 'hidden') {
		filtersBodyDiv$.css('visibility', 'visible');
		filtersBodyDiv$.css('height', 'auto');
	}
	else {
		filtersBodyDiv$.css('visibility', 'hidden');
		filtersBodyDiv$.css('height', '0px');
	}
}

/**
 * Returns value(s) for filter with number (index)
 */
function GetFilterValues(index) {
	var result = new Array();
	result[0] = '';
	switch (filtersData[index].ControlType) {
		case 1:
			result[0] = document.getElementById('ndbfc' + index).parentElement.id;
			result[1] = document.getElementById('ndbfc' + index).value;
			break;
		case 2:
			result[0] = document.getElementById('ndbfc' + index + '_l').parentElement.id;
			result[1] = document.getElementById('ndbfc' + index + '_l').value;
			result[2] = document.getElementById('ndbfc' + index + '_r').value;
			break;
		case 3:
			result[0] = document.getElementById('ndbfc' + index).parentElement.id;
			result[1] = document.getElementById('ndbfc' + index).value;
			break;
		case 4:
			result[0] = document.getElementById('ndbfc' + index).parentElement.id;
			result[1] = document.getElementById('ndbfc' + index).value;
			break;
		case 5:
			result[0] = document.getElementById('ndbfc' + index + '_1').parentElement.id;
			result[1] = document.getElementById('ndbfc' + index + '_1').value;
			result[2] = document.getElementById('ndbfc' + index + '_2').value;
			break;
		case 6:
			break;
		case 7:
			result[0] = document.getElementById('ndbfc' + index).parentElement.id;
			result[1] = document.getElementById('ndbfc' + index).value;
			break;
		case 8:
			result[0] = document.getElementById('ndbfc' + index).parentElement.id;
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
		case 9:
			result[0] = document.getElementById('ndbfc' + index).parentElement.id;
			result[1] = document.getElementById('ndbfc' + index).value;
			break;
		case 10:
			result[0] = document.getElementById('ndbfc' + index).parentElement.id;
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
}

/**
 * Save filters stored in 'filtersData' variable.
 */
function CommitFiltersData(updateReportSet) {
	if (filtersData == null || filtersData.length <= 0)
		return;
	var dataToCommit = new Array();
	for (var index = 0; index < filtersData.length; index++) {
		var filterObj = new Object();
		var fData = GetFilterValues(index);
		filterObj.Values = fData.slice(1);
		filterObj.Uid = fData[0];
		dataToCommit[index] = filterObj;
	}
	var cmd = 'setfiltersdata';
	if (!updateReportSet)
		cmd = 'refreshcascadingfilters';
	var requestString = 'wscmd=' + cmd + '&wsarg0=' + encodeURIComponent(JSON.stringify(dataToCommit));
	if (updateReportSet)
		AjaxRequest(urlSettings.urlRsPage, requestString, FiltersDataSet, null, 'setfiltersdata');
	else
		AjaxRequest(urlSettings.urlRsPage, requestString, CascadingFiltersChanged, null, 'refreshcascadingfilters');
}

/**
 * Reload and render filters
 */
function RefreshFilters(returnObj) {
	var htmlFiltersTd$ = $(document.getElementById('htmlFiltersTd'));
	var htmlFilters = document.getElementById('htmlFilters');
	if (returnObj.Filters == null || returnObj.Filters.length <= 0) {
		htmlFiltersTd$.fadeOut('normal', function () { htmlFilters.innerHTML = ''; });
		return;
	}
	filtersData = returnObj.Filters;
	calendars = new Array();
	var fHtml = '<table style="background-color:#FFFFFF;"><tr>';
	fHtml += '<td style="padding-top:8px; vertical-align:top;">';
	var index = 0;
	while (index < returnObj.Filters.length) {
		var filter = returnObj.Filters[index];
		fHtml += '<div id="' + filter.Uid + '" style="float:left;margin-left:8px;margin-right:8px;width:300px;">';
		fHtml += '<div style="background-color:#CCEEFF;padding:2px;padding-left:4px;margin-bottom:2px;">' + filter.Description + '</div>';
		fHtml += GenerateFilterControl(index, filter.ControlType, filter.Value, filter.Values, filter.ExistingLabels, filter.ExistingValues);
		fHtml += '</div></td>';
		index++;
		if (index > 0 && index % 3 == 0 && index < returnObj.Filters.length)
			fHtml += '</tr><tr>';
		if (index < returnObj.Filters.length)
			fHtml += '<td style="padding-top:8px; vertical-align:top;">';
	}
	fHtml += '</tr><tr><td><div id="updateBtnP" class="f-button" style="margin: 10px; margin-left:8px;">';
	fHtml += '<a class="blue" onclick="javascript:CommitFiltersData(true);" href="javascript:void(0);"><img src="Resources/images/refresh-white.png" alt="Refresh" /><span class="text">Update results</span></a>';
	fHtml += '</div></td></tr></table>';
	htmlFilters.innerHTML = fHtml;
	setTimeout(function () {
		for (var cnt = 0; cnt < calendars.length; cnt++) {
			var eqInput = document.getElementById(calendars[cnt]);
			jq$(eqInput).datepicker();
		}
		htmlFiltersTd$.fadeIn('normal', null);
	}, 100);
}

function CascadingFiltersChanged(returnObj, id) {
	if (id != 'refreshcascadingfilters' || returnObj == null)
		return;
	RefreshFilters(returnObj);
}

function FiltersDataSet(returnObj, id) {
	if (id != 'setfiltersdata' || returnObj == null)
		return;
	if (returnObj.Value != 'OK')
		return;
	GetFiltersData();
	RefreshParts();
}

function GenerateFilterControl(index, cType, value, values, existingLabels, existingValues) {
	var onChangeCmd = 'onchange="CommitFiltersData(false);"';
	//var onKeyUpCmd = 'onkeyup="CommitFiltersData(false);"';
	var onKeyUpCmd = '';
	var result = '';
	switch (cType) {
		case 1:
			result = '<input style="width:99%;" type="text" id="ndbfc' + index + '" value="' + value + '" ' + onKeyUpCmd + ' />';
			break;
		case 2:
			result = '<input style="width:99%;" type="text" id="ndbfc' + index + '_l" value="' + values[0] + '" ' + onKeyUpCmd + ' />';
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
			calendars[calendars.length] = 'ndbfc' + index + '_1';
			result += '<img onclick="javascript: if (showingC) {return;} showingC = true; setTimeout(function() {document.getElementById(\'ndbfc' + index + '_1\').focus(); setTimeout(function(){showingC = false;}, 500);}, 500); " style="cursor:pointer;position:relative;top:4px;" src="' + urlSettings.urlRsPage + '?image=calendar_icon.png">';
			result += '<input type="text" ' + onChangeCmd + ' value="' + values[1] + '" style="width:248px" id="ndbfc' + index + '_2" />';
			calendars[calendars.length] = 'ndbfc' + index + '_2';
			result += '<img onclick="javascript: if (showingC) {return;} showingC = true; setTimeout(function() {document.getElementById(\'ndbfc' + index + '_2\').focus(); setTimeout(function(){showingC = false;}, 500);}, 500); " style="cursor:pointer;position:relative;top:4px;" src="' + urlSettings.urlRsPage + '?image=calendar_icon.png">';
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
			calendars[calendars.length] = 'ndbfc' + index;
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
	return result;
}

/**
 * Load filters
 */
//function GetFiltersData() {
//	var requestString = 'wscmd=getfiltersdata';
//	AjaxRequest(urlSettings.urlRsPage, requestString, GotFiltersData, null, 'getfiltersdata');
//}
//
//function GotFiltersData(returnObj, id) {
//	if (id != 'getfiltersdata' || returnObj == null)
//		return;
//	RefreshFilters(returnObj);
//}

/**
 * Reload tiles content
 */
function RefreshParts() {
	var allTiles$ = $('.dd-tile-td');
	// calculate tile count in row
	var tc;
	var rows = [];
	for (tc = 0; tc < allTiles$.length; tc++) {
		rows[$(allTiles$[tc]).attr('y')] = 0;
	}
	for (tc = 0; tc < allTiles$.length; tc++) {
		rows[$(allTiles$[tc]).attr('y')] += 1;
	}

	// refresh all tiles content
	$.each(allTiles$, function (idx, tile) {
		var tile$ = $(tile);
		var reportFullName = tile$.attr('ReportFullName');
		if (reportFullName != null && reportFullName != '') {
			var body$ = tile$.find('.report');
			body$.html($('<img src="' + urlSettings.urlRsPage + '?image=loading.gif" alt="" />'));
			var previewReportUrl = 'wscmd=getcrsreportpartpreview&wsarg0=' + encodeURIComponent(reportFullName) + '&wsarg1=' + rows[tile$.attr('y')];
			AjaxRequest(urlSettings.urlRsPage, previewReportUrl, function(returnObj, id) {
			  if (id != 'getcrsreportpartpreview' || returnObj == undefined || returnObj == null || returnObj == null || returnObj.length == 0)
			    return;
			  designer.appendRealReport(tile$, returnObj, tile$.attr('ReportSetName'));
			}
	      , null, 'getcrsreportpartpreview');
		}
	});
}