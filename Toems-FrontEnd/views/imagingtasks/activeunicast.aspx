<%@ Page Title="" Language="C#" MasterPageFile="~/views/imagingtasks/imagingtasks.master" AutoEventWireup="true" CodeBehind="activeunicast.aspx.cs" Inherits="Toems_FrontEnd.views.imagingtasks.activeunicast" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>Active Unicasts</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
         Active Unicasts

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" runat="Server">
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" runat="Server">
  <script type="text/javascript">
        $(document).ready(function() {
            $('#unicast').addClass("nav-current");
        
        });

    </script>
  
       
   <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Timer ID="Timer" runat="server" Interval="2000" OnTick="Timer_Tick">
            </asp:Timer>
            <p class="total">
                <asp:Label ID="lblTotal" runat="server"></asp:Label>
            </p>
            <br class="clear"/>
            <br/>
            <asp:GridView ID="gvUcTasks" runat="server" AutoGenerateColumns="False" DataKeyNames="Id" CssClass="Gridview extraPad" AlternatingRowStyle-CssClass="alt">
                <Columns>
                    <asp:TemplateField ShowHeader="False" ItemStyle-CssClass="chkboxwidth">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnCancel" runat="server" CausesValidation="false" CommandName="" Text="Cancel" OnClick="btnCancel_Click"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Id" HeaderText="taskID" SortExpression="Id" InsertVisible="False" ReadOnly="True" Visible="False"/>
                    <asp:TemplateField ItemStyle-CssClass="width_150 mobi-hide-smaller" HeaderText="Name">
                        <ItemTemplate>
                            <asp:Label ID="lblComputer" runat="server" Text='<%# Bind("Computer.Name") %>'/>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" ItemStyle-CssClass="width_50"/>
                     <asp:BoundField DataField="ComServerName" HeaderText="Com Server" ItemStyle-CssClass="width_50"/>
                    <asp:BoundField DataField="Partition" HeaderText="Partition" ItemStyle-CssClass="mobi-hide-smaller" HeaderStyle-CssClass="mobi-hide-smaller"/>
                    <asp:BoundField DataField="Elapsed" HeaderText="Elapsed" ItemStyle-CssClass="mobi-hide-small" HeaderStyle-CssClass="mobi-hide-small"/>
                    <asp:BoundField DataField="Remaining" HeaderText="Remaining" ItemStyle-CssClass="mobi-hide-small" HeaderStyle-CssClass="mobi-hide-small"/>
                    <asp:BoundField DataField="Completed" HeaderText="Completed"/>
                    <asp:BoundField DataField="Rate" HeaderText="Rate" ItemStyle-CssClass="mobi-hide-smallest" HeaderStyle-CssClass="mobi-hide-smallest"/>

                </Columns>
                <EmptyDataTemplate>
                    No Active Unicasts
                </EmptyDataTemplate>
            </asp:GridView>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
   Displays all active upload and deploy tasks that have been started by the current user. Administrators see tasks created from all users. Individual tasks can also be cancelled from this page.
</asp:Content>
