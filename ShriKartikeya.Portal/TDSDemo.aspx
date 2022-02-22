<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TDSDemo.aspx.cs" Inherits="ShriKartikeya.Portal.TDSDemo" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- Required meta tags -->

    <!-- Bootstrap CSS -->

    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/css/bootstrap.min.css" />

    <script src="script/jquery.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="script/jscript.js"> </script>

    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />
    <script type="text/javascript" src="//code.jquery.com/jquery-1.10.2.js"></script>
    <script type="text/javascript" src="//code.jquery.com/ui/1.11.2/jquery-ui.js"></script>


    <title>Form12BB</title>
    <style type="text/css">
        .card {
            margin-top: 40px;
        }

        .card-header {
            font-weight: bold;
        }
    </style>

    <style>
        h1 {
            text-align: center;
            padding-top: 40px;
        }

        p {
            text-align: center;
            font-size: 20px;
        }

        .Centerallign {
            text-align: center;
            font-family: inherit;
            font-size: 18px;
        }

        .button1 {
            background-color: white;
            color: black;
            border: 2px solid #4CAF50;
        }

            .button1:hover {
                background-color: #4CAF50;
                color: white;
            }

        .button2 {
            background-color: white;
            color: black;
            border: 2px solid red;
        }

            .button2:hover {
                background-color: red;
                color: white;
            }

        .ModalPopupBG {
            background-color: rgba(128, 128, 128,0.5);
            z-index: 10000;
        }
    </style>


    <script type="text/javascript">

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


        function GetEmpid() {
            $('#txtempid').autocomplete({
                source: function (request, response) {


                    $.ajax({
                        url: 'Autocompletion.asmx/GetFormEmpIDs',
                        method: 'post',
                        contentType: 'application/json;charset=utf-8',
                        data: JSON.stringify({
                            term: request.term,
                        }),
                        datatype: 'json',
                        success: function (data) {
                            response(data.d);
                        },
                        error: function (err) {
                            alert(err);
                        }
                    });
                },
                minLength: 4,
                select: function (event, ui) {

                    $("#txtempid").attr("data-Empid", ui.item.value); OnAutoCompletetxtEmpidchange(event, ui);
                }
            });
        }

        function GetEmpName() {

            $('#txtname').autocomplete({
                source: function (request, response) {
                    $.ajax({

                        url: 'Autocompletion.asmx/GetFormEmpNames',
                        method: 'post',
                        contentType: 'application/json;charset=utf-8',
                        data: JSON.stringify({
                            term: request.term,
                        }),
                        datatype: 'json',
                        success: function (data) {
                            response(data.d);
                        },
                        error: function (err) {
                            alert(err);
                        }
                    });
                },
                minLength: 4,
                select: function (event, ui) {
                    $("#txtname").attr("data-EmpName", ui.item.value); OnAutoCompletetxtEmpNamechange(event, ui);
                }
            });

        }

        function OnAutoCompletetxtEmpidchange(event, ui) {
            $('#txtempid').trigger('change');

        }
        function OnAutoCompletetxtEmpNamechange(event, ui) {
            $('#txtname').trigger('change');

        }

        $(document).ready(function () {

            GetEmpid();
            GetEmpName();

        });


    </script>


    <script type="text/javascript">

        $("#ddlplaceofstay").change(function () {
            alert($("#ddlplaceofstay").val());
        });

    </script>

    <script type="text/javascript">

        //function pageLoad(sender, args) {
        //    if (!args.get_isPartialLoad()) {

        //        document.getElementById("txttempremarks").disabled = true;
        //        document.getElementById("txtDeactremarks").disabled = true;
        //    }
        //}




        //$(document).ready(function () {
        //    $("#txtDate").datepicker({
        //        dateFormat: 'dd/mm/yy'
        //    });
        //});

        //var prm = Sys.WebForms.PageRequestManager.getInstance();
        //if (prm != null) {
        //    prm.add_endRequest(function (sender, e) {
        //        if (sender._postBackSettings.panelsToUpdate != null) {
        //            $("#txtDate").datepicker({
        //                dateFormat: 'dd/mm/yy'
        //            });
        //        }
        //    })
        //}

    </script>
