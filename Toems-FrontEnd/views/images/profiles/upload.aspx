<%@ Page Title="" Language="C#" MasterPageFile="~/views/images/profiles/profiles.master" AutoEventWireup="true" CodeBehind="upload.aspx.cs" Inherits="Toems_FrontEnd.views.images.profiles.upload" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
      <li>
        <a href="<%= ResolveUrl("~/views/images/profiles/general.aspx") %>?imageId=<%= ImageEntity.Id %>&profileId=<%= ImageProfile.Id %>&sub=profiles"><%= ImageProfile.Name %></a>
    </li>
 <li>Upload Options</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
     <%= ImageProfile.Name %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="btnUpdate" runat="server" Text="Update Profile" OnClick="btnUpdate_Click" CssClass="main-action"/></li>
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#upload').addClass("nav-current");
           

        });
    </script>

    <div id="divGpt" runat="server">
    <div class="size-4 column">
        Remove GPT Structures
    </div>
    <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkRemoveGpt" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkRemoveGpt">Toggle</label>
        </div>
</div>

<div id="divShrink" runat="server">
    <div class="size-4 column">
        Don't Shrink Volumes
    </div>
     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkUpNoShrink" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkUpNoShrink">Toggle</label>
        </div>
    <br class="clear"/>

    <div class="size-4 column">
        Don't Shrink LVM Volumes
    </div>
    <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkUpNoShrinkLVM" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkUpNoShrinkLVM">Toggle</label>
        </div>
    <br class="clear"/>

      <div class="size-4 column">
        Skip Hibernation Check
    </div>
     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkSkipHibernation" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkSkipHibernation">Toggle</label>
        </div>
    <br class="clear"/>

      <div class="size-4 column">
        Skip Bitlocker Check
    </div>
    <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkSkipBitlocker" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkSkipBitlocker">Toggle</label>
        </div>
    <br class="clear"/>
</div>

<div id="divCompression" runat="server">
    <div class="size-4 column">
        Compression Algorithm:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlCompAlg" runat="server" CssClass="ddlist">
            <asp:ListItem>gzip</asp:ListItem>
            <asp:ListItem>lz4</asp:ListItem>
            <asp:ListItem>none</asp:ListItem>
        </asp:DropDownList>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Compression Level:
    </div>

    <div class="size-5 column">
        <asp:DropDownList ID="ddlCompLevel" runat="server" CssClass="ddlist">
            <asp:ListItem>1</asp:ListItem>
            <asp:ListItem>2</asp:ListItem>
            <asp:ListItem>3</asp:ListItem>
            <asp:ListItem>4</asp:ListItem>
            <asp:ListItem>5</asp:ListItem>
            <asp:ListItem>6</asp:ListItem>
            <asp:ListItem>7</asp:ListItem>
            <asp:ListItem>8</asp:ListItem>
            <asp:ListItem>9</asp:ListItem>
        </asp:DropDownList>
    </div>
</div>
<br class="clear"/>

<div id="DivSimple" runat="server">
<div class="size-4 column">
    Use Simple Upload Schema
</div>
  <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkSimpleSchema" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkSimpleSchema">Toggle</label>
        </div>

<br class="clear"/>
    </div>
<div class="size-4 column">
    Only Upload Schema
</div>
  <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkSchemaOnly" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkSchemaOnly">Toggle</label>
        </div>

<br class="clear"/>
<div class="size-4 column">
    Use Custom Upload Schema
</div>
  <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkCustomUpload" runat="server" ClientIDMode="Static" AutoPostBack="True" OnCheckedChanged="chkCustomUpload_OnCheckedChanged"></asp:CheckBox>
         <label for="chkCustomUpload">Toggle</label>
      </div>

<br class="clear"/>

