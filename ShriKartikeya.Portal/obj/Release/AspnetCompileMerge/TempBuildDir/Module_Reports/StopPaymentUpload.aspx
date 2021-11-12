<%@ Page Title="" Language="C#" MasterPageFile="~/Module_Reports/ReportMaster.master" AutoEventWireup="true" CodeBehind="StopPaymentUpload.aspx.cs" Inherits="ShriKartikeya.Portal.Module_Reports.StopPaymentUpload" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="RightOne" ContentPlaceHolderID="ContentPlaceHolder3" runat="Server">

    <link rel="shortcut icon" href="assets/Mushroom.ico" />
    <link href="css/global.css" rel="stylesheet" type="text/css" />

    <script language="javascript" src="scripts\Calendar.js" type="text/javascript"></script>

    <link href="css/Calendar.css" rel="stylesheet" type="text/css" />

    <style type="text/css">
        #social div {
            display: block;
        }

        .HeaderStyle {
            text-align: Left;
        }

        .style1 {
            width: 106px;
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
           // .replace(/'/g, '&#39;')
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
    </script>


    <!-- CONTENT AREA BEGIN -->
    <div id="content-holder">
        <div class="content-holder">
            <h1 class="dashboard_heading"></h1>
            <!-- DASHBOARD CONTENT BEGIN -->
            <div class="contentarea" id="contentarea">
                <div class="dashboard_full">
                    <div class="sidebox">
                        <div>
                            <h4 style="text-align: right" runat="server">
                                <asp:LinkButton ID="lnkImportfromexcel" Text="Export Sample Excel" runat="server"
                                    OnClick="lnkImportfromexcel_Click"></asp:LinkButton>
                            </h4>
                        </div>
                        <div class="boxhead">
                            <h2 style="text-align: center">Stop Payment Upload&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </h2>
                        </div>

                        <asp:ScriptManager ID="ScriptManager1" runat="server">
                        </asp:ScriptManager>
                        <div class="boxbody" style="padding: 5px 5px 5px 5px; height: auto">
                            <!--  Content to be add here> -->

                            <div>

                                <table>
                                    <tr>
                                        <td style="width: 60px">Month</td>
                                        <td>

                                            <asp:TextBox ID="txtmonth" runat="server" Text="" class="sinput" Width="100px" AutoPostBack="True" OnTextChanged="txtmonth_TextChanged"></asp:TextBox>
                                            <cc1:CalendarExtender ID="Txt_Month_CalendarExtender" runat="server" BehaviorID="calendar1"
                                                Enabled="true" Format="MMM-yyyy" TargetControlID="txtmonth" DefaultView="Months" OnClientHidden="onCalendarHidden" OnClientShown="onCalendarShown">
                                            </cc1:CalendarExtender>
                                        </td>

                                        <td>&nbsp;</td>
                                        <td style="width: 60px">Select File: </td>
                                        <td>
                                            <asp:FileUpload ID="fileupload1" runat="server" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btnImport" runat="server" Text="Import Data" class=" btn save" OnClick="btnImport_Click" />
                                        </td>

                                    </tr>
                                </table>

                            </div>

                            <br />
                            <div>
                                <div>
                                    <asp:GridView ID="GVStopPayment" runat="server" Visible="false"
                                        AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333"
                                        GridLines="Both" HeaderStyle-CssClass="HeaderStyle" Height="140px"
                                        Style="margin-left: 50px" Width="90%">
                                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="Client ID">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblClientid" runat="server" Text=" "></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Emp ID">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblEmpID" runat="server" Text=" "></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Stop Payment">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblEmpID" runat="server" Text=" "></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                        </Columns>
                                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                        <EditRowStyle BackColor="#999999" />
                                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                    </asp:GridView>
                                </div>
                                <div align="center">
                                    <asp:Label ID="lblresult" runat="server" Style="color: Red" Visible="false"></asp:Label>
                                </div>
                                <div>
                                    <asp:GridView ID="GridView1" runat="server" Visible="false"
                                        AutoGenerateColumns="true" CellPadding="4" ForeColor="#333333"
                                        GridLines="Both" HeaderStyle-CssClass="HeaderStyle" Height="140px"
                                        Style="margin-left: 50px" Width="90%">
                                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                        <Columns>
                                        </Columns>
                                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                        <EditRowStyle BackColor="#999999" />
                                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                    </asp:GridView>
                                </div>
                                <asp:Label ID="lblMessage" runat="server" Style="color: Red"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- DASHBOARD CONTENT END -->
            </div>
        </div>
        <!-- CONTENT AREA END -->
        <!-- FOOTER BEGIN -->
    </div>
    </div>

</asp:Content>


