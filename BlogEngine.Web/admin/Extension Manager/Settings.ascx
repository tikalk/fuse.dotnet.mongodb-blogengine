<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/admin/Extension Manager/Settings.ascx.cs" Inherits="User_controls_xmanager_Parameters" %>
<h1><%=SettingName%></h1>
<div id="ErrorMsg" runat="server" style="color:Red; padding:5px 0 5px 0; display:block;"></div>
<div id="InfoMsg" runat="server" style="color:Green; padding:5px 0 5px 0; display:block;"></div>

<% if(!string.IsNullOrEmpty(_settings.Help)) { %>
<div class="info" style="float:right; width: 350px;"><%=_settings.Help%></div>
<%} %>

<div class="mgr">
    <asp:PlaceHolder ID="phAddForm" runat="server"></asp:PlaceHolder>
</div>

<asp:Button runat="server" ID="btnAdd" ValidationGroup="new" />
<br /><hr />

<asp:GridView ID="grid"  
        runat="server"  
        GridLines="None"
        AutoGenerateColumns="False"
        HeaderStyle-BackColor="#f9f9f9"
        AlternatingRowStyle-BackColor="#f7f7f7"
        CellPadding="3" 
        HeaderStyle-HorizontalAlign="Left"
        BorderStyle="Ridge"
        BorderWidth="1"
        Width="100%"
        AllowPaging="True" 
        AllowSorting="True"
        onpageindexchanging="grid_PageIndexChanging" 
        OnRowDataBound="grid_RowDataBound" >
 </asp:GridView>
