// Copyright (c) 2005-2013 Izenda, L.L.C. - ALL RIGHTS RESERVED    
//Ajax request for JSON methods-----------------------------------------------------------
var JSON = JSON || {};
JSON.stringify = JSON.stringify || function (obj) {
	var t = typeof (obj);
	if (t != 'object' || obj === null) {
		if (t == 'string') obj = '"' + obj + '"';
		return String(obj);
	}
	else {
		var n, v, json = [], arr = (obj && obj.constructor == Array);
		for (n in obj) {
			v = obj[n]; t = typeof (v);
			if (t == 'string')
				v = '"' + v + '"';
			else if (t == 'object' && v !== null)
				v = JSON.stringify(v);
			json.push((arr ? '' : '"' + n + '":') + String(v));
		}
		return (arr ? '[' : '{') + String(json) + (arr ? ']' : '}');
	}
};

function AjaxRequest(url, parameters, callbackSuccess, callbackError, id, dataToKeep) {
	var thisRequestObject;
	if (window.XMLHttpRequest)
		thisRequestObject = new XMLHttpRequest();
	else if (window.ActiveXObject)
		thisRequestObject = new ActiveXObject('Microsoft.XMLHTTP');
	thisRequestObject.requestId = id;
	thisRequestObject.dtk = dataToKeep;
	thisRequestObject.onreadystatechange = ProcessRequest;

	/*thisRequestObject.open('GET', url + '?' + parameters, true);
	thisRequestObject.send();*/
	thisRequestObject.open('POST', url, true);
	thisRequestObject.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
	thisRequestObject.send(parameters);

	function DeserializeJson() {
		var responseText = thisRequestObject.responseText;
		while (responseText.indexOf('"\\/Date(') >= 0) {
			responseText = responseText.replace('"\\/Date(', 'eval(new Date(');
			responseText = responseText.replace(')\\/"', '))');
		}
		if (responseText.charAt(0) != '[' && responseText.charAt(0) != '{')
			responseText = '{' + responseText + '}';
		var isArray = true;
		if (responseText.charAt(0) != '[') {
			responseText = '[' + responseText + ']';
			isArray = false;
		}
		var retObj = eval(responseText);
		if (!isArray)
			return retObj[0];
		return retObj;
	}

	function ProcessRequest() {
		if (thisRequestObject.readyState == 4) {
		  if (thisRequestObject.status == 200 && callbackSuccess) {
		    var toRet;
		    if (thisRequestObject.requestId != 'getrenderedreportset' && thisRequestObject.requestId != 'getcrsreportpartpreview')
		      toRet = DeserializeJson();
		    else
		      toRet = thisRequestObject.responseText;			
				callbackSuccess(toRet, thisRequestObject.requestId, thisRequestObject.dtk);
			}
			else if (callbackError) {
				callbackError(thisRequestObject);
			}
		}
	}
}

jQuery.extend({
	getValues: function (url) {
		var result = null;
		$.ajax({
			url: url,
			type: 'get',
			dataType: 'json',
			async: false,
			success: function (data) {
				result = data;
			},
			error: function (data) {
				alert("Can't load (" + url + "). ERROR: " + JSON.stringify(data));
			}
		});
		return result;
	}
});

var tInd = 0;
var fieldsIndex;
var csOrder = 0;
var dsSelections = new Array();
var fieldsOpts = new Array();
var curPropField = '';
var curFieldIndexes = '';
var previewFieldTimeout;
var previewReportTimeout;
var dsState = new Array();
var databaseSchema;
var origRsData = '';
var nirConfig;
var oDatatable;

function IsIE() {
	if (navigator.appName == 'Microsoft Internet Explorer')
		return true;
	return false;
}

function CurrentRn() {
  var queryParameters = {}, queryString = location.search.substring(1),
            re = /([^&=]+)=([^&]*)/g, m;
  while (m = re.exec(queryString)) {
    queryParameters[decodeURIComponent(m[1]).toLowerCase()] = decodeURIComponent(m[2]);
  }
  reportName = '';
  if (queryParameters['rn'] != null && queryParameters['rn'].length > 0) {
    reportName = queryParameters['rn'];
  }
  return reportName;
}

function ExistingReportSetInit() {
  var crn = CurrentRn();
  if (crn == '')
    return;
	var requestString = 'wscmd=reversereportset';
	requestString += '&wsarg0=' + encodeURIComponent(crn);
	AjaxRequest('./rs.aspx', requestString, ReversedReportSet, null, 'reversereportset');
}

function ReversedReportSet(returnObj, id) {
	if (id != 'reversereportset' || returnObj == undefined || returnObj == null)
		return;
	if (returnObj.Value == "OK") {
	  alert('ok');
	}
	else
		alert("Error: " + returnObj.Value);
}

function initDataSources(url) {
	databaseSchema = $.getValues(url);
	if (databaseSchema != null) {
		var datasourcesSearch = new IzendaDatasourcesSearch(databaseSchema);
		$(".database").remove();
		tInd = 0;
		var html = "";
		for (key in databaseSchema)
			html += renderDatabase(databaseSchema[key], key);
	  $(html).prependTo("#databases");
		NDS_Init();
//		ExistingReportSetInit();
		/*var databases = $(".database");
		if (databases && databases.length == 1)
			$(databases[0]).addClass("opened");
		initDraggable();
		var datasourcesSearch = new IzendaDatasourcesSearch(databaseSchema);*/
	};
}

function renderDatabase(database, key) {
	return " \
	<div class='database' id='rdbh" + key + "'> \
		<div class='database-header'> \
			<a href='#" + database.DataSourceCategory + "'> \
				<span class='plus-minus'></span> \
				<span class='database-name'>" + database.DataSourceCategory + "</span> \
			</a> \
		</div> \
 \
		<div class='database-tables' id='rdb" + key + "'>Loading...</div> \
	</div> \ ";
}

