<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Dashboards-New-Body.ascx.cs" Inherits="Resources_html_Dashboard_New_Body" %>
<!-- Dashboard toolbar template -->
<script id="toolbarTemplate" type="text/x-jsrender">
	<nav class="navbar navbar-default" role="navigation">
		<div class="container-fluid">
			<!-- Brand and toggle get grouped for better mobile display -->
			<div class="navbar-header">
				<a class="pull-right hue-rotate-btn hidden-sm hidden-md hidden-lg" title="Toggle background hue rotate" href="#" style="margin: 10px;">
					<img class="icon" src="Resources/images/hue-rotate-inactive.png" style="width: 16px; height: 16px;" alt="Hue rotate" />
				</a>
				<ul class="pull-right hidden-sm hidden-md hidden-lg" style="margin: 10px; margin-top: 14px;">
					<li class="dropdown">
						<a href="#" class="dropdown-toggle" data-toggle="dropdown" title="Open dashboard">
							<span class="glyphicon glyphicon-folder-open">
								<b class="caret"></b>
							</span></a>
						<ul class="dashboard-links-btn dropdown-menu pull-right" role="menu"></ul>
					</li>
				</ul>
				<div class="navbar-brand hidden-sm hidden-md hidden-lg">{{>dashboardOptions.urlSettings.reportInfo.fullName}}</div>
			</div>

			<!-- Collect the nav links, forms, and other content for toggling -->
			<div id="izendaDashboardToolbar" class="collapse navbar-collapse">
				<ul id="izendaDashboardButtonsPanel" class="nav navbar-nav">
					<li>
						<a id="izendaDashboardCreateDash" href="#" title="Create New Dashboard">
							<span class="glyphicon glyphicon-plus"></span></a>
					</li>
					<li>
						<a id="izendaDashboardRefreshDash" href="#" title="Refresh Dashboard">
							<span class="glyphicon glyphicon-refresh"></span></a>
					</li>
					<li class="dropdown">
						<a href="#" class="dropdown-toggle" data-toggle="dropdown" title="Save Dashboard">
							<span class="glyphicon glyphicon-floppy-disk">
								<b class="caret"></b>
							</span>
						</a>
						<ul class="dropdown-menu">
							<li class="iz-dash-menu-item">
								<a id="izendaDashboardSaveDash" href="#" title="Save Dashboard">
									<span class="glyphicon glyphicon-floppy-disk"></span>
									Save Dashboard
								</a></li>
							<li class="iz-dash-menu-item">
								<a id="izendaDashboardSaveDashAs" href="#" title="Save Dashboard As">
									<span class="glyphicon glyphicon-floppy-disk"></span>
									Save Dashboard As
								</a></li>
						</ul>
					</li>
					<li>
						<a class="hue-rotate-btn" title="Toggle background hue rotate" href="#" style="margin-top: -2px;">
							<img class="icon" src="Resources/images/hue-rotate-inactive.png" style="width: 16px; height: 16px;" alt="Hue rotate" />
						</a>
					</li>
				</ul>
				<ul id="izendaDashboardDropdownPanel" class="nav navbar-nav navbar-right">
					<li class="dropdown">
						<a href="#" class="dropdown-toggle" data-toggle="dropdown" title="Open dashboard">
							<span class="glyphicon glyphicon-folder-open">
								<b class="caret"></b>
							</span></a>
						<ul class="dashboard-links-btn dropdown-menu pull-right" role="menu"></ul>
					</li>
				</ul>
				<ul id="izendaDashboardLinksPanel" class="nav navbar-nav navbar-right hidden-xs hidden-sm">
				</ul>
			</div>
			<!-- /.navbar-collapse -->
		</div>
		<!-- /.container-fluid -->
	</nav>
</script>

<!-- Tile editor modal template -->
<script id="modalEditorTemplate" type="text/x-jsrender">
	<div id="tileEditorModal" class="modal">
		<div class="modal-dialog modal-lg">
			<div class="modal-content">
				<div class="modal-header">
					<h4>TileEditor</h4>
				</div>
				<div class="modal-body">
					<div class="tile-grid">
						<div class="tile-items"></div>
					</div>
				</div>
				<div class="modal-footer">
					<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
				</div>
			</div>
		</div>
	</div>
</script>


<!-- Modal Confirm template -->
<script id="modalConfirmTemplate" type="text/x-jsrender">
	<div id="izDashConfirmModal" class="modal">
		<div class="modal-dialog">
			<div class="modal-content">
				<div class="modal-header"></div>
				<div class="modal-body"></div>
				<div class="modal-footer">
					<button id="izDashConfirmModalOk" type="button" class="btn btn-primary" data-dismiss="modal">OK</button>
					<button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
				</div>
			</div>
		</div>
	</div>
</script>

