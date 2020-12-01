<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/kernels/kernels.master" AutoEventWireup="true" CodeBehind="downloadkernel.aspx.cs" Inherits="Toems_FrontEnd.views.admin.kernels.downloadkernel" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TopBreadCrumbSub2" runat="server">
     <li><a href="<%= ResolveUrl("~/views/admin/kernels/downloadkernel.aspx") %>?level=2">Kernel Downloads</a></li>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SubNavTitle_Sub2" runat="server">

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DropDownActionsSub2" runat="server">

    
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SubContent2" runat="server">
     <script type="text/javascript">
        $(document).ready(function() {
            $('#download').addClass("nav-current");
             $("[id*=gvKernels] td").hover(function() {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function() {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
        });
    </script>

     <asp:GridView ID="gvKernels" runat="server" AutoGenerateColumns="False" CssClass="Gridview extraPad" AlternatingRowStyle-CssClass="alt">
        <Columns>


            <asp:BoundField DataField="FileName" HeaderText="File" ItemStyle-CssClass="width_200"/>
            <asp:BoundField DataField="Description" HeaderText="Description" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="BaseVersion" HeaderText="Version" ItemStyle-CssClass="width_200"/>

            <asp:TemplateField>

                <ItemStyle></ItemStyle>
                <ItemTemplate>
                    <asp:LinkButton ID="btnDownload" runat="server" OnClick="btnDownload_OnClick" Text="Download"/>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField ShowHeader="True" HeaderText="Installed">
                <ItemTemplate>
                    <asp:Label ID="lblInstalled" runat="server" CausesValidation="false" CssClass="lbl_file" Text="No"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>


        </Columns>

    </asp:GridView>

</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="subsubHelp" runat="server">
    The kernel downloads provide a simple interface to install new kernels when they are released.
</asp:Content>
