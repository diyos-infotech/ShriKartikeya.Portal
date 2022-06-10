<%@ Page Title="" Language="C#" MasterPageFile="~/Module_Reports/ReportMaster.master" AutoEventWireup="true" CodeBehind="Wagesheetreports.aspx.cs" Inherits="ShriKartikeya.Portal.Module_Reports.Wagesheetreport" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="RightOne" ContentPlaceHolderID="ContentPlaceHolder3" runat="Server">
    <link href="../css/global.css" rel="stylesheet" type="text/css" />
    <link href="../css/Calendar.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />
    <script type="text/javascript" src="//code.jquery.com/jquery-1.10.2.js"></script>
    <script type="text/javascript" src="//code.jquery.com/ui/1.11.2/jquery-ui.js"></script>
    <style type="text/css">
        .custom-combobox {
            position: relative;
            display: inline-block;
        }

        .custom-combobox-toggle {
            position: absolute;
            top: 0;
            bottom: 0;
            margin-left: -1px;
            padding: 0;
        }

        .custom-combobox-input {
            margin: 0;
            padding: 5px 10px;
        }

        .style2 {
            font-size: 10pt;
            font-weight: bold;
            color: #333333;
            background: #cccccc;
            padding: 5px 5px 2px 10px;
            border-bottom: 1px solid #999999;
            height: 26px;
        }
    </style>
    <script type="text/javascript">
        function AssignExportHTML() {
            document.getElementById("ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder3_hidGridView").value =
          htmlEscape(ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder3_forExport.innerHTML);
        }
        function htmlEscape(str) {
            return String(str)
            .replace(/&/g, '&amp;')
            .replace(/"/g, '&quot;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;');
        }
        function onCalendarShown() {
            var cal = $find("calendar1");
            //Setting the default mode to month
            cal._switchMode("months", true);
            //Iterate every month Item and attach click event to it
            if (cal._monthsBody) {
                for (var i = 0; i < cal._monthsBody.rows.length; i++) {
                    var row = cal._monthsBody.rows[i];
                    for (var j = 0; j < row.cells.length; j++) {
                        Sys.UI.DomEvent.addHandler(row.cells[j].firstChild, "click", call);
                    }
                }
            }
        }
        function onCalendarHidden() {
            var cal = $find("calendar1");
            //Iterate every month Item and remove click event from it
            if (cal._monthsBody) {
                for (var i = 0; i < cal._monthsBody.rows.length; i++) {
                    var row = cal._monthsBody.rows[i];
                    for (var j = 0; j < row.cells.length; j++) {
                        Sys.UI.DomEvent.removeHandler(row.cells[j].firstChild, "click", call);
                    }
                }
            }
        }
        function call(eventElement) {
            var target = eventElement.target;
            switch (target.mode) {
                case "month":
                    var cal = $find("calendar1");
                    cal._visibleDate = target.date;
                    cal.set_selectedDate(target.date);
                    cal._switchMonth(target.date);
                    cal._blur.post(true);
                    cal.raiseDateSelectionChanged();
                    break;
            }
        }
        function Check_Click(objRef) {
            //Get the Row based on checkbox
            var row = objRef.parentNode.parentNode;
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
                        inputList[i].checked = true;
                    }
                    else {
                        //If the header checkbox is checked
                        //uncheck all checkboxes
                        inputList[i].checked = false;
                    }
                }
            }
        }
    </script>
    <div id="content-holder">
        <div class="content-holder">
            <div id="breadcrumb">
                <ul class="crumbs">
                    <%-- <li class="first"><a href="#" style="z-index: 9;"><span></span>Reports</a></li>--%>
                    <li class="first"><a href="ClientReports.aspx" style="z-index: 8;">Client Reports</a></li>
                    <li class="active"><a href="WageSheetReports.aspx" style="z-index: 7;" class="active_bread">Pay Sheet Report</a></li>
                </ul>
            </div>
            <!-- DASHBOARD CONTENT BEGIN -->
            <div class="contentarea" id="contentarea">
                <div class="dashboard_center">
                    <div class="sidebox">
                        <div class="boxhead">
                            <h2 style="text-align: center">Pay Sheet
                            </h2>
                        </div>
                        <div class="boxbody" style="padding: 5px 5px 5px 5px;">
                            <div class="boxin">

                                <asp:ScriptManager runat="server" ID="ScriptEmployReports">
                                </asp:ScriptManager>

                                <div class="dashboard_firsthalf" style="width: 100%">

                                    <table width="100%" cellpadding="5" cellspacing="5">
                                        <tr>
                                            <td colspan="7">
                                                <div align="right">
                                                    <asp:LinkButton ID="lbtn_Export" runat="server" OnClick="lbtn_Export_Click" OnClientClick="AssignExportHTML()" Visible="false">Export to Excel</asp:LinkButton>
                                                    &nbsp;
                                                    <asp:LinkButton ID="lbtn_ExportNew" runat="server" OnClick="lbtn_ExportNew_Click" Visible="false">Export to Excel All</asp:LinkButton>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px">
                                                <asp:Label ID="Label1" runat="server" Text="Branch" />
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlBranch" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlBranch_SelectedIndexChanged"
                                                    CssClass="form-control">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td style="width: 100px">
                                                <asp:Label ID="lblmonth" runat="server" Text="Month" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtmonth" runat="server" Text="" CssClass="form-control" Width="180px" AutoComplete="off" class="sinput"></asp:TextBox>
                                                <cc1:CalendarExtender ID="Txt_Month_CalendarExtender" runat="server" BehaviorID="calendar1"
                                                    Enabled="true" Format="MMM-yyyy" TargetControlID="txtmonth" DefaultView="Months" OnClientHidden="onCalendarHidden" OnClientShown="onCalendarShown">
                                                </cc1:CalendarExtender>
                                            </td>

                                            <td style="padding-right: 120px">
                                                <asp:Button runat="server" ID="btn_Submit" Text="Submit" class="btn save" OnClick="btnsearch_Click" />

                                            </td>

                                            <td style="width: 230px">
                                                <asp:Label ID="lbloptions" runat="server" Text="Options" Visible="false" /></td>
                                            <td>
                                                <asp:DropDownList ID="ddloptions" runat="server" AutoPostBack="true" Width="150px" OnSelectedIndexChanged="ddloptions_SelectedIndexChanged" Visible="false">
                                                    <asp:ListItem>Monthly</asp:ListItem>
                                                    <asp:ListItem>From-To</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>


                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblfrom" runat="server" Text="From" Visible="false" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtfrom" runat="server" Text="" AutoComplete="off" class="sinput" Visible="false"></asp:TextBox>
                                                <cc1:CalendarExtender ID="CEFrom" runat="server" BehaviorID="calendar2"
                                                    Enabled="true" Format="MMM-yyyy" TargetControlID="txtfrom" DefaultView="Months" OnClientHidden="onCalendarHidden" OnClientShown="onCalendarShown">
                                                </cc1:CalendarExtender>
                                            </td>

                                            <td style="width: 100px; padding-left: 80px">
                                                <asp:Label ID="lblto" runat="server" Text="To" Visible="false" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtto" runat="server" Text="" AutoComplete="off" class="sinput" Visible="false"></asp:TextBox>
                                                <cc1:CalendarExtender ID="CalendarExtender1" runat="server" BehaviorID="calendar3"
                                                    Enabled="true" Format="MMM-yyyy" TargetControlID="txtto" DefaultView="Months" OnClientHidden="onCalendarHidden" OnClientShown="onCalendarShown">
                                                </cc1:CalendarExtender>
                                            </td>


                                            <td style="padding-right: 40px">
                                                <asp:Button runat="server" ID="btngetdata" Text="Get Data" class="btn save" Visible="false" OnClick="btngetdata_Click" />

                                            </td>

                                            <td style="padding-right: 120px">
                                                <asp:Button ID="btndownloadpdf" runat="server" Text="Download PDF" class="btn save" OnClick="btndownloadpdf_Click" Visible="false" />
                                            </td>

                                        </tr>

                                        <tr>
                                            <td colspan="3" style="width: 30%">
                                                <asp:Label ID="LblResult" runat="server" Text="" Style="color: Red"> </asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </div>

                                <div class="rounded_corners">
                                    <asp:GridView ID="GVClientsData" runat="server" AutoGenerateColumns="False" Width="90%" Style="margin: 0px auto"
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
                                            <asp:BoundField DataField="MonthName" HeaderText="Month Name" />
                                            <asp:TemplateField HeaderText="Month" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblmonth" runat="server" Text='<%#Eval("month") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="ActualAmount" HeaderText="Net Pay" />


                                        </Columns>
                                        <FooterStyle BackColor="#5071" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                        <HeaderStyle BackColor="#5071" Font-Bold="True" ForeColor="White" Height="30" />
                                        <EditRowStyle BackColor="#2461BF" />
                                        <AlternatingRowStyle BackColor="White" />
                                    </asp:GridView>
                                </div>

                                <asp:HiddenField ID="hidGridView" runat="server" />
                                <div id="forExport" class="rounded_corners" style="overflow: scroll" runat="server">
                                    <asp:GridView ID="GVListEmployees" runat="server" AutoGenerateColumns="False" Width="100%" CssClass="table table-striped table-bordered table-condensed table-hover"
                                        CellSpacing="3" CellPadding="5" ForeColor="#333333" GridLines="Both" ShowFooter="true"
                                        OnPageIndexChanging="GVListEmployees_PageIndexChanging"
                                        OnRowDataBound="GVListEmployees_RowDataBound">

                                        <Columns>

                                            <%-- 0--%>
                                            <asp:TemplateField HeaderText="S.No" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblSno" runat="server" Text='<%#Bind("sno") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:Label ID="lblSno" runat="server" Text="<%#Container.DataItemIndex+1 %>"></asp:Label>
                                                </EditItemTemplate>
                                            </asp:TemplateField>

                                            <%-- 1--%>
                                            <asp:TemplateField HeaderText="Client ID" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="80px" Visible="false">
                                                <HeaderStyle Width="15px" />
                                                <ItemTemplate>
                                                    <asp:Label ID="lblclientid" runat="server" Text='<%#Bind("clientid") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- 2--%>
                                            <asp:TemplateField HeaderText="SITE" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="180px" Visible="false">
                                                <HeaderStyle Width="15px" />
                                                <ItemTemplate>
                                                    <asp:Label ID="lblclientname" runat="server" Text='<%#Bind("clientname") %>'> </asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- 3--%>
                                            <asp:TemplateField HeaderText="AADHAAR NO" ItemStyle-Width="120px" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lblAADHAARNO" Text='<%# Bind("AadharNo") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- 4--%>
                                            <asp:TemplateField HeaderText="Old EmpId" ItemStyle-Width="120px" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lblMOB" Text='<%# Bind("MOB") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- 5--%>
                                            <asp:TemplateField HeaderText="Emp Id" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblempid" runat="server" Text='<%#Bind("EmpId") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- 6--%>
                                            <asp:TemplateField HeaderText="BANK" ItemStyle-HorizontalAlign="Left" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblempname" runat="server" Text='<%#Bind("Empbankname") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>

                                            <%-- 7--%>
                                            <asp:TemplateField HeaderText="IFSC CODE" ItemStyle-HorizontalAlign="Left" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblIFSCCODE" runat="server" Text='<%#Bind("Empifsccode") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>

                                            <%-- 8--%>
                                            <asp:TemplateField HeaderText="Bank A/C No" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblbankno" runat="server" Text='<%# Eval("EmpBankAcNo") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- 9--%>
                                            <asp:TemplateField HeaderText="UAN NO" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbluanno" runat="server" Text='<%# Eval("UANNo") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- 10--%>
                                            <asp:TemplateField HeaderText="ESI NO" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblesino" runat="server" Text='<%# Eval("ESINo") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- 11--%>
                                            <asp:TemplateField HeaderText="Name">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblName" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>


                                            <%-- 12--%>
                                            <asp:TemplateField HeaderText="BASIC+VDA" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCdbasic" runat="server" Text='<%#Eval("Cdbasic", "{0:0}") %>'>
                                                    </asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalCdBasic"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 13--%>
                                            <asp:TemplateField HeaderText="HRA" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCdhra" runat="server" Text='<%#Bind("Cdhra","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalCdHRA"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 14--%>
                                            <asp:TemplateField HeaderText="ALL" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblcdSpecialAllowance" runat="server" Text='<%#Bind("CdSpecialAllW","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalcdSpecialAllowance"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 15--%>
                                            <asp:TemplateField HeaderText="Gross Total" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblTempGross" runat="server" Text='<%#Math.Round(Convert.ToDouble(Eval("TempGross")))%>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lbltotalTempGross"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 16--%>
                                            <asp:TemplateField HeaderText="WD" ItemStyle-HorizontalAlign="Left">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblFromContracts" runat="server" Text='<%#Bind("NoofDaysFromContracts") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblnoofContracts"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 17--%>
                                            <asp:TemplateField HeaderText="ABS" ItemStyle-HorizontalAlign="Left">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblFromContracts2to" runat="server" Text='<%#Bind("CountABS") %>'></asp:Label>
                                                </ItemTemplate>
                                                 <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalFromContracts2to"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 18--%>
                                            <asp:TemplateField HeaderText="P.D" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbldutyhrs" runat="server" Text='<%#Bind("NoOfDuties") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotaldutyhrs"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 19--%>
                                            <asp:TemplateField HeaderText="Additional Days" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblOTs" runat="server" Text='<%#Bind("OTs") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalOts"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 20--%>
                                            <asp:TemplateField HeaderText="TD" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblTDdutyhrs" runat="server" Text='<%#Bind("TotalDuties") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalDuties"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 21--%>
                                            <asp:TemplateField HeaderText="Basic+VDA" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblFixedbasic" runat="server" Text='<%#Eval("basic", "{0:0}") %>'>
                                                    </asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalFixedBasic"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 22--%>
                                            <asp:TemplateField HeaderText="HRA" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblFixedhra" runat="server" Text='<%#Bind("hra","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalFixedHRA"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 23--%>
                                            <asp:TemplateField HeaderText="ALL" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblFixedSpecialAllowance1" runat="server" Text='<%#Bind("SpecialAllowance","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalFixedSpecialAllowance"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 24--%>
                                            <asp:TemplateField HeaderText="Gross Total" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblGross" runat="server" Text='<%#Math.Round(Convert.ToDouble(Eval("Gross"))) %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lbltotalGross"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                              <%-- 25--%>
                                            <asp:TemplateField HeaderText="VPF" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblVPF" runat="server" Text='<%#Bind("VPF","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalVPF"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 26--%>
                                            <asp:TemplateField HeaderText="PF" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblPF" runat="server" Text='<%#Bind("PF","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalPF"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 27--%>
                                            <asp:TemplateField HeaderText="ESI" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblESI" runat="server" Text='<%#Bind("ESI","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalESI"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 28--%>
                                            <asp:TemplateField HeaderText="PT" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblProfTax" runat="server" Text='<%#Bind("ProfTax","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalProfTax"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>


                                            <%-- 29--%>
                                            <asp:TemplateField HeaderText="Advance" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblPenalty" runat="server" Text='<%#Bind("Penalty","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <div style="text-align: justify">
                                                        <asp:Label runat="server" ID="lblTotalPenalty"></asp:Label>
                                                    </div>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 30--%>
                                            <asp:TemplateField HeaderText="TDS" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNightRoundDed" runat="server" Text='<%#Bind("Extra","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <div style="text-align: justify">
                                                        <asp:Label runat="server" ID="lblTotalNightRoundDed"></asp:Label>
                                                    </div>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 31--%>
                                            <asp:TemplateField HeaderText="L D" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblUniformDed" runat="server" Text='<%#Bind("UniformDed","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <div style="text-align: justify">
                                                        <asp:Label runat="server" ID="lblTotalUniformDed"></asp:Label>
                                                    </div>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                              <%-- 32--%>
                                            <asp:TemplateField HeaderText="Sal Adv Ded" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblsaladvded" runat="server" Text='<%#Bind("saladvded","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <div style="text-align: justify">
                                                        <asp:Label runat="server" ID="lblTotalsaladvded"></asp:Label>
                                                    </div>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 33--%>
                                            <asp:TemplateField HeaderText="Ded" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDeductions" runat="server" Text='<%#Bind("TotalDeductions","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalDeductions"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 34--%>
                                            <asp:TemplateField HeaderText="Driver Salary" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDriverSalary" runat="server" Text='<%#Bind("DriverSalary","{0:0}") %>'> </asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalDriverSalary"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 35--%>
                                            <asp:TemplateField HeaderText="Net Pay" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCenternetamountPay" runat="server" Text='<%#Bind("ActualAmount","{0:0}") %>'> </asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalNetAmount"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 36--%>

                                            <asp:TemplateField HeaderText="Arrears" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblArrears" runat="server" Text='<%#Bind("Arrears","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalArrears"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 37--%>
                                            <asp:TemplateField HeaderText="OT Amout" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblotamt" runat="server" Text='<%#Bind("OTAmt","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalotamt"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 38--%>
                                            <asp:TemplateField HeaderText="Inc" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblIncentivs" runat="server" Text='<%#Bind("Incentivs","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalIncentivs"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 39--%>
                                            <asp:TemplateField HeaderText="Reimbursement" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblReimbursement" runat="server" Text='<%#Bind("Reimbursement","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalReimbursement"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 40--%>
                                            <asp:TemplateField HeaderText="Payable Amt" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblnetPayableamount" runat="server" Text='<%#Bind("PayableAmt","{0:0}") %>'> </asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalNetPayableAmount"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                        </Columns>
                                        <FooterStyle BackColor="white" Font-Bold="True" ForeColor="black" />
                                        <PagerStyle BackColor="white" ForeColor="black" HorizontalAlign="Center" />
                                        <SelectedRowStyle BackColor="white" Font-Bold="True" ForeColor="#333333" />
                                        <HeaderStyle BackColor="white" Font-Bold="True" ForeColor="black" />
                                        <EditRowStyle BackColor="white" />
                                        <AlternatingRowStyle BackColor="White" />
                                    </asp:GridView>
                                </div>


                            </div>
                        </div>
                    </div>
                </div>
                <div class="clear">
                </div>
            </div>
        </div>
</asp:Content>