<!-- Modal template -->
<script id="modalTemplate" type="text/x-jsrender">
	<div id="selectPartModal" class="modal">
		<div class="modal-dialog modal-lg">
			<div class="modal-content">
				<div class="modal-header">
					<div class="row">
						<div class="col-md-2">
							<h4 style="margin-top: 5px;" class="pull-right">Category:</h4>
						</div>
						<div class="col-md-8">
							<select id="reportListCategorySelector" class="form-control"></select>
						</div>
						<div class="col-md-2">
							<button type="button" class="close" style="margin-left: 50px;" data-dismiss="modal" aria-hidden="true">&times;</button>
						</div>
					</div>
				</div>
				<div class="modal-body"></div>
				<div class="modal-footer">
					<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
				</div>
			</div>
		</div>
	</div>
</script>

<!-- Modal item template -->
<script id="modalItemTemplate" type="text/x-jsrender">
	<div class="col-md-3">
		<div class="thumb">
			<div class="thumb-container" style="background-color: white; width: 170px; height: 220px;">
				<img src="{{>ImgUrl}}">
			</div>
			<div class="thumb-title">{{>Name}}</div>
		</div>
	</div>
</script>

<!-- filters -->
<script id="filterTemplate" type="text/x-jsrender">
	<div class="iz-dash-filters">
		<div class="iz-dash-filter-header">
			<h4><a class="iz-dash-toggle-filter" href="#">Dashboard Filters <span class="glyphicon glyphicon-chevron-up"></span></a></h4>
		</div>
		<div>
			<div class="container-fluid iz-dash-filter-items">
				<div class="row"></div>
			</div>
		</div>
		<div>
			<button id="dashboardUpdateFiltersBtn" class="btn btn-primary">Update results</button>
		</div>
	</div>
</script>

<!-- add button template -->
<script id="tileAddButtonTemplate" type="text/x-jsrender">
	<div class="iz-dash-select-report-front-container">
		<button type="button" class="iz-dash-select-report-front-btn btn" title="Select Report">
			<span class="glyphicon glyphicon-plus"></span>
		</button>
	</div>
</script>

<!-- loading splash template -->
<script id="loadingSplashTemplate" type="text/x-jsrender">
	<div class="iz-dash-tile-vcentered-container">
		<div class="iz-dash-tile-vcentered-item">
			<img class="img-responsive" src="{{>dashboardOptions.urlSettings.urlRsPage}}?image=ModernImages.loading-grid.gif" alt="Loading..." />
		</div>
	</div>
</script>

<!-- Tile title template -->
<script id="tileTitleTemplate" type="text/x-jsrender">
	<span class="title-text">{{if tile.reportCategory}}
    <a title="{{>tile.reportSetName}}\{{>tile.reportPartName}}" href="{{>dashboardOptions.urlSettings.urlReportList}}#Subreports">{{>tile.reportCategory}}</a>
		/
    {{/if}}
    {{if tile.reportName}}
      <a class="db-title-repname" title="{{>tile.reportSetName}}\{{>tile.reportPartName}}" href="{{>dashboardOptions.urlSettings.urlReportViewer}}?rn={{if tile.reportCategory}}{{>tile.reportCategory}}\{{/if}}{{>tile.reportName}}">{{>tile.reportName}}</a>
		{{/if}}
    {{if tile.reportPartName}}
      /
      <a class="db-title-repname" title="{{>tile.reportSetName}}\{{>tile.reportPartName}}" href="{{>dashboardOptions.urlSettings.urlReportViewer}}?rn={{if tile.reportCategory}}{{>tile.reportCategory}}\{{/if}}{{>tile.reportName}}">{{>tile.reportPartName}}</a>
		{{/if}}
	</span>
</script>

