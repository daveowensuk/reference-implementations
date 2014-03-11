﻿var filtersData;
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
    case 11:
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
      result[0] = document.getElementById('ndbfc' + index).parentElement.id;
      result[1] = document.getElementById('ndbfc' + index).value;
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
      break;
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
  var htmlFilters = document.getElementById('htmlFilters');
  if (returnObj.Filters == null || returnObj.Filters.length <= 0) {
    var fHtml = '<div id="updateBtnP" class="f-button">';
    fHtml += '<a class="blue" onclick="javascript:GetRenderedReportSet(true);" href="javascript:void(0);"><img src="rs.aspx?image=ModernImages.refresh-white.png" alt="Refresh" /><span class="text">Update results</span></a>';
    fHtml += '</div>';
    htmlFilters.innerHTML = fHtml;
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
  fHtml += '<a class="blue" onclick="javascript:CommitFiltersData(true);" href="javascript:void(0);"><img src="rs.aspx?image=ModernImages.refresh-white.png" alt="Refresh" /><span class="text">Update results</span></a>';
  fHtml += '</div></td></tr></table>';
  htmlFilters.innerHTML = fHtml;

  for (var cc = 0; cc < calendars.length; cc++)
  	jq$(document.getElementById(calendars[cc])).datepicker(/*{ dateFormat: dateFormatString }*/);

  jq$(htmlFilters).find(".comboboxTreeMultyselect").each(function () {
      var treeControl = jq$(this);
      var index = treeControl.attr("index");
      var possibleValues = returnObj.Filters[index].ExistingValues[0];
      CC_InitializeComboTreeView(treeControl);

      if (possibleValues != null && possibleValues != "")
          possibleValues = possibleValues.substr(2, possibleValues.length - 4);
      CC_TreeUpdateValues(treeControl.parent(), possibleValues.split('", "'));

     
  });

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
  GetRenderedReportSet(true);
}

function HidePopupDialog(updateState) {
  if (updateState) {
    var filterIndex = document.getElementById('popupDlgFilterIndex').value;  
    var newVal = '';
    var cnt6 = 0;
    var cb = document.getElementById('ndbfc' + filterIndex + '_cb' + cnt6);
    while (cb != null) {
      if (cb.checked) {
        if (newVal.length > 1)
          newVal += ',';
        newVal += cb.value;
      }
      cnt6++;
      cb = document.getElementById('ndbfc' + filterIndex + '_cb' + cnt6);
    }
    var popupDlgValuesContainer = document.getElementById('ndbfc' + filterIndex);
    popupDlgValuesContainer.value = newVal;
  }
  var popupEsDialog = document.getElementById('popupEsDialog');
  popupEsDialog.style.display = 'none';
  if (updateState) {
    CommitFiltersData(false);
  }
}

function ShowEqualsPopupDialog(filterInd) {
  var filter = filtersData[filterInd];
  var valueInput = document.getElementById('ndbfc' + filterInd);
  var valuesSet6 = valueInput.value.split(',');
  var epdHtml = '<table width="100%"><tr>';
  var inLine = 0;
  var rowsNum = 1;
  var ecbCnt = 0;
  for (var evCnt = 0; evCnt < filter.ExistingValues.length; evCnt++) {
    if (filter.ExistingValues[evCnt] == '...') {
      continue;
    }
    var checked = '';
    for (var cCnt = 0; cCnt < valuesSet6.length; cCnt++) {
      if (valuesSet6[cCnt] == filter.ExistingValues[evCnt]) {
        checked = 'checked = "checked"';
      }
    }
    epdHtml += '<td width="33%" align="left"><input type="checkbox" id="ndbfc' + filterInd + '_cb' + ecbCnt + '" ' + checked + ' value="' + filter.ExistingValues[evCnt] + '" />&nbsp;' + filter.ExistingLabels[evCnt] + '</td>';
    ecbCnt++;
    inLine++;
    if (inLine >= 3) {
      rowsNum++;
      inLine = 0;
      if (evCnt < filter.ExistingValues.length - 1) {
        epdHtml += '</tr><tr>';
      }
    }
  }
  epdHtml += '<input type="hidden" id="popupDlgFilterIndex" value="' + filterInd + '" />';
  var epdContent = document.getElementById('epdContent');
  epdContent.innerHTML = epdHtml;
  var popupEsDialog = document.getElementById('popupEsDialog');
  var windowHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : document.body.clientHeight;
  popupEsDialog.style.height = windowHeight + 'px';
  var epdHeight = rowsNum * 28 + 110;  
  var padTop = ((windowHeight - epdHeight) / 2);
  if (padTop < 140) {
    padTop = 140;
  }
  popupEsDialog.style.paddingTop = padTop + 'px';
  popupEsDialog.style.display = '';
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
      result += '<input type="button" style="height:30px;width:300px;background-color:LightGray;border:1px solid DarkGray" onclick="ShowEqualsPopupDialog(' + index + ');" value="...">';
      result += '<input type="hidden" id="ndbfc' + index + '" value="' + value + '" />';
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
  case 11:
      result += '<div style="display: none;" visibilitymode="1"><input type="text" id="ndbfc' + index + '" value="' + value + '"/></div>';
      result += '<div class="comboboxTreeMultyselect" index=' + index + '></div>';
      break;
    default:
      result = '';
  }
  return result;
}

