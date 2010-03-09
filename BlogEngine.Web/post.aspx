<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="False" CodeFile="post.aspx.cs" Inherits="post" %>
<%@ Register Src="User controls/CommentView.ascx" TagName="CommentView" TagPrefix="uc" %>
<asp:content id="Content1" contentplaceholderid="cphBody" runat="Server">
  
  <asp:PlaceHolder ID="phCommentNotificationUnsubscription" runat="server" visible="false">
    <div id="commentNotificationUnsubscription">
        <h1><%= Resources.labels.commentNotificationUnsubscriptionHeader %></h1>
        <div><%= Resources.labels.commentNotificationUnsubscriptionText %></div>
    </div>
  </asp:PlaceHolder>

  <asp:placeholder runat="server" id="phPostNavigation" visible="false">
    <div id="postnavigation">
      <asp:hyperlink runat="server" id="hlPrev" /> | 
      <asp:hyperlink runat="server" id="hlNext" />
    </div>
  </asp:placeholder>
  
  <asp:placeholder runat="server" id="pwPost" />
  
  <asp:placeholder runat="server" id="phRDF">
    <!-- 
    <rdf:RDF xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#" xmlns:dc="http://purl.org/dc/elements/1.1/" xmlns:trackback="http://madskills.com/public/xml/rss/module/trackback/">
      <rdf:Description rdf:about="<%=Post.AbsoluteLink %>" dc:identifier="<%=Post.AbsoluteLink %>" dc:title="<%=Post.Title %>" trackback:ping="<%=Post.TrackbackLink %>" />
    </rdf:RDF>
    -->
  </asp:placeholder>
  
  <blog:RelatedPosts runat="server" ID="related" MaxResults="3" ShowDescription="true" DescriptionMaxLength="100" Visible="false" />
  <uc:CommentView ID="CommentView1" runat="server" />
</asp:content>