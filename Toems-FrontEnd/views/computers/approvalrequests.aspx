<%@ Page Title="" Language="C#" MasterPageFile="~/views/computers/computers.master" AutoEventWireup="true" CodeBehind="approvalrequests.aspx.cs" Inherits="Toems_FrontEnd.views.computers.approvalrequests" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>Approval Requests</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
 Computers
</asp:Content>

<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="btnApprove" runat="server" OnClick="btnApprove_OnClick" Text="Approve Request" CssClass="main-action" /></li>
    <li><asp:LinkButton ID="btnDeny" runat="server" OnClick="btnDeny_OnClick" Text="Deny Request" /></li>

</asp:Content>




<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#approval').addClass("nav-current");

            $("[id*=gvComputers] td").hover(function() {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function() {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
        });

    </script>

    <p class="total">
        <asp:Label ID="lblTotal" runat="server"></asp:Label>
    </p>
    <div class="size-7 column">

        <asp:TextBox ID="txtSearch" runat="server" CssClass="rounded-search" OnTextChanged="search_Changed"></asp:TextBox>
    </div>

    <br class="clear"/>


  
    <asp:GridView ID="gvComputers" runat="server" AllowSorting="True" DataKeyNames="Id" OnSorting="gridView_Sorting" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
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
            <asp:BoundField DataField="Id" HeaderText="computerID" SortExpression="computerID" Visible="False"/>
            <asp:BoundField DataField="ComputerName" HeaderText="Computer" SortExpression="ComputerName" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="IpAddress" HeaderText="IP Address" SortExpression="IpAddress" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:TemplateField HeaderText="Request Time" SortExpression="RequestTime">
                    <ItemTemplate>
                        <asp:Label ID="lblLocalTime" runat="server" Text='<%# Convert.ToDateTime(Eval("RequestTime")).ToLocalTime() %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
             <asp:BoundField DataField="InstallationId" HeaderText="Install ID"></asp:BoundField>
          
        
        
        </Columns>
        <EmptyDataTemplate>
            No Approval Requests Found
        </EmptyDataTemplate>
    </asp:GridView>

    <div id="confirmbox" class="confirm-box-outer">
        <div class="confirm-box-inner">
            <h4>
                <asp:Label ID="lblTitle" runat="server"></asp:Label>
            </h4>
            <div class="confirm-box-btns">
                <asp:LinkButton ID="ConfirmButton" OnClick="ButtonConfirmDelete_Click" runat="server" Text="Yes" CssClass="confirm_yes"/>
                <asp:LinkButton ID="CancelButton" runat="server" Text="No" CssClass="confirm_no"/>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>The approval requests page is related to the security settings, new computer provision approval required and Pre Provision Approval Required, found in Admin Settings->Security.  If either of those settings are enabled, computers cannot be provisioned until someone manually approves them.  When a client computer tries to provision itself, it will stop and create an approval request.  This page is used to either approve or deny the request.  After a computer is approved it will automatically complete the provision process.  A report is emailed twice a day if any approvals are waiting.</p>
</asp:Content>
