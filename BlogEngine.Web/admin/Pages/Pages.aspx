<%@ Page Language="C#" MasterPageFile="~/admin/admin1.master" AutoEventWireup="true" CodeFile="Pages.aspx.cs" ValidateRequest="false" Inherits="admin_Pages_pages" Title="Add page" %>
<%@ Register Src="../htmlEditor.ascx" TagPrefix="Blog" TagName="TextEditor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" Runat="Server">
  <script type="text/javascript">
  function ToggleVisibility()
  {
    var element = document.getElementById('<%=ulPages.ClientID%>');
    if (element.style.display == "none")
      element.style.display = "block";
    else
      element.style.display = "none";
  }

  function GetSlug()
  {
    var title = document.getElementById('<%=txtTitle.ClientID %>').value;
    WebForm_DoCallback('__Page', title, ApplySlug, 'slug', null, false) 
  }

  function ApplySlug(arg, context)
  {
    var slug = document.getElementById('<%=txtSlug.ClientID %>');
    slug.value = arg;
  }
  </script>
  
  <div id="divPages" runat="server" visible="true" enableviewstate="False" style="margin-bottom: 10px">
    <a id="aPages" runat="server" href="javascript:void(ToggleVisibility());" />
    <ul id="ulPages" runat="server" style="display:none;list-style-type:circle" />
  </div>
  
  <label for="<%=txtTitle.ClientID %>"><%=Resources.labels.title %></label>
  <asp:TextBox runat="server" ID="txtTitle" Width="400px" />
  <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTitle" Display="Dynamic" ErrorMessage="Please enter a title" />&nbsp;&nbsp;&nbsp;
  
  <label for="<%=ddlParent.ClientID %>"><%=Resources.labels.selectParent %></label>
  <asp:DropDownList runat="server" id="ddlParent" />&nbsp;&nbsp;&nbsp;
  
  <asp:CheckBox runat="Server" ID="cbIsFrontPage" Text="<%$ Resources:labels, isFrontPage %>" />
  
  <asp:CheckBox runat="Server" ID="cbShowInList" Text="<%$ Resources:labels, showInList %>" Checked="true" />
  <br /><br />

  <blog:TextEditor runat="server" id="txtContent" TabIndex="4" />
  
  <table id="entrySettings">
    <tr>
      <td class="label"><%=Resources.labels.uploadImage %></td>
      <td>
        <asp:FileUpload runat="server" ID="txtUploadImage" Width="400" size="50" />
        <asp:Button runat="server" ID="btnUploadImage" Text="Upload" ValidationGroup="imageupload" />
        <asp:RequiredFieldValidator runat="Server" ControlToValidate="txtUploadImage" ErrorMessage="<%$ Resources:labels, required %>" ValidationGroup="imageupload" />
      </td>
    </tr>
    <tr>
      <td class="label"><%=Resources.labels.uploadFile %></td>
      <td>
        <asp:FileUpload runat="server" ID="txtUploadFile" Width="400" size="50" />        
        <asp:Button runat="server" ID="btnUploadFile" Text="Upload" ValidationGroup="fileUpload" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtUploadFile" ErrorMessage="<%$ Resources:labels, required %>" ValidationGroup="fileUpload" />
      </td>
    </tr>
    <tr>
      <td class="label">Slug (optional)</td>
      <td>
        <asp:TextBox runat="server" ID="txtSlug" TabIndex="9" Width="400" />
        <a href="javascript:void(GetSlug());">Extract from title</a>
      </td>
    </tr>    
    <tr>
      <td class="label"><%=Resources.labels.description %></td>
      <td><asp:TextBox runat="server" ID="txtDescription" Width="400" TextMode="multiLine" Columns="50" Rows="4" /></td>
    </tr>
    <tr>
      <td class="label"><%=Resources.labels.keywords %></td>
      <td><asp:TextBox runat="server" ID="txtKeyword" Width="400" /></td>
    </tr>
    <tr>
      <td class="label"><%=Resources.labels.settings %></td>
      <td><asp:CheckBox runat="Server" ID="cbIsPublished" Checked="true" Text="<%$ Resources:labels, publish %>" /></td>
    </tr>
  </table>  
  
  <div style="text-align:right">
    <asp:Button runat="server" ID="btnSave"  />
  </div>
  <br />
</asp:Content>