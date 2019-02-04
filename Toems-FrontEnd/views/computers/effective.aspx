<%@ Page Title="" Language="C#" MasterPageFile="~/views/computers/computers.master" AutoEventWireup="true" CodeBehind="effective.aspx.cs" Inherits="Toems_FrontEnd.views.computers.effective" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li> <%= ComputerEntity.Name %></li>
    <li>Effective Policy</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    <%= ComputerEntity.Name %>
</asp:Content>


<asp:Content runat="server" ContentPlaceHolderID="DropDownActionsSub">

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" Runat="Server">
      <script type="text/javascript">
          $(document).ready(function () {
              $('#effective').addClass("nav-current");
          });
         
    </script>
     <div class="size-4 column">
        Client Action:
    </div>
    <div class="size-5 column">
        <div class="custom-select">
        <asp:DropDownList ID="ddlTrigger" runat="server" CssClass="ddlist" AutoPostBack="True" OnSelectedIndexChanged="ddlTrigger_OnSelectedIndexChanged"></asp:DropDownList>
            </div>
    </div>
    <br class="clear"/>

     <div class="size-4 column">
        Com Server:
    </div>
    <div class="size-5 column">
        <div class="custom-select">
        <asp:DropDownList ID="ddlComServer" runat="server" CssClass="ddlist" AutoPostBack="True" OnSelectedIndexChanged="ddlTrigger_OnSelectedIndexChanged"></asp:DropDownList>
            </div>
    </div>
    <br class="clear"/>

    
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
        </script>
    </div>

    
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>This page makes it easy to determine exactly which policies will run on that computer depending on the client action and com server.  The policy is displayed in JSON format representing exactly what is sent to the client.  It is important to note, this displays all policies that could run, the client makes the final decision on what to run based on the policy run history.</p>
</asp:Content>