/**
* Load filters
*/
function GetFiltersData() {
    var requestString = 'wscmd=getfiltersdata';
  AjaxRequest(urlSettings.urlRsPage, requestString, GotFiltersData, null, 'getfiltersdata');
}

function GotFiltersData(returnObj, id) {
  if (id != 'getfiltersdata' || returnObj == null)
    return;
  RefreshFilters(returnObj);
}

CC_InitializeComboTreeView = function (mainControl) {
    mainControl.addClass("comboboxTreeMultyselect");
    mainControl.each(function () {
        var currentControl = jq$(this);
        currentControl.empty();

        var combobox = jq$(document.createElement("div")).addClass("textArea");
        currentControl.append(combobox);

        var selectedValues = jq$(document.createElement("div")).addClass("selectedValues").addClass("hiddenTree");
        currentControl.append(selectedValues);

        var tree = jq$(document.createElement("div")).addClass("tree").addClass("hiddenTree");
        currentControl.append(tree);

        var search = jq$(document.createElement("input")).addClass("search");
        combobox.append(search);

        combobox.click(function () {
            search.focus();
        });

        search.keyup(function () {
            var realText = search.val();
            var text = realText.toLowerCase();

            var list = new Array();
            tree.find(".node").each(function () {
                var node = jq$(this);
                if (node.hasClass("haschild")) {
                    node.addClass("searchHide");
                }
                else {
                    var val = node.attr("value").toLowerCase();
                    var index = val.indexOf(text);
                    if (index > -1) {
                        list.push(node);
                    }
                    else {
                        node.addClass("searchHide");
                    }
                }
            });
            for (var i = 0; i < list.length; i++) {
                var parent = list[i].parent();
                while (!parent.hasClass("tree")) {
                    parent.removeClass("searchHide");
                    parent = parent.parent();
                }
            }

            tree.find(".hiddenBySearch").each(function () {
                var node = jq$(this);
                if (!node.hasClass("searchHide"))
                    node.removeClass("hiddenBySearch");
            });

            tree.find(".searchHide").addClass("hiddenBySearch").removeClass("searchHide");

            tree.find(".node").each(function () {
                var node = jq$(this);
                if (!node.hasClass("hiddenBySearch")) {
                    var textToReplace = node.attr("text");
                    var resultText = textToReplace;
                    var lowerText = textToReplace.toLowerCase();

                    var index = lowerText.indexOf(text);
                    if (index > -1) {
                        resultText = textToReplace.substring(0, index);
                        resultText += "<span class='highlight'>" + textToReplace.substring(index, index + realText.length) + "</span>";
                        resultText += textToReplace.substring(index + realText.length);
                    }
                    node.find("> span.text").html(resultText);
                }
                else {
                    node.find("> span.text").html(node.attr("text"));
                }
            });


            if (tree.hasClass("hiddenTree"))
                tree.removeClass("hiddenTree");

        });


        var showHide = jq$(document.createElement("div")).addClass("showHide");
        showHide.click(function () {
            CC_ClickShowHide(tree);
        });
        combobox.append(showHide);

        jq$(document).click(function (e) {
            var target = jq$(e.target);
            if (target.hasClass("chunkX")) return;
            if (target.closest(tree).length != 0) return;
            if (target.closest(combobox).length != 0) return;
            tree.addClass("hiddenTree");
        });
    });
};

