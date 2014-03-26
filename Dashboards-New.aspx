<%@ Page Title="" Language="C#" MasterPageFile="~/Default-New.master" AutoEventWireup="true" CodeFile="Dashboards.aspx.cs" 
  Inherits="Dashboards" %>
<%@ Register Src="~/DashboardsResources/html/Dashboards-New-Head.ascx" TagName="ccn1" TagPrefix="ccp1" %>
<%@ Register Src="~/DashboardsResources/html/Dashboards-New-Body.ascx" TagName="ccn2" TagPrefix="ccp2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
  <ccp1:ccn1 ID="Ccn1" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolder" runat="Server">
  <ccp2:ccn2 ID="Ccn2" runat="server" />
</asp:Content>