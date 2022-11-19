<%@ Page Title="" Language="C#" MasterPageFile="~/theopenem/views/admin/toec/toec.master" AutoEventWireup="true" CodeBehind="deployjobstatus.aspx.cs" Inherits="Toems_FrontEnd.views.admin.toec.deployjobstatus" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TopBreadCrumbSub2" runat="server">
     <li><a href="<%= ResolveUrl("~/views/admin/toec/deployjobstatus.aspx") %>?level=2">Deploy Job Status</a></li>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SubNavTitle_Sub2" runat="server">
    Deploy Job Status
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DropDownActionsSub2" runat="server">
    <li><asp:LinkButton ID="buttonResetStatus" runat="server" OnClick="buttonResetStatus_Click"  Text="Reset Selected Computer Status" CssClass="main-action" /></li> 
    <li><asp:LinkButton ID="buttonRestart" runat="server" OnClick="buttonRestart_Click" Text="Restart Deploy Job Service" CssClass="main-action" /></li>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SubContent2" runat="server">
     <script type="text/javascript">
        $(document).ready(function() {
            $('#deployjobstatus').addClass("nav-current");
        });
     </script>

     <div class="size-4 column">
        Select A Deploy Job:
    </div>
    <div class="size-5 column">
          <asp:DropDownList ID="ddlJobs" runat="server" CssClass="ddlist" OnSelectedIndexChanged="ddlJobs_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
    </div>

    <br class="clear"/>
  

         <asp:GridView ID="gvComputers" runat="server" AllowSorting="True" DataKeyNames="Id"  AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:TemplateField>
                <ItemStyle CssClass="chkboxwidth"></ItemStyle>
                <HeaderTemplate>
                  
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkSelector" runat="server"/>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:BoundField DataField="Id" HeaderText="computerID" SortExpression="computerID" Visible="False"/>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="width_200" ></asp:BoundField>
             <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" ItemStyle-CssClass="width_200" ></asp:BoundField>
             <asp:BoundField DataField="LastStatusDate" HeaderText="LastStatusDate" SortExpression="LastStatusDate" ItemStyle-CssClass="width_200" ></asp:BoundField>
             <asp:BoundField DataField="LastUpdateDetails" HeaderText="LastUpdateDetails" SortExpression="LastUpdateDetails" ></asp:BoundField>
        </Columns>
        <EmptyDataTemplate>
            No Computers Found
        </EmptyDataTemplate>
    </asp:GridView>


</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="subsubHelp" runat="server">
   
</asp:Content>