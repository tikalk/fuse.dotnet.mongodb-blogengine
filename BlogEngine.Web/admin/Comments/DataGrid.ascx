<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DataGrid.ascx.cs" Inherits="admin_Comments_DataGrid" %>
<%@ Import Namespace="Resources"%>
<script language="javascript">
    function editComment(id) {
        window.scrollTo(0, 0);
        var width = document.documentElement.clientWidth + document.documentElement.scrollLeft;
        var height = document.documentElement.clientHeight + document.documentElement.scrollTop;
        var layer = document.createElement('div');

        layer.style.zIndex = 2;
        layer.id = 'layer';
        layer.style.position = 'absolute';
        layer.style.top = '0px';
        layer.style.left = '0px';
        layer.style.height = document.documentElement.scrollHeight + 'px';
        layer.style.width = width + 'px';
        layer.style.backgroundColor = 'black';
        layer.style.opacity = '.6';
        layer.style.filter += ("progid:DXImageTransform.Microsoft.Alpha(opacity=60)");
        document.body.style.position = 'static';
        document.body.appendChild(layer);

        var size = { 'height': 450, 'width': 750 };
        var iframe = document.createElement('iframe');

        iframe.name = 'Comment Editor';
        iframe.id = 'CommentEditor';
        iframe.src = 'Editor.aspx?id=' + id;
        iframe.style.height = size.height + 'px';
        iframe.style.width = size.width + 'px';
        iframe.style.position = 'fixed';
        iframe.style.zIndex = 3;
        iframe.style.backgroundColor = 'white';
        iframe.style.border = '4px solid silver';
        iframe.frameborder = '0';

        iframe.style.top = ((height + document.documentElement.scrollTop) / 2) - (size.height / 2) + 'px';
        iframe.style.left = (width / 2) - (size.width / 2) + 'px';

        document.body.appendChild(iframe);
    }
    
    function closeEditor(reload)
    {
      var v = document.getElementById('CommentEditor');
      var l = document.getElementById('layer');
      document.body.removeChild(document.getElementById('CommentEditor'));
      document.body.removeChild(document.getElementById('layer'));
      document.body.style.position = '';

      if (reload) {
          location.reload();
      }
    }

    function ConfirmDelete() {
      return confirm('<%=AreYouSureDelete()%>');
    }
  </script>
   
<div style="border:1px solid #f3f3f3">
<asp:GridView ID="gridComments" 
    BorderColor="#f8f8f8" 
    BorderStyle="solid" 
    BorderWidth="1px" 
    RowStyle-BorderWidth="0"
    RowStyle-BorderStyle="None"
    gridlines="None"
    datakeynames="Id"
    runat="server"  
    width="100%"
    AlternatingRowStyle-BackColor="#f8f8f8"
    AlternatingRowStyle-BorderColor="#f8f8f8" 
    HeaderStyle-BackColor="#F1F1F1"
    cellpadding="3"
    AutoGenerateColumns="False"
    AllowPaging="True"
    OnPageIndexChanging="gridView_PageIndexChanging"
    ShowFooter="true"
    AllowSorting="True"       
    onrowdatabound="gridComments_RowDataBound">
  <Columns>
    <asp:BoundField DataField = "Id" Visible="false" />     
    <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="20">
        <ItemTemplate>
             <asp:CheckBox ID="chkSelect" 
             Enabled='<%#HasNoChildren((Guid)DataBinder.Eval(Container.DataItem, "Id"))%>' 
             runat="server"/>
        </ItemTemplate>
    </asp:TemplateField> 
    <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="24">
        <ItemTemplate>
             <%#Gravatar(DataBinder.Eval(Container.DataItem, "Email").ToString(), DataBinder.Eval(Container.DataItem, "Author").ToString())%>
        </ItemTemplate>
    </asp:TemplateField>   
    <asp:BoundField HeaderText="<%$ Resources:labels, author %>" HeaderStyle-HorizontalAlign="Left" DataField="Author" HtmlEncode="false" DataFormatString="<a href='?author={0}'>{0}</a>" />
    <asp:BoundField HeaderText="<%$ Resources:labels, ip %>" HeaderStyle-HorizontalAlign="Left" DataField="IP" HtmlEncode="false" DataFormatString="<a href='?ip={0}'>{0}</a>" />          
	<asp:BoundField HeaderText="<%$ Resources:labels, email %>" HeaderStyle-HorizontalAlign="Left" DataField="Email" HtmlEncode="False" DataFormatString="<a href='mailto:{0}'>{0}</a>" />		
    <asp:TemplateField HeaderText="<%$ Resources:labels, website %>" HeaderStyle-HorizontalAlign="Left">
        <ItemTemplate>
           <span><%# GetWebsite(DataBinder.Eval(Container.DataItem, "Website"))%></span>
        </ItemTemplate>
    </asp:TemplateField>
    
    <asp:BoundField DataField="IsApproved" Visible="false" />                                    
    <asp:TemplateField HeaderText="<%$ Resources:labels, comment %>" HeaderStyle-HorizontalAlign="Left">
        <ItemTemplate>
           <asp:LinkButton ID="lnkEditComment" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Teaser").ToString()%>' OnClientClick='<%#GetEditHtml(DataBinder.Eval(Container.DataItem, "Id").ToString())%>' />
        </ItemTemplate>
    </asp:TemplateField>
    <asp:BoundField HeaderText="<%$ Resources:labels, date %>" DataField="DateCreated" DataFormatString="{0:dd-MMM-yyyy HH:mm}" HeaderStyle-HorizontalAlign="Left" />   
    <asp:TemplateField HeaderText="<%$ Resources:labels, moderator %>" HeaderStyle-HorizontalAlign="Left">
        <ItemTemplate>
             <asp:literal ID="ltModerator" 
             Text='<%#DataBinder.Eval(Container.DataItem, "ModeratedBy") + "" %>' 
             runat="server"/>
        </ItemTemplate>
    </asp:TemplateField>  
  </Columns>
  <pagersettings Mode="NumericFirstLast" position="Bottom" pagebuttoncount="20" />
  <PagerStyle HorizontalAlign="Center"/>
</asp:GridView>
</div>

<div style="text-align:center;padding-top:10px">
    <asp:Button ID="btnSelect" runat="server" Text="<%$ Resources:labels, select %>" OnClick="btnSelect_Click"/>
    <asp:Button ID="btnClear" runat="server" Text="<%$ Resources:labels, clear %>" OnClick="btnClear_Click"/>
    <asp:Button ID="btnAction" runat="server" Text="<%$ Resources:labels, approve %>" OnClick="btnAction_Click" />
    <asp:Button ID="btnDelete" runat="server" Text="<%$ Resources:labels, delete %>" OnClick="btnDelete_Click" OnClientClick="return ConfirmDelete();" />   
</div>

<div id="ErrorMsg" runat="server" style="color: Red; display: block;"></div>
<div id="InfoMsg" runat="server" style="color: Green; display: block;"></div>