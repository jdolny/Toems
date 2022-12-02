<%@ Page Title="" Language="C#" MasterPageFile="~/views/policies/policies.master" AutoEventWireup="true" CodeBehind="status.aspx.cs" Inherits="Toems_FrontEnd.views.policies.status" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>Active Policy Status</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
 Policies
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">

   

</asp:Content>



<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
    
    <script type="text/javascript">
        $(document).ready(function () {

            $('#status').addClass("nav-current");

            $("[id*=gvPolicies] td").hover(function() {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function() {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
        });

    </script>

 
   
   
   
    <asp:GridView ID="gvPolicies" runat="server" AllowSorting="True" DataKeyNames="PolicyId" OnSorting="gridView_Sorting" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>

            <asp:HyperLinkField DataNavigateUrlFields="PolicyId" DataNavigateUrlFormatString="~/views/policies/clienthistory.aspx?policyId={0}" Text="View" ItemStyle-CssClass="chkboxwidth"/>
            <asp:BoundField DataField="PolicyId" HeaderText="PolicyId" SortExpression="PolicyId" Visible="False"/>
            <asp:BoundField DataField="PolicyName" HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="SuccessCount" HeaderText="Success Count" SortExpression="SuccessCount" ItemStyle-CssClass="width_200"/>
             <asp:BoundField DataField="FailedCount" HeaderText="Failed Count" SortExpression="FailedCount" ItemStyle-CssClass="width_200"/>
             <asp:BoundField DataField="SkippedCount" HeaderText="Skipped Count" SortExpression="SkippedCount" ItemStyle-CssClass="width_200"/>
              <asp:BoundField DataField="Description" HeaderText="Description"  />
          
            
        </Columns>
        <EmptyDataTemplate>
            No Active Policies Found
        </EmptyDataTemplate>
    </asp:GridView>

  
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">

</asp:Content>
