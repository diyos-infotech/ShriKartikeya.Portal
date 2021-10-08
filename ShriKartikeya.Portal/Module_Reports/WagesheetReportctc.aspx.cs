﻿using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using KLTS.Data;
using System.Collections.Generic;
using System.Globalization;
using System.Collections;
using ShriKartikeya.Portal.DAL;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Text;
using OfficeOpenXml;

namespace ShriKartikeya.Portal.Module_Reports
{
    public partial class WagesheetReportctc : System.Web.UI.Page
    {
        AppConfiguration config = new AppConfiguration();
        GridViewExportUtil gve = new GridViewExportUtil();
        DataTable dt;
        string EmpIDPrefix = "";
        string CmpIDPrefix = "";
        string BranchID = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GetWebConfigdata();
                if (!IsPostBack)
                {
                    if (Session["UserId"] != null && Session["AccessLevel"] != null)
                    {

                    }
                    else
                    {
                        Response.Redirect("login.aspx");
                    }
                    if (this.Master != null)
                    {
                        HtmlControl emplink = (HtmlControl)this.Master.Master.FindControl("ContentPlaceHolder1").FindControl("sli2");
                        if (emplink != null)
                        {
                            emplink.Attributes["class"] = "current";
                        }
                    }
                    LoadBranches();

                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Show alert", "alert('Your Session Expired');", true);
                Response.Redirect("~/Login.aspx");
            }

        }
        protected void GetWebConfigdata()
        {
            EmpIDPrefix = Session["EmpIDPrefix"].ToString();
            CmpIDPrefix = Session["CmpIDPrefix"].ToString();
            BranchID = Session["BranchID"].ToString();
        }
        protected void LoadBranches()
        {
            DataTable dtBranch = GlobalData.Instance.LoadBranchOnUserID(BranchID);

            DataTable DtBranches = GlobalData.Instance.LoadLoginBranch(dtBranch);
            if (DtBranches.Rows.Count > 0)
            {
                ddlBranch.DataValueField = "BranchId";
                ddlBranch.DataTextField = "BranchName";
                ddlBranch.DataSource = DtBranches;
                ddlBranch.DataBind();
            }
            ddlBranch.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-All-", "0"));
        }

