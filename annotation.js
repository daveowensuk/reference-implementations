function AnnotationAjaxRequest(url, parameters, callbackSuccess, callbackError, id, dataToKeep) {
  var thisRequestObject;
  if (window.XMLHttpRequest)
    thisRequestObject = new XMLHttpRequest();
  else if (window.ActiveXObject)
    thisRequestObject = new ActiveXObject('Microsoft.XMLHTTP');
  thisRequestObject.requestId = id;
  thisRequestObject.dtk = dataToKeep;
  thisRequestObject.onreadystatechange = ProcessRequest;
  thisRequestObject.open('POST', url, true);
  thisRequestObject.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
  thisRequestObject.send(parameters);

  function ProcessRequest() {
    if (thisRequestObject.readyState == 4) {
      if (thisRequestObject.status == 200 && callbackSuccess) {
        callbackSuccess(thisRequestObject.responseText, thisRequestObject.requestId, thisRequestObject.dtk);
      } else if (callbackError) {
        callbackError(thisRequestObject);
      }
    }
  }
}

function CurrentRn() {
  var queryParameters = {}, queryString = location.search.substring(1),
    re = /([^&=]+)=([^&]*)/g,
    m;
  while (m = re.exec(queryString)) {
    queryParameters[decodeURIComponent(m[1]).toLowerCase()] = decodeURIComponent(m[2]);
  }
  rn = '';
  if (queryParameters['rn'] != null && queryParameters['rn'].length > 0) {
    rn = queryParameters['rn'];
  }
  return rn;
}

function CreateAnnotationBox(parent) {
  annotationBox = document.createElement('textarea');
  annotationBox.style.width = '150px';
  annotationBox.style.height = '100px';
  annotationBox.style.border = '1px solid #000';
  annotationBox.style.background = 'yellow';
  annotationBox.style.position = 'absolute';
  annotationBox.style.display = 'none';
  annotationBox.style.overflow = 'hidden';
  annotationBox.onblur = function () {
    var aBox = cas_Cell.AnnotationBox;
    if (aBox.value != cas_Cell.Annotation) {
      aBox.disabled = true;
      SetAnnotation();
    } else {
      aBox.style.display = 'none';
      editingAnnotationNow = false;
    }
  };
  parent.appendChild(annotationBox);
  return annotationBox;
}

var firstInitialization = true;
var annotationsInitialized;
var columnNames;
var cas_ReportName;
var cas_Cell;
var nextCas_Cell;
var originalAnnotation;
var editingAnnotationNow = false;
var allowEditAnnotations = false;

function ExtractColumnName(node, depth) {
  var myDepth = depth + 1;
  var deepestResult = null;
  if (node.nodeName.toLowerCase() == 'div') {
    deepestResult = new Object;
    deepestResult.Data = node.innerHTML;
    deepestResult.Depth = myDepth;
  }
  for (var index = 0; index < node.children.length; index++) {
    var tmpResult = ExtractColumnName(node.children[index], myDepth);
    if (tmpResult != null && (deepestResult == null || deepestResult.Depth <= tmpResult.Depth)) {
      if (deepestResult == null)
        deepestResult = new Object();
      deepestResult.Data = tmpResult.Data;
      deepestResult.Depth = tmpResult.Depth;
    }
  }
  return deepestResult;
}

function CellHover() {
  if (allowEditAnnotations)
    this.style.backgroundImage = 'url("triangle.png")';
  if (editingAnnotationNow)
    return;
  if (typeof this.Annotation != 'undefined' && this.Annotation != null && this.Annotation != '') {
    var annotationBox = this.AnnotationBox;
    annotationBox.value = this.Annotation;
    annotationBox.style.display = '';
    annotationBox.style.left = (this.offsetLeft + this.parentElement.offsetLeft + this.parentElement.offsetLeft + this.parentElement.offsetLeft + this.parentElement.offsetLeft) + 'px';
    annotationBox.style.top = (this.offsetTop + this.clientHeight + 5) + 'px';
  }
}

function CellUnHover() {
  if (allowEditAnnotations && (typeof this.Annotation == 'undefined' || this.Annotation == null || this.Annotation == ''))
    this.style.backgroundImage = '';
  if (editingAnnotationNow)
    return;
  if (typeof this.Annotation != 'undefined' && this.Annotation != null && this.Annotation != '') {
    var annotationBox = this.AnnotationBox;
    annotationBox.style.display = 'none';
  }
}

function AssignAnnotation(cell, annotation) {
  cell.style.backgroundPosition = '100% 0%';
  cell.style.backgroundRepeat = 'no-repeat';
  cell.onmouseover = CellHover;
  cell.onmouseout = CellUnHover;
  if (annotation != null && annotation != '')
    cell.style.backgroundImage = 'url("triangle.png")';
  else
    cell.style.backgroundImage = '';
}

function ShowCellAnnotation() {
  var annotationBox = nextCas_Cell.AnnotationBox;
  editingAnnotationNow = true;
  if (annotationBox.disabled) {
    setTimeout(function () {
      ShowCellAnnotation();
    }, 100);
    return;
  }
  cas_Cell = nextCas_Cell;
  cas_ColumnName = cas_Cell.ColName;
  annotationBox.value = cas_Cell.Annotation;
  annotationBox.style.display = '';
  annotationBox.style.left = (cas_Cell.offsetLeft + cas_Cell.parentElement.offsetLeft + cas_Cell.parentElement.offsetLeft + cas_Cell.parentElement.offsetLeft + cas_Cell.parentElement.offsetLeft) + 'px';
  annotationBox.style.top = (cas_Cell.offsetTop + cas_Cell.clientHeight + 5) + 'px';
  annotationBox.focus();
}

