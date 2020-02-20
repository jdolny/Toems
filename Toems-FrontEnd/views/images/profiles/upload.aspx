<%@ Page Title="" Language="C#" MasterPageFile="~/views/images/profiles/profiles.master" AutoEventWireup="true" CodeBehind="upload.aspx.cs" Inherits="Toems_FrontEnd.views.images.profiles.upload" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
      <li>
        <a href="<%= ResolveUrl("~/views/images/profiles/general.aspx") %>?imageId=<%= ImageEntity.Id %>&profileId=<%= ImageProfile.Id %>&sub=profiles"><%= ImageProfile.Name %></a>
    </li>
 <li>Upload Options</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">

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
    <div class="size-5 column">
        <asp:CheckBox ID="chkRemoveGpt" runat="server" CssClass="textbox"></asp:CheckBox>
    </div>
    <br class="clear"/>
</div>

<div id="divShrink" runat="server">
    <div class="size-4 column">
        Don't Shrink Volumes
    </div>
    <div class="size-5 column">
        <asp:CheckBox ID="chkUpNoShrink" runat="server" CssClass="textbox"></asp:CheckBox>
    </div>
    <br class="clear"/>

    <div class="size-4 column">
        Don't Shrink LVM Volumes
    </div>
    <div class="size-5 column">
        <asp:CheckBox ID="chkUpNoShrinkLVM" runat="server" CssClass="textbox"></asp:CheckBox>
    </div>
    <br class="clear"/>

      <div class="size-4 column">
        Skip Hibernation Check
    </div>
    <div class="size-5 column">
        <asp:CheckBox ID="chkSkipHibernation" runat="server" CssClass="textbox"></asp:CheckBox>
    </div>
    <br class="clear"/>

      <div class="size-4 column">
        Skip Bitlocker Check
    </div>
    <div class="size-5 column">
        <asp:CheckBox ID="chkSkipBitlocker" runat="server" CssClass="textbox"></asp:CheckBox>
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
<div class="size-5 column">
    <asp:CheckBox ID="chkSimpleSchema" runat="server" CssClass="textbox"></asp:CheckBox>
</div>

<br class="clear"/>
    </div>
<div class="size-4 column">
    Only Upload Schema
</div>
<div class="size-5 column">
    <asp:CheckBox ID="chkSchemaOnly" runat="server" CssClass="textbox"></asp:CheckBox>
</div>

<br class="clear"/>
<div class="size-4 column">
    Use Custom Upload Schema
</div>
<div class="size-5 column">
    <asp:CheckBox ID="chkCustomUpload" runat="server" CssClass="textbox" AutoPostBack="True" OnCheckedChanged="chkCustomUpload_OnCheckedChanged"></asp:CheckBox>
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
    <h5><span style="color: #ff9900;">Display Name:</span></h5>
<p>A name used to identify the com server, it has not effect on the functionality.</p>
<h5><span style="color: #ff9900;">URL:</span></h5>
<p>The url used to access the com server, this must match the url that is set in IIS.</p>
<h5><span style="color: #ff9900;">Description:</span></h5>
<p>An optional description for the com server.</p>
<h5><span style="color: #ff9900;">Replicate Storage:</span></h5>
<p>This option is only available after the com server is added.  When multiple com servers are defined, files for your modules must be replicated across all com servers.  If you want to disable replication to a com server, then disable this option.  If this option is disabled, you must manually replicate the files.</p>
</asp:Content>
