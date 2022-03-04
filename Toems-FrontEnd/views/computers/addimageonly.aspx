<%@ Page Title="" Language="C#" MasterPageFile="~/views/computers/computers.master" AutoEventWireup="true" CodeBehind="addimageonly.aspx.cs" Inherits="Toems_FrontEnd.views.computers.addimageonly" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>Add Image Only Computers</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
 Computers
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="DropDownActionsSub">
      <li><asp:LinkButton ID="btnCreate" runat="server" OnClick="btnCreate_OnClick" Text="Create Computers" CssClass="main-action" /></li>
   
</asp:Content>





<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#createimageonly').addClass("nav-current");

        });

    </script>

  <asp:TextBox ID="txtImageOnly" runat="server" TextMode="MultiLine" Height="500" Width="400" CssClass="descbox border pad2" ></asp:TextBox>
  
   
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>This page is used to create Image Only computers from the web interface by providing a name and mac address, rather than needing to register them from the imaging client.  
        Enter the computer names and mac addresses, one per line, in the following format:<br />
        <b>computername,00:11:22:33:44:55</b><br />
        then select Actions -> Create Computers  </p>
</asp:Content>