<%@ Page Title="" Language="C#" MasterPageFile="~/views/images/profiles/profiles.master" AutoEventWireup="true" CodeBehind="sysprep.aspx.cs" Inherits="Toems_FrontEnd.views.images.profiles.sysprep" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
      <li>
        <a href="<%= ResolveUrl("~/views/images/profiles/general.aspx") %>?imageId=<%= ImageEntity.Id %>&profileId=<%= ImageProfile.Id %>&sub=profiles"><%= ImageProfile.Name %></a>
    </li>
 <li>Sysprep Modifications</li>
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
            $('#sysprep').addClass("nav-current");
           
 $("[id*=gvSysprep] td").hover(function() {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function() {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
        });
    </script>
    <asp:GridView ID="gvSysprep" runat="server" AllowSorting="True" DataKeyNames="Id" OnSorting="gridView_Sorting" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:TemplateField>
                <HeaderStyle CssClass="chkboxwidth"></HeaderStyle>
                <ItemStyle CssClass="chkboxwidth"></ItemStyle>
                <HeaderTemplate>
                    Enabled
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkEnabled" runat="server"/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:HyperLinkField Target="_blank" DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/modules/sysprepmodules/general.aspx?sysprepModuleId={0}" Text="View" ItemStyle-CssClass="chkboxwidth"/>
            <asp:BoundField DataField="Id" Visible="False"/>
            <asp:TemplateField ItemStyle-CssClass="width_100" HeaderText="Priority" SortExpression="Priority">
                <ItemTemplate>

                    <asp:TextBox ID="txtPriority" runat="server" CssClass="textbox height_18"/>

                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name"></asp:BoundField>


        </Columns>
        <EmptyDataTemplate>
            No Sysprep Tags Found
        </EmptyDataTemplate>
    </asp:GridView>


</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
  This view allows you to set any sysprep modifications you want to run during the imaging process. They are only applied to deploy or multicast tasks.  Sysprep mods are only assigned to the image profile here.  To create
    the sysprep mod select Modules->Sysprep Modules.  The priority can be used to control the order if multiple syspreps are assigned.  Lower numbers run first.
</asp:Content>
