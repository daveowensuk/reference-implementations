using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI;
using Izenda.AdHoc;

public partial class Annotation : Page {
  private static string connectionString = @"INSERT_CONNECTION_STRING_HERE";
  private static string tableName = "Annotations";
  private static SqlConnection connection = null;
  private static object locker = new object();

  private static bool CheckConnection() {
    bool success = true;
    try {
      if (connection == null) {
        connection = new SqlConnection(connectionString);
      }
      if (connection.State != ConnectionState.Open) {
        try {
          connection.Close();
        } catch {
        }
        connection.Open();
      }
    } catch {
      success = false;
    }
    if (success) {
      success = connection.State == ConnectionState.Open;
    }
    return success;
  }

  private static string GetAnnotationsList(string reportSet) {
    string emptyResult = "var ad = new Array();";
    lock (locker) {
      if (!CheckConnection()) {
        return emptyResult;
      }
      SqlDataReader reader;
      try {
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "select [ColName], [RowNumber], [CellData], [Value], [TenantId] from [" + tableName + "] where [ReportSet]=@reportName";
        SqlParameter nameParam = command.CreateParameter();
        nameParam.SqlDbType = SqlDbType.VarChar;
        nameParam.ParameterName = "@reportName";
        nameParam.Value = reportSet;
        command.Parameters.Add(nameParam);
        reader = command.ExecuteReader();
      } catch {
        return emptyResult;
      }
      if (reader == null || !reader.HasRows) {
        if (reader != null) {
          reader.Close();
        }
        return emptyResult;
      }
      Dictionary<string, List<AnnotationRecord>> annotations = new Dictionary<string, List<AnnotationRecord>>();
      while (reader.Read()) {
        AnnotationRecord annotation = new AnnotationRecord();
        annotation.ColName = reader.GetString(0);
        annotation.RowNumber = reader.GetInt32(1);
        annotation.CellData = reader.GetString(2);
        annotation.Value = reader.GetString(3);
        annotation.Value = annotation.Value.Replace("\n", "\\\n").Replace("'", "\\'");
        annotation.TenantId = reader.GetString(4);
        if (annotation.TenantId != "_global_" && annotation.TenantId != AdHocSettings.CurrentUserTenantId) {
          continue;
        }
        if (!annotations.ContainsKey(annotation.ColName)) {
          annotations.Add(annotation.ColName, new List<AnnotationRecord>());
        }
        bool skip = false;
        foreach (AnnotationRecord record in annotations[annotation.ColName]) {
          if (annotation.RowNumber == record.RowNumber && annotation.CellData == record.CellData) {
            if (annotation.TenantId != "_global_") {
              record.Value = annotation.Value;
              record.TenantId = annotation.TenantId;
            }
            skip = true;
            break;
          }
        }
        if (skip) {
          continue;
        }
        annotations[annotation.ColName].Add(annotation);
      }
      reader.Close();
      StringBuilder result = new StringBuilder();
      result.Append("var ad = new Array();");
      foreach (string colName in annotations.Keys) {
        result.Append("ad['" + colName + "'] = new Array();");
        foreach (AnnotationRecord annotation in annotations[colName]) {
          result.Append("ad['" + colName + "'][" + annotation.RowNumber + "] = new Object();");
          result.Append("ad['" + colName + "'][" + annotation.RowNumber + "].Cd = '" + annotation.CellData + "';");
          result.Append("ad['" + colName + "'][" + annotation.RowNumber + "].V = '" + annotation.Value + "';");
          result.Append("ad['" + colName + "'][" + annotation.RowNumber + "].T = '" + annotation.TenantId + "';");
        }
      }
      return result.ToString();
    }
  }

