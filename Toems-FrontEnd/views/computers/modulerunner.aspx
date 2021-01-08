<%@ Page Title="" Language="C#" MasterPageFile="~/views/computers/computers.master" AutoEventWireup="true" CodeBehind="modulerunner.aspx.cs" Inherits="Toems_FrontEnd.views.computers.modulerunner" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li> <%= ComputerEntity.Name %></li>
    <li>Module Runner</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    <%= ComputerEntity.Name %>
</asp:Content>

<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">


</asp:Content>



<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" Runat="Server">
      <script type="text/javascript">
            function module() {
              Swal.fire({
                  title: 'Please Wait',
                  text: 'Attempting To Start Module On Client',
                  timer: 9000,
                  timerProgressBar: true
              })
          }

          $(document).ready(function () {
              $('#module').addClass("nav-current");
          });

    </script>

      <p class="total">
        <asp:Label ID="lblTotal" runat="server"></asp:Label>
    </p>
    <div class="size-7 column">

        <asp:TextBox ID="txtSearch" runat="server" CssClass="rounded-search" OnTextChanged="search_Changed"></asp:TextBox>
    </div>
    
    
    <br class="clear"/>
    
     <div class="size-10 column hidden-check">
         Printer
        <asp:CheckBox runat="server" ID="chkPrinter" Text="Printer" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged"/>
    </div>
    <div class="size-10 column hidden-check">
        Software
        <asp:CheckBox runat="server" Id="chkSoftware" Text="Software" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged"/>
    </div>
    <div class="size-10 column hidden-check">
        Script
        <asp:CheckBox runat="server" ID="chkScript" Text="Script" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged"/>
    </div>
     <div class="size-10 column hidden-check">
         File Copy
        <asp:CheckBox runat="server" ID="chkFile" Text="File Copy" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged"/>
    </div>
     <div class="size-10 column hidden-check">
         Command
        <asp:CheckBox runat="server" ID="chkCommand" Text="Command" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged"/>
    </div>
    <div class="size-10 column hidden-check">
        Windows Update
        <asp:CheckBox runat="server" ID="chkWu" Text="Command" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged"/>
    </div>
      <div class="size-10 column hidden-check">
        Message
        <asp:CheckBox runat="server" ID="chkMessage" Text="Command" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged"/>
    </div>
      <div class="size-10 column hidden-check">
          Unassigned
        <asp:CheckBox runat="server" ID="chkUnassigned" Text="Unassigned" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged"/>
    </div>

  
    <br class="clear"/>
    <asp:GridView ID="gvModules" runat="server" AllowSorting="True" DataKeyNames="Id" OnSorting="gridView_Sorting" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>

            <asp:TemplateField>

                <ItemStyle CssClass="chkboxwidth"></ItemStyle>
                <HeaderTemplate>
                    
                </HeaderTemplate>
                <ItemTemplate>
                   <asp:LinkButton runat="server" ID="btnRunModule" OnClick="btnRunModule_Click" Text="Run Module" OnClientClick="module();"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:HyperLinkField DataNavigateUrlFields="ModuleType,Id" DataNavigateUrlFormatString="~/views/policies/moduleredirect.aspx?type={0}&id={1}" Text="View" ItemStyle-CssClass="chkboxwidth" Target="_blank"/>
            <asp:BoundField DataField="Id" HeaderText="moduleID" SortExpression="moduleID" Visible="False"/>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="ModuleType" HeaderText="Type" SortExpression="ModuleType" ItemStyle-CssClass="width_200"/>
               <asp:BoundField DataField="Guid" HeaderText="Guid"/>
              <asp:BoundField DataField="Description" HeaderText="Description"/>
        </Columns>
        <EmptyDataTemplate>
            No Modules Found
        </EmptyDataTemplate>
    </asp:GridView>

 
    

</asp:Content>