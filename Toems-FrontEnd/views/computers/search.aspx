<%@ Page Title="" Language="C#" MasterPageFile="~/views/computers/computers.master" AutoEventWireup="true" CodeBehind="search.aspx.cs" Inherits="Toems_FrontEnd.views.computers.search" Async="true" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>Active</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
 Computers
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="DropDownActionsSub">
    <li><asp:LinkButton ID="btnArchive" runat="server" OnClick="btnArchive_OnClick" Text="Archive Selected" CssClass="main-action" /></li>
    <li> <asp:LinkButton ID="btnDelete" runat="server" OnClick="btnDelete_OnClick" Text="Delete Selected"  /></li>

</asp:Content>



<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
     <script type="text/javascript">
        $(document).ready(function() {
            $('#search').addClass("nav-current");
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
         <asp:TextBox ID="txtSearch" runat="server" CssClass="rounded-search" OnTextChanged="search_Changed" ></asp:TextBox>
      
    </div>
    

    <div class="size-11 column">

        <div class="custom-select">
        <asp:DropDownList runat="server" ID="ddlLimit" AutoPostBack="True" OnSelectedIndexChanged="ddl_OnSelectedIndexChanged" >
             <asp:ListItem></asp:ListItem>
            <asp:ListItem>25</asp:ListItem>
            <asp:ListItem Selected="True">100</asp:ListItem>
            <asp:ListItem>250</asp:ListItem>
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
    <br/>
    <select class="categories display-none" name="categories" multiple="True" runat="server"  ID="selectCategory" style="width: 100%;" Visible="False"></select>   
    <br class="clear"/>
    <asp:Button runat="server" ID="CategorySubmit" Text="Apply Category Filter" CssClass="btn margin-top" OnClick="CategorySubmit_OnClick" Visible="False" />
    <br />
     <p class="total">
        <asp:Label ID="lblTotal" runat="server"></asp:Label>
    </p>
    <br class="clear"/>


           <asp:GridView ID="gvComputers" runat="server" AllowSorting="True" DataKeyNames="Id" OnSorting="gridView_Sorting" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>

            <asp:TemplateField>
                 <ItemStyle CssClass="chkboxwidth"></ItemStyle>

          
                <ItemTemplate>
                    <asp:CheckBox ID="chkSelector" runat="server"/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/computers/general.aspx?computerId={0}" Text="View" ItemStyle-CssClass="chkboxwidth"/>
            <asp:BoundField DataField="Id" HeaderText="computerID" SortExpression="computerID" Visible="False"/>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
     
        </Columns>
        <EmptyDataTemplate>
            No Computers Found
        </EmptyDataTemplate>
    </asp:GridView>

   
   

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
      <p>The search active page displays all computers that have been provisioned / checked into Theopenem. Filtering options include:</p>
<ul>
	<li><strong>Search by computer name</strong></li>
	<li><strong>By category</strong></li>
	<li><strong>By limit</strong></li>
</ul>


</asp:Content>
