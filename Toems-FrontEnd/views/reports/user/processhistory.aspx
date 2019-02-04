<%@ Page Title="" Language="C#" MasterPageFile="~/views/reports/user/user.master" AutoEventWireup="true" CodeBehind="processhistory.aspx.cs" Inherits="Toems_FrontEnd.views.reports.user.processhistory" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li>Process History</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
    User Reports
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="DropDownActionsSub2">
    <li> <asp:LinkButton runat="server" ID="btnRun" Text="Run Query" OnClick="btnRun_OnClick" CssClass="main-action"></asp:LinkButton></li>
    <li><asp:LinkButton ID="buttonExport" runat="server" OnClick="buttonExport_OnClick" Text="Export To CSV"></asp:LinkButton></li>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
      <script type="text/javascript">
          $(document).ready(function () {
              $('#userprocess').addClass("nav-current");
          });

    </script>
    
    <div class="size-14 column">
        <asp:TextBox runat="server" CssClass="rounded-search" ID="txtUser" OnTextChanged="txtUser_OnTextChanged"></asp:TextBox>
    </div>
    <div class="float_right">
<div class="size-10 column">
    <asp:DropDownList runat="server" ID="ddlType" CssClass="ddlist">
        <asp:ListItem>Report Type</asp:ListItem>
    <asp:ListItem>Time</asp:ListItem>
        <asp:ListItem>Count</asp:ListItem>
        <asp:ListItem>User</asp:ListItem>
        </asp:DropDownList>
    </div>
    
    <div class="size-10 column">
    <asp:DropDownList runat="server" ID="ddlLimit" CssClass="ddlist">
        <asp:ListItem>Limit</asp:ListItem>
        <asp:ListItem>1</asp:ListItem>
        <asp:ListItem>10</asp:ListItem>
        <asp:ListItem>50</asp:ListItem>
        <asp:ListItem>100</asp:ListItem>
        <asp:ListItem>1000</asp:ListItem>
        <asp:ListItem>5000</asp:ListItem>
        <asp:ListItem>10000</asp:ListItem>
    </asp:DropDownList>
    </div>
    
    <div class="size-10 column">
    <asp:DropDownList runat="server" ID="ddlDays" CssClass="ddlist">
        <asp:ListItem>In Last X Days</asp:ListItem>
        <asp:ListItem>1</asp:ListItem>
        <asp:ListItem>7</asp:ListItem>
        <asp:ListItem>14</asp:ListItem>
        <asp:ListItem>30</asp:ListItem>
        <asp:ListItem>60</asp:ListItem>
        <asp:ListItem>90</asp:ListItem>
        <asp:ListItem>180</asp:ListItem>
        <asp:ListItem>365</asp:ListItem>
    </asp:DropDownList>
        </div>
        </div>
   
    <br class="clear"/>
    <br/>
    
    <div id="divTime" runat="server">
    <asp:GridView ID="gvProcessTime" runat="server"  DataKeyNames="Id"  AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="TotalTime" HeaderText="Total Time (Minutes)" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Path" HeaderText="Path"></asp:BoundField>
        </Columns>
        <EmptyDataTemplate>
            No Processes Found
        </EmptyDataTemplate>
    </asp:GridView>
    </div>
    
    <div id="divCount" runat="server">
    <asp:GridView ID="gvProcessCount" runat="server"  DataKeyNames="Id"  AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="TotalCount" HeaderText="Total Count" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Path" HeaderText="Path"></asp:BoundField>
        </Columns>
        <EmptyDataTemplate>
            No Processes Found
        </EmptyDataTemplate>
    </asp:GridView>
    </div>
    
    <div id="divUser" runat="server">
        <asp:GridView ID="gvProcessUser" runat="server"   AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
            <Columns>
                <asp:BoundField DataField="ComputerName" HeaderText="Computer" ItemStyle-CssClass="width_200"></asp:BoundField>
                <asp:BoundField DataField="ProcessName" HeaderText="Process" ItemStyle-CssClass="width_200"></asp:BoundField>
               
                <asp:BoundField DataField="UserName" HeaderText="User" ItemStyle-CssClass="width_200"></asp:BoundField>
                <asp:TemplateField HeaderText="Open Time" ItemStyle-CssClass="width_200">
                    <ItemTemplate>
                        <asp:Label ID="lblOpenTime" runat="server" Text='<%# Convert.ToDateTime(Eval("StartTime")).ToLocalTime() %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Close Time" ItemStyle-CssClass="width_200">
                    <ItemTemplate>
                        <asp:Label ID="lblCloseTime" runat="server" Text='<%# Convert.ToDateTime(Eval("EndTime")).ToLocalTime() %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Path" HeaderText="Path" ></asp:BoundField>
            </Columns>
            
            <EmptyDataTemplate>
                No Processes Found
            </EmptyDataTemplate>
        </asp:GridView>
    </div>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>This report will show you every process that a specific user has run on every computer Theopenem is managing.  This search box behaves differently than most and requires the exact username to search for including domain.  Ex: theopenem.com\user1</p>
</asp:Content>
