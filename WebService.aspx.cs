using System;
using Izenda.AdHoc;
using Izenda.Fusion;

public partial class WebService : System.Web.UI.Page {
  protected void Page_Load(object sender, EventArgs e) {
    string connectionString = @"INSERT_CONNECTION_STRING_HERE";
    AdHocSettings.LicenseKey = "INSERT_LICENSE_KEY_HERE";
    AdHocSettings.SqlServerConnectionString = connectionString;
    SqlDataExtractor extractor = new SqlDataExtractor(connectionString);
    FusionEndPoint.ProcessRequest(Request, Response, extractor);
  }
}
