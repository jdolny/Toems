<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/toec/toec.master" AutoEventWireup="true" CodeBehind="edittargetlist.aspx.cs" Inherits="Toems_FrontEnd.views.admin.toec.edittargetlist" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TopBreadCrumbSub2" runat="server">
     <li><a href="<%= ResolveUrl("~/views/admin/toec/createtargetlist.aspx") %>?level=2">Edit Target List</a></li>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SubNavTitle_Sub2" runat="server">
    Edit Target List
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DropDownActionsSub2" runat="server">

     <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_Click" Text="Update Target List" CssClass="main-action" /></li>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SubContent2" runat="server">
    

     <div class="size-4 column">
        Target List Name:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtName" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>

     
      <div class="size-4 column">
        List Type:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlListType" runat="server" CssClass="ddlist" OnSelectedIndexChanged="ddlListType_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList> 
            </div>
    <br class="clear"/>

    <div id="ous" runat="server">
          <div class="tree">
        <asp:TreeView ID="treeOus" runat="server" ShowCheckBoxes="All"  ImageSet="Arrows" ExpandDepth="0" RootNodeStyle-CssClass="rootNode" LeafNodeStyle-CssClass="leafNode" NodeStyle-CssClass="treeNode" NodeIndent="10">
        </asp:TreeView>
              <asp:Label ID="lblNoOu" runat="server"></asp:Label>
    </div>

        
    </div>

     <div id="adGroups" runat="server">
         <asp:GridView ID="gvAdGroups" runat="server" AllowSorting="True" DataKeyNames="Id"  AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:TemplateField>
                <ItemStyle CssClass="chkboxwidth"></ItemStyle>
                <HeaderTemplate>
                  
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkSelector" runat="server"/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/groups/general.aspx?groupId={0}" Text="View" ItemStyle-CssClass="chkboxwidth"/>
            <asp:BoundField DataField="Id" HeaderText="computerID" SortExpression="computerID" Visible="False"/>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ></asp:BoundField>
        </Columns>
        <EmptyDataTemplate>
            No Active Directory Groups Found
        </EmptyDataTemplate>
    </asp:GridView>
    </div>

     <div id="computers" runat="server">
            <h2>Computer List</h2>
  <asp:TextBox ID="txtComputers" runat="server" TextMode="MultiLine" Height="500" Width="400" CssClass="descbox border pad2" ></asp:TextBox>
  
     </div>

</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="subsubHelp" runat="server">
   
</asp:Content>