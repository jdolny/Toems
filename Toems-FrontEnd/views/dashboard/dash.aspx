<%@ Page Title="" Language="C#" MasterPageFile="~/views/site.master" AutoEventWireup="true" Inherits="Toems_FrontEnd.views.dashboard.Dashboard" ValidateRequest="false" Codebehind="dash.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle">
Dashboard
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="SubNav" runat="Server">
   
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    
     <p class="denied_text">
        <asp:Label ID="lblDenied" runat="server"></asp:Label>
    </p>

   <div class="dash_item_graph">
                <canvas id="myChart" width="1000" height="250" ></canvas>
            </div>
    <br />
   

    <asp:PlaceHolder runat="server" Id="pinnedPolicyHolder"></asp:PlaceHolder>



    <asp:PlaceHolder runat="server" Id="pinnedGroupsHolder"></asp:PlaceHolder>
   
        <br class="clear" />

     <div id="divLocalStorage" runat="server">
         <asp:PlaceHolder runat="server" Id="comServerStorageHolder"></asp:PlaceHolder>
    </div>

    <div id="divRemoteStorage" runat="server">
    <div class="dash_item">
          <div class="dash_item_content">
                <span class="dash_item_category">System </span>
                 <br class="clear" />
        <span class="dash_item_title">Remote Storage - <%= RemotePath %></span>
        <br class="clear" />
        <asp:Label ID="lblDPfree" runat="server"></asp:Label>
        <asp:Label ID="lblDpTotal" runat="server"></asp:Label>
        <br class="clear"/>
        <asp:Label runat="server" ID="lblRemotePercent"></asp:Label>
    </div>
        </div>
    </div>


    
    <script type="text/javascript">
        $(document).ready(function () {

            $('.actions_left').addClass("display-none");
            $('#nav-dash').addClass("nav-current");
        });

    </script>

    <script>
var ctx = document.getElementById("myChart");
var myChart = new Chart(ctx, {
    type: 'line',
    data: {
        labels: ["Last Hour","2 Hours","3 Hours","4 Hours","5 Hours","6 Hours","7 Hours","8 Hours","9 Hours","10 Hours","11 Hours","12 Hours"],
        datasets: [{
            label: 'Checkins Per Hour - Last 12 Hours',
            data: "<%= CheckinData %>".split(','),
           
            borderColor: "#80b6f4",
            pointBorderColor: "#80b6f4",
            pointBackgroundColor: "#80b6f4",
            pointHoverBackgroundColor: "#80b6f4",
            pointHoverBorderColor: "#80b6f4",
            pointBorderWidth: 5,
            pointHoverRadius: 7,
            pointHoverBorderWidth: 1,
            pointRadius: 2,
            fill: false,
            borderWidth: 3,
        }]
    },
    
});
</script>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="Help">
    <iframe width="100%" height="400" src="https://docs.theopenem.com/latest" frameborder="0" ></iframe>
    
</asp:Content>