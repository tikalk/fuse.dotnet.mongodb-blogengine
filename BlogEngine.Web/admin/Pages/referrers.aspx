<%@ Page Language="C#" MasterPageFile="~/admin/admin1.master" AutoEventWireup="true" CodeFile="referrers.aspx.cs" Inherits="admin_Pages_referrers" Title="Referrers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" runat="Server">
    
    <div style="text-align: right">
        <asp:Button runat="server" ID="btnSaveTop" Text="<%$ Resources:labels, saveSettings %>" />
    </div>
        
    <div class="settings">
        <h1><%=Resources.labels.settings%></h1>
        <label for=""><%=Resources.labels.enableReferrerTracking %></label>
        <asp:CheckBox runat="Server" ID="cbEnableReferrers" AutoPostBack="true" /><br />
        
        <label for="<%= txtNumberOfDays.ClientID %>"><%= Resources.labels.numberOfDaysToKeep %></label>
        <asp:TextBox ID="txtNumberOfDays" runat="server"></asp:TextBox><br />
        
        <label for="<%=ddlDays.ClientID %>"><%=Resources.labels.selectDay %></label>
        <asp:DropDownList runat="server" ID="ddlDays" AutoPostBack="true" Style="text-transform: capitalize"
            DataTextFormatString="{0:d}">
        </asp:DropDownList>
    </div>
    
    <div class="settings">
        <h1><%=Resources.labels.referrers%></h1>
        <asp:GridView runat="server" ID="grid"
            BorderColor="#f8f8f8" 
            BorderStyle="solid" 
            BorderWidth="1px" 
            RowStyle-BorderWidth="0"
            RowStyle-BorderStyle="None"
            gridlines="None"
            width="100%"
            AlternatingRowStyle-BackColor="#f8f8f8"
            AlternatingRowStyle-BorderColor="#f8f8f8" 
            HeaderStyle-BackColor="#F1F1F1"
            cellpadding="3" 
            ShowFooter="true" 
            AutoGenerateColumns="False" 
            EnableViewState="false">
            <Columns>
                <asp:HyperLinkField HeaderText="<%$ Resources:labels, referrer %>" FooterStyle-HorizontalAlign="left"
                    DataNavigateUrlFields="url" Target="_blank" DataTextField="shortUrl" HeaderStyle-HorizontalAlign="left" />
                <asp:HyperLinkField HeaderText="<%$ Resources:labels, link %>" FooterStyle-HorizontalAlign="left"
                    DataNavigateUrlFields="target" Target="_blank" DataTextField="shortTarget" HeaderStyle-HorizontalAlign="left" />
                <asp:BoundField HeaderText="Hits" DataField="hits" HeaderStyle-HorizontalAlign="center"
                    ItemStyle-HorizontalAlign="center" ItemStyle-Width="40" />
            </Columns>
            <FooterStyle Font-Bold="true" HorizontalAlign="center" />
        </asp:GridView>
    </div>
    
    <div class="settings">
        <h1><%=Resources.labels.possibleSpam%></h1>
        <asp:GridView runat="server" ID="spamGrid"
            BorderColor="#f8f8f8" 
            BorderStyle="solid" 
            BorderWidth="1px" 
            RowStyle-BorderWidth="0"
            RowStyle-BorderStyle="None"
            gridlines="None"
            width="100%"
            AlternatingRowStyle-BackColor="#f8f8f8"
            AlternatingRowStyle-BorderColor="#f8f8f8" 
            HeaderStyle-BackColor="#F1F1F1"
            cellpadding="3"  
            ShowFooter="true" 
            AutoGenerateColumns="False"
            EnableViewState="false">
            <Columns>
                <asp:HyperLinkField HeaderText="<%$ Resources:labels, referrer %>" FooterStyle-HorizontalAlign="left"
                    DataNavigateUrlFields="url" Target="_blank" DataTextField="shortUrl" HeaderStyle-HorizontalAlign="left" />
                <asp:BoundField HeaderText="Hits" DataField="hits" HeaderStyle-HorizontalAlign="center"
                    ItemStyle-HorizontalAlign="center" ItemStyle-Width="40" />
            </Columns>
            <FooterStyle Font-Bold="true" HorizontalAlign="center" />
        </asp:GridView>
    </div>
    
    <div align="right">
        <asp:Button runat="server" ID="btnSave" Text="<%$ Resources:labels, saveSettings %>"/>
    </div>
</asp:Content>