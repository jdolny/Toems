<%@ Page Title="" Language="C#" MasterPageFile="~/views/modules/sysprepmodules/sysprepmodule.master" AutoEventWireup="true" CodeBehind="usages.aspx.cs" Inherits="Toems_FrontEnd.views.modules.sysprepmodules.usages" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li><%= SysprepModule.Name %></li>
    <li>Usages</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
<%= SysprepModule.Name %>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#usages').addClass("nav-current");
            $("[id*=gvImages] td").hover(function () {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function () {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
        });
    </script>
    
    <p class="total">
        <asp:Label ID="lblTotal" runat="server"></asp:Label>
    </p>
    
    <br class="clear"/>
    <div class="size-4 column">
        Usage Type:
    </div>
    <div class="size-setting column">
        <div class="custom-select">
        <asp:DropDownList ID="ddlUtil" runat="server" CssClass="ddlist" AutoPostBack="True" OnSelectedIndexChanged="ddlUtil_OnSelectedIndexChanged">
            <asp:ListItem>Images</asp:ListItem>
        </asp:DropDownList>
            </div>
    </div>
    <br class="clear"/>


     <asp:GridView ID="gvImages" runat="server"  DataKeyNames="Id"  AutoGenerateColumns="False" CssClass="Gridview extraPad" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="Id"  Visible="False"/>
             <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/images/general.aspx?imageId={0}" Text="View" ItemStyle-CssClass="chkboxwidth" Target="_blank"/>
            <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-CssClass="width_200" ></asp:BoundField>

        </Columns>
        <EmptyDataTemplate>
            No Images Found
        </EmptyDataTemplate>
         </asp:GridView>

   
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>Usages provide an easy way to determine which Policies, Groups, and Computers the module is currently assigned to.</p>
</asp:Content>
