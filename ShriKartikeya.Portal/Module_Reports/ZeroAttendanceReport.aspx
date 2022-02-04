<%@ Page Title="" Language="C#" MasterPageFile="~/Module_Reports/ReportMaster.master" AutoEventWireup="true" CodeBehind="ZeroAttendanceReport.aspx.cs" Inherits="ShriKartikeya.Portal.ZeroAttendanceReport" %>

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
                    <li class="active"><a href="#" style="z-index: 7;" class="active_bread">Zero Attendance Report</a></li>
                </ul>
            </div>
            <!-- DASHBOARD CONTENT BEGIN -->
            <div class="contentarea" id="contentarea">
                <div class="dashboard_center">
                    <div class="sidebox">
                         <div>
                            <h4 style="text-align: right">
                               <asp:LinkButton ID="lnkImportfromexcel" Text="Export Sample Excel" runat="server" 
                                    onclick="lnkImportfromexcel_Click"></asp:LinkButton> </h4>
                        </div>
                        <div class="boxhead">
                            <h2 style="text-align: center">
                                IMPORT LEFT EMPLOYEE DETAILS
                            </h2>
                        </div>
                        <div class="boxbody" style="padding: 5px 5px 5px 5px;">
                            <div class="boxin">
                                <asp:ScriptManager runat="server" ID="ScriptEmployReports">
                                </asp:ScriptManager>
                                <%--<div align="right">
                                <asp:LinkButton ID="lbtn_Export" runat="server" OnClick="lbtn_Export_Click" >Export to Excel</asp:LinkButton>
                            </div>--%>
                               
                                <div class="dashboard_firsthalf" style="width: 700px;">
                                    <br />                                                                                         
                                </div>
                                <table>
                                    <tr>
                                         <td>Month</td>
                             <td>
                             <asp:TextBox ID="txtmonth" runat="server" class="sinput" AutoComplete="off" AutoPostBack="true" OnTextChanged="txtmonth_TextChanged"></asp:TextBox>
                             <cc1:CalendarExtender ID="CalendarExtender1" runat="server" BehaviorID="calendar1"
                              Enabled="true" Format="MMM-yyyy" TargetControlID="txtmonth" DefaultView="Months" OnClientHidden="onCalendarHidden" OnClientShown="onCalendarShown">
                              </cc1:CalendarExtender>
                              </td>
                                        <td style="padding-left:50px">

                                            <asp:Label ID="lblfileupload" runat="server" Text="File Upload"></asp:Label>         

                                        </td>
                                        <td>
                                            <asp:FileUpload ID="FileUploadEmpDetails" runat="server" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btnsave" runat="server" Text="Import" OnClick="btnsave_Click" />
                                        </td>
                                        
                                    </tr>
                                </table>
                                <div>
                                    <div class="rounded_corners" >
                                         <div style="overflow: scroll; width: auto">
                                        <asp:GridView ID="gvlistofemp" runat="server" AutoGenerateColumns="False" Width="100%" Visible="false" OnRowDataBound="gvlistofemp_RowDataBound" 
                                            ForeColor="#333333" GridLines="None" CellPadding="4" CellSpacing="3" Style="text-align: center" Height="50px">
                                            <RowStyle BackColor="#EFF3FB" Height="30" />
                                            <Columns>
                                               
                                                <asp:TemplateField HeaderText="IDNO" ItemStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblIDNO" Text='<%#Bind("EmpId") %>' ></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                             

                                                 <asp:TemplateField HeaderText="Emp Name" ItemStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblEmpName" Text='<%#Bind("Name") %>' ></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                   <asp:TemplateField HeaderText="ClientName" ItemStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblSitePosted" Text='<%#Bind("ClientName") %>' ></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                
                                                 <asp:TemplateField HeaderText="Date of leaving" ItemStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblDateofleaving" Text='<%#Bind("Dateofleaving") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
        
                                            </Columns>
                                            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White"  Height="30" />
                                            <EditRowStyle BackColor="#2461BF" />
                                            <AlternatingRowStyle BackColor="White" />
                                        </asp:GridView>
                                    </div>

                                     <div class="rounded_corners" style="overflow:auto">
                                        <asp:GridView ID="GvNonInsertEmployees" runat="server" AutoGenerateColumns="False" Width="137%" Visible="false"
                                            ForeColor="#333333" GridLines="None" CellPadding="4" CellSpacing="3" Style="text-align: center" Height="238px">
                                            <RowStyle BackColor="#EFF3FB" Height="30" />
                                            <Columns>
                                                 <asp:TemplateField HeaderText="EmpId">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblEmpId"  ></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Date of Leaving">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblDtofleaving" Text='<%#Eval("AadharCardNo","{0:X}")%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                                 <asp:TemplateField HeaderText="Remarks">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblRemarks"  ></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>


                                                 </Columns>
                                            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White"  Height="30" />
                                            <EditRowStyle BackColor="#2461BF" />
                                            <AlternatingRowStyle BackColor="White" />
                                        </asp:GridView>
                                    </div>



                                     <div class="rounded_corners" style="overflow:auto">
                                        <asp:GridView ID="GvListOfInstructions" runat="server" AutoGenerateColumns="False" Width="137%" Visible="false"
                                            ForeColor="#333333" GridLines="None" CellPadding="4" CellSpacing="3" Style="text-align: center" Height="238px">
                                            <RowStyle BackColor="#EFF3FB" Height="30" />
                                            <Columns>
                                                 <asp:TemplateField HeaderText="SNO">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblSNO" Text='<%#Bind("Sno") %>' ></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                 <asp:TemplateField HeaderText="Instructions">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblInstructions" Text='<%#Bind("Instructions") %>' ></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>


                                                 </Columns>
                                            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White"  Height="30" />
                                            <EditRowStyle BackColor="#2461BF" />
                                            <AlternatingRowStyle BackColor="White" />
                                        </asp:GridView>
                                    </div>
<%--                                    <asp:Label ID="LblResult" runat="server" Text="" Style="color: red"></asp:Label>--%>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="clear">
                </div>
            </div>
        </div>
        </div>
        <!-- DASHBOARD CONTENT END -->
        <!-- CONTENT AREA END -->
    </div>
    <!-- DASHBOARD CONTENT END -->
</asp:Content>
