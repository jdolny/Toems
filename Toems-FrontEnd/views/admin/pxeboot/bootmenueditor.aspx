<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/pxeboot/pxeboot.master" AutoEventWireup="true" CodeBehind="bootmenueditor.aspx.cs" Inherits="Toems_FrontEnd.views.admin.pxeboot.bootmenueditor" %>
<asp:Content ID="Content3" ContentPlaceHolderID="TopBreadCrumbSub2" runat="server">
    <li><a href="<%= ResolveUrl("~/views/admin/pxeboot/bootmenueditor.aspx") %>?level=2">PXE Boot Menu Editor</a></li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
    PXE Boot Menu Editor
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
     <li><asp:LinkButton ID="btnSaveEditor" runat="server" Text="Save Changes" OnClick="saveEditor_Click" OnClientClick="update_click()" CssClass="main-action"/></li>

</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#bootmenueditor').addClass("nav-current");
        });
      
    </script>

    <div class="size-4 column">
        Com Server:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlComServer" runat="server" CssClass="ddlist" AutoPostBack="true" OnSelectedIndexChanged="ddlComServer_SelectedIndexChanged"/>
    </div>
    <br class="clear" />


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
    
    <br /><br />

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