function renderTables(tables, dbKey) {
	var html = "";
	for (key in tables) {
	  html += renderTable(dbKey, tables[key], key, tables[key].sysname, tInd);
		tInd++;
	}
	return html;
}

function renderTable(dbKey, table, key, tableId, ind) {
	var element = " \
			<div class='table'> \
				<div class='table-header'> \
					<a href='#" + key + "' tableInd='" + ind + "' id='rdbh" + dbKey + "_" + key + "'> \
						<span class='checkbox-container' locked='false' sorder='-1' id='tcb" + ind + "' tableid='" + tableId + "' onclick='DsClicked(" + ind + ")'><span class='checkbox'></span></span> \
						<span class='table-name'>" + key + "</span> \
						<div class='clearfix'></div> \
					</a> \
				</div> \
				<div class='table-fields' id='rdb" + dbKey + "_" + key + "'>Loading...</div> \
			</div> \ ";
	return element;
}

function renderSections(tableIndex, fields) {
	var html = "";

	var textSectionContent = renderFields(tableIndex, fields, "text");
	if (textSectionContent.length > 1) {
	  html += " \
					<div class='fields-section'> \
						<div class='fields-section-name'> \
							<span>" + "text" + "</span> \
						</div> \ " + textSectionContent + " \
					</div> \ ";
	}

	var datesSectionContent = renderFields(tableIndex, fields, "dates");
	if (datesSectionContent.length > 1) {
	  html += " \
					<div class='fields-section'> \
						<div class='fields-section-name'> \
							<span>" + "dates" + "</span> \
						</div> \ " + datesSectionContent + " \
					</div> \ ";
	}

	var numbersSectionContent = renderFields(tableIndex, fields, "numeric") + renderFields(tableIndex, fields, "money");
	if (numbersSectionContent.length > 1) {
	  html += " \
					<div class='fields-section'> \
						<div class='fields-section-name'> \
							<span>" + "numbers" + "</span> \
						</div> \ " + numbersSectionContent + " \
					</div> \ ";
	}

	var identifiersSectionContent = renderFields(tableIndex, fields, "identifiers");
	if (identifiersSectionContent.length > 1) {
	  html += " \
					<div class='fields-section'> \
						<div class='fields-section-name'> \
							<span>" + "identifiers" + "</span> \
						</div> \ " + identifiersSectionContent + " \
					</div> \ ";
	}

	return html;
}

function renderFields(tableIndex, fields, sectionName) {
	var html = "";
	for (key in fields) {
	  if (fields[key].type == sectionName) html += renderField(tableIndex, key, fields[key].sysname);
	}
	return html;
}

function renderField(tableIndex, fieldName, fieldId) {
	var html = " \
						<a class='field' href='#" + fieldName + "' sorder='-1' locked='false' id='tcb" + tableIndex + "fcb" + fieldsIndex + "' fieldId='" + fieldId + "' onmouseup='FiClick(" + tableIndex + ", " + fieldsIndex + ", false, false)'> \
							<span class='preview-image'></span> \
							</span> \
							<span class='checkbox' style='margin-top: 3px; margin-right: 6px;'></span> \
							<span class='field-name' style=''>" + fieldName + "</span> \
              <span class='field-popup-trigger' style='float:right; margin-top: 2px; left:2px;' title='Options' fieldId='" + fieldId + "' style='float:right;'></span> \
              <span style='visibility:hidden;  margin-top: 2px; left:2px; width:20px; float:right;height:0px;'>&nbsp;&nbsp;&nbsp;&nbsp;</span> \
							<span class='clearfix'></span> \
							</a> \ ";
	fieldsIndex++;
	return html;
}


function getRandomInt(min, max) {
	return Math.floor(Math.random() * (max - min + 1)) + min;
}

function getStyle(x, styleProp) {
	var y = null;
	if (x.currentStyle)
		y = x.currentStyle[styleProp];
	else if (window.getComputedStyle)
		y = document.defaultView.getComputedStyle(x, null).getPropertyValue(styleProp);
	return y;
}

function CollectReportData() {
	var dsList = new Array();
	var fieldsList = new Array();
	var fieldsOrders = new Array();
	var fieldWidths = new Array();
	var fOptions = new Array();
	var idList = new Array();
	var index = 0;
	var cb;
	var soVal;

	while (true) {
		cb = document.getElementById('tcb' + index);
		if (cb == null)
			break;
		soVal = cb.getAttribute('sorder');
		if (soVal == '-1') {
			index++;
			continue;
		}
		dsList[dsList.length] = cb.getAttribute('tableid');
		var fIndex = 0;
		while (true) {
			cb = document.getElementById('tcb' + index + 'fcb' + fIndex);
			if (cb == null)
				break;
			soVal = cb.getAttribute('sorder');
			if (soVal == '-1') {
				fIndex++;
				continue;
			}
			var widthVal = cb.getAttribute('itemwidth');
			if (!widthVal) widthVal = 0;
			idList[fieldsList.length] = 'tcb' + index + 'fcb' + fIndex;
			fieldsList[fieldsList.length] = cb.getAttribute('fieldid');
			fieldsOrders[fieldsOrders.length] = soVal;
			fieldWidths[fieldWidths.length] = widthVal;
			fOptions[fOptions.length] = fieldsOpts[cb.getAttribute('fieldid')];
			fIndex++;
		}
		index++;
	}
	var repObj = new Object();
	repObj.DsList = dsList;
	repObj.FldList = fieldsList;
	repObj.OrdList = fieldsOrders;
	repObj.WidthList = fieldWidths;
	repObj.FldOpts = fOptions;
	var uriResult = encodeURIComponent(JSON.stringify(repObj));
	return uriResult;
}

function DesignReportRequest(s) {
	var requestString = 'wscmd=designreport';
	requestString += '&wsarg0=' + s;
	AjaxRequest('./rs.aspx', requestString, ReportDesigned, null, 'designreport');
}

