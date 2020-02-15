<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/admin.master" AutoEventWireup="true" CodeBehind="client.aspx.cs" Inherits="Toems_FrontEnd.views.admin.client" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>Toec Client Settings</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    Admin Settings
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" runat="Server">
    <li><asp:LinkButton ID="btnUpdateSettings" runat="server" Text="Update Client Settings" OnClick="btnUpdateSettings_OnClick" CssClass="main-action"/></li>
</asp:Content>





<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#client').addClass("nav-current");
        });
    </script>
     <div class="size-4 column">
            Startup Delay Type:
        </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlStartupDelay" runat="server" CssClass="ddlist" AutoPostBack="True" OnSelectedIndexChanged="ddlStartupDelay_OnSelectedIndexChanged"/>
        </div>
        <br class="clear"/>
     <div id="divDelayFilePath" runat="server">
            <div class="size-4 column">
                File Path:
            </div>
            <div class="size-5 column">
                <asp:TextBox ID="txtStartupFilePath" runat="server" CssClass="textbox"></asp:TextBox>
            </div>
            <br class="clear" />
        </div>
        <div id="divDelayTime" runat="server">
            <div class="size-4 column">
                Time (Seconds):
            </div>
            <div class="size-5 column">
                <asp:TextBox ID="txtDelayTime" runat="server" CssClass="textbox"></asp:TextBox>
            </div>
            <br class="clear" />
        </div>
 
        <div class="size-4 column">
            Threshold Window (Seconds):
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtThreshold" runat="server" CssClass="textbox"></asp:TextBox>
        </div>
        <br class="clear"/>
    
          <div class="size-4 column">
            Checkin Interval (Minutes):
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtCheckin" runat="server" CssClass="textbox"></asp:TextBox>
        </div>
    
           <br class="clear"/>
    
          <div class="size-4 column">
            Shutdown Delay (Seconds):
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtShutdownDelay" runat="server" CssClass="textbox"></asp:TextBox>
        </div>

        <br class="clear"/>
      
       
     <br class="clear"/>
   
      <div class="size-4 column">
            Upload New Client MSI Version
        </div>
    <br class="clear"/>
     <script type="text/template" id="qq-template">
        <div class="qq-uploader-selector qq-uploader" qq-drop-area-text="Drop files here">
            <div class="qq-total-progress-bar-container-selector qq-total-progress-bar-container">
                <div role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" class="qq-total-progress-bar-selector qq-progress-bar qq-total-progress-bar"></div>
            </div>
            <div class="qq-upload-drop-area-selector qq-upload-drop-area" qq-hide-dropzone>
                <span class="qq-upload-drop-area-text-selector"></span>
            </div>
            <div class="buttons">
                <div class="qq-upload-button-selector qq-upload-button">
                    <div>Select files</div>
                </div>
                <button type="button" id="trigger-upload" class="btn btn-primary">
                    <i class="icon-upload icon-white"></i> Upload
                </button>
            </div>
            <span class="qq-drop-processing-selector qq-drop-processing">
                <span>Processing dropped files...</span>
                <span class="qq-drop-processing-spinner-selector qq-drop-processing-spinner"></span>
            </span>
            <ul class="qq-upload-list-selector qq-upload-list" aria-live="polite" aria-relevant="additions removals">
                <li>
                    <div class="qq-progress-bar-container-selector">
                        <div role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" class="qq-progress-bar-selector qq-progress-bar"></div>
                    </div>
                    <span class="qq-upload-spinner-selector qq-upload-spinner"></span>
                    <img class="qq-thumbnail-selector" qq-max-size="100" qq-server-scale>
                    <span class="qq-upload-file-selector qq-upload-file"></span>
                    <span class="qq-edit-filename-icon-selector qq-edit-filename-icon" aria-label="Edit filename"></span>
                    <input class="qq-edit-filename-selector qq-edit-filename" tabindex="0" type="text">
                    <span class="qq-upload-size-selector qq-upload-size"></span>
                    <button type="button" class="qq-btn qq-upload-cancel-selector qq-upload-cancel">Cancel</button>
                    <button type="button" class="qq-btn qq-upload-retry-selector qq-upload-retry">Retry</button>
                    <button type="button" class="qq-btn qq-upload-delete-selector qq-upload-delete">Delete</button>
                    <span role="status" class="qq-upload-status-text-selector qq-upload-status-text"></span>
                </li>
            </ul>

            <dialog class="qq-alert-dialog-selector">
                <div class="qq-dialog-message-selector"></div>
                <div class="qq-dialog-buttons">
                    <button type="button" class="qq-cancel-button-selector">Close</button>
                </div>
            </dialog>

            <dialog class="qq-confirm-dialog-selector">
                <div class="qq-dialog-message-selector"></div>
                <div class="qq-dialog-buttons">
                    <button type="button" class="qq-cancel-button-selector">No</button>
                    <button type="button" class="qq-ok-button-selector">Yes</button>
                </div>
            </dialog>

            <dialog class="qq-prompt-dialog-selector">
                <div class="qq-dialog-message-selector"></div>
                <input type="text">
                <div class="qq-dialog-buttons">
                    <button type="button" class="qq-cancel-button-selector">Cancel</button>
                    <button type="button" class="qq-ok-button-selector">Ok</button>
                </div>
            </dialog>
        </div>
    </script>
      <style>
  

      
          </style>
    
    
   
             <asp:HiddenField ID="uploadErrorMessage" runat="server"/>
            <script type="text/javascript">


                $(document).ready(function () {

                    var myToken = "<%= token %>";
                    var myBaseUrl = "<%= baseurl %>";

                    var uploader = new qq.FineUploader({
                        debug: true,
                        element: document.getElementById('uploader'),
                        request: {
                            endpoint: myBaseUrl + 'Upload/UploadClientMsi',
                            customHeaders: {
                                "Authorization": "Bearer " + myToken
                            },
                            params: {
                                uploadMethod: "standard",
                            }
                        },

                        retry: {
                            autoAttemptDelay: 2,
                            enableAuto: true,
                            maxAutoAttempts: 10
                        },
                        chunking: {
                            enabled: true,
                            partSize: 4096000,
                            
                        },
                        thumbnails: {
                            placeholders: {
                                waitingPath: "<%= ResolveUrl("~/content/js/fineuploader/placeholders/waiting-generic.png") %>",
                                notAvailablePath: "<%= ResolveUrl("~/content/js/fineuploader/placeholders/file-3.png") %>"
                            }
                        },
                      
                        autoUpload: false,
                        callbacks: {
                           
                            onError: function (id, name, errorReason, xhrOrXdr) {

                                document.getElementById('<%= uploadErrorMessage.ClientID %>').value = errorReason;
                                
                            }

                        }
                    });

                    qq(document.getElementById("trigger-upload")).attach("click", function () {
                        uploader.uploadStoredFiles();
                    });
                });
            </script>
    
      <div id="uploader"></div>
        <br class="clear" />
    <br />
        <div class="size-4 column">
            Client MSI Arguments:
        </div>
   
    <br class="clear" />
     
        <div class="size-5 column">
            <asp:TextBox ID="txtArguments" runat="server" CssClass="descbox" TextMode="MultiLine" Style="font-size:12px;"></asp:TextBox>
        </div>
        <br class="clear"/>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <h5><span style="color: #ff9900;">Startup Delay Type:</span></h5>
