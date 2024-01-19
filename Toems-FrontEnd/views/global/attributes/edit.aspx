﻿<%@ Page Title="" Language="C#" MasterPageFile="~/views/global/attributes/attributes.master" AutoEventWireup="true" CodeBehind="edit.aspx.cs" Inherits="Toems_FrontEnd.views.global.attributes.edit" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li><% =CustomAttribute.Name %></li>
    <li>General</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
    <% =CustomAttribute.Name %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_OnClick" Text="Update Attribute" CssClass="main-action"/></li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#general').addClass("nav-current");
        });
        </script>
        <div class="size-4 column">
            Name:
        </div>
            <div class="size-5 column">
            <asp:TextBox ID="txtName" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox >
            </div >
            <br class="clear" />
            <div class="size-4 column">
            Description:
        </div>
            <div class="size-5 column">
            <asp:TextBox ID="txtDescription" runat="server" CssClass="descbox" TextMode="MultiLine"></asp:TextBox >
            </div >
    <br class="clear"/>
            <div class="size-4 column">
            Text Mode:
        </div>
            <div class="size-5 column">
            <asp:DropDownList ID="ddlTextMode" runat="server" CssClass="ddlist" ClientIDMode="Static"></asp:DropDownList >
            </div >
    <br class="clear"/>
   
      <div class="size-4 column">
        Available To Imaging Client:
    </div>
     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkImaging" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkImaging">Toggle</label>
        </div>
     <br class="clear"/>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>The new page is used to create a new Custom Attribute.  The general page has the same options as the new page.</p>
<h5><span style="color: #ff9900;">Name:</span></h5>
<p>The name of the custom attribute.</p>
<h5><span style="color: #ff9900;">Description:</span></h5>
<p>An optional helpful description for the custom attribute.</p>
<h5><span style="color: #ff9900;">Text Mode:</span></h5>
<p>This field is used to specify how much data you plan on submitting with this attribute.  The SingleLine option is used for a small amount of data, such as serial number.  The MultiLine option is for something larger, such as a description field.</p>
<h5><span style="color: #ff9900;">Usage Type:</span></h5>
<p>Defines which Asset Types the attribute should be available to.  The list is populated with 3 Built-In types as well as all of your custom asset types.</p>
<ul>
	<li>Built-In Any - The attribute can be used by any asset or computer, a description field might be a good example of an attribute that should always be available.</li>
	<li>Built-In Computers - The attribute can only be used for computers.</li>
	<li>Built-In Software - The attribute can only be used for software assets.</li>
</ul>
</asp:Content>
