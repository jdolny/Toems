<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/admin.master" AutoEventWireup="true" CodeBehind="certificates.aspx.cs" Inherits="Toems_FrontEnd.views.admin.certificates" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>Certificates</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    Admin Settings
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" runat="Server">
    <li><asp:LinkButton ID="btnGenerate" runat="server" OnClick="btnGenerate_OnClick" Text="Generate Certificates" CssClass="main-action"/></li>

</asp:Content>





<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#certificate').addClass("nav-current");
            $("[id*=gvCertificates] td").hover(function () {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function () {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
        });
    </script>
    
      <asp:GridView ID="gvCertificates" runat="server"  DataKeyNames="Id" AutoGenerateColumns="False" CssClass="Gridview extraPad" AlternatingRowStyle-CssClass="alt">
        <Columns>
               <asp:TemplateField >
                <ItemTemplate>
                    <asp:LinkButton ID="btnExport" runat="server" OnClick="btnExport_OnClick" Text="Export"/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Id" HeaderText="computerID"  Visible="False"/>
            <asp:BoundField DataField="SubjectName" HeaderText="Subject Name"  ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Serial" HeaderText="Serial"  ItemStyle-CssClass="width_200"></asp:BoundField>
             <asp:BoundField DataField="Type" HeaderText="Type" ItemStyle-CssClass="width_200"></asp:BoundField>
             <asp:BoundField DataField="NotBefore" HeaderText="Not Before"  ItemStyle-CssClass="width_200"></asp:BoundField>
             <asp:BoundField DataField="NotAfter" HeaderText="Not After" ></asp:BoundField>
          
        
        
        </Columns>
        <EmptyDataTemplate>
            No Certificates Found
        </EmptyDataTemplate>
    </asp:GridView>
      
       
        
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>Certificates are used to generate and export the Toems CA and Intermediate certs.  These certificates are used as part of the communication process b/w Toec and Toems to verify identities and encrypt data.</p>
</asp:Content>
