<%@ Page Title="" Language="C#" MasterPageFile="~/views/modules/commandmodules/commandmodule.master" AutoEventWireup="true" CodeBehind="externalfiles.aspx.cs" Inherits="Toems_FrontEnd.views.modules.commandmodules.externalfiles" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li><%= CommandModule.Name %></li>
    <li>External Files</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
<%= CommandModule.Name %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="buttonDownload" runat="server" OnClick="buttonDownload_OnClick" Text="Download Files" CssClass="main-action"/></li>
</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#externalfiles').addClass("nav-current");
            $("[id*=gvFiles] td").hover(function () {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function () {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });

        });
    </script>
    
    <p class="total">
        <asp:Label ID="lblTotal" runat="server"></asp:Label>
    </p>
    
    <div class="size-4 column">
        Sync Servers When Complete:
    </div>
    <div class="size-setting column hidden-check">
        <asp:CheckBox ID="chkSync" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkSync"></label>
    </div>
    <br class="clear"/>
  
      <asp:GridView ID="gvExternalFiles" runat="server" AutoGenerateColumns="false"  CssClass="Gridview extraPad"
                      OnRowEditing="OnRowEditing" OnRowCancelingEdit="OnRowCancelingEdit" ShowFooter="True"
                      OnRowUpdating="OnRowUpdating" OnRowDeleting="OnRowDeleting" AlternatingRowStyle-CssClass="alt">
            <Columns>
                <asp:TemplateField HeaderText="File Name" ItemStyle-Width="150" FooterStyle-Width="150">
                    <ItemTemplate>
                        <asp:Label ID="lblFile" runat="server" Text='<%# Eval("FileName") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtFile" runat="server" Text='<%# Eval("FileName") %>' CssClass="textbox_dynamic"></asp:TextBox>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txtFile" runat="server" CssClass="textbox_dynamic"></asp:TextBox>
                    </FooterTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="URL" ItemStyle-Width="500" FooterStyle-Width="500">
                    <ItemTemplate>
                        <asp:Label ID="lblUrl" runat="server" Text='<%# Eval("Url") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtUrl" runat="server" Text='<%# Eval("Url") %>' CssClass="textbox_dynamic"></asp:TextBox>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txtUrl" runat="server" CssClass="textbox_dynamic"></asp:TextBox>
                    </FooterTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="Optional Sha256 Hash" ItemStyle-Width="150" FooterStyle-Width="150">
                    <ItemTemplate>
                        <asp:Label ID="lblHash" runat="server" Text='<%# Eval("Hash") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtHash" runat="server" Text='<%# Eval("Hash") %>' CssClass="textbox_dynamic"></asp:TextBox>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txtHash" runat="server" CssClass="textbox_dynamic"></asp:TextBox>
                    </FooterTemplate>
                </asp:TemplateField>
            

                <asp:TemplateField ShowHeader="False">
                    <EditItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True"
                                        CommandName="Update" Text="Update" CssClass="btn white">
                        </asp:LinkButton>
                        &nbsp;<asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False"
                                              CommandName="Cancel" Text="Cancel" CssClass="btn white">
                        </asp:LinkButton>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False"
                                        CommandName="Edit" Text="Edit" CssClass="btn white">
                        </asp:LinkButton>
                        <asp:LinkButton ID="LinkButton3" runat="server" CausesValidation="False"
                                        CommandName="Delete" Text="Delete" CssClass="btn white">
                        </asp:LinkButton>
                    </ItemTemplate>
                    <FooterTemplate>

                        <asp:LinkButton ID="btnAdd1" runat="server" Text="Add" OnClick="btnAdd1_OnClick" CssClass="btn white"/>
                    </FooterTemplate>
                </asp:TemplateField>

            </Columns>
        </asp:GridView>
   
    <br class="clear"/>
    
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
<ContentTemplate>
    <asp:Timer ID="Timer" runat="server" Interval="5000" OnTick="Timer_Tick">
    </asp:Timer>
    <asp:GridView ID="gvFiles" runat="server" AllowSorting="True" DataKeyNames="Id"  AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
              <asp:TemplateField ItemStyle-CssClass="chkboxwidth">
                <ItemTemplate>
                    <asp:LinkButton ID="btnDelete" runat="server" OnClick="btnDelete_OnClick" Text="Delete / Cancel"/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-CssClass="chkboxwidth">
                <ItemTemplate>
                    <asp:LinkButton ID="btnReDownload" runat="server" OnClick="btnReDownload_OnClick" Text="Re-Download"/>
                </ItemTemplate>
            </asp:TemplateField>
        
            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" Visible="False"/>
            <asp:BoundField DataField="FileName" HeaderText="File Name"  ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Progress" HeaderText="Progress"  ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Status" HeaderText="Status"  ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Url" HeaderText="Url"  ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="DateDownloaded" HeaderText="Date"  ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Sha256Hash" HeaderText="Hash"  ItemStyle-CssClass="width_200"></asp:BoundField>
             <asp:BoundField DataField="ErrorMessage" HeaderText="Error Message" />
        </Columns>
        <EmptyDataTemplate>
            No Files Found
        </EmptyDataTemplate>
    </asp:GridView>
</ContentTemplate>
    </asp:UpdatePanel>
   
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>External files provide an additional way to add files to the module. Instead of uploading an existing file from your computer, you can download directly from the internet. External files were mainly created to share policies. Policies can be exported and imported by other users. If external files are used, once the policy is imported it will also automatically download any required files. To download external files:</p>
<ol>
	<li>Enter a file name,</li>
	<li>Provide the url</li>
	<li>Optionally provide a sha256 checksum, if the checksum doesn't match after the file is downloaded, it will be deleted.</li>
	<li>Click Add</li>
	<li>Once all files are added, click Actions, then Download Files</li>
	<li>File Downloads run async, you can navigate away from the page once the download starts</li>
	<li>Finally there is button to sync servers when downloads are complete, this should be enabled before starting the download if you want to replicate all Client Communication Servers when complete.</li>
</ol>
</asp:Content>

