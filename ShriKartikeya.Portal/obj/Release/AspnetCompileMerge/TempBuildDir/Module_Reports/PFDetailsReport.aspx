﻿<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Module_Reports/ReportMaster.master" CodeBehind="PFDetailsReport.aspx.cs" Inherits="ShriKartikeya.Portal.PFDetailsReport" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="RightOne" ContentPlaceHolderID="ContentPlaceHolder3" runat="Server">
    <link href="../css/global.css" rel="stylesheet" type="text/css" />
    <link href="../css/Load.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript">
        function AssignExportHTML() {

            document.getElementById("ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder3_hidGridView").value =
        htmlEscape(ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder3_forExport.innerHTML);
        }
        function htmlEscape(str) {
            return String(str)
            .replace(/&/g, '&amp;')
            .replace(/"/g, '&quot;')
           // .replace(/'/g, '&#39;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;');
        }
    </script>

    <script type="text/javascript">

        function Check_Click(objRef) {
            //Get the Row based on checkbox
            var row = objRef.parentNode.parentNode;
            if (objRef.checked) {
                //If checked change color to Aqua
                //row.style.backgroundColor = "aqua";
            }
            else {
                //If not checked change back to original color
                if (row.rowIndex % 2 == 0) {
                    //Alternating Row Color
                    //row.style.backgroundColor = "#C2D69B";
                }
                else {
                    //row.style.backgroundColor = "white";
                }
            }

            //Get the reference of GridView
            var GridView = row.parentNode;

            //Get all input elements in Gridview
            var inputList = GridView.getElementsByTagName("input");

            for (var i = 0; i < inputList.length; i++) {
                //The First element is the Header Checkbox
                var headerCheckBox = inputList[0];

                //Based on all or none checkboxes
                //are checked check/uncheck Header Checkbox
                var checked = true;
                if (inputList[i].type == "checkbox" && inputList[i] != headerCheckBox) {
                    if (!inputList[i].checked) {
                        checked = false;
                        break;
                    }
                }
            }
            headerCheckBox.checked = checked;

        }
    </script>

    <script type="text/javascript">

        function checkAll(objRef) {

            var GridView = objRef.parentNode.parentNode.parentNode;
            var inputList = GridView.getElementsByTagName("input");
            for (var i = 0; i < inputList.length; i++) {
                //Get the Cell To find out ColumnIndex
                var row = inputList[i].parentNode.parentNode;
                if (inputList[i].type == "checkbox" && objRef != inputList[i]) {
                    if (objRef.checked) {
                        //If the header checkbox is checked
                        //check all checkboxes
                        //and highlight all rows
                        // row.style.backgroundColor = "aqua";
                        inputList[i].checked = true;
                    }
                    else {
                        //If the header checkbox is checked
                        //uncheck all checkboxes
                        //and change rowcolor back to original
                        if (row.rowIndex % 2 == 0) {
                            //Alternating Row Color
                            //row.style.backgroundColor = "#C2D69B";
                        }
                        else {
                            //row.style.backgroundColor = "white";
                        }
                        inputList[i].checked = false;
                    }
                }
            }
        }

    </script>
    <!-- CONTENT AREA BEGIN -->
    <div id="content-holder">
        <div class="content-holder">
            <div id="breadcrumb">
                <ul class="crumbs">
                    <%-- <li class="first"><a href="#" style="z-index: 9;"><span></span>Reports</a></li>--%>
                    <li class="first"><a href="ClientReports.aspx" style="z-index: 8;">Client Reports</a></li>
                    <li class="active"><a href="PFDetailsReport.aspx" style="z-index: 7;" class="active_bread">PF Details</a></li>
                </ul>
            </div>
            <!-- DASHBOARD CONTENT BEGIN -->
            <div class="contentarea" id="contentarea">
                <div class="dashboard_center">
                    <div class="sidebox">
                        <div class="boxhead">
                            <h2 style="text-align: center">PF Details
                            </h2>
                        </div>
                        <div class="boxbody" style="padding: 5px 5px 5px 5px;">
                            <div class="boxin">
                                <asp:ScriptManager runat="server" ID="ScriptEmployReports">
                                </asp:ScriptManager>
                                <div class="dashboard_firsthalf" style="width: 100%">

                                    <table width="100%" cellpadding="5" cellspacing="5">

                                        <tr>
                                            <td>PF Branch<span style="color: Red">*</span>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlPFBranch" OnSelectedIndexChanged="ddlPFBranch_SelectedIndexChanged" AutoPostBack="true" runat="server" TabIndex="30"
                                                    class="sdrop">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>

                                        <tr>

                                            <td>Month :
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtmonth" runat="server" Text="" AutoComplete="off" class="sinput"></asp:TextBox>
                                                <cc1:CalendarExtender ID="txtFrom_CalendarExtender" runat="server" Enabled="true"
                                                    TargetControlID="txtmonth" Format="dd/MM/yyyy">
                                                </cc1:CalendarExtender>
                                                <cc1:FilteredTextBoxExtender ID="FTBEDOI" runat="server" Enabled="True" TargetControlID="txtmonth"
                                                    ValidChars="/0123456789">
                                                </cc1:FilteredTextBoxExtender>
                                            </td>
                                            <td>
                                                <asp:Button runat="server" ID="btn_Submit" Text="Submit" class="btn save" OnClick="btn_Submit_Click" />
                                            </td>
                                            <td>
                                                <asp:Button runat="server" ID="btngetdata" Text="Get Data" class="btn save" OnClick="btn_getdata_Click" />
                                            </td>
                                            <td>
                                                <div align="right">
                                                    <asp:LinkButton ID="lbtn_Export" runat="server" OnClick="lbtn_Export_Click" OnClientClick="AssignExportHTML()">Export to Excel</asp:LinkButton>
                                                    &nbsp; &nbsp; &nbsp;<asp:LinkButton ID="lbtn_Export_Text" runat="server" OnClick="lbtn_Export_Text_Click">Export to Text</asp:LinkButton>
                                                    &nbsp; &nbsp; &nbsp;<asp:LinkButton ID="lbtn_Export_PDF" runat="server" OnClick="lbtn_Export_PDF_Click">Export Consolidated</asp:LinkButton>
                                                    &nbsp; &nbsp; &nbsp;<asp:LinkButton ID="lbtn_Export_Excel" runat="server" OnClick="lbtn_Export_Excel_Click">PF Consolidated</asp:LinkButton>

                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>

                                                <asp:RadioButton ID="radiooldpfdetails" runat="server" Text="OLD" Checked="true"
                                                    GroupName="pfdetails" Visible="false" />
                                            </td>
                                        </tr>



                                    </table>
                                </div>

                                <div class="rounded_corners">
                                    <asp:GridView ID="GVListEmployees" runat="server" AutoGenerateColumns="False" Width="90%" Style="margin: 0px auto"
                                        CellSpacing="3" CellPadding="5" ForeColor="#333333" GridLines="None">
                                        <RowStyle BackColor="#EFF3FB" Height="30" />
                                        <Columns>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:CheckBox ID="chkAll" runat="server" onclick="checkAll(this);" />
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkindividual" runat="server" onclick="Check_Click(this)" />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Client Id">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblclientid" runat="server" Text='<%#Eval("clientid") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="clientname" HeaderText="Client Name" />

                                            <asp:BoundField DataField="pf" HeaderText="PF Employee" />
                                            <asp:BoundField DataField="pfempr" HeaderText="PF Employer" />
                                            <asp:BoundField DataField="totalPF" HeaderText="Total PF" />


                                        </Columns>
                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" Height="30" />
                                        <EditRowStyle BackColor="#2461BF" />
                                        <AlternatingRowStyle BackColor="White" />
                                    </asp:GridView>
                                </div>
                                <asp:HiddenField ID="hidGridView" runat="server" />


                                <%-- OnDataBound="GVListOfClients_DataBound"--%>
                                <div class="rounded_corners" runat="server">
                                    <div id="forExport" class="rounded_corners" runat="server" style="overflow: scroll">

                                        <style type="text/css">
                                            .SubTotalRowStyle {
                                                color: black;
                                                font-weight: bold;
                                                background-color: white;
                                            }

                                            .HeaderRowStyle {
                                                font-weight: bold;
                                                background-color: #507CD1;
                                                color: black;
                                            }
                                        </style>
                                        <asp:GridView ID="GVListOfClients" runat="server" AutoGenerateColumns="False" ForeColor="Black" CssClass="table table-striped table-bordered table-condensed table-hover"
                                            Width="100%" OnRowCreated="GVListOfClients_RowCreated" OnDataBound="GVListOfClients_DataBound"
                                            CellPadding="5" CellSpacing="5" OnRowDataBound="GVListOfClients_RowDataBound">

                                            <Columns>

                                                <asp:BoundField HeaderText="PF NUMBER" DataField="EmpEpfNo" NullDisplayText=" " />
                                                <asp:BoundField HeaderText="CLIENT ID" DataField="clientid" />
                                                <asp:BoundField HeaderText="CLIENT NAME" DataField="Clientname" />
                                                <asp:BoundField HeaderText="EMP ID" DataField="EmpId" />
                                                <asp:BoundField HeaderText="MEMBER NAME" DataField="Fullname" />
                                                <asp:BoundField HeaderText="UAN NUMBER" DataField="EmpUANNumber" NullDisplayText=" " />
                                                <asp:BoundField HeaderText="GROSS_WAGES" DataField="PFWAGES" />
                                                <asp:BoundField HeaderText="EPF_WAGES" DataField="PFWAGES" />
                                                <asp:BoundField HeaderText="EPS_WAGES" DataField="EPSWAGESNEW" />
                                                <asp:BoundField HeaderText="EDLI_WAGES" DataField="EDLIWAGESNEW" />
                                                <asp:BoundField HeaderText="EPF_CONTRI_REMITTED" DataField="PF" />
                                                <asp:BoundField HeaderText="EPS_CONTRI_REMITTED" DataField="EPSDuenew" />
                                                <asp:BoundField HeaderText="EPF_EPS_CONTRI_REMITTED" DataField="pfdiffnew" />
                                                <asp:BoundField HeaderText="NCP Days" DataField="NCPDAYS" NullDisplayText=" " />
                                                <asp:BoundField HeaderText="REFUND_OF_ADVANCES" DataField="ADVREF" NullDisplayText=" " />
                                            </Columns>

                                        </asp:GridView>


                                        <asp:GridView ID="GVPF" runat="server" AutoGenerateColumns="False" ForeColor="Black" CssClass="table table-striped table-bordered table-condensed table-hover"
                                            Width="100%" CellPadding="5" CellSpacing="5" Visible="false" OnRowDataBound="GVPF_RowDataBound">

                                            <Columns>

                                                <asp:BoundField HeaderText="UAN NUMBER" DataField="EmpUANNumber" NullDisplayText=" " />
                                                <asp:BoundField HeaderText="MEMBER NAME" DataField="Fullname" />
                                                <asp:BoundField HeaderText="GROSS_WAGES" DataField="gross" />
                                                <asp:BoundField HeaderText="EPF_WAGES" DataField="PFWAGESNEW" />
                                                <asp:BoundField HeaderText="EPS_WAGES" DataField="EPSWAGESNEW" />
                                                <asp:BoundField HeaderText="EDLI_WAGES" DataField="EDLIWAGESNEW" />
                                                <asp:BoundField HeaderText="EPF_CONTRI_REMITTED" DataField="PF" />
                                                <asp:BoundField HeaderText="EPS_CONTRI_REMITTED" DataField="EPSDuenew" />
                                                <asp:BoundField HeaderText="EPF_EPS_CONTRI_REMITTED" DataField="PFDiffnew" />
                                                <asp:BoundField HeaderText="NCP Days" DataField="NCPDAYS" NullDisplayText=" " />
                                                <asp:BoundField HeaderText="REFUND_OF_ADVANCES" DataField="ADVREF" NullDisplayText=" " />



                                            </Columns>

                                        </asp:GridView>




                                        <asp:Label ID="LblResult" runat="server" Text="" Style="color: red"></asp:Label>


                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="clear">
                </div>
            </div>
        </div>
        <!-- DASHBOARD CONTENT END -->
</asp:Content>
