<%@ Page Title="" Language="C#" MasterPageFile="~/views/reports/computer/computer.master" AutoEventWireup="true" CodeBehind="custom.aspx.cs" Inherits="Toems_FrontEnd.views.reports.computer.custom" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TopBreadCrumbSub2" runat="server">
    <li>Custom</li>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SubNavTitle_Sub2" runat="server">
    Computer Reports
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DropDownActionsSub2" runat="server">
    <li><asp:LinkButton ID="btnTestQuery" runat="server" OnClick="btnTestQuery_OnClick" Text="Run Query" CssClass="main-action"></asp:LinkButton></li>
     <li><asp:LinkButton ID="btnRawSql" runat="server" OnClick="btnRawSql_Click" Text="Run SQL Query" CssClass="main-action"></asp:LinkButton></li>
    <li><asp:LinkButton ID="btnExport" runat="server" OnClick="btnExport_OnClick" Text="Export To CSV"></asp:LinkButton></li>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SubContent2" runat="server">
 <script type="text/javascript">
          $(document).ready(function () {
              $('#custom').addClass("nav-current");
          });
         
    </script>
   
        <asp:TextBox ID="txtCustomSql" runat="server" TextMode="MultiLine"></asp:TextBox>
        <asp:GridView ID="gvNetBoot" runat="server" AutoGenerateColumns="false"  CssClass="Gridview extraPad"
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

                <asp:TemplateField HeaderText="Value" ItemStyle-Width="150" FooterStyle-Width="150">
                    <ItemTemplate>
                        <asp:Label ID="lblValue" runat="server" Text='<%# Eval("Value") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtValue" runat="server" Text='<%# Eval("Value") %>' CssClass="textbox_dynamic order"></asp:TextBox>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txtValue" runat="server" CssClass="textbox_dynamic"></asp:TextBox>
                    </FooterTemplate>
                </asp:TemplateField>
                
                 <asp:TemplateField HeaderText="" ItemStyle-Width="50" FooterStyle-Width="60">
                    <ItemTemplate>
                        <asp:Label ID="lblRightPar" runat="server" Text='<%# Eval("RightParenthesis") %>' CssClass="ddlist order"></asp:Label>
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
    
    <div class="size-10 column hidden-check">
        Include Archived
        <asp:CheckBox runat="server" ID="chkArchived" Text="Include Archived" />
    </div>
    <div class="size-10 column hidden-check">
        Include Pre-Provisioned
        <asp:CheckBox runat="server" Id="chkPre" Text="Include Pre-Provisioned"/>
    </div>
    <div class="size-10 column">
        &nbsp;
    </div>
    <div class="size-14 column">
        Group By
        <asp:DropDownList runat="server" ID="ddlGroupBy" CssClass="ddlist order"/>
    </div>

    <br/>
    
    <br class="clear"/>
      <p class="total">
        <asp:Label ID="lblTotal" runat="server"></asp:Label>
    </p>

      <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="true" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
       
        <EmptyDataTemplate>
            No Results Found
        </EmptyDataTemplate>
    </asp:GridView>
  
   
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>A custom computer report will export a list of computers and attributes based on the criteria provided.  Any computer inventory attribute, as well as custom inventory modules, or custom attributes can be used to query on.  The value can use a wildcard value of % before or after the string.  You can also select if you want to include Archived and Pre-Provisioned computers in the results.  Finally, a group by option is available for any selected field.</p>
</asp:Content>