function ReportDesigned(returnObj, id) {
	if (id != 'designreport' || returnObj == undefined || returnObj == null)
		return;
	if (returnObj.Value == "OK")
	    window.location = nirConfig.ReportDesignerUrl + "?tab=fields";
	else
		alert("Error: " + returnObj.Value);
}

function ViewReportRequest(s) {
	var requestString = 'wscmd=viewreport';
	requestString += '&wsarg0=' + s;
	AjaxRequest('./rs.aspx', requestString, ReportViewed, null, 'viewreport');
}

function ReportViewed(returnObj, id) {
	if (id != 'viewreport' || returnObj == undefined || returnObj == null)
		return;
	if (returnObj.Value == "OK")
	    window.location = nirConfig.ReportViewerUrl;
	else
		alert("Error: " + returnObj.Value);
}

function UpdateMSV(lid) {
	var label = document.getElementById(lid);
	var msvs = label.getAttribute('msvs').split(',');
	var msv = label.getAttribute('msv');
	var curInd = -1;
	for (var ind = 0; ind < msvs.length; ind++) {
		if (msvs[ind] == msv) {
			curInd = ind;
			break;
		}
	}
	curInd++;
	if (curInd >= msvs.length)
		curInd = 0;
	label.setAttribute('msv', msvs[curInd]);
	label.innerHTML = msvs[curInd];
	PreviewFieldDelayed(1000);
}

function ShowFieldProperties(fieldSqlName, friendlyName, fiIds) {
  curFieldIndexes = fiIds;
	curPropField = fieldSqlName;
	var description = friendlyName;
	var totalChecked = 0;
	var vgChecked = 0;
	var labelJ = document.getElementById('labelJ');
	var valueJ = document.getElementById('valueJ');
	var labelJVal = labelJ.getAttribute('msvs')[0];
	var valueJVal = valueJ.getAttribute('msvs')[0];
	var opts = fieldsOpts[fieldSqlName];
	if (opts != null) {
		description = opts.Description;
		totalChecked = opts.TotalChecked;
		vgChecked = opts.VgChecked;
		labelJVal = opts.LabelJVal;
		valueJVal = opts.ValueJVal;
	}
	var titleDiv = document.getElementById('titleDiv');
	titleDiv.innerHTML = 'Field Properties for ' + friendlyName;
	document.getElementById('propDescription').value = description;
	var propTotal = document.getElementById('propTotal');
	propTotal.checked = false;
	if (totalChecked == 1)
		propTotal.checked = true;
	var propVG = document.getElementById('propVG');
	propVG.checked = false;
	if (vgChecked == 1)
		propVG.checked = true;
	labelJ.innerHTML = labelJVal;
	labelJ.setAttribute('msv', labelJVal);
	valueJ.innerHTML = valueJVal;
	valueJ.setAttribute('msv', valueJVal);
	if (IsIE()) {
		labelJ.style.left = '-17px';
		valueJ.style.left = '-17px';
	}
	UpdateFieldPropFormats();
}

function UpdateFieldPropFormats() {
	var requestString = 'wscmd=fieldformats';
	requestString += '&wsarg0=' + curPropField;
	AjaxRequest('./rs.aspx', requestString, FieldPropFormatsGot, null, 'fieldformats');
}

function FieldPropFormatsGot(returnObj, id) {
	if (id != 'fieldformats' || returnObj == undefined || returnObj == null)
		return;
	var propFormats = document.getElementById('propFormats');
	propFormats.options.length = 0;
	if (returnObj.Value == "OK" && returnObj.AdditionalData != null && returnObj.AdditionalData.length > 0) {
		var propFormat = '...';
		if (fieldsOpts[curPropField] != null)
			propFormat = fieldsOpts[curPropField].Format;
		var formatNames = new Array();
		var formatValues = new Array();
		var fCnt = 0;
		var avCnt = 0;
		while (avCnt < returnObj.AdditionalData.length) {
			formatNames[fCnt] = returnObj.AdditionalData[avCnt];
			avCnt++;
			formatValues[fCnt] = returnObj.AdditionalData[avCnt];
			avCnt++;
			fCnt++;
		}
		for (var formatCnt = 0; formatCnt < formatValues.length; formatCnt++) {
			var opt = new Option();
			opt.value = formatValues[formatCnt];
			opt.text = formatNames[formatCnt];
			if (propFormat == formatValues[formatCnt])
				opt.selected = 'selected';
			propFormats.add(opt);
		}
		PreviewFieldDelayed(100);
	}
}

function StoreFieldProps() {
	var opts = new Object();
	opts.Description = document.getElementById('propDescription').value;
	var propTotal = document.getElementById('propTotal');
	if (propTotal.checked)
		opts.TotalChecked = 1;
	else
		opts.TotalChecked = 0;
	var propVG = document.getElementById('propVG');
	if (propVG.checked)
		opts.VgChecked = 1;
	else
		opts.VgChecked = 0;
	var labelJ = document.getElementById('labelJ');
	var valueJ = document.getElementById('valueJ');
	opts.LabelJVal = labelJ.getAttribute('msv');
	opts.ValueJVal = valueJ.getAttribute('msv');
	var propFormats = document.getElementById('propFormats');
	opts.Format = propFormats.value;
	fieldsOpts[curPropField] = opts;
	if (curFieldIndexes == null || curFieldIndexes == '')
	  return;
	var s = curFieldIndexes.split('fcb');
	if (s.length != 2 || s[0].length < 4 || s[0].length <= 0)
	  return;
	var tcbInd = s[0].substr(3);
	var fcbInd = s[1];
  FiClick(tcbInd, fcbInd, true, true);
}

function PreviewFieldManual() {
  $(document.getElementById('fieldSamplePreview')).html('<table width="100%"><tr width="100%"><td width="100%" align="center"><img src="rs.aspx?image=loading.gif"></img></tr></td></table>');
  PreviewFieldToDiv();
}

