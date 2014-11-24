<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Dashboards-New-Head-Angular.ascx.cs" Inherits="Resources_html_Dashboard_New_Head" %>
<title>Dashboards</title>

<link rel="stylesheet" type="text/css" href="DashboardsResources/css/jquery.minicolors.css"/>
<link rel="stylesheet" type="text/css" href="DashboardsResources/css/bootstrap-slider.min.css"/>
<link rel="stylesheet" type="text/css" href="DashboardsResources/css/perfect-scrollbar.css"/>
<link rel="stylesheet" type="text/css" href="DashboardsResources/css/dashboard2.css"/>
<link rel="stylesheet" type="text/css" href="DashboardsResources/css/animate.css"/>

<script type="text/javascript" src="./rs.aspx?js=ModernScripts.modernizr-2.8.3.min"></script>
<script type="text/javascript" src="./rs.aspx?js=jQuery.jq"></script>

<script type="text/javascript">
	if (window.jQuery)
		window.jQueryTemp = window.jQuery;
	window.jQuery = jq$;
</script>
<% #if DEBUG %>
<script type="text/javascript" src="DashboardsResources/angular-js/vendor/angular.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/vendor/angular-route.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/vendor/angular-animate.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/vendor/angular-cookies.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/vendor/ngFx.js"></script>
<% #else %>
<script type="text/javascript" src="DashboardsResources/angular-js/vendor/angular.min.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/vendor/angular-route.min.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/vendor/angular-animate.min.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/vendor/angular-cookies.min.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/vendor/ngFx.min.js"></script>
<% #endif %>
<script type="text/javascript">
	if (window.jQueryTemp)
		window.jQuery = window.jQueryTemp;
</script>

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

<script type="text/javascript" src="DashboardsResources/angular-js/vendor/jquery.minicolors.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/vendor/bootstrap-slider.min.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/vendor/perfect-scrollbar.js"></script>