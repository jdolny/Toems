<%@ Page Title="" Language="C#" MasterPageFile="~/views/modules/sysprepmodules/sysprepmodule.master" AutoEventWireup="true" CodeBehind="create.aspx.cs"ValidateRequest="False" Inherits="Toems_FrontEnd.views.modules.sysprepmodules.create" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li>New</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
 Sysprep Modules
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="btnAdd" runat="server" OnClick="btnAdd_OnClick" Text="Add Module" CssClass="main-action" OnClientClick="update_click()" /></li>
</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#create').addClass("nav-current");
        });
    </script>
    <div class="size-4 column">
        Display Name
    </div>

    <div class="size-5 column">
        <asp:TextBox ID="txtDisplayName" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>
    </div>
    <br class="clear"/>
      <div class="size-4 column">
        Description
    </div>

    <div class="size-5 column">
        <asp:TextBox ID="txtDescription" runat="server" CssClass="descbox" TextMode="MultiLine"></asp:TextBox>
    </div>
     <br class="clear" />


       <div class="size-4 column">
        Opening Tag
    </div>

    <div class="size-5 column">
        <asp:TextBox ID="txtOpen" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>
    </div>
    <br class="clear"/>

       <div class="size-4 column">
        Closing Tag
    </div>

    <div class="size-5 column">
        <asp:TextBox ID="txtClose" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>
    </div>
    <br class="clear"/>



    
    <div class="size-4 column">
        Contents
    </div>
    <br class="clear"/>
    <div id="aceEditor" runat="server">
        <br class="clear"/>
        <pre id="editor" class="editor height_1200"></pre>
        <asp:HiddenField ID="scriptEditor" runat="server"/>


        <script>

            var editor = ace.edit("editor");
            editor.session.setValue($('#<%= scriptEditor.ClientID %>').val());

            editor.setTheme("ace/theme/idle_fingers");
            editor.getSession().setMode("ace/mode/sh");
            editor.setOption("showPrintMargin", false);
            editor.session.setUseWrapMode(true);
            editor.session.setWrapLimitRange(120, 120);


            function update_click() {
                var editor = ace.edit("editor");
                $('#<%= scriptEditor.ClientID %>').val(editor.session.getValue());
            }

        </script>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">

</asp:Content>
