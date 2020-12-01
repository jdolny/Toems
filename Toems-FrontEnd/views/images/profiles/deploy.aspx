<%@ Page Title="" Language="C#" MasterPageFile="~/views/images/profiles/profiles.master" AutoEventWireup="true" CodeBehind="deploy.aspx.cs" Inherits="Toems_FrontEnd.views.images.profiles.deploy" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
      <li>
        <a href="<%= ResolveUrl("~/views/images/profiles/general.aspx") %>?imageId=<%= ImageEntity.Id %>&profileId=<%= ImageProfile.Id %>&sub=profiles"><%= ImageProfile.Name %></a>
    </li>
 <li>Deploy Options</li>
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
            $('#deploy').addClass("nav-current");
           

        });
    </script>
    
<div class="size-4 column">
    Change Computer Name
</div>
 <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkChangeName" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkChangeName">Toggle</label>
        </div>
<br class="clear"/>

<div id="divExpandVol" runat="server">
    <div class="size-4 column">
        Don't Expand Volumes
    </div>
 <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkDownNoExpand" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkDownNoExpand">Toggle</label>
        </div>
    <br class="clear"/>
</div>

<div id="divBoot" runat="server">
    <div class="size-4 column">
        Update BCD
    </div>
 <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkAlignBCD" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkAlignBCD">Toggle</label>
        </div>
    <br class="clear"/>
    
     <div class="size-4 column">
        Randomize GUIDs
    </div>
 <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkRandomize" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkRandomize">Toggle</label>
        </div>
    <br class="clear"/>

    <div class="size-4 column">
        Fix Boot Sector
    </div>
 <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkRunFixBoot" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkRunFixBoot">Toggle</label>
        </div>
    <br class="clear"/>
       <div class="size-4 column">
       Don't Update NVRAM
    </div>
 <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkNvram" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkNvram">Toggle</label>
        </div>
    <br class="clear"/>
</div>
   

<div id="DivPartDdlLin" runat="server" Visible="False">
    <div class="size-4 column">
        Create Partitions Method
    </div>
    <div class="size-5 column">
        <asp:dropdownlist ID="ddlPartitionMethodLin" runat="server" CssClass="ddlist" OnSelectedIndexChanged="ddlPartitionMethod_OnSelectedIndexChanged" AutoPostBack="True">
            <asp:ListItem>Use Original MBR / GPT</asp:ListItem>
            <asp:ListItem>Standard</asp:ListItem>
            <asp:ListItem>Dynamic</asp:ListItem>
            <asp:ListItem>Custom Script</asp:ListItem>
        </asp:dropdownlist>
    </div>
    <br class="clear"/>
</div>

<div id="DivPartDdlWin" runat="server" Visible="False">
    <div class="size-4 column">
        Create Partitions Method
    </div>
    <div class="size-5 column">
        <asp:dropdownlist ID="ddlPartitionMethodWin" runat="server" CssClass="ddlist" OnSelectedIndexChanged="ddlPartitionMethod_OnSelectedIndexChanged" AutoPostBack="True">
            <asp:ListItem>Standard</asp:ListItem>
            <asp:ListItem>Dynamic</asp:ListItem>
            <asp:ListItem>Custom Script</asp:ListItem>
        </asp:dropdownlist>
    </div>
    <br class="clear"/>
</div>


<div id="DivStandardOptions" runat="server" Visible="False">
     <div class="size-4 column">
        Force Standard EFI Partitions
    </div>
 <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkForceEfi" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkForceEfi">Toggle</label>
        </div>
    <br class="clear"/>
     <div class="size-4 column">
        Force Standard Legacy Partitions
    </div>
 <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkForceLegacy" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkForceLegacy">Toggle</label>
        </div>
    <br class="clear"/>
</div>

<div id="ForceDiv" runat="server" Visible="False">
    <div class="size-4 column">
        Force Dynamic Partition For Exact Hdd Match
    </div>
     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkDownForceDynamic" runat="server" ClientIDMode="Static" AutoPostBack="True" OnCheckedChanged="chkForceDynamic_OnCheckedChanged"></asp:CheckBox>
         <label for="chkDownForceDynamic">Toggle</label>
        </div>

    <br class="clear"/>
