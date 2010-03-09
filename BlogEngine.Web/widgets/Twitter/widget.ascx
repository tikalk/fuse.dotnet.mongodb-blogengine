<%@ Control Language="C#" AutoEventWireup="true" CodeFile="widget.ascx.cs" Inherits="widgets_Twitter_widget" %>
<asp:Repeater runat="server" ID="repItems" OnItemDataBound="repItems_ItemDataBound">
  <ItemTemplate>
    <img src="<%=BlogEngine.Core.Utils.RelativeWebRoot %>widgets/twitter/twitter.ico" alt="Twitter" />
    <asp:Label runat="server" ID="lblDate" style="color:gray" /><br />
    <asp:Label runat="server" ID="lblItem" /><br /><br />
  </ItemTemplate>
</asp:Repeater>

<asp:HyperLink runat="server" ID="hlTwitterAccount" Text="Follow me on Twitter" rel="me" />