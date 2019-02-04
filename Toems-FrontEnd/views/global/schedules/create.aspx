<%@ Page Title="" Language="C#" MasterPageFile="~/views/global/schedules/schedules.master" AutoEventWireup="true" CodeBehind="create.aspx.cs" Inherits="Toems_FrontEnd.views.global.schedules.create" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li>New</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
Schedules
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="buttonAddSchedule" runat="server" OnClick="buttonAddSchedule_OnClick" Text="Add Schedule" CssClass="main-action"/></li>
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#create').addClass("nav-current");
        });
    </script>
    <div class="size-4 column">
        Name:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtName" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>

    </div>
    <br class="clear"/>
    <div class="size-4 column">
       Description:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtDescription" runat="server" CssClass="descbox" TextMode="MultiLine"></asp:TextBox>
    </div>


    <br class="clear"/>
     <div class="size-4 column">
        Sunday:
    </div>
     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkSunday" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkSunday"></label>
        </div>
     <br class="clear"/>
    
     <div class="size-4 column">
        Monday:
    </div>
     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkMonday" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkMonday"></label>
        </div>
     <br class="clear"/>
    
     <div class="size-4 column">
        Tuesday:
    </div>
     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkTuesday" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkTuesday"></label>
        </div>
     <br class="clear"/>
    
     <div class="size-4 column">
        Wednesday:
    </div>
     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkWednesday" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkWednesday"></label>
        </div>
     <br class="clear"/>
    
     <div class="size-4 column">
        Thursday:
    </div>
     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkThursday" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkThursday"></label>
        </div>
     <br class="clear"/>
    
     <div class="size-4 column">
        Friday:
    </div>
     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkFriday" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkFriday"></label>
        </div>
     <br class="clear"/>
    
      <div class="size-4 column">
        Saturday:
    </div>
     <div class="size-setting column hidden-check" >
            <asp:CheckBox ID="chkSaturday" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkSaturday"></label>
        </div>
     <br class="clear"/>
    

         <div class="size-4 column">
        Hour:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlHour" runat="server" CssClass="ddlist"></asp:DropDownList>
    </div>
    <br class="clear"/>
       <div class="size-4 column">
        Minute:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlMinute" runat="server" CssClass="ddlist">
      
        </asp:DropDownList>
    </div>
    <br class="clear"/>
    
      
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>A schedule is mostly straight forward.  Select the days and the time the schedule should run.  A schedule can also temporarily be deactivated to disable it.</p>
</asp:Content>