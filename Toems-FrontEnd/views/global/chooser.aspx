<%@ Page Title="" Language="C#" MasterPageFile="~/views/global/global.master" AutoEventWireup="true" CodeBehind="chooser.aspx.cs" Inherits="Toems_FrontEnd.views.global.chooser" %>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
 Global Properties
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavLevel1" ID="subNavLevel1">
     <ul class="ul-secondary-nav">
               <li id="schedules" runat="server" clientidmode="Static">
                <a href="<%= ResolveUrl("~/views/global/schedules/search.aspx") %>">
                    <span class="nav-text">Schedules</span>
                </a>
            </li>
           
         <li id="categories" runat="server" clientidmode="Static">
             <a href="<%= ResolveUrl("~/views/global/categories/search.aspx") %>">
                 <span class="nav-text">Categories</span>
             </a>
         </li>
    
       
        </ul>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubContent">
    <script type="text/javascript">
        $(document).ready(function() {
            $('.actions_left').addClass("display-none");

        });
    </script>
</asp:Content>

