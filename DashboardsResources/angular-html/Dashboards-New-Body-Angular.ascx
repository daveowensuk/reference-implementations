<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Dashboards-New-Body-Angular.ascx.cs" Inherits="Resources_html_Dashboard_New_Body" %>

<div ng-app="izendaDashboard">
	<!-- IzendaToolbarController template: Dropdown list with dashboard links template -->
	<script type='text/ng-template' id="toolbarDropdownTemplate"> 
		<li ng-repeat="category in dashboardCategories">
			<div class="iz-dash-menu-catergory">{{category.name}}</div>
			<div class="iz-dash-navigation-menu-item" 
				ng-repeat="dashboard in category.dashboards">
				<a href="#{{dashboard}}">{{dashboard}}</a>
			</div>
		</li>
	</script>
	
	<!-- IzendaTileController template: Tile title template -->
	<script type="text/ng-template" id="tileTitleTemplate">
		<span class="title-text">
			<a ng-if="reportCategory != null" title="{{reportCategory}}\{{reportName}}\{{reportPartName}}" 
				href="{{options.urlSettings.urlReportList}}#{{reportCategory}}">{{reportCategory}}
			</a><span ng-if="reportCategory != null">/</span>
			<a class="db-title-repname" title="{{reportCategory}}\{{reportName}}\{{reportPartName}}" 
				href="{{options.urlSettings.urlReportViewer}}?rn={{reportNameWithCategory}}">{{reportName}}
			</a> /
			<a class="db-title-repname" title="{{reportCategory}}\{{reportName}}\{{reportPartName}}" 
				href="{{options.urlSettings.urlReportViewer}}?rn={{reportNameWithCategory}}">{{reportPartName}}
			</a>
		</span>
	</script>

	<!-- dashboard toolbar -->
	<header ng-controller="IzendaToolbarController" ng-cloak>
		<nav class="navbar navbar-default" role="navigation">
			<div class="container-fluid">
				<!-- navbar header (visible: xs, sm) -->
				<div class="navbar-header hidden-sm hidden-md hidden-lg">
					<a class="pull-right hue-rotate-btn" title="Toggle background hue rotate" style="margin: 10px;"
						ng-hide="!isToggleHueRotateEnabled();" ng-click="toggleHueRotateHandler();">
						<img class="icon" src="Resources/images/hue-rotate-inactive.png" style="width: 16px; height: 16px;" alt="Hue rotate" />
					</a>
					<ul class="pull-right" style="margin: 10px; margin-top: 14px;">
						<li class="dropdown">
							<a class="dropdown-toggle" data-toggle="dropdown" title="Open dashboard">
								<span class="glyphicon glyphicon-folder-open">
									<b class="caret"></b>
								</span></a>
							<ul class="dropdown-menu pull-right" role="menu">
								<ng-include src="'toolbarDropdownTemplate'"></ng-include>
							</ul>
						</li>
					</ul>
					<div class="navbar-brand">{{$izendaUrl.getReportInfo().name}}</div>
				</div>

				<!-- (hidden: xs, sm) -->
				<div id="izendaDashboardToolbar" class="collapse navbar-collapse hidden-xs hidden-sm">
					<!-- button bar -->
					<ul id="izendaDashboardButtonsPanel" class="nav navbar-nav">
						<!-- create new -->
						<li><a id="izendaDashboardCreateDash" title="Create New Dashboard"
							ng-click="createNewDashboardHandler()">
							<span class="glyphicon glyphicon-plus"></span>
						</a></li>
						<!-- refresh -->
						<li><a id="izendaDashboardRefreshDash" title="Refresh Dashboard"
							ng-click="refreshDashboardHandler()">
							<span class="glyphicon glyphicon-refresh"></span>
						</a></li>
						<!-- save -->
						<li class="dropdown">
							<a class="dropdown-toggle" data-toggle="dropdown" title="Save Dashboard">
								<span class="glyphicon glyphicon-floppy-disk"><b class="caret"></b></span>
							</a>
							<ul class="dropdown-menu">
								<li class="iz-dash-menu-item">
									<a id="izendaDashboardSaveDash" title="Save Dashboard"
										ng-click="saveDashboardHandler(false)">
										<span class="glyphicon glyphicon-floppy-disk"></span>Save Dashboard
									</a>
								</li>
								<li class="iz-dash-menu-item"><a id="izendaDashboardSaveDashAs" title="Save Dashboard As"
									ng-click="saveDashboardHandler(true)">
									<span class="glyphicon glyphicon-floppy-disk"></span>Save Dashboard As
								</a></li>
							</ul>
						</li>
						<li><a class="hue-rotate-btn" title="Toggle background hue rotate" style="margin-top: -2px;"
							ng-hide="!isToggleHueRotateEnabled();" ng-click="toggleHueRotateHandler();">
							<img class="icon" src="Resources/images/hue-rotate-inactive.png" style="width: 16px; height: 16px;" alt="Hue rotate" />
						</a></li>
					</ul>
					<!-- navbar "folder" dropdown -->
					<ul class="nav navbar-nav navbar-right"
						ng-show="dashboardCategories.length">
						<li class="dropdown">
							<a class="dropdown-toggle" data-toggle="dropdown" title="Open dashboard">
								<span class="glyphicon glyphicon-folder-open">
									<b class="caret"></b>
								</span>
							</a>
							<ul class="dropdown-menu pull-right" role="menu">
								<ng-include src="'toolbarDropdownTemplate'"></ng-include>
							</ul>
						</li>
					</ul>
					<!-- navbar dashboard tabs -->
					<ul id="izendaDashboardLinksPanel" class="nav navbar-nav navbar-right">
						<li class="dropdown" ng-hide="leftDashboards.length == 0">
							<a class="dropdown-toggle" data-toggle="dropdown" title="Show previous dashboards">
								<b style="font-size: 12px;" class="glyphicon glyphicon-chevron-left"></b>
							</a>
							<ul class="dropdown-menu pull-right" role="menu">
								<li class="iz-dash-menu-item"
									ng-repeat="dashboard in leftDashboards">
									<a href="#{{dashboard}}">{{$izendaUrl.extractReportName(dashboard)}}</a>
								</li>
							</ul>
						</li>
						<li class="iz-dash-menu-item"
							ng-class="{active: dashboard == $izendaUrl.getReportInfo().fullName}"
							ng-repeat="dashboard in dashboardsInCurrentCategory">
							<a href="#{{dashboard}}">{{$izendaUrl.extractReportName(dashboard)}}</a>
						</li>
						<li class="dropdown" ng-hide="rightDashboards.length == 0">
							<a class="dropdown-toggle" data-toggle="dropdown" title="Show next dashboards">
								<b style="font-size: 12px;" class="glyphicon glyphicon-chevron-right"></b>
							</a>
							<ul class="dropdown-menu pull-right" role="menu">
								<li class="iz-dash-menu-item"
									ng-repeat="dashboard in rightDashboards">
									<a href="#{{dashboard}}">{{$izendaUrl.extractReportName(dashboard)}}</a>
								</li>
							</ul>
						</li>
					</ul>
				</div>
			</div>
		</nav>
	</header>

	<!-- dashboard body -->
	<div ng-controller="IzendaDashboardController as dash" ng-cloak>
		<div id="dashboardsDiv">
			<div id="dashboardBodyContainer" class="iz-dash-body-container" ng-style="tileContainerStyle">
				<!-- repeat tiles -->
				<div ng-repeat="tile in tiles" class="iz-dash-tile fx-bounce-down fx-speed-200 fx-trigger fx-easing-quint" tileid="{{tile.id}}"
					ng-style="{'top': ($parent.tileHeight * y) + 'px', 'height': ($parent.tileHeight * height) + 'px', 'left': ($parent.tileWidth * x) + 'px', 'width': ($parent.tileWidth * width) + 'px'}" 
					ng-controller="IzendaTileController"
					ng-init="initialize(tile)" ng-hide="isHidden" ng-cloak>

					<div class="animate-flip">
						<div class="flippy-front animated fast">
							<div class="frame">
								<div class="report" name="preview_control_container" ng-hide="$parent.isChangingNow"></div>
							</div>
							<div class="title-container-background glyphicon" title="Tile actions">
								<span class="bar"></span>
								<span class="bar"></span>
								<span class="bar"></span>
							</div>
							<div class="title-container" style="height: 35px; overflow: hidden;">
								<div class="title">
									<ng-include src="'tileTitleTemplate'"></ng-include>
									<span class="title-hide"></span>
									<span class="flip-button flip-trigger-refresh" title="Refresh tile"></span>
									<span class="flip-button flip-trigger-view"></span>
									<span class="flip-button flip-trigger" title="Show tile options" ng-click="flipBack()">
										<span class="bar"></span>
										<span class="bar"></span>
										<span class="bar"></span>
									</span>
									<span class="flip-button flip-trigger-remove"></span>
									<span class="flip-button flip-trigger-confirm-delete">
										<span class="confirm" title="Remove tile"><span class="glyphicon glyphicon-ok-sign"></span>remove tile</span>
										<span class="cancel" title="Undo request"><span class="glyphicon glyphicon-remove-sign"></span>undo request</span>
									</span>
								</div>
							</div>
						</div>
						<div class="flippy-back">
							<div class="frame">
								<div class="container-fluid">
									<div class="row" style="background-color: #0d70cd; overflow: hidden;">
										<div class="btn-group" style="width: 100%;">
											<span class="iz-dash-tile-top-label">Records:
												<span class="iz-dash-tile-top-slider-value">41</span>
											</span>
										</div>
									</div>
									<div class="row" style="background-color: #0d70cd; text-align: center; overflow: hidden;">
										<input type="text" class="slider" value="41" data-slider-min="1" data-slider-max="101"
											data-slider-step="1" data-slider-value="41" data-slider-orientation="horizontal"
											data-slider-selection="after" data-slider-tooltip="hide" style="width: 95%;" />
									</div>
									<div class="row" style="text-align: center;">
										<div class="btn-group iz-dash-tile-btn-container">
											<a class="btn btn-default iz-dash-tile-btn dd-tile-button-refresh"
												title="Refresh report" ng-click="flipFront(true, true)">
												<span class="glyphicon glyphicon-refresh"></span>
											</a>
										</div>
										<div class="btn-group iz-dash-tile-btn-container">
											<a class="btn btn-default iz-dash-tile-btn dd-tile-button-select-part"
												title="Select report part">
												<span class="glyphicon glyphicon-plus"></span>
											</a>
										</div>
										<div class="btn-group iz-dash-tile-btn-container">
											<a href="{{options.urlSettings.urlReportViewer}}?rn={{tile.reportFullName}}"
												title="Open report in viewer" class="btn btn-default iz-dash-tile-btn dd-tile-button-search">
												<span class="glyphicon glyphicon-search"></span>
											</a>
										</div>
										<div class="btn-group iz-dash-tile-btn-container">
											<a href="{{options.urlSettings.urlReportDesigner}}?rn{{tile.reportFullName}}"
												title="Open report in designer" class="btn btn-default iz-dash-tile-btn dd-tile-button-options">
												<span class="glyphicon glyphicon-pencil"></span>
											</a>
										</div>
										<div class="btn-group iz-dash-tile-btn-container">
											<a target="_blank" href="{{options.urlSettings.urlRsPage}}?rn={{tile.reportFullName}}&p=htmlreport&print=1"
												title="Print report" class="btn btn-default iz-dash-tile-btn dd-tile-button-export-html">
												<span class="glyphicon glyphicon-print"></span>
											</a>
										</div>
										<div class="btn-group iz-dash-tile-btn-container">
											<a href="{{options.urlSettings.urlRsPage}}?rn={{tile.reportFullName}}&output=pdf"
												title="Export to PDF" class="btn btn-default iz-dash-tile-btn dd-tile-button-export-pdf">
												<img class="img-responsive" src="DashboardsResources/images/pdf-big.png" alt="icon">
											</a>
										</div>
										<div class="btn-group iz-dash-tile-btn-container">
											<a href="{{options.urlSettings.urlRsPage}}?rn={{tile.reportFullName}}&output=xls"
												title="Export to Excel" class="btn btn-default iz-dash-tile-btn dd-tile-button-export-xls">
												<img class="img-responsive" src="DashboardsResources/images/xls-big.png" alt="icon">
											</a>
										</div>
									</div>
								</div>
							</div>
							<div class="title-container-background glyphicon" title="Tile actions">
								<span class="bar"></span>
								<span class="bar"></span>
								<span class="bar"></span>
							</div>
							<div class="title-container" style="height: 35px; overflow: hidden;">
								<div class="title">
									<ng-include src="'tileTitleTemplate'"></ng-include>
									<span class="title-hide"></span>
									<span class="flip-button flip-trigger-refresh" title="Refresh tile"></span>
									<span class="flip-button flip-trigger-view"></span>
									<span class="flip-button flip-trigger" title="Show tile options" ng-click="flipFront()"></span>
									<span class="flip-button flip-trigger-remove"></span>
									<span class="flip-button flip-trigger-confirm-delete">
										<span class="confirm" title="Remove tile"><span class="glyphicon glyphicon-ok-sign"></span>remove tile</span>
										<span class="cancel" title="Undo request"><span class="glyphicon glyphicon-remove-sign"></span>undo request</span>
									</span>
								</div>
							</div>
						</div>
					</div>

				</div>
			</div>
		</div>
	</div>
</div>

<script type="text/javascript" src="DashboardsResources/angular-js/legacy/bootstrap-slider.min.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/legacy/perfect-scrollbar.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/legacy/izenda-trace.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/legacy/izenda-query.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/legacy/izenda-filters.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/legacy/izenda-dashboards.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/modules/module-definition.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/services/url-service.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/services/rs-query-service.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/services/common-query-service.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/services/toolbar-query-service.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/services/dashboard-query-service.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/dashboard-app.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/controllers/toolbar-controller.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/controllers/tile-controller.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/controllers/dashboard-controller.js"></script>

<script>
	// Hue rotate function
	var urlSettings = new UrlSettings();
	var oldMouseX = 0;
	var oldMouseY = 0;
	var degree = 0;
	var hueRotateTimeOut = null;
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
</script>