</div>
<div class="size-4 column">
    Modify The Image Schema
</div>
     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkModifySchema" runat="server" ClientIDMode="Static" AutoPostBack="True" OnCheckedChanged="chkModifySchema_OnCheckedChanged"></asp:CheckBox>
         <label for="chkModifySchema">Toggle</label>
        </div>

<br class="clear"/>

<div id="imageSchema" runat="server" visible="false">
<div class="size-4 column">
    <asp:LinkButton ID="lnkExport" runat="server" OnClick="lnkExport_OnClick" Text="Export Schema" CssClass="submits"/>
</div>
<br class="clear"/>
<asp:GridView ID="gvHDs" runat="server" AutoGenerateColumns="false" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
<Columns>

<asp:TemplateField ShowHeader="False" ItemStyle-CssClass="width_30" HeaderStyle-CssClass="">
    <ItemTemplate>
        <div style="width: 0">
            <asp:LinkButton ID="btnHd" runat="server" CausesValidation="false" CommandName="" Text="+" OnClick="btnHd_Click"></asp:LinkButton>
        </div>
    </ItemTemplate>
</asp:TemplateField>

<asp:TemplateField ItemStyle-CssClass="width_50" HeaderText="Active">
    <ItemTemplate>
        <asp:CheckBox ID="chkHDActive" runat="server" Checked='<%# Bind("Active") %>'/>
    </ItemTemplate>
</asp:TemplateField>


<asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-CssClass="width_100"></asp:BoundField>
<asp:TemplateField ItemStyle-CssClass="width_50" HeaderText="Destination">
    <ItemTemplate>
        <asp:TextBox ID="txtDestination" runat="server" Text='<%# Bind("Destination") %>' CssClass="textbox"/>
    </ItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="Size" HeaderText="Size (Reported / Usable)" ItemStyle-CssClass="width_200"></asp:BoundField>
<asp:BoundField DataField="Table" HeaderText="Table" ItemStyle-CssClass="width_100"></asp:BoundField>
<asp:BoundField DataField="Boot" HeaderText="Boot Flag" ItemStyle-CssClass="width_100"></asp:BoundField>
<asp:BoundField DataField="Lbs" HeaderText="LBS" ItemStyle-CssClass="width_100"></asp:BoundField>
<asp:BoundField DataField="Pbs" HeaderText="PBS" ItemStyle-CssClass="width_100"></asp:BoundField>
<asp:BoundField DataField="Guid" HeaderText="GUID" ItemStyle-CssClass="width_100"></asp:BoundField>

