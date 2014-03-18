using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml;
using Izenda.AdHoc.Thumbnails;
using Izenda.Controls;
using Izenda.Web.UI;
using A = DocumentFormat.OpenXml.Drawing;
using Ap = DocumentFormat.OpenXml.ExtendedProperties;
using Vt = DocumentFormat.OpenXml.VariantTypes;
using Izenda.AdHoc;
using Image = System.Drawing.Image;

public partial class PowerPointExport : System.Web.UI.Page {
  private class ReportDashboardComparer : IComparer<Report> {
    public int Compare(Report x, Report y) {
      if (x.Position==null && y.Position==null) {
        return 0;
      }
      if (x.Position==null) {
        return -1;
      }
      if (y.Position==null) {
        return 1;
      }
      if (x.Position.Y-y.Position.Y!=0) {
        return x.Position.Y-y.Position.Y;
      }
      return x.Position.X-y.Position.X;
    }
  }

  ///
  /// Chrome rendering (removed)
  ///
  /*static void RenderPage(WebView webView, string html, string path)
  {
  	if (webView != null) {
  		var finishedLoading = false;
  		try {
  			webView.LoadHTML(html);
  			webView.LoadingFrameComplete += (sender, e) => {
  				finishedLoading = true;
  				/*JSObject jsobject = ((WebView)sender).CreateGlobalJavascriptObject("external");
  				jsobject.Bind("CreateScreenshot", false, delegate {
  					finishedLoading = true;
  				});#1#
  			};
  		} catch (Exception e) {
  			Console.WriteLine(e.Message);
  			finishedLoading = true;
  		}
  		// Wait for the page to load.
  		while (!finishedLoading) {
  			Thread.Sleep(100);
  			WebCore.Update();
  		}
  		((BitmapSurface)webView.Surface).SaveToJPEG(path);
  	}
  }*/

  /*
  private string CreateStandaloneHighchartScreenshot(string html, int width, int height)
  {
  	string path = HttpContext.Current.Server.MapPath("~/App_Data/"+Guid.NewGuid()+".jpeg");
  	var done = false;
  	AwesomiumTaskErrorHandler errorHandler = delegate(object sender, ErrorEventArgs args) {
  		done = true;
  	};

  	AwesomiumTaskDoneHandler doneHandler = delegate(object sender, EventArgs args) {
  		done = true;
  	};
  	AwesomiumEnviroment.DoneEvent += doneHandler;
  	AwesomiumEnviroment.ErrorEvent += errorHandler;
  	AwesomiumEnviroment.Run(new Task(delegate {
  		using (var wv = WebCore.CreateWebView(width, height)) {
  			RenderPage(wv, html, path);
  		}
  	}));
  	while (!done) {
  		Thread.Sleep(10);
  	}
  	return path;
  }
  */

  private string RenderReport(Report report, int width, int height) {
    string titleBackup = report.Title;
    report.Title = "";
    try {
      string result;
      if (report.IsChart && AdHocSettings.ChartingEngine==ChartingEngine.HtmlChart) {
        string reportFullName = report.ParentReportSet.ReportName;
        if (!string.IsNullOrEmpty(report.ParentReportSet.ReportCategory)) {
          reportFullName = report.ParentReportSet.ReportCategory+"\\"+reportFullName;
        }

        string html = null;
        string fid = ResponseServer.RenderHighChart(reportFullName, report.Name, width, height, false, false, null, out html);
        //string file = CreateStandaloneHighchartScreenshot(html, 840, 360);
        //fid = ResponseServer.AddToDisk(new ImageContentGenerator(Bitmap.FromFile(file), null));
        result =
          Utility.FixImgTagUrlsForLocalhost(string.Format(@"<html><body><img border=0 src='{0}'/></body></html>",
                                            AdHocSettings.ResponseServerWithDelimiter+"fid="+fid));
      } else {
        result = Utility.FixImgTagUrlsForLocalhost(report.RenderHtml(width, height));
      }
      if (result.IndexOf("No Fields")>=0) {
        return null;
      }
      return result;
    } catch (Exception e) {
      throw e;
    } finally {
      report.Title = titleBackup;
    }
  }

  private AutoResetEvent rsEvent;