        protected void btnsearch_Click(object sender, EventArgs e)
        {
            DataTable dtBranch = GlobalData.Instance.LoadBranchOnUserID(BranchID);
            LblResult.Text = "";
            GVClientsData.DataSource = null;
            GVClientsData.DataBind();
            var SPName = "";
            int options = 0;
            Hashtable HTPaysheet = new Hashtable();
            if (ddloptions.SelectedIndex == 0)
            {
                if (txtmonth.Text.Trim().Length == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showlalert", "alert('Please Select Month');", true);

                    return;
                }

                else
                {
                    string date = string.Empty;

                    if (txtmonth.Text.Trim().Length > 0)
                    {
                        date = DateTime.Parse(txtmonth.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
                    }
                    string monthnew = string.Empty;

                    string month = DateTime.Parse(date).Month.ToString();
                    string Year = DateTime.Parse(date).Year.ToString();

                    string Branch = "%";
                    if (ddlBranch.SelectedIndex > 0)
                    {
                        Branch = ddlBranch.SelectedValue;
                    }
                    if (ddloptions.SelectedIndex == 0)
                    {
                        monthnew = month + Year.Substring(2, 2);
                        SPName = "GetClientList";
                        options = ddloptions.SelectedIndex;
                        HTPaysheet.Add("@month", monthnew);
                        HTPaysheet.Add("@Options", options);
                        HTPaysheet.Add("@CmpIDPrefix", CmpIDPrefix);
                        HTPaysheet.Add("@BranchID", dtBranch);
                        HTPaysheet.Add("@Branch", Branch);
                        HTPaysheet.Add("@Type", ddlBranch.SelectedIndex);

                    }

                }
            }

            else
            {
                if (txtfrom.Text.Trim().Length == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showlalert", "alert('Please Select From Month');", true);

                    return;
                }

                if (txtto.Text.Trim().Length == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showlalert", "alert('Please Select To Month');", true);

                    return;
                }

                else
                {
                    string Fromdate = string.Empty;
                    string ToMonth = string.Empty;

                    if (txtfrom.Text.Trim().Length > 0)
                    {
                        Fromdate = DateTime.Parse(txtfrom.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
                    }
                    if (txtto.Text.Trim().Length > 0)
                    {
                        ToMonth = DateTime.Parse(txtto.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
                    }
                    string Branch = "%";
                    if (ddlBranch.SelectedIndex > 0)
                    {
                        Branch = ddlBranch.SelectedValue;
                    }
                    SPName = "GetClientList";
                    options = ddloptions.SelectedIndex;
                    HTPaysheet.Add("@FromMonth", Fromdate);
                    HTPaysheet.Add("@TOMonth", ToMonth);
                    HTPaysheet.Add("@Options", options);
                    HTPaysheet.Add("@CmpIDPrefix", CmpIDPrefix);
                    HTPaysheet.Add("@BranchID", dtBranch);
                    HTPaysheet.Add("@Branch", Branch);
                    HTPaysheet.Add("@Type", ddlBranch.SelectedIndex);
                }


            }
            DataTable dt = config.ExecuteAdaptorAsyncWithParams(SPName, HTPaysheet).Result;
            if (dt.Rows.Count > 0)
            {
                GVClientsData.DataSource = dt;
                GVClientsData.DataBind();
                btngetdata.Visible = true;
                btndownloadpdf.Visible = true;
                btn_Submit.Visible = false;

            }

            //DisplayData();
        }

        protected void Bindata(string Sqlqry)
        {
            DataTable dt = config.ExecuteAdaptorAsyncWithQueryParams(Sqlqry).Result;
            if (dt.Rows.Count > 0)
            {
                GVListEmployees1.DataSource = dt;
                GVListEmployees1.DataBind();
            }
            else
            {
                GVListEmployees1.DataSource = null;
                GVListEmployees1.DataBind();
                ScriptManager.RegisterStartupScript(this, GetType(), "showlalert", "alert('There Is No Salary Details For The Selected client');", true);

            }
        }

        protected void ClearData()
        {
            LblResult.Text = "";
            GVListEmployees1.DataSource = null;
            GVListEmployees1.DataBind();
            GVClientsData.DataSource = null;
            GVClientsData.DataBind();
            // lbtn_Export.Visible = false;
        }

        protected void lbtn_Export_Click(object sender, EventArgs e)
        {
            try
            {


                // gve.ExportGrid("WageSheetReport" + ".xls", hidGridView);

                string branchesi = "select * from companyinfo";
                string companyname = "";
                string line1 = "";
                string line2 = "";
                DataTable dtbranchesi = config.ExecuteReaderWithQueryAsync(branchesi).Result;
                string monthnew = string.Empty;

                if (ddloptions.SelectedIndex == 0)
                {
                    string date = string.Empty;


                    //if (txtmonth.Text.Trim().Length > 0)
                    //{
                    //    date = DateTime.Parse(txtmonth.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
                    //}

                    //string month = DateTime.Parse(date).Month.ToString();
                    //string Year = DateTime.Parse(date).Year.ToString();
                    //monthnew = month + Year.Substring(2, 2);
                }
                string sqlqry = string.Empty;

                //var list = new List<string>();
                var list = new List<Tuple<int, string>>();
                #region Declaring TotalVariables
                float totalActualamount = 0;
                float totalctcamount = 0;
                float totalDuties = 0;
                float totalOts = 0;
                float totalwo = 0;
                float totalnhs = 0;
                float totalnpots = 0;
                float totaltempgross = 0;
                float totalBasic = 0;
                float totalDA = 0;
                float totalHRA = 0;
                float totalCCA = 0;
                float totalConveyance = 0;
                float totalWA = 0;
                float totalOA = 0;
                float totalGrass = 0;
                float totalOTAmount = 0;
                float totalPF = 0;
                float totalESI = 0;
                float totalProfTax = 0;
                float totalSalAdv = 0;
                float totalUniformDed = 0;
                float totalAdvDed = 0;
                float totalWCDed = 0;
                float totalCanteenAdv = 0;
                float totalLeaveEncashAmt = 0;
                float totalGratuity = 0;
                float totalBonus = 0;
                float totalnfhs = 0;
                float totalDed = 0;
                float totalOtherDed = 0;
                float totalIncentivs = 0;
                float totalWoAmt = 0;
                float totalNhsAmt = 0;
                float totalNpotsAmt = 0;
                float totalPenalty = 0;
                float totalRC = 0;
                float totalCS = 0;
                float totalOWF = 0;
                float totalSecDepDed = 0;
                float totalloanded = 0;
                float totalGenDed = 0;
                float totalctc = 0;

                float totalAttBonus = 0;
                float totalTravelAllw = 0;
                float totalNightShiftAllw = 0;
                float totalFoodAllowance = 0;
                float totalmedicalallowance = 0;
                float totalUniformAllw = 0;

                float totalAdv4Ded = 0;
                float totalNightRoundDed = 0;
                float totalManpowerMobDed = 0;
                float totalMobileusageDed = 0;
                float totalMediClaimDed = 0;
                float totalCrisisDed = 0;
                float totalMobInstDed = 0;
                float totalTDSDed = 0;


                float totalSpecialAllowance = 0;
                float totalMobileAllowance = 0;
                float totalNPCl25Per = 0;
                float totalTransport6Per = 0;
                float totalTransport = 0;

                float totalRentDed = 0;
                float totalMedicalDed = 0;
                float totalMLWFDed = 0;
                float totalFoodDed = 0;
                float totalAddlAmount = 0;


                float totalElectricityDed = 0;
                float totalTransportDed = 0;
                float totalDccDed = 0;

                float totalLeaveDed = 0;
                float totalLicenseDed = 0;
                float totalpfempr = 0;
                float totalesiempr = 0;
                float totalDiv = 0;
                float totalArea = 0;
                float totalTelephoneBillDed = 0;
                float totalAdvBonus = 0;
                #endregion Declaring TotalVariables

                // var monthlist = new List<string>();
                var options = 0;
                if (GVClientsData.Rows.Count > 0)
                {
                    for (int i = 0; i < GVClientsData.Rows.Count; i++)
                    {
                        CheckBox chkclientid = GVClientsData.Rows[i].FindControl("chkindividual") as CheckBox;
                        Label lblclientid = GVClientsData.Rows[i].FindControl("lblclientid") as Label;
                        Label lblmonth = GVClientsData.Rows[i].FindControl("lblmonth") as Label;

                        if (chkclientid.Checked == true)
                        {
                            list.Add(Tuple.Create(Convert.ToInt32(lblmonth.Text), lblclientid.Text));
                        }

                    }
                }

                // string Clientids = string.Join(",", list.ToArray());


                DataTable dtClientList = new DataTable();
                DataTable dtmonth = new DataTable();
                dtClientList.Columns.Add("Clientid");
                dtmonth.Columns.Add("month");
                if (list.Count != 0)
                {
                    //foreach (var s in list)
                    //{
                    //    DataRow row = dtClientList.NewRow();
                    //    row["Clientid"] = s;
                    //    dtClientList.Rows.Add(row);
                    //}

                    foreach (Tuple<int, string> tuple in list)
                    {
                        DataRow row = dtClientList.NewRow();
                        DataRow row1 = dtmonth.NewRow();
                        row["Clientid"] = tuple.Item2;
                        row1["month"] = tuple.Item1;
                        dtClientList.Rows.Add(row["Clientid"]);
                        dtmonth.Rows.Add(row1["month"]);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "show alert", "alert('Please select Client IDs');", true);
                }

                var SPName = "";
                Hashtable HTPaysheet = new Hashtable();
                int countearnings = 0;
                int countdedutions = 0;
                int countot = 0;
                int countpfempr = 0;
                int countduties = 0;
                int countesiempr = 0;
                int countctc = 0;
                int countfixed = 0;
                int countAdvBonus = 0;
                int othrsount = 1;
                int countNetpay = 1;

                {
                    string Fromdate = string.Empty;
                    string ToMonth = string.Empty;

                    if (txtfrom.Text.Trim().Length > 0)
                    {
                        Fromdate = DateTime.Parse(txtfrom.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
                    }
                    if (txtto.Text.Trim().Length > 0)
                    {
                        ToMonth = DateTime.Parse(txtto.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
                    }
                    SPName = "[dbo].[EMPWAGESHEETREPORTFromToctcWise]";
                    options = ddloptions.SelectedIndex;
                    HTPaysheet.Add("@ClientIDList", dtClientList);
                    HTPaysheet.Add("@MonthList", dtmonth);
                }


                DataTable dt = config.ExecuteAdaptorAsyncWithParams(SPName, HTPaysheet).Result;

                if (dt.Rows.Count > 0)
                {
                    GVListEmployees1.DataSource = dt;
                    GVListEmployees1.DataBind();
                    lbtn_Export.Visible = true;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        // if (dt.Rows[i]["Monthname"].ToString() == "Sub Total")
                        {
                            float actAmount = 0;

                            if (actAmount >= 0)
                            {
                                #region

                                // Total Calculation 


                                //26-04-2019
                                string strCdBasic = dt.Rows[i]["CdBasic"].ToString();
                                if (strCdBasic.Trim().Length > 0)
                                {
                                    totalCdBasic += Convert.ToSingle(strCdBasic);
                                }
                                string strCdDA = dt.Rows[i]["CdDA"].ToString();
                                if (strCdDA.Trim().Length > 0)
                                {
                                    totalCdDA += Convert.ToSingle(strCdDA);
                                }
                                string strCdHRA = dt.Rows[i]["CdHRA"].ToString();
                                if (strCdHRA.Trim().Length > 0)
                                {
                                    totalCdHRA += Convert.ToSingle(strCdHRA);
                                }
                                string strCdCCA = dt.Rows[i]["CdCCA"].ToString();
                                if (strCdCCA.Trim().Length > 0)
                                {
                                    totalCdCCA += Convert.ToSingle(strCdCCA);
                                }
                                string strCdConv = dt.Rows[i]["CdConv"].ToString();
                                if (strCdConv.Trim().Length > 0)
                                {
                                    totalCdConveyance += Convert.ToSingle(strCdConv);
                                }

                                string strCdWA = dt.Rows[i]["CdWA"].ToString();
                                if (strCdWA.Trim().Length > 0)
                                {
                                    totalCdWA += Convert.ToSingle(strCdWA);
                                }
                                string strCdNfhs = dt.Rows[i]["CdNfhs"].ToString();
                                if (strCdNfhs.Trim().Length > 0)
                                {
                                    totalCdNfhs += Convert.ToSingle(strCdNfhs);
                                }
                                string strCdRC = dt.Rows[i]["CdRC"].ToString();
                                if (strCdRC.Trim().Length > 0)
                                {
                                    totalCdrc += Convert.ToSingle(strCdRC);
                                }
                                string strCdCS = dt.Rows[i]["CdCS"].ToString();
                                if (strCdCS.Trim().Length > 0)
                                {
                                    totalCdcs += Convert.ToSingle(strCdCS);
                                }
                                string strCdAddlAmount = dt.Rows[i]["CdAddlAmount"].ToString();
                                if (strCdAddlAmount.Trim().Length > 0)
                                {
                                    totalCdAddlAmount += Convert.ToSingle(strCdAddlAmount);
                                }
                                string strCdFoodAllw = dt.Rows[i]["CdFoodAllw"].ToString();
                                if (strCdFoodAllw.Trim().Length > 0)
                                {
                                    totalCdFoodAllw += Convert.ToSingle(strCdFoodAllw);
                                }
                                string strCdWOAmt = dt.Rows[i]["CdWOAmt"].ToString();
                                if (strCdWOAmt.Trim().Length > 0)
                                {
                                    totalCdWOAmt += Convert.ToSingle(strCdWOAmt);
                                }
                                string strCdNHsAmt = dt.Rows[i]["CdNHsAmt"].ToString();
                                if (strCdNHsAmt.Trim().Length > 0)
                                {
                                    totalCdNHsAmt += Convert.ToSingle(strCdNHsAmt);
                                }
                                string strCdMedicalReimbursement = dt.Rows[i]["CdMedicalReimbursement"].ToString();
                                if (strCdMedicalReimbursement.Trim().Length > 0)
                                {
                                    totalCdMedicalReimbursement += Convert.ToSingle(strCdMedicalReimbursement);
                                }
                                string strCdSpecialAllW = dt.Rows[i]["CdSpecialAllW"].ToString();
                                if (strCdSpecialAllW.Trim().Length > 0)
                                {
                                    totalCdSpecialAllW += Convert.ToSingle(strCdSpecialAllW);
                                }
                                string strCdTravelAllw = dt.Rows[i]["CdTravelAllw"].ToString();
                                if (strCdTravelAllw.Trim().Length > 0)
                                {
                                    totalCdTravelAllw += Convert.ToSingle(strCdTravelAllw);
                                }
                                string strCdMobileAllw = dt.Rows[i]["CdMobileAllw"].ToString();
                                if (strCdMobileAllw.Trim().Length > 0)
                                {
                                    totalCdMobileAllw += Convert.ToSingle(strCdMobileAllw);
                                }
                                string strCdPerformanceAllw = dt.Rows[i]["CdPerformanceAllw"].ToString();
                                if (strCdPerformanceAllw.Trim().Length > 0)
                                {
                                    totalCdPerformanceAllw += Convert.ToSingle(strCdPerformanceAllw);
                                }
                                string strCdLW = dt.Rows[i]["CdLW"].ToString();
                                if (strCdLW.Trim().Length > 0)
                                {
                                    totalCdLW += Convert.ToSingle(strCdLW);
                                }
                                string strCdNPOTsAmt = dt.Rows[i]["CdNPOTsAmt"].ToString();
                                if (strCdNPOTsAmt.Trim().Length > 0)
                                {
                                    totalCdNPOTsAmt += Convert.ToSingle(strCdNPOTsAmt);
                                }
                                string strCdIncentivs = dt.Rows[i]["CdIncentivs"].ToString();
                                if (strCdIncentivs.Trim().Length > 0)
                                {
                                    totalCdIncentivs += Convert.ToSingle(strCdIncentivs);
                                }
                                string strCdBonus = dt.Rows[i]["CdBonus"].ToString();
                                if (strCdBonus.Trim().Length > 0)
                                {
                                    totalCdBonus += Convert.ToSingle(strCdBonus);
                                }
                                string strCdGratuity = dt.Rows[i]["CdGratuity"].ToString();
                                if (strCdGratuity.Trim().Length > 0)
                                {
                                    totalCdGratuity += Convert.ToSingle(strCdGratuity);
                                }
                                string strCdOA = dt.Rows[i]["CdOA"].ToString();
                                if (strCdOA.Trim().Length > 0)
                                {
                                    totalCdOA += Convert.ToSingle(strCdOA);
                                }
                                string strCdOTAmt = dt.Rows[i]["CdOTAmt1"].ToString();
                                if (strCdOTAmt.Trim().Length > 0)
                                {
                                    totalcdOtRate += Convert.ToSingle(strCdOTAmt);
                                }
                                string strCdServiceWeightage = dt.Rows[i]["CdServiceWeightage"].ToString();
                                if (strCdServiceWeightage.Trim().Length > 0)
                                {
                                    totalCdServiceWeightage += Convert.ToSingle(strCdServiceWeightage);
                                }
                                string strCdArrears = dt.Rows[i]["CdArrears"].ToString();
                                if (strCdArrears.Trim().Length > 0)
                                {
                                    totalCdArrears += Convert.ToSingle(strCdArrears);
                                }
                                string strCdAttBonus = dt.Rows[i]["CdAttBonus"].ToString();
                                if (strCdAttBonus.Trim().Length > 0)
                                {
                                    totalCdAttBonus += Convert.ToSingle(strCdAttBonus);
                                }
                                string strCdNightAllw = dt.Rows[i]["CdNightAllw"].ToString();
                                if (strCdNightAllw.Trim().Length > 0)
                                {
                                    totalCdNightAllw += Convert.ToSingle(strCdNightAllw);
                                }
                                string strCdEmpty1 = dt.Rows[i]["CdEmpty1"].ToString();
                                if (strCdEmpty1.Trim().Length > 0)
                                {
                                    totalCdEmpty1 += Convert.ToSingle(strCdEmpty1);
                                }
                                string strCdEmpty2 = dt.Rows[i]["CdEmpty2"].ToString();
                                if (strCdEmpty2.Trim().Length > 0)
                                {
                                    totalCdEmpty2 += Convert.ToSingle(strCdEmpty2);
                                }


                                string duties = dt.Rows[i]["NoOfDuties"].ToString();
                                if (duties.Trim().Length > 0)
                                {
                                    totalDuties += Convert.ToSingle(duties);
                                }
                                string ots = dt.Rows[i]["OTs"].ToString();
                                if (ots.Trim().Length > 0)
                                {
                                    totalOts += Convert.ToSingle(ots);
                                }

                                string wos = dt.Rows[i]["wo"].ToString();
                                if (wos.Trim().Length > 0)
                                {
                                    totalwo += Convert.ToSingle(wos);
                                }
                                string nhs = dt.Rows[i]["nhs"].ToString();
                                if (nhs.Trim().Length > 0)
                                {
                                    totalnhs += Convert.ToSingle(nhs);
                                }
                                string npots = dt.Rows[i]["pldays"].ToString();
                                if (npots.Trim().Length > 0)
                                {
                                    totalnpots += Convert.ToSingle(npots);
                                }

                                string strBasic = dt.Rows[i]["Basic"].ToString();
                                if (strBasic.Trim().Length > 0)
                                {
                                    totalBasic += Convert.ToSingle(strBasic);
                                    // countearnings += 1;
                                }
                                string strDA = dt.Rows[i]["DA"].ToString();
                                if (strDA.Trim().Length > 0)
                                {
                                    totalDA += Convert.ToSingle(strDA);

                                }
                                string strhHRA = dt.Rows[i]["HRA"].ToString();
                                if (strhHRA.Trim().Length > 0)
                                {
                                    totalHRA += Convert.ToSingle(strhHRA);

                                }
                                string strCCA = dt.Rows[i]["CCA"].ToString();
                                if (strCCA.Trim().Length > 0)
                                {
                                    totalCCA += Convert.ToSingle(strCCA);

                                }
                                string strConveyance = dt.Rows[i]["Conveyance"].ToString();
                                if (strConveyance.Trim().Length > 0)
                                {
                                    totalConveyance += Convert.ToSingle(strConveyance);

                                }
                                string strWA = dt.Rows[i]["WashAllowance"].ToString();
                                if (strWA.Trim().Length > 0)
                                {
                                    totalWA += Convert.ToSingle(strWA);

                                }
                                string strNfhs = dt.Rows[i]["Nfhs"].ToString();
                                if (strNfhs.Trim().Length > 0)
                                {
                                    totalnfhs += Convert.ToSingle(strNfhs);

                                }
                                string strRC = dt.Rows[i]["RC"].ToString();
                                if (strRC.Trim().Length > 0)
                                {
                                    totalRC += Convert.ToSingle(strRC);

                                }

                                string strCS = dt.Rows[i]["CS"].ToString();
                                if (strCS.Trim().Length > 0)
                                {
                                    totalCS += Convert.ToSingle(strCS);
                                    //countearnings += 1;
                                }
                                string strAddlAmount = dt.Rows[i]["AddlAmount"].ToString();
                                if (strAddlAmount.Trim().Length > 0)
                                {
                                    totalAddlAmount += Convert.ToSingle(strAddlAmount);

                                }
                                string strFoodAllowance = dt.Rows[i]["FoodAllowance"].ToString();
                                if (strFoodAllowance.Trim().Length > 0)
                                {
                                    totalFoodAllowance += Convert.ToSingle(strFoodAllowance);

                                }
                                string strWoAmt = dt.Rows[i]["WOAmt"].ToString();
                                if (strWoAmt.Trim().Length > 0)
                                {
                                    totalWoAmt += Convert.ToSingle(strWoAmt);

                                }

                                string strNhsAmt = dt.Rows[i]["Nhsamt"].ToString();
                                if (strNhsAmt.Trim().Length > 0)
                                {
                                    totalNhsAmt += Convert.ToSingle(strNhsAmt);

                                }
                                string strmedicalallowance = dt.Rows[i]["medicalallowance"].ToString();
                                if (strmedicalallowance.Trim().Length > 0)
                                {
                                    totalmedicalallowance += Convert.ToSingle(strmedicalallowance);

                                }
                                string strSpecialAllowance = dt.Rows[i]["SpecialAllowance"].ToString();
                                if (strSpecialAllowance.Trim().Length > 0)
                                {
                                    totalSpecialAllowance += Convert.ToSingle(strSpecialAllowance);

                                }
                                string strTravelAllw = dt.Rows[i]["TravelAllw"].ToString();
                                if (strTravelAllw.Trim().Length > 0)
                                {
                                    totalTravelAllw += Convert.ToSingle(strTravelAllw);

                                }
                                string strMobileAllowance = dt.Rows[i]["MobileAllw"].ToString();
                                if (strMobileAllowance.Trim().Length > 0)
                                {
                                    totalMobileAllowance += Convert.ToSingle(strMobileAllowance);

                                }

                                string strPerformanceAllw = dt.Rows[i]["PerformanceAllw"].ToString();
                                if (strPerformanceAllw.Trim().Length > 0)
                                {
                                    totalPerformanceAllw += Convert.ToSingle(strPerformanceAllw);

                                }

                                string strLeaveEncashAmt = dt.Rows[i]["LeaveEncashAmt"].ToString();
                                if (strCCA.Trim().Length > 0)
                                {
                                    totalLeaveEncashAmt += Convert.ToSingle(strLeaveEncashAmt);

                                }
                                string strNpotsAmt = dt.Rows[i]["Npotsamt"].ToString();
                                if (strNpotsAmt.Trim().Length > 0)
                                {
                                    totalNpotsAmt += Convert.ToSingle(strNpotsAmt);

                                }
                                string strIncentivs = dt.Rows[i]["Incentivs"].ToString();
                                if (strIncentivs.Trim().Length > 0)
                                {
                                    totalIncentivs += Convert.ToSingle(strIncentivs);

                                }
                                string strBonus = dt.Rows[i]["Bonus"].ToString();
                                if (strBonus.Trim().Length > 0)
                                {
                                    totalBonus += Convert.ToSingle(strBonus);

                                }
                                string strGratuity = dt.Rows[i]["Gratuity"].ToString();
                                if (strGratuity.Trim().Length > 0)
                                {
                                    totalGratuity += Convert.ToSingle(strGratuity);

                                }

                                string strOA = dt.Rows[i]["OtherAllowance"].ToString();
                                if (strOA.Trim().Length > 0)
                                {
                                    totalOA += Convert.ToSingle(strOA);

                                }
                                //string strOTAmount = dt.Rows[i]["OTAmt"].ToString();
                                //if (strOTAmount.Trim().Length > 0)
                                //{
                                //    totalOTAmount += Convert.ToSingle(strOTAmount);
                                //}

                                string strNPCl25Per = dt.Rows[i]["OTHrs"].ToString();
                                if (strNPCl25Per.Trim().Length > 0)
                                {
                                    totalNPCl25Per += Convert.ToSingle(strNPCl25Per);
                                }
                                string strTransport6Per = dt.Rows[i]["ServiceWeightage"].ToString();
                                if (strTransport6Per.Trim().Length > 0)
                                {
                                    totalTransport6Per += Convert.ToSingle(strTransport6Per);

                                }
                                string strArrears = dt.Rows[i]["Arrears"].ToString();
                                if (strArrears.Trim().Length > 0)
                                {
                                    totalArrears += Convert.ToSingle(strArrears);

                                }
                                string strAttBonus = dt.Rows[i]["AttBonus"].ToString();
                                if (strAttBonus.Trim().Length > 0)
                                {
                                    totalAttBonus += Convert.ToSingle(strAttBonus);
                                    // countearnings += 1;
                                }
                                string strNightShiftAllw = dt.Rows[i]["NightAllw"].ToString();
                                if (strNightShiftAllw.Trim().Length > 0)
                                {
                                    totalNightShiftAllw += Convert.ToSingle(strNightShiftAllw);

                                }

                                string strEmpty = dt.Rows[i]["Empty"].ToString();
                                if (strEmpty.Trim().Length > 0)
                                {
                                    totalEmpty1 += Convert.ToSingle(strEmpty);
                                }
                                string strEmpty1 = dt.Rows[i]["Empty"].ToString();
                                if (strEmpty1.Trim().Length > 0)
                                {
                                    totalEmpty2 += Convert.ToSingle(strEmpty1);
                                }
                                string strEmpty2 = dt.Rows[i]["Empty"].ToString();
                                if (strEmpty2.Trim().Length > 0)
                                {
                                    totalEmpty3 += Convert.ToSingle(strEmpty2);
                                }
                                string strGross = dt.Rows[i]["Gross"].ToString();
                                if (strGross.Trim().Length > 0)
                                {
                                    totalGrass += Convert.ToSingle(strGross);

                                }
                                string strPF = dt.Rows[i]["PF"].ToString();
                                if (strPF.Trim().Length > 0)
                                {
                                    totalPF += Convert.ToSingle(strPF);

                                }
                                string strESI = dt.Rows[i]["ESI"].ToString();
                                if (strESI.Trim().Length > 0)
                                {
                                    totalESI += Convert.ToSingle(strESI);

                                }
                                string strProfTax = dt.Rows[i]["ProfTax"].ToString();
                                if (strProfTax.Trim().Length > 0)
                                {
                                    totalProfTax += Convert.ToSingle(strProfTax);

                                }

                                string strSalAdv = dt.Rows[i]["SalAdvDed"].ToString();
                                if (strSalAdv.Trim().Length > 0)
                                {
                                    totalSalAdv += Convert.ToSingle(strSalAdv);

                                }

                                string strAdvDed = dt.Rows[i]["ADVDed"].ToString();
                                if (strAdvDed.Trim().Length > 0)
                                {
                                    totalAdvDed += Convert.ToSingle(strAdvDed);

                                }

                                string strWCDed = dt.Rows[i]["WCDed"].ToString();
                                if (strWCDed.Trim().Length > 0)
                                {
                                    totalWCDed += Convert.ToSingle(strWCDed);

                                }

                                string strUniformDed = dt.Rows[i]["UniformDed"].ToString();
                                if (strUniformDed.Trim().Length > 0)
                                {
                                    totalUniformDed += Convert.ToSingle(strUniformDed);

                                }
                                string strOtherDed = dt.Rows[i]["OtherDed"].ToString();
                                if (strOtherDed.Trim().Length > 0)
                                {
                                    totalOtherDed += Convert.ToSingle(strOtherDed);

                                }

                                string strLoanDed = dt.Rows[i]["LoanDed"].ToString();
                                if (strLoanDed.Trim().Length > 0)
                                {
                                    totalloanded += Convert.ToSingle(strLoanDed);
                                }
                                string strCanteenAdv = dt.Rows[i]["CanteenAdv"].ToString();
                                if (strCanteenAdv.Trim().Length > 0)
                                {
                                    totalCanteenAdv += Convert.ToSingle(strCanteenAdv);

                                }
                                string strSecDep = dt.Rows[i]["SecurityDepDed"].ToString();
                                if (strSecDep.Trim().Length > 0)
                                {
                                    totalSecDepDed += Convert.ToSingle(strSecDep);
                                }


                                string strGeneralDed = dt.Rows[i]["GeneralDed"].ToString();
                                if (strGeneralDed.Trim().Length > 0)
                                {
                                    totalGenDed += Convert.ToSingle(strGeneralDed);
                                }
                                string strOWF = dt.Rows[i]["OWF"].ToString();
                                if (strOWF.Trim().Length > 0)
                                {
                                    totalOWF += Convert.ToSingle(strOWF);
                                }
                                string strPenalty = dt.Rows[i]["Penalty"].ToString();
                                if (strPenalty.Trim().Length > 0)
                                {
                                    totalPenalty += Convert.ToSingle(strPenalty);
                                }
                                string strRentDed = dt.Rows[i]["atmDed"].ToString();
                                if (strRentDed.Trim().Length > 0)
                                {
                                    totalRentDed += Convert.ToSingle(strRentDed);

                                }
                                string strMedicalDed = dt.Rows[i]["MedicalDed"].ToString();
                                if (strMedicalDed.Trim().Length > 0)
                                {
                                    totalMedicalDed += Convert.ToSingle(strMedicalDed);

                                }
                                string strMLWFDed = dt.Rows[i]["MLWFDed"].ToString();
                                if (strMLWFDed.Trim().Length > 0)
                                {
                                    totalMLWFDed += Convert.ToSingle(strMLWFDed);

                                }
                                string strFoodDed = dt.Rows[i]["FoodDed"].ToString();
                                if (strFoodDed.Trim().Length > 0)
                                {
                                    totalFoodDed += Convert.ToSingle(strFoodDed);

                                }

                                string strElectricityDed = dt.Rows[i]["IDCardDed"].ToString();
                                if (strElectricityDed.Trim().Length > 0)
                                {
                                    totalElectricityDed += Convert.ToSingle(strElectricityDed);

                                }


                                string strTransportDed = dt.Rows[i]["RentDed1"].ToString();
                                if (strTransportDed.Trim().Length > 0)
                                {
                                    totalTransportDed += Convert.ToSingle(strTransportDed);

                                }
                                string strDccDed = dt.Rows[i]["Finesded1"].ToString();
                                if (strDccDed.Trim().Length > 0)
                                {
                                    totalDccDed += Convert.ToSingle(strDccDed);

                                }
                                string strLeaveDed = dt.Rows[i]["PVCDed"].ToString();
                                if (strLeaveDed.Trim().Length > 0)
                                {
                                    totalLeaveDed += Convert.ToSingle(strLeaveDed);

                                }
                                string strLicenseDed = dt.Rows[i]["LicenseDed"].ToString();
                                if (strLicenseDed.Trim().Length > 0)
                                {
                                    totalLicenseDed += Convert.ToSingle(strLicenseDed);

                                }

                                //
                                string strAdv4Ded = dt.Rows[i]["Adv4Ded"].ToString();
                                if (strAdv4Ded.Trim().Length > 0)
                                {
                                    totalAdv4Ded += Convert.ToSingle(strAdv4Ded);

                                }
                                string strNightRoundDed = dt.Rows[i]["Extra"].ToString();
                                if (strNightRoundDed.Trim().Length > 0)
                                {
                                    totalNightRoundDed += Convert.ToSingle(strNightRoundDed);

                                }

                                string strManpowerMobDed = dt.Rows[i]["ManpowerMobDed"].ToString();
                                if (strManpowerMobDed.Trim().Length > 0)
                                {
                                    totalManpowerMobDed += Convert.ToSingle(strManpowerMobDed);

                                }

                                string strMobileusageDed = dt.Rows[i]["MobileusageDed"].ToString();
                                if (strMobileusageDed.Trim().Length > 0)
                                {
                                    totalMobileusageDed += Convert.ToSingle(strMobileusageDed);

                                }

                                string strMediClaimDed = dt.Rows[i]["MediClaimDed"].ToString();
                                if (strMediClaimDed.Trim().Length > 0)
                                {
                                    totalMediClaimDed += Convert.ToSingle(strMediClaimDed);

                                }

                                string strCrisisDed = dt.Rows[i]["CrisisDed"].ToString();
                                if (strCrisisDed.Trim().Length > 0)
                                {
                                    totalCrisisDed += Convert.ToSingle(strCrisisDed);

                                }
                                string strTelephoneBillDed = dt.Rows[i]["TelephoneBillDed"].ToString();
                                if (strTelephoneBillDed.Trim().Length > 0)
                                {
                                    totalTelephoneBillDed += Convert.ToSingle(strTelephoneBillDed);
                                }
                                string strDed = dt.Rows[i]["TotalDeductions"].ToString();
                                if (strDed.Trim().Length > 0)
                                {
                                    totalDed += Convert.ToSingle(strDed);
                                }
                                string actualAmount = dt.Rows[i]["ActualAmount"].ToString();
                                if (actualAmount.Trim().Length > 0)
                                {
                                    totalActualamount += Convert.ToSingle(actualAmount);
                                }

                                string strfixedADDL4HR = dt.Rows[i]["fixedADDL4HR"].ToString();
                                if (strfixedADDL4HR.Trim().Length > 0)
                                {
                                    totalfixedADDL4HR += Convert.ToSingle(strfixedADDL4HR);

                                }

                                string strfixedQTRALLOW = dt.Rows[i]["fixedQTRALLOW"].ToString();
                                if (strfixedQTRALLOW.Trim().Length > 0)
                                {
                                    totalfixedQTRALLOW += Convert.ToSingle(strfixedQTRALLOW);

                                }

                                string strfixedRELALLOW = dt.Rows[i]["fixedRELALLOW"].ToString();
                                if (strfixedRELALLOW.Trim().Length > 0)
                                {
                                    totalfixedRELALLOW += Convert.ToSingle(strfixedRELALLOW);

                                }

                                string strfixedSITEALLOW = dt.Rows[i]["fixedSITEALLOW"].ToString();
                                if (strfixedSITEALLOW.Trim().Length > 0)
                                {
                                    totalfixedSITEALLOW += Convert.ToSingle(strfixedSITEALLOW);

                                }

                                string strfixedGunAllw = dt.Rows[i]["fixedGunAllw"].ToString();
                                if (strfixedGunAllw.Trim().Length > 0)
                                {
                                    totalfixedGunAllw += Convert.ToSingle(strfixedGunAllw);

                                }

                                string strfixedFireAllw = dt.Rows[i]["fixedFireAllw"].ToString();
                                if (strfixedFireAllw.Trim().Length > 0)
                                {
                                    totalfixedFireAllw += Convert.ToSingle(strfixedFireAllw);

                                }

                                string strfixedTelephoneAllw = dt.Rows[i]["fixedTelephoneAllw"].ToString();
                                if (strfixedTelephoneAllw.Trim().Length > 0)
                                {
                                    totalfixedTelephoneAllw += Convert.ToSingle(strfixedTelephoneAllw);

                                }

                                string strfixedReimbursement = dt.Rows[i]["fixedReimbursement"].ToString();
                                if (strfixedReimbursement.Trim().Length > 0)
                                {
                                    totalfixedReimbursement += Convert.ToSingle(strfixedReimbursement);

                                }

                                string strfixedHardshipAllw = dt.Rows[i]["fixedHardshipAllw"].ToString();
                                if (strfixedHardshipAllw.Trim().Length > 0)
                                {
                                    totalfixedHardshipAllw += Convert.ToSingle(strfixedHardshipAllw);

                                }

                                string strfixedPaidHolidayAllw = dt.Rows[i]["fixedPaidHolidayAllw"].ToString();
                                if (strfixedPaidHolidayAllw.Trim().Length > 0)
                                {
                                    totalfixedPaidHolidayAllw += Convert.ToSingle(strfixedPaidHolidayAllw);

                                }

                                string strADDL4HR = dt.Rows[i]["ADDL4HR"].ToString();
                                if (strADDL4HR.Trim().Length > 0)
                                {
                                    totalADDL4HR += Convert.ToSingle(strADDL4HR);

                                }

                                string strQTRALLOW = dt.Rows[i]["QTRALLOW"].ToString();
                                if (strQTRALLOW.Trim().Length > 0)
                                {
                                    totalQTRALLOW += Convert.ToSingle(strQTRALLOW);

                                }

                                string strRELALLOW = dt.Rows[i]["RELALLOW"].ToString();
                                if (strRELALLOW.Trim().Length > 0)
                                {
                                    totalRELALLOW += Convert.ToSingle(strRELALLOW);

                                }

                                string strSITEALLOW = dt.Rows[i]["SITEALLOW"].ToString();
                                if (strSITEALLOW.Trim().Length > 0)
                                {
                                    totalSITEALLOW += Convert.ToSingle(strSITEALLOW);

                                }

                                string strGunAllw = dt.Rows[i]["GunAllw"].ToString();
                                if (strGunAllw.Trim().Length > 0)
                                {
                                    totalGunAllw += Convert.ToSingle(strGunAllw);

                                }

                                string strFireAllw = dt.Rows[i]["FireAllw"].ToString();
                                if (strFireAllw.Trim().Length > 0)
                                {
                                    totalFireAllw += Convert.ToSingle(strFireAllw);

                                }

                                string strTelephoneAllw = dt.Rows[i]["TelephoneAllw"].ToString();
                                if (strTelephoneAllw.Trim().Length > 0)
                                {
                                    totalTelephoneAllw += Convert.ToSingle(strTelephoneAllw);

                                }

                                string strReimbursement = dt.Rows[i]["Reimbursement"].ToString();
                                if (strReimbursement.Trim().Length > 0)
                                {
                                    totalReimbursement += Convert.ToSingle(strReimbursement);

                                }

                                string strHardshipAllw = dt.Rows[i]["HardshipAllw"].ToString();
                                if (strHardshipAllw.Trim().Length > 0)
                                {
                                    totalHardshipAllw += Convert.ToSingle(strHardshipAllw);

                                }

                                string strPaidHolidayAllw = dt.Rows[i]["PaidHolidayAllw"].ToString();
                                if (strPaidHolidayAllw.Trim().Length > 0)
                                {
                                    totalPaidHolidayAllw += Convert.ToSingle(strPaidHolidayAllw);

                                }

                                string strAdmincharges = dt.Rows[i]["AdminCharges"].ToString();
                                if (strAdmincharges.Trim().Length > 0)
                                {
                                    totalAdmincharges += Convert.ToSingle(strAdmincharges);

                                }

                                string strAdvBonus = dt.Rows[i]["AdvBonus"].ToString();
                                if (strAdvBonus.Trim().Length > 0)
                                {
                                    totalAdvBonus += Convert.ToSingle(strAdvBonus);

                                }

                                //string strUniformAllw = dt.Rows[i]["UniformAllw"].ToString();
                                //if (strUniformAllw.Trim().Length > 0)
                                //{
                                //    totalUniformAllw += Convert.ToSingle(strUniformAllw);

                                //}
                                //string strNPCl25Per = dt.Rows[i]["OTHrs"].ToString();
                                //if (strNPCl25Per.Trim().Length > 0)
                                //{
                                //    totalNPCl25Per += Convert.ToSingle(strNPCl25Per);
                                //}

                                //string Div = dt.Rows[i]["DivisionName"].ToString();
                                //if (Div.Trim().Length > 0)
                                //{
                                //    totalDiv += Convert.ToSingle(Div);
                                //}
                                //string Area = dt.Rows[i]["Location"].ToString();
                                //if (Area.Trim().Length > 0)
                                //{
                                //    totalArea += Convert.ToSingle(Area);
                                //}
                                string ntempgross = dt.Rows[i]["tempgross"].ToString();
                                if (ntempgross.Trim().Length > 0)
                                {
                                    totaltempgross += Convert.ToSingle(ntempgross);
                                }
                                //string strcdOtAmt = dt.Rows[i]["cdOTAmt"].ToString();
                                //if (strcdOtAmt.Trim().Length > 0)
                                //{
                                //    totalcdOtAmt += Convert.ToSingle(strcdOtAmt);

                                //}
                                //string strTransport = dt.Rows[i]["OTHrsAmt"].ToString();
                                //if (strTransport.Trim().Length > 0)
                                //{
                                //    totalTransport += Convert.ToSingle(strTransport);
                                //}
                                //string strMobInstDed = dt.Rows[i]["MobInstDed"].ToString();
                                //if (strMobInstDed.Trim().Length > 0)
                                //{
                                //    totalMobInstDed += Convert.ToSingle(strMobInstDed);

                                //}

                                //string strTDSDed = dt.Rows[i]["TDSDed"].ToString();
                                //if (strTDSDed.Trim().Length > 0)
                                //{
                                //    totalTDSDed += Convert.ToSingle(strTDSDed);

                                //}
                                //string strNightAllw = dt.Rows[i]["NightAllw"].ToString();
                                //if (strNightAllw.Trim().Length > 0)
                                //{
                                //    totalNightAllw += Convert.ToSingle(strNightAllw);

                                //}
                                //string strtotalwed = dt.Rows[i]["WCDed"].ToString();
                                //if (strtotalwed.Trim().Length > 0)
                                //{
                                //    totalwed += Convert.ToSingle(strtotalwed);

                                //}

                                ////
                                string stOTAmt = dt.Rows[i]["OTAmt"].ToString();
                                if (stOTAmt.Trim().Length > 0)
                                {
                                    totalOTAmt += Convert.ToSingle(stOTAmt);
                                }

                                string stotesi = dt.Rows[i]["otesi"].ToString();
                                if (stotesi.Trim().Length > 0)
                                {
                                    totalotesi += Convert.ToSingle(stotesi);
                                }

                                string stnetot = dt.Rows[i]["netot"].ToString();
                                if (stnetot.Trim().Length > 0)
                                {
                                    totalnetot += Convert.ToSingle(stnetot);
                                }

                                string stTOTALNETPAY = dt.Rows[i]["ActualAmount"].ToString();
                                if (stTOTALNETPAY.Trim().Length > 0)
                                {
                                    totalTOTALNETPAY += Convert.ToSingle(stTOTALNETPAY);
                                }

                                string strEmpty3 = dt.Rows[i]["uniformcharge"].ToString();
                                if (strEmpty3.Trim().Length > 0)
                                {
                                    totaluniformcharge += Convert.ToSingle(strEmpty3);
                                }

                                string strEmpty4 = dt.Rows[i]["Empty"].ToString();
                                if (strEmpty4.Trim().Length > 0)
                                {
                                    totalservicecharge += Convert.ToSingle(strEmpty4);
                                }

                                string strEmpty5 = dt.Rows[i]["Empty"].ToString();
                                if (strEmpty5.Trim().Length > 0)
                                {
                                    totalgst += Convert.ToSingle(strEmpty5);
                                }



                                //string strEmpty8 = dt.Rows[i]["Empty"].ToString();
                                //if (strEmpty8.Trim().Length > 0)
                                //{
                                //    totalothers += Convert.ToSingle(strEmpty8);
                                //}


                                string strEmpty10 = dt.Rows[i]["Empty"].ToString();
                                if (strEmpty10.Trim().Length > 0)
                                {
                                    totalESiDAYS += Convert.ToSingle(strEmpty10);
                                }

                                string strpfempr = dt.Rows[i]["pfempr"].ToString();
                                if (strpfempr.Trim().Length > 0)
                                {
                                    totalpfempr += Convert.ToSingle(strpfempr);
                                }

                                string stresiempr = dt.Rows[i]["esiempr"].ToString();
                                if (stresiempr.Trim().Length > 0)
                                {
                                    totalesiempr += Convert.ToSingle(stresiempr);
                                }

                                string strctc = dt.Rows[i]["CTC"].ToString();
                                if (strctc.Trim().Length > 0)
                                {
                                    totalctc += Convert.ToSingle(strctc);
                                }

                                string stservicecharge = dt.Rows[i]["servicecharge"].ToString();
                                if (stservicecharge.Trim().Length > 0)
                                {
                                    totalservicecharge1 += Convert.ToSingle(stservicecharge);
                                }

                                string stgstper = dt.Rows[i]["gstper"].ToString();
                                if (stgstper.Trim().Length > 0)
                                {
                                    totalgstper += Convert.ToSingle(stgstper);
                                }

                                string stgrandtotal = dt.Rows[i]["grandtotal"].ToString();
                                if (stgrandtotal.Trim().Length > 0)
                                {
                                    totalgrandtotal += Convert.ToSingle(stgrandtotal);
                                }

                                string stAdvances = dt.Rows[i]["Advances"].ToString();
                                if (stAdvances.Trim().Length > 0)
                                {
                                    totalAdvances += Convert.ToSingle(stAdvances);
                                }

                                string stUnifom = dt.Rows[i]["Unifom"].ToString();
                                if (stUnifom.Trim().Length > 0)
                                {
                                    totalUnifom += Convert.ToSingle(stUnifom);
                                }

                                string stOthers = dt.Rows[i]["Others"].ToString();
                                if (stOthers.Trim().Length > 0)
                                {
                                    totalOthers += Convert.ToSingle(stOthers);
                                }

                                string stTotDeductions = dt.Rows[i]["TotDeductions"].ToString();
                                if (stTotDeductions.Trim().Length > 0)
                                {
                                    totalTotDeductions += Convert.ToSingle(stTotDeductions);
                                }

                            }
                            #endregion
                        }
                    }
                    #region TotalCount

                    //Fixed Wages 
                    if (totalCdBasic > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdDA > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdHRA > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdCCA > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdConveyance > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdWA > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdNfhs > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdrc > 0)
                    {
                        countfixed += 1;
                    }
                    //if (totalCdrc > 0)
                    //{
                    //    countfixed += 1;
                    //}
                    if (totalCdcs > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdAddlAmount > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdFoodAllw > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdWOAmt > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdNHsAmt > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdMedicalReimbursement > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdSpecialAllW > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdTravelAllw > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdMobileAllw > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdPerformanceAllw > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdLW > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdNPOTsAmt > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdIncentivs > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdBonus > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdGratuity > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdOA > 0)
                    {
                        countfixed += 1;
                    }

                    if (totalcdOtRate > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdServiceWeightage > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdArrears > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdAttBonus > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalCdNightAllw > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalfixedADDL4HR > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalfixedQTRALLOW > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalfixedRELALLOW > 0)
                    {
                        countfixed += 1;
                    }

                    if (totalfixedSITEALLOW > 0)
                    {
                        countfixed += 1;
                    }

                    if (totalfixedGunAllw > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalfixedFireAllw > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalfixedTelephoneAllw > 0)
                    {
                        countfixed += 1;
                    }

                    if (totalfixedReimbursement > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalfixedHardshipAllw > 0)
                    {
                        countfixed += 1;
                    }
                    if (totalfixedPaidHolidayAllw > 0)
                    {
                        countfixed += 1;
                    }

                    if (totaltempgross > 0)
                    {
                        countfixed += 1;
                    }


                    //Duties
                    if (totalDuties > 0)
                    {
                        countduties += 1;
                    }

                    if (totalOts > 0)
                    {
                        countduties += 1;
                    }


                    if (totalwo > 0)
                    {
                        countduties += 1;
                    }


                    if (totalnhs > 0)
                    {
                        countduties += 1;
                    }


                    if (totalnpots > 0)
                    {
                        countduties += 1;
                    }


                    //Earnings
                    if (totalBasic > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalDA > 0)
                    {
                        countearnings += 1;
                    }

                    if (totalHRA > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalCCA > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalConveyance > 0)
                    {
                        countearnings += 1;
                    }

                    if (totalWA > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalnfhs > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalRC > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalAddlAmount > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalFoodAllowance > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalWoAmt > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalNhsAmt > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalmedicalallowance > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalSpecialAllowance > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalTravelAllw > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalMobileAllowance > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalPerformanceAllw > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalLeaveEncashAmt > 0)
                    {
                        countearnings += 1;
                    }

                    if (totalNpotsAmt > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalIncentivs > 0)
                    {
                        countearnings += 1;
                    }

                    if (totalBonus > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalGratuity > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalOA > 0)
                    {
                        countearnings += 1;
                    }
                    //if (totalOTAmount > 0)
                    //{
                    //    countearnings += 1;
                    //}
                    if (totalServiceWeightage > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalArrears > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalAttBonus > 0)
                    {
                        countearnings += 1;
                    }

                    if (totalNightShiftAllw > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalGrass > 0)
                    {
                        countearnings += 1;
                    }
                    //if (totalNPCl25Per>0)
                    //{
                    //    countearnings += 1;
                    //}

                    if (totalTransport6Per > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalCS > 0)
                    {
                        countearnings += 1;
                    }

                    if (totalUniformAllw > 0)
                    {
                        countearnings += 1;
                    }

                    if (totalTransport > 0)
                    {
                        countearnings += 1;
                    }

                    if (totalADDL4HR > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalQTRALLOW > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalRELALLOW > 0)
                    {
                        countearnings += 1;
                    }

                    if (totalSITEALLOW > 0)
                    {
                        countearnings += 1;
                    }

                    if (totalGunAllw > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalFireAllw > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalTelephoneAllw > 0)
                    {
                        countearnings += 1;
                    }

                    if (totalReimbursement > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalHardshipAllw > 0)
                    {
                        countearnings += 1;
                    }
                    if (totalPaidHolidayAllw > 0)
                    {
                        countearnings += 1;
                    }

                    //Deductions
                    if (totalPF > 0)
                    {
                        countdedutions += 1;
                    }

                    if (totalESI > 0)
                    {
                        countdedutions += 1;
                    }

                    if (totalProfTax > 0)
                    {
                        countdedutions += 1;
                    }


                    if (totalSalAdv > 0)
                    {
                        countdedutions += 1;
                    }

                    if (totalAdvDed > 0)
                    {
                        countdedutions += 1;
                    }


                    if (totalWCDed > 0)
                    {
                        countdedutions += 1;
                    }

                    if (totalUniformDed > 0)
                    {
                        countdedutions += 1;
                    }

                    if (totalOtherDed > 0)
                    {
                        countdedutions += 1;
                    }
                    if (totalloanded > 0)
                    {
                        countdedutions += 1;
                    }
                    if (totalCanteenAdv > 0)
                    {
                        countdedutions += 1;
                    }
                    if (totalSecDepDed > 0)
                    {
                        countdedutions += 1;
                    }
                    if (totalGenDed > 0)
                    {
                        countdedutions += 1;
                    }
                    if (totalPenalty > 0)
                    {
                        countdedutions += 1;
                    }

                    if (totalOWF > 0)
                    {
                        countdedutions += 1;
                    }
                    if (totalRentDed > 0)
                    {
                        countdedutions += 1;
                    }
                    if (totalMedicalDed > 0)
                    {
                        countdedutions += 1;
                    }

                    if (totalMLWFDed > 0)
                    {
                        countdedutions += 1;
                    }
                    if (totalFoodDed > 0)
                    {
                        countdedutions += 1;
                    }
                    if (totalElectricityDed > 0)
                    {
                        countdedutions += 1;
                    }
                    if (totalTransportDed > 0)
                    {
                        countdedutions += 1;
                    }

                    if (totalDed > 0)
                    {
                        countdedutions += 1;
                    }
                    if (totalLeaveDed > 0)
                    {
                        countdedutions += 1;
                    }

                    if (totalLicenseDed > 0)
                    {
                        countdedutions += 1;
                    }
                    if (totalAdv4Ded > 0)
                    {
                        countdedutions += 1;
                    }
                    if (totalNightRoundDed > 0)
                    {
                        countdedutions += 1;
                    }
                    if (totalManpowerMobDed > 0)
                    {
                        countdedutions += 1;
                    }


                    if (totalMobileusageDed > 0)
                    {
                        countdedutions += 1;
                    }
                    if (totalMediClaimDed > 0)
                    {
                        countdedutions += 1;
                    }

                    if (totalCrisisDed > 0)
                    {
                        countdedutions += 1;
                    }
                    if (totalTelephoneBillDed > 0)
                    {
                        countdedutions += 1;
                    }
                    if (totalAdmincharges > 0)
                    {
                        countdedutions += 1;
                    }
                    if (totalDccDed > 0)
                    {
                        countdedutions += 1;
                    }


                    //if (totalMobInstDed > 0)
                    //{
                    //    countdedutions += 1;
                    //}


                    //if (totalTDSDed > 0)
                    //{
                    //    countdedutions += 1;
                    //}
                    if (totalOTAmt > 0)
                    {
                        countpfempr += 1;
                    }
                    if (totalotesi > 0)
                    {
                        countpfempr += 1;
                    }
                    if (totalnetot > 0)
                    {
                        countpfempr += 1;
                    }
                    if (totalpfempr > 0)
                    {
                        countpfempr += 1;
                    }

                    if (totalesiempr > 0)
                    {
                        countpfempr += 1;
                    }

                    if (totalotesi > 0)
                    {
                        countpfempr += 1;
                    }
                    if (totaluniformcharge > 0)
                    {
                        countpfempr += 1;
                    }


                    if (totalctc > 0)
                    {
                        countpfempr += 1;
                    }

                    if (totalservicecharge1 > 0)
                    {
                        countpfempr += 1;
                    }

                    if (totalgstper > 0)
                    {
                        countpfempr += 1;
                    }

                    if (totalgrandtotal > 0)
                    {
                        countpfempr += 1;
                    }

                    if (totalAdvances > 0)
                    {
                        countpfempr += 1;
                    }

                    if (totalUnifom > 0)
                    {
                        countpfempr += 1;
                    }

                    if (totalOthers > 0)
                    {
                        countpfempr += 1;
                    }

                    if (totaltotdeductions > 0)
                    {
                        countpfempr += 1;
                    }

                    if (totalTOTALNETPAY > 0)
                    {
                        countpfempr += 1;
                    }




                    // Fixed Count

                    if (totalAdvBonus > 0)
                    {
                        countAdvBonus = 1;
                    }

                    if (totalNPCl25Per > 0)
                    {
                        othrsount = 1;
                    }

                    //if (totalESiDAYS > 0)
                    //{
                    //    othrsount = 1;
                    //}




                }
                #endregion TotalCount

                if (dtbranchesi.Rows.Count > 0)
                {
                    string date = "";
                    string Year = "";
                    int EmpDetialscount = 9;
                    if (txtmonth.Text.Trim().Length > 0)
                    {
                        date = DateTime.Parse(txtmonth.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
                        Year = DateTime.Parse(date).Year.ToString().Substring(2, 2);
                    }
                    string filename = "PaySheetReport " + ".xls";
                    //int count = GVListEmployees.Columns.Count;
                    int count = countduties + countfixed + countearnings + countdedutions + countNetpay + EmpDetialscount + countAdvBonus;
                    companyname = dtbranchesi.Rows[0]["CompanyName"].ToString();
                    string Form = "FORM XVII";
                    string wages = "REGISTER OF WAGES";
                    string Rule = "[Rule 78(1)(a)(i)]";
                    string ContractorName = companyname + ", Miyapur";
                    string WorkLocation = "Nature and location of work";
                    string Address = "Name and address of establishment in/under which contract is carried on";
                    string PrincipalEmployeer = "Name and address of principal employer";
                    string Wageperiod = "Wage period: Monthly";
                    line2 = "Danube Showroom Wages for the Month of " + GetMonthName() + "-" + Year;
                    //GVUtill.ExportGrid("Netpay-(" + txtmonth.Text + ")" + ".xls", hidGridView);
                    gve.ExportGridForWagesheetctcReport(filename, countduties, countfixed, countearnings, countdedutions, countpfempr, countAdvBonus, countNetpay, EmpDetialscount, Form, wages, Rule, ContractorName, WorkLocation, count, othrsount, line2, hidGridView);

                }
            }
            catch (Exception ex)
            {

            }
        }
        protected void GVListEmployees_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GVListEmployees1.PageIndex = e.NewPageIndex;
        }

        public string GetMonthName()
        {
            string monthname = string.Empty;
            int payMonth = 0;
            DateTimeFormatInfo mfi = new DateTimeFormatInfo();

            if (txtmonth.Text.Trim().Length > 0)
            {
                DateTime date = Convert.ToDateTime(txtmonth.Text, CultureInfo.GetCultureInfo("en-gb"));
                monthname = mfi.GetMonthName(date.Month).ToString();
            }
            //payMonth = GetMonth(monthname);

            return monthname;

        }

        public int GetMonthBasedOnSelectionDateorMonth()
        {

            var testDate = 0;
            string EnteredDate = "";

            #region Validation

            if (txtmonth.Text.Trim().Length > 0)
            {

                try
                {

                    testDate = GlobalData.Instance.CheckEnteredDate(txtmonth.Text);
                    if (testDate > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "show alert", "alert('You Are Entered Invalid  DATE.Date Format Should be [DD/MM/YYYY].Ex.01/01/1990');", true);
                        return 0;
                    }
                    EnteredDate = DateTime.Parse(txtmonth.Text, CultureInfo.GetCultureInfo("en-gb")).ToString();
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "show alert", "alert('You Are Entered Invalid  DATE.Date Format Should be [DD/MM/YYYY].Ex.01/01/1990');", true);
                    return 0;
                }
            }
            #endregion


            #region  Month Get Based on the Control Selection
            int month = 0;

            DateTime date = DateTime.Parse(txtmonth.Text, CultureInfo.GetCultureInfo("en-gb"));
            month = Timings.Instance.GetIdForEnteredMOnth(date);
            return month;


            #endregion
        }

        public string GetMonthOfYear()
        {
            string MonthYear = "";

            int month = GetMonthBasedOnSelectionDateorMonth();
            if (month.ToString().Length == 4)
            {
                MonthYear = "20" + month.ToString().Substring(2, 2);
            }
            if (month.ToString().Length == 3)
            {
                MonthYear = "20" + month.ToString().Substring(1, 2);

            }
            return MonthYear;
        }
        #region Total Variables
        float totalActualamount = 0;
        float totalctcamount = 0;
        float totalDuties = 0;
        float totalOts = 0;
        float totalwo = 0;
        float totalnhs = 0;
        float totalnpots = 0;
        float totaltempgross = 0;
        float totalBasic = 0;
        float totalDA = 0;
        float totalHRA = 0;
        float totalCCA = 0;
        float totalConveyance = 0;
        float totalWA = 0;
        float totalOA = 0;
        float totalGrass = 0;
        float totalOTAmount = 0;
        float totalServiceWeightage = 0;
        float totalArrears = 0;
        float totalPF = 0;
        float totalESI = 0;
        float totalProfTax = 0;
        float totalSalAdv = 0;
        float totalUniformDed = 0;
        float totalAdvDed = 0;
        float totalwed = 0;
        float totalWCDed = 0;
        float totalCanteenAdv = 0;
        float totalSeepDed = 0;
        float totalLeaveEncashAmt = 0;
        float totalGratuity = 0;
        float totalBonus = 0;
        float totalnfhs = 0;
        float totalDed = 0;
        float totalOtherDed = 0;
        float totalIncentivs = 0;
        float totalWoAmt = 0;
        float totalNhsAmt = 0;
        float totalNpotsAmt = 0;
        float totalPenalty = 0;
        float totalRC = 0;
        float totalCS = 0;
        float totalOWF = 0;
        float totalSecDepDed = 0;
        float totalloanded = 0;
        float totalGenDed = 0;
        float totalctc = 0;

        float totalAttBonus = 0;
        float totalNightAllw = 0;
        float totalEmpty1 = 0;
        float totalEmpty2 = 0;
        float totalEmpty3 = 0;
        float totalTravelAllw = 0;
        float totalNightShiftAllw = 0;
        float totalFoodAllowance = 0;
        float totalmedicalallowance = 0;
        float totalUniformAllw = 0;

        float totalAdv4Ded = 0;
        float totalNightRoundDed = 0;
        float totalManpowerMobDed = 0;
        float totalMobileusageDed = 0;
        float totalMediClaimDed = 0;
        float totalCrisisDed = 0;
        float totalMobInstDed = 0;
        float totalTDSDed = 0;


        float totalSpecialAllowance = 0;
        float totalMobileAllowance = 0;
        float totalPerformanceAllw = 0;
        float totalNPCl25Per = 0;
        float totalstrESIDays = 0;
        float totalTransport6Per = 0;
        float totalTransport = 0;

        float totalRentDed = 0;
        float totalMedicalDed = 0;
        float totalMLWFDed = 0;
        float totalFoodDed = 0;
        float totalAddlAmount = 0;


        float totalElectricityDed = 0;
        float totalTransportDed = 0;
        float totalDced = 0;
        float totalDccDed = 0;
        float totalLeaveDed = 0;
        float totalLicenseDed = 0;
        float totalpfempr = 0;
        float totalesiempr = 0;
        float totalDiv = 0;
        float totalArea = 0;
        float totalTelephoneBillDed = 0;
        float totalcdOtRate = 0;

        float totalCdTempGross = 0;
        float totalCdBasic = 0;
        float totalCdDA = 0;
        float totalCdHRA = 0;
        float totalCdCCA = 0;
        float totalCdConveyance = 0;
        float totalCdWA = 0;
        float totalCdNfhs = 0;
        float totalCdrc = 0;
        float totalCdcs = 0;

        float totalCdAddlAmount = 0;
        float totalCdFoodAllw = 0;
        float totalCdWOAmt = 0;
        float totalCdNHsAmt = 0;
        float totalCdMedicalReimbursement = 0;
        float totalCdSpecialAllW = 0;
        float totalCdMobileAllw = 0;
        float totalCdTravelAllw = 0;
        float totalCdPerformanceAllw = 0;
        float totalCdLW = 0;
        float totalCdNPOTsAmt = 0;
        float totalCdOA = 0;
        float totalCdIncentivs = 0;
        float totalCdBonus = 0;
        float totalCdGratuity = 0;
        float totalCdServiceWeightage = 0;
        float totalCdArrears = 0;
        float totalCdAttBonus = 0;
        float totalCdNightAllw = 0;
        float totalCdEmpty1 = 0;
        float totalCdEmpty2 = 0;
        float totalCdEmpty3 = 0;
        float totalADDL4HR = 0;
        float totalQTRALLOW = 0;
        float totalRELALLOW = 0;
        float totalSITEALLOW = 0;
        float totalGunAllw = 0;
        float totalFireAllw = 0;
        float totalTelephoneAllw = 0;
        float totalReimbursement = 0;
        float totalHardshipAllw = 0;
        float totalPaidHolidayAllw = 0;
        float totalfixedADDL4HR = 0;
        float totalfixedQTRALLOW = 0;
        float totalfixedRELALLOW = 0;
        float totalfixedSITEALLOW = 0;
        float totalfixedGunAllw = 0;
        float totalfixedFireAllw = 0;
        float totalfixedTelephoneAllw = 0;
        float totalfixedReimbursement = 0;
        float totalfixedHardshipAllw = 0;
        float totalfixedPaidHolidayAllw = 0;
        float totalAdmincharges = 0;
        float totalRegistrationFee = 0;
        float totalcdRankAllowance = 0;
        float totalRankAllowance = 0;
        float totalAdvBonus = 0;
        float totalOTAmt = 0;
        float totalotesi = 0;
        float totalnetot = 0;
        float totalTOTALNETPAY = 0;
        float totaluniformcharge = 0;
        float totalservicecharge = 0;
        float totalgst = 0;
        float totaladvance = 0;
        float totaluniform = 0;
        float totalothers = 0;
        float totaltotdeductions = 0;
        float totalESiDAYS = 0;
        float totalCTC = 0;
        float totalservicecharge1 = 0;
        float totalgstper = 0;
        float totalgrandtotal = 0;
        float totalAdvances = 0;
        float totalUnifom = 0;
        float totalOthers = 0;
        float totalTotDeductions = 0;
        #endregion Total Variables

        protected void DisplayData()
        {
            // if (ddlclientid.SelectedIndex > 0)
            {
                try
                {
                    string monthnew = string.Empty;
                    //if (ddloptions.SelectedIndex == 0)
                    //{
                    //    string date = string.Empty;


                    //    if (txtmonth.Text.Trim().Length > 0)
                    //    {
                    //        date = DateTime.Parse(txtmonth.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
                    //    }

                    //    string month = DateTime.Parse(date).Month.ToString();
                    //    string Year = DateTime.Parse(date).Year.ToString();
                    //    monthnew = month + Year.Substring(2, 2);
                    //}
                    string sqlqry = string.Empty;

                    //var list = new List<string>();
                    var list = new List<Tuple<int, string>>();
                    // var monthlist = new List<string>();
                    var options = 0;
                    if (GVClientsData.Rows.Count > 0)
                    {
                        for (int i = 0; i < GVClientsData.Rows.Count; i++)
                        {
                            CheckBox chkclientid = GVClientsData.Rows[i].FindControl("chkindividual") as CheckBox;
                            Label lblclientid = GVClientsData.Rows[i].FindControl("lblclientid") as Label;
                            Label lblmonth = GVClientsData.Rows[i].FindControl("lblmonth") as Label;

                            if (chkclientid.Checked == true)
                            {
                                list.Add(Tuple.Create(Convert.ToInt32(lblmonth.Text), lblclientid.Text));
                            }

                        }
                    }

                    // string Clientids = string.Join(",", list.ToArray());


                    DataTable dtClientList = new DataTable();
                    DataTable dtmonth = new DataTable();
                    dtClientList.Columns.Add("Clientid");
                    dtmonth.Columns.Add("month");
                    if (list.Count != 0)
                    {

                        foreach (Tuple<int, string> tuple in list)
                        {
                            DataRow row = dtClientList.NewRow();
                            DataRow row1 = dtmonth.NewRow();
                            row["Clientid"] = tuple.Item2;
                            row1["month"] = tuple.Item1;
                            dtClientList.Rows.Add(row["Clientid"]);
                            dtmonth.Rows.Add(row1["month"]);
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "show alert", "alert('Please select Client IDs');", true);
                    }

                    var SPName = "";
                    Hashtable HTPaysheet = new Hashtable();
                    int countearnings = 0;
                    int countdedutions = 0;
                    int countot = 0;
                    int countpfempr = 0;
                    int countesiempr = 0;
                    int countctc = 0;

                    {
                        string Fromdate = string.Empty;
                        string ToMonth = string.Empty;

                        if (txtfrom.Text.Trim().Length > 0)
                        {
                            Fromdate = DateTime.Parse(txtfrom.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
                        }
                        if (txtto.Text.Trim().Length > 0)
                        {
                            ToMonth = DateTime.Parse(txtto.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
                        }
                        SPName = "[dbo].[EMPWAGESHEETREPORTFromToctcWise]";
                        options = ddloptions.SelectedIndex;
                        HTPaysheet.Add("@ClientIDList", dtClientList);
                        HTPaysheet.Add("@MonthList", dtmonth);
                    }


                    DataTable dt = config.ExecuteAdaptorAsyncWithParams(SPName, HTPaysheet).Result;

                    if (dt.Rows.Count > 0)
                    {
                        GVListEmployees1.DataSource = dt;
                        GVListEmployees1.DataBind();


                        lbtn_Export.Visible = true;
                        lbtn_ExportNew.Visible = true;



                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (dt.Rows[i]["EmpIFSCcode"].ToString() == "Sub Total")
                            {
                                float actAmount = 0;

                                if (actAmount >= 0)
                                {
                                    #region

                                    // Total Calculation 

                                    //26-04-2019

                                    string strCdTempGross = dt.Rows[i]["TempGross"].ToString();
                                    if (strCdTempGross.Trim().Length > 0)
                                    {
                                        totalCdTempGross += Convert.ToSingle(strCdTempGross);
                                    }

                                    string strCdBasic = dt.Rows[i]["CdBasic"].ToString();
                                    if (strCdBasic.Trim().Length > 0)
                                    {
                                        totalCdBasic += Convert.ToSingle(strCdBasic);
                                    }
                                    string strCdDA = dt.Rows[i]["CdDA"].ToString();
                                    if (strCdDA.Trim().Length > 0)
                                    {
                                        totalCdDA += Convert.ToSingle(strCdDA);
                                    }
                                    string strCdHRA = dt.Rows[i]["CdHRA"].ToString();
                                    if (strCdHRA.Trim().Length > 0)
                                    {
                                        totalCdHRA += Convert.ToSingle(strCdHRA);
                                    }
                                    string strCdCCA = dt.Rows[i]["CdCCA"].ToString();
                                    if (strCdCCA.Trim().Length > 0)
                                    {
                                        totalCdCCA += Convert.ToSingle(strCdCCA);
                                    }
                                    string strCdConv = dt.Rows[i]["CdConv"].ToString();
                                    if (strCdConv.Trim().Length > 0)
                                    {
                                        totalCdConveyance += Convert.ToSingle(strCdConv);
                                    }

                                    string strCdWA = dt.Rows[i]["CdWA"].ToString();
                                    if (strCdWA.Trim().Length > 0)
                                    {
                                        totalCdWA += Convert.ToSingle(strCdWA);
                                    }
                                    string strCdNfhs = dt.Rows[i]["CdNfhs"].ToString();
                                    if (strCdNfhs.Trim().Length > 0)
                                    {
                                        totalCdNfhs += Convert.ToSingle(strCdNfhs);
                                    }
                                    string strCdRC = dt.Rows[i]["CdRC"].ToString();
                                    if (strCdRC.Trim().Length > 0)
                                    {
                                        totalCdrc += Convert.ToSingle(strCdRC);
                                    }
                                    string strCdCS = dt.Rows[i]["CdCS"].ToString();
                                    if (strCdCS.Trim().Length > 0)
                                    {
                                        totalCdcs += Convert.ToSingle(strCdCS);
                                    }
                                    string strCdAddlAmount = dt.Rows[i]["CdAddlAmount"].ToString();
                                    if (strCdAddlAmount.Trim().Length > 0)
                                    {
                                        totalCdAddlAmount += Convert.ToSingle(strCdAddlAmount);
                                    }
                                    string strCdFoodAllw = dt.Rows[i]["CdFoodAllw"].ToString();
                                    if (strCdFoodAllw.Trim().Length > 0)
                                    {
                                        totalCdFoodAllw += Convert.ToSingle(strCdFoodAllw);
                                    }
                                    string strCdWOAmt = dt.Rows[i]["CdWOAmt"].ToString();
                                    if (strCdWOAmt.Trim().Length > 0)
                                    {
                                        totalCdWOAmt += Convert.ToSingle(strCdWOAmt);
                                    }
                                    string strCdNHsAmt = dt.Rows[i]["CdNHsAmt"].ToString();
                                    if (strCdNHsAmt.Trim().Length > 0)
                                    {
                                        totalCdNHsAmt += Convert.ToSingle(strCdNHsAmt);
                                    }
                                    string strCdMedicalReimbursement = dt.Rows[i]["CdMedicalReimbursement"].ToString();
                                    if (strCdMedicalReimbursement.Trim().Length > 0)
                                    {
                                        totalCdMedicalReimbursement += Convert.ToSingle(strCdMedicalReimbursement);
                                    }
                                    string strCdSpecialAllW = dt.Rows[i]["CdSpecialAllW"].ToString();
                                    if (strCdSpecialAllW.Trim().Length > 0)
                                    {
                                        totalCdSpecialAllW += Convert.ToSingle(strCdSpecialAllW);
                                    }
                                    string strCdTravelAllw = dt.Rows[i]["CdTravelAllw"].ToString();
                                    if (strCdTravelAllw.Trim().Length > 0)
                                    {
                                        totalCdTravelAllw += Convert.ToSingle(strCdTravelAllw);
                                    }
                                    string strCdMobileAllw = dt.Rows[i]["CdMobileAllw"].ToString();
                                    if (strCdMobileAllw.Trim().Length > 0)
                                    {
                                        totalCdMobileAllw += Convert.ToSingle(strCdMobileAllw);
                                    }
                                    string strCdPerformanceAllw = dt.Rows[i]["CdPerformanceAllw"].ToString();
                                    if (strCdPerformanceAllw.Trim().Length > 0)
                                    {
                                        totalCdPerformanceAllw += Convert.ToSingle(strCdPerformanceAllw);
                                    }
                                    string strCdLW = dt.Rows[i]["CdLW"].ToString();
                                    if (strCdLW.Trim().Length > 0)
                                    {
                                        totalCdLW += Convert.ToSingle(strCdLW);
                                    }
                                    string strCdNPOTsAmt = dt.Rows[i]["CdNPOTsAmt"].ToString();
                                    if (strCdNPOTsAmt.Trim().Length > 0)
                                    {
                                        totalCdNPOTsAmt += Convert.ToSingle(strCdNPOTsAmt);
                                    }
                                    string strCdIncentivs = dt.Rows[i]["CdIncentivs"].ToString();
                                    if (strCdIncentivs.Trim().Length > 0)
                                    {
                                        totalCdIncentivs += Convert.ToSingle(strCdIncentivs);
                                    }
                                    string strCdBonus = dt.Rows[i]["CdBonus"].ToString();
                                    if (strCdBonus.Trim().Length > 0)
                                    {
                                        totalCdBonus += Convert.ToSingle(strCdBonus);
                                    }
                                    string strCdGratuity = dt.Rows[i]["CdGratuity"].ToString();
                                    if (strCdGratuity.Trim().Length > 0)
                                    {
                                        totalCdGratuity += Convert.ToSingle(strCdGratuity);
                                    }
                                    string strCdOA = dt.Rows[i]["CdOA"].ToString();
                                    if (strCdOA.Trim().Length > 0)
                                    {
                                        totalCdOA += Convert.ToSingle(strCdOA);
                                    }
                                    string strCdOTAmt = dt.Rows[i]["CdOtAmt1"].ToString();
                                    if (strCdOTAmt.Trim().Length > 0)
                                    {
                                        totalcdOtRate += Convert.ToSingle(strCdOTAmt);
                                    }
                                    string strCdServiceWeightage = dt.Rows[i]["CdServiceWeightage"].ToString();
                                    if (strCdServiceWeightage.Trim().Length > 0)
                                    {
                                        totalCdServiceWeightage += Convert.ToSingle(strCdServiceWeightage);
                                    }
                                    string strcdRankAllowance = dt.Rows[i]["CdRankAllowance"].ToString();
                                    if (strcdRankAllowance.Trim().Length > 0)
                                    {
                                        totalcdRankAllowance += Convert.ToSingle(strcdRankAllowance);
                                    }
                                    string strCdAttBonus = dt.Rows[i]["CdAttBonus"].ToString();
                                    if (strCdAttBonus.Trim().Length > 0)
                                    {
                                        totalCdAttBonus += Convert.ToSingle(strCdAttBonus);
                                    }
                                    string strCdNightAllw = dt.Rows[i]["CdNightAllw"].ToString();
                                    if (strCdNightAllw.Trim().Length > 0)
                                    {
                                        totalCdNightAllw += Convert.ToSingle(strCdNightAllw);
                                    }
                                    string strCdEmpty1 = dt.Rows[i]["CdEmpty1"].ToString();
                                    if (strCdEmpty1.Trim().Length > 0)
                                    {
                                        totalCdEmpty1 += Convert.ToSingle(strCdEmpty1);
                                    }
                                    string strCdEmpty2 = dt.Rows[i]["CdEmpty2"].ToString();
                                    if (strCdEmpty2.Trim().Length > 0)
                                    {
                                        totalCdEmpty2 += Convert.ToSingle(strCdEmpty2);
                                    }


                                    string duties = dt.Rows[i]["NoOfDuties"].ToString();
                                    if (duties.Trim().Length > 0)
                                    {
                                        totalDuties += Convert.ToSingle(duties);
                                    }
                                    string ots = dt.Rows[i]["OTs"].ToString();
                                    if (ots.Trim().Length > 0)
                                    {
                                        totalOts += Convert.ToSingle(ots);
                                    }

                                    string wos = dt.Rows[i]["wo"].ToString();
                                    if (wos.Trim().Length > 0)
                                    {
                                        totalwo += Convert.ToSingle(wos);
                                    }
                                    string nhs = dt.Rows[i]["nhs"].ToString();
                                    if (nhs.Trim().Length > 0)
                                    {
                                        totalnhs += Convert.ToSingle(nhs);
                                    }
                                    string npots = dt.Rows[i]["pldays"].ToString();
                                    if (npots.Trim().Length > 0)
                                    {
                                        totalnpots += Convert.ToSingle(npots);
                                    }

                                    string strBasic = dt.Rows[i]["Basic"].ToString();
                                    if (strBasic.Trim().Length > 0)
                                    {
                                        totalBasic += Convert.ToSingle(strBasic);
                                        countearnings += 1;
                                    }
                                    string strDA = dt.Rows[i]["DA"].ToString();
                                    if (strDA.Trim().Length > 0)
                                    {
                                        totalDA += Convert.ToSingle(strDA);

                                    }
                                    string strhHRA = dt.Rows[i]["HRA"].ToString();
                                    if (strhHRA.Trim().Length > 0)
                                    {
                                        totalHRA += Convert.ToSingle(strhHRA);

                                    }
                                    string strCCA = dt.Rows[i]["CCA"].ToString();
                                    if (strCCA.Trim().Length > 0)
                                    {
                                        totalCCA += Convert.ToSingle(strCCA);

                                    }
                                    string strConveyance = dt.Rows[i]["Conveyance"].ToString();
                                    if (strConveyance.Trim().Length > 0)
                                    {
                                        totalConveyance += Convert.ToSingle(strConveyance);

                                    }
                                    string strWA = dt.Rows[i]["WashAllowance"].ToString();
                                    if (strWA.Trim().Length > 0)
                                    {
                                        totalWA += Convert.ToSingle(strWA);

                                    }
                                    string strNfhs = dt.Rows[i]["Nfhs"].ToString();
                                    if (strNfhs.Trim().Length > 0)
                                    {
                                        totalnfhs += Convert.ToSingle(strNfhs);

                                    }
                                    string strRC = dt.Rows[i]["RC"].ToString();
                                    if (strRC.Trim().Length > 0)
                                    {
                                        totalRC += Convert.ToSingle(strRC);

                                    }

                                    string strCS = dt.Rows[i]["CS"].ToString();
                                    if (strCS.Trim().Length > 0)
                                    {
                                        totalCS += Convert.ToSingle(strCS);
                                        countearnings += 1;
                                    }
                                    string strAddlAmount = dt.Rows[i]["AddlAmount"].ToString();
                                    if (strAddlAmount.Trim().Length > 0)
                                    {
                                        totalAddlAmount += Convert.ToSingle(strAddlAmount);

                                    }
                                    string strFoodAllowance = dt.Rows[i]["FoodAllowance"].ToString();
                                    if (strFoodAllowance.Trim().Length > 0)
                                    {
                                        totalFoodAllowance += Convert.ToSingle(strFoodAllowance);

                                    }
                                    string strWoAmt = dt.Rows[i]["WOAmt"].ToString();
                                    if (strWoAmt.Trim().Length > 0)
                                    {
                                        totalWoAmt += Convert.ToSingle(strWoAmt);

                                    }

                                    string strNhsAmt = dt.Rows[i]["Nhsamt"].ToString();
                                    if (strNhsAmt.Trim().Length > 0)
                                    {
                                        totalNhsAmt += Convert.ToSingle(strNhsAmt);

                                    }
                                    string strmedicalallowance = dt.Rows[i]["medicalallowance"].ToString();
                                    if (strmedicalallowance.Trim().Length > 0)
                                    {
                                        totalmedicalallowance += Convert.ToSingle(strmedicalallowance);

                                    }
                                    string strSpecialAllowance = dt.Rows[i]["SpecialAllowance"].ToString();
                                    if (strSpecialAllowance.Trim().Length > 0)
                                    {
                                        totalSpecialAllowance += Convert.ToSingle(strSpecialAllowance);

                                    }
                                    string strTravelAllw = dt.Rows[i]["TravelAllw"].ToString();
                                    if (strTravelAllw.Trim().Length > 0)
                                    {
                                        totalTravelAllw += Convert.ToSingle(strTravelAllw);

                                    }
                                    string strMobileAllowance = dt.Rows[i]["MobileAllw"].ToString();
                                    if (strMobileAllowance.Trim().Length > 0)
                                    {
                                        totalMobileAllowance += Convert.ToSingle(strMobileAllowance);

                                    }

                                    string strPerformanceAllw = dt.Rows[i]["PerformanceAllw"].ToString();
                                    if (strPerformanceAllw.Trim().Length > 0)
                                    {
                                        totalPerformanceAllw += Convert.ToSingle(strPerformanceAllw);

                                    }

                                    string strLeaveEncashAmt = dt.Rows[i]["LeaveEncashAmt"].ToString();
                                    if (strCCA.Trim().Length > 0)
                                    {
                                        totalLeaveEncashAmt += Convert.ToSingle(strLeaveEncashAmt);

                                    }
                                    string strNpotsAmt = dt.Rows[i]["Npotsamt"].ToString();
                                    if (strNpotsAmt.Trim().Length > 0)
                                    {
                                        totalNpotsAmt += Convert.ToSingle(strNpotsAmt);

                                    }
                                    string strIncentivs = dt.Rows[i]["Incentivs"].ToString();
                                    if (strIncentivs.Trim().Length > 0)
                                    {
                                        totalIncentivs += Convert.ToSingle(strIncentivs);

                                    }
                                    string strBonus = dt.Rows[i]["Bonus"].ToString();
                                    if (strBonus.Trim().Length > 0)
                                    {
                                        totalBonus += Convert.ToSingle(strBonus);

                                    }
                                    string strGratuity = dt.Rows[i]["Gratuity"].ToString();
                                    if (strGratuity.Trim().Length > 0)
                                    {
                                        totalGratuity += Convert.ToSingle(strGratuity);

                                    }

                                    string strOA = dt.Rows[i]["OtherAllowance"].ToString();
                                    if (strOA.Trim().Length > 0)
                                    {
                                        totalOA += Convert.ToSingle(strOA);

                                    }
                                    string strOTAmount = dt.Rows[i]["OTAmt"].ToString();
                                    if (strOTAmount.Trim().Length > 0)
                                    {
                                        totalOTAmount += Convert.ToSingle(strOTAmount);
                                    }

                                    string strTransport6Per = dt.Rows[i]["ServiceWeightage"].ToString();
                                    if (strTransport6Per.Trim().Length > 0)
                                    {
                                        totalTransport6Per += Convert.ToSingle(strTransport6Per);

                                    }
                                    string strArrears = dt.Rows[i]["Arrears"].ToString();
                                    if (strArrears.Trim().Length > 0)
                                    {
                                        totalArrears += Convert.ToSingle(strArrears);

                                    }
                                    string strAttBonus = dt.Rows[i]["AttBonus"].ToString();
                                    if (strAttBonus.Trim().Length > 0)
                                    {
                                        totalAttBonus += Convert.ToSingle(strAttBonus);
                                        countearnings += 1;
                                    }
                                    string strNightShiftAllw = dt.Rows[i]["NightAllw"].ToString();
                                    if (strNightShiftAllw.Trim().Length > 0)
                                    {
                                        totalNightShiftAllw += Convert.ToSingle(strNightShiftAllw);

                                    }

                                    string strEmpty = dt.Rows[i]["Empty"].ToString();
                                    if (strEmpty.Trim().Length > 0)
                                    {
                                        totalEmpty1 += Convert.ToSingle(strEmpty);
                                    }
                                    string strEmpty1 = dt.Rows[i]["Empty"].ToString();
                                    if (strEmpty1.Trim().Length > 0)
                                    {
                                        totalEmpty2 += Convert.ToSingle(strEmpty1);
                                    }
                                    string strRankAllowance = dt.Rows[i]["RankAllowance"].ToString();
                                    if (strRankAllowance.Trim().Length > 0)
                                    {
                                        totalRankAllowance += Convert.ToSingle(strRankAllowance);
                                    }
                                    string strGross = dt.Rows[i]["Gross"].ToString();
                                    if (strGross.Trim().Length > 0)
                                    {
                                        totalGrass += Convert.ToSingle(strGross);

                                    }
                                    string strPF = dt.Rows[i]["PF"].ToString();
                                    if (strPF.Trim().Length > 0)
                                    {
                                        totalPF += Convert.ToSingle(strPF);

                                    }
                                    string strESI = dt.Rows[i]["ESI"].ToString();
                                    if (strESI.Trim().Length > 0)
                                    {
                                        totalESI += Convert.ToSingle(strESI);

                                    }
                                    string strProfTax = dt.Rows[i]["ProfTax"].ToString();
                                    if (strProfTax.Trim().Length > 0)
                                    {
                                        totalProfTax += Convert.ToSingle(strProfTax);

                                    }

                                    string strSalAdv = dt.Rows[i]["SalAdvDed"].ToString();
                                    if (strSalAdv.Trim().Length > 0)
                                    {
                                        totalSalAdv += Convert.ToSingle(strSalAdv);

                                    }

                                    string strAdvDed = dt.Rows[i]["ADVDed"].ToString();
                                    if (strAdvDed.Trim().Length > 0)
                                    {
                                        totalAdvDed += Convert.ToSingle(strAdvDed);

                                    }

                                    string strWCDed = dt.Rows[i]["WCDed"].ToString();
                                    if (strWCDed.Trim().Length > 0)
                                    {
                                        totalWCDed += Convert.ToSingle(strWCDed);

                                    }

                                    string strUniformDed = dt.Rows[i]["UniformDed"].ToString();
                                    if (strUniformDed.Trim().Length > 0)
                                    {
                                        totalUniformDed += Convert.ToSingle(strUniformDed);

                                    }
                                    string strOtherDed = dt.Rows[i]["OtherDed"].ToString();
                                    if (strOtherDed.Trim().Length > 0)
                                    {
                                        totalOtherDed += Convert.ToSingle(strOtherDed);

                                    }

                                    string strLoanDed = dt.Rows[i]["LoanDed"].ToString();
                                    if (strLoanDed.Trim().Length > 0)
                                    {
                                        totalloanded += Convert.ToSingle(strLoanDed);
                                    }
                                    string strCanteenAdv = dt.Rows[i]["CanteenAdv"].ToString();
                                    if (strCanteenAdv.Trim().Length > 0)
                                    {
                                        totalCanteenAdv += Convert.ToSingle(strCanteenAdv);

                                    }
                                    string strSecDep = dt.Rows[i]["SecurityDepDed"].ToString();
                                    if (strSecDep.Trim().Length > 0)
                                    {
                                        totalSecDepDed += Convert.ToSingle(strSecDep);
                                    }


                                    string strGeneralDed = dt.Rows[i]["GeneralDed"].ToString();
                                    if (strGeneralDed.Trim().Length > 0)
                                    {
                                        totalGenDed += Convert.ToSingle(strGeneralDed);
                                    }
                                    string strOWF = dt.Rows[i]["OWF"].ToString();
                                    if (strOWF.Trim().Length > 0)
                                    {
                                        totalOWF += Convert.ToSingle(strOWF);
                                    }
                                    string strPenalty = dt.Rows[i]["Penalty"].ToString();
                                    if (strPenalty.Trim().Length > 0)
                                    {
                                        totalPenalty += Convert.ToSingle(strPenalty);
                                    }
                                    string strRentDed = dt.Rows[i]["atmDed"].ToString();
                                    if (strRentDed.Trim().Length > 0)
                                    {
                                        totalRentDed += Convert.ToSingle(strRentDed);

                                    }
                                    string strMedicalDed = dt.Rows[i]["MedicalDed"].ToString();
                                    if (strMedicalDed.Trim().Length > 0)
                                    {
                                        totalMedicalDed += Convert.ToSingle(strMedicalDed);

                                    }
                                    string strMLWFDed = dt.Rows[i]["MLWFDed"].ToString();
                                    if (strMLWFDed.Trim().Length > 0)
                                    {
                                        totalMLWFDed += Convert.ToSingle(strMLWFDed);

                                    }
                                    string strFoodDed = dt.Rows[i]["FoodDed"].ToString();
                                    if (strFoodDed.Trim().Length > 0)
                                    {
                                        totalFoodDed += Convert.ToSingle(strFoodDed);

                                    }

                                    string strElectricityDed = dt.Rows[i]["IDCardDed"].ToString();
                                    if (strElectricityDed.Trim().Length > 0)
                                    {
                                        totalElectricityDed += Convert.ToSingle(strElectricityDed);
                                    }


                                    string strTransportDed = dt.Rows[i]["RentDed1"].ToString();
                                    if (strTransportDed.Trim().Length > 0)
                                    {
                                        totalTransportDed += Convert.ToSingle(strTransportDed);

                                    }
                                    string strDccDed = dt.Rows[i]["Finesded1"].ToString();
                                    if (strDccDed.Trim().Length > 0)
                                    {
                                        totalDccDed += Convert.ToSingle(strDccDed);

                                    }
                                    string strLeaveDed = dt.Rows[i]["PVCDed"].ToString();
                                    if (strLeaveDed.Trim().Length > 0)
                                    {
                                        totalLeaveDed += Convert.ToSingle(strLeaveDed);

                                    }
                                    string strLicenseDed = dt.Rows[i]["LicenseDed"].ToString();
                                    if (strLicenseDed.Trim().Length > 0)
                                    {
                                        totalLicenseDed += Convert.ToSingle(strLicenseDed);

                                    }

                                    //
                                    string strAdv4Ded = dt.Rows[i]["Adv4Ded"].ToString();
                                    if (strAdv4Ded.Trim().Length > 0)
                                    {
                                        totalAdv4Ded += Convert.ToSingle(strAdv4Ded);

                                    }
                                    string strNightRoundDed = dt.Rows[i]["Extra"].ToString();
                                    if (strNightRoundDed.Trim().Length > 0)
                                    {
                                        totalNightRoundDed += Convert.ToSingle(strNightRoundDed);

                                    }

                                    string strManpowerMobDed = dt.Rows[i]["ManpowerMobDed"].ToString();
                                    if (strManpowerMobDed.Trim().Length > 0)
                                    {
                                        totalManpowerMobDed += Convert.ToSingle(strManpowerMobDed);

                                    }

                                    string strMobileusageDed = dt.Rows[i]["MobileusageDed"].ToString();
                                    if (strMobileusageDed.Trim().Length > 0)
                                    {
                                        totalMobileusageDed += Convert.ToSingle(strMobileusageDed);

                                    }

                                    string strMediClaimDed = dt.Rows[i]["MediClaimDed"].ToString();
                                    if (strMediClaimDed.Trim().Length > 0)
                                    {
                                        totalMediClaimDed += Convert.ToSingle(strMediClaimDed);

                                    }

                                    string strCrisisDed = dt.Rows[i]["CrisisDed"].ToString();
                                    if (strCrisisDed.Trim().Length > 0)
                                    {
                                        totalCrisisDed += Convert.ToSingle(strCrisisDed);

                                    }
                                    string strTelephoneBillDed = dt.Rows[i]["TelephoneBillDed"].ToString();
                                    if (strTelephoneBillDed.Trim().Length > 0)
                                    {
                                        totalTelephoneBillDed += Convert.ToSingle(strTelephoneBillDed);
                                    }
                                    string strDed = dt.Rows[i]["TotalDeductions"].ToString();
                                    if (strDed.Trim().Length > 0)
                                    {
                                        totalDed += Convert.ToSingle(strDed);
                                    }
                                    string actualAmount = dt.Rows[i]["ActualAmount"].ToString();
                                    if (actualAmount.Trim().Length > 0)
                                    {
                                        totalActualamount += Convert.ToSingle(actualAmount);
                                    }

                                    string strfixedADDL4HR = dt.Rows[i]["fixedADDL4HR"].ToString();
                                    if (strfixedADDL4HR.Trim().Length > 0)
                                    {
                                        totalfixedADDL4HR += Convert.ToSingle(strfixedADDL4HR);

                                    }

                                    string strfixedQTRALLOW = dt.Rows[i]["fixedQTRALLOW"].ToString();
                                    if (strfixedQTRALLOW.Trim().Length > 0)
                                    {
                                        totalfixedQTRALLOW += Convert.ToSingle(strfixedQTRALLOW);

                                    }

                                    string strfixedRELALLOW = dt.Rows[i]["fixedRELALLOW"].ToString();
                                    if (strfixedRELALLOW.Trim().Length > 0)
                                    {
                                        totalfixedRELALLOW += Convert.ToSingle(strfixedRELALLOW);

                                    }

                                    string strfixedSITEALLOW = dt.Rows[i]["fixedSITEALLOW"].ToString();
                                    if (strfixedSITEALLOW.Trim().Length > 0)
                                    {
                                        totalfixedSITEALLOW += Convert.ToSingle(strfixedSITEALLOW);

                                    }

                                    string strfixedGunAllw = dt.Rows[i]["fixedGunAllw"].ToString();
                                    if (strfixedGunAllw.Trim().Length > 0)
                                    {
                                        totalfixedGunAllw += Convert.ToSingle(strfixedGunAllw);

                                    }

                                    string strfixedFireAllw = dt.Rows[i]["fixedFireAllw"].ToString();
                                    if (strfixedFireAllw.Trim().Length > 0)
                                    {
                                        totalfixedFireAllw += Convert.ToSingle(strfixedFireAllw);

                                    }

                                    string strfixedTelephoneAllw = dt.Rows[i]["fixedTelephoneAllw"].ToString();
                                    if (strfixedTelephoneAllw.Trim().Length > 0)
                                    {
                                        totalfixedTelephoneAllw += Convert.ToSingle(strfixedTelephoneAllw);

                                    }

                                    string strfixedReimbursement = dt.Rows[i]["fixedReimbursement"].ToString();
                                    if (strfixedReimbursement.Trim().Length > 0)
                                    {
                                        totalfixedReimbursement += Convert.ToSingle(strfixedReimbursement);

                                    }

                                    string strfixedHardshipAllw = dt.Rows[i]["fixedHardshipAllw"].ToString();
                                    if (strfixedHardshipAllw.Trim().Length > 0)
                                    {
                                        totalfixedHardshipAllw += Convert.ToSingle(strfixedHardshipAllw);

                                    }

                                    string strfixedPaidHolidayAllw = dt.Rows[i]["fixedPaidHolidayAllw"].ToString();
                                    if (strfixedPaidHolidayAllw.Trim().Length > 0)
                                    {
                                        totalfixedPaidHolidayAllw += Convert.ToSingle(strfixedPaidHolidayAllw);

                                    }

                                    string strADDL4HR = dt.Rows[i]["ADDL4HR"].ToString();
                                    if (strADDL4HR.Trim().Length > 0)
                                    {
                                        totalADDL4HR += Convert.ToSingle(strADDL4HR);

                                    }

                                    string strQTRALLOW = dt.Rows[i]["QTRALLOW"].ToString();
                                    if (strQTRALLOW.Trim().Length > 0)
                                    {
                                        totalQTRALLOW += Convert.ToSingle(strQTRALLOW);

                                    }

                                    string strRELALLOW = dt.Rows[i]["RELALLOW"].ToString();
                                    if (strRELALLOW.Trim().Length > 0)
                                    {
                                        totalRELALLOW += Convert.ToSingle(strRELALLOW);

                                    }

                                    string strSITEALLOW = dt.Rows[i]["SITEALLOW"].ToString();
                                    if (strSITEALLOW.Trim().Length > 0)
                                    {
                                        totalSITEALLOW += Convert.ToSingle(strSITEALLOW);

                                    }

                                    string strGunAllw = dt.Rows[i]["GunAllw"].ToString();
                                    if (strGunAllw.Trim().Length > 0)
                                    {
                                        totalGunAllw += Convert.ToSingle(strGunAllw);

                                    }

                                    string strFireAllw = dt.Rows[i]["FireAllw"].ToString();
                                    if (strFireAllw.Trim().Length > 0)
                                    {
                                        totalFireAllw += Convert.ToSingle(strFireAllw);

                                    }

                                    string strTelephoneAllw = dt.Rows[i]["TelephoneAllw"].ToString();
                                    if (strTelephoneAllw.Trim().Length > 0)
                                    {
                                        totalTelephoneAllw += Convert.ToSingle(strTelephoneAllw);

                                    }

                                    string strReimbursement = dt.Rows[i]["Reimbursement"].ToString();
                                    if (strReimbursement.Trim().Length > 0)
                                    {
                                        totalReimbursement += Convert.ToSingle(strReimbursement);

                                    }

                                    string strHardshipAllw = dt.Rows[i]["HardshipAllw"].ToString();
                                    if (strHardshipAllw.Trim().Length > 0)
                                    {
                                        totalHardshipAllw += Convert.ToSingle(strHardshipAllw);

                                    }

                                    string strPaidHolidayAllw = dt.Rows[i]["PaidHolidayAllw"].ToString();
                                    if (strPaidHolidayAllw.Trim().Length > 0)
                                    {
                                        totalPaidHolidayAllw += Convert.ToSingle(strPaidHolidayAllw);

                                    }

                                    string strAdmincharges = dt.Rows[i]["AdminCharges"].ToString();
                                    if (strAdmincharges.Trim().Length > 0)
                                    {
                                        totalAdmincharges += Convert.ToSingle(strAdmincharges);

                                    }

                                    string strRegistrationFee = dt.Rows[i]["RegistrationFee"].ToString();
                                    if (strRegistrationFee.Trim().Length > 0)
                                    {
                                        totalRegistrationFee += Convert.ToSingle(strRegistrationFee);
                                    }

                                    string stAdvBonus = dt.Rows[i]["AdvBonus"].ToString();
                                    if (stAdvBonus.Trim().Length > 0)
                                    {
                                        totalAdvBonus += Convert.ToSingle(stAdvBonus);
                                    }

                                    string stOTAmt = dt.Rows[i]["OTAmt"].ToString();
                                    if (stOTAmt.Trim().Length > 0)
                                    {
                                        totalOTAmt += Convert.ToSingle(stOTAmt);
                                    }

                                    string stotesi = dt.Rows[i]["otesi"].ToString();
                                    if (stotesi.Trim().Length > 0)
                                    {
                                        totalotesi += Convert.ToSingle(stotesi);
                                    }

                                    string stnetot = dt.Rows[i]["netot"].ToString();
                                    if (stnetot.Trim().Length > 0)
                                    {
                                        totalnetot += Convert.ToSingle(stnetot);
                                    }

                                    string stTOTALNETPAY = dt.Rows[i]["ActualAmount"].ToString();
                                    if (stTOTALNETPAY.Trim().Length > 0)
                                    {
                                        totalTOTALNETPAY += Convert.ToSingle(stTOTALNETPAY);
                                    }

                                    string stpfempr = dt.Rows[i]["pfempr"].ToString();
                                    if (stpfempr.Trim().Length > 0)
                                    {
                                        totalpfempr += Convert.ToSingle(stpfempr);
                                    }

                                    string stesiempr = dt.Rows[i]["esiempr"].ToString();
                                    if (stesiempr.Trim().Length > 0)
                                    {
                                        totalesiempr += Convert.ToSingle(stesiempr);
                                    }
                                    string stCTC = dt.Rows[i]["CTC"].ToString();
                                    if (stCTC.Trim().Length > 0)
                                    {
                                        totalCTC += Convert.ToSingle(stCTC);
                                    }

                                    string stservicecharge = dt.Rows[i]["servicecharge"].ToString();
                                    if (stservicecharge.Trim().Length > 0)
                                    {
                                        totalservicecharge1 += Convert.ToSingle(stservicecharge);
                                    }

                                    string stgstper = dt.Rows[i]["gstper"].ToString();
                                    if (stgstper.Trim().Length > 0)
                                    {
                                        totalgstper += Convert.ToSingle(stgstper);
                                    }

                                    string stgrandtotal = dt.Rows[i]["grandtotal"].ToString();
                                    if (stgrandtotal.Trim().Length > 0)
                                    {
                                        totalgrandtotal += Convert.ToSingle(stgrandtotal);
                                    }


                                    string struniformcharge = dt.Rows[i]["uniformcharge"].ToString();
                                    if (struniformcharge.Trim().Length > 0)
                                    {
                                        totaluniformcharge += Convert.ToSingle(struniformcharge);
                                    }

                                    string stAdvances = dt.Rows[i]["Advances"].ToString();
                                    if (stAdvances.Trim().Length > 0)
                                    {
                                        totalAdvances += Convert.ToSingle(stAdvances);
                                    }

                                    string stUnifom = dt.Rows[i]["Unifom"].ToString();
                                    if (stUnifom.Trim().Length > 0)
                                    {
                                        totalUnifom += Convert.ToSingle(stUnifom);
                                    }

                                    string stOthers = dt.Rows[i]["Others"].ToString();
                                    if (stOthers.Trim().Length > 0)
                                    {
                                        totalOthers += Convert.ToSingle(stOthers);
                                    }

                                    string stTotDeductions = dt.Rows[i]["TotDeductions"].ToString();
                                    if (stTotDeductions.Trim().Length > 0)
                                    {
                                        totalTotDeductions += Convert.ToSingle(stTotDeductions);
                                    }




                                    string strNPCl25Per = dt.Rows[i]["OTHrs"].ToString();
                                    if (strNPCl25Per.Trim().Length > 0)
                                    {
                                        totalNPCl25Per += Convert.ToSingle(strNPCl25Per);
                                    }
                                    //string strESIDays = dt.Rows[i]["ESIDays"].ToString();
                                    //if (strESIDays.Trim().Length > 0)
                                    //{
                                    //    totalstrESIDays += Convert.ToSingle(strESIDays);
                                    //}

                                    //string Div = dt.Rows[i]["DivisionName"].ToString();
                                    //if (Div.Trim().Length > 0)
                                    //{
                                    //    totalDiv += Convert.ToSingle(Div);
                                    //}
                                    //string Area = dt.Rows[i]["Location"].ToString();
                                    //if (Area.Trim().Length > 0)
                                    //{
                                    //    totalArea += Convert.ToSingle(Area);
                                    //}
                                    //string ntempgross = dt.Rows[i]["tempgross"].ToString();
                                    //if (ntempgross.Trim().Length > 0)
                                    //{
                                    //    totaltempgross += Convert.ToSingle(ntempgross);
                                    //}
                                    //string strcdOtAmt = dt.Rows[i]["cdOTAmt"].ToString();
                                    //if (strcdOtAmt.Trim().Length > 0)
                                    //{
                                    //    totalcdOtAmt += Convert.ToSingle(strcdOtAmt);

                                    //}
                                    //string strTransport = dt.Rows[i]["OTHrsAmt"].ToString();
                                    //if (strTransport.Trim().Length > 0)
                                    //{
                                    //    totalTransport += Convert.ToSingle(strTransport);
                                    //}
                                    //string strMobInstDed = dt.Rows[i]["MobInstDed"].ToString();
                                    //if (strMobInstDed.Trim().Length > 0)
                                    //{
                                    //    totalMobInstDed += Convert.ToSingle(strMobInstDed);

                                    //}

                                    //string strTDSDed = dt.Rows[i]["TDSDed"].ToString();
                                    //if (strTDSDed.Trim().Length > 0)
                                    //{
                                    //    totalTDSDed += Convert.ToSingle(strTDSDed);

                                    //}
                                    //string strNightAllw = dt.Rows[i]["NightAllw"].ToString();
                                    //if (strNightAllw.Trim().Length > 0)
                                    //{
                                    //    totalNightAllw += Convert.ToSingle(strNightAllw);

                                    //}
                                    //string strtotalwed = dt.Rows[i]["WCDed"].ToString();
                                    //if (strtotalwed.Trim().Length > 0)
                                    //{
                                    //    totalwed += Convert.ToSingle(strtotalwed);

                                    //}

                                    ////
                                    //string strpfempr = dt.Rows[i]["pfempr"].ToString();
                                    //if (strpfempr.Trim().Length > 0)
                                    //{
                                    //    totalpfempr += Convert.ToSingle(strpfempr);
                                    //}

                                    //string stresiempr = dt.Rows[i]["esiempr"].ToString();
                                    //if (stresiempr.Trim().Length > 0)
                                    //{
                                    //    totalesiempr += Convert.ToSingle(stresiempr);
                                    //}

                                    //string strctc = dt.Rows[i]["CTC"].ToString();
                                    //if (strctc.Trim().Length > 0)
                                    //{
                                    //    totalctc += Convert.ToSingle(strctc);
                                    //}

                                }
                                #endregion
                            }
                        }
                        #region for total


                        // Total Fixed Variables 

                        //26-04-2018


                        Label lbltotalTempGross = GVListEmployees1.FooterRow.FindControl("lbltotalTempGross") as Label;
                        lbltotalTempGross.Text = Math.Round(totalCdTempGross).ToString();

                        if (totalCdTempGross > 0)
                        {
                            GVListEmployees1.Columns[19].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[19].Visible = false;

                        }

                        Label lblTotalDuties = GVListEmployees1.FooterRow.FindControl("lblTotalDuties") as Label;
                        lblTotalDuties.Text = Math.Round(totalDuties).ToString();
                        if (totalDuties > 0)
                        {
                            GVListEmployees1.Columns[20].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[20].Visible = false;

                        }

                        Label lbltotalOTHours = GVListEmployees1.FooterRow.FindControl("lbltotalOTHours") as Label;
                        lbltotalOTHours.Text = Math.Round(totalNPCl25Per).ToString();
                        if (totalNPCl25Per > 0)
                        {
                            GVListEmployees1.Columns[21].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[21].Visible = false;

                        }



                        Label lblTotalCdBasic = GVListEmployees1.FooterRow.FindControl("lblTotalCdBasic") as Label;
                        lblTotalCdBasic.Text = Math.Round(totalCdBasic).ToString();

                        if (totalCdBasic > 0)
                        {
                            GVListEmployees1.Columns[22].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[22].Visible = false;

                        }
                        Label lblTotalCdDA = GVListEmployees1.FooterRow.FindControl("lblTotalCdDA") as Label;
                        lblTotalCdDA.Text = Math.Round(totalCdDA).ToString();

                        if (totalCdDA > 0)
                        {
                            GVListEmployees1.Columns[23].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[23].Visible = false;

                        }
                        Label lblTotalCdHRA = GVListEmployees1.FooterRow.FindControl("lblTotalCdHRA") as Label;
                        lblTotalCdHRA.Text = Math.Round(totalCdHRA).ToString();

                        if (totalCdHRA > 0)
                        {
                            GVListEmployees1.Columns[24].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[24].Visible = false;

                        }
                        Label lblTotalCdCCA = GVListEmployees1.FooterRow.FindControl("lblTotalCdCCA") as Label;
                        lblTotalCdCCA.Text = Math.Round(totalCdCCA).ToString();
                        if (totalCdCCA > 0)
                        {
                            GVListEmployees1.Columns[25].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[25].Visible = false;

                        }
                        Label lblTotalCdConveyance = GVListEmployees1.FooterRow.FindControl("lblTotalCdConveyance") as Label;
                        lblTotalCdConveyance.Text = Math.Round(totalCdConveyance).ToString();
                        if (totalCdConveyance > 0)
                        {
                            GVListEmployees1.Columns[26].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[26].Visible = false;

                        }
                        Label lblTotalcdWA = GVListEmployees1.FooterRow.FindControl("lblTotalcdWA") as Label;
                        lblTotalcdWA.Text = Math.Round(totalCdWA).ToString();
                        if (totalCdWA > 0)
                        {
                            GVListEmployees1.Columns[27].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[27].Visible = false;

                        }
                        Label lblTotalcdNfhs = GVListEmployees1.FooterRow.FindControl("lblTotalcdNfhs") as Label;
                        lblTotalcdNfhs.Text = Math.Round(totalCdNfhs).ToString();
                        if (totalCdNfhs > 0)
                        {
                            GVListEmployees1.Columns[28].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[28].Visible = false;

                        }
                        Label lblTotalcdrc = GVListEmployees1.FooterRow.FindControl("lblTotalcdrc") as Label;
                        lblTotalcdrc.Text = Math.Round(totalCdrc).ToString();
                        if (totalCdrc > 0)
                        {
                            GVListEmployees1.Columns[29].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[29].Visible = false;

                        }
                        Label lblTotalcdcs = GVListEmployees1.FooterRow.FindControl("lblTotalcdcs") as Label;
                        lblTotalcdcs.Text = Math.Round(totalCdcs).ToString();
                        if (totalCdcs > 0)
                        {
                            GVListEmployees1.Columns[30].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[30].Visible = false;

                        }
                        Label lbltTotalcdAddlAmount = GVListEmployees1.FooterRow.FindControl("lbltTotalcdAddlAmount") as Label;
                        lbltTotalcdAddlAmount.Text = Math.Round(totalCdAddlAmount).ToString();
                        if (totalCdAddlAmount > 0)
                        {
                            GVListEmployees1.Columns[31].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[31].Visible = false;

                        }
                        Label lblTotalcdFoodAllowance = GVListEmployees1.FooterRow.FindControl("lblTotalcdFoodAllowance") as Label;
                        lblTotalcdFoodAllowance.Text = Math.Round(totalCdFoodAllw).ToString();
                        if (totalCdFoodAllw > 0)
                        {
                            GVListEmployees1.Columns[32].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[32].Visible = false;

                        }
                        Label lblTotalcdWOAmount = GVListEmployees1.FooterRow.FindControl("lblTotalcdWOAmount") as Label;
                        lblTotalcdWOAmount.Text = Math.Round(totalCdWOAmt).ToString();
                        if (totalCdWOAmt > 0)
                        {
                            GVListEmployees1.Columns[33].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[33].Visible = false;

                        }
                        Label lblTotalcdNhsAmount = GVListEmployees1.FooterRow.FindControl("lblTotalcdNhsAmount") as Label;
                        lblTotalcdNhsAmount.Text = Math.Round(totalCdNHsAmt).ToString();
                        if (totalCdNHsAmt > 0)
                        {
                            GVListEmployees1.Columns[34].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[34].Visible = false;

                        }
                        Label lblTotalcdmedicalallowance = GVListEmployees1.FooterRow.FindControl("lblTotalcdmedicalallowance") as Label;
                        lblTotalcdmedicalallowance.Text = Math.Round(totalCdMedicalReimbursement).ToString();
                        if (totalCdMedicalReimbursement > 0)
                        {
                            GVListEmployees1.Columns[35].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[35].Visible = false;


                        }
                        Label lblTotalcdSpecialAllowance = GVListEmployees1.FooterRow.FindControl("lblTotalcdSpecialAllowance") as Label;
                        lblTotalcdSpecialAllowance.Text = Math.Round(totalCdSpecialAllW).ToString();
                        if (totalCdSpecialAllW > 0)
                        {
                            GVListEmployees1.Columns[36].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[36].Visible = false;

                        }
                        Label lblTotalcdTravelAllw = GVListEmployees1.FooterRow.FindControl("lblTotalcdTravelAllw") as Label;
                        lblTotalcdTravelAllw.Text = Math.Round(totalCdTravelAllw).ToString();
                        if (totalCdTravelAllw > 0)
                        {
                            GVListEmployees1.Columns[37].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[37].Visible = false;

                        }
                        Label lblTotalcdMobileAllowance = GVListEmployees1.FooterRow.FindControl("lblTotalcdMobileAllowance") as Label;
                        lblTotalcdMobileAllowance.Text = Math.Round(totalCdMobileAllw).ToString();
                        if (totalCdMobileAllw > 0)
                        {
                            GVListEmployees1.Columns[38].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[38].Visible = false;

                        }
                        Label lblTotalcdPerformanceAllw = GVListEmployees1.FooterRow.FindControl("lblTotalcdPerformanceAllw") as Label;
                        lblTotalcdPerformanceAllw.Text = Math.Round(totalCdPerformanceAllw).ToString();
                        if (totalCdPerformanceAllw > 0)
                        {
                            GVListEmployees1.Columns[39].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[39].Visible = false;

                        }
                        Label lblTotalcdLeaveEncashAmt = GVListEmployees1.FooterRow.FindControl("lblTotalcdLeaveEncashAmt") as Label;
                        lblTotalcdLeaveEncashAmt.Text = Math.Round(totalCdLW).ToString();
                        if (totalCdLW > 0)
                        {
                            GVListEmployees1.Columns[40].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[40].Visible = false;

                        }
                        Label lblTotalcdNpotsAmount = GVListEmployees1.FooterRow.FindControl("lblTotalcdNpotsAmount") as Label;
                        lblTotalcdNpotsAmount.Text = Math.Round(totalCdNPOTsAmt).ToString();
                        if (totalCdNPOTsAmt > 0)
                        {
                            GVListEmployees1.Columns[41].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[41].Visible = false;

                        }
                        Label lblTotalcdIncentivs = GVListEmployees1.FooterRow.FindControl("lblTotalcdIncentivs") as Label;
                        lblTotalcdIncentivs.Text = Math.Round(totalCdIncentivs).ToString();
                        if (totalCdIncentivs > 0)
                        {
                            GVListEmployees1.Columns[42].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[42].Visible = false;

                        }
                        Label lblTotalcdBonus = GVListEmployees1.FooterRow.FindControl("lblTotalcdBonus") as Label;
                        lblTotalcdBonus.Text = Math.Round(totalCdBonus).ToString();
                        if (totalCdBonus > 0)
                        {
                            GVListEmployees1.Columns[43].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[43].Visible = false;

                        }
                        Label lblTotalcdGratuity = GVListEmployees1.FooterRow.FindControl("lblTotalcdGratuity") as Label;
                        lblTotalcdGratuity.Text = Math.Round(totalCdGratuity).ToString();
                        if (totalCdGratuity > 0)
                        {
                            GVListEmployees1.Columns[44].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[44].Visible = false;

                        }
                        Label lblTotalcdOA = GVListEmployees1.FooterRow.FindControl("lblTotalcdOA") as Label;
                        lblTotalcdOA.Text = Math.Round(totalCdOA).ToString();
                        if (totalCdOA > 0)
                        {
                            GVListEmployees1.Columns[45].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[45].Visible = false;

                        }

                        Label lblTotalcdOTAmount = GVListEmployees1.FooterRow.FindControl("lblTotalcdOTAmount") as Label;
                        lblTotalcdOTAmount.Text = Math.Round(totalcdOtRate).ToString();
                        if (totalcdOtRate > 0)
                        {
                            GVListEmployees1.Columns[46].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[46].Visible = false;

                        }
                        Label lblTotalcdServiceWeightage = GVListEmployees1.FooterRow.FindControl("lblTotalcdServiceWeightage") as Label;
                        lblTotalcdServiceWeightage.Text = Math.Round(totalCdServiceWeightage).ToString();
                        if (totalCdServiceWeightage > 0)
                        {
                            GVListEmployees1.Columns[47].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[47].Visible = false;

                        }
                        Label lblTotalcdRankAllowance = GVListEmployees1.FooterRow.FindControl("lblTotalcdRankAllowance") as Label;
                        lblTotalcdRankAllowance.Text = Math.Round(totalcdRankAllowance).ToString();
                        if (totalcdRankAllowance > 0)
                        {
                            GVListEmployees1.Columns[48].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[48].Visible = false;

                        }
                        Label lblTotalcdAttBonus = GVListEmployees1.FooterRow.FindControl("lblTotalcdAttBonus") as Label;
                        lblTotalcdAttBonus.Text = Math.Round(totalCdAttBonus).ToString();
                        if (totalCdAttBonus > 0)
                        {
                            GVListEmployees1.Columns[49].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[49].Visible = false;

                        }
                        Label lblTotalcdNightAllw = GVListEmployees1.FooterRow.FindControl("lblTotalcdNightAllw") as Label;
                        lblTotalcdNightAllw.Text = Math.Round(totalCdNightAllw).ToString();
                        if (totalCdNightAllw > 0)
                        {
                            GVListEmployees1.Columns[50].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[50].Visible = false;

                        }
                        Label lblTotalfixedADDL4HR = GVListEmployees1.FooterRow.FindControl("lblTotalfixedADDL4HR") as Label;
                        lblTotalfixedADDL4HR.Text = Math.Round(totalfixedADDL4HR).ToString();
                        if (totalfixedADDL4HR > 0)
                        {
                            GVListEmployees1.Columns[51].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[51].Visible = false;

                        }
                        Label lblTotalfixedQTRALLOW = GVListEmployees1.FooterRow.FindControl("lblTotalfixedQTRALLOW") as Label;
                        lblTotalfixedQTRALLOW.Text = Math.Round(totalfixedQTRALLOW).ToString();
                        if (totalfixedQTRALLOW > 0)
                        {
                            GVListEmployees1.Columns[52].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[52].Visible = false;

                        }
                        Label lblTotalfixedRELALLOW = GVListEmployees1.FooterRow.FindControl("lblTotalfixedRELALLOW") as Label;
                        lblTotalfixedRELALLOW.Text = Math.Round(totalfixedRELALLOW).ToString();
                        if (totalfixedRELALLOW > 0)
                        {
                            GVListEmployees1.Columns[53].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[53].Visible = false;

                        }
                        Label lblTotalfixedSITEALLOW = GVListEmployees1.FooterRow.FindControl("lblTotalfixedSITEALLOW") as Label;
                        lblTotalfixedSITEALLOW.Text = Math.Round(totalfixedSITEALLOW).ToString();
                        if (totalfixedSITEALLOW > 0)
                        {
                            GVListEmployees1.Columns[54].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[54].Visible = false;

                        }

                        Label lblTotalfixedGunAllw = GVListEmployees1.FooterRow.FindControl("lblTotalfixedGunAllw") as Label;
                        lblTotalfixedGunAllw.Text = Math.Round(totalfixedGunAllw).ToString();
                        if (totalfixedGunAllw > 0)
                        {
                            GVListEmployees1.Columns[56].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[56].Visible = false;

                        }
                        Label lblTotalfixedFireAllw = GVListEmployees1.FooterRow.FindControl("lblTotalfixedFireAllw") as Label;
                        lblTotalfixedFireAllw.Text = Math.Round(totalfixedFireAllw).ToString();
                        if (totalfixedFireAllw > 0)
                        {
                            GVListEmployees1.Columns[56].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[56].Visible = false;

                        }

                        Label lblTotalfixedTelephoneAllw = GVListEmployees1.FooterRow.FindControl("lblTotalfixedTelephoneAllw") as Label;
                        lblTotalfixedTelephoneAllw.Text = Math.Round(totalfixedTelephoneAllw).ToString();
                        if (totalfixedTelephoneAllw > 0)
                        {
                            GVListEmployees1.Columns[57].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[57].Visible = false;

                        }
                        Label lblTotalfixedReimbursement = GVListEmployees1.FooterRow.FindControl("lblTotalfixedReimbursement") as Label;
                        lblTotalfixedReimbursement.Text = Math.Round(totalfixedReimbursement).ToString();
                        if (totalfixedReimbursement > 0)
                        {
                            GVListEmployees1.Columns[58].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[58].Visible = false;

                        }

                        Label lblTotalfixedHardshipAllw = GVListEmployees1.FooterRow.FindControl("lblTotalfixedHardshipAllw") as Label;
                        lblTotalfixedHardshipAllw.Text = Math.Round(totalfixedHardshipAllw).ToString();
                        if (totalfixedHardshipAllw > 0)
                        {
                            GVListEmployees1.Columns[59].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[59].Visible = false;

                        }
                        Label lblTotalfixedPaidHolidayAllw = GVListEmployees1.FooterRow.FindControl("lblTotalfixedPaidHolidayAllw") as Label;
                        lblTotalfixedPaidHolidayAllw.Text = Math.Round(totalfixedPaidHolidayAllw).ToString();
                        if (totalfixedPaidHolidayAllw > 0)
                        {
                            GVListEmployees1.Columns[60].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[60].Visible = false;

                        }

                        Label lbltotalcdTempGross = GVListEmployees1.FooterRow.FindControl("lbltotalTempGross") as Label;
                        lbltotalcdTempGross.Text = Math.Round(totalCdTempGross).ToString();

                        if (totalCdTempGross > 0)
                        {
                            GVListEmployees1.Columns[61].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[61].Visible = false;

                        }


                        //Label lblTotalDuties = GVListEmployees.FooterRow.FindControl("lblTotalDuties") as Label;
                        //lblTotalDuties.Text = Math.Round(totalDuties).ToString();
                        //if (totalDuties > 0)
                        //{
                        //    GVListEmployees.Columns[58].Visible = true;
                        //}
                        //else
                        //{
                        //    GVListEmployees.Columns[58].Visible = false;

                        //}
                        Label lblTotalOts = GVListEmployees1.FooterRow.FindControl("lblTotalOts") as Label;
                        lblTotalOts.Text = Math.Round(totalOts).ToString();
                        if (totalOts > 0)
                        {
                            GVListEmployees1.Columns[62].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[62].Visible = false;

                        }


                        Label lblTotalwos = GVListEmployees1.FooterRow.FindControl("lblTotalwos") as Label;
                        lblTotalwos.Text = Math.Round(totalwo).ToString();

                        if (totalwo > 0)
                        {
                            GVListEmployees1.Columns[63].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[63].Visible = false;

                        }

                        Label lblTotalNhs = GVListEmployees1.FooterRow.FindControl("lblTotalNhs") as Label;
                        lblTotalNhs.Text = Math.Round(totalnhs).ToString();

                        if (totalnhs > 0)
                        {
                            GVListEmployees1.Columns[64].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[64].Visible = false;

                        }
                        Label lblTotalNpots = GVListEmployees1.FooterRow.FindControl("lblTotalNpots") as Label;
                        lblTotalNpots.Text = Math.Round(totalnpots).ToString();

                        if (totalnpots > 0)
                        {
                            GVListEmployees1.Columns[65].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[65].Visible = false;

                        }

                        Label lblTotalBasic = GVListEmployees1.FooterRow.FindControl("lblTotalBasic") as Label;
                        lblTotalBasic.Text = Math.Round(totalBasic).ToString();
                        if (totalBasic > 0)
                        {
                            GVListEmployees1.Columns[66].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[66].Visible = false;

                        }
                        Label lblTotalDA = GVListEmployees1.FooterRow.FindControl("lblTotalDA") as Label;
                        lblTotalDA.Text = Math.Round(totalDA).ToString();

                        if (totalDA > 0)
                        {
                            GVListEmployees1.Columns[67].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[67].Visible = false;

                        }

                        Label lblTotalHRA = GVListEmployees1.FooterRow.FindControl("lblTotalHRA") as Label;
                        lblTotalHRA.Text = Math.Round(totalHRA).ToString();

                        if (totalHRA > 0)
                        {
                            GVListEmployees1.Columns[68].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[68].Visible = false;

                        }

                        Label lblTotalCCA = GVListEmployees1.FooterRow.FindControl("lblTotalCCA") as Label;
                        lblTotalCCA.Text = Math.Round(totalCCA).ToString();

                        if (totalCCA > 0)
                        {
                            GVListEmployees1.Columns[69].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[69].Visible = false;

                        }

                        Label lblTotalConveyance = GVListEmployees1.FooterRow.FindControl("lblTotalConveyance") as Label;
                        lblTotalConveyance.Text = Math.Round(totalConveyance).ToString();

                        if (totalConveyance > 0)
                        {
                            GVListEmployees1.Columns[70].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[70].Visible = false;

                        }

                        Label lblTotalWA = GVListEmployees1.FooterRow.FindControl("lblTotalWA") as Label;
                        lblTotalWA.Text = Math.Round(totalWA).ToString();

                        if (totalWA > 0)
                        {
                            GVListEmployees1.Columns[71].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[71].Visible = false;

                        }

                        Label lblTotalNfhs = GVListEmployees1.FooterRow.FindControl("lblTotalNfhs") as Label;
                        lblTotalNfhs.Text = Math.Round(totalnfhs).ToString();

                        if (totalnfhs > 0)
                        {
                            GVListEmployees1.Columns[72].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[72].Visible = false;

                        }

                        Label lblTotalrc = GVListEmployees1.FooterRow.FindControl("lblTotalrc") as Label;
                        lblTotalrc.Text = Math.Round(totalRC).ToString();

                        if (totalRC > 0)
                        {
                            GVListEmployees1.Columns[73].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[73].Visible = false;

                        }

                        Label lblTotalcs = GVListEmployees1.FooterRow.FindControl("lblTotalcs") as Label;
                        lblTotalcs.Text = Math.Round(totalCS).ToString();

                        if (totalCS > 0)
                        {
                            GVListEmployees1.Columns[74].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[74].Visible = false;

                        }
                        Label lbltTotalAddlAmount = GVListEmployees1.FooterRow.FindControl("lbltTotalAddlAmount") as Label;
                        lbltTotalAddlAmount.Text = Math.Round(totalAddlAmount).ToString();

                        if (totalAddlAmount > 0)
                        {
                            GVListEmployees1.Columns[75].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[75].Visible = false;

                        }
                        Label lblTotalFoodAllowance = GVListEmployees1.FooterRow.FindControl("lblTotalFoodAllowance") as Label;
                        lblTotalFoodAllowance.Text = Math.Round(totalFoodAllowance).ToString();

                        if (totalFoodAllowance > 0)
                        {
                            GVListEmployees1.Columns[77].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[77].Visible = false;
                        }

                        Label lblTotalWOAmount = GVListEmployees1.FooterRow.FindControl("lblTotalWOAmount") as Label;
                        lblTotalWOAmount.Text = Math.Round(totalWoAmt).ToString();

                        if (totalWoAmt > 0)
                        {
                            GVListEmployees1.Columns[77].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[77].Visible = false;

                        }

                        Label lblTotalNhsAmount = GVListEmployees1.FooterRow.FindControl("lblTotalNhsAmount") as Label;
                        lblTotalNhsAmount.Text = Math.Round(totalNhsAmt).ToString();

                        if (totalNhsAmt > 0)
                        {
                            GVListEmployees1.Columns[78].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[78].Visible = false;

                        }

                        Label lblTotalmedicalallowance = GVListEmployees1.FooterRow.FindControl("lblTotalmedicalallowance") as Label;
                        lblTotalmedicalallowance.Text = Math.Round(totalmedicalallowance).ToString();

                        if (totalmedicalallowance > 0)
                        {
                            GVListEmployees1.Columns[79].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[79].Visible = false;
                        }


                        Label lblTotalSpecialAllowance = GVListEmployees1.FooterRow.FindControl("lblTotalSpecialAllowance") as Label;
                        lblTotalSpecialAllowance.Text = Math.Round(totalSpecialAllowance).ToString();

                        if (totalSpecialAllowance > 0)
                        {
                            GVListEmployees1.Columns[80].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[80].Visible = false;

                        }

                        Label lblTotalTravelAllw = GVListEmployees1.FooterRow.FindControl("lblTotalTravelAllw") as Label;
                        lblTotalTravelAllw.Text = Math.Round(totalTravelAllw).ToString();

                        if (totalTravelAllw > 0)
                        {
                            GVListEmployees1.Columns[81].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[81].Visible = false;
                        }

                        Label lblTotalMobileAllowance = GVListEmployees1.FooterRow.FindControl("lblTotalMobileAllowance") as Label;
                        lblTotalMobileAllowance.Text = Math.Round(totalMobileAllowance).ToString();

                        if (totalMobileAllowance > 0)
                        {
                            GVListEmployees1.Columns[82].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[82].Visible = false;

                        }
                        Label lblTotalPerformanceAllw = GVListEmployees1.FooterRow.FindControl("lblTotalPerformanceAllw") as Label;
                        lblTotalPerformanceAllw.Text = Math.Round(totalPerformanceAllw).ToString();
                        if (totalPerformanceAllw > 0)
                        {
                            GVListEmployees1.Columns[83].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[83].Visible = false;

                        }

                        Label lblTotalLeaveEncashAmt = GVListEmployees1.FooterRow.FindControl("lblTotalLeaveEncashAmt") as Label;
                        lblTotalLeaveEncashAmt.Text = Math.Round(totalLeaveEncashAmt).ToString();

                        if (totalLeaveEncashAmt > 0)
                        {
                            GVListEmployees1.Columns[84].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[84].Visible = false;

                        }
                        Label lblTotalNpotsAmount = GVListEmployees1.FooterRow.FindControl("lblTotalNpotsAmount") as Label;
                        lblTotalNpotsAmount.Text = Math.Round(totalNpotsAmt).ToString();

                        if (totalNpotsAmt > 0)
                        {
                            GVListEmployees1.Columns[85].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[85].Visible = false;

                        }


                        Label lblTotalIncentivs = GVListEmployees1.FooterRow.FindControl("lblTotalIncentivs") as Label;
                        lblTotalIncentivs.Text = Math.Round(totalIncentivs).ToString();
                        if (totalIncentivs > 0)
                        {
                            GVListEmployees1.Columns[86].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[86].Visible = false;

                        }
                        Label lblTotalBonus = GVListEmployees1.FooterRow.FindControl("lblTotalBonus") as Label;
                        lblTotalBonus.Text = Math.Round(totalBonus).ToString();
                        if (totalBonus > 0)
                        {
                            GVListEmployees1.Columns[87].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[87].Visible = false;

                        }

                        Label lblTotalGratuity = GVListEmployees1.FooterRow.FindControl("lblTotalGratuity") as Label;
                        lblTotalGratuity.Text = Math.Round(totalGratuity).ToString();

                        if (totalGratuity > 0)
                        {
                            GVListEmployees1.Columns[88].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[88].Visible = false;

                        }
                        Label lblTotalOA = GVListEmployees1.FooterRow.FindControl("lblTotalOA") as Label;
                        lblTotalOA.Text = Math.Round(totalOA).ToString();

                        if (totalOA > 0)
                        {
                            GVListEmployees1.Columns[89].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[89].Visible = false;

                        }

                        Label lblTotalServiceWeightage = GVListEmployees1.FooterRow.FindControl("lblTotalServiceWeightage") as Label;
                        lblTotalServiceWeightage.Text = Math.Round(totalServiceWeightage).ToString();

                        if (totalServiceWeightage > 0)
                        {
                            GVListEmployees1.Columns[90].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[90].Visible = false;

                        }
                        Label lblTotalArrears = GVListEmployees1.FooterRow.FindControl("lblTotalArrears") as Label;
                        lblTotalArrears.Text = Math.Round(totalArrears).ToString();

                        if (totalArrears > 0)
                        {
                            GVListEmployees1.Columns[91].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[91].Visible = false;

                        }

                        Label lblTotalAttBonus = GVListEmployees1.FooterRow.FindControl("lblTotalAttBonus") as Label;
                        lblTotalAttBonus.Text = Math.Round(totalAttBonus).ToString();

                        if (totalAttBonus > 0)
                        {
                            GVListEmployees1.Columns[92].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[92].Visible = false;

                        }
                        Label lblTotalNightAllw = GVListEmployees1.FooterRow.FindControl("lblTotalNightAllw") as Label;
                        lblTotalNightAllw.Text = Math.Round(totalNightAllw).ToString();

                        if (totalNightAllw > 0)
                        {
                            GVListEmployees1.Columns[93].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[93].Visible = false;

                        }
                        Label lblTotalADDL4HR = GVListEmployees1.FooterRow.FindControl("lblTotalADDL4HR") as Label;
                        lblTotalADDL4HR.Text = Math.Round(totalADDL4HR).ToString();

                        if (totalADDL4HR > 0)
                        {
                            GVListEmployees1.Columns[94].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[94].Visible = false;

                        }
                        Label lblTotalQTRALLOW = GVListEmployees1.FooterRow.FindControl("lblTotalQTRALLOW") as Label;
                        lblTotalQTRALLOW.Text = Math.Round(totalQTRALLOW).ToString();

                        if (totalQTRALLOW > 0)
                        {
                            GVListEmployees1.Columns[95].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[95].Visible = false;

                        }

                        Label lblTotalRELALLOW = GVListEmployees1.FooterRow.FindControl("lblTotalRELALLOW") as Label;
                        lblTotalRELALLOW.Text = Math.Round(totalRELALLOW).ToString();
                        if (totalRELALLOW > 0)
                        {
                            GVListEmployees1.Columns[96].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[96].Visible = false;

                        }
                        Label lblTotalSITEALLOW = GVListEmployees1.FooterRow.FindControl("lblTotalSITEALLOW") as Label;
                        lblTotalSITEALLOW.Text = Math.Round(totalSITEALLOW).ToString();
                        if (totalSITEALLOW > 0)
                        {
                            GVListEmployees1.Columns[97].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[97].Visible = false;

                        }

                        Label lblTotalGunAllw = GVListEmployees1.FooterRow.FindControl("lblTotalGunAllw") as Label;
                        lblTotalGunAllw.Text = Math.Round(totalGunAllw).ToString();
                        if (totalGunAllw > 0)
                        {
                            GVListEmployees1.Columns[98].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[98].Visible = false;

                        }
                        Label lblTotalFireAllw = GVListEmployees1.FooterRow.FindControl("lblTotalFireAllw") as Label;
                        lblTotalFireAllw.Text = Math.Round(totalFireAllw).ToString();
                        if (totalFireAllw > 0)
                        {
                            GVListEmployees1.Columns[99].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[99].Visible = false;

                        }

                        Label lblTotalTelephoneAllw = GVListEmployees1.FooterRow.FindControl("lblTotalTelephoneAllw") as Label;
                        lblTotalTelephoneAllw.Text = Math.Round(totalTelephoneAllw).ToString();
                        if (totalTelephoneAllw > 0)
                        {
                            GVListEmployees1.Columns[100].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[100].Visible = false;

                        }
                        Label lblTotalReimbursement = GVListEmployees1.FooterRow.FindControl("lblTotalReimbursement") as Label;
                        lblTotalReimbursement.Text = Math.Round(totalReimbursement).ToString();
                        if (totalReimbursement > 0)
                        {
                            GVListEmployees1.Columns[101].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[101].Visible = false;

                        }

                        Label lblTotalHardshipAllw = GVListEmployees1.FooterRow.FindControl("lblTotalHardshipAllw") as Label;
                        lblTotalHardshipAllw.Text = Math.Round(totalHardshipAllw).ToString();
                        if (totalHardshipAllw > 0)
                        {
                            GVListEmployees1.Columns[102].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[102].Visible = false;

                        }
                        Label lblTotalPaidHolidayAllw = GVListEmployees1.FooterRow.FindControl("lblTotalPaidHolidayAllw") as Label;
                        lblTotalPaidHolidayAllw.Text = Math.Round(totalPaidHolidayAllw).ToString();
                        if (totalPaidHolidayAllw > 0)
                        {
                            GVListEmployees1.Columns[103].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[103].Visible = false;

                        }

                        Label lblTotalRankAllownace = GVListEmployees1.FooterRow.FindControl("lblTotalRankAllownace") as Label;
                        lblTotalRankAllownace.Text = Math.Round(totalRankAllowance).ToString();

                        if (totalRankAllowance > 0)
                        {
                            GVListEmployees1.Columns[104].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[104].Visible = false;

                        }

                        Label lblTotalGross = GVListEmployees1.FooterRow.FindControl("lblTotalGross") as Label;
                        lblTotalGross.Text = Math.Round(totalGrass).ToString();
                        if (totalGrass > 0)
                        {
                            GVListEmployees1.Columns[105].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[105].Visible = false;

                        }

                        Label lblTotalPF = GVListEmployees1.FooterRow.FindControl("lblTotalPF") as Label;
                        lblTotalPF.Text = Math.Round(totalPF).ToString();
                        if (totalPF > 0)
                        {
                            GVListEmployees1.Columns[106].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[106].Visible = false;

                        }

                        Label lblTotalESI = GVListEmployees1.FooterRow.FindControl("lblTotalESI") as Label;
                        lblTotalESI.Text = Math.Round(totalESI).ToString();
                        if (totalESI > 0)
                        {
                            GVListEmployees1.Columns[107].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[107].Visible = false;

                        }

                        Label lblTotalProfTax = GVListEmployees1.FooterRow.FindControl("lblTotalProfTax") as Label;
                        lblTotalProfTax.Text = Math.Round(totalProfTax).ToString();
                        if (totalProfTax > 0)
                        {
                            GVListEmployees1.Columns[108].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[108].Visible = false;

                        }


                        Label lblTotalsaladv = GVListEmployees1.FooterRow.FindControl("lblTotalsaladv") as Label;
                        lblTotalsaladv.Text = Math.Round(totalSalAdv).ToString();

                        if (totalSalAdv > 0)
                        {
                            GVListEmployees1.Columns[109].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[109].Visible = false;

                        }

                        Label lblTotaladvded = GVListEmployees1.FooterRow.FindControl("lblTotaladvded") as Label;
                        lblTotaladvded.Text = Math.Round(totalAdvDed).ToString();
                        if (totalAdvDed > 0)
                        {
                            GVListEmployees1.Columns[110].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[110].Visible = false;

                        }
                        Label lblTotalwed = GVListEmployees1.FooterRow.FindControl("lblTotalwed") as Label;
                        lblTotalwed.Text = Math.Round(totalwed).ToString();
                        if (totalwed > 0)
                        {
                            GVListEmployees1.Columns[111].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[111].Visible = false;

                        }

                        Label lblTotalUniformDed = GVListEmployees1.FooterRow.FindControl("lblTotalUniformDed") as Label;
                        lblTotalUniformDed.Text = Math.Round(totalUniformDed).ToString();

                        if (totalUniformDed > 0)
                        {
                            GVListEmployees1.Columns[112].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[112].Visible = false;

                        }

                        Label lblTotalOtherDed = GVListEmployees1.FooterRow.FindControl("lblTotalOtherDed") as Label;
                        lblTotalOtherDed.Text = Math.Round(totalOtherDed).ToString();

                        if (totalOtherDed > 0)
                        {
                            GVListEmployees1.Columns[113].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[113].Visible = false;

                        }

                        Label lblTotaltotalloanded = GVListEmployees1.FooterRow.FindControl("lblTotaltotalloanded") as Label;
                        lblTotaltotalloanded.Text = Math.Round(totalloanded).ToString();

                        if (totalloanded > 0)
                        {
                            GVListEmployees1.Columns[114].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[114].Visible = false;

                        }

                        Label lblTotalcantadv = GVListEmployees1.FooterRow.FindControl("lblTotalcantadv") as Label;
                        lblTotalcantadv.Text = Math.Round(totalCanteenAdv).ToString();

                        if (totalCanteenAdv > 0)
                        {
                            GVListEmployees1.Columns[115].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[115].Visible = false;

                        }
                        Label lblTotalSeepDed = GVListEmployees1.FooterRow.FindControl("lblTotalSeepDed") as Label;
                        lblTotalSeepDed.Text = Math.Round(totalSeepDed).ToString();

                        if (totalSeepDed > 0)
                        {
                            GVListEmployees1.Columns[116].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[116].Visible = false;

                        }


                        Label lblTotalGeneralDed = GVListEmployees1.FooterRow.FindControl("lblTotalGeneralDed") as Label;
                        lblTotalGeneralDed.Text = Math.Round(totalGenDed).ToString();


                        if (totalGenDed > 0)
                        {
                            GVListEmployees1.Columns[117].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[117].Visible = false;

                        }

                        Label lblTotalowf = GVListEmployees1.FooterRow.FindControl("lblTotalowf") as Label;
                        lblTotalowf.Text = Math.Round(totalOWF).ToString();

                        if (totalOWF > 0)
                        {

                            GVListEmployees1.Columns[118].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[118].Visible = false;

                        }

                        Label lblTotalPenalty = GVListEmployees1.FooterRow.FindControl("lblTotalPenalty") as Label;
                        lblTotalPenalty.Text = Math.Round(totalPenalty).ToString();

                        if (totalPenalty > 0)
                        {
                            GVListEmployees1.Columns[119].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[119].Visible = false;

                        }


                        Label lblTotalRentDed = GVListEmployees1.FooterRow.FindControl("lblTotalRentDed") as Label;
                        lblTotalRentDed.Text = Math.Round(totalRentDed).ToString();

                        if (totalRentDed > 0)
                        {
                            GVListEmployees1.Columns[120].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[120].Visible = false;

                        }


                        Label lblTotalMedicalDed = GVListEmployees1.FooterRow.FindControl("lblTotalMedicalDed") as Label;
                        lblTotalMedicalDed.Text = Math.Round(totalMedicalDed).ToString();

                        if (totalMedicalDed > 0)
                        {
                            GVListEmployees1.Columns[121].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[121].Visible = false;

                        }

                        Label lblTotalMLWFDed = GVListEmployees1.FooterRow.FindControl("lblTotalMLWFDed") as Label;
                        lblTotalMLWFDed.Text = Math.Round(totalMLWFDed).ToString();

                        if (totalMLWFDed > 0)
                        {
                            GVListEmployees1.Columns[122].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[122].Visible = false;

                        }

                        Label lblTotalFoodDed = GVListEmployees1.FooterRow.FindControl("lblTotalFoodDed") as Label;
                        lblTotalFoodDed.Text = Math.Round(totalFoodDed).ToString();

                        if (totalFoodDed > 0)
                        {
                            GVListEmployees1.Columns[123].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[123].Visible = false;

                        }

                        Label lblTotalElectricityDed = GVListEmployees1.FooterRow.FindControl("lblTotalElectricityDed") as Label;
                        lblTotalElectricityDed.Text = Math.Round(totalElectricityDed).ToString();

                        if (totalElectricityDed > 0)
                        {
                            GVListEmployees1.Columns[124].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[124].Visible = false;
                        }

                        Label lblTotalTransportDed = GVListEmployees1.FooterRow.FindControl("lblTotalTransportDed") as Label;
                        lblTotalTransportDed.Text = Math.Round(totalTransportDed).ToString();

                        if (totalTransportDed > 0)
                        {
                            GVListEmployees1.Columns[125].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[125].Visible = false;
                        }
                        Label lblTotalDced = GVListEmployees1.FooterRow.FindControl("lblTotalDced") as Label;
                        lblTotalDced.Text = Math.Round(totalDced).ToString();

                        if (totalDced > 0)
                        {
                            GVListEmployees1.Columns[126].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[126].Visible = false;
                        }

                        Label lblTotalLeaveDed = GVListEmployees1.FooterRow.FindControl("lblTotalLeaveDed") as Label;
                        lblTotalLeaveDed.Text = Math.Round(totalLeaveDed).ToString();

                        if (totalLeaveDed > 0)
                        {
                            GVListEmployees1.Columns[127].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[127].Visible = false;
                        }

                        Label lblTotalLicenseDed = GVListEmployees1.FooterRow.FindControl("lblTotalLicenseDed") as Label;
                        lblTotalLicenseDed.Text = Math.Round(totalLicenseDed).ToString();

                        if (totalLicenseDed > 0)
                        {
                            GVListEmployees1.Columns[128].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[128].Visible = false;
                        }


                        ///

                        Label lblTotalAdv4Ded = GVListEmployees1.FooterRow.FindControl("lblTotalAdv4Ded") as Label;
                        lblTotalAdv4Ded.Text = Math.Round(totalAdv4Ded).ToString();
                        if (totalAdv4Ded > 0)
                        {
                            GVListEmployees1.Columns[129].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[129].Visible = false;

                        }

                        Label lblTotalNightRoundDed = GVListEmployees1.FooterRow.FindControl("lblTotalNightRoundDed") as Label;
                        lblTotalNightRoundDed.Text = Math.Round(totalNightRoundDed).ToString();
                        if (totalNightRoundDed > 0)
                        {
                            GVListEmployees1.Columns[130].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[130].Visible = false;

                        }

                        Label lblTotalManpowerMobDed = GVListEmployees1.FooterRow.FindControl("lblTotalManpowerMobDed") as Label;
                        lblTotalManpowerMobDed.Text = Math.Round(totalManpowerMobDed).ToString();
                        if (totalManpowerMobDed > 0)
                        {
                            GVListEmployees1.Columns[131].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[131].Visible = false;

                        }


                        Label lblTotalMobileusageDed = GVListEmployees1.FooterRow.FindControl("lblTotalMobileusageDed") as Label;
                        lblTotalMobileusageDed.Text = Math.Round(totalMobileusageDed).ToString();
                        if (totalMobileusageDed > 0)
                        {
                            GVListEmployees1.Columns[132].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[132].Visible = false;

                        }

                        Label lblTotalMediClaimDed = GVListEmployees1.FooterRow.FindControl("lblTotalMediClaimDed") as Label;
                        lblTotalMediClaimDed.Text = Math.Round(totalMediClaimDed).ToString();
                        if (totalMediClaimDed > 0)
                        {
                            GVListEmployees1.Columns[133].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[133].Visible = false;

                        }


                        Label lblTotalCrisisDed = GVListEmployees1.FooterRow.FindControl("lblTotalCrisisDed") as Label;
                        lblTotalCrisisDed.Text = Math.Round(totalCrisisDed).ToString();
                        if (totalCrisisDed > 0)
                        {
                            GVListEmployees1.Columns[134].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[134].Visible = false;

                        }



                        Label lblTotalTelephoneBillDed = GVListEmployees1.FooterRow.FindControl("lblTotalTelephoneBillDed") as Label;
                        lblTotalTelephoneBillDed.Text = Math.Round(totalTelephoneBillDed).ToString();
                        if (totalTelephoneBillDed > 0)
                        {
                            GVListEmployees1.Columns[135].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[135].Visible = false;

                        }

                        Label lblTotalAdminChargesDed = GVListEmployees1.FooterRow.FindControl("lblTotalAdminChargesDed") as Label;
                        lblTotalAdminChargesDed.Text = Math.Round(totalAdmincharges).ToString();
                        if (totalAdmincharges > 0)
                        {
                            GVListEmployees1.Columns[136].Visible = true;

                        }
                        else
                        {
                            GVListEmployees1.Columns[136].Visible = false;

                        }

                        Label lblTotalRegistrationFee = GVListEmployees1.FooterRow.FindControl("lblTotalRegistrationFee") as Label;
                        lblTotalRegistrationFee.Text = Math.Round(totalRegistrationFee).ToString();
                        if (totalRegistrationFee > 0)
                        {
                            GVListEmployees1.Columns[137].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[137].Visible = false;

                        }


                        Label lblTotalDeductions = GVListEmployees1.FooterRow.FindControl("lblTotalDeductions") as Label;
                        lblTotalDeductions.Text = Math.Round(totalDed).ToString();

                        Label lblTotalAdvanceAgainstBonus = GVListEmployees1.FooterRow.FindControl("lblTotalAdvanceAgainstBonus") as Label;
                        lblTotalAdvanceAgainstBonus.Text = Math.Round(totalAdvBonus).ToString();
                        if (totalAdvBonus > 0)
                        {
                            GVListEmployees1.Columns[139].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[139].Visible = false;

                        }

                        Label lblTotalNetAmount = GVListEmployees1.FooterRow.FindControl("lblTotalNetAmount") as Label;
                        lblTotalNetAmount.Text = Math.Round(totalActualamount).ToString();

                        Label lblTotalOTEARNED = GVListEmployees1.FooterRow.FindControl("lblTotalOTEARNED") as Label;
                        lblTotalOTEARNED.Text = Math.Round(totalOTAmt).ToString();
                        if (totalOTAmt > 0)
                        {
                            GVListEmployees1.Columns[141].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[141].Visible = false;

                        }

                        Label lblTotalOTESIDeduction = GVListEmployees1.FooterRow.FindControl("lblTotalOTESIDeduction") as Label;
                        lblTotalOTESIDeduction.Text = Math.Round(totalotesi).ToString();
                        if (totalotesi > 0)
                        {
                            GVListEmployees1.Columns[142].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[142].Visible = false;

                        }


                        Label lblTotalNETOT = GVListEmployees1.FooterRow.FindControl("lblTotalNETOT") as Label;
                        lblTotalNETOT.Text = Math.Round(totalnetot).ToString();
                        if (totalnetot > 0)
                        {
                            GVListEmployees1.Columns[143].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[143].Visible = false;

                        }


                        Label lblTotalTOTALNETPAY = GVListEmployees1.FooterRow.FindControl("lblTotalTOTALNETPAY") as Label;
                        lblTotalTOTALNETPAY.Text = Math.Round(totalTOTALNETPAY).ToString();
                       


                        Label lblTotalPFEMPR = GVListEmployees1.FooterRow.FindControl("lblTotalPFEMPR") as Label;
                        lblTotalPFEMPR.Text = Math.Round(totalpfempr).ToString();
                       



                        Label lblTotalESIEMPR = GVListEmployees1.FooterRow.FindControl("lblTotalESIEMPR") as Label;
                        lblTotalESIEMPR.Text = Math.Round(totalesiempr).ToString();
                        


                        Label lblTotalESIONOT = GVListEmployees1.FooterRow.FindControl("lblTotalESIONOT") as Label;
                        lblTotalESIONOT.Text = Math.Round(totalotesi).ToString();
                       

                        Label lblTotalUniformCharges = GVListEmployees1.FooterRow.FindControl("lblTotalUniformCharges") as Label;
                        lblTotalUniformCharges.Text = Math.Round(totaluniformcharge).ToString();
                       


                        Label lblTotalTOTALCTC = GVListEmployees1.FooterRow.FindControl("lblTotalTOTALCTC") as Label;
                        lblTotalTOTALCTC.Text = Math.Round(totalCTC).ToString();


                        Label lblTotalSERVCHRG = GVListEmployees1.FooterRow.FindControl("lblTotalSERVCHRG") as Label;
                        lblTotalSERVCHRG.Text = Math.Round(totalservicecharge1).ToString();
                        if (totalservicecharge1 > 0)
                        {
                            GVListEmployees1.Columns[150].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[150].Visible = false;

                        }

                        Label lblTotalGST = GVListEmployees1.FooterRow.FindControl("lblTotalGST") as Label;
                        lblTotalGST.Text = Math.Round(totalgstper).ToString();
                        if (totalgstper > 0)
                        {
                            GVListEmployees1.Columns[151].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[151].Visible = false;

                        }



                        Label lblTotalGRANDTOTAL = GVListEmployees1.FooterRow.FindControl("lblTotalGRANDTOTAL") as Label;
                        lblTotalGRANDTOTAL.Text = Math.Round(totalgrandtotal).ToString();
                        if (totalgrandtotal > 0)
                        {
                            GVListEmployees1.Columns[152].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[152].Visible = false;

                        }

                        Label lblTotalAdvances = GVListEmployees1.FooterRow.FindControl("lblTotalAdvances") as Label;
                        lblTotalAdvances.Text = Math.Round(totalAdvances).ToString();
                        if (totalAdvances > 0)
                        {
                            GVListEmployees1.Columns[153].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[153].Visible = false;

                        }

                        Label lblTotalUnifom = GVListEmployees1.FooterRow.FindControl("lblTotalUnifom") as Label;
                        lblTotalUnifom.Text = Math.Round(totalUnifom).ToString();
                        if (totalUnifom > 0)
                        {
                            GVListEmployees1.Columns[154].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[154].Visible = false;

                        }

                        Label lblTotalOthers = GVListEmployees1.FooterRow.FindControl("lblTotalOthers") as Label;
                        lblTotalOthers.Text = Math.Round(totalOthers).ToString();
                        if (totalOthers > 0)
                        {
                            GVListEmployees1.Columns[155].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[155].Visible = false;

                        }

                        Label lblTotalTotalDeductions = GVListEmployees1.FooterRow.FindControl("lblTotalTotalDeductions") as Label;
                        lblTotalTotalDeductions.Text = Math.Round(totalTotDeductions).ToString();
                        if (totalTotDeductions > 0)
                        {
                            GVListEmployees1.Columns[156].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[156].Visible = false;

                        }



                        Label lblTotalNetPayy = GVListEmployees1.FooterRow.FindControl("lblTotalNetPayy") as Label;
                        lblTotalNetPayy.Text = Math.Round(totalTOTALNETPAY).ToString();
                        if (totalTOTALNETPAY > 0)
                        {
                            GVListEmployees1.Columns[157].Visible = true;
                        }
                        else
                        {
                            GVListEmployees1.Columns[157].Visible = false;

                        }



                        //New code add as on 10/08/2021 by Mahesh Goud

                        #endregion


                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        protected void GVListEmployees_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[3].Attributes.Add("class", "text");
                e.Row.Cells[4].Attributes.Add("class", "text");
                e.Row.Cells[9].Attributes.Add("class", "text");
                e.Row.Cells[10].Attributes.Add("class", "text");
                e.Row.Cells[11].Attributes.Add("class", "text");
                if (((Label)e.Row.FindControl("lblSno")).Text == "99999999")
                {
                    ((Label)e.Row.FindControl("lblSno")).Text = "";
                    ((Label)e.Row.FindControl("lblclientid")).Text = "";
                    e.Row.Style.Add("font-weight", "bold");
                }

                if (((Label)e.Row.FindControl("lblSno")).Text == "0")
                {
                    ((Label)e.Row.FindControl("lblempid")).Text = "";
                    ((Label)e.Row.FindControl("lblempname")).Text = "";
                    ((Label)e.Row.FindControl("lblSno")).Text = "";
                    e.Row.Style.Add("font-weight", "bold");

                    ((Label)e.Row.FindControl("lblTempGross")).Text = "";

                    ((Label)e.Row.FindControl("lblbasic")).Text = "";
                    ((Label)e.Row.FindControl("lbldutyhrs")).Text = "";
                    ((Label)e.Row.FindControl("lblOTs")).Text = "";
                    //((Label)e.Row.FindControl("lblOts")).Text = "";
                    ((Label)e.Row.FindControl("lblwos")).Text = "";
                    ((Label)e.Row.FindControl("lblNhs")).Text = "";
                    ((Label)e.Row.FindControl("lblNpots")).Text = "";
                    //((Label)e.Row.FindControl("lbltempgross")).Text = "";
                    ((Label)e.Row.FindControl("lblda")).Text = "";
                    ((Label)e.Row.FindControl("lblhra")).Text = "";
                    ((Label)e.Row.FindControl("lblcca")).Text = "";
                    //((Label)e.Row.FindControl("lblNPCl25Per")).Text = "";
                    ((Label)e.Row.FindControl("lblConveyance")).Text = "";
                    ((Label)e.Row.FindControl("lblwashallowance")).Text = "";
                    ((Label)e.Row.FindControl("lblOtherallowance")).Text = "";
                    ((Label)e.Row.FindControl("lblLeaveEncashAmt")).Text = "";
                    ((Label)e.Row.FindControl("lblGratuity")).Text = "";
                    ((Label)e.Row.FindControl("lblBonus")).Text = "";
                    ((Label)e.Row.FindControl("lblNfhs")).Text = "";
                    ((Label)e.Row.FindControl("lblrc")).Text = "";
                    ((Label)e.Row.FindControl("lblcs")).Text = "";
                    ((Label)e.Row.FindControl("lblIncentivs")).Text = "";
                    ((Label)e.Row.FindControl("lblArrears")).Text = "";
                    ((Label)e.Row.FindControl("lblWoAmt")).Text = "";
                    ((Label)e.Row.FindControl("lblNhsAmt")).Text = "";
                    ((Label)e.Row.FindControl("lblNpotsAmt")).Text = "";
                    ((Label)e.Row.FindControl("lblTravelAllw")).Text = "";
                    ((Label)e.Row.FindControl("lblmedicalallowance")).Text = "";
                    ((Label)e.Row.FindControl("lblAddlAmount")).Text = "";
                    ((Label)e.Row.FindControl("lblSpecialAllowance")).Text = "";
                    //((Label)e.Row.FindControl("lblUniformAllw")).Text = "";
                    ((Label)e.Row.FindControl("lblPerformanceAllw")).Text = "";
                    ((Label)e.Row.FindControl("lblMobileAllowance")).Text = "";
                    ((Label)e.Row.FindControl("lblFoodAllowance")).Text = "";
                    ((Label)e.Row.FindControl("lblOTAmt")).Text = "";
                    ((Label)e.Row.FindControl("lblGross")).Text = "";
                    ((Label)e.Row.FindControl("lblPF")).Text = "";
                    ((Label)e.Row.FindControl("lblESI")).Text = "";
                    ((Label)e.Row.FindControl("lblProfTax")).Text = "";
                    ((Label)e.Row.FindControl("lblsaladv")).Text = "";
                    ((Label)e.Row.FindControl("lbladvded")).Text = "";
                    // ((Label)e.Row.FindControl("lblwcded")).Text = "";
                    ((Label)e.Row.FindControl("lbluniform")).Text = "";
                    ((Label)e.Row.FindControl("lblOtherDed")).Text = "";
                    ((Label)e.Row.FindControl("lbltotalloanded")).Text = "";
                    ((Label)e.Row.FindControl("lblcantadv")).Text = "";
                    // ((Label)e.Row.FindControl("lblSecDepDed")).Text = "";
                    ((Label)e.Row.FindControl("lblGeneralDed")).Text = "";
                    ((Label)e.Row.FindControl("lblrentded")).Text = "";
                    ((Label)e.Row.FindControl("lblowf")).Text = "";
                    ((Label)e.Row.FindControl("lblPenalty")).Text = "";
                    ((Label)e.Row.FindControl("lblNightAllw")).Text = "";
                    ((Label)e.Row.FindControl("lblMLWFDed")).Text = "";
                    ((Label)e.Row.FindControl("lblDeductions")).Text = "";
                    ((Label)e.Row.FindControl("lblnetamount")).Text = "";
                    ((Label)e.Row.FindControl("lblLeaveDed")).Text = "";
                    //((Label)e.Row.FindControl("lblTransport")).Text = "";
                    ((Label)e.Row.FindControl("lblAttBonus")).Text = "";
                    //((Label)e.Row.FindControl("lblTransport6Per")).Text = "";
                    //((Label)e.Row.FindControl("lblGeneralDed")).Text = "";
                    //((Label)e.Row.FindControl("lblRentDed")).Text = "";
                    ((Label)e.Row.FindControl("lblMedicalDed")).Text = "";


                    ((Label)e.Row.FindControl("lblFoodDed")).Text = "";
                    ((Label)e.Row.FindControl("lblElectricityDed")).Text = "";
                    ((Label)e.Row.FindControl("lblTransportDed")).Text = "";
                    //((Label)e.Row.FindControl("lblDccDed")).Text = "";
                    ((Label)e.Row.FindControl("lblLicenseDed")).Text = "";
                    ((Label)e.Row.FindControl("lblAdv4Ded")).Text = "";
                    ((Label)e.Row.FindControl("lblNightRoundDed")).Text = "";

                    //((Label)e.Row.FindControl("lblPFEmpr")).Text = "";
                    //((Label)e.Row.FindControl("lblESIEmpr")).Text = "";
                    //((Label)e.Row.FindControl("lblctc")).Text = "";

                    //(19)
                    ((Label)e.Row.FindControl("lblCdbasic")).Text = "";
                    ((Label)e.Row.FindControl("lblCdda")).Text = "";
                    //((Label)e.Row.FindControl("lblOts")).Text = "";
                    ((Label)e.Row.FindControl("lblCdhra")).Text = "";
                    ((Label)e.Row.FindControl("lblCdcca")).Text = "";
                    ((Label)e.Row.FindControl("lblCdConveyance")).Text = "";
                    //((Label)e.Row.FindControl("lbltempgross")).Text = "";
                    ((Label)e.Row.FindControl("lblcdwashallowance")).Text = "";
                    ((Label)e.Row.FindControl("lblcdNfhs")).Text = "";
                    ((Label)e.Row.FindControl("lblcdrc")).Text = "";
                    //((Label)e.Row.FindControl("lblNPCl25Per")).Text = "";
                    ((Label)e.Row.FindControl("lblcdcs")).Text = "";
                    ((Label)e.Row.FindControl("lblcdAddlAmount")).Text = "";
                    ((Label)e.Row.FindControl("lblcdFoodAllowance")).Text = "";
                    ((Label)e.Row.FindControl("lblcdWoAmt")).Text = "";
                    ((Label)e.Row.FindControl("lblcdNhsAmt")).Text = "";
                    ((Label)e.Row.FindControl("lblcdmedicalallowance")).Text = "";
                    ((Label)e.Row.FindControl("lblcdSpecialAllowance")).Text = "";
                    ((Label)e.Row.FindControl("lblcdTravelAllw")).Text = "";
                    ((Label)e.Row.FindControl("lblcdMobileAllowance")).Text = "";
                    ((Label)e.Row.FindControl("lblcdPerformanceAllw")).Text = "";
                    ((Label)e.Row.FindControl("lblcdLeaveEncashAmt")).Text = "";
                    ((Label)e.Row.FindControl("lblcdNpotsAmt")).Text = "";
                    ((Label)e.Row.FindControl("lblcdIncentivs")).Text = "";
                    ((Label)e.Row.FindControl("lblcdBonus")).Text = "";
                    ((Label)e.Row.FindControl("lblcdGratuity")).Text = "";
                    ((Label)e.Row.FindControl("lblcdOtherallowance")).Text = "";
                    ((Label)e.Row.FindControl("lblcdOTAmt")).Text = "";
                    //((Label)e.Row.FindControl("lblUniformAllw")).Text = "";
                    ((Label)e.Row.FindControl("lblcdServiceWeightage")).Text = "";
                    ((Label)e.Row.FindControl("lblcdRankAllowance")).Text = "";
                    ((Label)e.Row.FindControl("lblcdAttBonus")).Text = "";
                    ((Label)e.Row.FindControl("lblcdNightAllw")).Text = "";
                    ((Label)e.Row.FindControl("lblfixedADDL4HR")).Text = "";
                    ((Label)e.Row.FindControl("lblfixedQTRALLOW")).Text = "";
                    ((Label)e.Row.FindControl("lblfixedRELALLOW")).Text = "";
                    ((Label)e.Row.FindControl("lblfixedSITEALLOW")).Text = "";
                    ((Label)e.Row.FindControl("lblfixedGunAllw")).Text = "";
                    ((Label)e.Row.FindControl("lblfixedFireAllw")).Text = "";
                    ((Label)e.Row.FindControl("lblfixedTelephoneAllw")).Text = "";
                    ((Label)e.Row.FindControl("lblfixedReimbursement")).Text = "";
                    ((Label)e.Row.FindControl("lblfixedHardshipAllw")).Text = "";
                    ((Label)e.Row.FindControl("lblfixedPaidHolidayAllw")).Text = "";
                    ((Label)e.Row.FindControl("lblADDL4HR")).Text = "";
                    ((Label)e.Row.FindControl("lblQTRALLOW")).Text = "";
                    ((Label)e.Row.FindControl("lblRELALLOW")).Text = "";
                    ((Label)e.Row.FindControl("lblSITEALLOW")).Text = "";
                    ((Label)e.Row.FindControl("lblGunAllw")).Text = "";
                    ((Label)e.Row.FindControl("lblFireAllw")).Text = "";
                    ((Label)e.Row.FindControl("lblTelephoneAllw")).Text = "";
                    ((Label)e.Row.FindControl("lblReimbursement")).Text = "";
                    ((Label)e.Row.FindControl("lblHardshipAllw")).Text = "";
                    ((Label)e.Row.FindControl("lblPaidHolidayAllw")).Text = "";
                    ((Label)e.Row.FindControl("lblRankAllowance")).Text = "";
                    ((Label)e.Row.FindControl("lblAdvanceAgainstBonus")).Text = "";

                }
                // ((Label)e.Row.FindControl("lblmonth")).Text = txtmonth.Text;
            }
        }

        protected void ddloptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddloptions.SelectedIndex == 1)
            {
                GVListEmployees1.DataSource = null;
                GVListEmployees1.DataBind();
                GVClientsData.DataSource = null;
                GVClientsData.DataBind();
                txtfrom.Visible = true;
                txtto.Visible = true;
                lblfrom.Visible = true;
                lblto.Visible = true;
                lblmonth.Visible = false;
                txtmonth.Visible = false;
                btngetdata.Visible = false;
                btn_Submit.Visible = true;
            }
            else
            {
                GVListEmployees1.DataSource = null;
                GVListEmployees1.DataBind();
                GVClientsData.DataSource = null;
                GVClientsData.DataBind();
                txtfrom.Visible = false;
                txtto.Visible = false;
                lblfrom.Visible = false;
                lblto.Visible = false;
                lblmonth.Visible = true;
                txtmonth.Visible = true;
                btngetdata.Visible = false;
                btn_Submit.Visible = true;
            }
        }

        protected void btngetdata_Click(object sender, EventArgs e)
        {
            DisplayData();
        }

        public class PageEventHelperPageNo : PdfPageEventHelper
        {
            PdfContentByte cb;
            PdfTemplate template;

            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                cb = writer.DirectContent;
                template = cb.CreateTemplate(50, 50);
            }
            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnEndPage(writer, document);

                int pageN = writer.PageNumber;
                String text = "Page " + pageN.ToString() + " of ";
                float len = bf.GetWidthPoint(text, 8);


                iTextSharp.text.Rectangle pageSize = document.PageSize;

                cb.SetRGBColorFill(100, 100, 100);

                cb.BeginText();
                cb.SetFontAndSize(bf, 8);

                cb.SetTextMatrix(document.LeftMargin + 25, pageSize.GetBottom(document.BottomMargin - 10));
                cb.ShowText(text);

                cb.EndText();

                cb.AddTemplate(template, document.LeftMargin + len, pageSize.GetBottom(document.BottomMargin - 10));
            }

            int totalpgcount = 0;
            public override void OnCloseDocument(PdfWriter writer, Document document)
            {

                base.OnCloseDocument(writer, document);

                template.BeginText();
                template.SetFontAndSize(bf, 8);
                template.SetTextMatrix(25, 0);
                template.SetGrayStroke(11);
                totalpgcount = writer.PageNumber;
                totalpgcount = totalpgcount - 1;
                template.ShowText("" + totalpgcount);
                template.EndText();

            }
        }

        protected void btndownloadpdf_Click(object sender, EventArgs e)
        {
            if (txtmonth.Text.Trim().Length == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Please Select The Month')", true);
                return;
            }


            string date = string.Empty;


            if (txtmonth.Text.Trim().Length > 0)
            {
                date = DateTime.Parse(txtmonth.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
            }

            string month = DateTime.Parse(date).Month.ToString();
            string Year = DateTime.Parse(date).Year.ToString();


            var list = new List<Tuple<int, string>>();
            // var monthlist = new List<string>();
            var options = 0;
            if (GVClientsData.Rows.Count > 0)
            {
                for (int i = 0; i < GVClientsData.Rows.Count; i++)
                {
                    CheckBox chkclientid = GVClientsData.Rows[i].FindControl("chkindividual") as CheckBox;
                    Label lblclientid = GVClientsData.Rows[i].FindControl("lblclientid") as Label;
                    Label lblmonth = GVClientsData.Rows[i].FindControl("lblmonth") as Label;

                    if (chkclientid.Checked == true)
                    {
                        list.Add(Tuple.Create(Convert.ToInt32(lblmonth.Text), lblclientid.Text));
                    }

                }
            }

            // string Clientids = string.Join(",", list.ToArray());


            DataTable dtClientList = new DataTable();
            DataTable dtmonth = new DataTable();
            dtClientList.Columns.Add("Clientid");
            dtmonth.Columns.Add("month");
            if (list.Count != 0)
            {

                foreach (Tuple<int, string> tuple in list)
                {
                    DataRow row = dtClientList.NewRow();
                    DataRow row1 = dtmonth.NewRow();
                    row["Clientid"] = tuple.Item2;
                    row1["month"] = tuple.Item1;
                    dtClientList.Rows.Add(row["Clientid"]);
                    dtmonth.Rows.Add(row1["month"]);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "show alert", "alert('Please select Client IDs');", true);
            }

            string Fromdate = string.Empty;
            string ToMonth = string.Empty;

            if (txtfrom.Text.Trim().Length > 0)
            {
                Fromdate = DateTime.Parse(txtfrom.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
            }
            if (txtto.Text.Trim().Length > 0)
            {
                ToMonth = DateTime.Parse(txtto.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
            }

            DataTable dt = null;

            string SPName = "WageSheetSummary";
            Hashtable ht = new Hashtable();
            ht.Add("@ClientIDList", dtClientList);
            ht.Add("@MonthList", dtmonth);


            dt = config.ExecuteAdaptorWithParams(SPName, ht).Result;
            DataTable dtgroup = dt.DefaultView.ToTable(true, "ClientID");


            string strQry = "Select * from CompanyInfo  where   ClientidPrefix='" + CmpIDPrefix + "'";
            DataTable compInfo = config.ExecuteAdaptorAsyncWithQueryParams(strQry).Result;
            string companyName = "Your Company Name";
            string companyAddress = "Your Company Address";
            string PFNOForms = "";
            string TotalPFNOForms = "";
            if (compInfo.Rows.Count > 0)
            {
                companyName = compInfo.Rows[0]["CompanyName"].ToString();
                companyAddress = compInfo.Rows[0]["Address"].ToString();
                //PFNOForms = compInfo.Rows[0]["PFNoForms"].ToString();
            }


            MemoryStream ms = new MemoryStream();
            string fontsyle = "Courier New";

            int totalfonts = FontFactory.RegisterDirectory("c:\\WINDOWS\\fonts");
            StringBuilder sa = new StringBuilder();
            foreach (string fontname in FontFactory.RegisteredFonts)
            {
                sa.Append(fontname + "\n");
            }
            int Fontsize = 12;
            if (dt.Rows.Count > 0)
            {

                Document document = new Document(PageSize.A4.Rotate());
                var writer = PdfWriter.GetInstance(document, ms);
                PageEventHelperPageNo pageEventHelper = new PageEventHelperPageNo();
                writer.PageEvent = pageEventHelper;
                document.Open();
                BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                for (int j = 0; j < dtgroup.Rows.Count; j++)
                {
                    document.NewPage();


                    PdfPTable Maintable = new PdfPTable(14);
                    Maintable.TotalWidth = 790;
                    Maintable.HeaderRows = 3;
                    Maintable.LockedWidth = true;
                    float[] width = new float[] { 1f, 1.5f, 1.5f, 1f, 1f, 1f, 1f, 1.2f, 1f, 1f, 1f, 1f, 1f, 1f };
                    Maintable.SetWidths(width);



                    PdfPCell cell = new PdfPCell(new Phrase("", FontFactory.GetFont(fontsyle, Fontsize + 1, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                    cell.Colspan = 7;
                    cell.Border = 0;
                    //Maintable.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Zone", FontFactory.GetFont(fontsyle, Fontsize + 1, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                    cell.Colspan = 1;
                    cell.Border = 0;
                    //Maintable.AddCell(cell);

                    cell = new PdfPCell(new Phrase("PF1/WB1/KOLKATA/WEST/SEC", FontFactory.GetFont(fontsyle, Fontsize + 1, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cell.Colspan = 6;
                    cell.Border = 0;
                    // Maintable.AddCell(cell);

                    PdfPCell cellunit = new PdfPCell(new Phrase("UNIT SUMMARY OF", FontFactory.GetFont(fontsyle, Fontsize + 1, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cellunit.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                    cellunit.Colspan = 3;
                    cellunit.Border = 0;
                    Maintable.AddCell(cellunit);

                    PdfPCell cell33 = new PdfPCell(new Phrase("", FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cell33.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cell33.Colspan = 2;
                    cell33.Border = 0;
                    Maintable.AddCell(cell33);

                    PdfPCell cell52 = new PdfPCell(new Phrase("", FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cell52.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cell52.Colspan = 2;
                    cell52.Border = 0;
                    Maintable.AddCell(cell52);

                    cell52 = new PdfPCell(new Phrase("Unit Code", FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cell52.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                    cell52.Colspan = 2;
                    cell52.Border = 0;
                    Maintable.AddCell(cell52);

                    string Clientid = dtgroup.Rows[j]["Clientid"].ToString();
                    PdfPCell cell22 = new PdfPCell(new Phrase(Clientid, FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cell22.HorizontalAlignment = 3; //0=Left, 1=Centre, 2=Right
                    cell22.Colspan = 5;
                    cell22.Border = 0;
                    Maintable.AddCell(cell22);

                    PdfPCell celldata2 = new PdfPCell(new Phrase("EMPLOYEE", FontFactory.GetFont(fontsyle, Fontsize + 1, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    celldata2.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                    celldata2.Colspan = 3;
                    celldata2.Border = 0;
                    Maintable.AddCell(celldata2);

                    PdfPCell cell3 = new PdfPCell(new Phrase("Salary Month", FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cell3.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cell3.Colspan = 2;
                    cell3.Border = 0;
                    Maintable.AddCell(cell3);

                    PdfPCell cell5 = new PdfPCell(new Phrase(GetMonthName() + " " + Year, FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cell5.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cell5.Colspan = 2;
                    cell5.Border = 0;
                    Maintable.AddCell(cell5);

                    cell5 = new PdfPCell(new Phrase("Unit", FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cell5.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                    cell5.Colspan = 1;
                    cell5.Border = 0;
                    Maintable.AddCell(cell5);

                    string strClientname = "select ClientName from Clients where clientid='" + Clientid + "'";
                    DataTable dtclientname = config.ExecuteAdaptorAsyncWithQueryParams(strClientname).Result;
                    string Clientname = "";
                    if (dtclientname.Rows.Count > 0)
                    {
                        Clientname = dtclientname.Rows[0]["ClientName"].ToString();
                    }

                    PdfPCell cell2 = new PdfPCell(new Phrase(Clientname, FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cell2.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cell2.Colspan = 6;
                    cell2.Border = 0;
                    Maintable.AddCell(cell2);



                    PdfPCell cell6 = new PdfPCell(new Phrase("Slno", FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cell6.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cell6.Colspan = 1;
                    cell6.BorderWidthTop = 0.2f;
                    cell6.BorderWidthBottom = 0.2f;
                    cell6.BorderWidthLeft = 0;
                    cell6.BorderWidthRight = 0;
                    Maintable.AddCell(cell6);

                    PdfPCell cell7 = new PdfPCell(new Phrase("Emp No\n" + "Employee Name", FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cell7.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cell7.Colspan = 2;
                    cell7.BorderWidthTop = 0.2f;
                    cell7.BorderWidthBottom = 0.2f;
                    cell7.BorderWidthLeft = 0;
                    cell7.BorderWidthRight = 0;
                    Maintable.AddCell(cell7);

                    PdfPCell cell8 = new PdfPCell(new Phrase("DUTY\n" + "OT DUTY", FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cell8.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cell8.Colspan = 1;
                    cell8.BorderWidthTop = 0.2f;
                    cell8.BorderWidthBottom = 0.2f;
                    cell8.BorderWidthLeft = 0;
                    cell8.BorderWidthRight = 0;
                    Maintable.AddCell(cell8);

                    PdfPCell cell9 = new PdfPCell(new Phrase("Gross", FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cell9.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cell9.Colspan = 1;
                    cell9.BorderWidthTop = 0.2f;
                    cell9.BorderWidthBottom = 0.2f;
                    cell9.BorderWidthLeft = 0;
                    cell9.BorderWidthRight = 0;
                    Maintable.AddCell(cell9);

                    PdfPCell cell11 = new PdfPCell(new Phrase("PF\n" + "PT", FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cell11.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cell11.Colspan = 1;
                    cell11.BorderWidthTop = 0.2f;
                    cell11.BorderWidthBottom = 0.2f;
                    cell11.BorderWidthLeft = 0;
                    cell11.BorderWidthRight = 0;
                    Maintable.AddCell(cell11);

                    PdfPCell cell12 = new PdfPCell(new Phrase(" ESI\n" + "LWF\n" + "ADM", FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cell12.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cell12.Colspan = 1;
                    cell12.BorderWidthTop = 0.2f;
                    cell12.BorderWidthBottom = 0.2f;
                    cell12.BorderWidthLeft = 0;
                    cell12.BorderWidthRight = 0;
                    Maintable.AddCell(cell12);

                    PdfPCell cell13 = new PdfPCell(new Phrase("Adv\n" + "Uni\n" + "Misd", FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cell13.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cell13.Colspan = 1;
                    cell13.BorderWidthTop = 0.2f;
                    cell13.BorderWidthBottom = 0.2f;
                    cell13.BorderWidthLeft = 0;
                    cell13.BorderWidthRight = 0;
                    Maintable.AddCell(cell13);

                    PdfPCell cellFine = new PdfPCell(new Phrase("Fine\n" + "Misc", FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cellFine.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cellFine.Colspan = 1;
                    cellFine.BorderWidthTop = 0.2f;
                    cellFine.BorderWidthBottom = 0.2f;
                    cellFine.BorderWidthLeft = 0;
                    cellFine.BorderWidthRight = 0;
                    Maintable.AddCell(cellFine);

                    PdfPCell cellDed = new PdfPCell(new Phrase("TotalDeduction", FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cellDed.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cellDed.Colspan = 1;
                    cellDed.BorderWidthTop = 0.2f;
                    cellDed.BorderWidthBottom = 0.2f;
                    cellDed.BorderWidthLeft = 0;
                    cellDed.BorderWidthRight = 0;
                    Maintable.AddCell(cellDed);

                    PdfPCell cellnetpay = new PdfPCell(new Phrase("NetPay\nOthers", FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cellnetpay.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cellnetpay.Colspan = 1;
                    cellnetpay.BorderWidthTop = 0.2f;
                    cellnetpay.BorderWidthBottom = 0.2f;
                    cellnetpay.BorderWidthLeft = 0;
                    cellnetpay.BorderWidthRight = 0;
                    Maintable.AddCell(cellnetpay);

                    PdfPCell cellOtpay = new PdfPCell(new Phrase("OT PAY\n" + "ESI\n" + "OT NET", FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cellOtpay.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cellOtpay.Colspan = 1;
                    cellOtpay.BorderWidthTop = 0.2f;
                    cellOtpay.BorderWidthBottom = 0.2f;
                    cellOtpay.BorderWidthLeft = 0;
                    cellOtpay.BorderWidthRight = 0;
                    Maintable.AddCell(cellOtpay);

                    PdfPCell cellGrand = new PdfPCell(new Phrase("GRAND TOTAL", FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cellGrand.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cellGrand.Colspan = 1;
                    cellGrand.BorderWidthTop = 0.2f;
                    cellGrand.BorderWidthBottom = 0.2f;
                    cellGrand.BorderWidthLeft = 0;
                    cellGrand.BorderWidthRight = 0;
                    Maintable.AddCell(cellGrand);

                    PdfPCell cellSign = new PdfPCell(new Phrase("SIGN", FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cellSign.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cellSign.Colspan = 1;
                    cellSign.BorderWidthTop = 0.2f;
                    cellSign.BorderWidthBottom = 0.2f;
                    cellSign.BorderWidthLeft = 0;
                    cellSign.BorderWidthRight = 0;
                    Maintable.AddCell(cellSign);

                    int pagecount = 0;
                    string Sno = "";
                    string ClientID = "";
                    //string Clientname = "";
                    string Empid = "";
                    string Empname = "";
                    string Desgn = "";
                    float Noofduties = 0;
                    float Ots = 0;
                    float Tot = 0;
                    float Wo = 0;
                    float Leave = 0;
                    float PH = 0;
                    float CanteenAdv = 0;
                    float Uniform = 0;
                    float Penalty = 0;
                    float ATMDed = 0;
                    float TotalDeds = 0;
                    float Gross = 0;
                    float PF = 0;
                    float ESI = 0;
                    float ProfTax = 0;
                    float OWF = 0;
                    float AdminCharges = 0;
                    float Misd = 0;
                    float TotalDeductions = 0;
                    float Netpay = 0;
                    float OTAmt = 0;
                    float ESIONOT = 0;
                    float OTNet = 0;
                    float ActualAmount = 0;
                    string BankName = "";
                    string AcNo = "";
                    string RefNo = "";
                    string IFSCCode = "";
                    float Others = 0;
                    string Remarks = "";

                    float TotalNoofduties = 0;
                    float TotalOts = 0;
                    float TotalTot = 0;
                    float TotalWo = 0;
                    float TotalLeave = 0;
                    float TotalPH = 0;
                    float TotalCanteenAdv = 0;
                    float TotalUniform = 0;
                    float TotalPenalty = 0;
                    float TotalATMDed = 0;
                    float TotalTotalDeds = 0;
                    float TotalGross = 0;
                    float TotalPF = 0;
                    float TotalESI = 0;
                    float TotalProfTax = 0;
                    float TotalOWF = 0;
                    float TotalAdminCharges = 0;
                    float TotalMisd = 0;
                    float TotalTotalDeductions = 0;
                    float TotalNetpay = 0;
                    float TotalOTAmt = 0;
                    float TotalESIONOT = 0;
                    float TotalOTNet = 0;
                    float TotalActualAmount = 0;
                    float TotalOthers = 0;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dtgroup.Rows[j]["Clientid"].ToString() == dt.Rows[i]["Clientid"].ToString())
                        {

                            Sno = dt.Rows[i]["Sno"].ToString();
                            ClientID = dt.Rows[i]["Clientid"].ToString();
                            // Clientname = dt.Rows[i]["Client Name"].ToString();
                            Empid = dt.Rows[i]["EmpId"].ToString();
                            Empname = dt.Rows[i]["Name"].ToString();
                            Noofduties = Convert.ToSingle(dt.Rows[i]["NoOfDuties"].ToString());
                            Ots = Convert.ToSingle(dt.Rows[i]["ots"].ToString());
                            CanteenAdv = Convert.ToSingle(dt.Rows[i]["CanteenAdv"].ToString());
                            Uniform = Convert.ToSingle(dt.Rows[i]["UniformDed"].ToString());
                            Penalty = Convert.ToSingle(dt.Rows[i]["penalty"].ToString());
                            ATMDed = Convert.ToSingle(dt.Rows[i]["ATMDed"].ToString());
                            Gross = Convert.ToSingle(dt.Rows[i]["Gross"].ToString());
                            PF = Convert.ToSingle(dt.Rows[i]["PF"].ToString());
                            ESI = Convert.ToSingle(dt.Rows[i]["ESI"].ToString());
                            ProfTax = Convert.ToSingle(dt.Rows[i]["ProfTax"].ToString());
                            OWF = Convert.ToSingle(dt.Rows[i]["OWF"].ToString());
                            AdminCharges = Convert.ToSingle(dt.Rows[i]["AdminCharges"].ToString());
                            Misd = Convert.ToSingle(dt.Rows[i]["Misd"].ToString());
                            TotalDeductions = Convert.ToSingle(dt.Rows[i]["TotalDeductions"].ToString());
                            Netpay = Convert.ToSingle(dt.Rows[i]["Netpay"].ToString());
                            OTAmt = Convert.ToSingle(dt.Rows[i]["OTAmt"].ToString());
                            ESIONOT = Convert.ToSingle(dt.Rows[i]["ESIONOT"].ToString());
                            OTNet = Convert.ToSingle(dt.Rows[i]["OTNet"].ToString());
                            Others = Convert.ToSingle(dt.Rows[i]["OthersAllw"].ToString());
                            ActualAmount = Convert.ToSingle(dt.Rows[i]["ActualAmount"].ToString());
                            BankName = dt.Rows[i]["Bank"].ToString();
                            AcNo = dt.Rows[i]["ACNo"].ToString();
                            RefNo = dt.Rows[i]["RefNo"].ToString();
                            IFSCCode = dt.Rows[i]["IFSCCode"].ToString();
                            Remarks = dt.Rows[i]["Remarks"].ToString();

                            PdfPCell cellsno = new PdfPCell(new Phrase(Sno, FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cellsno.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                            cellsno.Colspan = 1;
                            cellsno.BorderWidthTop = 0.2f;
                            cellsno.BorderWidthBottom = 0;
                            cellsno.BorderWidthLeft = 0;
                            cellsno.BorderWidthRight = 0;
                            Maintable.AddCell(cellsno);

                            PdfPCell cellempname = new PdfPCell(new Phrase(Empid + "\n" + Empname, FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cellempname.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                            cellempname.Colspan = 2;
                            cellempname.BorderWidthTop = 0.2f;
                            cellempname.BorderWidthBottom = 0;
                            cellempname.BorderWidthLeft = 0;
                            cellempname.BorderWidthRight = 0;
                            Maintable.AddCell(cellempname);

                            PdfPCell cellduty = new PdfPCell(new Phrase(Noofduties.ToString() + "\n" + Ots.ToString(), FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cellduty.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                            cellduty.Colspan = 1;
                            cellduty.BorderWidthTop = 0.2f;
                            cellduty.BorderWidthBottom = 0;
                            cellduty.BorderWidthLeft = 0;
                            cellduty.BorderWidthRight = 0;
                            Maintable.AddCell(cellduty);
                            TotalNoofduties += Noofduties;
                            TotalOts += Ots;

                            PdfPCell cellGross = new PdfPCell(new Phrase(Gross.ToString(), FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cellGross.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                            cellGross.Colspan = 1;
                            cellGross.BorderWidthTop = 0.2f;
                            cellGross.BorderWidthBottom = 0;
                            cellGross.BorderWidthLeft = 0;
                            cellGross.BorderWidthRight = 0;
                            Maintable.AddCell(cellGross);
                            TotalGross += Gross;

                            PdfPCell cellPF = new PdfPCell(new Phrase(PF.ToString() + "\n" + ProfTax.ToString(), FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cellPF.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                            cellPF.Colspan = 1;
                            cellPF.BorderWidthTop = 0.2f;
                            cellPF.BorderWidthBottom = 0;
                            cellPF.BorderWidthLeft = 0;
                            cellPF.BorderWidthRight = 0;
                            Maintable.AddCell(cellPF);
                            TotalPF += PF;
                            TotalProfTax += ProfTax;

                            PdfPCell cellESI = new PdfPCell(new Phrase(ESI + "\n" + OWF + "\n" + AdminCharges, FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cellESI.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                            cellESI.Colspan = 1;
                            cellESI.BorderWidthTop = 0.2f;
                            cellESI.BorderWidthBottom = 0;
                            cellESI.BorderWidthLeft = 0;
                            cellESI.BorderWidthRight = 0;
                            Maintable.AddCell(cellESI);
                            TotalESI += ESI;
                            TotalOWF += OWF;
                            TotalAdminCharges += AdminCharges;

                            PdfPCell celladv = new PdfPCell(new Phrase(CanteenAdv + "\n" + Uniform + "\n" + Misd, FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            celladv.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                            celladv.Colspan = 1;
                            celladv.BorderWidthTop = 0.2f;
                            celladv.BorderWidthBottom = 0;
                            celladv.BorderWidthLeft = 0;
                            celladv.BorderWidthRight = 0;
                            Maintable.AddCell(celladv);
                            TotalCanteenAdv += CanteenAdv;
                            TotalUniform += Uniform;
                            TotalMisd += Misd;

                            PdfPCell cellFine2 = new PdfPCell(new Phrase(Penalty + "\n" + ATMDed, FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cellFine2.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                            cellFine2.Colspan = 1;
                            cellFine2.BorderWidthTop = 0.2f;
                            cellFine2.BorderWidthBottom = 0;
                            cellFine2.BorderWidthLeft = 0;
                            cellFine2.BorderWidthRight = 0;
                            Maintable.AddCell(cellFine2);
                            TotalPenalty += Penalty;
                            TotalATMDed += ATMDed;

                            PdfPCell cellDed2 = new PdfPCell(new Phrase(TotalDeductions.ToString(), FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cellDed2.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                            cellDed2.Colspan = 1;
                            cellDed2.BorderWidthTop = 0.2f;
                            cellDed2.BorderWidthBottom = 0;
                            cellDed2.BorderWidthLeft = 0;
                            cellDed2.BorderWidthRight = 0;
                            Maintable.AddCell(cellDed2);
                            TotalTotalDeductions += TotalDeductions;

                            PdfPCell cellnetpay2 = new PdfPCell(new Phrase(Netpay.ToString() + "\n" + Others.ToString(), FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cellnetpay2.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                            cellnetpay2.Colspan = 1;
                            cellnetpay2.BorderWidthTop = 0.2f;
                            cellnetpay2.BorderWidthBottom = 0;
                            cellnetpay2.BorderWidthLeft = 0;
                            cellnetpay2.BorderWidthRight = 0;
                            Maintable.AddCell(cellnetpay2);
                            TotalNetpay += Netpay;
                            TotalOthers += Others;

                            PdfPCell cellOtpay2 = new PdfPCell(new Phrase(OTAmt + "\n" + ESIONOT + "\n" + OTNet, FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cellOtpay2.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                            cellOtpay2.Colspan = 1;
                            cellOtpay2.BorderWidthTop = 0.2f;
                            cellOtpay2.BorderWidthBottom = 0;
                            cellOtpay2.BorderWidthLeft = 0;
                            cellOtpay2.BorderWidthRight = 0;
                            Maintable.AddCell(cellOtpay2);
                            TotalOTAmt += OTAmt;
                            TotalESIONOT += ESIONOT;
                            TotalOTNet += OTNet;

                            PdfPCell cellGrand2 = new PdfPCell(new Phrase(ActualAmount.ToString(), FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cellGrand2.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                            cellGrand2.Colspan = 1;
                            cellGrand2.BorderWidthTop = 0.2f;
                            cellGrand2.BorderWidthBottom = 0;
                            cellGrand2.BorderWidthLeft = 0;
                            cellGrand2.BorderWidthRight = 0;
                            Maintable.AddCell(cellGrand2);
                            TotalActualAmount += ActualAmount;

                            PdfPCell cellSign2 = new PdfPCell(new Phrase("", FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cellSign2.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                            cellSign2.Colspan = 1;
                            cellSign2.BorderWidthTop = 0.2f;
                            cellSign2.BorderWidthBottom = 0;
                            cellSign2.BorderWidthLeft = 0;
                            cellSign2.BorderWidthRight = 0;
                            Maintable.AddCell(cellSign2);

                            PdfPCell cellSno2 = new PdfPCell(new Phrase("", FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cellSno2.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                            cellSno2.Colspan = 1;
                            cellSno2.BorderWidthTop = 0;
                            cellSno2.BorderWidthBottom = 0.2f;
                            cellSno2.BorderWidthLeft = 0;
                            cellSno2.BorderWidthRight = 0;
                            Maintable.AddCell(cellSno2);

                            PdfPCell cellBank = new PdfPCell(new Phrase("Bank : " + BankName, FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cellBank.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                            cellBank.Colspan = 2;
                            cellBank.BorderWidthTop = 0;
                            cellBank.BorderWidthBottom = 0.2f;
                            cellBank.BorderWidthLeft = 0;
                            cellBank.BorderWidthRight = 0;
                            Maintable.AddCell(cellBank);

                            PdfPCell cellacno = new PdfPCell(new Phrase("AC No " + AcNo, FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cellacno.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                            cellacno.Colspan = 4;
                            cellacno.BorderWidthTop = 0;
                            cellacno.BorderWidthBottom = 0.2f;
                            cellacno.BorderWidthLeft = 0;
                            cellacno.BorderWidthRight = 0;
                            Maintable.AddCell(cellacno);

                            PdfPCell cellrefno = new PdfPCell(new Phrase("Refno " + RefNo, FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cellrefno.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                            cellrefno.Colspan = 3;
                            cellrefno.BorderWidthTop = 0;
                            cellrefno.BorderWidthBottom = 0.2f;
                            cellrefno.BorderWidthLeft = 0;
                            cellrefno.BorderWidthRight = 0;
                            Maintable.AddCell(cellrefno);

                            PdfPCell cellifsccode = new PdfPCell(new Phrase("IFSC " + IFSCCode, FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cellifsccode.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                            cellifsccode.Colspan = 2;
                            cellifsccode.BorderWidthTop = 0;
                            cellifsccode.BorderWidthBottom = 0.2f;
                            cellifsccode.BorderWidthLeft = 0;
                            cellifsccode.BorderWidthRight = 0;
                            Maintable.AddCell(cellifsccode);

                            PdfPCell cellspace = new PdfPCell(new Phrase(Remarks, FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cellspace.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                            cellspace.Colspan = 2;
                            cellspace.BorderWidthTop = 0;
                            cellspace.BorderWidthBottom = 0.2f;
                            cellspace.BorderWidthLeft = 0;
                            cellspace.BorderWidthRight = 0;
                            Maintable.AddCell(cellspace);
                        }

                    }

                    PdfPCell cellsnototal = new PdfPCell(new Phrase("", FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cellsnototal.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cellsnototal.Colspan = 1;
                    cellsnototal.BorderWidthTop = 0.2f;
                    cellsnototal.BorderWidthBottom = 0.2f;
                    cellsnototal.BorderWidthLeft = 0;
                    cellsnototal.BorderWidthRight = 0;
                    Maintable.AddCell(cellsnototal);

                    PdfPCell cellempnametotal = new PdfPCell(new Phrase("", FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cellempnametotal.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cellempnametotal.Colspan = 2;
                    cellempnametotal.BorderWidthTop = 0.2f;
                    cellempnametotal.BorderWidthBottom = 0.2f;
                    cellempnametotal.BorderWidthLeft = 0;
                    cellempnametotal.BorderWidthRight = 0;
                    Maintable.AddCell(cellempnametotal);

                    PdfPCell celldutytotal = new PdfPCell(new Phrase(TotalNoofduties.ToString() + "\n" + TotalOts.ToString(), FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    celldutytotal.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    celldutytotal.Colspan = 1;
                    celldutytotal.BorderWidthTop = 0.2f;
                    celldutytotal.BorderWidthBottom = 0.2f;
                    celldutytotal.BorderWidthLeft = 0;
                    celldutytotal.BorderWidthRight = 0;
                    Maintable.AddCell(celldutytotal);

                    PdfPCell cellGrosstotal = new PdfPCell(new Phrase(TotalGross.ToString(), FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cellGrosstotal.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cellGrosstotal.Colspan = 1;
                    cellGrosstotal.BorderWidthTop = 0.2f;
                    cellGrosstotal.BorderWidthBottom = 0.2f;
                    cellGrosstotal.BorderWidthLeft = 0;
                    cellGrosstotal.BorderWidthRight = 0;
                    Maintable.AddCell(cellGrosstotal);

                    PdfPCell cellPFtotal = new PdfPCell(new Phrase(TotalPF.ToString() + "\n" + TotalProfTax.ToString(), FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cellPFtotal.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cellPFtotal.Colspan = 1;
                    cellPFtotal.BorderWidthTop = 0.2f;
                    cellPFtotal.BorderWidthBottom = 0.2f;
                    cellPFtotal.BorderWidthLeft = 0;
                    cellPFtotal.BorderWidthRight = 0;
                    Maintable.AddCell(cellPFtotal);

                    PdfPCell cellESItotal = new PdfPCell(new Phrase(TotalESI + "\n" + TotalOWF + "\n" + TotalAdminCharges, FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cellESItotal.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cellESItotal.Colspan = 1;
                    cellESItotal.BorderWidthTop = 0.2f;
                    cellESItotal.BorderWidthBottom = 0.2f;
                    cellESItotal.BorderWidthLeft = 0;
                    cellESItotal.BorderWidthRight = 0;
                    Maintable.AddCell(cellESItotal);

                    PdfPCell celladvtotal = new PdfPCell(new Phrase(TotalCanteenAdv + "\n" + TotalUniform + "\n" + TotalMisd, FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    celladvtotal.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    celladvtotal.Colspan = 1;
                    celladvtotal.BorderWidthTop = 0.2f;
                    celladvtotal.BorderWidthBottom = 0.2f;
                    celladvtotal.BorderWidthLeft = 0;
                    celladvtotal.BorderWidthRight = 0;
                    Maintable.AddCell(celladvtotal);

                    PdfPCell cellFine2total = new PdfPCell(new Phrase(TotalPenalty + "\n" + TotalATMDed, FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cellFine2total.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cellFine2total.Colspan = 1;
                    cellFine2total.BorderWidthTop = 0.2f;
                    cellFine2total.BorderWidthBottom = 0.2f;
                    cellFine2total.BorderWidthLeft = 0;
                    cellFine2total.BorderWidthRight = 0;
                    Maintable.AddCell(cellFine2total);

                    PdfPCell cellDed2total = new PdfPCell(new Phrase(TotalTotalDeductions.ToString(), FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cellDed2total.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cellDed2total.Colspan = 1;
                    cellDed2total.BorderWidthTop = 0.2f;
                    cellDed2total.BorderWidthBottom = 0.2f;
                    cellDed2total.BorderWidthLeft = 0;
                    cellDed2total.BorderWidthRight = 0;
                    Maintable.AddCell(cellDed2total);


                    PdfPCell cellnetpay2total = new PdfPCell(new Phrase(TotalNetpay.ToString() + "\n" + TotalOthers.ToString(), FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cellnetpay2total.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cellnetpay2total.Colspan = 1;
                    cellnetpay2total.BorderWidthTop = 0.2f;
                    cellnetpay2total.BorderWidthBottom = 0.2f;
                    cellnetpay2total.BorderWidthLeft = 0;
                    cellnetpay2total.BorderWidthRight = 0;
                    Maintable.AddCell(cellnetpay2total);


                    PdfPCell cellOtpay2total = new PdfPCell(new Phrase(TotalOTAmt + "\n" + TotalESIONOT + "\n" + TotalOTNet, FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cellOtpay2total.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cellOtpay2total.Colspan = 1;
                    cellOtpay2total.BorderWidthTop = 0.2f;
                    cellOtpay2total.BorderWidthBottom = 0.2f;
                    cellOtpay2total.BorderWidthLeft = 0;
                    cellOtpay2total.BorderWidthRight = 0;
                    Maintable.AddCell(cellOtpay2total);

                    PdfPCell cellGrand2total = new PdfPCell(new Phrase(TotalActualAmount.ToString(), FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cellGrand2total.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cellGrand2total.Colspan = 1;
                    cellGrand2total.BorderWidthTop = 0.2f;
                    cellGrand2total.BorderWidthBottom = 0.2f;
                    cellGrand2total.BorderWidthLeft = 0;
                    cellGrand2total.BorderWidthRight = 0;
                    Maintable.AddCell(cellGrand2total);


                    PdfPCell cellSign2total = new PdfPCell(new Phrase("", FontFactory.GetFont(fontsyle, Fontsize - 2, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cellSign2total.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    cellSign2total.Colspan = 1;
                    cellSign2total.BorderWidthTop = 0.2f;
                    cellSign2total.BorderWidthBottom = 0.2f;
                    cellSign2total.BorderWidthLeft = 0;
                    cellSign2total.BorderWidthRight = 0;
                    Maintable.AddCell(cellSign2total);

                    document.Add(Maintable);


                }


                string filename = "PaySheetSummary" + ".pdf";

                document.Close();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=\"" + filename + "\"");
                Response.Buffer = true;
                Response.Clear();
                Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                Response.OutputStream.Flush();
                Response.End();
            }
        }

        protected void lbtn_ExportNew_Click(object sender, EventArgs e)
        {
            try
            {

                string date = string.Empty;

                if (txtmonth.Text.Trim().Length > 0)
                {
                    date = DateTime.Parse(txtmonth.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
                }
                string monthnew = string.Empty;

                string month = DateTime.Parse(date).Month.ToString();
                string Year = DateTime.Parse(date).Year.ToString();
                var SPName = "";
                Hashtable HTPaysheet = new Hashtable();

                monthnew = month + Year.Substring(2, 2);
                SPName = "WageSheetCTCALL";
                HTPaysheet.Add("@month", monthnew);
                DataTable dt = config.ExecuteAdaptorAsyncWithParams(SPName, HTPaysheet).Result;
                if (dt.Rows.Count > 0)
                {
                    string filename = "PaySheetALL.xls";

                    var products = dt;
                    ExcelPackage excel = new ExcelPackage();
                    var workSheet = excel.Workbook.Worksheets.Add("WageSheetALL");
                    var totalCols = products.Columns.Count;
                    var totalRows = products.Rows.Count;

                    for (var col = 1; col <= totalCols; col++)
                    {
                        workSheet.Cells[1, col].Value = products.Columns[col - 1].ColumnName;
                        workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();
                    }
                    for (var row = 1; row <= totalRows; row++)
                    {
                        for (var col = 0; col < totalCols; col++)
                        {
                            workSheet.Cells[row + 1, col + 1].Value = products.Rows[row - 1][col];
                        }

                    }
                    using (var memoryStream = new MemoryStream())
                    {
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment ;filename=\"" + filename + "\"");
                        excel.SaveAs(memoryStream);
                        memoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "show alert", "alert('The details Are Not Available What you are searching...');", true);
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            GVClientsData.DataSource = null;
            GVClientsData.DataBind();
            btn_Submit.Visible = true;
            btngetdata.Visible = false;
            btndownloadpdf.Visible = false;
        }

        protected void ddltype_SelectedIndexChanged(object sender, EventArgs e)
        {
            GVListEmployees1.DataSource = null;
            GVListEmployees1.DataBind();
            lbtn_Export.Visible = false;
            lbtn_ExportNew.Visible = false;

        }

        protected void GVListEmployees1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[3].Attributes.Add("class", "text");
                e.Row.Cells[4].Attributes.Add("class", "text");
                e.Row.Cells[9].Attributes.Add("class", "text");
                e.Row.Cells[10].Attributes.Add("class", "text");
                e.Row.Cells[11].Attributes.Add("class", "text");
            }
        }
    }
}