<asp:TemplateField>
    <ItemTemplate>
        <tr>
            <td id="tdParts" runat="server" visible="false" colspan="900">
                <asp:GridView ID="gvParts" AutoGenerateColumns="false" runat="server" CssClass="Gridview gv_parts" ShowHeader="true" Visible="false" AlternatingRowStyle-CssClass="alt">
                    <Columns>


                        <asp:TemplateField ItemStyle-CssClass="width_50" HeaderText="Active">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkPartActive" runat="server" Checked='<%# Bind("Active") %>'/>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="Number" HeaderText="#" ItemStyle-CssClass="width_100"></asp:BoundField>

                        <asp:BoundField DataField="Size" HeaderText="Size" ItemStyle-CssClass="width_100"></asp:BoundField>
                        <asp:BoundField DataField="VolumeSize" HeaderText="Volume" ItemStyle-CssClass="width_100"></asp:BoundField>
                        <asp:BoundField DataField="Type" HeaderText="Type" ItemStyle-CssClass="width_100"></asp:BoundField>
                        <asp:BoundField DataField="FsType" HeaderText="FS" ItemStyle-CssClass="width_100"></asp:BoundField>
                        <asp:BoundField DataField="FsId" HeaderText="FSID" ItemStyle-CssClass="width_105"></asp:BoundField>
                        <asp:BoundField DataField="UsedMb" HeaderText="Used" ItemStyle-CssClass="width_100"></asp:BoundField>
                        <asp:TemplateField ItemStyle-CssClass="width_100" HeaderText="Custom Size">
                            <ItemTemplate>
                                <div id="settings">
                                    <asp:TextBox ID="txtCustomSize" runat="server" Text='<%# Bind("CustomSize") %>' CssClass="textbox"/>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-CssClass="width_150" HeaderText="Unit">
                            <ItemTemplate>

                                <asp:DropDownList ID="ddlUnit" runat="server" CssClass="ddlist" Text='<%# Bind("CustomSizeUnit") %>'>
                                    <asp:ListItem></asp:ListItem>
                                    <asp:ListItem>MB</asp:ListItem>
                                    <asp:ListItem>GB</asp:ListItem>
                                    <asp:ListItem>%</asp:ListItem>

                                </asp:DropDownList>

                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-CssClass="width_150" HeaderText="Fixed">
                            <ItemTemplate>

                                <asp:CheckBox runat="server" id="chkFixed" Checked='<%# Bind("ForceFixedSize") %>'/>

                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>

                                <tr>

                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>


                                    <td>
                                        <asp:Label ID="Label1" runat="server" Text="UUID" Font-Bold="true"/>
                                        <asp:Label ID="lblUUID" runat="server" Text='<%# Bind("uuid") %>'/>

                                    </td>
                                    <td>
                                        <asp:Label ID="Label2" runat="server" Text="GUID" Font-Bold="true"/>
                                        <asp:Label ID="lblGUID" runat="server" Text='<%# Bind("guid") %>'/>
                                    </td>
                                </tr>
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

                                                                        <asp:TemplateField ItemStyle-CssClass="width_50" HeaderText="Active">
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

                                                                        <asp:TemplateField ItemStyle-CssClass="width_100" HeaderText="Custom Size (MB)">
                                                                            <ItemTemplate>
                                                                                <div id="subsettings">
                                                                                    <asp:TextBox ID="txtCustomSize" runat="server" Text='<%# Bind("CustomSize") %>' CssClass="textbox_specs"/>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField ItemStyle-CssClass="width_150" HeaderText="Unit">
                                                                            <ItemTemplate>

                                                                                <asp:DropDownList ID="ddlUnit" runat="server" CssClass="ddlist" Text='<%# Bind("CustomSizeUnit") %>'>
                                                                                    <asp:ListItem></asp:ListItem>
                                                                                    <asp:ListItem>MB</asp:ListItem>
                                                                                    <asp:ListItem>GB</asp:ListItem>
                                                                                    <asp:ListItem>%</asp:ListItem>

                                                                                </asp:DropDownList>

                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField ItemStyle-CssClass="width_150" HeaderText="Fixed">
                                                                            <ItemTemplate>

                                                                                <asp:CheckBox runat="server" id="chkFixed" Checked='<%# Bind("ForceFixedSize") %>'/>

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


<div id="customScript" runat="server">
    <br/><br/>
    <h3 class="txt-left">Custom Partition Script</h3>
    <div class="full column">
        <pre id="editor" class="editor height_400"></pre>
        <asp:HiddenField ID="scriptEditorText" runat="server"/>

        <script>

            var editor = ace.edit("editor");
            editor.session.setValue($('#<%= scriptEditorText.ClientID %>').val());

            editor.setTheme("ace/theme/idle_fingers");
            editor.getSession().setMode("ace/mode/sh");
            editor.setOption("showPrintMargin", false);
            editor.session.setUseWrapMode(true);
            editor.session.setWrapLimitRange(60, 60);


            function update_click() {
                var editor = ace.edit("editor");
                $('#<%= scriptEditorText.ClientID %>').val(editor.session.getValue());
            }

        </script>
        <br class="clear"/>
    </div>