</head>
<body>
    <form id="form1" runat="server" autocomplete="off">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>

        <div style="margin-top: 10px; float: right">

            <asp:LinkButton ID="lnkBack" runat="server" Text="Back" OnClick="lnkBack_Click" Style="margin-right: 100px;"></asp:LinkButton>


        </div>

        <div style="width: 90%; padding-left: 110px">


           <%-- <h1>FORM NO. 12BB</h1>
            <p>(See Rule 26c)</p>--%>

            <div class="Centerallign" style="font-size: 20px; font-weight: 500" runat="server" visible="false">
                Declaration by employee for claiming tax benefits for House Rent Allowance (HRA), Leave Travel Allowance / Concession (LTA / LTC), interest paid on housing loans and other tax saving deductions.
            </div>


            <div class="Centerallign" style="padding-top: 30px" runat="server" visible="false">
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
             Earlier there was no standard format required by salaried employees to disclose their investment details to their employers. The Central Board of Direct Taxes
(CBDT) has introduced a new Form 12BB. This form, applicable from June 1, 2016, needs to be submitted to your employer and will be used to declare your
investments and claim tax deductions under HRA, LTA, Section 80, interest paid on home loans etc. Use this page to generate your Form 12BB and submit to HR of
 your company.
            </div>
            <br />
            <br />

            <div class="card">

                <div class="card-body">
                    <table width="100%" cellpadding="5" cellspacing="5">
                        <tr>
                            <td>Financial Year
                            </td>
                            <td>
                                <asp:Label runat="server" Visible="false" ID="lblFyear" Text=""></asp:Label>
                                <asp:Label runat="server" ID="lblfinancialyearDates" Text=""></asp:Label>
                                <%-- <asp:Label runat="server" ID="lblFYear" Text="2021-22 (1st April 2021 to 31st March 2022)" class="form-control"></asp:Label>--%>
                            </td>
                        </tr>
                    </table>
                </div>

            </div>



            <div class="card">

                <div class="card-body">
                    <table width="60%" cellpadding="5" cellspacing="5">



                        <tr>
                            <td>Emp ID<span style="color: Red">*</span>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtempid" AutoPostBack="true" OnTextChanged="txtemplyid_TextChanged" class="form-control"></asp:TextBox>
                            </td>
                        </tr>



                        <tr>
                            <td>Name <span style="color: Red">*</span>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtname" OnTextChanged="txtFname_TextChanged" class="form-control"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td>DOB<span style="color: Red">*</span>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtDOB" Width="150px" Enabled="false" class="form-control"></asp:TextBox>
                                <asp:TextBox runat="server" ID="txtage" Width="50px" Enabled="false" Style="margin-left: 160px; margin-top: -37px;" class="form-control"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td>PAN
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtpan" Enabled="true" class="form-control"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td>Email ID<span style="color: Red">*</span>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtemailid" Enabled="false" class="form-control"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td>Mobile<span style="color: Red">*</span>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtmobile" Enabled="false" class="form-control"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td>Father's Name<span style="color: Red">*</span>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtfathername" Enabled="false" class="form-control"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td>Address<%--<span style="color: Red">*</span>--%>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtaddress" Enabled="false" class="form-control"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td>Place<%--<span style="color: Red">*</span>--%>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtplace" Enabled="false" class="form-control"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td>Date<span style="color: Red">*</span>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtdate" Enabled="false" class="form-control"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td>Designation
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtdesignation" Enabled="false" class="form-control"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td>Name of Organisation<span style="color: Red">*</span>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtOrganisation" Enabled="false" class="form-control"></asp:TextBox>
                            </td>
                        </tr>


                        <tr>
                            <td>Email ID of Organisation
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtemailOrganisation" Enabled="false" placeholder="Email ID of HR of your Organisation" class="form-control"></asp:TextBox>
                            </td>
                        </tr>


                        <tr>
                            <td>Salary<span style="color: Red">*</span>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtincome" class="form-control"></asp:TextBox>
                                <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender8" runat="server" Enabled="True"
                                    TargetControlID="txtIncome" ValidChars="0123456789.">
                                </cc1:FilteredTextBoxExtender>
                            </td>
                        </tr>

                        <tr>
                            <td>Other Income
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtotherIncome" class="form-control"></asp:TextBox>
                                <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender9" runat="server" Enabled="True"
                                    TargetControlID="txtIncome" ValidChars="0123456789.">
                                </cc1:FilteredTextBoxExtender>
                            </td>
                        </tr>

                        <tr>
                            <td>Remarks
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtOtherincomeRemarks" placeholder="Mention Other Income Remarks" class="form-control"></asp:TextBox>
                            </td>
                        </tr>

                    </table>
                </div>

            </div>


            <div class="card">

                <div class="card-body">

                    <p style="font-size: 20px; font-weight: 500">HRA Exemption Calculation</p>
                    <table width="60%" cellpadding="5" cellspacing="5">

                        <tr>
                            <td>Basic: 
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtbasic" Text="0" class="form-control"></asp:TextBox>
                                <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender10" runat="server" Enabled="True"
                                    TargetControlID="txtbasic" ValidChars="0123456789.">
                                </cc1:FilteredTextBoxExtender>
                            </td>
                        </tr>

                        <tr>
                            <td>HRA: 
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtHra" Text="0" class="form-control"></asp:TextBox>
                                <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender11" runat="server" Enabled="True"
                                    TargetControlID="txtHra" ValidChars="0123456789.">
                                </cc1:FilteredTextBoxExtender>
                            </td>
                        </tr>

                        <tr>
                            <td>Rent Paid: 
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtRentPaid" Text="0" class="form-control"></asp:TextBox>
                                <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender12" runat="server" Enabled="True"
                                    TargetControlID="txtRentPaid" ValidChars="0123456789.">
                                </cc1:FilteredTextBoxExtender>
                            </td>
                        </tr>


                        <tr>
                            <td>Place of stay
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlplaceofstay" runat="server" CssClass="form-control">
                                    <asp:ListItem>--Select--</asp:ListItem>
                                    <asp:ListItem>MCDC(50%)</asp:ListItem>
                                    <asp:ListItem>Others(40%)</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>


                        <tr>
                            <td>Final HRA
                            </td>
                            <td>

                                <asp:TextBox runat="server" ID="txtFinalHRA" Text="0" Enabled="false" class="form-control"></asp:TextBox>
                                <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender13" runat="server" Enabled="True"
                                    TargetControlID="txtFinalHRA" ValidChars="0123456789.">
                                </cc1:FilteredTextBoxExtender>

                            </td>
                        </tr>

                        <tr>
                            <td>PAN No
                            </td>
                            <td>

                                <asp:TextBox runat="server" ID="txthrapano" Text="" class="form-control"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td>Address of Landlord
                            </td>
                            <td>

                                <asp:TextBox runat="server" ID="txthraAddressofLandlord" Text="" class="form-control"></asp:TextBox>
                            </td>
                        </tr>


                    </table>



                    <div style="margin-left: 790px; float: right">
                        <table>
                            <tr>
                                <td>
                                    <asp:Button ID="btnHRACalculate" runat="server" class="btn save" Text="Calculate" OnClick="btnHRACalculate_Click"
                                        Style="width: 100px" /><br />
                                </td>

                            </tr>


                        </table>

                    </div>


                </div>

            </div>

            <div class="card" runat="server" visible="false">

                <div class="card-body">

                    <p style="font-size: 20px; font-weight: 500">House Rent Allowance (HRA)</p>
                    <%-- <tr>
                        <td>(If you do not have your rent receipt click here to generate HRA rent receipt.)
                        </td>
                    </tr>--%>
                    <table width="100%" cellpadding="5" cellspacing="5">

                        <asp:GridView ID="GVHRA" runat="server" Width="100%" Style="margin-left: 5px"
                            AutoGenerateColumns="False" CellPadding="5" CellSpacing="3" ForeColor="#333333" GridLines="None"
                            CssClass="table table-striped table-bordered table-condensed table-hover">
                            <Columns>


                                <asp:TemplateField HeaderText="Rent paid to the landlord">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtRentpaid" runat="server" placeholder="Amount" Width="100%" class="form-control" Style="text-align: center"></asp:TextBox>
                                        <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" Enabled="True"
                                            TargetControlID="txtRentpaid" ValidChars="0123456789.">
                                        </cc1:FilteredTextBoxExtender>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Proof of Rent Paid">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtrentProof" runat="server" placeholder="HRA rent receipt" Width="100%" class="form-control" Style="text-align: center"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Particulars of Landlord">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtLandlord" runat="server" placeholder="Name" Width="100%" class="form-control" Style="text-align: center"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="PAN">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtPAN" runat="server" placeholder="PAN" Width="100%" class="form-control" Style="text-align: center"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Address of Landlord">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtAddressofLandlord" runat="server" placeholder="" Width="100%" class="form-control" Style="text-align: center"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                        </asp:GridView>

                        <div style="margin-left: 90px; float: left">
                            <asp:Button ID="Button3" runat="server" class="btn save" Visible="false" Text="+Add"
                                OnClick="btnadddesgn_Click" Style="width: 125px" /><br />

                        </div>

                    </table>
                </div>

            </div>


            <div class="card" runat="server">

                <div class="card-body">

                    <p style="font-size: 20px; font-weight: 500">Income from House Property</p>
                    <%-- <tr>
                        <td>(If you do not have your rent receipt click here to generate HRA rent receipt.)
                        </td>
                    </tr>--%>
                    <table width="100%" cellpadding="5" cellspacing="5">

                        <asp:GridView ID="gvIncomeHouse" runat="server" Width="100%" Style="margin-left: 5px"
                            AutoGenerateColumns="False" CellPadding="5" CellSpacing="3" ForeColor="#333333" GridLines="None"
                            CssClass="table table-striped table-bordered table-condensed table-hover">
                            <Columns>


                                <asp:TemplateField HeaderText="Type" HeaderStyle-HorizontalAlign="Center" Visible="false" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center" FooterStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddlrenttypes" class="form-control" runat="server" Width="150px">
                                            <asp:ListItem Selected="True" Value="0">--Select-- </asp:ListItem>
                                        </asp:DropDownList>
                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="House">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txttype" runat="server" Width="100%" class="form-control" Style="text-align: center"></asp:TextBox>
                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Self/Let Out">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtselflayout" runat="server" Width="100%" class="form-control" Style="text-align: center"></asp:TextBox>
                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Gross Annual Rent">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtgrossannualreport" runat="server" Width="100%" Text="0" class="form-control" Style="text-align: center"></asp:TextBox>
                                        <cc1:FilteredTextBoxExtender ID="fldtxtgrossannualreport" runat="server" Enabled="True"
                                            TargetControlID="txtgrossannualreport" ValidChars="-0123456789.">
                                        </cc1:FilteredTextBoxExtender>
                                    </ItemTemplate>

                                </asp:TemplateField>


                                <asp:TemplateField HeaderText="Municipal Tax Paid">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtMunicipaltax" runat="server" Width="100%" Text="0" class="form-control" Style="text-align: center"></asp:TextBox>
                                        <cc1:FilteredTextBoxExtender ID="fldtxtMunicipaltax" runat="server" Enabled="True"
                                            TargetControlID="txtMunicipaltax" ValidChars="-0123456789.">
                                        </cc1:FilteredTextBoxExtender>
                                    </ItemTemplate>

                                </asp:TemplateField>


                                <asp:TemplateField HeaderText="Net Annual Rent">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtnetannaul" runat="server" Width="100%" Text="0" Enabled="false" class="form-control" Style="text-align: center"></asp:TextBox>
                                        <cc1:FilteredTextBoxExtender ID="fldtxtnetannaul" runat="server" Enabled="True"
                                            TargetControlID="txtnetannaul" ValidChars="-0123456789.">
                                        </cc1:FilteredTextBoxExtender>
                                    </ItemTemplate>

                                </asp:TemplateField>


                                <asp:TemplateField HeaderText="Less Std Ded @30%">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtlessstd" runat="server" Width="100%" Text="0" Enabled="false" class="form-control" Style="text-align: center"></asp:TextBox>
                                        <cc1:FilteredTextBoxExtender ID="fldtxtlessstd" runat="server" Enabled="True"
                                            TargetControlID="txtlessstd" ValidChars="-0123456789.">
                                        </cc1:FilteredTextBoxExtender>
                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Interest  on Housing Loan">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtInterest" runat="server" Width="110%" Text="0" class="form-control" Style="text-align: center"></asp:TextBox>
                                        <cc1:FilteredTextBoxExtender ID="fldtxtInterest" runat="server" Enabled="True"
                                            TargetControlID="txtInterest" ValidChars="-0123456789.">
                                        </cc1:FilteredTextBoxExtender>
                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Income from House Property">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtincome" runat="server" Width="110%" Text="0" Enabled="false" class="form-control" Style="text-align: center"></asp:TextBox>
                                        <cc1:FilteredTextBoxExtender ID="fldtxtincome" runat="server" Enabled="True"
                                            TargetControlID="txtincome" ValidChars="-0123456789.">
                                        </cc1:FilteredTextBoxExtender>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                        </asp:GridView>

                        <div style="margin-left: 790px; float: right">
                            <table>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtincometotal" runat="server" Width="100%" Text="0" Enabled="false" class="form-control" Style="text-align: center"></asp:TextBox>
                                        <cc1:FilteredTextBoxExtender ID="fldtxtincome" runat="server" Enabled="True"
                                            TargetControlID="txtincome" ValidChars="-0123456789.">
                                        </cc1:FilteredTextBoxExtender>
                                    </td>
                                    <td>
                                        <asp:Button ID="btninvcalculate" runat="server" class="btn save" Text="Calculate" OnClick="btninvcalculate_Click"
                                            Style="width: 100px" /><br />
                                    </td>

                                </tr>


                            </table>

                        </div>

                        <div style="margin-left: 90px; float: left">
                            <asp:Button ID="Button4" runat="server" class="btn save" Visible="false" Text="+Add"
                                OnClick="btnadddesgnre_Click1" Style="width: 125px" /><br />

                        </div>

                    </table>
                </div>

            </div>


        </div>
        <div style="width: 90%; padding-left: 110px">


            <div class="card">

                <div class="card-body">
                    <p style="font-size: 20px; font-weight: 500">Leave Travel Allowance / Concession (LTA / LTC)</p>


                    <table width="100%" cellpadding="5" cellspacing="5">

                        <asp:GridView ID="gvLTA" runat="server" Width="100%" Style="margin-left: 5px"
                            AutoGenerateColumns="False" CellPadding="5" CellSpacing="3" ForeColor="#333333" GridLines="None"
                            CssClass="table table-striped table-bordered table-condensed table-hover">
                            <Columns>


                                <asp:TemplateField HeaderText="Leave travel concessions or assistance">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtLTAAmount" runat="server" placeholder="Amount" Width="100%" class="form-control" Style="text-align: center"></asp:TextBox>
                                        <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender2" runat="server" Enabled="True"
                                            TargetControlID="txtLTAAmount" ValidChars="0123456789.">
                                        </cc1:FilteredTextBoxExtender>
                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Proof of Travel Expenses">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtltaExpenses" runat="server" placeholder="Mention proof that you will submit" Width="100%" class="form-control" Style="text-align: center"></asp:TextBox>
                                    </ItemTemplate>

                                </asp:TemplateField>

                            </Columns>
                        </asp:GridView>

                        <div style="margin-left: 90px; float: left">
                            <asp:Button ID="btnadddesgn" runat="server" class="btn save" Visible="false" Text="+Add"
                                OnClick="btnadddesgn_Click1" Style="width: 125px" /><br />

                        </div>

                    </table>
                </div>

            </div>
        </div>
        <div style="width: 90%; padding-left: 110px">

            <div class="card">

                <div class="card-body">

                    <table width="80%" cellpadding="5" cellspacing="5">
                        <p style="font-size: 20px; font-weight: 500">Deduction of Interest on Housing Loan</p>
                        <tr>
                            <td>Interest payable/paid to the lender
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtinterestpayable" placeholder="Amount" class="form-control"></asp:TextBox>
                                <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender3" runat="server" Enabled="True"
                                    TargetControlID="txtinterestpayable" ValidChars="0123456789.">
                                </cc1:FilteredTextBoxExtender>
                            </td>

                            <td>
                                <asp:TextBox runat="server" ID="txtproofofinterest" placeholder="Proof of Interest on Housing Loan " Width="280px" class="form-control"></asp:TextBox>
                            </td>

                        </tr>



                    </table>

                    <table width="82%" cellpadding="5" cellspacing="5">
                        <p style="font-size: 20px; font-weight: 500">Particulars of Lender</p>
                        <tr>
                            <td>Type of Lender
                            </td>
                            <td>
                                <asp:DropDownList ID="ddltypeofvendor" runat="server" CssClass="form-control">
                                    <asp:ListItem>--Select--</asp:ListItem>
                                    <asp:ListItem>Financial Institutions</asp:ListItem>
                                    <asp:ListItem>Employer</asp:ListItem>
                                    <asp:ListItem>Others</asp:ListItem>

                                </asp:DropDownList>
                            </td>
                        </tr>

                        <tr>
                            <td>Name
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtLendername" class="form-control"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td>Address
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtLenderaddress" class="form-control"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td>PAN
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtLenderpan" class="form-control"></asp:TextBox>
                            </td>
                        </tr>


                    </table>



                </div>

            </div>
        </div>
        <div style="width: 90%; padding-left: 110px">

            <div class="card">

                <div class="card-body">
                    <p style="font-size: 20px; font-weight: 500">Tax Saving Deductions</p>
                    <table width="100%" cellpadding="5" cellspacing="5">
                        <tr>
                            <td style="font-size: 15px">(A) Section 80C, 80CCC and 80CCD
                   
                            </td>
                        </tr>
                        <tr>
                            <td style="font-size: 12px">&nbsp;&nbsp;&nbsp;&nbsp;(i) Section 80C - Payments or investments in Life insurance, PPF, PF, NSC, tuition fees, etc.
                    
                   
                            </td>
                        </tr>

                    </table>
                    <table width="100%" cellpadding="5" cellspacing="5">


                        <asp:GridView ID="GvTaxdeds" runat="server" Width="100%" Style="margin-left: 5px"
                            AutoGenerateColumns="False" CellPadding="5" CellSpacing="3" ForeColor="#333333" GridLines="None"
                            CssClass="table table-striped table-bordered table-condensed table-hover">
                            <Columns>


                                <asp:TemplateField HeaderText="Deductions" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center" FooterStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddltaxDed" class="form-control" runat="server" Width="280px">
                                            <asp:ListItem Selected="True" Value="0">--Select-- </asp:ListItem>
                                        </asp:DropDownList>
                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Amount">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txttaxdedamount" runat="server" placeholder="Amount" Width="100%" class="form-control" Style="text-align: center"></asp:TextBox>
                                        <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender4" runat="server" Enabled="True"
                                            TargetControlID="txttaxdedamount" ValidChars="0123456789.">
                                        </cc1:FilteredTextBoxExtender>
                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Proof of investement">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtltaExpenses" runat="server" placeholder="Mention proof that you will submit" Width="100%" class="form-control" Style="text-align: center"></asp:TextBox>
                                    </ItemTemplate>

                                </asp:TemplateField>

                            </Columns>
                        </asp:GridView>

                        <div style="margin-left: 90px; float: left">
                            <asp:Button ID="Button1" runat="server" Visible="false" class="btn save" Text="+Add"
                                OnClick="btnadddesgn2_Click1" Style="width: 125px" /><br />

                        </div>



                    </table>
                    <table width="100%" cellpadding="5" cellspacing="5">

                        <tr>
                            <td style="font-size: 12px">&nbsp;&nbsp;&nbsp;&nbsp;(ii) Section 80CCC - Contribution to Pension Fund
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txt80CCAmt" class="form-control"></asp:TextBox>
                                <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender5" runat="server" Enabled="True"
                                    TargetControlID="txt80CCAmt" ValidChars="0123456789.">
                                </cc1:FilteredTextBoxExtender>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txt80CCproof" placeholder="Mention proof that you will submit" class="form-control"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td style="font-size: 12px">&nbsp;&nbsp;&nbsp;&nbsp;(ii) Section 80CCD - Contribution to National Pension Scheme of Government
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txt80CCDAmt" class="form-control"></asp:TextBox>
                                <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender6" runat="server" Enabled="True"
                                    TargetControlID="txt80CCDAmt" ValidChars="0123456789.">
                                </cc1:FilteredTextBoxExtender>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txt80CCDproof" placeholder="Mention proof that you will submit" class="form-control"></asp:TextBox>
                            </td>
                        </tr>

                    </table>



                    <table width="100%" cellpadding="5" cellspacing="5">
                        <tr>
                            <td style="font-size: 15px">(B) Other sections (e.g. 80E, 80G, 80TTA, etc.) under Chapter VI-A.
                   
                            </td>
                        </tr>
                    </table>
                    <table width="100%" cellpadding="5" cellspacing="5">


                        <asp:GridView ID="GVOtherSections" runat="server" Width="99%" Style="margin-left: 5px"
                            AutoGenerateColumns="False" CellPadding="5" CellSpacing="3" ForeColor="#333333" GridLines="None"
                            CssClass="table table-striped table-bordered table-condensed table-hover">
                            <Columns>
                                <asp:TemplateField HeaderText="Section" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center" FooterStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddlsection" class="form-control" runat="server" Width="280px">
                                            <asp:ListItem Selected="True" Value="0">--Select-- </asp:ListItem>
                                        </asp:DropDownList>
                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Amount">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtothersecamount" runat="server" placeholder="Amount" Width="100%" class="form-control" Style="text-align: center"></asp:TextBox>
                                        <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender7" runat="server" Enabled="True"
                                            TargetControlID="txtothersecamount" ValidChars="0123456789.">
                                        </cc1:FilteredTextBoxExtender>
                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Proof of investement">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtothersProof" runat="server" placeholder="Mention proof that you will submit" Width="100%" class="form-control" Style="text-align: center"></asp:TextBox>
                                    </ItemTemplate>

                                </asp:TemplateField>

                            </Columns>
                        </asp:GridView>

                        <div style="margin-left: 90px; float: left">
                            <asp:Button ID="Button2" runat="server" class="btn save" Visible="false" Text="+Add"
                                OnClick="btnadddesgn3_Click1" Style="width: 125px" /><br />

                        </div>



                    </table>

                </div>

            </div>
        </div>



        <div style="margin-right: 150px; margin-top: 10px; float: right">
            <asp:Button ID="btnCalculate" runat="server" class="button button1" Text="Calculate"
                OnClick="btnCalculate_Click" Style="width: 125px" />

        </div>
        <br />
        <br />
        <br />


        <asp:TextBox ID="hfHidden" runat="server" Style="display: none" Text=""></asp:TextBox>
        <cc1:ModalPopupExtender ID="ModalPopupExtender1"
            runat="server"
            TargetControlID="hfHidden"
            PopupControlID="pnlTDSAmts"
            PopupDragHandleControlID="PopupHeader"
            BackgroundCssClass="ModalPopupBG"
            Drag="true">
        </cc1:ModalPopupExtender>

        <asp:Panel ID="pnlTDSAmts" runat="server" Height="97%" Width="800px" Style="display: none; padding: 15px; position: absolute; background-color: white; border-radius: 5px; box-shadow: 7px 7px 5px #888888;">

            <br />
            <div style="width: 100%" runat="server" visible="false" id="divOldscheme">
                <p style="font-size: 15px; background-color: #09ea09"><b>Tax Advice:</b> <b>Old Scheme</b> seems to be beneficial as it will allow you to avail exemptions and deductions from your income sources.</p>
            </div>
            <div style="width: 100%" runat="server" visible="false" id="divNewscheme">
                <p style="font-size: 15px; background-color: #09ea09"><b>Tax Advice:</b> <b>New Scheme</b> seems to be beneficial for the mentioned employee.</p>
            </div>

            <div style="width: 100%">
                <table>

                    <tr>
                        <td style="font-weight: bold">Financial Year:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblPreFinancialYear" Text=""></asp:Label>
                        </td>
                    </tr>

                    <tr>
                        <td style="font-weight: bold">Emp ID:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblPreEmpid" Text=""></asp:Label>
                        </td>
                    </tr>

                    <tr>
                        <td style="font-weight: bold">Emp Name:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblPreEmpname" Text=""></asp:Label>
                        </td>
                    </tr>

                    <tr>
                        <td style="font-weight: bold">PAN No:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblpanno" Text=""></asp:Label>
                        </td>
                    </tr>

                    <tr>
                        <td style="font-weight: bold">Age:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblage" Text=""></asp:Label>
                        </td>
                    </tr>

                </table>


                <table>

                    <asp:GridView ID="GvTDSPreAmounts" runat="server" Width="100%" Style="margin-left: 5px"
                        AutoGenerateColumns="false" CellPadding="5" CellSpacing="3" ForeColor="#333333" GridLines="Both"
                        CssClass="table table-striped table-bordered table-condensed table-hover">
                        <Columns>
                            <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Left" ControlStyle-Font-Bold="true" ItemStyle-Width="15px">
                                <HeaderStyle Width="15px" />
                                <ItemTemplate>
                                    <asp:Label ID="lblHeading" runat="server" Text='<%#Bind("Heading") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Old Regime" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="15px">
                                <HeaderStyle Width="15px" />
                                <ItemTemplate>
                                    <asp:Label ID="lblPostBudgetOld" runat="server" Text='<%#Bind("PostBudgetOld") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="New Regime" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="15px">
                                <HeaderStyle Width="15px" />
                                <ItemTemplate>
                                    <asp:Label ID="lblPostBudgetOld" runat="server" Text='<%#Bind("PostBudgetNew") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                    </asp:GridView>

                </table>


                <div style="margin-top: 10px; float: right">
                    <asp:RadioButton ID="RdboldScheme" Text="Old Scheme" GroupName="Scheme" runat="server" />
                    <asp:RadioButton ID="RdbNewScheme" Text="New Scheme" GroupName="Scheme" runat="server" />

                    <asp:Button ID="btnTDSSave" runat="server" class="button button1" Text="Save"
                        OnClick="btnTDSSave_Click" Style="width: 125px" />

                    <asp:Button ID="btnTDSClose" runat="server" class="button button2" Text="Close"
                        OnClick="btnTDSClose_Click" Style="width: 125px" />

                </div>


            </div>

        </asp:Panel>

    </form>

    <script type="text/javascript">
        Sys.Browser.WebKit = {};
        if (navigator.userAgent.indexOf('WebKit/') > -1) {
            Sys.Browser.agent = Sys.Browser.WebKit;
            Sys.Browser.version = parseFloat(navigator.userAgent.match(/WebKit\/(\d+(\.\d+)?)/)[1]);
            Sys.Browser.name = 'WebKit';
        }

        var prm = Sys.WebForms.PageRequestManager.getInstance();
        if (prm != null) {
            prm.add_endRequest(function () {

                GetEmpid();
                GetEmpName();


            });
        };
    </script>

</body>
</html>
