<%@ Control Language="C#" AutoEventWireup="true" CodeFile="edit.ascx.cs" Inherits="widgets_Tag_cloud_edit" %>
<%@ Reference VirtualPath="~/widgets/Tag cloud/widget.ascx" %>

<label for="<%=ddlNumber.ClientID %>">Minimum posts in each tag</label><br />
<asp:DropDownList runat="server" ID="ddlNumber">
  <asp:ListItem Value="1" Text="1 (default)" />
  <asp:ListItem Text="2" />
  <asp:ListItem Text="3" />
  <asp:ListItem Text="4" />
  <asp:ListItem Text="5" />
  <asp:ListItem Text="6" />
  <asp:ListItem Text="7" />
  <asp:ListItem Text="8" />
  <asp:ListItem Text="9" />
  <asp:ListItem Text="10" />
</asp:DropDownList>