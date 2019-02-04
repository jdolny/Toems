<%@ Page Title="" Language="C#" MasterPageFile="~/views/computers/computers.master" AutoEventWireup="true" CodeBehind="processreport.aspx.cs" Inherits="Toems_FrontEnd.views.computers.processreport" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li> <%= ComputerEntity.Name %></li>
    <li>Process Report</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    <%= ComputerEntity.Name %>
</asp:Content>

<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li>  <asp:LinkButton runat="server" ID="btnRun" Text="Run Query" OnClick="btnRun_OnClick" CssClass="main-action"></asp:LinkButton></li>
    <li><asp:LinkButton ID="buttonExport" runat="server" OnClick="buttonExport_OnClick" Text="Export To CSV" ></asp:LinkButton></li>

</asp:Content>



<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#process').addClass("nav-current");
        });

    </script>

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
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>The process report page makes it easy to determine the most commonly used applications for a computer. A report can be displayed by time spent with application open, or the count of the most opened applications.  You can also view a report for all users.  This report may take a long time to run if the time frame goes back to a longer time frame.</p>
</asp:Content>