<p>When the Toec service starts, an optional delay can be configured to delay the initial checkin time.  The delay can be specified in seconds or by a file condition.  A file condition means that Toec will not checkin until the specified File Path exists.  An example usage for the File Condition might be a long Sysprep workflow.  You might not want Toec to checkin until everything else is completed.  One of the last steps in your Sysprep workflow would be to create this file and the specified File Path Condition.</p>
<h5><span style="color: #ff9900;">Threshold Window:</span></h5>
<p>The threshold window is used to try and prevent all of computers from checking in at the same time.  If the Window value is greater than 0, each client will wait to checkin by selecting a random number b/w 0 and the value specified in seconds before checking in.</p>
<h5><span style="color: #ff9900;">Checkin Interval:</span></h5>
<p>This specifies how often computers should Checkin.  The recommended value is 60 minutes.</p>
<h5><span style="color: #ff9900;">Shutdown Delay:</span></h5>
<p>When a computer or group is issued a reboot or shutdown command via the actions menu, this will delay the shutdown by the provided value on the computer, giving a user time to save work if they are still logged in.  This does not apply to a Policy's reboot option.</p>
<h5><span style="color: #ff9900;">Upload New Client MSI Version:</span></h5>
<p>When updates to Toec are available, the are uploaded here.  Existing Toec endpoints will automatically update themselves at the next checkin.</p>
<h5><span style="color: #ff9900;">Client MSI Arguments:</span></h5>
<p>Displays the arguments that must be passed to the Toec MSI installer.</p>
</asp:Content>
