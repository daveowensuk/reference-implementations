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
			<a ng-if="title != null && title != ''" class="db-title-repname" title="{{reportCategory}}\{{reportName}}\{{reportPartName}}" 
				href="{{options.urlSettings.urlReportViewer}}?rn={{reportNameWithCategory}}">{{title}}
			</a>
			<a ng-if="(title == null || title == '') && reportCategory != null" title="{{reportCategory}}\{{reportName}}\{{reportPartName}}" 
				href="{{options.urlSettings.urlReportList}}#{{reportCategory}}">{{reportCategory}}
			</a>
			<span ng-if="(title == null || title == '') && title != '' && reportCategory != null">/</span>
			<a ng-if="(title == null || title == '')" class="db-title-repname" title="{{reportCategory}}\{{reportName}}\{{reportPartName}}" 
				href="{{options.urlSettings.urlReportViewer}}?rn={{reportNameWithCategory}}">{{reportName}}
			</a>
			<span ng-if="(title == null || title == '') && reportPartName != null">/</span>
			<a ng-if="(title == null || title == '')" class="db-title-repname" title="{{reportCategory}}\{{reportName}}\{{reportPartName}}" 
				href="{{options.urlSettings.urlReportViewer}}?rn={{reportNameWithCategory}}">{{reportPartName}}
			</a>
		</span>
	</script>
	
	<!-- select report name modal dialog -->
	<div id="izendaSelectReportNameModal" class="modal" tabindex="-1" role="dialog" aria-hidden="true"
		ng-controller="IzendaSelectReportNameController">
		<div class="modal-dialog">
			<div class="modal-content">
				<div class="modal-body">
					<form class="form-horizontal" role="form">
						<div class="form-group">
							<div class="alert alert-danger" role="alert" ng-hide="errorMessages.length === 0">
								<div ng-repeat="errorMessage in errorMessages">
									{{errorMessage}}
								</div>
							</div>
						</div>
						<div class="form-group">
							<label for="izendaDashboardNameModalName" class="col-sm-2 control-label">Name</label>
							<div class="col-sm-10">
								<input type="text" class="form-control" id="izendaDashboardNameModalName"
									ng-model="reportName" placeholder="Report Name">
							</div>
						</div>
						<div class="form-group">
							<label for="izendaDashboardNameModalCategory" class="col-sm-2 control-label">Category</label>
							<div class="col-sm-10" ng-hide="!isCreatingNewCategory">
								<input id="izendaDashboardNameModalCategoryName" type="text" class="form-control" placeholder="Category Name" 
									ng-model="newCategoryName"/>
							</div>
							<div class="col-sm-10" ng-hide="isCreatingNewCategory">
								<select id="izendaDashboardNameModalCategory" class="form-control"
									ng-options="category.id as category.name for category in categories"
									ng-model="selectedCategoryId"
									ng-init="selectedCategoryId = 1"
									ng-change="categorySelectedHandler()"></select>
							</div>
						</div>
					</form>
				</div>
				<div class="modal-footer">
					<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
					<button id="izendaDashboardNameModalOk" type="button" class="btn btn-primary" 
						ng-click="completeHandler()">OK</button>
				</div>
			</div>
		</div>
	</div>

	<!-- select report part modal dialog -->
	<div id="izendaSelectPartModal" class="modal" tabindex="-1" role="dialog" aria-hidden="true" ng-controller="IzendaSelectReportController">
		<div class="modal-dialog modal-lg">
			<div class="modal-content">
				<div class="modal-header">
					<div class="row">
						<div class="col-md-2">
							<h4 style="margin-top: 5px;" class="pull-right">Category:</h4>
						</div>
						<div class="col-md-8">
							<select class="form-control" ng-model="category" 
								ng-options="category for category in categories"
								ng-change="categoryChangedHandler()">
							</select>
						</div>
						<div class="col-md-2">
							<button type="button" class="close" style="margin-left: 50px;" data-dismiss="modal" aria-hidden="true">&times;</button>
						</div>
					</div>
				</div>
				<div class="modal-body" style="min-height: 300px;">
					<div class="iz-dash-tile-vcentered-container" ng-if="noReportsFound">
						<div class="iz-dash-tile-vcentered-item">
							No reports found!
						</div>
					</div>
					<div class="iz-dash-tile-vcentered-container" ng-if="!noReportsFound && groups.length == 0">
						<div class="iz-dash-tile-vcentered-item">
							<img class="img-responsive" ng-src="{{$izendaUrl.urlSettings.urlRsPage}}?image=ModernImages.loading-grid.gif" alt="Loading..." />
						</div>
					</div>
					<div ng-if="!noReportsFound && groups.length > 0">
						<div ng-repeat="group in groups" class="row">
							<div class="col-md-3" ng-repeat="item in group">
								<div class="thumb" ng-click="itemSelectedHandler(item)">
									<div class="thumb-container" style="background-color: white; width: 170px; height: 220px;">
										<img class="img-responsive" ng-src="{{item.ImgUrl}}" />
									</div>
									<div class="thumb-title">{{item.Name}}</div>
								</div>
							</div>
						</div>
					</div>
				</div>
				<div class="modal-footer">
					<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
				</div>
			</div>
		</div>
	</div>

	<!-- dashboard toolbar -->
	<header ng-controller="IzendaToolbarController" ng-cloak>
		<nav class="navbar navbar-default iz-dash-navbar" role="navigation">
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
					<ul ng-class="buttonbarCollapsedClass">
						<li><a class="navbar-toggle" style="border-color: transparent; margin-top: 5px; margin-bottom: 5px;"
								ng-click="showButtonBar()">
							<span class="sr-only">Toggle na	vigation</span>
							<span class="icon-bar"></span>
							<span class="icon-bar"></span>
							<span class="icon-bar"></span>
						  </a></li>
					</ul>
					<ul ng-class="buttonbarClass">
						<!-- create new -->
						<li><a ng-click="hideButtonBar()" title="Hide buttons" style="border-right: 1px solid #ddd;">
							<span class="glyphicon glyphicon-chevron-left"></span>
						</a></li>
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
								<span class="glyphicon glyphicon-floppy-disk">&nbsp;<b class="caret"></b></span>
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
						<li class="dropdown">
							<a class="hue-rotate-btn dropdown-toggle" data-toggle="dropdown"
								title="Toggle background hue rotate" ng-style="backgroundColorStyle" style="padding-top: 2px;">
								<img class="icon" src="DashboardsResources/images/color-bw.png" style="width: 16px; height: 16px; padding-top: 2px;" alt="Hue rotate" />
								&nbsp;<b class="caret"></b>
							</a>
							<div class="dropdown-menu dropdown-no-close-on-click" style="min-width: 200px; text-align: center;">
								<div class="iz-dash-color-preview-container">
									<span ng-style="backgroundColorStyle" class="iz-dash-color-preview">&nbsp;</span>
									<span class="iz-dash-color-preview">{{izendaBackgroundColor}}</span>
								</div>
								<input type="text" id="izendaDashboardColorPicker" class="form-control" data-inline="true" ng-value="izendaBackgroundColor">
								<input id="izendaDashboardBackground" type="file" name="file" style="display: none;" 
									onchange="angular.element(this).scope().backgroundFileChangedHandler(arguments)"/>
								<div style="padding: 5px;" ng-hide="!isToggleHueRotateEnabled()">
									<hr style="margin-top: 5px; margin-bottom: 10px;"/>
									<span class="iz-dash-switcher-label">Color hue rotate</span>
									<span id="izendaDashboardHueRotateSwitcher" class="iz-dash-switcher" ng-click="toggleHueRotateHandler()">
										<span class="iz-dash-switcher-text-off">O</span>
										<span class="iz-dash-switcher-item"></span>
										<span class="iz-dash-switcher-text-on">I</span>
									</span>
								</div>
							</div>
						</li>
					</ul>
					<!-- navbar "folder" dropdown -->
					<ul id="izendaDashboardLinksDropdown" class="nav navbar-nav navbar-right" ng-show="dashboardCategories.length">
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
					<div id="izendaDashboardLinksPanel" style="display: inline-block; margin-left: 50px;">
						<ul class="nav navbar-nav unselectable" style="position: absolute; left: 50px; top: 0;z-index: 1; background-color: #ddd;">
							<li><a title="Show previous dashboards" ng-click="shiftTabs(-1)" ng-hide="hiddenShiftTabs(-1)">
								<b style="font-size: 12px;" class="glyphicon glyphicon-chevron-left"></b>
							</a></li>
						</ul>
						<div style="width: 100%;">
						<ul class="unselectable nav navbar-nav iz-dash-nav-tabs">
						</ul>
						</div>
						</div>
						<ul class="nav navbar-nav unselectable" style="position: absolute; right: 100px; top: 0; z-index: 1; background-color: #ddd;">
							<li><a title="Show next dashboards" ng-click="shiftTabs(1)" ng-hide="hiddenShiftTabs(1)">
								<b style="font-size: 12px;" class="glyphicon glyphicon-chevron-right"></b>
							</a></li>
						</ul>
					</div>
					<%--<ul id="izendaDashboardLinksPanel" class="nav navbar-nav navbar-right">
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
							ng-style="getToolItemStyle(dashboard)"
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
					</ul>--%>
				</div>
			</div>
		</nav>
	</header>

	<!-- dashboard body -->
	<div ng-controller="IzendaDashboardController as dash" ng-cloak>
		<div id="dashboardsDiv">
			<div id="dashboardBodyContainer" class="iz-dash-body-container" ng-style="tileContainerStyle">
				<!-- repeat tiles -->
				<div ng-repeat="tile in tiles" class="iz-dash-tile fx-fade-down fx-speed-500 fx-trigger fx-easing-quint" tileid="{{tile.id}}"
					ng-style="{'top': ($parent.tileHeight * y) + 'px', 'height': ($parent.tileHeight * height) + 'px', 'left': ($parent.tileWidth * x) + 'px', 'width': ($parent.tileWidth * width) + 'px'}" 
					ng-controller="IzendaTileController"
					ng-init="initialize(tile)" ng-hide="isHidden" ng-cloak>

					<div class="animate-flip">
						<div class="flippy-front animated fast">
							<div class="frame">
								<div ng-if="description != null && description != ''" class="iz-dash-tile-description">{{description}}</div>
								<div style="cursor: pointer;" class="iz-dash-select-report-front-container" ng-hide="reportFullName != null" 
									title="Select report part" ng-click="selectReportPart()">
									<a class="btn btn-default iz-dash-select-report-front-btn">
										<span class="glyphicon glyphicon-plus"></span>
									</a>
								</div>
								<div class="report" name="preview_control_container" ng-hide="isReportDivHidden()">
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
									<a title="Show tile options" class="title-button button2" ng-click="flipBack()">
										<span class="bar"></span>
										<span class="bar"></span>
										<span class="bar"></span>
									</a>
									<a title="Delete tile" class="title-button title-button-remove button1" 
												ng-click="showConfirmDelete()">
										<img src="DashboardsResources/images/remove-18.png" class="img-responsive"/>
									</a>
									<a title="Confirm delete" ng-class="deleteConfirmClass + ' ' + getConfirmDeleteClass()"
												ng-click="deleteTile()">
										<img src="DashboardsResources/images/tile/confirm-delete.png"/><span ng-if="width > 1">remove tile</span>
									</a>
									<a title="Cancel delete" ng-class="deleteConfirmClass + ' ' + getCancelDeleteClass()"
												ng-click="hideConfirmDelete()">
										<img src="DashboardsResources/images/tile/turn-back.png"/><span ng-if="width > 1">undo request</span>
									</a>
								</div>
							</div>
						</div>
						<div class="flippy-back">
							<div class="frame">
								<div class="container-fluid iz-dash-tile-fb-main">
									<div class="row">
										<div class="col-md-3 col-xs-12 iz-dash-tile-fb-label">
											<label>Title</label>
										</div>
										<div class="col-md-9 col-xs-12">
											<input type="text" class="form-control" ng-model="title">
										</div>
									</div>
									<div class="row">
										<div class="col-md-3 col-xs-12 iz-dash-tile-fb-label">
											<label>Description</label>
										</div>
										<div class="col-md-9 col-xs-12">
											<input type="text" class="form-control" ng-model="description">
										</div>
									</div>
								</div>
								<div class="iz-dash-tile-fb-toolbar">
									<div class="iz-dash-tile-fb-toolbtn">
										<a title="Print tile report" href="{{options.urlSettings.urlRsPage}}?rn={{getSourceReportName()}}&p=htmlreport&print=1">
											<img class="img-responsive" src="DashboardsResources/images/back-tile/print.png"/>
										</a>
									</div>
									<div class="iz-dash-tile-fb-toolbtn">
										<a title="Export tile report to excel" href="{{options.urlSettings.urlRsPage}}?rn={{getSourceReportName()}}&output=xls">
											<img class="img-responsive" src="DashboardsResources/images/back-tile/excel.png">
										</a>
									</div>
									<div class="iz-dash-tile-fb-toolbtn">
										<a href="{{options.urlSettings.urlReportDesigner}}?rn={{getSourceReportName()}}" title="Open tile report in designer">
											<img class="img-responsive" src="DashboardsResources/images/back-tile/edit.png">
										</a>
									</div>
									<div class="iz-dash-tile-fb-toolbtn" title="Open tile report in viewer">
										<a href="{{options.urlSettings.urlReportViewer}}?rn={{getSourceReportName()}}" title="Open tile report in viewer">
											<img class="img-responsive" src="DashboardsResources/images/back-tile/view.png">
										</a>
									</div>
									<div class="iz-dash-tile-fb-toolbtn">
										<a title="Reload tile from it's source" ng-click="flipFront(true, true)">
											<img class="img-responsive" src="DashboardsResources/images/back-tile/reload.png">
										</a>
									</div>
									<div class="iz-dash-tile-fb-toolbtn">
										<a title="Add new report part to tile" ng-click="selectReportPart()">
											<img class="img-responsive" src="DashboardsResources/images/back-tile/add.png">
										</a>
									</div>

									<span class="iz-dash-tile-fb-rcount">{{getTopString()}}</span>
									<span class="iz-dash-tile-fb-rcount-label">RECORDS COUNT</span>
									<span class="iz-dash-tile-fb-rcount-slider">
										<input type="text" class="slider" value="100" data-slider-min="1" data-slider-max="101"
											data-slider-step="1" data-slider-value="100" data-slider-orientation="horizontal"
											data-slider-selection="after" data-slider-tooltip="hide" style="width: 180px;">
									</span>
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
									<a title="Show tile options" class="title-button title-button-remove button2" ng-click="flipFront(true)">
										<img src="DashboardsResources/images/turn-18.png" class="img-responsive"/>
									</a>
									<a title="Delete tile" class="title-button title-button-remove button1" 
												ng-click="showConfirmDelete()">
										<img src="DashboardsResources/images/remove-18.png" class="img-responsive"/>
									</a>
									<a title="Confirm delete" ng-class="deleteConfirmClass + ' ' + getConfirmDeleteClass()"
												ng-click="deleteTile()">
										<img src="DashboardsResources/images/tile/confirm-delete.png"/><span ng-if="width > 1">remove tile</span>
									</a>
									<a title="Cancel delete" ng-class="deleteConfirmClass + ' ' + getCancelDeleteClass()"
												ng-click="hideConfirmDelete()">
										<img src="DashboardsResources/images/tile/turn-back.png"/><span ng-if="width > 1">undo request</span>
									</a>
								</div>
							</div>
						</div>
					</div>

				</div>
			</div>
		</div>
	</div>
</div>

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
<script type="text/javascript" src="DashboardsResources/angular-js/controllers/select-report-controller.js"></script>
<script type="text/javascript" src="DashboardsResources/angular-js/controllers/select-report-name-controller.js"></script>


<script type="text/javascript">
	jq$('.dropdown-no-close-on-click.dropdown-menu .hue-rotate-switcher').click(function (e) {
		e.stopPropagation();
	});
</script>
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
