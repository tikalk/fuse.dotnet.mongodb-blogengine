<%@ Page Language="C#" MasterPageFile="~/admin/admin1.master" ValidateRequest="false" AutoEventWireup="true" CodeFile="Settings.aspx.cs" Inherits="admin_Pages_configuration" Title="Settings" %>
<%@ Import Namespace="BlogEngine.Core" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" runat="Server">

    <script type="text/javascript">
      function PreviewTheme()
      {
        var theme = document.getElementById('<%=ddlTheme.ClientID %>').value;
        var path = '../../?theme=' + theme;
        window.open(path);
      }
      
			function geodeAsk()
			{
				if (navigator.geolocation)
					navigator.geolocation.getCurrentPosition(geoFound, geoNotFound);
			}

			function geoFound(pos)
			{
				document.getElementById('<%=txtGeocodingLatitude.ClientID %>').value = pos.latitude;
				document.getElementById('<%=txtGeocodingLongitude.ClientID %>').value = pos.longitude;
			}

			function geoNotFound()
			{
				alert('You must be on a wifi network for us to determine your location');
			}       
    </script>

    <div style="text-align: right">
        <asp:Button runat="server" ID="btnSaveTop" />
    </div>
    
    <div class="settings">
        <h1><%=Resources.labels.basic %> <%=Resources.labels.settings.ToLowerInvariant() %></h1>
        
        <div style="margin-bottom:3px">
        <label for="<%=txtName.ClientID %>"><%=Resources.labels.name %></label>
        <asp:TextBox runat="server" ID="txtName" Width="300" />
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtName" ErrorMessage="Required" />
        </div>
        <div style="margin-bottom:3px">
        <label for="<%=txtDescription.ClientID %>"><%=Resources.labels.description %></label>
        <asp:TextBox runat="server" ID="txtDescription" Width="300" /><br />
        </div>
        <div>
        <label for="<%=txtPostsPerPage.ClientID %>"><%=Resources.labels.postPerPage %></label>
        <asp:TextBox runat="server" ID="txtPostsPerPage" Width="50" MaxLength="4" />
        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtPostsPerPage" ErrorMessage="Required" />
        <asp:CompareValidator runat="server" ControlToValidate="txtPostsPerPage" Operator="DataTypeCheck" Type="integer" ErrorMessage="Please enter a valid number" />
        </div>
        <div style="margin-bottom:0">
        <label for="<%=ddlTheme.ClientID %>"><%=Resources.labels.theme %></label>
        <asp:DropDownList runat="server" ID="ddlTheme" />
        <a href="javascript:void(PreviewTheme());">Preview</a> | 
        <a href="http://www.dotnetblogengine.net/page/themes.aspx" target="_blank">Download</a>
        </div>
        <div style="margin-bottom:3px">
        <label for="<%=ddlMobileTheme.ClientID %>"><%=Resources.labels.mobileTheme %></label>
        <asp:DropDownList runat="server" ID="ddlMobileTheme" />
        </div>
        <div style="margin-bottom:3px">
        <label for="<%=cbUseBlogNameInPageTitles.ClientID %>"><%=Resources.labels.useBlogNameInPageTitles%></label>
        <asp:CheckBox runat="server" ID="cbUseBlogNameInPageTitles" /><%=Resources.labels.useBlogNameInPageTitlesDescription%>
        </div>
        <div style="margin-bottom:3px">
        <label for="<%=cbShowRelatedPosts.ClientID %>"><%=Resources.labels.showRelatedPosts %></label>
        <asp:CheckBox runat="server" ID="cbShowRelatedPosts" />
        </div>
        <div style="margin-bottom:3px">
        <label for="<%=cbEnableRating.ClientID %>"><%=Resources.labels.enableRating %></label>
        <asp:CheckBox runat="server" ID="cbEnableRating" />
        </div>
        <div style="margin-bottom:3px">
        <label for="<%=cbShowDescriptionInPostList.ClientID %>"><%=Resources.labels.showDescriptionInPostList %></label>
        <asp:CheckBox runat="server" ID="cbShowDescriptionInPostList" />
        <label for="<%=txtDescriptionCharacters.ClientID %>" style="float:none; position:relative; top:-2px;"><%=Resources.labels.numberOfCharacters %></label>
        <asp:TextBox runat="server" ID="txtDescriptionCharacters" Width="40" />
        <asp:CompareValidator runat="server" ControlToValidate="txtDescriptionCharacters" Type="Integer" Operator="DataTypeCheck" ID="valDescChar" SetFocusOnError="true" />
        </div>
        <div style="margin-bottom:0">
        <label for="<%=cbShowDescriptionInPostListForPostsByTagOrCategory.ClientID %>"><%=Resources.labels.showDescriptionInPostListForPostsByTagOrCategory %></label>
        <asp:CheckBox runat="server" ID="cbShowDescriptionInPostListForPostsByTagOrCategory" />
        <label for="<%=txtDescriptionCharactersForPostsByTagOrCategory.ClientID %>" style="float:none; position:relative; top:-2px;"><%=Resources.labels.numberOfCharacters %></label>
        <asp:TextBox runat="server" ID="txtDescriptionCharactersForPostsByTagOrCategory" Width="40" />
        <asp:CompareValidator runat="server" ControlToValidate="txtDescriptionCharactersForPostsByTagOrCategory" Type="Integer" Operator="DataTypeCheck" SetFocusOnError="true" />
        </div><br />
        <div style="margin-bottom:3px">
        <label for="<%=cbTimeStampPostLinks.ClientID %>"><%=Resources.labels.timeStampPostLinks %></label>
        <asp:CheckBox runat="server" ID="cbTimeStampPostLinks" />
        </div>
        <div style="margin-bottom:3px">
        <label for="<%=cbShowPostNavigation.ClientID %>"><%=Resources.labels.showPostNavigation %></label>
        <asp:CheckBox runat="server" ID="cbShowPostNavigation" />
        </div>
        <div style="margin-bottom:0">
        <label for="<%=ddlCulture.ClientID %>"><%=Resources.labels.language %></label>
        <asp:DropDownList runat="Server" ID="ddlCulture" Style="text-transform: capitalize">
            <asp:ListItem Text="Auto" />
            <asp:ListItem Text="english" Value="en" />
        </asp:DropDownList>
        </div>
        <div style="margin-bottom:3px">
        <label for="<%=txtTimeZone.ClientID %>"><%=Resources.labels.timezone %></label>
        <asp:TextBox runat="Server" ID="txtTimeZone" Width="30" /> Server time: <%=DateTime.Now.ToShortTimeString() %>
        <asp:CompareValidator runat="server" ControlToValidate="txtTimeZone" Operator="dataTypeCheck" Type="double" Display="dynamic" ErrorMessage="Please specify a valid number (positive or negative)" />
        </div>
    </div>
    
    <div class="settings">
        <h1><%=Resources.labels.advancedSettings %></h1>
        
        <div style='display: <%= (Utils.IsMono) ? "none" : "block" %>'>
            <label for="<%=cbEnableCompression.ClientID %>"><%=Resources.labels.enableHttpCompression %></label>
            <asp:CheckBox runat="server" ID="cbEnableCompression" /><%=Resources.labels.enableHttpCompressionDescription %><br />
        </div>
        
        <label for="<%=cbRemoveWhitespaceInStyleSheets.ClientID %>"><%=Resources.labels.trimStylesheet %></label>
        <asp:CheckBox runat="server" ID="cbRemoveWhitespaceInStyleSheets" /><%=Resources.labels.trimStylesheetDescription %><br />
        
        <label for="<%=cbCompressWebResource.ClientID %>"><%=Resources.labels.compressWebResource %></label>
        <asp:CheckBox runat="server" ID="cbCompressWebResource" /><%=Resources.labels.compressWebResourceDescription%><br />
        
        <label for="<%=cbEnableOpenSearch.ClientID %>"><%=Resources.labels.enableOpenSearch %></label>
        <asp:CheckBox runat="server" ID="cbEnableOpenSearch" /><%=Resources.labels.enableOpenSearchDescription %><br />
        
        <label for="<%=cbRequireSslForMetaWeblogApi.ClientID %>"><%=Resources.labels.requireSslForMetaWeblogApi %></label>
        <asp:CheckBox runat="server" ID="cbRequireSslForMetaWeblogApi" /><%=Resources.labels.requireSslForMetaWeblogApiDescription%><br />
        
        <label for=""><%=Resources.labels.enableTrackbacks %></label>
        <asp:CheckBox runat="server" ID="cbEnableTrackBackSend" /><%=Resources.labels.send %> &nbsp;&nbsp;
        <asp:CheckBox runat="server" ID="cbEnableTrackBackReceive" /><%=Resources.labels.receive %><br />
        
        <label for=""><%=Resources.labels.enablePingbacks %></label>
        <asp:CheckBox runat="server" ID="cbEnablePingBackSend" /><%=Resources.labels.send %> &nbsp;&nbsp;
        <asp:CheckBox runat="server" ID="cbEnablePingBackReceive" /><%=Resources.labels.receive %><br />
        
        <label for="<%=rblWwwSubdomain.ClientID %>"><%=Resources.labels.handleWwwSubdomain %></label>
        <asp:RadioButtonList runat="server" ID="rblWwwSubdomain" RepeatLayout="flow" RepeatDirection="horizontal">
            <asp:ListItem Text="<%$ Resources:labels, remove %>" Value="remove" />
            <asp:ListItem Text="<%$ Resources:labels, enforce %>" Value="add" />
            <asp:ListItem Text="<%$ Resources:labels, ignore %>" Value="" Selected="true" />
        </asp:RadioButtonList><br />
        
        <label for="<%=cbEnableErrorLogging.ClientID %>"><%=Resources.labels.enableErrorLogging %></label>
        <asp:CheckBox runat="server" ID="cbEnableErrorLogging" /><%=Resources.labels.enableErrorLoggingDescription%><br />
    </div>

    <div class="settings">
        <h1>E-mail</h1>
        
        <div style="margin-bottom:3px">
        <label for="<%=txtEmail.ClientID %>"><%=Resources.labels.emailAddress %></label>
        <asp:TextBox runat="server" ID="txtEmail" Width="300" /><br />
        </div>
        <div style="margin-bottom:3px">
        <label for="<%=txtSmtpServer.ClientID %>">SMTP server</label>
        <asp:TextBox runat="server" ID="txtSmtpServer" Width="300" /><br />
        </div>
        <div style="margin-bottom:3px">
        <label for="<%=txtSmtpServerPort.ClientID %>"><%=Resources.labels.portNumber %></label>
        <asp:TextBox runat="server" ID="txtSmtpServerPort" Width="35" /> <%=Resources.labels.portNumberDescription %>
        <asp:CompareValidator ID="CompareValidator2" runat="Server" ControlToValidate="txtSmtpServerPort" Operator="datatypecheck" Type="integer" ErrorMessage="Not a valid number" /><br />
        </div>
        <div style="margin-bottom:3px">
        <label for="<%=txtSmtpUsername.ClientID %>"><%=Resources.labels.userName %></label>
        <asp:TextBox runat="server" ID="txtSmtpUsername" Width="300" /><br />
        </div>
        <label for="<%=txtSmtpPassword.ClientID %>"><%=Resources.labels.password %></label>
        <asp:TextBox runat="server" ID="txtSmtpPassword" Width="300" /><br />
        
        <label for="<%=cbEnableSsl.ClientID %>"><%=Resources.labels.enableSsl%></label>
        <asp:CheckBox runat="Server" ID="cbEnableSsl" /><br />
        
        <label for="<%=cbComments.ClientID %>"><%=Resources.labels.sendCommentEmail %></label>
        <asp:CheckBox runat="Server" ID="cbComments" /><br />
        
        <label for="<%=txtEmailSubjectPrefix.ClientID %>"><%=Resources.labels.emailSubjectPrefix %></label>
        <asp:TextBox runat="server" ID="txtEmailSubjectPrefix" Width="300" /><br /><br />
        
        <asp:Button runat="server" CausesValidation="False" ID="btnTestSmtp" Text="Test mail settings" />
        <asp:Label runat="Server" ID="lbSmtpStatus" />
    </div>
    
    <div class="settings">
        <h1>Feed <%=Resources.labels.settings.ToLowerInvariant() %></h1>
        <div>
        <label for="<%=ddlSyndicationFormat.ClientID %>" style="position: relative; top: 4px"><%=Resources.labels.defaultFeedOutput %></label>
        <asp:DropDownList runat="server" ID="ddlSyndicationFormat">
            <asp:ListItem Text="RSS 2.0" Value="Rss" Selected="True" />
            <asp:ListItem Text="Atom 1.0" Value="Atom" />
        </asp:DropDownList>
        format.
        </div>
        <div style="margin-bottom:3px">
        <label for="<%=txtPostsPerFeed.ClientID %>"><%=Resources.labels.postsPerFeed %></label>
        <asp:TextBox runat="server" ID="txtPostsPerFeed" Width="50" MaxLength="4" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPostsPerFeed" ErrorMessage="Required" />
        <asp:CompareValidator runat="server" ControlToValidate="txtPostsPerPage" Operator="DataTypeCheck" Type="integer" ErrorMessage="Please enter a valid number" /><br />
        </div>
        <div style="margin-bottom:3px">
        <label for="<%=txtDublinCoreCreator.ClientID %>"><%=Resources.labels.author %></label>
        <asp:TextBox runat="server" ID="txtDublinCoreCreator" Width="300" />
        </div>
        
        <div style="margin-bottom:3px">
        <label for="<%=txtDublinCoreLanguage.ClientID %>"><%=Resources.labels.languageCode %></label>
        <asp:TextBox runat="server" ID="txtDublinCoreLanguage" Width="60" />
        </div>
        <div style="margin-bottom:3px">
        <label for="<%=txtGeocodingLatitude.ClientID %>"><%=Resources.labels.latitude %></label>
        <asp:TextBox runat="server" ID="txtGeocodingLatitude" Width="300" />
        </div>
        <div style="margin-bottom:3px">
        <label for="<%=txtGeocodingLongitude.ClientID %>"><%=Resources.labels.longtitude %></label>
        <asp:TextBox runat="server" ID="txtGeocodingLongitude" Width="300" />&nbsp;
        <input type="button" id="findPosition" onclick="geodeAsk()" value="Find position" style="display:none" />
        <script type="text/javascript">
        if (navigator.geolocation)
					document.getElementById('findPosition').style.display = 'inline';
        </script>
        </div>
        <div style="margin-bottom:3px">
        <label for="<%=cbEnableEnclosures.ClientID %>">Enable Enclosures</label>
        <asp:CheckBox runat="server" ID="cbEnableEnclosures" />
        </div>
        <div style="margin-bottom:3px">
        <label for="<%=txtBlogChannelBLink.ClientID %>"><%=Resources.labels.endorsment %></label>
        <asp:TextBox runat="server" ID="txtBlogChannelBLink" MaxLength="255" Width="300" />
        </div>
        <div style="margin-bottom:3px">
        <label for="<%=txtAlternateFeedUrl.ClientID %>"><%=Resources.labels.alternateFeedUrl %></label>
        <asp:TextBox runat="server" ID="txtAlternateFeedUrl"  Width="300" /> <em>(http://feeds.feedburner.com/username)</em>
        <asp:RegularExpressionValidator runat="Server" ControlToValidate="txtAlternateFeedUrl" ValidationExpression="(http://|https://|)([\w-]+\.)+[\w-]+(/[\w- ./?%&=;~]*)?" ErrorMessage="Please enter a valid URL" Display="Dynamic" />
        </div>
    </div>
    
    <div class="settings">
        <h1>
            <%=Resources.labels.htmlHeadSection %>
        </h1>
        <label for="<%=txtHtmlHeader.ClientID %>">
            <%=Resources.labels.addCustomCodeToHeader %>
        </label>
        <asp:TextBox runat="server" ID="txtHtmlHeader" TextMode="multiLine" Rows="9" Columns="30"
            Width="500" />
    </div>
    
    <div class="settings">
        <h1>Tracking script</h1>
        <label for="<%=txtTrackingScript.ClientID %>">
          Visitor tracking script<br /><br />The JavaScript code from i.e. Google Analytics.<br /><br />
          Will be added in the bottom of each page regardless of the theme.<br /><br />(remember to add the &lt;script&gt; tags)</label>
        <asp:TextBox runat="server" ID="txtTrackingScript" TextMode="multiLine" Rows="9" Columns="30" Width="500" />
    </div>
    
    <div class="settings">
      <h1><%=Resources.labels.import %> & <%=Resources.labels.export %></h1>
      <p>
        <%=Resources.labels.blogMLDescription %>
        (<a href="http://blogml.org/" target="_blank">blogml.org</a>)
      </p>
      <input type="button" value="<%=Resources.labels.import %>" onclick="location.href='http://dotnetblogengine.net/clickonce/blogimporter/blog.importer.application?url=<%=Utils.AbsoluteWebRoot %>&username=<%=Page.User.Identity.Name %>'" />&nbsp;&nbsp;
      <input type="button" value="<%=Resources.labels.export %>" onclick="location.href='blogml.axd'" />
    </div>
    
    <div align="right">
        <asp:Button runat="server" ID="btnSave" />
    </div>
    
</asp:Content>