function PreviewFieldDelayed(timeOut) {
	try {
		clearTimeout(previewFieldTimeout);
	}
	catch (e) {
	}
	$(document.getElementById('fieldSamplePreview')).html('<table width="100%"><tr width="100%"><td width="100%" align="center"><img src="rs.aspx?image=loading.gif"></img></tr></td></table>');
	previewFieldTimeout = setTimeout(PreviewFieldToDiv, timeOut);
}

function PreviewFieldToDiv() {
	PreviewField(curPropField, document.getElementById('fieldSamplePreview'));
}

function PreviewField(field, container) {
	var requestString = 'wscmd=getfieldpreview';
	var description = document.getElementById('propDescription').value;
	var propTotal = document.getElementById('propTotal');
	var totalChecked = 0;
	if (propTotal.checked)
		totalChecked = 1;
	var propVG = document.getElementById('propVG');
	var vgChecked = 0;
	if (propVG.checked)
		vgChecked = 1;
	var propFormats = document.getElementById('propFormats');
	var format = propFormats.value;
	var fieldOpts = '';
	var labelJ = document.getElementById('labelJ');
	var valueJ = document.getElementById('valueJ');
	var labelJVal = labelJ.getAttribute('msv');
	var valueJVal = valueJ.getAttribute('msv');
	fieldOpts += ',\'Desc\':\'' + description + '\',\'Total\':\'' + totalChecked + '\',\'Vg\':\'' + vgChecked + '\',\'LabelJ\':\'' + labelJVal + '\',\'ValueJ\':\'' + valueJVal + '\',\'Format\':\'' + format + '\'';
	requestString += "&wsarg0=" + encodeURIComponent("{'Na':'" + field + "','Cnt':'10'" + fieldOpts + "}");

	var thisRequestObject;
	if (window.XMLHttpRequest)
		thisRequestObject = new XMLHttpRequest();
	else if (window.ActiveXObject)
		thisRequestObject = new ActiveXObject('Microsoft.XMLHTTP');
	thisRequestObject.requestId = 'getfieldpreview';
	thisRequestObject.dtk = container;
	thisRequestObject.onreadystatechange = FieldPreviewed;

	thisRequestObject.open('GET', './rs.aspx?' + requestString, true);
	thisRequestObject.send();

	function FieldPreviewed(returnObj, id) {
		if (thisRequestObject.readyState == 4 && thisRequestObject.status == 200)
			$(container).html(thisRequestObject.responseText);
	}
}

function PreviewReportManual() {
  $(document.getElementById('rightHelpDiv')).html('<table width="100%"><tr width="100%"><td width="100%" align="center"><img src="rs.aspx?image=loading.gif"></img></tr></td></table>');
  PreviewReportToDiv();
}

function PreviewReportDelayed(timeOut) {
	try {
		clearTimeout(previewReportTimeout);
	}
	catch (e) {
	}
	$(document.getElementById('rightHelpDiv')).html('<table width="100%"><tr width="100%"><td width="100%" align="center"><img src="rs.aspx?image=loading.gif"></img></tr></td></table>');
	previewReportTimeout = setTimeout(PreviewReportToDiv, timeOut);
}

function PreviewReportToDiv() {
	PreviewReport(document.getElementById('rightHelpDiv'));
}

function InitEmptyPreviewArea(container) {
	var container$ = $(container);
	container$.empty();

	var h2$ = $('<h2 style="margin:0px; margin-bottom:20px;"><a class="button default" href="#update_preview" onclick="javascript:PreviewReportManual();">Preview</a></h2>');
	container$.append(h2$);
  
  var div$ = $('<div>');
	
	div$.addClass('preview-wrapper-empty');
	div$.text('Drag field here to preview');
	container$.append(div$);
	
	container$.droppable({
		accept: 'a.field',
		drop: function (event, ui) {
			fieldsDragPreformingNow = false;
		}
	});
}

function PreviewReport(container) {
	var requestString = 'wscmd=getreportpreview';
	requestString += "&wsarg0=" + CollectReportData();
	var origRn = CurrentRn();
	if (origRn != '') {
	  requestString += "&wsarg1=" + encodeURIComponent(origRn);
	  requestString += "&wsarg2=" + encodeURIComponent(origRsData);
	}
	var thisRequestObject;
	if (window.XMLHttpRequest)
		thisRequestObject = new XMLHttpRequest();
	else if (window.ActiveXObject)
		thisRequestObject = new ActiveXObject('Microsoft.XMLHTTP');
	thisRequestObject.requestId = 'getreportpreview';
	thisRequestObject.dtk = container;
	thisRequestObject.onreadystatechange = ReportPreviewed;
	thisRequestObject.open('POST', './rs.aspx', true);
	thisRequestObject.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
	thisRequestObject.send(requestString);

	function ReportPreviewed(returnObj, id) {
		if (thisRequestObject.readyState == 4 && thisRequestObject.status == 200) {
			$(container).empty();

			var h2$ = $('<h2 style="margin:0px; margin-bottom:20px;"><a class="button default" href="#update_preview" onclick="javascript:PreviewReportManual();">Preview</a></h2>');
			h2$.appendTo(container);

			var containerWrapper$ = $('<div>');
			containerWrapper$.addClass('preview-wrapper');
			containerWrapper$.appendTo(container);

			containerWrapper$.html(thisRequestObject.responseText);

			var visualGroupUsed = (thisRequestObject.responseText.indexOf('class=\'VisualGroup\'') >= 0);
			if (visualGroupUsed) {
			    var tablesContainer$ = $('<div>');
			    var mainTableTemplate$ = $('.preview-wrapper table.ReportTable').clone().html('');
			    var tableIndex = 1;
			    $('.preview-wrapper table.ReportTable').find('tr').each(function (i) {
			        if ($(this).attr("class") == 'VisualGroup' && i == 0) {
			            var vgTitleTable$ = $('<table>');
			            vgTitleTable$.attr('targettable', 'ReportTable_1');
			            vgTitleTable$.append($(this).clone());
			            tablesContainer$.append(vgTitleTable$);
			        }
			        else if ($(this).attr("class") == 'VisualGroup' && i != 0) {
			            var tblToInsert = mainTableTemplate$.clone();
			            tblToInsert.attr('class', 'ReportTable_' + tableIndex).attr('name', 'ReportTable_' + tableIndex);
			            tablesContainer$.append(tblToInsert);

			            var vgTitleTable$ = $('<table>');
			            var nextIndex = tableIndex + 1;
			            vgTitleTable$.attr('targettable', 'ReportTable_' + nextIndex);
			            vgTitleTable$.append($(this).clone());
			            tablesContainer$.append(vgTitleTable$);
			            
			            tableIndex++;
			            mainTableTemplate$.html('');
			            //mainTableTemplate$.append($(this).clone());
			        }
			        else if (i == $('.preview-wrapper table.ReportTable').find('tr').length - 1) {
			        		mainTableTemplate$.append($(this).clone());
			            tablesContainer$.append(mainTableTemplate$.clone().attr('class', 'ReportTable_' + tableIndex).attr('name', 'ReportTable_' + tableIndex));
			            tableIndex++;
			        }
			        else {
			            mainTableTemplate$.append($(this).clone());
			        }
			    });
			    tablesContainer$.find('tr.VisualGroup').find('td').attr('style', 'border-width:0px;overflow:hidden;white-space: nowrap;');
			    tablesContainer$.find('tr.VisualGroup').attr('onclick', 'javascript:EBC_ExpandTable_New(this);');
			    $('.preview-wrapper table.ReportTable').replaceWith(tablesContainer$.html());
			    try {
			        var preview;
			        for (var i = 1 ; i < tableIndex ; i++) {
			            preview = new DataSourcesPreview('table.ReportTable_' + i);
			            if (i == tableIndex - 1)
			                setTimeout(function () { preview.ResizeColumnsInAllTables($('table.ReportTable_1')); }, 0);
			        }
			    }
			    catch (e) {
			    }
			}
			else {
			    try {
			        var preview = new DataSourcesPreview('table.ReportTable');
			    }
			    catch (e) {
			    }
			}
			

			//initializePreviewTable();
		}
	}
}

