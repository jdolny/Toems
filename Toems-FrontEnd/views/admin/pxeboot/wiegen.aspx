<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/pxeboot/pxeboot.master" AutoEventWireup="true" CodeBehind="wiegen.aspx.cs" Inherits="Toems_FrontEnd.views.admin.pxeboot.wiegen" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TopBreadCrumbSub2" runat="server">
     <li><a href="<%= ResolveUrl("~/views/admin/pxeboot/wiegen.aspx") %>?level=2">WIE Generator</a></li>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SubNavTitle_Sub2" runat="server">
    WIE Generator
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DropDownActionsSub2" runat="server">

     <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_Click" OnClientClick="status();" Text="Generate WIE" CssClass="main-action" /></li>
      <li><asp:LinkButton ID="btnDownloadIso" runat="server" OnClick="btnDownloadIso_Click" Text="Download ISO" OnClientClick="iso();"  /></li>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SubContent2" runat="server">
     <script type="text/javascript">
         function status() {
             Swal.fire({
                 title: 'Please Wait',
                 text: 'Attempting To Start Wie Builder',
                 showConfirmButton: false,


             })
         }
         function iso() {
             Swal.fire({
                 title: 'Please Wait',
                 text: 'Downloading ISO',


             })
         }
        $(document).ready(function() {
            $('#wie').addClass("nav-current");
        });
     </script>

      <br class="clear"/>
  
   <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <h2>Last Build Details</h2>

               <div class="size-4 column">
        Build Date
    </div>

    <div class="size-5 column">
        <asp:Label runat="server" ID="lblBuildDate"></asp:Label>
    </div>

              <div class="size-4 column">
        Build Options
    </div>

    <div class="size-5 column">
        <asp:Label runat="server" ID="lblBuildOptions"></asp:Label>
    </div>
                        <br class="clear" />
              <h2>Active Build Processes</h2>

            <asp:Timer ID="Timer" runat="server" Interval="4000" OnTick="Timer_Tick">
            </asp:Timer>
        
            <asp:GridView ID="gvProcess" runat="server" AutoGenerateColumns="False" DataKeyNames="Pid" CssClass="Gridview extraPad" AlternatingRowStyle-CssClass="alt">
                <Columns>

                    <asp:BoundField DataField="Pid" HeaderText="PID" InsertVisible="False" ReadOnly="True" ItemStyle-CssClass="width_50"/>
                
                    <asp:BoundField DataField="ProcessName" HeaderText="Process" SortExpression="Status" ItemStyle-CssClass="width_200"/>
                  

                </Columns>
                <EmptyDataTemplate>
                    No Active Processes
                </EmptyDataTemplate>
            </asp:GridView>

        </ContentTemplate>
      
    </asp:UpdatePanel>
     <br class="clear" />
       <hr />
        <br class="clear" />
         <div class="size-4 column">
        Run As
    </div>

    <div class="size-5 column">
        <asp:DropDownList ID="ddlImpersonation" runat="server" CssClass="ddlist"/>
    </div>
     <div class="size-4 column">
        Time Zone:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtTimezone" runat="server" CssClass="textbox">
        </asp:TextBox>
    </div>
    <br class="clear" />

    <div class="size-4 column">
        Input Locale:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtInput" runat="server" CssClass="textbox">
        </asp:TextBox>
    </div>
    <br class="clear" />

    <div class="size-4 column">
        Language:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtLanguage" runat="server" CssClass="textbox">
        </asp:TextBox>
    </div>
    <br class="clear" />

       <div class="size-4 column">
        Com Servers:
    </div>
    
    <br class="clear"/>
    <asp:GridView ID="gvServers" runat="server"  DataKeyNames="Id"  AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
    <Columns>
        <asp:TemplateField>
            <HeaderStyle CssClass="chkboxwidth"></HeaderStyle>
            <ItemStyle CssClass="chkboxwidth"></ItemStyle>
      
            <ItemTemplate>
                <asp:CheckBox ID="chkSelector" runat="server"/>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/admin/comservers/editcomserver.aspx?level=2&serverId={0}" Text="View" ItemStyle-CssClass="chkboxwidth"/>
        <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" Visible="False"/>
        <asp:BoundField DataField="DisplayName" HeaderText="Display Name" ItemStyle-CssClass="width_200"></asp:BoundField>
        <asp:BoundField DataField="Url" HeaderText="URL"></asp:BoundField>
         

    </Columns>
    <EmptyDataTemplate>
        No Client Communication Servers Found
    </EmptyDataTemplate>
</asp:GridView>
        <br class="clear"/>
  
    <div class="size-4 column">
        Universal Token:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtToken" runat="server" CssClass="textbox">
        </asp:TextBox>
    </div>
    <div class="size-4 column">
        Restrict Com Servers
    </div>
     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkRestrictComServers" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkRestrictComServers">Toggle</label>
        </div>
    <br class="clear"/>
     <div class="size-4 column">
        Additional Drivers:
    </div>
     <asp:GridView ID="gvDrivers" runat="server" AllowSorting="True" DataKeyNames="Id" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>

            <asp:TemplateField>

                <ItemStyle CssClass="chkboxwidth"></ItemStyle>

                <ItemTemplate>
                    <asp:CheckBox ID="chkSelector" runat="server"/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/modules/filecopymodules/general.aspx?fileCopyModuleId={0}" Text="View" ItemStyle-CssClass="chkboxwidth"/>
            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" Visible="False"/>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ></asp:BoundField>      

        </Columns>
        <EmptyDataTemplate>
            No File Copy Driver Modules Found
        </EmptyDataTemplate>
    </asp:GridView>
    <br class="clear"/>
     <div class="size-4 column">
        Skip ADK Check
    </div>
     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkAdk" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkAdk">Toggle</label>
        </div>
    <br class="clear"/>
    </asp:Content>

