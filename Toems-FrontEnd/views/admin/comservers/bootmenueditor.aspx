<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/comservers/comservers.master" AutoEventWireup="true" CodeBehind="bootmenueditor.aspx.cs" Inherits="Toems_FrontEnd.views.admin.comservers.bootmenueditor" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li><a href="<%= ResolveUrl("~/views/admin/comservers/comservers.aspx") %>?level=2">Com Servers</a></li>
    <li><%=ComServer.DisplayName %></li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
<%=ComServer.DisplayName %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
     <li><asp:LinkButton ID="btnSaveEditor" runat="server" Text="Save Changes" OnClick="saveEditor_Click" OnClientClick="update_click()" CssClass="main-action"/></li>

</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#bootmenu').addClass("nav-current");
        });
      
    </script>


     <div id="proxyEditor" runat="server" visible="false">
        <div class="size-4 column">
            Select A Menu To Edit:
        </div>
        <div class="size-4 column">
            <asp:DropDownList ID="ddlEditProxyType" runat="server" CssClass="ddlist" OnSelectedIndexChanged="EditProxy_Changed" AutoPostBack="true">
                <asp:ListItem>bios</asp:ListItem>
                <asp:ListItem>efi32</asp:ListItem>
                <asp:ListItem>efi64</asp:ListItem>
            </asp:DropDownList>
        </div>
        <br class="clear"/>
    </div>
    


    <asp:Label ID="lblFileName1" runat="server"></asp:Label>

    <br class="clear"/>
    <div id="srvEdit" runat="server">
        <pre id="editor" class="editor height_1200"></pre>
    </div>
    <asp:HiddenField ID="scriptEditorText" runat="server"/>


    <script>
        var editor = ace.edit("editor");
        editor.session.setValue($('#<%= scriptEditorText.ClientID %>').val());
        editor.setTheme("ace/theme/idle_fingers");
        editor.getSession().setMode("ace/mode/sh");
        editor.setOption("showPrintMargin", false);
        editor.session.setUseWrapMode(true);
        editor.session.setWrapLimitRange(120, 120);
        function update_click() {
            var editor = ace.edit("editor");
            $('#<%= scriptEditorText.ClientID %>').val(editor.session.getValue());
        }
    </script>
  
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">

</asp:Content>