<div id="imageSchema" runat="server">
    <asp:GridView ID="gvHDs" runat="server" AutoGenerateColumns="false" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>

            <asp:TemplateField ShowHeader="False" ItemStyle-CssClass="width_30" HeaderStyle-CssClass="">
                <ItemTemplate>
                    <div style="width: 0">
                        <asp:LinkButton ID="btnHd" runat="server" CausesValidation="false" CommandName="" Text="+" OnClick="btnHd_Click"></asp:LinkButton>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField ItemStyle-CssClass="width_50" HeaderText="Upload">
                <ItemTemplate>
                    <asp:CheckBox ID="chkHDActive" runat="server" Checked='<%# Bind("Active") %>'/>
                </ItemTemplate>
            </asp:TemplateField>


            <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-CssClass="width_100"></asp:BoundField>
            <asp:BoundField DataField="Size" HeaderText="Size (Reported / Usable)" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Table" HeaderText="Table" ItemStyle-CssClass="width_100"></asp:BoundField>
            <asp:BoundField DataField="Boot" HeaderText="Boot Flag" ItemStyle-CssClass="width_100"></asp:BoundField>

            <asp:TemplateField>
                <ItemTemplate>
                    <tr>
                        <td id="tdParts" runat="server" visible="false" colspan="900">
                            <asp:GridView ID="gvParts" AutoGenerateColumns="false" runat="server" CssClass="Gridview gv_parts" ShowHeader="true" Visible="false" AlternatingRowStyle-CssClass="alt">
                                <Columns>
                                    <asp:TemplateField ItemStyle-CssClass="width_50" HeaderText="Upload">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkPartActive" runat="server" Checked='<%# Bind("Active") %>'/>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="Number" HeaderText="#" ItemStyle-CssClass="width_100"></asp:BoundField>

                                    <asp:BoundField DataField="Size" HeaderText="Size" ItemStyle-CssClass="width_100"></asp:BoundField>
                                    <asp:BoundField DataField="VolumeSize" HeaderText="Volume" ItemStyle-CssClass="width_100"></asp:BoundField>

                                    <asp:BoundField DataField="FsType" HeaderText="FS" ItemStyle-CssClass="width_100"></asp:BoundField>

                                    <asp:BoundField DataField="UsedMb" HeaderText="Used" ItemStyle-CssClass="width_100"></asp:BoundField>

                                    <asp:TemplateField ItemStyle-CssClass="width_150" HeaderText="Fixed Size">
                                        <ItemTemplate>

                                            <asp:CheckBox runat="server" id="chkFixed" Checked='<%# Bind("ForceFixedSize") %>'/>

                                        </ItemTemplate>
                                    </asp:TemplateField>


                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <tr>
                                                <td id="tdVG" runat="server" visible="false" colspan="900">
                                                    <h4>
                                                        <asp:Label ID="LVM" runat="server" Text="Volume Group" style="margin-left: 30px;"></asp:Label>
                                                    </h4>
                                                    <asp:GridView ID="gvVG" AutoGenerateColumns="false" runat="server" CssClass="Gridview gv_vg" ShowHeader="true" Visible="false" AlternatingRowStyle-CssClass="alt">
                                                        <Columns>
                                                            <asp:TemplateField ShowHeader="False" ItemStyle-CssClass="width_30" HeaderStyle-CssClass="">
                                                                <ItemTemplate>
                                                                    <div style="width: 20px">
                                                                        <asp:LinkButton ID="vgClick" runat="server" CausesValidation="false" CommandName="" Text="+" OnClick="btnVG_Click"></asp:LinkButton>
                                                                    </div>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-CssClass="width_100"/>
                                                            <asp:BoundField DataField="PhysicalVolume" HeaderText="PV" ItemStyle-CssClass="width_200"/>
                                                            <asp:BoundField DataField="Uuid" HeaderText="UUID" ItemStyle-CssClass="width_200"/>

                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <tr>
                                                                        <td id="tdLVS" runat="server" visible="false" colspan="900">
                                                                            <asp:GridView ID="gvLVS" AutoGenerateColumns="false" runat="server" CssClass="Gridview gv_parts" ShowHeader="true" Visible="false" AlternatingRowStyle-CssClass="alt">
                                                                                <Columns>

                                                                                    <asp:TemplateField ItemStyle-CssClass="width_50" HeaderText="Upload">
                                                                                        <ItemTemplate>
                                                                                            <asp:CheckBox ID="chkPartActive" runat="server" Checked='<%# Bind("Active") %>'/>
                                                                                        </ItemTemplate>
                                                                                    </asp:TemplateField>
                                                                                    <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-CssClass="width_100"></asp:BoundField>

                                                                                    <asp:BoundField DataField="Size" HeaderText="Size" ItemStyle-CssClass="width_100"></asp:BoundField>
                                                                                    <asp:BoundField DataField="VolumeSize" HeaderText="Resize" ItemStyle-CssClass="width_100"></asp:BoundField>

                                                                                    <asp:BoundField DataField="FsType" HeaderText="FS" ItemStyle-CssClass="width_100"></asp:BoundField>
                                                                                    <asp:BoundField DataField="Uuid" HeaderText="UUID" ItemStyle-CssClass="width_100"></asp:BoundField>

                                                                                    <asp:BoundField DataField="UsedMb" HeaderText="Used" ItemStyle-CssClass="width_100"></asp:BoundField>


                                                                                </Columns>
                                                                            </asp:GridView>
                                                                        </td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                        </Columns>


                                                    </asp:GridView>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:TemplateField>


                                </Columns>
                            </asp:GridView>


                        </td>


                    </tr>
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
        <EmptyDataTemplate>
            No Image Schema Found
        </EmptyDataTemplate>
    </asp:GridView>
