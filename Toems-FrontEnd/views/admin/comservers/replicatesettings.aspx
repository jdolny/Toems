<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/comservers/comservers.master" AutoEventWireup="true" CodeBehind="replicatesettings.aspx.cs" Inherits="Toems_FrontEnd.views.admin.comservers.replicatesettings" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li><a href="<%= ResolveUrl("~/views/admin/comservers/replicatesettings.aspx") %>?level=2">Replication Settings</a></li>
    <li><%=ComServer.DisplayName %></li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
<%=ComServer.DisplayName %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
   <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_OnClick" Text="Update Server" CssClass="main-action" /></li>
     <li><asp:LinkButton ID="btnCert" runat="server" OnClick="btnCert_Click" Text="Create Certificate" /></li>
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#replicate').addClass("nav-current");
                 $("[id*=gvProcess] td").hover(function() {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function() {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
        });
    </script>
   
      <div class="size-4 column">
        Replicate Storage:
    </div>
    <div class="size-5 column hidden-check">
        <asp:CheckBox ID="chkReplicateStorage" runat="server" ClientIDMode="Static" ></asp:CheckBox>
        <label for="chkReplicateStorage"></label>
    </div>
      <div class="size-4 column">
       Max Bitrate (IGP):
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtMaxBitrate" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
     <br class="clear"/>
    <h1>Active Replications</h1>
   <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Timer ID="Timer" runat="server" Interval="2000" OnTick="Timer_Tick">
            </asp:Timer>
        
            <asp:GridView ID="gvProcess" runat="server" AutoGenerateColumns="False" DataKeyNames="Pid" CssClass="Gridview extraPad" AlternatingRowStyle-CssClass="alt">
                <Columns>
                    <asp:TemplateField ShowHeader="False" ItemStyle-CssClass="chkboxwidth">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnCancel" runat="server" CausesValidation="false" CommandName="" Text="Cancel" OnClick="btnCancel_Click"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Pid" HeaderText="PID" InsertVisible="False" ReadOnly="True" ItemStyle-CssClass="width_50"/>
                
                    <asp:BoundField DataField="ProcessName" HeaderText="Process" SortExpression="Status" ItemStyle-CssClass="width_200"/>
                    <asp:BoundField DataField="ProcessArguments" HeaderText="Start Time" />
                  

                </Columns>
                <EmptyDataTemplate>
                    No Active Processes
                </EmptyDataTemplate>
            </asp:GridView>

        </ContentTemplate>
    </asp:UpdatePanel>
  
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
<h5><span style="color: #ff9900;">Replicate Storage:</span></h5>
<p>When multiple com servers are defined, files for your modules and images must be replicated across all com servers.  
    If you want to disable replication to a com server, then disable this option.  If this option is disabled, you must manually replicate the files.</p>
    <h5><span style="color: #ff9900;">Max Bitrate:</span></h5>
    <p>The max bitrate for files to replicate in IGP.  What's an IGP?  Who knows.  Check the robocopy docs for somewhat of a description.</p>
</asp:Content>
