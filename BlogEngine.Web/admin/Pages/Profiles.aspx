<%@ Page Language="C#" MasterPageFile="~/admin/admin1.master" AutoEventWireup="true" ValidateRequest="False" CodeFile="Profiles.aspx.cs" Inherits="admin_profiles" Title="Modify Profiles" %>
<%@ Register Src="~/admin/htmlEditor.ascx" TagPrefix="Blog" TagName="TextEditor" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" runat="Server">

    <div class="settings" id="dropdown" runat="server"> 
        <h1>
            <%=Resources.labels.userProfiles %>
        </h1>
        <asp:Panel ID="pnlAdmin" runat="server" Visible='<%# User.IsInRole("Administrator") %>'>
            <asp:DropDownList ID="ddlUserList" runat="server"></asp:DropDownList>
            <asp:LinkButton ID="lbChangeUserProfile" runat="server" 
                OnClick="lbChangeUserProfile_Click"><%= Resources.labels.switchUserProfile %></asp:LinkButton>
        </asp:Panel>
    </div>
    
    <div class="settings" style="padding: 0 10px">
      <br />
      <label for="<%=cbIsPublic.ClientID %>"><%=Resources.labels.isPrivate %></label>
      <asp:CheckBox ID="cbIsPublic" runat="server" />       <br />
       
      <div style="margin-bottom:3px">
      <label for="<%=tbDisplayName.ClientID %>"> <%=Resources.labels.displayName %></label>
      <asp:TextBox ID="tbDisplayName" runat="server"></asp:TextBox>
      </div>
      <div style="margin-bottom:3px">
      <label for="<%=tbFirstName.ClientID %>"> <%=Resources.labels.firstName %></label>
      <asp:TextBox ID="tbFirstName" runat="server"></asp:TextBox>
      </div>
      <div style="margin-bottom:3px">
      <label for="<%=tbMiddleName.ClientID %>"><%=Resources.labels.middleName %></label>
      <asp:TextBox ID="tbMiddleName" runat="server"></asp:TextBox>
      </div>
      <div style="margin-bottom:3px">
      <label for="<%=tbLastName.ClientID %>"><%=Resources.labels.lastName %></label>
      <asp:TextBox ID="tbLastName" runat="server"></asp:TextBox>
      </div>

      <div style="margin-bottom:3px">
      <label for="<%=tbPhoneMain.ClientID %>"><%=Resources.labels.phoneMain %></label>
      <asp:TextBox ID="tbPhoneMain" runat="server"></asp:TextBox>
      </div>
      <div style="margin-bottom:3px">
      <label for="<%=tbPhoneMobile.ClientID %>"><%=Resources.labels.phoneMobile %></label>   
      <asp:TextBox ID="tbPhoneMobile" runat="server"></asp:TextBox>
      </div>
      <div style="margin-bottom:3px">
      <label for="<%=tbPhoneFax.ClientID %>"><%=Resources.labels.phoneFax %></label>
      <asp:TextBox ID="tbPhoneFax" runat="server"></asp:TextBox>
      </div>
      <div style="margin-bottom:3px">
      <label for="<%=tbEmailAddress.ClientID %>"><%=Resources.labels.emailAddress %></label>
      <asp:TextBox ID="tbEmailAddress" runat="server"></asp:TextBox>
      </div>
              
      <div style="margin-bottom:3px">
      <label for="<%=tbCityTown.ClientID %>"><%=Resources.labels.cityTown %></label>
      <asp:TextBox ID="tbCityTown" runat="server"></asp:TextBox>
      </div>
      <div style="margin-bottom:0">
      <label for="<%=tbRegionState.ClientID %>"><%=Resources.labels.regionState %></label>
      <asp:TextBox ID="tbRegionState" runat="server"></asp:TextBox>
      </div>
      <div style="margin-bottom:0">
      <label for="<%=ddlCountry.ClientID %>"><%=Resources.labels.country %></label>
      <asp:DropDownList ID="ddlCountry" runat="server" AutoPostBack="false" />
      </div>
      <div style="margin-bottom:3px">
      <label for="<%=tbCompany.ClientID %>"><%=Resources.labels.company %></label>
      <asp:TextBox runat="server" id="tbCompany" />
      </div>
      <div style="margin-bottom:3px">
      <label for="<%=tbPhotoUrl.ClientID %>"><%=Resources.labels.photoURL %></label>
      <asp:TextBox ID="tbPhotoUrl" runat="server" Columns="50"></asp:TextBox>  
      </div>
      <div style="margin-bottom:3px">
      <label for="<%=tbBirthdate.ClientID %>"><%=Resources.labels.birthday %></label>
      <asp:TextBox ID="tbBirthdate" runat="server"></asp:TextBox>
      <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="tbBirthdate" Type="date" Operator="datatypecheck" ErrorMessage="Please enter a valid date (yyyy-mm-dd)" />
      </div>
      <div style="margin-bottom:3px">
      <label for="<%=tbAboutMe.ClientID %>"><%=Resources.labels.aboutMe %></label><br />
      <Blog:TextEditor runat="server" id="tbAboutMe" Rows="5" Width="50px" TextMode="MultiLine"  />
      </div>
    </div>
        
    <p style="text-align:right;">
        <asp:button ID="lbSaveProfile" runat="server" OnClick="lbSaveProfile_Click" />
    </p>
    
</asp:Content>