</div>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <h5><span style="color: #ff9900;">Remove GPT Structures:</span></h5>
<p>A hard drive can actually have an mbr and a gpt partition table. Hard drives that have both will not function with Theopenem. 
    If you are certain that you are using mbr, this will clear out the gpt part of the table before the upload.</p>

        <h5><span style="color: #ff9900;">Don't Shrink Volumes:</span></h5>
        <p>By default, when a Block image is uploaded, all ntfs or extfs filesystems that are larger than 5GB are shrunk to the smallest volume size possible to allow restoring to 
            hard drives that are smaller than the current one being captured. If this is causing problems you can disable that here.</p>
        <h5><span style="color: #ff9900;">Don't Shrink LVM Volumes:</span></h5>
        <p>Same as above but specifically for LVM. Don’t Shrink Volumes will supersede this setting, but not vice versa.

</p>
        <h5><span style="color: #ff9900;">Skip Hibernation Check:</span></h5>
        <p>Before an image is uploaded, it checks for the presence of hiberfil.sys and cancels the upload if it exists. Uploading an image while hibernated can completely break the original image. 
            Enabling this option will skip this check.</p>
        <h5><span style="color: #ff9900;">Skip Bitlocker Check:</span></h5>
        <p>Bitlocker is not supported and must be disabled before uploading an image. CloneDeploy will attempt to see if Bitlocker is enabled and error out if so. Enabling this option skips the bitlocker check.</p>
        <h5><span style="color: #ff9900;">Compression Algorithm:</span></h5>
        <p>A few different ways to compress or not compress your image</p>
        <h5><span style="color: #ff9900;">Compression Level:</span></h5>
        <p>Higher number is greater compression</p>
        <h5><span style="color: #ff9900;">Only Upload Schema:</span></h5>
        <p>If you want to control what hard drives or partitions to upload, this is the first step. Turn this setting On and start an upload task. Instead of uploading an entire image, it will only upload the schema.</p>
        <h5><span style="color: #ff9900;">Use Custom Upload Schema:</span></h5>
        <p>If you want to control what hard drives or partitions to upload, this is the second step. Check this box and a new table will be available to visually pick your partitions but only after you 
            have uploaded the schema. The table will list each hard drive and partition that was found, simply check or uncheck the one’s you want. There is also an option for each partition called fixed. 
            If this box is checked the filesystem for that partition will not be shrunk. This is a more flexible option than setting the Don’t shrink volumes setting which applies to all partitions. 
            The Upload schema only box must be unchecked when use custom upload schema is checked. This does not modify the schema that was uploaded, it generates a new one that is stored in the database, 
            the original is never modified.</p>
</asp:Content>