</div>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <h5><span style="color: #ff9900;">Change Computer Name:</span></h5>
    <p>When this option is checked the computer’s name will be updated match what you have stored in CloneDeploy either by modifying the sysprep file or the registry during the imaging process.</p>

     <h5><span style="color: #ff9900;">Don't Expand Volumes:</span></h5>
    <p>During the deployment process ntfs, extfs, and xfs filesystems are expanded to fill the full partition. Setting this option will disable that. I honestly can’t think of reason to do this, but it may be helpful for debugging.</p>

       <h5><span style="color: #ff9900;">Update BCD:</span></h5>
    <p>If your computer does not boot after deployment, try turning this on. CloneDeploy will update the BCD boot file on Windows machines to correct the boot problem.</p>

       <h5><span style="color: #ff9900;">Randomize GUIDs:</span></h5>
    <p>Generates new partition Guids on GPT tables to avoid collisions.</p>

       <h5><span style="color: #ff9900;">Fix Boot Sector:</span></h5>
    <p>This fixes the partition that is set to active / boot in cases where the NTFS geometry is incorrect. This should almost be left checked. Only applies to mbr partition tables.</p>

       <h5><span style="color: #ff9900;">Don't Update NVRAM:</span></h5>
    <p>When deploying an EFI image. The NVRAM is updated in order to make the machine bootable. In some cases this may cause problems and can be disabled here. Many new systems will automatically detect the correct partition to boot even without correct NVRAM values.</p>

       <h5><span style="color: #ff9900;">Create Partitons Method:</span></h5>
    <p>This option selects how the partition tables will be setup on the client machine before image deployment. The default option is Dynamic and should work for most situations. 
        This means that Theopenem will generate the appropriate sized partitions based on many different factors. It could possibly shrink or grow a partition to make them fit the new hard drive.<br /></p>
        <h5>Use Original MBR / GPT</h5>
    <p>Restores the exact same partition table that was used on the original image. This option should only be used if you are having problems with the dynamic option. 
        You can also only use this if the new hard drive is the same size or larger than the original. If you create an image from 80GB hard drive and you deploy to a 120GB hard drive, 
        the 120GB hard drive will effectively become an 80GB hard drive. You would manually need to resize the partitions.</p>
    <h5>Dynamic</h5>
    <p>Generates / resizes the appropriate partitions to fit on the destination drive based on many different factors.</p>
    <h5>Standard</h5>
    <p>A simple standard partition table is created, as if a new installation with default partitioning and then only the OS volume image is restored. Using this option enables you to deploy an EFI image to a legacy bios machine, and vice versa. When using this option 
        you must select on a single partition to deploy in the modify image schema.</p>
    <h5>Custom Script</h5>
    <p>Allows you to make your own partitions via a shell script. You can use the partitioning tools available in the client boot image. These include fdisk, gdisk, and parted.</p>
       <h5><span style="color: #ff9900;">Force Dynamic Partition For Exact Hdd Match:</span></h5>
    <p>When deploying an image to a hard drive that is the exact same size as the source. They dynamic partition method is not used even if selected. The Use Original MBR / GPT option is used. This option will disable that and force the Dynamic partition option to 
        be used, for cases where the mbr / gpt does not restore properly.</p>

       <h5><span style="color: #ff9900;">Modify The Image Schema:</span></h5>
    <p>Checking this box gives you control over what hard drives and partitions will be deployed, where they will restore to and give you custom sizing options. A new table will be 
        displayed with these options. Any hard drive or partition that is checked will be deployed. Hard drives are always deployed in the order they are listed in this table. You can 
        modify this by setting a value in the Destination box. For example, if the first hard drive listed in the table was originally /dev/sda it will be restored to the first hard drive that is 
        found during the imaging process, it may also be /dev/sda or may not be. If you wanted to send this hard drive to the second hard drive in the system, you would set the Destination
        to /dev/sdb or whatever the matching hard drive name may be. Next, each partition can be set with a Custom Size. Enter a size you want the partition to be and select a unit for the size. 
        Current options are MB, GB, and %. If you are using a percentage they do not need to add up to 100. Whatever percentage is remaining will automatically be spread across the remaining partitions. 
        The same is true when using MB or GB. Finally each partition has check box called Fixed. When this box is checked the partition will not be adjusted to fit the new hard drive size. It will 
        remain the exact same as the original partition that you uploaded. Partitions smaller than 5GB use this logic automatically. When you change the deploy schema, the original schema is never modified, 
        a new copy is created and stored in the database so don’t worry about messing anything up. There is also an option to export the schema you have defined, I may ask for this for debugging info 
        if you are having problems. One final note, if you set a custom size to use a percentage, the minimum client size that is displayed in the profile list will show N/A. This is because a minimum 
        size cannot be known without knowing the size of the hard you are deploying to.</p>
</asp:Content>
