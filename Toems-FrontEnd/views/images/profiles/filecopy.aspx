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
   Allows you to copy additional files or folders to a specific partition after the image is applied. They are only applied to deploy or multicast tasks. 
    Like scripts and sysprep they can be assigned a priority to run. Lower numbers run first. The Destination Partition is just the number of the partition you want to send to. 
    You can find the correct number by viewing the image schema. If you are copying to an LVM partition you would use volumegroupname-logicalvolumename for the destination. 
  
</asp:Content>
