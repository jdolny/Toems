<%@ Page Title="" Language="C#" MasterPageFile="~/views/groups/groups.master" AutoEventWireup="true" CodeBehind="dynamic.aspx.cs" Inherits="Toems_FrontEnd.views.groups.dynamic" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li><%= GroupEntity.Name %></li>
    <li>Dynamic Criteria</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
<%= GroupEntity.Name %>
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
     <li><asp:LinkButton ID="btnTestQuery" runat="server" OnClick="btnTestQuery_OnClick" Text="Test Query" CssClass="main-action"></asp:LinkButton></li>
    <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_OnClick" Text="Save Dynamic Criteria" ></asp:LinkButton></li>
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" Runat="Server">
      <script type="text/javascript">
          $(document).ready(function () {
              $('#dynamic').addClass("nav-current");
          });     
    </script>
  
        <asp:GridView ID="gvExists" runat="server" AutoGenerateColumns="false"  CssClass="Gridview extraPad"
                      OnRowEditing="OnRowEditing" OnRowCancelingEdit="OnRowCancelingEdit" ShowFooter="True"
                      OnRowUpdating="OnRowUpdating" OnRowDataBound="gv_OnRowDataBound" OnRowDeleting="OnRowDeleting" AlternatingRowStyle-CssClass="alt">
            <Columns>
                  <asp:TemplateField HeaderText="And/Or" ItemStyle-Width="50" FooterStyle-Width="60">
                    <ItemTemplate>
                        <asp:Label ID="lblAndOr" runat="server" Text='<%# Eval("AndOr") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddlAndOr" runat="server" Text='<%# Eval("AndOr") %>' CssClass="ddlist order">
                              <asp:ListItem></asp:ListItem>
                            <asp:ListItem>And</asp:ListItem>
                            <asp:ListItem>Or</asp:ListItem>
                             <asp:ListItem>Not</asp:ListItem> 
                        </asp:DropDownList>
                    </EditItemTemplate>
                    <FooterTemplate>
                         <asp:DropDownList ID="ddlAndOr" runat="server" CssClass="ddlist order">
                            <asp:ListItem></asp:ListItem>
                            <asp:ListItem>And</asp:ListItem>
                            <asp:ListItem>Or</asp:ListItem> 
                              <asp:ListItem>Not</asp:ListItem> 
                        </asp:DropDownList>
                    </FooterTemplate>
                </asp:TemplateField>
                  <asp:TemplateField HeaderText="" ItemStyle-Width="50" FooterStyle-Width="60">
                    <ItemTemplate>
                        <asp:Label ID="lblLeftPar" runat="server" Text='<%# Eval("LeftParenthesis") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddlLeftPar" runat="server" Text='<%# Eval("LeftParenthesis") %>' CssClass="ddlist order">
                              <asp:ListItem></asp:ListItem>
                            <asp:ListItem>(</asp:ListItem> 
                        </asp:DropDownList>
                    </EditItemTemplate>
                    <FooterTemplate>
                         <asp:DropDownList ID="ddlLeftPar" runat="server" CssClass="ddlist order">
                              <asp:ListItem></asp:ListItem>
                            <asp:ListItem>(</asp:ListItem> 
                        </asp:DropDownList>
                    </FooterTemplate>
                </asp:TemplateField>
                
                 <asp:TemplateField HeaderText="Table" ItemStyle-Width="150" FooterStyle-Width="150">
                    <ItemTemplate>
                        <asp:Label ID="lblTable" runat="server" Text='<%# Eval("Table") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddlTable" runat="server"  AutoPostBack="True" OnSelectedIndexChanged="ddlTable_OnSelectedIndexChanged" CssClass="ddlist order"></asp:DropDownList>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:DropDownList ID="ddlTable" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlTable_OnSelectedIndexChanged" CssClass="ddlist order"></asp:DropDownList>
                    </FooterTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Field" ItemStyle-Width="150" FooterStyle-Width="150">
                    <ItemTemplate>
                        <asp:Label ID="lblField" runat="server" Text='<%# Eval("Field") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddlField" runat="server" CssClass="ddlist order" ></asp:DropDownList>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:DropDownList ID="ddlField" runat="server" CssClass="ddlist order"></asp:DropDownList>
                    </FooterTemplate>
                </asp:TemplateField>
                
                   <asp:TemplateField HeaderText="Operator" ItemStyle-Width="50" FooterStyle-Width="60">
                    <ItemTemplate>
                        <asp:Label ID="lblOperator" runat="server" Text='<%# Eval("Operator") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddlOperator" runat="server" Text='<%# Eval("Operator") %>' CssClass="ddlist order">
                              <asp:ListItem></asp:ListItem>
                            <asp:ListItem>=</asp:ListItem> 
                             <asp:ListItem>!=</asp:ListItem>
                               <asp:ListItem>&gt;</asp:ListItem>
                             <asp:ListItem>&lt;</asp:ListItem>
                             <asp:ListItem>&gt;=</asp:ListItem>
                             <asp:ListItem>&lt;=</asp:ListItem>
                             <asp:ListItem>LIKE</asp:ListItem>
                             <asp:ListItem>NOT LIKE</asp:ListItem>    
                        </asp:DropDownList>
                    </EditItemTemplate>
                    <FooterTemplate>
                         <asp:DropDownList ID="ddlOperator" runat="server" CssClass="ddlist order">
                              <asp:ListItem></asp:ListItem>
                            <asp:ListItem>=</asp:ListItem> 
                             <asp:ListItem>!=</asp:ListItem>
                              <asp:ListItem>&gt;</asp:ListItem>
                             <asp:ListItem>&lt;</asp:ListItem>
                             <asp:ListItem>&gt;=</asp:ListItem>
                             <asp:ListItem>&lt;=</asp:ListItem>
                             <asp:ListItem>LIKE</asp:ListItem>
                             <asp:ListItem>NOT LIKE</asp:ListItem>    
                        </asp:DropDownList>
                    </FooterTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Value" ItemStyle-Width="200" FooterStyle-Width="200">
                    <ItemTemplate>
                        <asp:Label ID="lblValue" runat="server" Text='<%# Eval("Value") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtValue" runat="server" Text='<%# Eval("Value") %>' CssClass="textbox_dynamic"></asp:TextBox>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txtValue" runat="server" CssClass="textbox_dynamic"></asp:TextBox>
                    </FooterTemplate>
                </asp:TemplateField>
                
                 <asp:TemplateField HeaderText="" ItemStyle-Width="50" FooterStyle-Width="60">
                    <ItemTemplate>
                        <asp:Label ID="lblRightPar" runat="server" Text='<%# Eval("RightParenthesis") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddlRightPar" runat="server" Text='<%# Eval("RightParenthesis") %>' CssClass="ddlist order">
                              <asp:ListItem></asp:ListItem>
                            <asp:ListItem>)</asp:ListItem> 
                        </asp:DropDownList>
                    </EditItemTemplate>
                    <FooterTemplate>
                         <asp:DropDownList ID="ddlRightPar" runat="server" CssClass="ddlist order">
                              <asp:ListItem></asp:ListItem>
                            <asp:ListItem>)</asp:ListItem> 
                        </asp:DropDownList>
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
      <p class="total">
        <asp:Label ID="lblTotal" runat="server"></asp:Label>
    </p>

      <asp:GridView ID="gvResult" runat="server" DataKeyNames="computer_id" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="computer_name" HeaderText="Computer Name"></asp:BoundField>
        </Columns>
        <EmptyDataTemplate>
            No Results Found
        </EmptyDataTemplate>
    </asp:GridView>
  
   
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>This page is only available when the group type is set to dynamic.  This is used to set the criteria for the group members.  Criteria is based on any inventory collection value or custom inventory script value or custom attribute value.  Each line must be started with and / or, except for the first line.  If using the not operator, there can be only one and it must be first.  The example below would find all computers that have Firefox installed and the major version is less than 63.  A wildcard % can be used before or after the string to match.  The Actions menu has option to test the criteria before saving it.  Dynamic groups update themselves every 3 hours based on the Admin Settings->Task Scheduler->Cron Expression.  You can update this value to be more or less often.</p>
</asp:Content>
