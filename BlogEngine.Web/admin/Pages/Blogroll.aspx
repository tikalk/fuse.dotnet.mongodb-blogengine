<%@ Page Language="C#" MasterPageFile="~/admin/admin1.master" AutoEventWireup="true" CodeFile="Blogroll.aspx.cs" Inherits="admin_Pages_blogroll" Title="Blogroll" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" runat="Server">

<div class="settings">
  <h1 style="margin: 0 0 5px 0"><%=Resources.labels.settings %></h1> 
  <div>
      <label for="<%=ddlVisiblePosts.ClientID %>" class="wide"><%=Resources.labels.numberOfDisplayedItems %></label>
      <asp:DropDownList runat="server" id="ddlVisiblePosts">
        <asp:ListItem Text="0" />
        <asp:ListItem Text="1" />
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
  </div>
  <div style="padding-bottom:5px">
      <label for="<%=txtMaxLength.ClientID %>" class="wide"><%=Resources.labels.maxLengthOfItems %></label>
      <asp:TextBox runat="server" ID="txtMaxLength" MaxLength="3" Width="50" />
      <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtMaxLength" Operator="dataTypeCheck" Type="integer" ValidationGroup="settings" ErrorMessage="Not a valid number" />
  </div>
  <div>
      <label for="<%=txtUpdateFrequency.ClientID %>" class="wide"><%=Resources.labels.updateFrequenzy %></label>
      <asp:TextBox runat="server" ID="txtUpdateFrequency" MaxLength="3" Width="50" />
      <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToValidate="txtUpdateFrequency" Operator="dataTypeCheck" Type="integer" ValidationGroup="settings" ErrorMessage="Not a valid number" />
  </div>  
  <div style="text-align:right">
    <asp:Button runat="server" ID="btnSaveSettings" ValidationGroup="settings" Width="120" />
  </div> 
 </div>
 
<div class="settings">
  
  <h1 style="margin: 0 0 5px 0"><%=Resources.labels.add %> blog</h1>
  
  <div style="margin-bottom:3px">
      <label for="<%=txtTitle.ClientID %>" class="wide"><%=Resources.labels.title %></label>
      <asp:TextBox runat="server" ID="txtTitle" Width="600px" />
      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="Server" ControlToValidate="txtTitle" ErrorMessage="required" ValidationGroup="addNew" />
  </div>
  <div style="margin-bottom:3px">
      <label for="<%=txtDescription.ClientID %>" class="wide"><%=Resources.labels.description %></label>
      <asp:TextBox runat="server" ID="txtDescription" Width="600px" />
      <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="Server" ControlToValidate="txtDescription" ErrorMessage="required" ValidationGroup="addNew" />
  </div>
  <div style="margin-bottom:3px">
     <label for="<%=txtWebUrl.ClientID %>" class="wide"><%=Resources.labels.website %></label>
     <asp:TextBox runat="server" ID="txtWebUrl" Width="600px" />
     <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="Server" ControlToValidate="txtWebUrl" ErrorMessage="required" Display="Dynamic" ValidationGroup="addNew" />
     <asp:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="txtWebUrl" ErrorMessage="Invalid" EnableClientScript="false" OnServerValidate="validateWebUrl" ValidationGroup="addnew"></asp:CustomValidator>
  </div>
  <div style="margin-bottom:3px">
      <label for="<%=txtFeedUrl.ClientID %>" class="wide">RSS url</label>
      <asp:TextBox runat="server" ID="txtFeedUrl" Width="600px" />
      <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="Server" ControlToValidate="txtFeedUrl" ErrorMessage="required" Display="Dynamic" ValidationGroup="addNew" />
      <asp:CustomValidator ID="CustomValidator2" runat="server" ControlToValidate="txtFeedUrl" ErrorMessage="Invalid" EnableClientScript="false" OnServerValidate="validateFeedUrl" ValidationGroup="addnew"></asp:CustomValidator>
  </div>
  
  <label for="<%=cblXfn.ClientID %>" class="wide">XFN tag</label>
  <asp:CheckBoxList runat="server" ID="cblXfn" CssClass="nowidth" RepeatColumns="8">
    <asp:ListItem Text="contact" />
    <asp:ListItem Text="acquaintance " />
    <asp:ListItem Text="friend " />
    <asp:ListItem Text="met" />
    <asp:ListItem Text="co-worker" />
    <asp:ListItem Text="colleague " />
    <asp:ListItem Text="co-resident" />
    <asp:ListItem Text="neighbor " />
    <asp:ListItem Text="child" />
    <asp:ListItem Text="parent" />
    <asp:ListItem Text="sibling" />
    <asp:ListItem Text="spouse" />
    <asp:ListItem Text="kin" />
    <asp:ListItem Text="muse" />
    <asp:ListItem Text="crush" />
    <asp:ListItem Text="date" />
    <asp:ListItem Text="sweetheart" />
    <asp:ListItem Text="me" />
  </asp:CheckBoxList>
  
  <div style="padding:3px; text-align:right; margin-bottom:5px">
    <asp:Button runat="server" ID="btnSave" ValidationGroup="addNew" Width="120" />
  </div>
  
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
    AutoGenerateColumns="False" 
    onrowdeleting="grid_RowDeleting"
    onrowcommand="grid_RowCommand">
        <Columns>
            <asp:TemplateField>
            <ItemTemplate>
                <asp:HyperLink ID="feedLink" runat="server" ImageUrl="~/pics/rssButton.gif" 
        NavigateUrl='<%# Eval("FeedUrl").ToString() %>' Text="<%# string.Empty %>"></asp:HyperLink>
            </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ControlStyle-BackColor="Transparent">
               <ItemTemplate>
                   <asp:ImageButton ID="ibMoveUp" ImageUrl="~/pics/up_arrow_small.gif" runat="server" CommandArgument="<%# ((GridViewRow)Container).RowIndex %>" CommandName="moveUp" Width="16" Height="8" />
                   <asp:ImageButton ID="ibMoveDown" ImageUrl="~/pics/down_arrow_small.gif" runat="server" CommandArgument="<%# ((GridViewRow)Container).RowIndex %>" CommandName="moveDown" Width="16" Height="8" />
               </ItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:HyperLink ID="HyperLink1" runat="server" 
                        NavigateUrl='<%# Eval("BlogUrl").ToString() %>' Text='<%# Eval("Title") %>'></asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Literal ID="Literal1" runat="server" Text='<%# Eval("Description") %>'></asp:Literal>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ShowDeleteButton="True" />
        </Columns>
    </asp:GridView>
</div>
</asp:Content>
