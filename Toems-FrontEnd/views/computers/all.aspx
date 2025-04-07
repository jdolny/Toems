<%@ Page Title="" Language="C#" MasterPageFile="~/views/computers/computers.master" AutoEventWireup="true" CodeBehind="all.aspx.cs" Inherits="Toems_FrontEnd.views.computers.all" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>All</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
 Computers
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="DropDownActionsSub">
    <li><asp:LinkButton ID="btnArchive" runat="server" OnClick="btnArchive_OnClick" Text="Archive Selected" CssClass="main-action" /></li>
    <li> <asp:LinkButton ID="btnDelete" runat="server" OnClick="btnDelete_OnClick" Text="Delete Selected" /></li>

</asp:Content>





<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
   
    <script type="text/javascript">
        $(document).ready(function() {
            $('#all').addClass("nav-current");
            $('.categories').select2();
            $("[id*=gvComputers] td").hover(function() {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function() {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
        });

        function RefreshUpdatePanel() {
            __doPostBack('<%= txtSearch.ClientID %>', '');
        };

    </script>
       
    <div class="size-7 column">

        <asp:TextBox ID="txtSearch" runat="server" CssClass="rounded-search" OnTextChanged="search_Changed" onkeyup="RefreshUpdatePanel();"></asp:TextBox>
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

      <div class="size-11 column">
     <div class="custom-select">
        <asp:DropDownList ID="ddlDisabled" runat="server" CssClass="ddlist" AutoPostBack="True" OnSelectedIndexChanged="ddlDisabled_OnSelectedIndexChanged">
             <asp:ListItem></asp:ListItem>
        <asp:ListItem Selected="True">Any State</asp:ListItem>
            <asp:ListItem>Enabled</asp:ListItem>
            <asp:ListItem>Disabled</asp:ListItem>    
        </asp:DropDownList>
              </div>
          </div>
    
       <div class="size-13 column">
            <div class="custom-select">
        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="ddlist" AutoPostBack="True" OnSelectedIndexChanged="ddlStatus_OnSelectedIndexChanged">
        </asp:DropDownList>
                
                </div>
    </div>
   

    <br class="clear"/>
    <br/>
    <select class="categories display-none" name="categories" multiple="True" runat="server"  ID="selectCategory" style="width: 100%;" Visible="False"></select>   
    <br class="clear"/>
    <asp:Button runat="server" ID="CategorySubmit" Text="Apply Category Filter" CssClass="btn margin-top" OnClick="CategorySubmit_OnClick" Visible="False" />
    <br />
     <p class="total">
        <asp:Label ID="lblTotal" runat="server"></asp:Label>
    </p>

   

   
      <asp:UpdatePanel ID="Update" runat="server">
           <ContentTemplate>
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
            <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/computers/general.aspx?computerId={0}" Text="View" ItemStyle-CssClass="chkboxwidth"/>
            <asp:BoundField DataField="Id" HeaderText="computerID" SortExpression="computerID" Visible="False"/>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
               <asp:BoundField DataField="ProvisionStatus" HeaderText="Provision Status" SortExpression="ProvisionStatus" ItemStyle-CssClass="width_200"></asp:BoundField>
              <asp:BoundField DataField="IsAdSync" HeaderText="AD Sync" SortExpression="IsAdSync" ItemStyle-CssClass="width_200"></asp:BoundField>
              <asp:BoundField DataField="AdDisabled" HeaderText="AD Disabled" SortExpression="AdDisabled" ItemStyle-CssClass="width_200"></asp:BoundField>
             <asp:BoundField DataField="LastCheckinTime" HeaderText="Last Checkin" SortExpression="LastCheckinTime" ItemStyle-CssClass="width_200"></asp:BoundField>
             <asp:BoundField DataField="LastIp" HeaderText="Last Known Ip" SortExpression="LastIp" ItemStyle-CssClass="width_200" ></asp:BoundField>
               <asp:BoundField DataField="ClientVersion" HeaderText="Client Version" SortExpression="ClientVersion" ItemStyle-CssClass="width_200"></asp:BoundField> 
            <asp:BoundField DataField="ProvisionedTime" HeaderText="Provision Date" SortExpression="ProvisionedTime" ItemStyle-CssClass="width_200" ></asp:BoundField>
            <asp:TemplateField>                 
                <ItemTemplate>
                    <asp:LinkButton runat="server" ID="btnRestore" Text="Restore" OnClick="btnRestore_OnClick"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
         
            
         
        
        
        </Columns>
        <EmptyDataTemplate>
            No Computers Found
        </EmptyDataTemplate>
    </asp:GridView>
                </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="txtSearch" />
        </Triggers>
    </asp:UpdatePanel>

    <div id="confirmbox" class="confirm-box-outer">
        <div class="confirm-box-inner">
            <h4>
                <asp:Label ID="lblTitle" runat="server" Text="Delete The Selected Computers?"></asp:Label>
            </h4>
            <div class="confirm-box-btns">
                <asp:LinkButton ID="ConfirmButton" OnClick="ButtonConfirmDelete_Click" runat="server" Text="Yes" CssClass="confirm_yes"/>
                <asp:LinkButton ID="CancelButton" runat="server" Text="No" CssClass="confirm_no"/>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>The search all page displays every computer that Theopenem is aware of, regardless of it's status.  The most common status to lookup on this page is pre-provisioned and archived. </p>
<p>Filtering options include:</p>
<ul>
	<li><strong>Search by computer name</strong></li>
	<li><strong>By category</strong></li>
	<li><strong>By limit</strong></li>
	<li>By status</li>
	<li>By state - The state filter is used for AD synced computers, the AD sync will sync disabled computers also.  If a computer is not synced from Active Directory, it's state is always enabled.</li>
</ul>
</asp:Content>