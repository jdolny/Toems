<%@ Page Title="" Language="C#" MasterPageFile="~/views/images/profiles/profiles.master" AutoEventWireup="true" CodeBehind="filecopy.aspx.cs" Inherits="Toems_FrontEnd.views.images.profiles.filecopy" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
      <li>
        <a href="<%= ResolveUrl("~/views/images/profiles/general.aspx") %>?imageId=<%= ImageEntity.Id %>&profileId=<%= ImageProfile.Id %>&sub=profiles"><%= ImageProfile.Name %></a>
    </li>
 <li>File Copy</li>
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
            $('#filecopy').addClass("nav-current");
           
 $("[id*=gvFile] td").hover(function() {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function() {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
        });
    </script>
    <asp:GridView ID="gvFile" runat="server" AllowSorting="True" DataKeyNames="Id" OnSorting="gridView_Sorting" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
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

            <asp:HyperLinkField Target="_blank" DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/modules/filecopymodules/general.aspx?fileCopyModuleId={0}" Text="View" ItemStyle-CssClass="chkboxwidth"/>
            <asp:TemplateField ItemStyle-CssClass="width_50" HeaderText="Priority" SortExpression="Priority">
                <ItemTemplate>

                    <asp:TextBox ID="txtPriority" runat="server" CssClass="textbox height_18"/>

                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Id" Visible="False"/>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="width_200"></asp:BoundField>

            <asp:TemplateField HeaderText="Destination Partition" SortExpression="Destination Partition" ItemStyle-CssClass="width_100">
                <ItemTemplate>

                    <asp:TextBox ID="txtPartition" runat="server" CssClass="textbox height_18"/>
                </ItemTemplate>
            </asp:TemplateField>
           <asp:TemplateField >
                <ItemTemplate>

                   

                </ItemTemplate>
            </asp:TemplateField>
          
        </Columns>
        <EmptyDataTemplate>
            No Files Copy Modules Found
        </EmptyDataTemplate>
    </asp:GridView>


</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <h5><span style="color: #ff9900;">Display Name:</span></h5>
<p>A name used to identify the com server, it has not effect on the functionality.</p>
<h5><span style="color: #ff9900;">URL:</span></h5>
<p>The url used to access the com server, this must match the url that is set in IIS.</p>
<h5><span style="color: #ff9900;">Description:</span></h5>
<p>An optional description for the com server.</p>
<h5><span style="color: #ff9900;">Replicate Storage:</span></h5>
<p>This option is only available after the com server is added.  When multiple com servers are defined, files for your modules must be replicated across all com servers.  If you want to disable replication to a com server, then disable this option.  If this option is disabled, you must manually replicate the files.</p>
</asp:Content>
