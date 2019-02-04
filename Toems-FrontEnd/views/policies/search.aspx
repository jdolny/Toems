<%@ Page Title="" Language="C#" MasterPageFile="~/views/policies/policies.master" AutoEventWireup="true" CodeBehind="search.aspx.cs" Inherits="Toems_FrontEnd.views.policies.search" EnableEventValidation="false" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>Search</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
 Policies
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">

     <li><asp:LinkButton ID="btnArchive" runat="server" OnClick="btnArchive_OnClick" Text="Archive Selected" CssClass="main-action" /></li>
    <li><asp:LinkButton ID="btnDelete" runat="server" OnClick="btnDelete_OnClick" Text="Delete Selected" /></li>


</asp:Content>



<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
    
    <script type="text/javascript">
        $(document).ready(function () {

            $('.categories').select2();

            $('#search').addClass("nav-current");

            $("[id*=gvPolicies] td").hover(function() {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function() {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
        });

    </script>

 
    <div class="size-7 column">

        <asp:TextBox ID="txtSearch" runat="server" CssClass="rounded-search" OnTextChanged="search_Changed"></asp:TextBox>
       
    </div>  
    
   
   

    <div class="size-11 column">
        <div class="custom-select">
        <asp:DropDownList runat="server" ID="ddlLimit" AutoPostBack="True" OnSelectedIndexChanged="ddl_OnSelectedIndexChanged" CssClass="ddlist">
            <asp:ListItem></asp:ListItem>
            <asp:ListItem>25</asp:ListItem>
            <asp:ListItem>100</asp:ListItem>
            <asp:ListItem Selected="True">250</asp:ListItem>
            <asp:ListItem >500</asp:ListItem>
            <asp:ListItem>1000</asp:ListItem>
            <asp:ListItem>5000</asp:ListItem>
            <asp:ListItem>All</asp:ListItem>
        </asp:DropDownList>
            </div>
    </div>
    <div class="size-11 column">
        <div class="custom-select">
            <asp:DropDownList runat="server" ID="ddlCatType" AutoPostBack="True" CssClass="ddlist" OnSelectedIndexChanged="ddlCatType_OnSelectedIndexChanged">
                <asp:ListItem></asp:ListItem> 
                <asp:ListItem Selected="True">Any Category</asp:ListItem>
                <asp:ListItem>And Category</asp:ListItem>
                <asp:ListItem>Or Category</asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>
    <br class="clear"/>
   
    <select class="categories display-none" name="categories" multiple="True" runat="server"  ID="selectCategory" style="width: 100%;" Visible="False"></select>   
    <br class="clear"/>
    <asp:Button runat="server" ID="CategorySubmit" Text="Apply Category Filter" CssClass="btn margin-top" OnClick="CategorySubmit_OnClick" Visible="False" />
    <br />
       <p class="total">
        <asp:Label ID="lblTotal" runat="server"></asp:Label>
    </p>
    
   
    <asp:GridView ID="gvPolicies" runat="server" AllowSorting="True" DataKeyNames="Id" OnRowDataBound="gvPolicies_OnRowDataBound" OnSorting="gridView_Sorting" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>

            <asp:TemplateField>

                <ItemStyle CssClass="chkboxwidth"></ItemStyle>
                <HeaderTemplate>
                    <asp:CheckBox ID="chkSelectAll" runat="server" AutoPostBack="True" OnCheckedChanged="chkSelectAll_CheckedChanged"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkSelector" runat="server"/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/policies/general.aspx?policyId={0}" Text="View" ItemStyle-CssClass="chkboxwidth"/>
            <asp:BoundField DataField="Id" HeaderText="computerID" SortExpression="computerID" Visible="False"/>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Trigger" HeaderText="Trigger" SortExpression="Trigger" ItemStyle-CssClass="width_200"/>
              <asp:BoundField DataField="Frequency" HeaderText="Frequency" SortExpression="Frequency" ItemStyle-CssClass="width_200"/>
            <asp:TemplateField ItemStyle-CssClass="width_200" HeaderText="Active">
                 
                <ItemTemplate>
                  <asp:Label runat="server" Id="lblActive"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
             <asp:TemplateField ItemStyle-CssClass="chkboxwidth">
                <ItemTemplate>
                    <asp:LinkButton ID="btnActive" runat="server" OnClick="btnActive_OnClick" Text="Activate" CausesValidation="False" />
                </ItemTemplate>
            </asp:TemplateField>
              <asp:TemplateField >
                <ItemTemplate>
                    <asp:LinkButton ID="btnDeactive" runat="server" OnClick="btnDeactive_OnClick" Text="Deactivate"/>
                </ItemTemplate>
            </asp:TemplateField>
                <asp:TemplateField >
                <ItemTemplate>
                    <asp:LinkButton ID="btnClone" runat="server" OnClick="btnClone_OnClick" Text="Clone"/>
                </ItemTemplate>
            </asp:TemplateField>
            
        </Columns>
        <EmptyDataTemplate>
            No Policies Found
        </EmptyDataTemplate>
    </asp:GridView>

    <div id="confirmbox" class="confirm-box-outer">
        <div class="confirm-box-inner">
            <h4>
                <asp:Label ID="lblTitle" runat="server" Text="Delete The Selected Policies?"></asp:Label>
            </h4>
            <div class="confirm-box-btns">
                <asp:LinkButton ID="ConfirmButton" OnClick="ButtonConfirmDelete_Click" runat="server" Text="Yes" CssClass="confirm_yes"/>
                <asp:LinkButton ID="CancelButton" runat="server" Text="No" CssClass="confirm_no"/>
            </div>
        </div>
    </div>
    
     <div id="confirmbox2" class="confirm-box-outer">
        <div class="confirm-box-inner">
            <h4>
                <asp:Label ID="lblConfirm2" runat="server" Text="This Policy Has Changed Since The Last Activation.  Should The Policy Run Again On Clients That Have Already Run This Policy?"></asp:Label>
            </h4>
            <div class="confirm-box-btns">
                <asp:LinkButton ID="btnAll" OnClick="btnAll_OnClick" runat="server" Text="Yes" CssClass="confirm_yes"/>
                <asp:LinkButton ID="btnNewOnly" OnClick="btnNewOnly_OnClick" runat="server" Text="No" CssClass="confirm_no"/>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>The search page allows you to view, delete, archive, activate, deactivate, and clone existing policies.  For quick reference, all active policies are highlighted in green. When a policy is no longer needed, best practice is to archive the policy. Archiving a policy keeps all historical data about the policy and if the policy is needed again at a later date, it can easily be restored. Filtering options include:</p>
<ul>
	<li><strong>Search by policy name</strong></li>
	<li><strong>By category</strong></li>
	<li><strong>By limit</strong></li>
</ul>
</asp:Content>
