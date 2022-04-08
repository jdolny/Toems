<%@ Page Title="" Language="C#" MasterPageFile="~/views/computers/computers.master" AutoEventWireup="true" CodeBehind="addimageonly.aspx.cs" Inherits="Toems_FrontEnd.views.computers.addimageonly" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>Add Image Only Computers</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
 Computers
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="DropDownActionsSub">
          <li><asp:LinkButton ID="btnCreateSingle" runat="server" OnClick="btnCreateSingle_OnClick" Text="Create Single Computer" CssClass="main-action" /></li>
      <li><asp:LinkButton ID="btnCreate" runat="server" OnClick="btnCreate_OnClick" Text="Create Computers From List" CssClass="main-action" /></li>

   
</asp:Content>





<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#createimageonly').addClass("nav-current");

        });
    
    </script>
        <h2>Single Computer</h2>
      <div class="size-lbl column">
        Computer Name:
    </div>
    <div class="size-lbl2 column">
        <asp:Textbox runat="server" ID="txtName" CssClass="textbox" ></asp:Textbox>
    </div>
    <br class="clear"/>

      <div class="size-lbl column">
        Computer Mac Address:
    </div>
    <div class="size-lbl2 column">
        <asp:Textbox runat="server" ID="txtMac" CssClass="textbox" ></asp:Textbox>
    </div>
    <br class="clear"/>
    <br class="clear"/>
    <br class="clear"/>
    <h2>Computer List</h2>
  <asp:TextBox ID="txtImageOnly" runat="server" TextMode="MultiLine" Height="500" Width="400" CssClass="descbox border pad2" ></asp:TextBox>
  
   
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>This page is used to create Image Only computers from the web interface instead of registering them from the imaging client.<br />
        To add a single computer:<br />
        Provide the computer name and mac address, then select Actions -> Create Single Computer<br />
        To add multiple computers:<br />
        Enter the computer names and mac addresses, one per line, in the following format:<br />
        <b>computername,00:11:22:33:44:55</b><br />
        then select Actions -> Create Computers From List </p>
</asp:Content>