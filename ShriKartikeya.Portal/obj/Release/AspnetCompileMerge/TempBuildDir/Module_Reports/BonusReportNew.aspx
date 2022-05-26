<%@ Page Title="" Language="C#" MasterPageFile="~/Module_Reports/ReportMaster.master" AutoEventWireup="true" CodeBehind="BonusReportNew.aspx.cs" Inherits="ShriKartikeya.Portal.Module_Reports.BonusReportNew" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="RightOne" ContentPlaceHolderID="ContentPlaceHolder3" runat="server">

    <link href="../css/global.css" rel="stylesheet" type="text/css" />
    <script language="javascript" src="../scripts\Calendar.js" type="text/javascript"></script>
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />
    <script type="text/javascript" src="//code.jquery.com/jquery-1.10.2.js"></script>
    <script type="text/javascript" src="//code.jquery.com/ui/1.11.2/jquery-ui.js"></script>
    <%--<style type="text/css">
        .style2 {
            font-size: 10pt;
            font-weight: bold;
            color: #333333;
            background: #cccccc;
            padding: 5px 5px 2px 10px;
            border-bottom: 1px solid #999999;
            height: 26px;
        }
    </style>--%>
    <script type="text/javascript">

        function pageLoad(sender, args) {
            if (!args.get_isPartialLoad()) {
                //  add our handler to the document's
                //  keydown event
                $addHandler(document, "keydown", onKeyDown);
            }
        }

         function dtval(d, e) {
            var pK = e ? e.which : window.event.keyCode;
            if (pK == 8) { d.value = substr(0, d.value.length - 1); return; }
            var dt = d.value;
            var da = dt.split('/');
            for (var a = 0; a < da.length; a++) { if (da[a] != +da[a]) da[a] = da[a].substr(0, da[a].length - 1); }
            if (da[0] > 31) { da[1] = da[0].substr(da[0].length - 1, 1); da[0] = '0' + da[0].substr(0, da[0].length - 1); }
            if (da[1] > 12) { da[2] = da[1].substr(da[1].length - 1, 1); da[1] = '0' + da[1].substr(0, da[1].length - 1); }
            if (da[2] > 9999) da[1] = da[2].substr(0, da[2].length - 1);
            dt = da.join('/');
            if (dt.length == 2 || dt.length == 5) dt += '/';
            d.value = dt;
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


        function onCalendarShown1() {
            var cal = $find("calendar2");
            //Setting the default mode to month
            cal._switchMode("months", true);
            //Iterate every month Item and attach click event to it
            if (cal._monthsBody) {
                for (var i = 0; i < cal._monthsBody.rows.length; i++) {
                    var row = cal._monthsBody.rows[i];
                    for (var j = 0; j < row.cells.length; j++) {
                        Sys.UI.DomEvent.addHandler(row.cells[j].firstChild, "click", call1);
                    }
                }
            }
        }
        function onCalendarHidden1() {
            var cal = $find("calendar2");
            //Iterate every month Item and remove click event from it
            if (cal._monthsBody) {
                for (var i = 0; i < cal._monthsBody.rows.length; i++) {
                    var row = cal._monthsBody.rows[i];
                    for (var j = 0; j < row.cells.length; j++) {
                        Sys.UI.DomEvent.removeHandler(row.cells[j].firstChild, "click", call1);
                    }
                }
            }
        }
        function call1(eventElement) {
            var target = eventElement.target;
            switch (target.mode) {
                case "month":
                    var cal = $find("calendar2");
                    cal._visibleDate = target.date;
                    cal.set_selectedDate(target.date);
                    cal._switchMonth(target.date);
                    cal._blur.post(true);
                    cal.raiseDateSelectionChanged();
                    break;
            }
        }


    </script>
    <link href="../css/Calendar.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #social div {
            display: block;
        }

        .HeaderStyle {
            text-align: Left;
        }


        .modalBackground {
            background-color: Gray;
            z-index: 10000;
        }

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

        .PnlBackground {
            background-color: rgba(128, 128, 128,0.5);
            z-index: 10000;
        }
    </style>
    <!-- CONTENT AREA BEGIN -->
    <div id="content-holder">
        <div class="content-holder">
            <div id="breadcrumb">
                <ul class="crumbs">
                    <li class="first"><a href="#" style="z-index: 9;"><span></span>Reports</a></li>
                    <li><a href="ClientReports.aspx" style="z-index: 8;">Client Reports</a></li>
                    <li class="active"><a href="#" style="z-index: 7;" class="active_bread">Quarterly & Yearly Report</a></li>
                </ul>
            </div>
            <!-- DASHBOARD CONTENT BEGIN -->
            <div class="contentarea" id="contentarea">
                <asp:ScriptManager runat="server" ID="Scriptmanager1"></asp:ScriptManager>
                <div class="dashboard_center">
                    <div class="sidebox">
                        <div class="boxhead">
                            <h2 style="text-align: center">Quarterly & Yearly Report
                            </h2>
                        </div>
                        <div class="boxbody" style="padding: 5px 5px 5px 5px;">
                            <div class="boxin">
                                <div class="dashboard_firsthalf" style="width: 100%">

                                    <div style="text-align: right">
                                        <asp:LinkButton ID="lbtn_Export" OnClick="lbtn_Export_Click" runat="server">Export to Excel</asp:LinkButton>
                                        <asp:Button runat="server" ID="btn_Submit" Text="Submit" class="btn save" TabIndex="4" OnClick="btn_Submit_Click" />
                                    </div>
                                    <table width="100%" cellpadding="5" cellspacing="5">
                                        <tr>
                                            <td>Component</td>
                                            <td>
                                                <asp:DropDownList ID="ddlComponent" runat="server" class="sdrop" TabIndex="4">
                                                    <asp:ListItem>--Select--</asp:ListItem>
                                                    <asp:ListItem>LA</asp:ListItem>
                                                    <asp:ListItem>Gratuity</asp:ListItem>
                                                    <asp:ListItem>Bonus</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td>Type</td>
                                            <td>
                                                <asp:DropDownList ID="ddlType" runat="server" class="sdrop" TabIndex="4">
                                                    <asp:ListItem Text="--Select--"></asp:ListItem>
                                                    <asp:ListItem Value="0" Text="Monthly"> </asp:ListItem>
                                                    <asp:ListItem Value="1" Text="Quarterly"></asp:ListItem>
                                                    <asp:ListItem Value="2" Text="Half Yearly"></asp:ListItem>
                                                    <asp:ListItem Value="3" Text="Yearly"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>

                                        </tr>

                                        <tr>
                                            <td>From Month</td>
                                            <td>
                                                <asp:TextBox ID="txtfmonth" runat="server" class="sinput" AutoComplete="off"></asp:TextBox>
                                                <cc1:CalendarExtender ID="Txt_Month_CalendarExtender" runat="server" BehaviorID="calendar1"
                                                    Enabled="true" Format="MMM-yyyy" TargetControlID="txtfmonth" DefaultView="Months" OnClientHidden="onCalendarHidden" OnClientShown="onCalendarShown">
                                                </cc1:CalendarExtender>
                                            </td>

                                            <td>TO Month</td>
                                            <td>
                                                <asp:TextBox ID="txtTmonth" runat="server" class="sinput" AutoComplete="off"></asp:TextBox>
                                                <cc1:CalendarExtender ID="CalendarExtender1" runat="server" BehaviorID="calendar2"
                                                    Enabled="true" Format="MMM-yyyy" TargetControlID="txtTmonth" DefaultView="Months" OnClientHidden="onCalendarHidden1" OnClientShown="onCalendarShown1">
                                                </cc1:CalendarExtender>
                                            </td>
                                        </tr>

                                    </table>
                                </div>

                                <br />
                                <div id="forExport" class="rounded_corners" style="overflow: scroll">
                                    <asp:GridView ID="GVListEmployees" runat="server" AutoGenerateColumns="true" Width="100%" CssClass="table table-striped table-bordered table-condensed table-hover"
                                        EmptyDataText="" CellPadding="4" ForeColor="#333333" CellSpacing="3" ShowFooter="True"
                                        OnRowDataBound="GVListEmployees_RowDataBound">
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
        <!-- DASHBOARD CONTENT END -->

        <!-- CONTENT AREA END -->
    </div>

</asp:Content>