/**
 * Drag'n'drop from datasource fields to preview table
 */
var fieldsDragPreformingNow = false;
var fieldDragged$ = null;
var isEmptyTable = false;
var newThCurrent = null;
var newThCurrent_index = null;

function initDraggable() {
  $('a.field').draggable({
    cancel: 'a.field.checked, a.field[locked="true"]',
    cursor: 'move',
    accept: 'table.ReportTable, div.preview-wrapper-empty',
    helper: function(event, ui) {
      var foo = $('<span style="z-index: 1001; background-color: #0d70cd; white-space: nowrap;"></span>');
      var target = $(event.currentTarget).clone();
      target.css('background-color', '#0d70cd');
      foo.append(target);
      return foo;
    },
    //helper: 'clone',
    revert: false,
    opacity: 0.5,

    start: function(event, ui) {
      fieldsDragPreformingNow = true;
      fieldDragged$ = $(event.currentTarget);
      if ($('table.ReportTable').length == 0 && $('table.ReportTable_1').length == 0) {
        // no preview
        isEmptyTable = true;
      } else {
        // preview exists
        isEmptyTable = false;
      }
    },

    drag: function(event, ui) {
      var dragTarget = $('table.ReportTable');
      var rTableOffset = dragTarget.offset();
      var w = $(dragTarget).width();
      var h = $(dragTarget).height();
      if (rTableOffset != null) {
          if (ColReorder.aoInstances == 0)
              return;
          var colReorder = ColReorder.aoInstances[ColReorder.aoInstances.length - 1];
          if (colReorder == null)
              return;
          if (event.pageX < rTableOffset.left - 100 || event.pageX > rTableOffset.left + w + 100
                      || event.pageY < rTableOffset.top || event.pageY > rTableOffset.top + h) {
              if (newThCurrent != null)
                  newThCurrent.remove();
              newThCurrent = null;
              colReorder._fnClearDrag.call(colReorder, event);
              return;
          } else {
              if (newThCurrent != null)
                  return;
              var nTh = $('table.ReportTable thead tr:first-child');
              newThCurrent = $('<th>');
              event['target'] = newThCurrent[0];
              colReorder._fnMouseDownHiddenHelper.call(colReorder, event, nTh);
          }
      }
      else {
          // Dirty workaround. Need to know the exact tables count
          for (var i = 1; i < 1000; i++) {
              {
                var table = $('table.ReportTable_' + i);
                if (table == undefined || table == null)
                  break;

                var rTableOffset_i = table.offset();
                var w_i = $(table).width();
                var h_i = $(table).height();
                if (rTableOffset_i != null) {
                    if (oDatatable['table.ReportTable_' + i] == null || oDatatable['table.ReportTable_' + i]._oPluginColReorder == null)
                        return;
                    var colReorder = oDatatable['table.ReportTable_' + i]._oPluginColReorder;
                    if (event.pageX < rTableOffset_i.left - 100 || event.pageX > rTableOffset_i.left + w_i + 100
                                || event.pageY < rTableOffset_i.top || event.pageY > rTableOffset_i.top + h_i) {
                        if (newThCurrent_index == i) {
                            if (newThCurrent != null)
                                newThCurrent.remove();
                            newThCurrent = null;
                            colReorder._fnClearDrag.call(colReorder, event);
                        }
                        continue;
                    } else {
                        if (newThCurrent != null && newThCurrent_index == i)
                            return;
                        var nTh = $('table.ReportTable_' + i + ' thead tr:first-child');
                        newThCurrent = $('<th>');
                        event['target'] = newThCurrent[0];

                        if (oDatatable['table.ReportTable_' + newThCurrent_index] != null && oDatatable['table.ReportTable_' + newThCurrent_index]._oPluginColReorder != null) {
                            var colReorder_prev = oDatatable['table.ReportTable_' + newThCurrent_index]._oPluginColReorder;
                            colReorder_prev._fnClearDrag.call(colReorder_prev, event);
                        }
                        newThCurrent_index = i;
                        colReorder._fnMouseDownHiddenHelper.call(colReorder, event, nTh);
                    }
                }
              } (i);
          }
      }
    },

    stop: function(event, ui) {
      fieldsDragPreformingNow = false;
      if (isEmptyTable) {
        var dragTarget = $('div.preview-wrapper-empty');
        var rTableOffset = dragTarget.offset();
        var w = $(dragTarget).outerWidth();
        var h = $(dragTarget).outerHeight();
        if (rTableOffset != null)
          if (event.pageX < rTableOffset.left || event.pageX > rTableOffset.left + w
						|| event.pageY < rTableOffset.top || event.pageY > rTableOffset.top + h)
          return;
        fieldDragged$.attr('sorder', 1);
        var helperAttr = fieldDragged$.attr('onmouseup');
        if (helperAttr != null) {
          var helper2 = helperAttr.replace('FiClick', 'FiClickForcedDrag');
          eval(helper2);
        }
      }
      if (updateOnDrag)
        setTimeout(PreviewReportManual, 100);
    }
  });

}