<!-- Tile template -->
<script id="tileTemplate" type="text/x-jsrender">
	<div class="iz-dash-tile">
		<div class="animate-flip">
			<div class="flippy-front animated fast flipInY">
				<div class="frame">
					<div class="report" name="preview_control_container">
						{{if tile.isNew}}
							{{include tmpl="#tileAddButtonTemplate"/}}
						{{/if}}
					</div>
				</div>
				<div class="title-container-background glyphicon" title="Tile actions">
					<span class="bar"></span>
					<span class="bar"></span>
					<span class="bar"></span>
				</div>
				<div class="title-container" style="height: 35px; overflow: hidden;">
					<div class="title">
						{{include tmpl="#tileTitleTemplate"/}}
						<span class="title-hide"></span>
						<span class="flip-button flip-trigger-refresh" title="Refresh tile"></span>
						<span class="flip-button flip-trigger-view"></span>
						<span class="flip-button flip-trigger" title="Show tile options">
							<span class="bar"></span>
							<span class="bar"></span>
							<span class="bar"></span>
						</span>
						<span class="flip-button flip-trigger-remove"></span>
						<span class="flip-button flip-trigger-confirm-delete">
							<span class="confirm" title="Remove tile"><span class="glyphicon glyphicon-ok-sign"></span> remove tile</span>
							<span class="cancel" title="Undo request"><span class="glyphicon glyphicon-remove-sign"></span> undo request</span>
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
								<a href="#" class="btn btn-default iz-dash-tile-btn dd-tile-button-refresh"
									title="Refresh report">
									<span class="glyphicon glyphicon-refresh"></span>
								</a>
							</div>
							<div class="btn-group iz-dash-tile-btn-container">
								<a href="#" class="btn btn-default iz-dash-tile-btn dd-tile-button-select-part"
									title="Select report part">
									<span class="glyphicon glyphicon-plus"></span>
								</a>
							</div>
							<div class="btn-group iz-dash-tile-btn-container">
								<a href="{{>dashboardOptions.urlSettings.urlReportViewer}}?rn={{if tile.reportCategory}}{{>tile.reportCategory}}\{{/if}}{{>tile.reportName}}"
									title="Open report in viewer" class="btn btn-default iz-dash-tile-btn dd-tile-button-search">
									<span class="glyphicon glyphicon-search"></span>
								</a>
							</div>
							<div class="btn-group iz-dash-tile-btn-container">
								<a href="{{>dashboardOptions.urlSettings.urlReportDesigner}}?rn={{if tile.reportCategory}}{{>tile.reportCategory}}\{{/if}}{{>tile.reportName}}"
									title="Open report in designer" class="btn btn-default iz-dash-tile-btn dd-tile-button-options">
									<span class="glyphicon glyphicon-pencil"></span>
								</a>
							</div>
							<div class="btn-group iz-dash-tile-btn-container">
								<a target="_blank" href="{{>dashboardOptions.urlSettings.urlRsPage}}?rn={{if tile.reportCategory}}{{>tile.reportCategory}}\{{/if}}{{>tile.reportName}}&p=htmlreport&print=1"
									title="Print report" class="btn btn-default iz-dash-tile-btn dd-tile-button-export-html">
									<span class="glyphicon glyphicon-print"></span>
								</a>
							</div>
							<div class="btn-group iz-dash-tile-btn-container">
								<a href="{{>dashboardOptions.urlSettings.urlRsPage}}?rn={{if tile.reportCategory}}{{>tile.reportCategory}}\{{/if}}{{>tile.reportName}}&output=pdf"
									title="Export to PDF" class="btn btn-default iz-dash-tile-btn dd-tile-button-export-pdf">
									<img class="img-responsive" src="DashboardsResources/images/pdf-big.png" alt="icon">
								</a>
							</div>
							<div class="btn-group iz-dash-tile-btn-container">
								<a href="{{>dashboardOptions.urlSettings.urlRsPage}}?rn={{if tile.reportCategory}}{{>tile.reportCategory}}\{{/if}}{{>tile.reportName}}&output=xls"
									title="Export to Excel" class="btn btn-default iz-dash-tile-btn dd-tile-button-export-xls">
									<img class="img-responsive" src="DashboardsResources/images/xls-big.png" alt="icon">
								</a>
							</div>
							<%--<div class="btn-group iz-dash-tile-btn-container">
								<a href="#" title="Remove tile from dashboard" class="btn btn-default iz-dash-tile-btn dd-tile-button-remove">
									<span class="glyphicon glyphicon-remove"></span>
								</a>
							</div>--%>
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
						{{include tmpl="#tileTitleTemplate"/}}
						<span class="title-hide"></span>
						<span class="flip-button flip-trigger-refresh" title="Refresh tile"></span>
						<span class="flip-button flip-trigger-view"></span>
						<span class="flip-button flip-trigger" title="Show tile options"></span>
						<span class="flip-button flip-trigger-remove"></span>
						<span class="flip-button flip-trigger-confirm-delete">
							<span class="confirm" title="Remove tile"><span class="glyphicon glyphicon-ok-sign"></span> remove tile</span>
							<span class="cancel" title="Undo request"><span class="glyphicon glyphicon-remove-sign"></span> undo request</span>
						</span>
					</div>
				</div>
			</div>
		</div>
	</div>
</script>

<%-- Modals --%>
<div class="modal fade" id="izendaDashboardNameModal" tabindex="-1" role="dialog" aria-hidden="true">
	<div class="modal-dialog">
		<div class="modal-content">
			<div class="modal-body">
				<form class="form-horizontal" role="form">
					<div class="form-group">
						<label for="izendaDashboardNameModalName" class="col-sm-2 control-label">Name</label>
						<div class="col-sm-10">
							<input type="email" class="form-control" id="izendaDashboardNameModalName" placeholder="Dashboard Name">
						</div>
					</div>
					<div class="form-group">
						<label for="izendaDashboardNameModalCategory" class="col-sm-2 control-label">Category</label>
						<div class="col-sm-10 hidden">
							<input id="izendaDashboardNameModalCategoryName" type="text" class="form-control" placeholder="Category Name" />
						</div>
						<div class="col-sm-10">
							<select id="izendaDashboardNameModalCategory" class="form-control"></select>
						</div>
					</div>
				</form>
			</div>
			<div class="modal-footer">
				<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
				<button id="izendaDashboardNameModalOk" type="button" class="btn btn-primary disabled">OK</button>
			</div>
		</div>
	</div>
</div>

<%--Dashboard main area--%>
<div id="dashboardsDiv"></div>

<%--Preloader run--%>
<script type="text/javascript">
	jq$(document).ready(function ($) {
		$('body').prepend($('<div class="iz-dash-background"></div>'));
		// preloader completed:
		dashboard = $('#dashboardsDiv').izendaDashboard({
			urlSettings: new UrlSettings()
		});
	});
</script>
