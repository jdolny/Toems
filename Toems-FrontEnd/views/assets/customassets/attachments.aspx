<%@ Page Title="" Language="C#" MasterPageFile="~/views/assets/customassets/customassets.master" AutoEventWireup="true" CodeBehind="attachments.aspx.cs" Inherits="Toems_FrontEnd.views.assets.customassets.attachments" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li> <%=Asset.DisplayName %></li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
    <%=Asset.DisplayName %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
   <li><asp:LinkButton runat="server" ID="btnDelete" Text="Delete Selected Files" OnClick="btnDelete_OnClick" CssClass="main-action"></asp:LinkButton></li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#attachments').addClass("nav-current");
        });
    </script>
    <asp:Button runat="server" ID="UploadButton" Text="" style="display:none;" OnClick="UploadButton_OnClick" ClientIDMode="Static" />
    <asp:Button runat="server" ID="ErrorButton" Text="" style="display:none;" OnClick="ErrorButton_OnClick" ClientIDMode="Static" />
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
                    var attachmentGuid = "<%= attachmentGuid %>";
                    var assetId = "<%= assetId %>";

                    var uploader = new qq.FineUploader({
                        debug: true,
                        element: document.getElementById('uploader'),
                        request: {
                            endpoint: myBaseUrl + 'Upload/UploadAttachment',
                            customHeaders: {
                                "Authorization": "Bearer " + myToken
                            },
                            params: {
                                uploadMethod: "standard",
                                attachmentGuid: attachmentGuid,
                                assetId: assetId
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
                            success: {
                                endpoint: myBaseUrl + 'Upload/AttachmentChunkingComplete'
                            }
                        },
                        thumbnails: {
                            placeholders: {
                                waitingPath: "<%= ResolveUrl("~/content/js/fineuploader/placeholders/waiting-generic.png") %>",
                                notAvailablePath: "<%= ResolveUrl("~/content/js/fineuploader/placeholders/file-3.png") %>"
                            }
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
    
      <div id="uploader"></div>
    
    <br class="clear"/>
    <br/>
    <asp:GridView ID="gvAttachments" runat="server"  DataKeyNames="Id"  AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:TemplateField>
                <HeaderStyle CssClass="chkboxwidth"></HeaderStyle>
                <ItemStyle CssClass="chkboxwidth"></ItemStyle>
                <HeaderTemplate>
                    <asp:CheckBox ID="chkSelectAll" runat="server" AutoPostBack="True" OnCheckedChanged="chkSelectAll_CheckedChanged"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkSelector" runat="server"/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle CssClass="width_100"></ItemStyle>
                <ItemTemplate>
                    <asp:LinkButton runat="server" ID="btnDownload" OnClick="btnDownload_OnClick" Text="Download"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" Visible="False"/>
            <asp:BoundField DataField="Name" HeaderText="File Name" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="AttachmentTime" HeaderText="Time" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="DirectoryGuid" HeaderText="Directory Guid" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="UserName" HeaderText="User"></asp:BoundField>

        </Columns>
        <EmptyDataTemplate>
            No Attachments Found
        </EmptyDataTemplate>
    </asp:GridView>
  
    <div id="confirmbox" class="confirm-box-outer">
        <div class="confirm-box-inner">
            <h4>
                <asp:Label ID="lblTitle" runat="server"></asp:Label>
            </h4>
            <div class="confirm-box-btns">
                <asp:LinkButton ID="ConfirmButton" OnClick="ConfirmButton_OnClick" runat="server" Text="Yes" CssClass="confirm_yes"/>
                <asp:LinkButton ID="CancelButton" runat="server" Text="No" CssClass="confirm_no"/>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>Allows you to add any files to the Asset, for later viewing.  An example usage could be a purchase order.</p>
</asp:Content>