function NDS_CanBeJoined(dsArray) {
	if (dsArray.length < 2)
		return true;
	for (var i = 0; i < dsArray.length; i++) {
		var canBeJoined = false;
		for (var j = 0; j < dsArray.length; j++) {
			if (i == j)
				continue;
			for (var k = 0; k < ebc_constraintsInfo.length; k++) {
				if (ebc_constraintsInfo[k][0] == dsArray[i] && ebc_constraintsInfo[k][1] == dsArray[j]) {
					canBeJoined = true;
					break;
				}
				if (ebc_constraintsInfo[k][0] == dsArray[j] && ebc_constraintsInfo[k][1] == dsArray[i]) {
					canBeJoined = true;
					break;
				}
			}
			if (canBeJoined)
				break;
		}
		if (!canBeJoined)
			return false;
	}
	return true;
}

function NDS_CanBeJoinedWithGiven(dsArray, dsAdded) {
	dsArray[dsArray.length] = dsAdded;
	var canBe = NDS_CanBeJoined(dsArray);
	dsArray.splice(dsArray.length - 1, 1);
	return canBe;
}

function NDS_CanBeJoinedWithoutGiven(dsArray, dsRemoved) {
	var canBe = NDS_CanBeJoined(dsArray);
	for (var i = 0; i < dsArray.length; i++) {
		if (dsArray[i] == dsRemoved) {
			dsArray.splice(i, 1);
			canBe = NDS_CanBeJoined(dsArray);
			dsArray[dsArray.length] = dsRemoved;
			break;
		}
	}
	return canBe;
}

function NDS_SetFiSOrd(fcb, fiOrd) {
	fcb.setAttribute('sorder', fiOrd);
	jq$(fcb).removeClass('checked');
	if (fiOrd >= 0)
		jq$(fcb).addClass('checked');
}

function NDS_SetFiAvailability(fcb, available, checkBoxChildInd) {
	if (available) {
		fcb.childNodes[checkBoxChildInd].style.backgroundColor = '#FFFFFF';
		fcb.setAttribute('locked', false);
	}
	else {
		fcb.childNodes[checkBoxChildInd].style.backgroundColor = '#CCCCCC';
		fcb.setAttribute('locked', true);
	}
}

function NDS_UpdateFiOpacity(fcb) {
	var locked = fcb.getAttribute('locked');
	var fiOrd = fcb.getAttribute('sorder');
	if (fiOrd != '-1' || locked == 'true')
		fcb.childNodes[3].style.opacity = '1';
	else
		fcb.childNodes[3].style.opacity = '0.25';
}

function NDS_SetDsSOrd(cb, sord, dsInd, forceRefresh) {
  var wasOrd = cb.getAttribute('sorder');
  if (wasOrd == '-1' && sord == '-1' && !forceRefresh)
    return;
	cb.setAttribute('sorder', sord);
	var pe = cb.parentElement.parentElement.parentElement;
	jq$(pe).removeClass('checked');
	if (sord >= 0)
		jq$(pe).addClass('checked');
	if (sord == '-1') {
		var fdInd = 0;
		var fcb;
		while (true) {
			fcb = document.getElementById('tcb' + dsInd + 'fcb' + fdInd);
			if (fcb == null)
				break;
			NDS_SetFiSOrd(fcb, -1);
			fdInd++;
		}
	}
}

function NDS_SetDsAvailability(cb, available, tInd) {
	if (available) {
		cb.childNodes[0].style.backgroundColor = '#FFFFFF';
		cb.setAttribute('locked', false);
	}
	else {
		cb.childNodes[0].style.backgroundColor = '#CCCCCC';
		cb.setAttribute('locked', true);
	}
	if (dsState[tInd] < 2)
		return;
	var dsSelected = cb.getAttribute('sorder');
	var fInd = 0;
	var f;
	var firstField = document.getElementById('tcb' + tInd + 'fcb0');
	if (firstField != null) {
		var checkBoxChildInd = -1;
		for (var ci = 0; ci < firstField.childNodes.length; ci++) {
			if (typeof firstField.childNodes[ci].getAttribute != 'undefined' && firstField.childNodes[ci].getAttribute('class') == 'checkbox') {
				checkBoxChildInd = ci;
				break;
			}
		}
		if (checkBoxChildInd < 0) {
			checkBoxChildInd = 3;
		}
		while (true) {
			f = document.getElementById('tcb' + tInd + 'fcb' + fInd);
			if (f == null)
				break;
			NDS_SetFiAvailability(f, available || dsSelected != '-1', checkBoxChildInd);
			fInd++;
		}
	}
}

