<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Dashboards-New-Body.ascx.cs" Inherits="Resources_html_Dashboard_New_Body" %>
<script id="tileTemplate" type="text/x-jsrender">
  <div class="animate-flip">
			<div class="flippy-front">
				<div class="title-container">
					<div class="title">
						<span class="title-text">header text</span>
						<span class="title-hide"></span>
						<span class="flip-trigger-refresh"></span>
						<span class="flip-trigger-remove"></span>
						<span class="flip-trigger-view"></span>
						<span class="flip-trigger"></span>
					</div>
				</div>
				<div class="frame">
					<div class="report">
						report
					</div>
				</div>
			</div>
			<div class="flippy-back">
				<div class="frame">
					<div class="menu-small" style="text-align: left;">
						<div style="white-space: nowrap; margin: 2px 40px;">
							Records count
								<input type="text" class="dd-tile-input-record-count" />
							<select class="dd-tile-select-record-count">
								<option value="-999">All (no scrollbar)</option>
								<option value="-1">All</option>
								<option value="1">1</option>
								<option value="10">10</option>
								<option value="20">20</option>
								<option value="30">30</option>
								<option value="40">40</option>
								<option value="50">50</option>
								<option value="60">60</option>
								<option value="70">70</option>
								<option value="80">80</option>
								<option value="90">90</option>
								<option value="100" selected="selected">100</option>
							</select>
						</div>
						<hr />
						<a href="#selectPart" class="dd-tile-button-select-part">
							<img src="DashboardsResources/images/add-new.png" alt="Select report part" />
							<span class="first">Select report part</span>
						</a>
						<a href="#viewPart" class="dd-tile-button-search">
							<img src="DashboardsResources/images/view-18.png" alt="icon">
							<span class="first">View this tile's report</span>
						</a>
						<a href="#2" class="dd-tile-button-options">
							<img src="DashboardsResources/images/edit-18.png" alt="icon">
							<span class="first">Edit in designer</span>
						</a>
						<hr />
						<a href="#3" class="dd-tile-button-export-html">
							<img src="DashboardsResources/images/print-18.png" alt="icon">
							<span class="first">Print Tile's Report as HTML</span>
						</a>
						<a href="#4" class="dd-tile-button-export-pdf">
							<img src="DashboardsResources/images/pdf-18.png" alt="icon">
							<span class="first">Print Tile's Report as PDF</span>
						</a>
						<a href="#5" class="dd-tile-button-export-xls">
							<img src="DashboardsResources/images/xls-18.png" alt="icon">
							<span class="first">Print Tile's Report as Excel</span>
						</a>
						<a href="#6" class="dd-tile-button-remove">
							<img src="DashboardsResources/images/remove-18.png" alt="icon">
							<span class="first">Hide from Dashboard</span>
						</a>
					</div>

				</div>
			</div>
		</div>
</script>

<%--Dashboard main area--%>
<div style="padding: 20px;">
  <div class="row">
    <div class="col-md-12">
      <div id="dashboardsDiv" style="background-color: powderblue"></div>
    </div>
  </div>
  <div class="row">
    <div class="col-md-12" style="height: 500px;">
      <textarea id="dashboardConfigText" style="width: 100%; height: 400px;">
  {"tiles": [{
    "x": 6,
    "y": 0,
    "width": 6
  }, {
    "x": 0,
    "y": 1,
    "width": 6
  }, {
    "x": 6,
    "y": 1,
    "height": 2,
    "width": 6
  }, {
    "x": 0,
    "y": 2,
    "height": 2,
    "width": 6
  }, {
    "x": 6,
    "y": 3,
    "height": 2,
    "width": 6
  }, {
    "x": 0,
    "y": 4,
    "width": 6
  }, {
    "x": 0,
    "y": 5
  }, {
    "x": 6,
    "y": 5,
    "height": 3
  }, {
    "x": 0,
    "y": 6
  }, {
    "x": 0,
    "y": 7
  }, {
    "x": 0,
    "y": 8,
    "width": 6
  }, {
    "x": 2,
    "y": 14,
    "width": 2
  }, {
    "x": 4,
    "y": 14,
    "width": 2
  }]}
      

</textarea>
      <a id="dashboardConfigTextBtn" class="btn btn-primary" href="#">Apply config</a>
    </div>
  </div>
</div>

<%--Preloader run--%>
<script type="text/javascript">
  /**
   * Overriden function:
   * This function defined at master page and runs after preload complete.
   */
  izendaPreloadCompleted = function() {
    // preloader completed:
    var configText = $('#dashboardConfigText').val();
    var config = window.JSON.parse(configText);
    var dashboard = $('#dashboardsDiv').izendaDashboard({
      dashboardLayout: config,
      onGridInitialized: function() {
        var tiles = $('#dashboardsDiv .dd-tile-placeholder');
        $.each(tiles, function (iTile, tile) {
          var $tile = $(tile);
          $tile.html($("#tileTemplate").render());
        });
      }
    });
    $('#dashboardConfigTextBtn').click(function () {
      configText = $('#dashboardConfigText').val();
      config = window.JSON.parse(configText);
      dashboard.setConfig({
        dashboardLayout: config
      });
    });
  };

</script>
