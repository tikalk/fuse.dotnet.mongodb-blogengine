<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CommentView.ascx.cs" Inherits="User_controls_CommentView" %>
<%@ Import Namespace="BlogEngine.Core" %>

<% if (Post.Comments.Count > 0){ %>
<p id="comment"><%=Resources.labels.comments %></p>
<%} %>

<div id="commentlist" <%= (Post.Comments.Count == 0) ? " style=\"display:none;\"" : ""%>>
  <asp:PlaceHolder runat="server" ID="phComments" />  
</div>

<asp:PlaceHolder runat="Server" ID="phAddComment">

<div id="comment-form">

	<img src="<%=Utils.RelativeWebRoot %>pics/ajax-loader.gif" alt="Saving the comment" style="display:none" id="ajaxLoader" />  
	<span id="status"></span>

	<div class="commentForm">
	  <p id="addcomment"><%=Resources.labels.addComment %></p>

	  <% if (NestingSupported){ %>
		<asp:HiddenField runat="Server" ID="hiddenReplyTo"  />
		<p id="cancelReply" style="display:none;"><a href="javascript:void(0);" onclick="BlogEngine.cancelReply();"><%=Resources.labels.cancelReply %></a></p>
	  <%} %>

	  <label for="<%=NameInputId %>"><%=Resources.labels.name %>*</label>
	  <input type="text" name="<%= NameInputId %>" id="<%= NameInputId %>" tabindex="2" value="<%= DefaultName %>" />
	  <span id="spnNameRequired" style="color:Red;display:none;"> <asp:Literal runat="server" Text="<%$Resources:labels, required %>"></asp:Literal></span>
	  <span id="spnChooseOtherName" style="color:Red;display:none;"> <asp:Literal runat="server" Text="<%$Resources:labels, chooseOtherName %>"></asp:Literal></span><br />

	  <label for="<%=txtEmail.ClientID %>"><%=Resources.labels.email %>*</label>
	  <asp:TextBox runat="Server" ID="txtEmail" TabIndex="3" ValidationGroup="AddComment" />
	  <span id="gravatarmsg">
	  <%if (BlogSettings.Instance.Avatar != "none" && BlogSettings.Instance.Avatar != "monster"){ %>
	  (<%=string.Format(Resources.labels.willShowGravatar, "<a href=\"http://www.gravatar.com\" target=\"_blank\">Gravatar</a>")%>)
	  <%} %>
	  </span>
	  <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtEmail" ErrorMessage="<%$Resources:labels, required %>" Display="dynamic" ValidationGroup="AddComment" />
	  <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtEmail" ErrorMessage="<%$Resources:labels, enterValidEmail%>" Display="dynamic" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="AddComment" /><br />

	  <label for="<%=txtWebsite.ClientID %>"><%=Resources.labels.website %></label>
	  <asp:TextBox runat="Server" ID="txtWebsite" TabIndex="4" ValidationGroup="AddComment" />
	  <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="Server" ControlToValidate="txtWebsite" ValidationExpression="(http://|https://|)([\w-]+\.)+[\w-]+(/[\w- ./?%&=;~]*)?" ErrorMessage="<%$Resources:labels, enterValidUrl %>" Display="Dynamic" ValidationGroup="AddComment" /><br />
	  
	  <% if(BlogSettings.Instance.EnableCountryInComments){ %>
	  <label for="<%=ddlCountry.ClientID %>"><%=Resources.labels.country %></label>
	  <asp:DropDownList runat="server" ID="ddlCountry" onchange="BlogEngine.setFlag(this.value)" TabIndex="5" EnableViewState="false" ValidationGroup="AddComment" />&nbsp;
	  <asp:Image runat="server" ID="imgFlag" AlternateText="Country flag" Width="16" Height="11" EnableViewState="false" /><br /><br />
	  <%} %>

	  <span class="bbcode<%= !BlogSettings.Instance.ShowLivePreview ? " bbcodeNoLivePreview" : "" %>" title="BBCode tags"><%=BBCodes() %></span>
	  <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtContent" ErrorMessage="<%$Resources:labels, required %>" Display="dynamic" ValidationGroup="AddComment" /><br />

	  <% if (BlogSettings.Instance.ShowLivePreview) { %>  
	  <ul id="commentMenu">
		<li id="compose" class="selected" onclick="BlogEngine.composeComment()"><%=Resources.labels.comment%></li>
		<li id="preview" onclick="BlogEngine.showCommentPreview()"><%=Resources.labels.livePreview%></li>
	  </ul>
	  <% } %>
	  <div id="commentCompose">
			<label for="<%=txtContent.ClientID %>" style="display:none"><%=Resources.labels.comment%></label>
		<asp:TextBox runat="server" ID="txtContent" TextMode="multiLine" Columns="50" Rows="10" TabIndex="6" ValidationGroup="AddComment" />
	  </div>
	  <div id="commentPreview">
		<img src="<%=Utils.RelativeWebRoot %>pics/ajax-loader.gif" alt="Loading" />  
	  </div>
	  
	  <br />
	  <input type="checkbox" id="cbNotify" style="width: auto" tabindex="7" />
	  <label for="cbNotify" style="width:auto;float:none;display:inline"><%=Resources.labels.notifyOnNewComments %></label><br /><br />
	 
	  <input type="button" id="btnSaveAjax" value="<%=Resources.labels.saveComment %>" onclick="return BlogEngine.validateAndSubmitCommentForm()" tabindex="7" />    
	  <asp:HiddenField runat="server" ID="hfCaptcha" />
	</div>

