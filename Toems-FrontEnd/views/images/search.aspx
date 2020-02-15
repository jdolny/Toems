<%@ Page Title="" Language="C#" MasterPageFile="~/views/images/images.master" AutoEventWireup="true" CodeBehind="search.aspx.cs" Inherits="Toems_FrontEnd.views.images.search" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>Search</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" runat="Server">
    <li><asp:LinkButton ID="btnDelete" runat="server" Text="Delete Selected Images" OnClick="btnDelete_Click" CssClass="main-action"/></li>
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" runat="Server">
  <script type="text/javascript">
        $(document).ready(function() {
            $('#search').addClass("nav-current");
            $("[id*=gvImages] td").hover(function() {
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
    <br/>
    <select class="categories display-none" name="categories" multiple="True" runat="server"  ID="selectCategory" style="width: 100%;" Visible="False"></select>   
    <br class="clear"/>
    <asp:Button runat="server" ID="CategorySubmit" Text="Apply Category Filter" CssClass="btn margin-top" OnClick="CategorySubmit_OnClick" Visible="False" />
    <br />
      <p class="total">
        <asp:Label ID="lblTotal" runat="server"></asp:Label>
    </p>

    <asp:GridView ID="gvImages" runat="server" AllowSorting="True" AutoGenerateColumns="False" OnSorting="gridView_Sorting" DataKeyNames="Id" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
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
            <asp:TemplateField ShowHeader="False" ItemStyle-CssClass="width_30 mobi-hide-smaller" HeaderStyle-CssClass="mobi-hide-smaller">
                <ItemTemplate>
                    <div style="width: 0">
                        <asp:LinkButton ID="btnHds" runat="server" CausesValidation="false" CommandName="" Text="+" OnClick="btnHds_Click"></asp:LinkButton>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/images/general.aspx?imageId={0}" Text="View" ItemStyle-CssClass="chkboxwidth"/>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:HiddenField ID="HiddenID" runat="server" Value='<%# Bind("Id") %>'/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Id" HeaderText="imageID" SortExpression="Id" Visible="False"/>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="width_200"/>
            <asp:TemplateField ShowHeader="True" HeaderText="Size On Server" ItemStyle-CssClass="width_200 mobi-hide-smaller" HeaderStyle-CssClass="mobi-hide-smaller">
                <ItemTemplate>
                    <asp:Label ID="lblSize" runat="server" CausesValidation="false" CssClass="lbl_file"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Environment" HeaderText="Imaging Environment" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="LastUsed" HeaderText="Last Used" SortExpression="LastUsed" ItemStyle-CssClass="width_200"/>

            <asp:TemplateField>
                <ItemTemplate>
                    <tr>
                        <td id="tdHds" runat="server" visible="false" colspan="900">
                            <asp:GridView ID="gvHDs" AutoGenerateColumns="false" runat="server" CssClass="Gridview gv_parts hdlist" ShowHeader="false" Visible="false" AlternatingRowStyle-CssClass="alt">
                                <Columns>
                                    <asp:BoundField DataField="name" HeaderText="#" ItemStyle-CssClass="width_100"></asp:BoundField>
                                    <asp:TemplateField ShowHeader="True" HeaderText="Server Size" ItemStyle-CssClass="mobi-hide-smaller" HeaderStyle-CssClass="mobi-hide-smaller">
                                        <ItemTemplate>
                                            <asp:Label ID="lblHDSize" runat="server" CausesValidation="false" CssClass="lbl_file"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>


                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:TemplateField>


        </Columns>
        <EmptyDataTemplate>
            No Images Found
        </EmptyDataTemplate>
    </asp:GridView>

    <div id="confirmbox" class="confirm-box-outer">
        <div class="confirm-box-inner">
            <h4>
                <asp:Label ID="lblTitle" runat="server" Text="Delete The Selected Images?" CssClass="modaltitle"></asp:Label>
            </h4>
            <div class="confirm-box-btns">
                <asp:LinkButton ID="ConfirmButton" OnClick="ButtonConfirmDelete_Click" runat="server" Text="Yes" CssClass="confirm_yes"/>
                <asp:LinkButton ID="CancelButton" runat="server" Text="No" CssClass="confirm_no"/>
            </div>
        </div>
    </div>
       

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>Specifies if the storage path is local to this server or on a remote SMB share.  The local type can only be used if only a single server is used for all Toec Api's, Toems Api's, and Toems Front Ends.</p>
<h5><span style="color: #ff9900;">Storage Path:</span></h5>
<p>The path to the storage directory.  If the storage type is local, the path should be a local drive, such as c:\toems-local-storage.  If the storage type is SMB, the path must be a UNC path, such as \\server\toems-remote-storage.</p>
<h5><span style="color: #ff9900;">Username:</span></h5>
<p>Only available for a storage type of SMB.  This is the username used to connect to the share.</p>
<h5><span style="color: #ff9900;">Password:</span></h5>
<p>Only available for a storage type of SMB.  This is the password used to connect to the share.</p>
<h5><span style="color: #ff9900;">Domain:</span></h5>
<p>Only available for a storage type of SMB.  This is the domain used to connect to the share.</p>
</asp:Content>
