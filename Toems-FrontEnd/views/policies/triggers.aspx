<%@ Page Title="" Language="C#" MasterPageFile="~/views/policies/policies.master" AutoEventWireup="true" CodeBehind="triggers.aspx.cs" Inherits="Toems_FrontEnd.views.policies.triggers" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li><%= Policy.Name %></li>
    <li>Triggers</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
        <%= Policy.Name %>
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_OnClick" Text="Update Policy" CssClass="main-action"></asp:LinkButton></li>
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" Runat="Server">
      <script type="text/javascript">
          $(document).ready(function () {
              $('#triggers').addClass("nav-current");
          });
          $(function () {
              $("#txtStartDate").datepicker();
          });
      </script>
   
     <div class="size-4 column">
        Trigger:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlTrigger" runat="server" CssClass="ddlist" AutoPostBack="True" OnSelectedIndexChanged="ddlTrigger_OnSelectedIndexChanged"></asp:DropDownList>
    </div>
    <br class="clear"/>
  
     <div class="size-4 column">
        Frequency:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlFrequency" runat="server" CssClass="ddlist" AutoPostBack="True" OnSelectedIndexChanged="ddlFrequency_OnSelectedIndexChanged"></asp:DropDownList>
    </div>
    <br class="clear"/>  

    <div id="divWeekly" runat="server">
         <div class="size-4 column">
        Day Of Week:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlWeekDay" runat="server" CssClass="ddlist"></asp:DropDownList>
    </div>
    <br class="clear"/>
    </div>
    
    <div id="divMonthly" runat="server">
        <div class="size-4 column">
        Day Of Month:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlMonthDay" runat="server" CssClass="ddlist"></asp:DropDownList>
    </div>
    <br class="clear"/>
       
    </div>
    <div id="divXHours" runat="server">
        <div class="size-4 column">
            Hour Interval:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtHours" runat="server" CssClass="textbox" ></asp:TextBox>
        </div>
        <br class="clear"/>
    </div>
    <div id="divXDays" runat="server">
        <div class="size-4 column">
            Day Interval:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtDays" runat="server" CssClass="textbox"></asp:TextBox>
        </div>
        <br class="clear"/>
    </div>
    <br class="clear"/>
     <div id="divMissed" runat="server">
         <div class="size-4 column">
        Frequency Missed Action:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlMissed" runat="server" CssClass="ddlist"></asp:DropDownList>
    </div>
    <br class="clear"/>
    </div>
    <div class="size-4 column">
        Window Start Schedule:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlScheduleStart" runat="server" CssClass="ddlist"/>
    </div>
    
    <br class="clear"/>
    <div class="size-4 column">
        Window End Schedule:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlScheduleEnd" runat="server" CssClass="ddlist"/>
    </div>
    <br class="clear" />
    
    <div class="size-4 column">
        Start Date:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtStartDate" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>
    </div>
    <br class="clear"/>
    
   
    </asp:Content>