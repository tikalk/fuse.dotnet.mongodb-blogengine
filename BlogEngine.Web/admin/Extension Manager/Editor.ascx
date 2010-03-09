<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/admin/Extension Manager/Editor.ascx.cs" Inherits="User_controls_xmanager_SourceEditor" %>
<h1>Source Viewer: <%=_extensionName%></h1>
<div>
    <asp:TextBox ID="txtEditor" runat="server" TextMode="multiLine" Width="100%" Height="320"></asp:TextBox>
</div>
<div style="padding:5px 0 0 0">
    <asp:Button ID="btnSave" Visible="true" runat="server" Text="Save" OnClick="btnSave_Click" OnClientClick="return confirm('The website will be unavailable for a few seconds.\nAre you sure you wish to continue?');" />
    <span style="padding-left:10px">[<%=GetExtFileName() %>]</span>
</div>