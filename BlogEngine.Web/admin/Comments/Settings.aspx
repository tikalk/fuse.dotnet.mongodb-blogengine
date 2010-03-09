<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin1.master" AutoEventWireup="true" CodeFile="Settings.aspx.cs" Inherits="admin_Comments_Settings" %>
<%@ Import Namespace="Resources"%>
<%@ Register src="Menu.ascx" tagname="TabMenu" tagprefix="menu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" Runat="Server">

    <script type="text/javascript">

        function ToggleEnableComments() {
            var bx = document.getElementById('<%= cbEnableComments.ClientID %>'); 
            if (bx.checked) {
                document.getElementById('SettingsFields').style.display = "";
                document.getElementById('Moderation').style.display = "";
                ToggleModeration();
            }
            else{
                document.getElementById('SettingsFields').style.display = "none";
                document.getElementById('Moderation').style.display = "none";
                document.getElementById('Rules').style.display = "none";
                document.getElementById('Filters').style.display = "none";
                document.getElementById('CustomFilters').style.display = "none";
            }
        }

        function ToggleModeration() {
            var bx = document.getElementById('<%= cbEnableCommentsModeration.ClientID %>');
            if (bx.checked) {
                document.getElementById('tblModeration').style.display = "";
                ToggleModType();
            }
            else {
                document.getElementById('tblModeration').style.display = "none";
                document.getElementById('Rules').style.display = "none";
                document.getElementById('Filters').style.display = "none";
                document.getElementById('CustomFilters').style.display = "none";
            }
        }

        function ToggleModType() {
            var gbx = document.getElementsByName('RadioGroup1');
            var rdo = 0;

            for (var x = 0; x < gbx.length; x++) {
                if (gbx[x].checked) {
                    rdo = gbx[x].value;
                }
            }
            if (rdo == 1) { 
                document.getElementById('Rules').style.display = "";
                document.getElementById('Filters').style.display = "";
                document.getElementById('CustomFilters').style.display = "";
            }
            else {
                document.getElementById('Rules').style.display = 'none';
                document.getElementById('Filters').style.display = "none";
                document.getElementById('CustomFilters').style.display = "none";
            }
        }

        function ConfirmReset() {
            return confirm('<%=labels.confirmResetCounters%>');  
        } 
    </script>
  
    <div class="settings" id="GeneralSettings">   
        <menu:TabMenu ID="TabMenu" runat="server" />
        
        <div id="ErrorMsg" runat="server" style="color: Red; display: block;"></div>
        <div id="InfoMsg" runat="server" style="color: Green; display: block;"></div>
            
        <label for="<%=cbEnableComments.ClientID %>"><%=labels.enableComments %></label>
        <asp:CheckBox runat="server" ID="cbEnableComments" onclick="ToggleEnableComments();" /><%=labels.enableCommentsDescription %><br />
        
        <div id="SettingsFields">
        <label for="<%=cbEnableCommentNesting.ClientID %>"><%=labels.enableCommentNesting %></label>
        <asp:CheckBox runat="server" ID="cbEnableCommentNesting" /><%=labels.enableCommentNestingDescription%><br />
        
        <label for="<%=cbEnableCoComment.ClientID %>"><%=labels.enableCoComments %></label>
        <asp:CheckBox runat="server" ID="cbEnableCoComment" /><br />
        
        <label for="<%=cbEnableCountryInComments.ClientID %>"><%=labels.showCountryChooser %></label>
        <asp:CheckBox runat="server" ID="cbEnableCountryInComments" /><%=labels.showCountryChooserDescription %><br />
        
        <label for="<%=cbShowLivePreview.ClientID %>"><%=labels.showLivePreview %></label>
        <asp:CheckBox runat="server" ID="cbShowLivePreview" /><br />
        
        <label for="<%=rblAvatar.ClientID %>"><%=labels.avatars %></label>
        <asp:RadioButtonList runat="Server" ID="rblAvatar" RepeatLayout="flow" RepeatDirection="horizontal">
          <asp:ListItem Text="MonsterID" Value="monster" />
          <asp:ListItem Text="Wavatar" Value="wavatar" />
          <asp:ListItem Text="Identicon" Value="identicon" />
          <asp:ListItem Text="<%$ Resources:labels, none %>" Value="none" />
        </asp:RadioButtonList><br />

        <label for="<%=ddlCloseComments.ClientID %>" style="position: relative; top: 4px">
            <%=labels.closeCommetsAfter %>
        </label>
        <asp:DropDownList runat="server" ID="ddlCloseComments">
            <asp:ListItem Text="Never" Value="0" />
            <asp:ListItem Text="1" />
            <asp:ListItem Text="2" />
            <asp:ListItem Text="3" />
            <asp:ListItem Text="7" />
            <asp:ListItem Text="10" />
            <asp:ListItem Text="14" />
            <asp:ListItem Text="21" />
            <asp:ListItem Text="30" />
            <asp:ListItem Text="60" />
            <asp:ListItem Text="90" />
            <asp:ListItem Text="180" />
            <asp:ListItem Text="365" />
        </asp:DropDownList>
        <%=labels.days%>.<br />
        
        <label for="<%=ddlCommentsPerPage.ClientID %>" style="position: relative; top: 4px">
            <%=labels.commentsPerPage%>
        </label>
        <asp:DropDownList runat="server" ID="ddlCommentsPerPage">
            <asp:ListItem Text="5" />
            <asp:ListItem Text="10" />
            <asp:ListItem Text="15" />
            <asp:ListItem Text="20" />
            <asp:ListItem Text="50" />
        </asp:DropDownList>
        </div>
    </div>
    
    <div class="settings" id="Moderation">
        <h1><%=labels.moderation%></h1>
        <label for="<%=cbEnableCommentsModeration.ClientID %>"><%=labels.enableCommentsModeration%></label>
        <asp:CheckBox runat="server" ID="cbEnableCommentsModeration" onclick="ToggleModeration();" /> <%=labels.commentsUnmodApproved%>.<br />
        <table width="550px" border="0" id="tblModeration">
            <tr>
                <td><input type="radio" name="RadioGroup1" onclick="ToggleModType();" value="0" style="border:0" <%=RadioChecked(0)%> /></td> 
                <td><%=labels.commentsApprovedByAdmin%></td>
            </tr>
            <tr>
                <td><input type="radio" name="RadioGroup1" onclick="ToggleModType();" value="1" style="border:0" <%=RadioChecked(1)%> /></td> 
                <td><%=labels.commentsAuto%>.</td>
            </tr>
        </table>
    </div>
    
    <div class="settings" id="Rules">
        <h1><%=labels.rules%></h1>
        
        <div class="cbox">
            <label for="<%=cbTrustAuthenticated.ClientID %>"><%=labels.trustAuthenticated%></label>
            <asp:CheckBox runat="server" ID="cbTrustAuthenticated" /><%=labels.alwaysTrust%>
        </div><br />
        
        <div class="ddown">
            <label><%=labels.addToWhiteList%></label>
            <asp:DropDownList runat="server" ID="ddWhiteListCount">
                <asp:ListItem Text="0" />
                <asp:ListItem Text="1" />
                <asp:ListItem Text="2" />
                <asp:ListItem Text="3" />
                <asp:ListItem Text="5" />
            </asp:DropDownList> 
            <span><%=labels.authorApproved%></span>  
        </div>
        
        <div class="ddown">
            <label for="<%=ddBlackListCount.ClientID %>"><%=labels.commentsBlacklist%></label>
            <asp:DropDownList runat="server" ID="ddBlackListCount">
                <asp:ListItem Text="0" />
                <asp:ListItem Text="1" />
                <asp:ListItem Text="2" />
                <asp:ListItem Text="3" />
                <asp:ListItem Text="5" />
            </asp:DropDownList> 
            <span><%=labels.authorRejected%></span>
        </div><br />
        
        <div class="cbox">
            <label for="<%=cbBlockOnDelete.ClientID %>"><%=labels.commentsBlockOnDelete%></label>
            <asp:CheckBox runat="server" ID="cbBlockOnDelete" />
            <span><%=labels.authorBlocked%></span>
        </div>
    </div>
    
    <div class="settings" id="Filters">
        <h1><%=labels.filters%></h1>
        
        <table>
            <tr>
                <td>
                    <asp:DropDownList ID="ddAction" runat="server">
                        <asp:ListItem Text="<%$ Resources:labels, block %>" Value="Block" Selected=true></asp:ListItem>
                        <asp:ListItem Text="<%$ Resources:labels, allow %>" Value="Allow" Selected=false></asp:ListItem>
                        <asp:ListItem Text="<%$ Resources:labels, delete %>" Value="Delete" Selected=false></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:DropDownList ID="ddSubject" runat="server">
                        <asp:ListItem Text="<%$ Resources:labels, ip %>" Value="IP" Selected=true></asp:ListItem>
                        <asp:ListItem Text="<%$ Resources:labels, author %>" Value="Author" Selected=false></asp:ListItem>
                        <asp:ListItem Text="<%$ Resources:labels, website %>" Value="Website" Selected=false></asp:ListItem>
                        <asp:ListItem Text="<%$ Resources:labels, email %>" Value="Email" Selected=false></asp:ListItem>
                        <asp:ListItem Text="<%$ Resources:labels, comment %>" Value="Comment" Selected=false></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:DropDownList ID="ddOperator" runat="server">
                        <asp:ListItem Text="<%$ Resources:labels, eqls %>" Value="Equals" Selected=true></asp:ListItem>
                        <asp:ListItem Text="<%$ Resources:labels, contains %>" Value="Contains" Selected=false></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td><asp:TextBox ID="txtFilter" runat="server" MaxLength="250" Width="300px"></asp:TextBox></td>
                <td>
                    <asp:Button ID="btnAddFilter" runat="server" Text="<%$ Resources:labels, addFilter %>" OnClick="btnAddFilter_Click"/>
                </td>
                <td><span runat="Server" ID="FilterValidation" style="color:Red"></span></td>
            </tr>
        </table>
        
        <div style="border:1px solid #f3f3f3">
        <asp:GridView ID="gridFilters" 
                PageSize="10" 
                BorderColor="#f8f8f8" 
                BorderStyle="solid" 
                BorderWidth="1px"
                cellpadding="2"
                runat="server"  
                width="100%"
                gridlines="None"
                AlternatingRowStyle-BackColor="#f8f8f8" 
                HeaderStyle-BackColor="#f3f3f3"
                AutoGenerateColumns="False"
                AllowPaging="True"
                datakeynames="ID"
                OnPageIndexChanging="gridView_PageIndexChanging"
                AllowSorting="True">
              <Columns>
                <asp:BoundField DataField = "ID" Visible="false" />
                <asp:TemplateField HeaderText="<%$ Resources:labels, action %>" HeaderStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                        <%# Eval("Action") %> 
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        comments where
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <%# Eval("Subject") %>
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <%# Eval("Operator") %>
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="<%$ Resources:labels, filter %>" HeaderStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                         <%# Eval("Filter") %> 
                    </ItemTemplate>
                </asp:TemplateField>
                        
                <asp:TemplateField ShowHeader="False" ItemStyle-VerticalAlign="middle" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="25">
                    <ItemTemplate>
                        <asp:ImageButton ID="btnDelete" runat="server" ImageAlign="middle" CausesValidation="false" ImageUrl="~/admin/images/del.png" OnClick="btnDelete_Click" CommandName="btnDelete" AlternateText="<%=labels.delete%>" />
                    </ItemTemplate>
                </asp:TemplateField>
              </Columns>
              <pagersettings Mode="NumericFirstLast" position="Bottom" pagebuttoncount="20" />
              <PagerStyle HorizontalAlign="Center"/>
          </asp:GridView>
          </div>
    </div>
    
    <div class="settings" id="CustomFilters">
        <h1><%=labels.custom %> <%=labels.filters %></h1>
        <div style="border:1px solid #f3f3f3">       
        <asp:GridView ID="gridCustomFilters" 
                BorderColor="#f8f8f8" 
                BorderStyle="solid" 
                BorderWidth="1px"
                gridlines="None"
                AlternatingRowStyle-BackColor="#f8f8f8" 
                HeaderStyle-BackColor="#f3f3f3"
                cellpadding="2"
                runat="server"  
                width="100%" 
                datakeynames="FullName"
                AutoGenerateColumns="False" 
                onrowcommand="gridCustomFilters_RowCommand">
              <Columns>
                <asp:BoundField DataField = "FullName" Visible="false" />
                <asp:TemplateField HeaderText="<%$ Resources:labels, enabled%>" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="60">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkEnabled" Checked='<%# CustomFilterEnabled(DataBinder.Eval(Container.DataItem, "FullName").ToString()) %>' Enabled="false" runat="server"/>
                    </ItemTemplate>
                </asp:TemplateField>  
                <asp:BoundField DataField = "FullName" HeaderText="<%$ Resources:labels, filterName %>" HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField = "Checked" HeaderText="<%$ Resources:labels, cheked %>" HeaderStyle-HorizontalAlign="Left" />
                <asp:TemplateField HeaderText="<%$ Resources:labels, approved %>" HeaderStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                        <%# ApprovedCnt(DataBinder.Eval(Container.DataItem, "Checked"), DataBinder.Eval(Container.DataItem, "Cought")) %> 
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField = "Cought" HeaderText="<%$ Resources:labels, spam %>" HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField = "Reported" HeaderText="<%$ Resources:labels, mistakes %>" HeaderStyle-HorizontalAlign="Left" />
                <asp:TemplateField HeaderText="<%$ Resources:labels, accuracy %>" HeaderStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                        <%# Accuracy(DataBinder.Eval(Container.DataItem, "Checked"), DataBinder.Eval(Container.DataItem, "Reported"))%> % 
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="<%$ Resources:labels, resetCounters %>" HeaderStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="btnResetCnt" CommandName="btnResetCnt"  CommandArgument='<%# Eval("FullName") %>' OnClientClick="return ConfirmReset();" Text="Reset"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField = "Priority" HeaderText="<%$ Resources:labels, priority %>" HeaderStyle-HorizontalAlign="Left" />
                <asp:TemplateField ShowHeader="False" ItemStyle-VerticalAlign="middle" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="25">
                    <ItemTemplate>
                        <asp:ImageButton ID="btnPriorityUp" runat="server" OnClick="btnPriorityUp_click" ImageAlign="middle" CausesValidation="false" ImageUrl="~/pics/up_arrow_small.gif" CommandName="btnPriorityUp" AlternateText="<%=labels.up%>" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="False" ItemStyle-VerticalAlign="middle" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="25">
                    <ItemTemplate>
                        <asp:ImageButton ID="btnPriorityDwn" runat="server" OnClick="btnPriorityDwn_click" ImageAlign="middle" CausesValidation="false" ImageUrl="~/pics/down_arrow_small.gif" CommandName="btnPriorityDwn" AlternateText="<%=labels.down %>" />
                    </ItemTemplate>
                </asp:TemplateField>
              </Columns>
        </asp:GridView>
        </div>
        
        <div style="padding-top:8px">
            <asp:CheckBox runat="server" ID="cbReportMistakes" />
            <span><%=labels.reportMistakesToService %></span>
        </div>
    </div>

    <div style="text-align: center; margin-bottom: 10px">
        <asp:Button runat="server" ID="btnSave" />
    </div>
      
</asp:Content>