<%@ Page Title="" Language="C#" MasterPageFile="~/views/images/profiles/profiles.master" AutoEventWireup="true" CodeBehind="scripts.aspx.cs" Inherits="Toems_FrontEnd.views.images.profiles.scripts" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
      <li>
        <a href="<%= ResolveUrl("~/views/images/profiles/general.aspx") %>?imageId=<%= ImageEntity.Id %>&profileId=<%= ImageProfile.Id %>&sub=profiles"><%= ImageProfile.Name %></a>
    </li>
 <li>Scripts</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
     <%= ImageProfile.Name %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="btnUpdate" runat="server" Text="Update Profile" OnClick="btnUpdate_Click" CssClass="main-action"/></li>
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#scripts').addClass("nav-current");
           
 $("[id*=gvScripts] td").hover(function() {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function() {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
        });
    </script>
    <asp:GridView ID="gvScripts" runat="server" AllowSorting="True" DataKeyNames="Id" OnSorting="gridView_Sorting" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:TemplateField>
                <HeaderStyle CssClass="width_200"></HeaderStyle>
                <ItemStyle CssClass="width_200"></ItemStyle>
                <HeaderTemplate>
                    Run When
                </HeaderTemplate>
                <ItemTemplate>
                      <asp:DropDownList ID="ddlRunWhen" runat="server" CssClass="ddlist"></asp:DropDownList>
                </ItemTemplate>
            </asp:TemplateField>
           
            <asp:HyperLinkField Target="_blank" DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/modules/scriptmodules/general.aspx?scriptModuleId={0}" Text="View" ItemStyle-CssClass="chkboxwidth"/>
            <asp:TemplateField ItemStyle-CssClass="width_100" HeaderText="Priority" SortExpression="Priority">
                <ItemTemplate>

                    <asp:TextBox ID="txtPriority" runat="server" CssClass="textbox height_18"/>

                </ItemTemplate>
            </asp:TemplateField>

            <asp:BoundField DataField="Id" HeaderText="computerID" SortExpression="computerID" Visible="False"/>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name"></asp:BoundField>


        </Columns>
        <EmptyDataTemplate>
            No Scripts Found
        </EmptyDataTemplate>
    </asp:GridView>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
   This view allows you to set any custom scripts you want to run during the imaging process. They are only applied to deploy or multicast tasks.  Scripts are only assigned to the image profile here.  To create
    the script select Modules->Script Modules.  The priority can be used to control the order if multiple scripts are assigned.  Lower numbers run first.  You can also select when to run the script in the imaging
    process.  It can be before imaging occurs, or after imaging.  After imaging is broken down into before file copy and after file copy.  If you are using a file copy module, you may want a script to run before
    or after that.  If you are not using a file copy module, you can select either option to run the script after imaging.
</asp:Content>