  protected void Page_Load(object sender, EventArgs e) {
    var reportSet = AdHocContext.CurrentReportSet;
    var backupCollection = new Dictionary<string, Report>();
    foreach (String reportKey in AdHocContext.CurrentReportSet.Reports.AllKeys) {
      backupCollection.Add(reportKey, AdHocContext.CurrentReportSet.Reports[reportKey]);
    }
    if (reportSet==null) {
      return;
    }
    try {
      rsEvent = new AutoResetEvent(false);
      List<PageInfo> bitmaps = new List<PageInfo>();
      List<Report> reports = new List<Report>();
      foreach (string key in reportSet.Reports.AllKeys) {
        var report = reportSet.Reports[key];
        reports.Add(report);
      }
      reports.Sort(new ReportDashboardComparer());
      foreach (Report report in reports) {
        string html = null;
        if (report.IsChart) {
          html = RenderReport(report, 840, 360);
        } else {
          html = RenderReport(report, 1000, 650);
        }
        if (html!=null) {
          bool isChart = report.IsChart;
          var renderer = new HtmlToPictureRenderer(HttpContext.Current, delegate(HttpContext context, Bitmap bmp, Report currentReport) {
            bitmaps.Add(new PageInfo {
              Bitmap = bmp,
              Title = (currentReport != null && !string.IsNullOrEmpty(currentReport.Title)) ? currentReport.Title : " ",
              IsChart = isChart
            });
            rsEvent.Set();
          }, delegate(HttpContext context, string message, Bitmap bmp) {
            rsEvent.Set();
          });
          if (report.IsChart) {
            renderer.Render(report, html, new Unit(840, UnitType.Pixel), new Unit(360, UnitType.Pixel));
          } else {
            renderer.Render(report, html, new Unit(1000, UnitType.Pixel), new Unit(650, UnitType.Pixel));
          }
          rsEvent.WaitOne();
        }
      }

      // clear app_data directory
      string appdata = HttpContext.Current.Server.MapPath("~/App_Data");
      DirectoryInfo downloadedMessageInfo = new DirectoryInfo(appdata);
      foreach (FileInfo file in downloadedMessageInfo.GetFiles()) {
        file.Delete();
      }

      // wait for all bitmaps
      if (bitmaps.Count==0) {
        //Response.Write(string.Format("No data found for exporting (report set name: {0})", reportSet.ReportName));
        return;
      }

      // create powerpoint presentation
      var documentHelper = new PowerPointHelper2();
      byte[] bytesInStream = null;
      using (MemoryStream memoryStream = new MemoryStream()) {
        using (
          PresentationDocument package = PresentationDocument.Create(memoryStream, PresentationDocumentType.Presentation,
                                         true)) {
          documentHelper.CreateParts(package, bitmaps);
          #region validation
          /*var validationErrors = PowerPointHelper2.IsDocumentValid(package);
          if (validationErrors!=null) {
          	foreach (var err in validationErrors) {
          		Response.Write(string.Format("Presentation validation failed: {0}", err.Description));
          		Response.Write("<br/>");
          		Response.Write(string.Format("{0}, {1}, {2}, {3}", err.ErrorType, err.Id, err.Node, err.Part));
          	}
          	Response.End();
          	return;
          }*/
          #endregion
        }

        TextWriter textWriter = new StreamWriter(memoryStream);
        textWriter.Flush();
        bytesInStream = memoryStream.ToArray();
      }

      AdHocContext.CurrentReportSet.Reports.Clear();
      foreach (var key in backupCollection.Keys) {
        AdHocContext.CurrentReportSet.Reports.Add(key, backupCollection[key]);
      }

      Response.Clear();
      Response.ClearContent();
      Response.ClearHeaders();
      Response.ContentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
      Response.AppendHeader("content-disposition", "attachment;filename=report.pptx");
      Response.Charset = "";
      Response.BinaryWrite(bytesInStream);
      Response.Flush();
      Response.End();
    } catch (Exception ex) {
      throw ex;
      /*Response.Write(string.Format("Export exception occured: {0}", ex.Message));
      Response.Write(Environment.NewLine + "<br/>" + Environment.NewLine);
      Response.Write(ex.StackTrace);*/
    }
  }
}

