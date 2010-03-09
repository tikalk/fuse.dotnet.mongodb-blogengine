<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Editor.aspx.cs" Inherits="admin_Comments_Editor" %>
<%@ Import Namespace="Resources"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
  <title><%=labels.widget %> <%=labels.editor %></title>
  <style type="text/css">
    body {font: 11px verdana; margin:0; overflow: hidden}
    #title {background: #F1F1F1; border-bottom: 1px solid silver; padding: 10px; font-weight:bold;}
    label {font-weight: bold; margin: 2px;}
    .field {padding:2px;}
    #phEdit {padding:10px; height: 325px; overflow: auto; overflow-x: hidden}
    #bottom {background: #F1F1F1; border-top: 1px solid silver; padding: 10px; text-align: center}
  </style>
</head>

<body scroll="no" onkeypress="ESCclose(event)">

  <script type="text/javascript">
    function ESCclose(evt) 
    {
      if (!evt) evt = window.event;

      if (evt && evt.keyCode == 27) 
        window.parent.closeEditor();
    }
  </script>
  
  <form id="form1" runat="server">
  
    <div id="title">
      <div style="font-size:14px"><a href="<%=CurrentComment.AbsoluteLink%>" target="_new" ><%=CurrentComment.Title%></a></div>
      <div style="padding-top: 5px; font-weight:normal;">
        <%=labels.author %>: <%=CurrentComment.Author %>        
        <%=labels.ip %>: <a href='http://www.domaintools.com/go/?service=whois&q=<%=CurrentComment.IP%>' target='_new'><%=CurrentComment.IP%></a>
        &nbsp;&nbsp;
        <asp:LinkButton ID="btnBlockIP" runat="server" Text="<%$ Resources:labels, block %>" 
              onclick="btnBlockIP_Click"></asp:LinkButton>
        &nbsp;&nbsp;
        <asp:LinkButton ID="btnAllowIP" runat="server" Text="<%$ Resources:labels, allow %>" 
              onclick="btnAllowIP_Click"></asp:LinkButton>
      </div>
    </div>
    
    <div runat="server" ID="phEdit">
        <p class="gravatar" style="float:right;margin: 0 14px 0 0;border:1px solid #ccc;"><%=Gravatar(80)%></p>
        <div class="field">
            <label for="<%=txtWebsite.ClientID%>"><%= labels.website %></label><br />
            <asp:TextBox ID="txtWebsite" runat="server" MaxLength="250" Width="300px"></asp:TextBox>
            <% if (!string.IsNullOrEmpty(CurrentComment.Country)){%><%=Flag%><%} %>
        </div>
        <div class="field">
            <label for="<%=txtEmail.ClientID%>"><%=labels.email %></label><br />
            <asp:TextBox ID="txtEmail" runat="server" MaxLength="250" Width="300px"></asp:TextBox>
        </div>
        <div id="txtComment" class="field">
            <label for="<%=txtArea.ClientID%>"><%=labels.comment %></label><br />
            <textarea id="txtArea" runat="server" cols="86" rows="12"></textarea>
        </div>       
        <span id="commId" runat="server" style="visibility:hidden; margin:0; padding:0"></span>
    </div>
    
    <div id="bottom">
        <asp:Button ID="BtnSave" runat="server" Text="<%$ Resources:labels, update %>" OnClick="BtnSaveClick" />
        <asp:Button ID="BtnAction" runat="server" Text="<%$ Resources:labels, approve %>" onclick="BtnActionClick" />
        <asp:Button ID="BtnDelete" runat="server" Text="<%$ Resources:labels, delete %>" 
            OnClientClick="javascript:return confirm('Are you sure you want to delte this comment?')" 
            onclick="BtnDeleteClick"  />
        <input type="button" value="<%=labels.cancel %>" onclick="parent.closeEditor()" />  
    </div>
  
  </form> 
</body>
</html>
