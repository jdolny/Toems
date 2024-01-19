<%@ Page Title="" Language="C#" MasterPageFile="~/views/policies/policies.master" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeBehind="general.aspx.cs" Inherits="Toems_FrontEnd.views.policies.general" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li><%= Policy.Name %></li>
    <li>General</li>
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
              $('#general').addClass("nav-current");
          });
    </script>
     <div class="size-4 column">
        Name:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtName" runat="server" CssClass="textbox"></asp:TextBox>
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
        Execution Type:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlExecType" runat="server" CssClass="ddlist"></asp:DropDownList>
    </div>
    <br class="clear"/>

     <div class="size-4 column">
        Completed Action:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlCompletedAction" runat="server" CssClass="ddlist"></asp:DropDownList>
    </div>
    <br class="clear"/>
   
     <div class="size-4 column">
        Error Action:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlErrorAction" runat="server" CssClass="ddlist"></asp:DropDownList>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Delete Cache:
    </div>
     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkDeleteCache" runat="server" ClientIDMode="Static"></asp:CheckBox>
            <label for="chkDeleteCache"></label>
        </div>
   
    <br class="clear"/>
    <div class="size-4 column">
        Auto Archive:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlAutoArchive" runat="server" CssClass="ddlist" AutoPostBack="True" OnSelectedIndexChanged="ddlAutoArchive_OnSelectedIndexChanged"></asp:DropDownList>
    </div>
    <br class="clear"/> <div id="divArchiveDays" runat="server">
        <div class="size-4 column">
            Days: 
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtAutoArchiveDays" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>
        </div>
        <br class="clear" />
    </div>
       <br class="clear"/>

    <div class="size-4 column">
        Log Level:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlLogLevel" runat="server" CssClass="ddlist"></asp:DropDownList>
    </div>
     <div class="size-4 column">
        Skip Server Logging Result:
    </div>
     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkSkipResult" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkSkipResult"></label>
        </div>

<br class="clear"/>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
<h5><span style="color: #ff9900;">Name:</span></h5>
<p>The name of the policy, module names must be unique and contain only alphanumeric characters, space, underscore, or dash.</p>
<h5><span style="color: #ff9900;">Description:</span></h5>
<p>The description field is optional for you to give a short description for what the policy does.</p>
<h5><span style="color: #ff9900;">Trigger:</span></h5>
<p>The trigger defines at which point of the client process the Policy should run, there are four options.  StartupOrCheckin, Startup, Checkin, and Login.  This also helps define what user the policy will run as.  The decision to run the policy or not will be a combination of Trigger, Frequency, Window Schedule, Start Date, and client policy history.</p>
<ul>
	<li><strong>StartupOrCheckin </strong>- The policy will attempt to run any time the computer or service starts or checks in.  All modules will run as the local system account unless the module's run as field is changed.</li>
	<li><strong>Startup</strong> - The policy will only attempt to run when the computer or service starts.  All modules will run as the local system account unless the module's run as field is changed.</li>
	<li><strong>Checkin</strong> - The policy will only attempt to run when doing a recurring checkin, this excludes starting the service or computer.  All modules will run as the local system account unless the module's run as field is changed.</li>
	<li><strong>Login</strong> - The policy will only attempt to run when a user logs in.  All modules will run in the context of that user, the user will need the appropriate permissions for whatever the modules are doing</li>
</ul>
<h5><span style="color: #ff9900;">Frequency:</span></h5>
<p>The frequency defines how often a policy should run relative to the trigger type.</p>
<ul>
	<li><strong>Ongoing</strong> - The policy will run every time the trigger type is hit</li>
	<li><strong>OncePerComputer</strong> - The policy will only run 1 time on the endpoint</li>
	<li><strong>OncePerDay</strong> - The policy will run once per day.  A new day is considered 12:00am, not 24 hours.  It is possible a policy could run at 11:00pm and then 1 hour later at 12:00am.</li>
	<li><strong>OncePerWeek</strong> - The policy will run once per calendar week.  Two additional sub options are available for this frequency.  Day of Week and Frequency Missed Action.</li>
	<li><strong>OncePerMonth</strong> - The policy will run once per calendar month.  Two aditional sub options are available for this frequency.  Day of Month and Frequency Missed Action.  Setting the day of month to 31, means to run on the last day of the month.</li>
	<li><strong>EveryXhours</strong> - The policy will run on an hourly interval that you define.  The additional sub option, Hour Interval is available for this Frequency.</li>
	<li><strong>EveryXdays</strong> - The policy will run on a daily interval that you define.  The additional sub option, Hour Interval is available for this Frequency.</li>
