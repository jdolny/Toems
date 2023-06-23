<%@ Page Title="" Language="C#" MasterPageFile="~/views/modules/modules.master" AutoEventWireup="true" CodeBehind="all.aspx.cs" Inherits="Toems_FrontEnd.views.modules.all" %>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
All Modules List
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavLevel1" ID="subNavLevel1">
     <ul class="ul-secondary-nav">
            <li id="software">
                <a href="<%= ResolveUrl("~/views/modules/softwaremodules/search.aspx") %>">
                    <span class="sub-nav-text">Software Modules</span></a>
            </li>
             <li id="printer">
                <a href="<%= ResolveUrl("~/views/modules/printermodules/search.aspx") %>">
                    <span class="sub-nav-text">Printer Modules</span></a>
            </li>
             <li id="command">
                <a href="<%= ResolveUrl("~/views/modules/commandmodules/search.aspx") %>">
                    <span class="sub-nav-text">Command Modules</span></a>
            </li>
             <li id="script">
                <a href="<%= ResolveUrl("~/views/modules/scriptmodules/search.aspx") %>">
                    <span class="sub-nav-text">Script Modules</span></a>
            </li>
             <li id="file">
                <a href="<%= ResolveUrl("~/views/modules/filecopymodules/search.aspx") %>">
                    <span class="sub-nav-text">File Copy Modules</span></a>
            </li>
         <li id="windowsupdate">
             <a href="<%= ResolveUrl("~/views/modules/wumodules/search.aspx") %>">
                 <span class="sub-nav-text">Windows Update Modules</span></a>
         </li>
          <li id="message">
             <a href="<%= ResolveUrl("~/views/modules/messagemodules/search.aspx") %>">
                 <span class="sub-nav-text">Message Modules</span></a>
         </li>
              <li id="sysprep">
             <a href="<%= ResolveUrl("~/views/modules/sysprepmodules/search.aspx") %>">
                 <span class="sub-nav-text">Sysprep Modules</span></a>
         </li>
           <li id="winpe">
             <a href="<%= ResolveUrl("~/views/modules/winpemodules/search.aspx") %>">
                 <span class="sub-nav-text">WinPE Modules</span></a>
         </li>
           <li id="winget">
             <a href="<%= ResolveUrl("~/views/modules/wingetmodules/search.aspx") %>">
                 <span class="sub-nav-text">Winget Modules</span></a>
         </li>
          <li id="all">
             <a href="<%= ResolveUrl("~/views/modules/all.aspx") %>">
                 <span class="sub-nav-text">All Modules List</span></a>
         </li>
        </ul>

     
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="SubContent" runat="server">
    <script>
    $(document).ready(function () {
    $('.actions_left').addClass("display-none");
        });
    </script>
     <asp:GridView ID="gvModules" runat="server" AllowSorting="True" DataKeyNames="Id" OnSorting="gridView_Sorting" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>

            <asp:TemplateField>

                <ItemStyle CssClass="chkboxwidth"></ItemStyle>
                <HeaderTemplate>
                    <asp:CheckBox ID="chkSelectAll" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkSelector" runat="server"/>
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

<asp:Content runat="server" ContentPlaceHolderID="SubHelp">
    </asp:Content>