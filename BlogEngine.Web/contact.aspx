<%@ Page Language="C#" AutoEventWireup="true" CodeFile="contact.aspx.cs" Inherits="contact" ValidateRequest="false" %>
<%@ Import Namespace="BlogEngine.Core" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" Runat="Server">
  <div id="contact">
    <div id="divForm" runat="server">
      <h1><%=Resources.labels.contact %></h1>
      
      <div><%=BlogSettings.Instance.ContactFormMessage %></div>
      
      <label for="<%=txtName.ClientID %>"><%=Resources.labels.name %></label>
      <asp:TextBox runat="server" id="txtName" cssclass="field" />
      <asp:requiredfieldvalidator runat="server" controltovalidate="txtName" ErrorMessage="<%$Resources:labels, required %>" validationgroup="contact" /><br />
      
      <label for="<%=txtEmail.ClientID %>"><%=Resources.labels.email %></label>
      <asp:TextBox runat="server" id="txtEmail" cssclass="field" />
      <asp:RegularExpressionValidator runat="server" ControlToValidate="txtEmail" display="dynamic" ErrorMessage="<%$Resources:labels, enterValidEmail %>" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" validationgroup="contact" />
      <asp:requiredfieldvalidator runat="server" controltovalidate="txtEmail" ErrorMessage="<%$Resources:labels, required %>" validationgroup="contact" /><br />
      
      <label for="<%=txtSubject.ClientID %>"><%=Resources.labels.subject %></label>
      <asp:TextBox runat="server" id="txtSubject" cssclass="field" />
      <asp:requiredfieldvalidator runat="server" controltovalidate="txtSubject" ErrorMessage="<%$Resources:labels, required %>" validationgroup="contact" /><br />
      
      <label for="<%=txtMessage.ClientID %>"><%=Resources.labels.message %></label>
      <asp:TextBox runat="server" id="txtMessage" textmode="multiline" rows="5" columns="30" />
      <asp:requiredfieldvalidator runat="server" controltovalidate="txtMessage" ErrorMessage="<%$Resources:labels, required %>" display="dynamic" validationgroup="contact" />    
      
      <asp:placeholder runat="server" id="phAttachment">      
        <label for="<%=txtAttachment.ClientID %>"><%=Resources.labels.attachFile %></label>
        <asp:FileUpload runat="server" id="txtAttachment" />
      </asp:placeholder>
      
      <br /><br />
      
      <asp:button runat="server" id="btnSend" Text="Send" OnClientClick="return beginSendMessage();" validationgroup="contact" />    
      <asp:label runat="server" id="lblStatus" visible="false">This form does not work at the moment. Sorry for the inconvenience.</asp:label>
    </div>
    
    <div id="thanks">
      <div id="divThank" runat="Server" visible="False">      
        <%=BlogSettings.Instance.ContactThankMessage %>
      </div>
    </div>
  </div>
  
  <script type="text/javascript">
    function beginSendMessage()
    {
    	if (BlogEngine.$('<%=txtAttachment.ClientID %>') && BlogEngine.$('<%=txtAttachment.ClientID %>').value.length > 0)
        return true;
        
      if(!Page_ClientValidate('contact'))
        return false;
        
      var name = BlogEngine.$('<%=txtName.ClientID %>').value;
      var email = BlogEngine.$('<%=txtEmail.ClientID %>').value;
      var subject = BlogEngine.$('<%=txtSubject.ClientID %>').value;
      var message = BlogEngine.$('<%=txtMessage.ClientID %>').value;
      var sep = '-||-';
      var arg = name + sep + email + sep + subject + sep + message;
      WebForm_DoCallback('__Page', arg, endSendMessage, 'contact', null, false) 
      
      BlogEngine.$('<%=btnSend.ClientID %>').disabled = true;
      
      return false;
    }
    
    function endSendMessage(arg, context)
    {
      BlogEngine.$('<%=btnSend.ClientID %>').disabled = false;
      var form = BlogEngine.$('<%=divForm.ClientID %>')
      var thanks = BlogEngine.$('thanks');
      
      form.style.display = 'none';
      thanks.innerHTML = arg;
    }
  </script>
</asp:Content>