function NDS_UpdateDsOpacity(cb, dsInd) {
	var locked = cb.getAttribute('locked');
	var sorder = cb.getAttribute('sorder');
	if (locked == 'false' && sorder == '-1')
		cb.childNodes[0].style.opacity = '0.25';
	else
		cb.childNodes[0].style.opacity = '1';
  if (dsState[dsInd] < 2)
    return;
	var fdInd = 0;
	var fcb;
	while (true) {
		fcb = document.getElementById('tcb' + dsInd + 'fcb' + fdInd);
		if (fcb == null)
			break;
		NDS_UpdateFiOpacity(fcb);
		fdInd++;
	}
}

function NDS_UpdateDatasourcesAvailability(forceRefresh) {
	var dsArr = new Array();
	var dsNames = new Array();
	var dsChecked = new Array();
	var dsSelected = new Array();
	var cb;
	var soVal;
	var index = 0;
	while (true) {
		cb = document.getElementById('tcb' + index);
		if (cb == null)
			break;
		dsArr[dsArr.length] = cb;
		dsNames[dsNames.length] = cb.getAttribute('tableid');
		dsChecked[dsChecked.length] = false;
		soVal = cb.getAttribute('sorder');
		if (soVal == null || soVal == '-1') {
			index++;
			continue;
		}
		dsChecked[dsChecked.length - 1] = true;
		dsSelected[dsSelected.length] = dsNames[dsNames.length - 1];
		index++;
	}
	for (var i = 0; i < dsArr.length; i++) {
		var toDisable = true;
		if (!dsChecked[i]) {
			if (NDS_CanBeJoinedWithGiven(dsSelected, dsNames[i])) {
				toDisable = false;
			}
		}
		else {
			if (NDS_CanBeJoinedWithoutGiven(dsSelected, dsNames[i])) {
				toDisable = false;
			}
	  }
	  if (dsState[i] > 0) {
	    if (toDisable) {
	      NDS_SetDsAvailability(dsArr[i], false, i);
	    } else {
	      NDS_SetDsAvailability(dsArr[i], true, i);
	    }
	  }
	  NDS_SetDsSOrd(dsArr[i], dsArr[i].getAttribute('sorder'), i, forceRefresh);
	  if (dsState[i] > 0)
		  NDS_UpdateDsOpacity(dsArr[i], i);
	}
}

function NDS_RefreshOpenedList() {
  dsState.length = 0;
  var tcbInd = -1;
  while (true) {
    tcbInd++;
    var ds = document.getElementById('tcb' + tcbInd);
    if (ds == null)
      break;
    dsState[tcbInd] = 0;
    ds = ds.parentElement.parentElement.parentElement;
    var isOpened = false;
    if (jq$(ds).hasClass('opened'))
      isOpened = true;
    ds = ds.parentElement.parentElement;
    var isShown = false;
    if (jq$(ds).hasClass('opened'))
      isShown = true;
    if (isShown) {
      dsState[tcbInd] = 1;
      if (isOpened)
        dsState[tcbInd] = 2;
    }
  }
}

function DsDomChanged() {
  NDS_RefreshOpenedList();
  NDS_UpdateDatasourcesAvailability(false);
}

function NDS_Init() {
	EBC_Init("rs.aspx?", 0, false, 0);
	EBC_LoadConstraints();
}

function EngageDs(clicked) {
	var cso = clicked.getAttribute('sorder');
	if (cso == null || cso == '-1') {
		clicked.setAttribute('sorder', csOrder);
		csOrder++;
	}
}

function DisengageDs(clicked) {
	clicked.setAttribute('sorder', '-1');
}

function NDS_StoreDsSelection(tind) {
	var fi;
	var fIndex = 0;
	var selected = new Array();
	while (true) {
		fi = document.getElementById('tcb' + tind + 'fcb' + fIndex);
		if (fi == null)
			break;
		var so = fi.getAttribute('sorder');
		if (so != '-1')
			selected[selected.length] = fIndex + '-' + so;
		fIndex++;
	}
	dsSelections[tind] = selected;
}

function NDS_RestoreDsSelection(tind) {
	if (dsSelections[tind] != null && dsSelections[tind].length > 0) {
		for (var dsCnt = 0; dsCnt < dsSelections[tind].length; dsCnt++) {
			var sVals = dsSelections[tind][dsCnt].split('-');
			if (sVals.length == 2) {
				var fc = document.getElementById('tcb' + tind + 'fcb' + sVals[0]);
				NDS_SetFiSOrd(fc, sVals[1]);
			}
		}
	}
}

function initFieldsDsp(nwid) {
  var hId = nwid.id;
  hId = hId.substr(4);
  var contentDiv = document.getElementById('rdb' + hId);
  var currHtml = contentDiv.innerHTML;
  if (currHtml != 'Loading...')
    return;
  var firstUnder = hId.indexOf('_');
  var dbKey = hId.substr(0, firstUnder);
  var tKey = hId.substr(firstUnder + 1);

  var willBeTableIndex = $(nwid).attr('tableInd');
  fieldsIndex = 0;
  var html = renderSections(willBeTableIndex, databaseSchema[dbKey].tables[tKey].fields);
  html = '<div class=\'table-fields-sections-background\'></div>' + html;
  contentDiv.innerHTML = html;

  initDraggable();
  $(".database-header a, .table-header a, a.field, .table-header a .checkbox-container, a.uncheck, a.collapse").click(function(event) {
    event.preventDefault();
  });
  var triggersHtml = "<span class='f-trigger' data-view='fields-view'> \
							<img src='rs.aspx?image=ModernImages.fields-icon.png' alt='' /> <span class='text'>Fields</span> \
						</span> \
						<span class='p-trigger' data-view='preview-view'>Preview</span> \
						<span class='v-trigger' data-view='visuals-view'>Visuals</span> \
						<span class='b-trigger' data-view='relationships-view'>Relationships</span> \ ";
  $(".table-view-triggers").filter(function(index) {
    var shouldBeReturned = false;
    var npAttr;
    try {
      npAttr = this.getAttribute('notProcessed1');
    }
    catch (e) {
      npAttr = '0';
    }
    if (npAttr == '1') {
      shouldBeReturned = true;
      this.setAttribute('notProcessed1', '0');
    }
    return shouldBeReturned;
  }).append(triggersHtml);

  $(".table").each(function() {
    setView($(this), "fields-view");
  });

  $(".field-popup-trigger").mouseup(function(event) {
  	event.cancelBubble = true;
  	(event.stopPropagation) ? event.stopPropagation() : event.returnValue = false;
  	(event.preventDefault) ? event.preventDefault() : event.returnValue = false;
    var parent = this.parentElement;
    var fieldSqlName = parent.getAttribute('fieldid');
    if (fieldSqlName != null && fieldSqlName != '') {
      ShowFieldProperties(fieldSqlName, parent.children[2].innerHTML, parent.getAttribute('id'));
    }
    var fieldName = $(this).parent().find(".field-name").text();

    fieldPopup.dialog("option", "title", fieldName);
    fieldPopup.dialog("open");
    return false;
  });
}

