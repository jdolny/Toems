<%@ Page Title="" Language="C#" MasterPageFile="~/views/assets/assetgroups/assetgroups.master" AutoEventWireup="true" CodeBehind="newmembers.aspx.cs" Inherits="Toems_FrontEnd.views.assets.assetgroups.newmembers" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li> <%=AssetGroup.Name %></li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
    <%=AssetGroup.Name %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="btnUpdate_OnClick" Text="Add Group Members" CssClass="main-action"/></li>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#addmembers').addClass("nav-current");
            $('.categories').select2();
        });
    </script>
 
    <div class="size-7 column">
        <asp:TextBox ID="txtSearch" runat="server" CssClass="rounded-search" OnTextChanged="search_Changed"></asp:TextBox>
    </div>
    <div class="size-11 column">

        <div class="custom-select">
            <asp:DropDownList runat="server" ID="ddlLimit" AutoPostBack="True" OnSelectedIndexChanged="ddl_OnSelectedIndexChanged" >
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
    <div class="size-13 column">
        <div class="custom-select">
            <asp:DropDownList ID="ddlAssetType" runat="server" CssClass="ddlist" AutoPostBack="True" OnSelectedIndexChanged="ddlAssetType_OnSelectedIndexChanged">
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


    <asp:GridView ID="gvAssets" runat="server"  DataKeyNames="AssetId"  AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:TemplateField>
                <HeaderStyle CssClass="chkboxwidth"></HeaderStyle>
                <ItemStyle CssClass="chkboxwidth"></ItemStyle>
                <HeaderTemplate>
                    <asp:CheckBox ID="chkSelectAll" runat="server" AutoPostBack="True" OnCheckedChanged="chkSelectAll_CheckedChanged"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkSelector" runat="server"/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:HyperLinkField DataNavigateUrlFields="AssetId" DataNavigateUrlFormatString="~/views/assets/customassets/edit.aspx?level=2&assetId={0}" Target="_default"  Text="View" ItemStyle-CssClass="chkboxwidth"/>
            <asp:BoundField DataField="AssetId" HeaderText="Id" SortExpression="Id" Visible="False"/>
            <asp:BoundField DataField="Name" HeaderText="Display Name" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="AssetType" HeaderText="Asset Type" ></asp:BoundField>
         

        </Columns>
        <EmptyDataTemplate>
            No Assets Found
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>Displays all available Assets that may be added to the group.</p>
</asp:Content>