</ul>
<h5><span style="color: #ff9900;">Frequency Missed Action:</span></h5>
<p>The frequency missed action is a sub option that is only available when the Frequency is set to OncePerWeek or OncePerMonth.</p>
<ul>
	<li>NextOpportunity - If a policy frequency is missed by an endpoint, it will run at the next opportunity.</li>
	<li>ScheduleDayOnly - If a policy frequency is missed by an endpoint, it will only if it's day of week or day of month sub option match the current day.</li>
</ul>
<h5><span style="color: #ff9900;">Day Of Week:</span></h5>
<p>The day of week is sub option that is available when the Frequency is set to OncePerWeek.  This sets the day of the week the policy should run on.</p>
<h5><span style="color: #ff9900;">Day Of Month:</span></h5>
<p>The day of month is sub option that is available when the Frequency is set to OncePerMonth.  This sets the day of the month the policy should run on.</p>
<h5><span style="color: #ff9900;">Hour Interval:</span></h5>
<p>The hour interval is sub option that is available when the Frequency is set to EveryXhours.  The hour interval value replaces the X.  An hour interval value of 5 would run the policy every 5 hours.</p>
<h5><span style="color: #ff9900;">Day Interval:</span></h5>
<p>The day interval is sub option that is available when the Frequency is set to EveryXdays.  The day interval value replaces the X.  A day interval value of 3 would run the policy every 3 days.</p>
<h5><span style="color: #ff9900;">Window Start Schedule:</span></h5>
<p>The window start schedule is a way to provide more granular control over the frequency of a policy.  Schedules allow you to define specific days the policy should run, and specific time frames.  Schedules can be created in Global Properties-&gt;Schedules.  When using start and end schedules on a policy, both the start and end schedule must be provided, if only 1 is assigned, it will be ignored.  Finally, when evaluating the days to run on, only the start schedule is used.  The start schedule is also used to specify the starting time range.</p>
<h5><span style="color: #ff9900;">Window End Schedule:</span></h5>
<p>The window end schedule is a way to provide more granular control over the frequency of a policy.  Schedules allow you to define specific days the policy should run, and specific time frames.  Schedules can be created in Global Properties-&gt;Schedules.  When using start and end schedules on a policy, both the start and end schedule must be provided, if only 1 is assigned, it will be ignored.  Finally, when evaluating the days to run on, only the start schedule is used.  The end schedule is only used to provide the ending time range.</p>
<h5><span style="color: #ff9900;">Start Date:</span></h5>
<p>Specifies the starting date the policy can be used.  The start date can be dated in the future to plan policies that will execute at a later time.  When using a date in the future, the policy must still be activated prior to that date.</p>
<h5><span style="color: #ff9900;">Completed Action:</span></h5>
<p>Specifies the action to take after a policy has completed successfully, does not apply to a failed policy.</p>
<ul>
	<li><strong>DoNothing</strong> - Don't do anything, continue on to the next policy if any.</li>
	<li><strong>Reboot</strong> - Reboots the computer immediately after the policy is complete.</li>
</ul>
<h5><span style="color: #ff9900;">Collect Inventory:</span></h5>
<p>Specifies if the policy should collect inventory information about the endpoint.  There are four options.</p>
<ul>
	<li><strong>Disabled</strong> - Don't collect inventory with the policy.</li>
	<li><strong>Before</strong> - Collect inventory before any assigned modules are run.</li>
	<li><strong>After</strong> - Collect inventory after all assigned modules are run.</li>
	<li><strong>Both</strong> - Collect inventory both before and after modules are run.</li>
