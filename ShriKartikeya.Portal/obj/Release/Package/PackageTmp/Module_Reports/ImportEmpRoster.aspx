<%@ Page Title="" Language="C#" MasterPageFile="~/Module_Reports/ReportMaster.master" AutoEventWireup="true" CodeBehind="ImportEmpRoster.aspx.cs" Inherits="ShriKartikeya.Portal.ImportEmpRoster" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder3" runat="server">



    <link href="css/global.css" rel="stylesheet" type="text/css" />


    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/css/bootstrap.min.css" />
    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>

    <style type="text/css">
        .col-md-12 {
            max-width: 98%;
        }
    </style>
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

    <script type="text/javascript">
        $(document).ready(function () {
            $('#lnkDownloadRoster').on('click', function (e) {

                e.preventDefault();

                if ($("#<%=txtMonth.ClientID %>").val() == "") {

                    alert("Enter month");
                }
                else {

                    $("#<%=btnhidden.ClientID %>").click();

                }
            })


            $('#btnImport').on('click', function (e) {

                e.preventDefault();

                if ($("#<%=txtMonth.ClientID %>").val() == "") {

                            alert("Enter month");
                        }
                        else if (document.getElementById("fileupload").files.length == 0) {

                            alert("Please choose file");
                        }
                        else {

                            $("#<%=btnhide.ClientID %>").click();

                        }
                    })
        });
    </script>

    <div class="cotainer" style="margin-top: 10px">
        <div class="row justify-content-center">
            <div class="col-md-12">
                <div class="card">

                       <asp:ScriptManager runat="server" ID="ScriptEmployReports">
                                </asp:ScriptManager>

                    <div class="card-header">Import Roster</div>
                    <div class="card-body">

                        <table style="margin: 0px auto" width="87%">
                            <tr>


                                <td>
                                    <asp:Label ID="lblMonth" runat="server" Text="Month"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtMonth" runat="server" class="sinput" autocomplete="off"></asp:TextBox>
                                    <cc1:CalendarExtender ID="Txt_Month_CalendarExtender" runat="server" BehaviorID="calendar1"
                                        Enabled="true" Format="MMM-yyyy" TargetControlID="txtMonth" DefaultView="Months" OnClientHidden="onCalendarHidden" OnClientShown="onCalendarShown"></cc1:CalendarExtender>
                                </td>

                                <td>
                                    <asp:FileUpload ID="fileupload" runat="server" />
                                </td>

                                <td>
                                    <asp:Button ID="btnImport" runat="server" Text="Import" class=" btn save" ToolTip="Submit" OnClick="btnImport_Click" />
                                </td>

                                <td>
                                    <asp:Button ID="btnNotInsert" runat="server" Text="Not Imported Data" class=" btn save" ToolTip="Submit" OnClick="btnNotInsert_Click" Visible="false" />
                                </td>

                                <td>
                                    <asp:LinkButton ID="btnExport" runat="server" Text="Export Sample Sheet" class=" btn save"
                                        OnClick="btnExport_Click" />
                                </td>

                                <td>
                                    <asp:LinkButton ID="lnkDownloadRoster" runat="server" Text="Download Roster" class=" btn save" OnClick="lnkDownloadRoster_Click" />

                                </td>

                            </tr>
                        </table>

                        <asp:Button ID="btnhidden" runat="server" Text="Submit" class="btn cnt-create-btn" Style="font-size: 12px; visibility: hidden" OnClick="lnkDownloadRoster_Click"></asp:Button>
                        <asp:Button ID="btnhide" runat="server" Text="Submit" class="btn cnt-create-btn" Style="font-size: 12px; visibility: hidden" OnClick="btnImport_Click"></asp:Button>


                        <div>
                            <asp:GridView ID="GVAttendanceData" runat="server" AutoGenerateColumns="true" CssClass="table table-striped table-bordered table-condensed table-hover" Visible="false">
                                <Columns>
                                </Columns>
                            </asp:GridView>

                        </div>

                        <div style="margin-top: 10px">
                            <asp:GridView ID="GvEmpList" runat="server" AutoGenerateColumns="true" CssClass="table table-striped table-bordered table-condensed table-hover">
                                <Columns>
                                </Columns>
                            </asp:GridView>
                        </div>

                    </div>
                </div>

            </div>
        </div>
        <div class="clear">
        </div>
    </div>

</asp:Content>