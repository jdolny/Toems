<%@ Page Title="" Language="C#" MasterPageFile="~/theopenem/views/admin/toec/toec.master" AutoEventWireup="true" CodeBehind="createdeployjob.aspx.cs" Inherits="Toems_FrontEnd.views.admin.toec.createdeployjob" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TopBreadCrumbSub2" runat="server">
     <li><a href="<%= ResolveUrl("~/views/admin/toec/createdeployjob.aspx") %>?level=2">Create Deploy Job</a></li>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SubNavTitle_Sub2" runat="server">
    Create Deploy Job
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DropDownActionsSub2" runat="server">

     <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_Click" Text="Create Deploy Job" CssClass="main-action" /></li>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SubContent2" runat="server">
     <script type="text/javascript">
        $(document).ready(function() {
            $('#createdeployjob').addClass("nav-current");
        });
     </script>

     <div class="size-4 column">
        Job Name:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtName" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>

     <div class="size-4 column">
        User Name:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtUsername" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Password:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtPassword" runat="server" CssClass="textbox" TextMode="Password"></asp:TextBox>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Domain (NetBIOS Format):
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtDomain" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Job Type:
    </div>
    <div class="size-5 column">
       
        <asp:DropDownList ID="ddlJobType" runat="server" CssClass="ddlist"></asp:DropDownList>
  
            </div>
    <br class="clear"/>
      <div class="size-4 column">
        Run Mode:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlRunMode" runat="server" CssClass="ddlist"></asp:DropDownList> 
            </div>
    <br class="clear"/>

       <div class="size-4 column">
        Target List:
    </div>
    <div class="size-5 column">
        <div class="custom-select">
        <asp:DropDownList ID="ddlTargetList" runat="server" CssClass="ddlist" ></asp:DropDownList>
            </div>
    </div>
    <br class="clear"/>

       <div class="size-4 column">
        Exception List:
    </div>
    <div class="size-5 column">
        <div class="custom-select">
        <asp:DropDownList ID="ddlExceptionList" runat="server" CssClass="ddlist" ></asp:DropDownList>
            </div>
    </div>
  
    <br class="clear"/>
      <div class="size-4 column">
        Job Enabled:
    </div>
     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkJobEnabled" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkJobEnabled">Toggle</label>
        </div>
     <br class="clear"/>

</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="subsubHelp" runat="server">
   
</asp:Content>