<%@ Page Title="" Language="C#" MasterPageFile="~/Module_Reports/ReportMaster.master" AutoEventWireup="true" CodeBehind="TDSReport.aspx.cs" Inherits="ShriKartikeya.Portal.TDSReport" %>

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
                    <li class="first"><a href="#" style="z-index: 9;"><span></span>Reports</a></li>
                    <li><a href="Reports.aspx" style="z-index: 8;">Employee Reports</a></li>
                    <li class="active"><a href="#" style="z-index: 7;" class="active_bread">TDS REPORT</a></li>
                </ul>
            </div>
            <!-- DASHBOARD CONTENT BEGIN -->
            <div class="contentarea" id="contentarea">
                <div class="dashboard_center">
                    <div class="sidebox">
                        <div class="boxhead">
                            <h2 style="text-align: center">TDS REPORT
                            </h2>
                        </div>
                        <div class="boxbody" style="padding: 5px 5px 5px 5px;">
                            <div class="boxin">

                                <asp:ScriptManager runat="server" ID="ScriptEmployReports">
                                </asp:ScriptManager>

                                <div class="dashboard_firsthalf" style="width: 100%">

                                    <table width="100%" cellpadding="5" cellspacing="5">
                                        <tr>

                                            <td>Month
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtmonth" runat="server" Text="" AutoComplete="off" class="sinput"></asp:TextBox>
                                                <cc1:CalendarExtender ID="Txt_Month_CalendarExtender" runat="server" BehaviorID="calendar1"
                                                    Enabled="true" Format="MMM-yyyy" TargetControlID="txtmonth" DefaultView="Months" OnClientHidden="onCalendarHidden" OnClientShown="onCalendarShown">
                                                </cc1:CalendarExtender>
                                            </td>
                                            <td>
                                                <asp:Button runat="server" ID="btn_Submit" Text="Submit" class="btn save" OnClick="btn_Submit_Click" />
                                            </td>
                                            <td>
                                                <div align="right">
                                                    <asp:LinkButton ID="lbtn_Export" runat="server" OnClick="lbtn_Export_Click">Export to Excel</asp:LinkButton>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3" style="width: 30%">
                                                <asp:Label ID="LblResult" runat="server" Text="" Style="color: Red"> </asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </div>

                                <div id="forExport" class="rounded_corners" style="overflow: scroll">
                                    <asp:GridView ID="GVListEmployees" runat="server" AutoGenerateColumns="False" Width="100%"
                                        PageSize="15" CellPadding="5" CellSpacing="3" ForeColor="#333333" GridLines="None">

                                        <Columns>

                                            <%-- 0--%>
                                            <asp:TemplateField HeaderText="S.No" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblSno" runat="server" Text="<%#Container.DataItemIndex+1 %>"></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:Label ID="lblSno" runat="server" Text="<%#Container.DataItemIndex+1 %>"></asp:Label>
                                                </EditItemTemplate>
                                            </asp:TemplateField>

                                            <%-- 1--%>
                                            <asp:TemplateField HeaderText="EMP ID" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="80px">
                                                <HeaderStyle Width="15px" />
                                                <ItemTemplate>
                                                    <asp:Label ID="lblclientid" runat="server" Text='<%#Bind("EMPID") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%-- 2--%>

                                            <asp:TemplateField HeaderText="EMP Name" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="180px">
                                                <HeaderStyle Width="15px" />
                                                <ItemTemplate>
                                                    <asp:Label ID="lblclientname" runat="server" Text='<%#Bind("Fullname") %>'> </asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <%-- 3--%>
                                            <asp:TemplateField HeaderText="PAN NO" ItemStyle-Width="120px">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lblAreaName" Text='<%# Bind("PANNO") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <%-- 4--%>
                                            <asp:TemplateField HeaderText="UNIT" ItemStyle-Width="120px">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lblZoneName" Text='<%# Bind("UnitID") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <%-- 5--%>
                                            <asp:TemplateField HeaderText="Basic" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblbasic" runat="server" Text='<%#Eval("basic", "{0:0}") %>'>
                                                    </asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalBasic"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 6--%>
                                            <asp:TemplateField HeaderText="DA" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblda" runat="server" Text='<%#Eval("da","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalDA"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 7--%>
                                            <asp:TemplateField HeaderText="HRA" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblhra" runat="server" Text='<%#Bind("hra","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalHRA"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 8--%>
                                            <asp:TemplateField HeaderText="CCA" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblcca" runat="server" Text='<%#Bind("CCa","{0:0}") %>'>  
                                                    </asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalCCA"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 9--%>
                                            <asp:TemplateField HeaderText="Conv" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblConveyance" runat="server" Text='<%#Bind("conveyance","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalConveyance"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 10--%>
                                            <asp:TemplateField HeaderText="W.A." ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblwashallowance" runat="server" Text='<%#Bind("WashAllowance","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalWA"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 11--%>
                                            <asp:TemplateField HeaderText="O.A." ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblOtherallowance" runat="server" Text='<%#Bind("OtherAllowance","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalOA"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 12--%>
                                            <asp:TemplateField HeaderText="Special AllW" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblSpecialAllowance" runat="server" Text='<%#Bind("SpecialAllowance","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalSpecialAllowance"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 13--%>
                                            <asp:TemplateField HeaderText="Prof AllW" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblUniformAllw" runat="server" Text='<%#Bind("Profallowance","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalUniformAllw"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 14--%>
                                            <asp:TemplateField HeaderText="Mobile Allw" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblMobileAllowance" runat="server" Text='<%#Bind("MobileAllowance","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalMobileAllowance"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 15--%>
                                            <asp:TemplateField HeaderText="Medical Re-imbursement" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblmedicalallowance" runat="server" Text='<%#Bind("medicalallowance","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalmedicalallowance"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 16--%>
                                            <asp:TemplateField HeaderText="Food Allw" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblFoodAllowance" runat="server" Text='<%#Bind("FoodAllowance","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalFoodAllowance"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 17--%>
                                            <asp:TemplateField HeaderText="NightShift Allw" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNightShiftAllw" runat="server" Text='<%#Bind("NightShiftAllw","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalNightShiftAllw"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 18--%>
                                            <asp:TemplateField HeaderText="Travel Allw" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblTravelAllw" runat="server" Text='<%#Bind("TravelAllw","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalTravelAllw"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 19--%>
                                            <asp:TemplateField HeaderText="Outfit Allw" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblOutFitAllw" runat="server" Text='<%#Bind("TravelAllw","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalOutFitAllwAllw"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 20--%>

                                            <asp:TemplateField HeaderText="L.W" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblLeaveEncashAmt" runat="server" Text='<%#Bind("LeaveEncashAmt","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalLeaveEncashAmt"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 21--%>
                                            <asp:TemplateField HeaderText="Gratuity" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblGratuity" runat="server" Text='<%#Bind("Gratuity","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalGratuity"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 22--%>
                                            <asp:TemplateField HeaderText="Bonus" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblBonus" runat="server" Text='<%#Bind("Bonus","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalBonus"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 23--%>

                                            <asp:TemplateField HeaderText="Nfhs" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNfhs" runat="server" Text='<%#Bind("Nfhs","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalNfhs"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 24--%>
                                            <asp:TemplateField HeaderText="RC" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblrc" runat="server" Text='<%#Bind("rc","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalrc"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 25--%>
                                            <asp:TemplateField HeaderText="CS" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblcs" runat="server" Text='<%#Bind("cs","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalcs"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 26--%>
                                            <asp:TemplateField HeaderText="Incentivs" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblIncentivs" runat="server" Text='<%#Bind("Incentivs","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalIncentivs"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 27--%>
                                            <asp:TemplateField HeaderText="WO Amt" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblWoAmt" runat="server" Text='<%#Bind("WOAmt","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalWOAmount"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 28--%>
                                            <asp:TemplateField HeaderText="NHs Amt" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNhsAmt" runat="server" Text='<%#Bind("Nhsamt","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalNhsAmount"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 29--%>
                                            <asp:TemplateField HeaderText="NPOTs Amt" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNpotsAmt" runat="server" Text='<%#Bind("Npotsamt","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalNpotsAmount"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 30--%>
                                            <asp:TemplateField HeaderText="Att Bonus" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblAttBonus" runat="server" Text='<%#Bind("Npotsamt","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalAttBonus"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 31--%>
                                            <asp:TemplateField HeaderText="Pay 1" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNPCl25Per" runat="server" Text='<%#Bind("pay1","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalNPCl25Per"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 32--%>
                                            <asp:TemplateField HeaderText="Pay 2" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblTransport6Per" runat="server" Text='<%#Bind("pay2","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalTransport6Per"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 33--%>
                                            <asp:TemplateField HeaderText="Pay 3" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblTransport" runat="server" Text='<%#Bind("pay3","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalTransport"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 34--%>
                                            <asp:TemplateField HeaderText="OT Amt" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblOTAmt" runat="server" Text='<%#Bind("OTAmt","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalOTAmount"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 35--%>
                                            <asp:TemplateField HeaderText="Addl Amount" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblAddlAmount" runat="server" Text='<%#Bind("AddlAmount","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lbltTotalAddlAmount"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <%-- 36--%>

                                            <asp:TemplateField HeaderText="EL Amount" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblELAmount" runat="server" Text='<%#Bind("ELAmount","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lbltTotalELAmount"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>


                                            <%-- 37--%>
                                            <asp:TemplateField HeaderText="Lunch Amount" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblLunchAmount" runat="server" Text='<%#Bind("LunchAmount","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lbltTotalLunchAmount"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <%-- 38--%>
                                            <asp:TemplateField HeaderText="Gross" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblGross" runat="server" Text='<%#Bind("Gross","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblTotalGross"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>


                                            <%-- 73--%>
                                            <asp:TemplateField HeaderText="PF" ItemStyle-HorizontalAlign="Left">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblGender" runat="server" Text='<%#Bind("PF") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>

                                            <%-- 74--%>
                                            <asp:TemplateField HeaderText="PT" ItemStyle-HorizontalAlign="Left">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblESINOr" runat="server" Text='<%#Bind("PT") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>


                                            <%-- 69--%>
                                            <asp:TemplateField HeaderText="TDSDed" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblMTDSDed" runat="server" Text='<%#Bind("TDS","{0:0}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <div style="text-align: justify">
                                                        <asp:Label runat="server" ID="lblTotaTDSDed"></asp:Label>
                                                    </div>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                        </Columns>
                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#2461BF" HorizontalAlign="Center" BorderWidth="1px" CssClass="GridPager" />
                                        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" Height="30" />
                                        <EditRowStyle ForeColor="#000" BackColor="#C2D69B" />
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
        <!-- DASHBOARD CONTENT END -->
        <!-- CONTENT AREA END -->
    </div>
    <!-- DASHBOARD CONTENT END -->
</asp:Content>
