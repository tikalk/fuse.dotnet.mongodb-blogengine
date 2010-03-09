<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/admin/Extension Manager/Extensions.ascx.cs" Inherits="UserControlsXmanagerExtensionsList" %>
<asp:Literal ID="Literal1" runat="server" Text="<h1>Extensions</h1>" />
<div id="lblErrorMsg" style="padding:5px; color:Red;" runat="server"></div>

<div style="border:1px solid #f3f3f3">
<asp:GridView ID="gridExtensionsList" 
    BorderColor="#f8f8f8" 
    BorderStyle="solid" 
    BorderWidth="1px" 
    RowStyle-BorderWidth="0"
    RowStyle-BorderStyle="None"
    gridlines="None"
    datakeynames="Name"
    runat="server"  
    width="100%"
    AlternatingRowStyle-BackColor="#f8f8f8"
    AlternatingRowStyle-BorderColor="#f8f8f8" 
    HeaderStyle-BackColor="#F1F1F1"
    cellpadding="3"
    AutoGenerateColumns="False"
    ShowFooter="true" >
  <Columns>
    <asp:BoundField HeaderText="Name" HeaderStyle-HorizontalAlign="Left" DataField = "Name" />  
    <asp:BoundField HeaderText="Version" DataField = "Version" />
    <asp:BoundField HeaderText="Description" HeaderStyle-HorizontalAlign="Left" DataField = "Description" /> 
    <asp:TemplateField HeaderText="Author" HeaderStyle-HorizontalAlign="Left">
        <ItemTemplate>
           <span><%# HttpContext.Current.Server.HtmlDecode(DataBinder.Eval(Container.DataItem, "Author").ToString())%></span>
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Status" HeaderStyle-HorizontalAlign="Left">
        <ItemTemplate>
           <%# StatusLink(DataBinder.Eval(Container.DataItem, "Name").ToString())%>
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Source" HeaderStyle-HorizontalAlign="Left">
        <ItemTemplate>
           <span><%# string.Format("<a href='?ctrl=editor&ext={0}'>View</a>", DataBinder.Eval(Container.DataItem, "Name"))%></span>
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Settings" HeaderStyle-HorizontalAlign="Left">
        <ItemTemplate>
           <span><%# SettingsLink(DataBinder.Eval(Container.DataItem, "Name").ToString())%></span>
        </ItemTemplate>
    </asp:TemplateField>
    <asp:BoundField HeaderText="Priority" DataField = "Priority" />
    <asp:TemplateField ShowHeader="False" ItemStyle-VerticalAlign="middle" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="25">
        <ItemTemplate>
            <asp:ImageButton ID="btnPriorityUp" runat="server" OnClick="btnPriorityUp_click" ImageAlign="middle" CausesValidation="false" ImageUrl="~/pics/up_arrow_small.gif" CommandName="btnPriorityUp" AlternateText="Up" />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField ShowHeader="False" ItemStyle-VerticalAlign="middle" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="25">
        <ItemTemplate>
            <asp:ImageButton ID="btnPriorityDwn" runat="server" OnClick="btnPriorityDwn_click" ImageAlign="middle" CausesValidation="false" ImageUrl="~/pics/down_arrow_small.gif" CommandName="btnPriorityDwn" AlternateText="" />
        </ItemTemplate>
    </asp:TemplateField>
  </Columns>
  <pagersettings Mode="NumericFirstLast" position="Bottom" pagebuttoncount="20" />
  <PagerStyle HorizontalAlign="Center"/>
</asp:GridView>
</div>

<br />
<div style="text-align:right">
  <asp:Button runat="Server" ID="btnRestart" 
    Text="Apply changes" 
    OnClientClick="return confirm('The website will be unavailable for a few seconds.\nAre you sure you wish to continue?')" 
  />
</div>