function CellActivate() {
  if (!allowEditAnnotations)
    return;
  nextCas_Cell = this;
  setTimeout(function () {
    ShowCellAnnotation();
  }, 100);
}

function ProcessReportTable(ad) {
  var reportTables = document.getElementsByName('reportTable');
  if (reportTables == null || reportTables.length <= 0) {
    setTimeout(function () {
      ProcessReportTable(ad);
    }, 500);
    return;
  }
  for (var tableCnt = 0; tableCnt < reportTables.length; tableCnt++) {
    var reportTable = reportTables[tableCnt];
    var annotationBox;
    if (typeof reportTable.AnnotationBox == 'undefined' || reportTable.AnnotationBox == null) {
      annotationBox = CreateAnnotationBox(reportTable);
      reportTable.AnnotationBox = annotationBox;
    } else
      annotationBox = reportTable.AnnotationBox;
    var currentRow = -1;
    while (currentRow < reportTable.rows.length - 1) {
      var innerHeaderFound = false;
      var headerRow = null;
      var firstDataRow = 0;
      while (currentRow < reportTable.rows.length - 1 && !innerHeaderFound) {
        currentRow++;
        if (reportTable.rows[currentRow].className == 'ReportHeader' && reportTable.rows[currentRow + 1].className != 'ReportHeader' && reportTable.rows[currentRow + 1].className != 'VisualGroup') {
          innerHeaderFound = true;
          headerRow = reportTable.rows[currentRow];
          firstDataRow = currentRow + 1;
        }
      }
      if (!innerHeaderFound || headerRow == null)
        break;
      columnNames = new Array();
      for (var colCnt = 0; colCnt < headerRow.cells.length; colCnt++) {
        var colNameObject = ExtractColumnName(headerRow.cells[colCnt], 0);
        if (colNameObject != null)
          columnNames[columnNames.length] = colNameObject.Data;
        else
          columnNames[columnNames.length] = '';
      }
      for (var rowCnt = firstDataRow; rowCnt < reportTable.rows.length; rowCnt++) {
        if (reportTable.rows[rowCnt].className == 'ReportHeader' || reportTable.rows[rowCnt].className == 'VisualGroup')
          break;
        var row = reportTable.rows[rowCnt];
        for (var colCnt = 0; colCnt < row.cells.length; colCnt++) {
          var cell = row.cells[colCnt];
          var currentCellAnnotation = '';
          var currentCellTenantId = '';
          var adCol = ad[columnNames[colCnt]];
          if (typeof adCol != 'undefined' && adCol != null) {
            var adCell = adCol[rowCnt];
            if (typeof adCell != 'undefined' && adCell != null && cell.innerHTML == adCell.Cd) {
              currentCellAnnotation = adCell.V;
              currentCellTenantId = adCell.T;
            }
          }
          cell.style.cursor = 'default';
          cell.ColName = columnNames[colCnt];
          cell.RowNum = rowCnt;
          cell.CellData = cell.innerHTML;
          cell.TenantId = currentCellTenantId;
          cell.Annotation = currentCellAnnotation;
          cell.onclick = CellActivate;
          cell.AnnotationBox = annotationBox;
          AssignAnnotation(cell, currentCellAnnotation);
        }
      }
    }
    annotationBox.style.display = 'none';
    annotationBox.disabled = false;
    annotationsInitialized = true;
  }
  editingAnnotationNow = false;
}

function AnnotationsGot(returnObj, id) {
  if (id != 'getannotations' || typeof returnObj == 'undefined' || returnObj == null)
    return;
  ad = new Array();
  try {
    eval(returnObj);
  } catch (e) {
    ad = new Array();
  }
  setTimeout(function () {
    ProcessReportTable(ad);
  }, 500);
}

function RefreshAnnotations() {
  if (firstInitialization) {
    firstInitialization = false;
    var toolbar = jq$('.btn-toolbar');
    var div = document.createElement('div');
    div.className = 'btn-group';
    var inner = '<button type="button" class="btn" title="Annotations" id="annotationsBtn" onclick="javascript:AllowEditAnnotations();">';
    inner += '<img class="icon" src="annotation.png" alt="Annotations" />';
    inner += '<span class="hide">Annotations</span>';
    inner += '</button>';
    div.innerHTML = inner;
    toolbar.append(div);
  }
  annotationsInitialized = false;
  var curRn;
  if (typeof reportName == 'undefined' || reportName == null || reportName == '')
    curRn = CurrentRn();
  else
    curRn = reportName;
  cas_ReportName = curRn;
  AnnotationAjaxRequest('Annotation.aspx?', 'acmd=getannotations&reportset=' + cas_ReportName, AnnotationsGot, null, 'getannotations');
}

function AnnotationSet(returnObj, id) {
  if (id != 'setannotation' || typeof returnObj == 'undefined' || returnObj == null)
    return;
  RefreshAnnotations();
}

function SetAnnotation() {
  if (!annotationsInitialized)
    return;
  AnnotationAjaxRequest('Annotation.aspx?', 'acmd=setannotation&reportset=' + cas_ReportName + '&colname=' + cas_Cell.ColName + '&rownum=' + cas_Cell.RowNum + '&celldata=' + cas_Cell.CellData + '&tenantid=' + cas_Cell.TenantId + '&value=' + cas_Cell.AnnotationBox.value, AnnotationSet, null, 'setannotation');
}

function AllowEditAnnotations() {
  allowEditAnnotations = true;
  RefreshAnnotations();
}