var CC_appendItem = function (node, itemText, prevText, tree, selectedValues, row) {
    var index = itemText.indexOf("|");
    var text = itemText;
    var subNode = "";
    if (index > -1) {
        text = itemText.substr(0, index);
        subNode = itemText.substr(index + 1);
    }

    var value = text;
    if (prevText != "")
        value = prevText + "|" + value;



    var newNode = node.find("> .node[value='" + value.replace(/'/g, "''") + "']");
    if (newNode.length == 0) {
        newNode = jq$(document.createElement("div")).addClass("node").attr("value", value).attr("text", text);
        if (subNode != "")
            newNode.addClass("haschild");
        newNode.html('<div class="collapse" ></div><input type="checkbox" class="checkbox"/><span class="text">' + text + "</span>");
        node.append(newNode);

        newNode.find("> .collapse").click(function () {
            if (newNode.hasClass("haschild")) {
                if (newNode.hasClass("collapsed")) {
                    newNode.removeClass("collapsed");
                } else {
                    newNode.addClass("collapsed");
                }
            }
        });

        newNode.find("> .checkbox").click(function () {
            var isChecked = jq$(this).is(':checked');
            CC_CheckUnchekChild(newNode.find("> .node"), isChecked);
            if (!isChecked) {
                var parent = newNode.parent();
                while (parent.hasClass("node")) {
                    parent.find("> .checkbox").prop('checked', isChecked);
                    parent = parent.parent();
                }
            }
            CC_CheckStatusWasChanged(selectedValues, tree.find("> .node"), tree, row);
        });

    }

    if (subNode != "") {
        CC_appendItem(newNode, subNode, value, tree, selectedValues, row);
    }

};

CC_CheckUnchekChild = function (element, check) {
    element.find("> .checkbox").prop('checked', check);
    element.find("> .node").each(function () {
        CC_CheckUnchekChild(jq$(this), check);
    });
};

CC_CheckStatusWasChanged = function (selectedValues, nodes, tree, row) {
    selectedValues.empty();
    nodes.each(function () {
        CC_FillCombobox(selectedValues, jq$(this), row);
    });

    var strVal = "";
    selectedValues.find(".cValid").each(function () {
        strVal += ", " + jq$(this).attr("value");
    });
    strVal = strVal.substr(2);
    if (strVal == "")
        selectedValues.addClass("hiddenTree");
    else
        selectedValues.removeClass("hiddenTree");

    jq$(row).find("div[visibilitymode=1] input").attr("value", strVal);
};

CC_ClickShowHide = function (tree) {
    if (tree.hasClass("hiddenTree"))
        tree.removeClass("hiddenTree");
    else
        tree.addClass("hiddenTree");
};

CC_FillCombobox = function (selectedValues, node, row) {
    if (node.find("> .checkbox").is(':checked')) {

        var hasChild = node.hasClass("haschild");

        var text = node.attr("text");
        var val = node.attr("value");

        if (text == null || text == "" || val == "" || val == null)
            return;


        var cValid = jq$(document.createElement("a"));

        var displayTesxt = val.replace(/\|/g, "\\");
        if (displayTesxt.length > 50) {
            var newText = "...\\" + text;
            var len = newText.length;
            var i = 0;
            var s = "";
            while (len < 40) {
                s += displayTesxt[i];
                i++;
                len++;
            }
            displayTesxt = s + newText;
        }

        if (hasChild)
            displayTesxt += "\\";

        cValid.addClass("cValid");
        cValid.attr("value", val);
        cValid.html('<nobr>' + displayTesxt + '<img src="Resources/images/icon-blue-x.gif" class="chunkX"></nobr>');
        selectedValues.append(cValid);
        cValid.find(".chunkX").click(function () {
            cValid.remove();
            node.find("> .checkbox").prop("checked", false);
            CC_CheckUnchekChild(node, false);

            var strVal = "";
            selectedValues.find(".cValid").each(function () {
                strVal += ", " + jq$(this).attr("value");
            });
            strVal = strVal.substr(2);
            jq$(row).find("div[visibilitymode=1] input").attr("value", strVal);

            if (strVal == "")
                selectedValues.addClass("hiddenTree");
            else
                selectedValues.removeClass("hiddenTree");
        });
    } else {
        node.find("> .node").each(function () {
            CC_FillCombobox(selectedValues, jq$(this), row);
        });
    }
};

CC_TreeUpdateValues = function (row, values) {

    var selectedValues = jq$(row).find("div[visibilitymode=1] input").attr("value");

    var tree = jq$(row).find(".comboboxTreeMultyselect .tree");
    var selectedValuesControl = jq$(row).find(".comboboxTreeMultyselect .selectedValues");
    tree.empty();
    for (var i = 0; i < values.length; i++) {
        CC_appendItem(tree, values[i], "", tree, selectedValuesControl, row);
    }

    var valueList = selectedValues.split(", ");
    for (var i = 0; i < valueList.length; i++)
        tree.find('.node[value="' + valueList[i] + '"]').each(function () {
            CC_CheckUnchekChild(jq$(this), true);
        });
    CC_CheckStatusWasChanged(selectedValuesControl, tree.find("> .node"), tree, row);
};