  private static void SetAnnotation(string reportName, string colName, int rowNumber, string cellData, string value, string tenantId) {
    lock (locker) {
      if (!CheckConnection()) {
        return;
      }
      try {
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "delete from [" + tableName + "] where [ReportSet]=@reportName and [ColName]=@colName and [RowNumber]=@rowNum and [TenantId]=@tfu";
        SqlParameter nameParam = command.CreateParameter();
        nameParam.SqlDbType = SqlDbType.VarChar;
        nameParam.ParameterName = "@reportName";
        nameParam.Value = reportName;
        command.Parameters.Add(nameParam);
        SqlParameter colParam = command.CreateParameter();
        colParam.SqlDbType = SqlDbType.VarChar;
        colParam.ParameterName = "@colName";
        colParam.Value = colName;
        command.Parameters.Add(colParam);
        SqlParameter rowNumParam = command.CreateParameter();
        rowNumParam.SqlDbType = SqlDbType.Int;
        rowNumParam.ParameterName = "@rowNum";
        rowNumParam.Value = rowNumber;
        command.Parameters.Add(rowNumParam);
        SqlParameter tfuParam = command.CreateParameter();
        tfuParam.SqlDbType = SqlDbType.VarChar;
        tfuParam.ParameterName = "@tfu";
        tfuParam.Value = tenantId;
        command.Parameters.Add(tfuParam);
        command.ExecuteNonQuery();
        if (String.IsNullOrEmpty(value)) {
          return;
        }
        command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "insert into [" + tableName + "] ([ReportSet], [ColName], [RowNumber], [CellData], [Value], [TenantId]) values (@reportName, @colName, @rowNum, @cellData, @value, @tfu)";
        nameParam = command.CreateParameter();
        nameParam.SqlDbType = SqlDbType.VarChar;
        nameParam.ParameterName = "@reportName";
        nameParam.Value = reportName;
        command.Parameters.Add(nameParam);
        colParam = command.CreateParameter();
        colParam.SqlDbType = SqlDbType.VarChar;
        colParam.ParameterName = "@colName";
        colParam.Value = colName;
        command.Parameters.Add(colParam);
        rowNumParam = command.CreateParameter();
        rowNumParam.SqlDbType = SqlDbType.Int;
        rowNumParam.ParameterName = "@rowNum";
        rowNumParam.Value = rowNumber;
        command.Parameters.Add(rowNumParam);
        SqlParameter cellDataParam = command.CreateParameter();
        cellDataParam.SqlDbType = SqlDbType.VarChar;
        cellDataParam.ParameterName = "@cellData";
        cellDataParam.Value = cellData;
        command.Parameters.Add(cellDataParam);
        SqlParameter valueParam = command.CreateParameter();
        valueParam.SqlDbType = SqlDbType.VarChar;
        valueParam.ParameterName = "@value";
        valueParam.Value = value;
        command.Parameters.Add(valueParam);
        tfuParam = command.CreateParameter();
        tfuParam.SqlDbType = SqlDbType.VarChar;
        tfuParam.ParameterName = "@tfu";
        tfuParam.Value = tenantId;
        command.Parameters.Add(tfuParam);
        command.ExecuteNonQuery();
      } catch {
      }
    }
  }

  protected override void Render(HtmlTextWriter writer) {
    if (HttpContext.Current == null || HttpContext.Current.Request == null) {
      return;
    }
    string command = HttpContext.Current.Request.Params["acmd"];
    if (String.IsNullOrEmpty(command)) {
      return;
    }
    string reportSingleTenant;
    string[] reportTenants;
    try {
      reportTenants = AdHocContext.CurrentReportSet.OwnerTenantID;
    } catch {
      reportTenants = null;
    }
    if (reportTenants == null) {
      reportSingleTenant = AdHocSettings.CurrentUserTenantId;
    } else {
      reportSingleTenant = reportTenants.Length > 0 ? reportTenants[0] : "_global_";
    }
    string response = "";
    switch (command) {
    case "getannotations":
      string reportName = HttpContext.Current.Request.Params["reportset"];
      if (String.IsNullOrEmpty(reportName)) {
        break;
      }
      reportName = reportSingleTenant + "###" + reportName;
      response = GetAnnotationsList(reportName);
      break;
    case "setannotation":
      string report = HttpContext.Current.Request.Params["reportset"];
      report = reportSingleTenant + "###" + report;
      string column = HttpContext.Current.Request.Params["colname"];
      if (String.IsNullOrEmpty(report) || String.IsNullOrEmpty(column)) {
        break;
      }
      if (String.IsNullOrEmpty(HttpContext.Current.Request.Params["rownum"])) {
        break;
      }
      int? rowNumber;
      try {
        rowNumber = int.Parse(HttpContext.Current.Request.Params["rownum"]);
      } catch {
        rowNumber = null;
      }
      if (!rowNumber.HasValue) {
        break;
      }
      string cellData = HttpContext.Current.Request.Params["celldata"];
      string tenantId = HttpContext.Current.Request.Params["tenantid"];
      string value = HttpContext.Current.Request.Params["value"];
      if (String.IsNullOrEmpty(cellData)) {
        cellData = "";
      }
      if (String.IsNullOrEmpty(tenantId)) {
        tenantId = "_global_";
      }
      if (tenantId == "_global_" && !String.IsNullOrEmpty(AdHocSettings.CurrentUserTenantId) && !String.IsNullOrEmpty(value)) {
        tenantId = AdHocSettings.CurrentUserTenantId;
      }
      if (String.IsNullOrEmpty(value)) {
        value = "";
      }
      SetAnnotation(report, column, rowNumber.Value, cellData, value, tenantId);
      break;
    default:
      break;
    }
    writer.Write(response);
  }
}

public class AnnotationRecord {
  public string ColName;
  public int RowNumber;
  public string CellData;
  public string Value;
  public string TenantId;
}