</ul>
<h5><span style="color: #ff9900;">Login Tracker:</span></h5>
<p>If this option is enabled, the policy will record the date and time of when a user logs in and out of an endpoint.</p>
<h5><span style="color: #ff9900;">Application Monitor:</span></h5>
<p>If this option is enabled, the policy will record the date, time, and user of all application loads or exits.</p>
<h5><span style="color: #ff9900;">Install Available Windows Updates:</span></h5>
<p>This option can be used to install all available updates from Microsoft or a SUS server.  This differs from a Windows Update module because it installs all available updates, where as a Windows update module installs a specific update.  The Windows Update module has no relation to this setting.</p>
<ul>
	<li><strong>Disabled</strong> - The policy does not install any windows updates.</li>
	<li><strong>MicrosoftSkipUpgrades</strong> - The endpoint will reach out to Microsoft's update servers to install all available updates, skipping feature upgrades.</li>
	<li><strong>WsusSkipUpgrades</strong> - The endpoint will reach out to the designated SUS server to install all approved updates, skipping feature upgrades.</li>
	<li><strong>Microsoft</strong> - The endpoint will reach out to Microsoft's update servers to install all available updates, including feature upgrades.</li>
	<li><strong>Wsus</strong> - The endpoint will reach out to the designated SUS server to install all approved updates, including feature upgrades.</li>
</ul>
<h5><span style="color: #ff9900;">Execution Type:</span></h5>
<p>Policies that contain software,script, file copy, or windows update modules, always copy all of the required files to the toec appdata folder for the entire policy before installing or running the commands.  Setting the execution type to Cache will stop the policy after this occurs, preventing anything from installing.  If you want to cache files ahead of time to make for a faster installation later, this is how you do it.  After you have finished caching all the required files on your endpoints, flip this setting back to Install, to perform the installation.</p>
<h5><span style="color: #ff9900;">Error Action:</span></h5>
<p>Specifies what should happen if the policy encounters an error.</p>
<ul>
	<li><strong>AbortCurrentPolicy</strong> - Stops the current policy but continues with any additional policies that may apply.</li>
	<li><strong>AbortRemainingPolicies</strong> - Stops the current policy and does not load any additional policies.</li>
	<li><strong>Continue</strong> - Continues processing the current policy and any additional policies.</li>
</ul>
<h5><span style="color: #ff9900;">Delete Cache:</span></h5>
<p>If the policy completed successfully, any files that were copied to the endpoint are removed if this option is enabled.  This only applies when the execution type is set to Install.</p>
<h5><span style="color: #ff9900;">Log Level:</span></h5>
<p>Defines the endpoint log level for the policy.  The endpoint logs are written to program files\toec\logs on each endpoint.</p>
<ul>
	<li>Full - Everything for the policy is written to the log file, same as DEBUG.</li>
	<li>HiddenArguments - Hides sensitive information from the log file, same as INFO.</li>
	<li>None - Does not log anything about the policy, same as OFF.</li>
</ul>
<h5><span style="color: #ff9900;">Auto Archive:</span></h5>
<p>An option to automatically archive the policy after a condition.</p>
<ul>
	<li><strong>None</strong> - Auto archive is disabled for the policy.</li>
	<li><strong>WhenComplete</strong> - Archives the policy when all endpoints in the target group have reported success.</li>
	<li><strong>AfterXdays</strong> - Archives the policy after an interval of days, the sub option Days, is available when this option is selected.</li>
</ul>
<h5><span style="color: #ff9900;">Days:</span></h5>
<p>When Auto Archive is set to AfterXdays, this is number of days that must pass since the Policy Start Date, before it is archived.</p>
<h5><span style="color: #ff9900;">Skip Server Logging Result:</span></h5>
<p>Each time an endpoint runs a policy, the result is recorded and stored in the database, viewable under the policie's client history.  Some policies don't make sense to capture this information.  For example, if a policy was setup to collect the inventory of an endpoint once per day, and Theopenem contained 5000 endpoints, the Client History log would get 5000 entries per day of information that isn't really needed.  On the other hand, if deploying a software application to those same endpoints, it would only run once per computer and we could then easily determine which endpoints successfully installed the software if we don't skip the server logging result.</p>
<h5><span style="color: #ff9900;">Com Server Condition:</span></h5>
<p>Policies can be setup to only run when the endpoint is connected to specific Com Servers.  The default value for this field is Any, meaning the policy will run regardless of the Com Server.  The other option is Selective.  If this is set to selective, a sub option will appear to select the Com Servers this should policy should be allowed to run from.  The best use case for this is a Com Server in the DMZ.  Theopenem can manage endpoints outside of your network if a Com Server is setup in your DMZ.  There may be policies that require internal resources in order to complete.  If the policy requires internal resources, then you could set the Com Server Condition to Selective, and check all Com Servers except the one in the DMZ.  This will prevent the policy from running until the computer makes it back to your internal network.</p>
</asp:Content>

