﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Module_Reports/ReportMaster.master" AutoEventWireup="true" CodeBehind="PTReport.aspx.cs" Inherits="ShriKartikeya.Portal.Module_Reports.PTReport" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="RightOne" ContentPlaceHolderID="ContentPlaceHolder3" runat="Server">
    <link href="css/global.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />
    <script type="text/javascript" src="//code.jquery.com/jquery-1.10.2.js"></script>
    <script type="text/javascript" src="//code.jquery.com/ui/1.11.2/jquery-ui.js"></script>


    <script type="text/javascript">
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

    </script>

    <style type="text/css">
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
    <!-- CONTENT AREA BEGIN -->
    <div id="content-holder">
        <div class="content-holder">
            <div id="breadcrumb">
                <ul class="crumbs">
                    <%-- <li class="first"><a href="#" style="z-index: 9;"><span></span>Reports</a></li>--%>
                    <li class="first"><a href="Reports.aspx" style="z-index: 8;">Employee Reports</a></li>
                    <li class="active"><a href="FullandFinal.aspx" style="z-index: 7;" class="active_bread">PT Report</a></li>
                </ul>
            </div>
            <!-- DASHBOARD CONTENT BEGIN -->
            <div class="contentarea" id="contentarea">
                <div class="dashboard_center">
                    <div class="sidebox">
                        <div class="boxhead">
                            <h2 style="text-align: center">PT Report
                            </h2>
                        </div>
                        <div class="boxbody" style="padding: 5px 5px 5px 5px;">
                            <div class="boxin">
                                <asp:ScriptManager runat="server" ID="ScriptEmployReports">
                                </asp:ScriptManager>
                                <div style="width: 100%">
                                    <table style="width:100%" cellpadding="7" cellspacing="7">
                                        <tr>
                                            <td>Month
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtmonth" runat="server" Text="" class="sinput" AutoComplete="off"></asp:TextBox>
                                                <cc1:CalendarExtender ID="Txt_Month_CalendarExtender" runat="server" BehaviorID="calendar1"
                                                    Enabled="true" Format="MMM-yyyy" TargetControlID="txtmonth" DefaultView="Months" OnClientHidden="onCalendarHidden" OnClientShown="onCalendarShown">
                                                </cc1:CalendarExtender>

                                            </td>
                                            <td style="padding-left: 50px;padding-left: -100px;width: 100px;">PT State
                                            </td>
                                            <td style="padding-left: 100px;">
                                                <asp:DropDownList runat="server" ID="ddlPTState" TabIndex="19" class="sdrop">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:Button runat="server" ID="btn_Submit" Text="Submit" class="btn save" OnClick="btnsearch_Click" />
                                            </td>
                                            <td>
                                                <div align="right">
                                                    <asp:LinkButton ID="lbtn_Export" runat="server" OnClick="lbtn_Export_Click" Visible="False">Export to Excel</asp:LinkButton>&nbsp;&nbsp;&nbsp;
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3" style="width: 30%">
                                                <asp:Label ID="LblResult" runat="server" Text="" Style="color: Red"> </asp:Label>
                                            </td>
                                        </tr>
                                    </table>


                                    <div class="rounded_corners" style="width: 950px">
                                        <asp:GridView ID="GVListEmployees" runat="server" AutoGenerateColumns="False" Width="100%" CssClass="table table-striped table-bordered table-condensed table-hover"
                                            EmptyDataText="" CellPadding="4" ForeColor="#333333" CellSpacing="3" ShowFooter="True"
                                            OnRowDataBound="GVListEmployees_RowDataBound">
                                            <Columns>
                                               <asp:TemplateField HeaderText="SNo" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                             <asp:Label ID="lblSno" runat="server" Text="<%#Container.DataItemIndex+1 %>"></asp:Label>
                                        </ItemTemplate>
                                        </asp:TemplateField>

                                         <asp:TemplateField HeaderText="Client Id" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblClientid" runat="server" Text='<%#Bind("Clientid")%>'></asp:Label>
                                        </ItemTemplate>
                                        </asp:TemplateField>

                                         <asp:TemplateField HeaderText="Client Name" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="lblclientname" runat="server" Text='<%#Bind("ClientName")%>'></asp:Label>
                                        </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Emp Id" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblempid" runat="server" Text='<%#Bind("empid")%>'></asp:Label>
                                        </ItemTemplate>
                                        </asp:TemplateField>


                                        <asp:TemplateField HeaderText="Emp Name" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="lblempname" runat="server" Text='<%#Bind("Name")%>'></asp:Label>
                                        </ItemTemplate>
                                        </asp:TemplateField>

                                                 <asp:TemplateField HeaderText="Month" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="lblMonthName" runat="server" Text='<%#Bind("MonthName")%>'></asp:Label>
                                        </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="PT Gross" ItemStyle-HorizontalAlign="Right">
                                        <ItemTemplate>
                                            <asp:Label ID="lblgross" runat="server" Text='<%#Bind("PTWages") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label ID="lblTotalgross" runat="server" ></asp:Label>
                                        </FooterTemplate>
                                        </asp:TemplateField>
                                        

                                    <asp:TemplateField HeaderText="PT Deducted" ItemStyle-HorizontalAlign="Right">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPT" runat="server" Text='<%#Bind("ProfTax") %>'></asp:Label>
                                        </ItemTemplate>
                                         <FooterTemplate>
                                            <asp:Label ID="lblTotalPT" runat="server" ></asp:Label>
                                        </FooterTemplate>
                                        </asp:TemplateField>

                                            </Columns>
                                        </asp:GridView>
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
