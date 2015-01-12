using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Izenda.AdHoc;

public partial class DashboardsNew : Page
{
	protected override void OnPreInit(System.EventArgs e)
	{
		ASP.global_asax.CustomAdHocConfig.InitializeReporting();
	}

	protected override void OnLoad(EventArgs e)
	{
		string isNewParam = Request.Params["isNew"];
		string rnParam = Request.Params["rn"];
		bool isNew = !string.IsNullOrEmpty(isNewParam) && isNewParam == "1";
		if (!isNew) {
			if (string.IsNullOrEmpty(rnParam)) {
				ReportInfo[] reports = AdHocSettings.AdHocConfig.FilteredListReports();
				foreach (ReportInfo reportInfo in reports) {
					if (reportInfo.Dashboard) {
						Response.Redirect("Dashboards.aspx?rn=" + reportInfo.FullName);
						return;
						break;
					}
				}
			} else {
				AdHocContext.CurrentReportSet = AdHocSettings.AdHocConfig.LoadFilteredReportSet(rnParam);
			}
		} else {
			// new report
			AdHocContext.CurrentReportSet = new ReportSet();
			AdHocContext.CurrentReportSet.ReportSetType = ReportSetType.DashboardDesigner2;
			string name = string.IsNullOrEmpty(rnParam) ? "" : rnParam;
			if (name.Contains("\\")) {
				string[] nameParts = name.Split('\\');
				AdHocContext.CurrentReportSet.ReportName = nameParts[1];
				AdHocContext.CurrentReportSet.ReportCategory = nameParts[0];
			} else {
				AdHocContext.CurrentReportSet.ReportName = name;
			}
		}
	}
}