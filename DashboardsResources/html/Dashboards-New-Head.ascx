<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Dashboards-New-Head.ascx.cs" Inherits="Resources_html_Dashboard_New_Head" %>
<title>Dashboards</title>

<link rel="stylesheet" type="text/css" href="DashboardsResources/css/bootstrap-slider.min.css"/>
<link rel="stylesheet" type="text/css" href="DashboardsResources/css/perfect-scrollbar.css"/>
<link rel="stylesheet" type="text/css" href="DashboardsResources/css/dashboard2.css"/>
<link rel="stylesheet" type="text/css" href="DashboardsResources/css/animate.css"/>

<%--Scripts--%>
<script type="text/javascript" src="./rs.aspx?js=ModernScripts.modernizr-2.8.3.min"></script>
<script type="text/javascript" src="./rs.aspx?js=jQuery.jq"></script>
<script type="text/javascript" src="./rs.aspx?js=jQuery.jqui"></script>
<script type="text/javascript" src="./rs.aspx?js=ModernScripts.jquery.purl"></script>
<% #if DEBUG %>
<script type="text/javascript" src="./rs.aspx?js=ModernScripts.jquery.nicescroll"></script>
<% #else %> 
<script type="text/javascript" src="./rs.aspx?js=ModernScripts.jquery.nicescroll.min"></script>
<% #endif %>
<script type="text/javascript" src="./rs.aspx?js=ModernScripts.jsrender.min"></script>
<script type="text/javascript" src="./rs.aspx?js=ModernScripts.bootstrap"></script>
<script type="text/javascript" src="./rs.aspx?js=ModernScripts.url-settings"></script>
<script type="text/javascript" src="./rs.aspx?js=Utility"></script>
<script type="text/javascript" src="./rs.aspx?js=AdHocServer"></script>
<script type="text/javascript" src="./rs.aspx?js=EditorBaseControl"></script>
<script type="text/javascript" src="./rs.aspx?js=HtmlOutputReportResults"></script>
<script type="text/javascript" src="./rs.aspx?js=FilterList"></script>
<script type="text/javascript" src="./rs.aspx?js=NumberFormatter"></script>
<script type="text/javascript" src="./rs.aspx?js=GaugeControl"></script>
<script type="text/javascript" src="./rs.aspx?js=HtmlCharts"></script>
<script type="text/javascript" src="./rs.aspx?js=htmlcharts-more"></script>
<script type="text/javascript" src="./rs.aspx?js=HtmlChartsFunnel"></script>
<script type="text/javascript" src="./rs.aspx?js=ReportScripting"></script>
<script type="text/javascript" src="./rs.aspx?js_nocache=ModernScripts.IzendaLocalization"></script>
<script type="text/javascript" src="./rs.aspx?js=datepicker.langpack"></script>

<%--Dashboards scripts--%>

<script type="text/javascript" src="DashboardsResources/js/bootstrap-slider.min.js"></script>
<script type="text/javascript" src="DashboardsResources/js/perfect-scrollbar.js"></script>
<script type="text/javascript" src="DashboardsResources/js/izenda-trace.js"></script>
<script type="text/javascript" src="DashboardsResources/js/izenda-query.js"></script>
<script type="text/javascript" src="DashboardsResources/js/izenda-filters.js"></script>
<script type="text/javascript" src="DashboardsResources/js/izenda-dashboards.js"></script>

<script>
  // initialize scripts needed for reports
  var urlSettings;
  var nrvConfig;
  var responseServer;
  var responseServerWithDelimeter;

  var oldMouseX = 0;
  var oldMouseY = 0;
  var degree = 0;
  var hueRotateTimeOut = null;

  //will be needed for 10890
  function mfs() {
    var a = document.documentElement.outerHTML;
    AjaxRequest('dashboards.aspx', 'rn=dr&makesnapshot=' + encodeURIComponent(a), null, null, 'setfiltersdata');
  }

  function rotate(e) {
    e.css({ 'filter': 'hue-rotate(' + degree + 'deg)' });
    e.css({ '-webkit-filter': 'hue-rotate(' + degree + 'deg)' });
    e.css({ '-moz-filter': 'hue-rotate(' + degree + 'deg)' });
    e.css({ '-o-filter': 'hue-rotate(' + degree + 'deg)' });
    e.css({ '-ms-filter': 'hue-rotate(' + degree + 'deg)' });
    // Animate rotation with a recursive call
    hueRotateTimeOut = setTimeout(function () {
      var addPath = 0;
      var dx = (window.mouseX - oldMouseX);
      var dy = (window.mouseY - oldMouseY);
      addPath = Math.sqrt(dx * dx + dy * dy);
      var wndPath = Math.sqrt(window.innerHeight * window.innerHeight + window.innerWidth * window.innerWidth);
      addPath = addPath * 360 / wndPath;
      oldMouseX = window.mouseX;
      oldMouseY = window.mouseY;
      if (isNaN(addPath))
        addPath = 0;
      degree += 6 + addPath;
      while (degree > 360)
        degree -= 360;
      rotate(e);
    }, 100);
  }

  function loadTemplates() {
    $.ajax({
      url: urlSettings.urlResources + '/html/dashboards.html',
      async: false
    }).done(function (data) {
      $(data).appendTo('head');
      var placeHolder$ = $('#placeDashboardsHolderId');
      var content$ = $.tmpl($('#dashboardsMainTemlpate'), [{}]);
      placeHolder$.append(content$);
      initializeDashboards();
    });
  }

  function GetReportViewerConfig() {
    var requestString = 'wscmd=reportviewerconfig';
    AjaxRequest('./rs.aspx', requestString, GotReportViewerConfig, null, 'reportviewerconfig');
  }

  function GotReportViewerConfig(returnObj, id) {
    if (id != 'reportviewerconfig' || returnObj == undefined || returnObj == null)
      return;
    nrvConfig = returnObj;
    nrvConfig.serverDelimiter = '?';
    if (nrvConfig.ResponseServerUrl.indexOf('?') >= 0)
      nrvConfig.serverDelimiter = '&';
    if (document.getElementById('rlhref') != null)
      document.getElementById('rlhref').href = nrvConfig.ReportListUrl;
    urlSettings = new UrlSettings(nrvConfig.ResponseServerUrl);
    var delimiter = '';
    if (urlSettings.urlRsPage.lastIndexOf(nrvConfig.serverDelimiter) != urlSettings.urlRsPage.length - 1)
      delimiter = nrvConfig.serverDelimiter;
    responseServer = new AdHoc.ResponseServer(urlSettings.urlRsPage + delimiter, 0);
    responseServerWithDelimeter = responseServer.ResponseServerUrl;
  }


  jq$(document).ready(function () {
    GetReportViewerConfig();
  });
</script>