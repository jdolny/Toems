<%@ Page Title="" Language="C#" MasterPageFile="~/views/reports/process/process.master" AutoEventWireup="true" CodeBehind="topprocesses.aspx.cs" Inherits="Toems_FrontEnd.views.reports.process.toptentime" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li>Top Processes</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
    Process Reports
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="DropDownActionsSub2">
    <li><asp:LinkButton runat="server" ID="btnRun" Text="Run Query" OnClick="btnRun_OnClick" CssClass="main-action"></asp:LinkButton></li>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#top').addClass("nav-current");
            $("[id*=gvProcesses] td").hover(function() {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function() {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
        });
    </script>
   
    <div class="size-10 column">
    <asp:DropDownList runat="server" ID="ddlType" CssClass="ddlist">
        <asp:ListItem>Report Type</asp:ListItem>
    <asp:ListItem>Time</asp:ListItem>
        <asp:ListItem>Count</asp:ListItem>
        </asp:DropDownList>
    </div>
    
    <div class="size-10 column">
    <asp:DropDownList runat="server" ID="ddlLimit" CssClass="ddlist">
        <asp:ListItem>Limit</asp:ListItem>
        <asp:ListItem>1</asp:ListItem>
        <asp:ListItem>10</asp:ListItem>
        <asp:ListItem>50</asp:ListItem>
        <asp:ListItem>100</asp:ListItem>
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
  
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>Displays a report of the most commonly used applications across all computers that have checked in with Theopenem.  Two report types are available, time and count.  The time report will show the amount of time an application has been opened.  Since this is a summary against all computers, this may take a long time to run, it is recommended to keep the timeframe low.  The count report will show how many times the application has been opened.  This report runs against all computers, a similar report can be run against a specific group, or computer on their respective navigational pages.</p>
</asp:Content>
