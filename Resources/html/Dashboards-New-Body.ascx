<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Dashboards-New-Body.ascx.cs" Inherits="Resources_html_Dashboard_New_Body" %>
<div style="padding: 20px;">
  <div class="row">
    <div class="col-md-12" style="height: 500px;">
      <textarea id="dashboardConfigText" style="width: 100%; height: 400px;">
  {"tiles": [{
    "x": 0,
    "y": 0,
    "width": 12
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
    "x": 1,
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
    "width": 2
  }, {
    "x": 0,
    "y": 10,
    "width": 2
  }]}
      </textarea>
      <a id="dashboardConfigTextBtn" class="btn btn-primary" href="#">Apply config</a>
    </div>
    <div class="row">
      <div class="col-md-12">
        <div id="dashboardsDiv"></div>
      </div>
    </div>
  </div>
</div>

<script type="text/javascript">
  $(document).ready(function() {
    var configText = $('#dashboardConfigText').val();
    var config = window.JSON.parse(configText);
    
    var dashboard = $('#dashboardsDiv').izendaDashboard({
      dashboardLayout: config
    });

    $('#dashboardConfigTextBtn').click(function() {
      configText = $('#dashboardConfigText').val();
      config = window.JSON.parse(configText);
      dashboard.setConfig({
        dashboardLayout: config
      });
    });
  });
</script>
