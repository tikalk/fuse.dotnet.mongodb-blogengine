<%@ Import namespace="BlogEngine.Core"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="widget.ascx.cs" Inherits="widgets_BlogRoll_widget" %>
 <blog:Blogroll ID="Blogroll1" runat="server" />
  <a href="<%=Utils.AbsoluteWebRoot %>opml.axd" style="display:block;text-align:right" title="Download OPML file" >Download OPML file <img src="<%=Utils.AbsoluteWebRoot %>pics/opml.png" alt="OPML" /></a>