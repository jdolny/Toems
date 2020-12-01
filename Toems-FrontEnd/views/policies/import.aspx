<%@ Page Title="" Language="C#" MasterPageFile="~/views/policies/policies.master" AutoEventWireup="true" CodeBehind="import.aspx.cs" Inherits="Toems_FrontEnd.views.policies.import" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>Import</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
 Policies
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
     <li><asp:LinkButton ID="btnImport" runat="server" OnClick="btnImport_OnClick" Text="Import Policy" CssClass="main-action"/></li>
</asp:Content>



<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
    
    <script type="text/javascript">
        $(document).ready(function () {

            $('#import').addClass("nav-current");

        });

    </script>
    <asp:FileUpload runat="server" ID="FileImport"/>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>The import page allows you to import Policies that have already been created and exported by someone.  An imported policy will create the policy 
        and all required modules for that policy in a deactivated state.  After the import is complete you can review the policy and activate when ready.  
        An official repository of Theopenem approved policies can be found at https://github.com/theopenem/Policy-Templates.  It is highly encouraged for users to contribute their policies in order to help grow the repo.</p>
</asp:Content>
