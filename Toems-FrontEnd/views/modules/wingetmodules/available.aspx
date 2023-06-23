<%@ Page Title="" Language="C#" MasterPageFile="~/views/modules/wingetmodules/wingetmodule.master" AutoEventWireup="true" CodeBehind="available.aspx.cs" Inherits="Toems_FrontEnd.views.modules.wingetmodules.available" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li>Available Winget Packages</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
 Available Winget Packages
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">

</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#available').addClass("nav-current");
            $("[id*=gvManifests] td").hover(function () {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function () {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });

        });
    </script>
    
   
    <div class="size-7 column">
        <asp:TextBox ID="txtSearch" runat="server" CssClass="rounded-search" OnTextChanged="search_Changed"></asp:TextBox>
    </div>


     <div class="size-11 column">
         <div class="custom-select">
        <asp:DropDownList runat="server" ID="ddlLimit" AutoPostBack="True" OnSelectedIndexChanged="ddl_OnSelectedIndexChanged" CssClass="ddlist">
            <asp:ListItem></asp:ListItem>
            <asp:ListItem Selected="True">25</asp:ListItem>
            <asp:ListItem>100</asp:ListItem>
            <asp:ListItem>250</asp:ListItem>
            <asp:ListItem>500</asp:ListItem>
        </asp:DropDownList>
             </div>
     </div>
   
     <br class="clear"/>
    <br class="clear"/>

     <div class="size-10 column hidden-check">
         Exact Match
        <asp:CheckBox runat="server" ID="chkExact" Text="Printer" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged" Checked="false"/>
    </div>
    <div class="size-10 column hidden-check">
        Latest Version Only
        <asp:CheckBox runat="server" Id="chkLatest" Text="Software" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged" Checked="true"/>
    </div>

    <div class="size-10 column hidden-check">
        Package Identifier
        <asp:CheckBox runat="server" ID="chkIdentifier" Text="Script" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged" Checked="true"/>
    </div>
     <div class="size-10 column hidden-check">
         Package Name
        <asp:CheckBox runat="server" ID="chkName" Text="File Copy" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged" Checked="true"/>
    </div>
     <div class="size-10 column hidden-check">
         Publisher
        <asp:CheckBox runat="server" ID="chkPublisher" Text="Command" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged" Checked="true"/>
    </div>
    <div class="size-10 column hidden-check">
        Tags
        <asp:CheckBox runat="server" ID="chkTags" Text="Command" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged" Checked="false"/>
    </div>
      <div class="size-10 column hidden-check">
        Moniker
        <asp:CheckBox runat="server" ID="chkMoniker" Text="Command" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged" Checked="true"/>
    </div>
   
  
     <p class="total">
        <asp:Label ID="lblTotal" runat="server"></asp:Label>
    </p>
    <asp:GridView ID="gvManifests" runat="server" AllowSorting="True" DataKeyNames="Id" OnSorting="gridView_Sorting" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>

            <asp:TemplateField>

                <ItemStyle CssClass="chkboxwidth"></ItemStyle>
               
                <ItemTemplate>
                    <asp:CheckBox ID="chkSelector" runat="server"/>
                </ItemTemplate>
                <ItemTemplate>
                    <asp:LinkButton runat="server" ID="lnkCustDetails"
                Text='Details'
                CommandArgument='<%# Eval("Id") %>'
                OnCommand="details_Click" />
                </ItemTemplate>
            </asp:TemplateField>

           
            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" Visible="False"/>
             <asp:BoundField DataField="PackageName" HeaderText="Package Name" SortExpression="PackageName" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="PackageVersion" HeaderText="Package Version" SortExpression="PackageVersion" ItemStyle-CssClass="width_200"></asp:BoundField>
           
            <asp:BoundField DataField="Scope" HeaderText="Scope" SortExpression="Scope" ItemStyle-CssClass="width_200"></asp:BoundField>
             <asp:BoundField DataField="PackageIdentifier" HeaderText="Package Identifier" SortExpression="PackageIdentifier" ItemStyle-CssClass="width_200"></asp:BoundField>
             <asp:BoundField DataField="Publisher" HeaderText="Publisher" SortExpression="Publisher"></asp:BoundField>
        </Columns>
        <EmptyDataTemplate>
            No Winget Packages Found
        </EmptyDataTemplate>
    </asp:GridView>


<br />
<div id="divModal" runat="server" class ="modalDialog" visible="false">
    <div>
        <asp:LinkButton ID="lbtnModalClose" runat="server"  CssClass="close" Text="X" OnClick="CloseModal" />
      <div class="size-lbl3 column">
        Package Name
    </div>
   <div class="size-lbl4 column">
        <asp:Label ID="txtPackageName" runat="server"  ClientIDMode="Static"></asp:Label>
    </div>
    <br class="clear" />
    <div class="size-lbl3 column">
        Package Identifier
    </div>
    <div class="size-lbl4 column">
        <asp:Label ID="txtPackageIdentifier" runat="server"  ClientIDMode="Static"></asp:Label>
    </div>
    <br class="clear" />
    <div class="size-lbl3 column">
        Package Version
    </div>
    <div class="size-lbl4 column">
        <asp:Label ID="txtPackageVersion" runat="server"  ClientIDMode="Static"></asp:Label>
    </div>
    <br class="clear" />
    <div class="size-lbl3 column">
        Package URL
    </div>
   <div class="size-lbl4 column">
        <asp:HyperLink ID="txtPackageUrl" runat="server" Target="_blank"  ></asp:HyperLink>
    </div>
    <br class="clear" />
    <div class="size-lbl3 column">
        Publisher
    </div>
     <div class="size-lbl4 column">
        <asp:Label ID="txtPublisher" runat="server"  ClientIDMode="Static"></asp:Label>
    </div>
    <br class="clear" />
    <div class="size-lbl3 column">
        Publisher URL
    </div>
   <div class="size-lbl4 column">
       <asp:HyperLink ID="txtPublisherUrl" runat="server" Target="_blank"></asp:HyperLink>

    </div>
    <br class="clear" />
    <div class="size-lbl3 column">
        License
    </div>
    <div class="size-lbl4 column">
        <asp:Label ID="txtLicense" runat="server"  ClientIDMode="Static"></asp:Label>
    </div>
    <br class="clear" />
     <div class="size-lbl3 column">
        Short Description
    </div>
    <div class="size-lbl4 column">
        <asp:Label ID="txtDescription" runat="server"  ClientIDMode="Static"></asp:Label>
    </div>
    <br class="clear" />
    <div class="size-lbl3 column">
        Tags
    </div>
    <div class="size-lbl4 column">
        <asp:Label ID="txtTags" runat="server"  ClientIDMode="Static"></asp:Label>
    </div>
    <br class="clear" />
    <div class="size-lbl3 column">
        Moniker
    </div>
    <div class="size-lbl4 column">
        <asp:Label ID="txtMoniker" runat="server"  ClientIDMode="Static"></asp:Label>
    </div>
    <br class="clear" />
    </div>
</div>
  

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">

</asp:Content>