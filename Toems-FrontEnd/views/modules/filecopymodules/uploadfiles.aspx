<%@ Page Title="" Language="C#" MasterPageFile="~/views/modules/filecopymodules/filecopymodule.master" AutoEventWireup="true" CodeBehind="uploadfiles.aspx.cs" Inherits="Toems_FrontEnd.views.modules.filecopymodules.uploadfiles" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li><%= FileCopyModule.Name %></li>
    <li>Upload Files</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
<%= FileCopyModule.Name %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <asp:Button runat="server" ID="UploadButton" Text="" style="display:none;" OnClick="UploadButton_OnClick" ClientIDMode="Static" />
       <asp:Button runat="server" ID="ErrorButton" Text="" style="display:none;" OnClick="ErrorButton_OnClick" ClientIDMode="Static" />
    <script type="text/javascript">
        $(document).ready(function() {
            $('#upload').addClass("nav-current");
        });
    </script>
    
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
             

                $(document).ready(function() {

                    var myToken = "<%= token %>";
                    var myBaseUrl = "<%= baseurl %>";
                    var moduleGuid = "<%= moduleGuid %>";
                    var moduleId = "<%= moduleId %>";

                    var uploader = new qq.FineUploader({
                        debug: true,
                        element: document.getElementById('uploader'),
                        request: {
                            endpoint: myBaseUrl + 'Upload/UploadFile',
                            customHeaders: {
                                "Authorization": "Bearer " + myToken
                            },
                            params: {
                                uploadMethod: "standard",
                                moduleGuid: moduleGuid,
                                moduleId: moduleId,
                                moduleType: "4" //moduletype enum filecopy = 4
                            },
                          
                        },

                        retry: {
                            autoAttemptDelay: 2,
                            enableAuto: true,
                            maxAutoAttempts: 10
                        },
                        chunking: {
                            enabled: true,
                            partSize: 4096000,
                            success: {
                                endpoint: myBaseUrl + 'Upload/ChunkingComplete'
                            }
                        },
                        thumbnails: {
                            placeholders: {
                                waitingPath: "<%= ResolveUrl("~/content/js/fineuploader/placeholders/waiting-generic.png") %>",
                                notAvailablePath: "<%= ResolveUrl("~/content/js/fineuploader/placeholders/file-3.png") %>"
                            }
                        },
                        validation: {
                            itemLimit: 100

                        },
                        autoUpload: false,
                        callbacks: {
                            onAllComplete: function (succeeded, failed) {
                                if (succeeded.length > 0) {
                                    document.getElementById("UploadButton").click();
                                }
                            },
                            onError: function (id, name, errorReason, xhrOrXdr) {

                                document.getElementById('<%= uploadErrorMessage.ClientID %>').value = errorReason;
                                document.getElementById("ErrorButton").click();
                            }

                        }
                    });

                    qq(document.getElementById("trigger-upload")).attach("click", function () {
                        uploader.uploadStoredFiles();
                    });

                 

                });
            </script>
    
    <div id="divSrvUploader" runat="server">
      <div id="uploader"></div>
    </div>
    <br class="clear"/>
    <br/>
    <p class="total">
        <asp:Label ID="lblTotal" runat="server"></asp:Label>
    </p>

    <asp:GridView ID="gvModules" runat="server" AllowSorting="True" DataKeyNames="Id" OnSorting="gridView_Sorting" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>

           
            <asp:TemplateField ItemStyle-CssClass="chkboxwidth">
                <ItemTemplate>
                    <asp:LinkButton ID="btnDelete" runat="server" OnClick="btnDelete_OnClick" Text="Delete"/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" Visible="False"/>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Hash" HeaderText="Hash" SortExpression="Hash" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="DateUploaded" HeaderText="Upload Date" SortExpression="DateUploaded"/>
        </Columns>
        <EmptyDataTemplate>
            No Files Found
        </EmptyDataTemplate>
    </asp:GridView>

            <div id="confirmbox" class="confirm-box-outer">
        <div class="confirm-box-inner">
            <h4>
                <asp:Label ID="lblTitle" runat="server" Text="Upload Complete. Would You Like To Replicate To All Servers Now?"></asp:Label>
            </h4>
            <div class="confirm-box-btns">
                <asp:LinkButton ID="ConfirmButton" OnClick="ButtonConfirm_Click" runat="server" Text="Yes" CssClass="confirm_yes"/>
                <asp:LinkButton ID="CancelButton" OnClick="ButtonCancel_Click" runat="server" Text="No" CssClass="confirm_no"/>
            </div>
        </div>
    </div>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>The upload files page is where you add or remove your files to / from the software module. Uploading file copy module files will accept any extension with a maximum number of 100 files per upload.  If you are using multiple Client Communication Servers, you will be prompted to replicate the files to all servers when uploading is complete. Finally, you must stay on the upload files page while files are uploading, navigating to a different page will stop the uploads.</p>
</asp:Content>