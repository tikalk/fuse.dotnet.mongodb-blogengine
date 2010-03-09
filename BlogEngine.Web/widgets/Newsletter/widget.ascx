<%@ Control Language="C#" AutoEventWireup="true" CodeFile="widget.ascx.cs" Inherits="widgets_Newsletter_widget" %>

<div id="newsletterform">
  <p>Get notified when a new post is published.</p>

  <label for="<%=txtEmail.ClientID %>" style="font-weight:bold">Enter your e-mail</label><br />
  <asp:TextBox runat="server" ID="txtEmail" Width="100%" />
  <asp:RequiredFieldValidator 
    runat="server" 
    ControlToValidate="txtEmail" 
    ErrorMessage="Please enter an e-mail address" 
    Display="dynamic" 
    ValidationGroup="newsletter" />
    
  <asp:RegularExpressionValidator 
    runat="server" 
    ControlToValidate="txtEmail" 
    ErrorMessage="<%$Resources:labels, enterValidEmail %>" 
    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
    Display="dynamic" 
    ValidationGroup="newsletter" />
    
    <br />

  <div style="text-align:center">
    <asp:Button runat="server" ID="btnSave" ValidationGroup="newsletter" Text="Notify me" OnClientClick="return beginAddEmail()" />
  </div>
</div>

<div id="newsletterthanks" style="display:none;text-align:center">
  <br /><br />
  <h2 id="newsletteraction">Thank you</h2>
  <br /><br />
</div>

<script type="text/javascript">
  function beginAddEmail()
  {
    if(!Page_ClientValidate('newsletter'))
      return false;
      
    var arg = BlogEngine.$('<%=txtEmail.ClientID %>').value;
    var context = 'newsletter';
    <%=Page.ClientScript.GetCallbackEventReference(this, "arg", "endAddEmail", "context") %>;
    
    return false;
  }
  
  function endAddEmail(arg, context)
  {
    BlogEngine.$('newsletterform').style.display = 'none';
    BlogEngine.$('newsletterthanks').style.display = 'block';
    if (arg == "false")
    {
      BlogEngine.$('newsletteraction').innerHTML = "You are now unsubscribed";
    }
  }
</script>