function DsClicked(dsInd) {
	var clicked = document.getElementById('tcb' + dsInd);

	if (clicked.getAttribute('locked') == 'false') {
		var cso = clicked.getAttribute('sorder');
		if (cso == null || cso == '-1') {
			EngageDs(clicked);
			NDS_RestoreDsSelection(dsInd);
		}
		else
			DisengageDs(clicked);
	}
	NDS_UpdateDatasourcesAvailability(false);
	var clicked$ = $(clicked);
	var table$ = clicked$.closest("div.table");
	initFieldsDsp(clicked.parentNode);  
	table$.addClass("opened");
	if (table$.hasClass('checked')) {
		table$.removeClass('checked');
		$.each(table$.find('a.field'), function (i, f) {
			var field$ = $(f);
			var sorder = field$.attr('sorder');
			if (sorder != -1 && sorder != '-1') {
				eval(field$.attr('onmouseup'));
			}
		});
	}
}

function NDS_UnckeckAllDs() {
	var index = 0;
	var cb;
	while (true) {
		cb = document.getElementById('tcb' + index);
		if (cb == null)
			break;
		DisengageDs(cb);
		index++;
	}
	NDS_UpdateDatasourcesAvailability(true);
}

function FiClick(tind, find, programmatic, enableOnly) {
  if (fieldsDragPreformingNow && !programmatic)
    return;
	var storeSelection = false;
	var clickedDs = document.getElementById('tcb' + tind);
	if (clickedDs.getAttribute('locked') == 'false' || clickedDs.getAttribute('sorder') != '-1') {
		var clickedFi = document.getElementById('tcb' + tind + 'fcb' + find);
		var cso = clickedFi.getAttribute('sorder');
		if (cso == null || cso == '-1') {
			EngageDs(clickedDs);
			NDS_SetFiSOrd(clickedFi, csOrder);
			storeSelection = true;
			csOrder++;
	  } else {
	    if (!enableOnly)
			  NDS_SetFiSOrd(clickedFi, '-1');
		}
	}
	NDS_UpdateDatasourcesAvailability(false);
	if (storeSelection)
	  NDS_StoreDsSelection(tind);
	if (updateOnClick)
	  PreviewReportManual();
}

function FiClickForcedDrag(tind, find, programmatic, enableOnly) {
  var clickedDs = document.getElementById('tcb' + tind);
  if (clickedDs.getAttribute('locked') == 'false' || clickedDs.getAttribute('sorder') != '-1') {
    var clickedFi = document.getElementById('tcb' + tind + 'fcb' + find);
    EngageDs(clickedDs);
    $(clickedFi).addClass('checked');
    csOrder++;
  }
  NDS_UpdateDatasourcesAvailability(false);
  NDS_StoreDsSelection(tind);
  if (updateOnDrag)
    PreviewReportManual();
}

function GetInstantReportConfig() {
    var requestString = 'wscmd=instantreportconfig';
    AjaxRequest('./rs.aspx', requestString, GotInstantReportConfig, null, 'instantreportconfig');
}

function GotInstantReportConfig(returnObj, id) {
    if (id != 'instantreportconfig' || returnObj == undefined || returnObj == null)
        return;
    nirConfig = returnObj;

    $(".database-header a, .table-header a, a.field, .table-header a .checkbox-container, a.uncheck, a.collapse").click(function (event) {
        event.preventDefault();
    });

    var triggersHTML = "<span class='f-trigger' data-view='fields-view'> \
							<img src='rs.aspx?image=ModernImages.fields-icon.png' alt='' /> <span class='text'>Fields</span> \
						</span> \
						<span class='p-trigger' data-view='preview-view'>Preview</span> \
						<span class='v-trigger' data-view='visuals-view'>Visuals</span> \
						<span class='b-trigger' data-view='relationships-view'>Relationships</span> \ ";
    $(triggersHTML).appendTo(".table-view-triggers");

    $(".table-header a .table-view-triggers span").live("click", function (event) {
    	event.cancelBubble = true;
    	(event.stopPropagation) ? event.stopPropagation() : event.returnValue = false;
    	(event.preventDefault) ? event.preventDefault() : event.returnValue = false;
        var trigger = $(this);
        var table = $(this).closest(".table");
        var view = trigger.attr("data-view");
        setView(table, view);
        if (!table.hasClass('opened')) {
            collapseTables();
            table.addClass("opened", animationTime);
        }
    });

    $(".table").each(function () {
        setView($(this), "fields-view");
    });

    leftDiv = document.getElementById('leftDiv');
    pdiv = document.getElementById('rightHelpDiv');
    whiteHeader = document.getElementById('whiteHeader');
    blueHeader = document.getElementById('blueHeader');
    setInterval(checkLeftHeight, 100);

    $(window).resize(function (event) {
        checkLeftHeight();
        updatePreviewPosition(event);
    });
    $(window).scroll(function (event) {
        updatePreviewPosition(event);
    });
    checkLeftHeight();
    updatePreviewPosition(null);
}