</div>

<script type="text/javascript">
<!--//
function registerCommentBox(){
	BlogEngine.comments.flagImage = BlogEngine.$("<%= imgFlag.ClientID %>");
	BlogEngine.comments.contentBox = BlogEngine.$("<%=txtContent.ClientID %>");
	BlogEngine.comments.moderation = <%=ManualModeration %>;
	BlogEngine.comments.checkName = <%=(!Page.User.Identity.IsAuthenticated).ToString().ToLowerInvariant() %>;
	BlogEngine.comments.postAuthor = "<%=Post.Author %>";
	BlogEngine.comments.nameBox = BlogEngine.$("<%= NameInputId %>");
	BlogEngine.comments.emailBox = BlogEngine.$("<%=txtEmail.ClientID %>");
	BlogEngine.comments.websiteBox = BlogEngine.$("<%=txtWebsite.ClientID %>");
	BlogEngine.comments.countryDropDown = BlogEngine.$("<%=ddlCountry.ClientID %>"); 
	BlogEngine.comments.captchaField = BlogEngine.$('<%=hfCaptcha.ClientID %>');
	BlogEngine.comments.controlId = '<%=this.UniqueID %>';
	BlogEngine.comments.replyToId = BlogEngine.$("<%=hiddenReplyTo.ClientID %>"); 
}
//-->
</script>

<% if (BlogSettings.Instance.IsCoCommentEnabled){ %>
<script type="text/javascript">
// this ensures coComment gets the correct values
coco =
{
     tool          : "BlogEngine",
     siteurl       : "<%=Utils.AbsoluteWebRoot %>",
     sitetitle     : "<%=BlogSettings.Instance.Name %>",
     pageurl       : location.href,
     pagetitle     : "<%=this.Post.Title %>",
     author        : "<%=this.Post.Title %>",
     formID        : "<%=Page.Form.ClientID %>",
     textareaID    : "<%=txtContent.UniqueID %>",
     buttonID      : "btnSaveAjax"
}
</script>
<script id="cocomment-fetchlet" src="http://www.cocomment.com/js/enabler.js" type="text/javascript">
</script>
<%} %>
</asp:PlaceHolder>

<asp:label runat="server" id="lbCommentsDisabled" visible="false"><%=Resources.labels.commentsAreClosed %></asp:label>