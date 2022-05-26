using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using KLTS.Data;
using System.Globalization;
using ShriKartikeya.Portal.DAL;

namespace ShriKartikeya.Portal.Module_Reports
{
    public partial class ArrearPaySheetReport : System.Web.UI.Page
    {
        AppConfiguration config = new AppConfiguration();
        string EmpIDPrefix = "";
        string CmpIDPrefix = "";
        string Fontstyle = "";
        string CFontstyle = "";
        string BranchID = "";
        string Branch = "";
        string Emp_id = "";

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
                    FillClientList();
                    FillClientNameList();
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
            if (Session.Keys.Count > 0)
            {
                EmpIDPrefix = Session["EmpIDPrefix"].ToString();
                CmpIDPrefix = Session["CmpIDPrefix"].ToString();
                BranchID = Session["BranchID"].ToString();
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        protected void FillClientList()
        {
            string qry = "select Clientid from clients where clientid like '%" + CmpIDPrefix + "%'  and  Paysheet=1 order by clientid";
            DataTable DtClientNames = config.ExecuteReaderWithQueryAsync(qry).Result;
            if (DtClientNames.Rows.Count > 0)
            {
                ddlClientId.DataValueField = "Clientid";
                ddlClientId.DataTextField = "Clientid";
                ddlClientId.DataSource = DtClientNames;
                ddlClientId.DataBind();
            }
            ddlClientId.Items.Insert(0, "-Select-");
        }

        protected void FillClientNameList()
        {

            string qry = "select Clientid,Clientname from clients where clientid like '%" + CmpIDPrefix + "%'  and  Paysheet=1 order by clientname";
            DataTable DtClientids = config.ExecuteReaderWithQueryAsync(qry).Result;
            if (DtClientids.Rows.Count > 0)
            {
                ddlcname.DataValueField = "Clientid";
                ddlcname.DataTextField = "clientname";
                ddlcname.DataSource = DtClientids;
                ddlcname.DataBind();
            }
            ddlcname.Items.Insert(0, "-Select-");
        }

        protected void ddlcname_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ClearData();

            if (ddlcname.SelectedIndex > 0)
            {
                ddlClientId.SelectedValue = ddlcname.SelectedValue;
                if (txtmonth.Text != "")
                {
                    NonGeneratedPaysheetList();
                }
            }
            else
            {
                ddlClientId.SelectedIndex = 0;
            }
        }

        protected void ddlClientId_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearData();

            if (ddlClientId.SelectedIndex > 0)
            {
                ddlcname.SelectedValue = ddlClientId.SelectedValue;
                if (txtmonth.Text != "")
                {
                    NonGeneratedPaysheetList();
                }
            }
            else
            {
                ddlcname.SelectedIndex = 0;
            }
        }

        protected void btnsearch_Click(object sender, EventArgs e)
        {

            ClearData();
            var monthNew = 0;

            string month = DateTime.Parse(txtmonth.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).Month.ToString();
            string Year = DateTime.Parse(txtmonth.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).Year.ToString();
            DateTime date = DateTime.Now;


            DateTime firstday = DateTime.Now;
            firstday = GlobalData.Instance.GetFirstDayMonth(int.Parse(Year), int.Parse(month));
            string fday = firstday.ToShortDateString();

            #region Begin Code  For Validation as on [16-11-2013]
            if (ddlClientId.SelectedIndex == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "show alert", "alert('Please Select The client');", true);
                return;
            }

            if (txtmonth.Text.Trim().Length == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "show alert", "alert('Please Select The Month');", true);
                return;
            }

            #endregion End  Code  For Validation as on [16-11-2013]

            var Gendays = 0;
            var G_Sdays = 0;
            var bPaySheetDates = 0;

            monthNew = int.Parse(month + Year.Substring(2, 2));

            DateTime mGendays = DateTime.Now;

            DateTime dates = DateTime.Parse(txtmonth.Text, CultureInfo.GetCultureInfo("en-gb"));
            mGendays = DateTime.Parse(dates.ToString());

            string Qry = " select max(contractid),PaySheetDates PaySheetDates  from contracts where clientid='" + ddlClientId.SelectedValue + "' group by PaySheetDates";

            DataTable DtQry = config.ExecuteAdaptorAsyncWithQueryParams(Qry).Result;
            {
                bPaySheetDates = int.Parse(DtQry.Rows[0]["PaySheetDates"].ToString());
            }

            if (bPaySheetDates == 0)
            {
                Gendays = Timings.Instance.GetTotalNoofdaysInMonth(mGendays.Month, mGendays.Year);
            }
            else
            {
                if (mGendays.Month == 1)
                {
                    Gendays = Timings.Instance.GetTotalNoofdaysInMonth(mGendays.AddMonths(-1).Month, mGendays.AddYears(-1).Year);
                }
                else
                {
                    Gendays = Timings.Instance.GetTotalNoofdaysInMonth(mGendays.AddMonths(-1).Month, mGendays.Year);
                }
            }

            G_Sdays = Timings.Instance.Get_GS_Days(monthNew, Gendays);

            string monthdays = txtNoofdays.Text;
            string Spname = "GetArrearDetails";
            Hashtable ht = new Hashtable();
            ht.Add("@clientid", ddlClientId.SelectedValue);
            ht.Add("@month", month + Year.Substring(2, 2));
            ht.Add("@MonthDays", monthdays);
            ht.Add("@Gendays", Gendays);
            ht.Add("@G_Sdays", G_Sdays);

            DataTable dt = config.ExecuteAdaptorAsyncWithParams(Spname, ht).Result;


            Bindata(Spname, ht);

        }



        #region values declares


        float totalnoofdays = 0;
        float totalots = 0;
        float totalwo = 0;
        float totalnhs = 0;
        float totalsplduties = 0;
        float totaldays = 0;
        float totalsalaryrate = 0;
        float totalbasic = 0;
        float totalnewbasic = 0;
        float totalArrearbasic = 0;
        float totalda = 0;
        float totalnewda = 0;
        float totalArrearda = 0;
        float totalHRA = 0;
        float totalnewHRA = 0;
        float totalArrearHRA = 0;
        float totalCCA = 0;
        float totalnewCCA = 0;
        float totalArrearCCA = 0;
        float totalCONV = 0;
        float totalnewCONV = 0;
        float totalArrearCONV = 0;
        float totalWA = 0;
        float totalnewWA = 0;
        float totalArrearWA = 0;
        float totalOA = 0;
        float totalnewOA = 0;
        float totalArrearOA = 0;
        float totalLeaveEncashAmt = 0;
        float totalnewLeaveEncashAmt = 0;
        float totalArrearLeaveEncashAmt = 0;
        float totalGratuity = 0;
        float totalnewGratuity = 0;
        float totalArrearGratuity = 0;
        float totalnfhs = 0;
        float totalnewnfhs = 0;
        float totalArrearnfhs = 0;
        float totalbonus = 0;
        float totalnewbonus = 0;
        float totalArrearbonus = 0;
        float totalgross = 0;
        float totalnewgross = 0;
        float totalArreargross = 0;
        float totalPF = 0;
        float totalnewpf = 0;
        float totalarrearpf = 0;
        float totalesi = 0;
        float totalnewesi = 0;
        float totalarrearesi = 0;
        float totalarrearproftax = 0;
        float totalarrearnet = 0;
        float totalSpecialAllow = 0;
        float totalnewSpecialAllow = 0;
        float totalArrearSpecialAllow = 0;
        float totalProfAllow = 0;
        float totalnewProfAllow = 0;
        float totalArrearProfAllow = 0;
        float totalUniformAllow = 0;
        float totalnewUniformAllow = 0;
        float totalArrearUniformAllow = 0;
        float totalOTamt = 0;
        float totalnewOTamt = 0;
        float totalArrearOTamt = 0;
        float totalattbonus = 0;
        float totalnewattbonus = 0;
        float totalArrearattbonus = 0;
        float totalRC = 0;
        float totalnewRC = 0;
        float totalArrearRC = 0;
        float totalCS = 0;
        float totalnewCS = 0;
        float totalArrearCS = 0;
        float totalNHsAmt = 0;
        float totalnewNHsAmt = 0;
        float totalArrearNHsAmt = 0;
        float totalFoodAllw = 0;
        float totalnewFoodAllw = 0;
        float totalArrearFoodAllw = 0;
        float totalMedicalAllw = 0;
        float totalnewMedicalAllw = 0;
        float totalArrearMedicalAllw = 0;
        float totalTravelAllw = 0;
        float totalnewTravelAllw = 0;
        float totalArrearTravelAllw = 0;
        float totalMobileAllw = 0;
        float totalnewMobileAllw = 0;
        float totalArrearMobileAllw = 0;
        float totalPerformanceAllw = 0;
        float totalnewPerformanceAllw = 0;
        float totalArrearPerformanceAllw = 0;
        float totalNightShiftAllw = 0;
        float totalnewNightShiftAllw = 0;
        float totalArrearNightShiftAllw = 0;
        float totalArrears = 0;
        float totalnewArrears = 0;
        float totalArrearArrears = 0;
        float totalPLAmount = 0;
        float totalnewPLAmount = 0;
        float totalArrearPLAmount = 0;
        float totalOTHrsAmt = 0;
        float totalnewOTHrsAmt = 0;
        float totalArrearOTHrsAmt = 0;
        float totalWOAmt = 0;
        float totalnewWOAmt = 0;
        float totalArrearWOAmt = 0;
        float totalServiceWeightage = 0;
        float totalnewServiceWeightage = 0;
        float totalArrearServiceWeightage = 0;

        float totalIncetivs = 0;
        float totalnewIncentivs = 0;
        float totalArrearIncentivs = 0;

        float totalNpotsAmt = 0;
        float totalNewNpotsAmt = 0;
        float totalArrearNpotsAmt = 0;
        #endregion

        protected void Bindata(string Spname, Hashtable ht)
        {
            DataTable dt = config.ExecuteAdaptorAsyncWithParams(Spname, ht).Result;

            if (dt.Rows.Count > 0)
            {
                //labelDays.Visible = true;
                //txtNoofdays.Visible = true;
                btnGeneratePayment.Visible = true;
                //btnGenValues.Visible = true;
                btnCalculate.Visible = true;
                GVListEmployeess.DataSource = dt;
                GVListEmployeess.DataBind();
                // Fillpfandesidetails();

                for (int k = 0; k < GVListEmployeess.Rows.Count; k++)
                {
                    CheckBox chkindividual = GVListEmployeess.Rows[k].FindControl("chkindividual") as CheckBox;
                    chkindividual.Checked = true;
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string duties = dt.Rows[i]["NoOfDuties"].ToString();
                    if (duties.Trim().Length > 0)
                    {
                        totalnoofdays += Convert.ToSingle(duties);
                    }
                    string ots = dt.Rows[i]["OTs"].ToString();
                    if (ots.Trim().Length > 0)
                    {
                        totalots += Convert.ToSingle(ots);
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
                    string npots = dt.Rows[i]["npots"].ToString();
                    if (npots.Trim().Length > 0)
                    {
                        totalsplduties += Convert.ToSingle(npots);
                    }


                    string ntempgross = dt.Rows[i]["tempgross"].ToString();
                    if (ntempgross.Trim().Length > 0)
                    {
                        totalsalaryrate += Convert.ToSingle(ntempgross);
                    }

                    string strBasic = dt.Rows[i]["Basic"].ToString();
                    if (strBasic.Trim().Length > 0)
                    {
                        totalbasic += Convert.ToSingle(strBasic);
                    }
                    string strNewBasic = dt.Rows[i]["BasicSS"].ToString();
                    if (strNewBasic.Trim().Length > 0)
                    {
                        totalnewbasic += Convert.ToSingle(strNewBasic);
                    }
                    string strArrrearBasic = dt.Rows[i]["Basic"].ToString();
                    if (strArrrearBasic.Trim().Length > 0)
                    {
                        totalArrearbasic += Convert.ToSingle(strArrrearBasic);
                    }
                    string strDA = dt.Rows[i]["DA"].ToString();
                    if (strDA.Trim().Length > 0)
                    {
                        totalda += Convert.ToSingle(strDA);
                    }
                    string strNewDA = dt.Rows[i]["DASS"].ToString();
                    if (strNewDA.Trim().Length > 0)
                    {

                        totalnewda += Convert.ToSingle(strNewDA);
                    }



                    string strArrearDA = dt.Rows[i]["DA"].ToString();
                    if (strArrearDA.Trim().Length > 0)
                    {
                        totalArrearda += Convert.ToSingle(strArrearDA);
                    }
                    string strhHRA = dt.Rows[i]["HRA"].ToString();
                    if (strhHRA.Trim().Length > 0)
                    {
                        totalHRA += Convert.ToSingle(strhHRA);
                    }
                    string strNewHRA = dt.Rows[i]["HRASS"].ToString();
                    if (strNewHRA.Trim().Length > 0)
                    {
                        totalnewHRA += Convert.ToSingle(strNewHRA);
                    }
                    string strArrearHRA = dt.Rows[i]["HRA"].ToString();
                    if (strArrearHRA.Trim().Length > 0)
                    {
                        totalArrearHRA += Convert.ToSingle(strArrearHRA);
                    }
                    string strCCA = dt.Rows[i]["CCA"].ToString();
                    if (strCCA.Trim().Length > 0)
                    {
                        totalCCA += Convert.ToSingle(strCCA);
                    }
                    string strNewCCA = dt.Rows[i]["CCASS"].ToString();
                    if (strNewCCA.Trim().Length > 0)
                    {
                        totalnewCCA += Convert.ToSingle(strNewCCA);
                    }
                    string strArrearCCA = dt.Rows[i]["CCA"].ToString();
                    if (strArrearCCA.Trim().Length > 0)
                    {
                        totalArrearCCA += Convert.ToSingle(strArrearCCA);
                    }
                    string strConveyance = dt.Rows[i]["Conveyance"].ToString();
                    if (strConveyance.Trim().Length > 0)
                    {
                        totalCONV += Convert.ToSingle(strConveyance);
                    }

                    string strNewConveyance = dt.Rows[i]["ConveyanceSS"].ToString();
                    if (strNewConveyance.Trim().Length > 0)
                    {
                        totalnewCONV += Convert.ToSingle(strNewConveyance);
                    }
                    string strArrearConveyance = dt.Rows[i]["Conveyance"].ToString();
                    if (strArrearConveyance.Trim().Length > 0)
                    {
                        totalArrearCONV += Convert.ToSingle(strArrearConveyance);
                    }
                    string strIncentiv = dt.Rows[i]["Incentivs"].ToString();
                    if (strIncentiv.Trim().Length > 0)
                    {
                        totalIncetivs += Convert.ToSingle(strIncentiv);
                    }
                    string strNewIncentiv = dt.Rows[i]["IncentivsSS"].ToString();
                    if (strNewIncentiv.Trim().Length > 0)
                    {
                        totalnewIncentivs += Convert.ToSingle(strNewIncentiv);
                    }
                    string strArrearIncentiv = dt.Rows[i]["Incentivs"].ToString();
                    if (strArrearIncentiv.Trim().Length > 0)
                    {
                        totalArrearIncentivs += Convert.ToSingle(strArrearIncentiv);
                    }

                    string strWA = dt.Rows[i]["WashAllowance"].ToString();
                    if (strWA.Trim().Length > 0)
                    {
                        totalWA += Convert.ToSingle(strWA);
                    }
                    string strNewWA = dt.Rows[i]["WASS"].ToString();
                    if (strNewWA.Trim().Length > 0)
                    {
                        totalnewWA += Convert.ToSingle(strNewWA);
                    }
                    string strArrearWA = dt.Rows[i]["WashAllowance"].ToString();
                    if (strArrearWA.Trim().Length > 0)
                    {
                        totalArrearWA += Convert.ToSingle(strArrearWA);
                    }
                    string strOA = dt.Rows[i]["OtherAllowance"].ToString();
                    if (strOA.Trim().Length > 0)
                    {
                        totalOA += Convert.ToSingle(strOA);
                    }
                    string strNewOA = dt.Rows[i]["OASS"].ToString();
                    if (strNewOA.Trim().Length > 0)
                    {
                        totalnewOA += Convert.ToSingle(strNewOA);
                    }
                    string strArrearOA = dt.Rows[i]["OtherAllowance"].ToString();
                    if (strArrearOA.Trim().Length > 0)
                    {
                        totalArrearOA += Convert.ToSingle(strArrearOA);
                    }
                    string strLeaveEncashAmt = dt.Rows[i]["LeaveEncashAmt"].ToString();
                    if (strLeaveEncashAmt.Trim().Length > 0)
                    {
                        totalLeaveEncashAmt += Convert.ToSingle(strLeaveEncashAmt);
                    }
                    string strNewLeaveEncashAmt = dt.Rows[i]["LeaveAmountSS"].ToString();
                    if (strNewLeaveEncashAmt.Trim().Length > 0)
                    {
                        totalnewLeaveEncashAmt += Convert.ToSingle(strNewLeaveEncashAmt);
                    }
                    string strArrearLeaveEncashAmt = dt.Rows[i]["LeaveEncashAmt"].ToString();
                    if (strArrearLeaveEncashAmt.Trim().Length > 0)
                    {
                        totalArrearLeaveEncashAmt += Convert.ToSingle(strArrearLeaveEncashAmt);
                    }
                    string strGratuity = dt.Rows[i]["Gratuity"].ToString();
                    if (strGratuity.Trim().Length > 0)
                    {
                        totalGratuity += Convert.ToSingle(strGratuity);
                    }
                    string strNewGratuity = dt.Rows[i]["GratuitySS"].ToString();
                    if (strNewGratuity.Trim().Length > 0)
                    {
                        totalnewGratuity += Convert.ToSingle(strNewGratuity);
                    }
                    string strArrearGratuity = dt.Rows[i]["Gratuity"].ToString();
                    if (strArrearGratuity.Trim().Length > 0)
                    {
                        totalArrearGratuity += Convert.ToSingle(strArrearGratuity);
                    }
                    string strNfhs = dt.Rows[i]["Nfhs"].ToString();
                    if (strNfhs.Trim().Length > 0)
                    {
                        totalnfhs += Convert.ToSingle(strNfhs);
                    }
                    string strNewNfhs = dt.Rows[i]["NfhsSS"].ToString();
                    if (strNewNfhs.Trim().Length > 0)
                    {
                        totalnewnfhs += Convert.ToSingle(strNewNfhs);
                    }
                    string strArrearNfhs = dt.Rows[i]["Nfhs"].ToString();
                    if (strArrearNfhs.Trim().Length > 0)
                    {
                        totalArrearnfhs += Convert.ToSingle(strArrearNfhs);
                    }
                    string strBonus = dt.Rows[i]["Bonus"].ToString();
                    if (strBonus.Trim().Length > 0)
                    {
                        totalbonus += Convert.ToSingle(strBonus);
                    }
                    string strNewBonus = dt.Rows[i]["BonusSS"].ToString();
                    if (strNewBonus.Trim().Length > 0)
                    {
                        totalnewbonus += Convert.ToSingle(strNewBonus);
                    }
                    string strArrearBonus = dt.Rows[i]["Bonus"].ToString();
                    if (strArrearBonus.Trim().Length > 0)
                    {
                        totalArrearbonus += Convert.ToSingle(strArrearBonus);
                    }
                    //
                    string strattBonus = dt.Rows[i]["ATTBonus"].ToString();
                    if (strattBonus.Trim().Length > 0)
                    {
                        totalattbonus += Convert.ToSingle(strattBonus);
                    }
                    string strNewattBonus = dt.Rows[i]["attBonusSS"].ToString();
                    if (strNewattBonus.Trim().Length > 0)
                    {
                        totalnewattbonus += Convert.ToSingle(strNewattBonus);
                    }
                    string strArrearattBonus = dt.Rows[i]["attBonus"].ToString();
                    if (strArrearattBonus.Trim().Length > 0)
                    {
                        totalArrearattbonus += Convert.ToSingle(strArrearattBonus);
                    }
                    ////
                    string strSpecialAllow = dt.Rows[i]["PLAmount"].ToString();
                    if (strSpecialAllow.Trim().Length > 0)
                    {
                        totalPLAmount += Convert.ToSingle(strSpecialAllow);
                    }
                    string strNewSpecialAllow = dt.Rows[i]["PLAmountSS"].ToString();
                    if (strNewSpecialAllow.Trim().Length > 0)
                    {
                        totalnewPLAmount += Convert.ToSingle(strNewSpecialAllow);
                    }
                    string strArrearSpecialAllow = dt.Rows[i]["PLAmount"].ToString();
                    if (strArrearSpecialAllow.Trim().Length > 0)
                    {
                        totalArrearPLAmount += Convert.ToSingle(strArrearSpecialAllow);
                    }
                    ////
                    string strProfAllw = dt.Rows[i]["Performanceallowance"].ToString();
                    if (strProfAllw.Trim().Length > 0)
                    {
                        totalProfAllow += Convert.ToSingle(strProfAllw);
                    }
                    string strNewProfAllw = dt.Rows[i]["PerformanceAllowanceSS"].ToString();
                    if (strNewProfAllw.Trim().Length > 0)
                    {
                        totalnewProfAllow += Convert.ToSingle(strNewProfAllw);
                    }
                    string strArrearProfAllow = dt.Rows[i]["Performanceallowance"].ToString();
                    if (strArrearProfAllow.Trim().Length > 0)
                    {
                        totalArrearProfAllow += Convert.ToSingle(strArrearProfAllow);
                    }

                    string strUniformAllw = dt.Rows[i]["Arrears"].ToString();
                    if (strUniformAllw.Trim().Length > 0)
                    {
                        totalArrears += Convert.ToSingle(strUniformAllw);
                    }
                    string strNewUniformAllw = dt.Rows[i]["ArrearsSS"].ToString();
                    if (strNewUniformAllw.Trim().Length > 0)
                    {
                        totalnewArrears += Convert.ToSingle(strNewUniformAllw);
                    }
                    string strArrearUniformAllw = dt.Rows[i]["Arrears"].ToString();
                    if (strArrearUniformAllw.Trim().Length > 0)
                    {
                        totalArrearArrears += Convert.ToSingle(strArrearUniformAllw);
                    }

                    string strNpots = dt.Rows[i]["NpotsAmt"].ToString();
                    if (strNpots.Trim().Length > 0)
                    {
                        totalNpotsAmt += Convert.ToSingle(strNpots);
                    }
                    string strNewNpots = dt.Rows[i]["NpotsAmtss"].ToString();
                    if (strNewNpots.Trim().Length > 0)
                    {
                        totalNewNpotsAmt += Convert.ToSingle(strNewNpots);
                    }
                    string strArrearNpots = dt.Rows[i]["NpotsAmt"].ToString();
                    if (strArrearNpots.Trim().Length > 0)
                    {
                        totalArrearNpotsAmt += Convert.ToSingle(strArrearNpots);
                    }

                    string strOTAmt = dt.Rows[i]["OTAmt"].ToString();
                    if (strOTAmt.Trim().Length > 0)
                    {
                        totalOTamt += Convert.ToSingle(strOTAmt);
                    }
                    string strNewOTAmt = dt.Rows[i]["OTAmtss"].ToString();
                    if (strNewOTAmt.Trim().Length > 0)
                    {
                        totalnewOTamt += Convert.ToSingle(strNewOTAmt);
                    }
                    string strArrearOTamt = dt.Rows[i]["OTAmt"].ToString();
                    if (strArrearOTamt.Trim().Length > 0)
                    {
                        totalArrearOTamt += Convert.ToSingle(strArrearOTamt);
                    }
                    //
                    string strRC = dt.Rows[i]["RC"].ToString();
                    if (strRC.Trim().Length > 0)
                    {
                        totalRC += Convert.ToSingle(strRC);
                    }
                    string strNewRC = dt.Rows[i]["RCSS"].ToString();
                    if (strNewRC.Trim().Length > 0)
                    {
                        totalnewRC += Convert.ToSingle(strNewRC);
                    }
                    string strArrearRC = dt.Rows[i]["RC"].ToString();
                    if (strArrearRC.Trim().Length > 0)
                    {
                        totalArrearRC += Convert.ToSingle(strArrearRC);
                    }
                    //
                    string strCS = dt.Rows[i]["CS"].ToString();
                    if (strCS.Trim().Length > 0)
                    {
                        totalCS += Convert.ToSingle(strCS);
                    }
                    string strNewCS = dt.Rows[i]["CSSS"].ToString();
                    if (strNewCS.Trim().Length > 0)
                    {
                        totalnewCS += Convert.ToSingle(strNewCS);
                    }
                    string strArrearCS = dt.Rows[i]["CS"].ToString();
                    if (strArrearCS.Trim().Length > 0)
                    {
                        totalArrearCS += Convert.ToSingle(strArrearCS);
                    }

                    //
                    string strNhsAmt = dt.Rows[i]["NhsAmt"].ToString();
                    if (strNhsAmt.Trim().Length > 0)
                    {
                        totalNHsAmt += Convert.ToSingle(strNhsAmt);
                    }
                    string strNewNHsAmt = dt.Rows[i]["NhsAmtss"].ToString();
                    if (strNewNHsAmt.Trim().Length > 0)
                    {
                        totalnewNHsAmt += Convert.ToSingle(strNewNHsAmt);
                    }
                    string strArrearNHsAmt = dt.Rows[i]["NhsAmt"].ToString();
                    if (strArrearNHsAmt.Trim().Length > 0)
                    {
                        totalArrearNHsAmt += Convert.ToSingle(strArrearNHsAmt);
                    }

                    string strGross = dt.Rows[i]["Gross"].ToString();
                    if (strGross.Trim().Length > 0)
                    {
                        totalgross += Convert.ToSingle(strGross);
                    }
                    string strNewGross = dt.Rows[i]["Gross"].ToString();
                    if (strNewGross.Trim().Length > 0)
                    {
                        totalnewgross += Convert.ToSingle(strNewGross);
                    }
                    string strArrearGross = dt.Rows[i]["Gross"].ToString();
                    if (strArrearGross.Trim().Length > 0)
                    {
                        totalArreargross += Convert.ToSingle(strArrearGross);
                    }
                    string strPF = dt.Rows[i]["pf"].ToString();
                    if (strPF.Trim().Length > 0)
                    {
                        totalPF += Convert.ToSingle(strPF);
                    }
                    string strNewPF = dt.Rows[i]["pf"].ToString();
                    if (strNewPF.Trim().Length > 0)
                    {
                        totalnewpf += Convert.ToSingle(strNewPF);
                    }
                    string strArrearPF = dt.Rows[i]["pf"].ToString();
                    if (strArrearPF.Trim().Length > 0)
                    {
                        totalarrearpf += Convert.ToSingle(strArrearPF);
                    }
                    string strESI = dt.Rows[i]["esi"].ToString();
                    if (strESI.Trim().Length > 0)
                    {
                        totalesi += Convert.ToSingle(strESI);
                    }
                    string strNewESI = dt.Rows[i]["esi"].ToString();
                    if (strNewESI.Trim().Length > 0)
                    {
                        totalnewesi += Convert.ToSingle(strNewESI);
                    }
                    string strArrearESI = dt.Rows[i]["esi"].ToString();
                    if (strArrearESI.Trim().Length > 0)
                    {
                        totalarrearesi += Convert.ToSingle(strArrearESI);
                    }
                    string strArrearPT = dt.Rows[i]["proftax"].ToString();
                    if (strArrearPT.Trim().Length > 0)
                    {
                        totalarrearproftax += Convert.ToSingle(strArrearPT);
                    }

                    string strFoodAllw = dt.Rows[i]["FoodAllw"].ToString();
                    if (strFoodAllw.Trim().Length > 0)
                    {
                        totalFoodAllw += Convert.ToSingle(strFoodAllw);
                    }
                    string strNewFoodAllw = dt.Rows[i]["FoodAllwSS"].ToString();
                    if (strNewFoodAllw.Trim().Length > 0)
                    {
                        totalFoodAllw += Convert.ToSingle(strNewFoodAllw);
                    }
                    string strArrearFoodAllw = dt.Rows[i]["FoodAllw"].ToString();
                    if (strArrearFoodAllw.Trim().Length > 0)
                    {
                        totalArrearFoodAllw += Convert.ToSingle(strArrearFoodAllw);
                    }

                    string strMedicalAllw = dt.Rows[i]["MedicalAllw"].ToString();
                    if (strMedicalAllw.Trim().Length > 0)
                    {
                        totalMedicalAllw += Convert.ToSingle(strMedicalAllw);
                    }
                    string strNewMedicalAllw = dt.Rows[i]["MedicalAllwSS"].ToString();
                    if (strNewMedicalAllw.Trim().Length > 0)
                    {
                        totalnewMedicalAllw += Convert.ToSingle(strNewMedicalAllw);
                    }
                    string strArrearMedicalAllw = dt.Rows[i]["MedicalAllw"].ToString();
                    if (strArrearMedicalAllw.Trim().Length > 0)
                    {
                        totalArrearMedicalAllw += Convert.ToSingle(strArrearMedicalAllw);
                    }

                    string strSpecialAllw = dt.Rows[i]["splallowance"].ToString();
                    if (strSpecialAllw.Trim().Length > 0)
                    {
                        totalSpecialAllow += Convert.ToSingle(strSpecialAllw);
                    }
                    string strNewSpecialAllw = dt.Rows[i]["splSS"].ToString();
                    if (strNewSpecialAllw.Trim().Length > 0)
                    {
                        totalnewSpecialAllow += Convert.ToSingle(strNewSpecialAllw);
                    }
                    string strArrearspecialAllw = dt.Rows[i]["splallowance"].ToString();
                    if (strArrearspecialAllw.Trim().Length > 0)
                    {
                        totalArrearSpecialAllow += Convert.ToSingle(strArrearspecialAllw);
                    }

                    string strTravelAllw = dt.Rows[i]["TravelAllw"].ToString();
                    if (strTravelAllw.Trim().Length > 0)
                    {
                        totalTravelAllw += Convert.ToSingle(strTravelAllw);
                    }
                    string strNewTravelAllw = dt.Rows[i]["TravelAllwSS"].ToString();
                    if (strNewTravelAllw.Trim().Length > 0)
                    {
                        totalnewTravelAllw += Convert.ToSingle(strNewTravelAllw);
                    }
                    string strArrearTravelAllw = dt.Rows[i]["TravelAllw"].ToString();
                    if (strArrearTravelAllw.Trim().Length > 0)
                    {
                        totalnewTravelAllw += Convert.ToSingle(strArrearTravelAllw);
                    }

                    string strMobileAllw = dt.Rows[i]["MobileAllw"].ToString();
                    if (strMobileAllw.Trim().Length > 0)
                    {
                        totalMobileAllw += Convert.ToSingle(strMobileAllw);
                    }
                    string strNewMobileAllw = dt.Rows[i]["MobileAllwSS"].ToString();
                    if (strNewMobileAllw.Trim().Length > 0)
                    {
                        totalnewMobileAllw += Convert.ToSingle(strNewMobileAllw);
                    }
                    string strArrearMobileAllw = dt.Rows[i]["MobileAllw"].ToString();
                    if (strArrearMobileAllw.Trim().Length > 0)
                    {
                        totalArrearMobileAllw += Convert.ToSingle(strArrearMobileAllw);
                    }

                    string strNightAllw = dt.Rows[i]["NightAllw"].ToString();
                    if (strNightAllw.Trim().Length > 0)
                    {
                        totalNightShiftAllw += Convert.ToSingle(strNightAllw);
                    }
                    string strNewNightAllw = dt.Rows[i]["NightAllwSS"].ToString();
                    if (strNewNightAllw.Trim().Length > 0)
                    {
                        totalnewNightShiftAllw += Convert.ToSingle(strNewNightAllw);
                    }
                    string strArrearNightAllw = dt.Rows[i]["NightAllw"].ToString();
                    if (strArrearNightAllw.Trim().Length > 0)
                    {
                        totalArrearNightShiftAllw += Convert.ToSingle(strArrearNightAllw);
                    }

                    string strRLAmount = dt.Rows[i]["OTHrsAmt"].ToString();
                    if (strRLAmount.Trim().Length > 0)
                    {
                        totalOTHrsAmt += Convert.ToSingle(strRLAmount);
                    }
                    string strNewRLAmount = dt.Rows[i]["OTHrsAmtSS"].ToString();
                    if (strNewRLAmount.Trim().Length > 0)
                    {
                        totalnewOTHrsAmt += Convert.ToSingle(strNewRLAmount);
                    }
                    string strArrearRLAmount = dt.Rows[i]["OTHrsAmt"].ToString();
                    if (strArrearRLAmount.Trim().Length > 0)
                    {
                        totalArrearOTHrsAmt += Convert.ToSingle(strArrearRLAmount);
                    }

                    string strWOAmount = dt.Rows[i]["WOAmount"].ToString();
                    if (strWOAmount.Trim().Length > 0)
                    {
                        totalWOAmt += Convert.ToSingle(strWOAmount);
                    }
                    string strNewWOAmount = dt.Rows[i]["WOAmountSS"].ToString();
                    if (strNewWOAmount.Trim().Length > 0)
                    {
                        totalnewWOAmt += Convert.ToSingle(strNewWOAmount);
                    }
                    string strArrearWOAmount = dt.Rows[i]["WOAmount"].ToString();
                    if (strArrearWOAmount.Trim().Length > 0)
                    {
                        totalArrearWOAmt += Convert.ToSingle(strArrearWOAmount);
                    }

                    string strServiceWeightage = dt.Rows[i]["ServiceWeightage"].ToString();
                    if (strWOAmount.Trim().Length > 0)
                    {
                        totalServiceWeightage += Convert.ToSingle(strServiceWeightage);
                    }
                    string strNewSericeWeightage = dt.Rows[i]["ServiceWeightageSS"].ToString();
                    if (strNewSericeWeightage.Trim().Length > 0)
                    {
                        totalnewServiceWeightage += Convert.ToSingle(strNewSericeWeightage);
                    }
                    string strArrearSericeWeightage = dt.Rows[i]["ServiceWeightage"].ToString();
                    if (strArrearSericeWeightage.Trim().Length > 0)
                    {
                        totalArrearServiceWeightage += Convert.ToSingle(strArrearSericeWeightage);
                    }

                    Label lblTotalNoDays = GVListEmployeess.FooterRow.FindControl("lblTotalNoDays") as Label;
                    lblTotalNoDays.Text = Math.Round(totalnoofdays).ToString();



                    Label lblTotalOTs = GVListEmployeess.FooterRow.FindControl("lblTotalOTs") as Label;
                    lblTotalOTs.Text = Math.Round(totalots).ToString();

                    if (totalots > 0)
                    {
                        GVListEmployeess.Columns[6].Visible = true;
                    }
                    else
                    {
                        GVListEmployeess.Columns[6].Visible = false;

                    }
                    Label lblTotalwos = GVListEmployeess.FooterRow.FindControl("lblTotalWOs") as Label;
                    lblTotalwos.Text = Math.Round(totalwo).ToString();

                    if (totalwo > 0)
                    {
                        GVListEmployeess.Columns[7].Visible = true;
                    }
                    else
                    {
                        GVListEmployeess.Columns[7].Visible = false;

                    }

                    Label lblTotalNhs = GVListEmployeess.FooterRow.FindControl("lblTotalNHs") as Label;
                    lblTotalNhs.Text = Math.Round(totalnhs).ToString();

                    if (totalnhs > 0)
                    {
                        GVListEmployeess.Columns[8].Visible = true;
                    }
                    else
                    {
                        GVListEmployeess.Columns[8].Visible = false;

                    }



                    Label lblTotalNpots = GVListEmployeess.FooterRow.FindControl("lblTotalsplduties") as Label;
                    lblTotalNpots.Text = Math.Round(totalsplduties).ToString();

                    if (totalsplduties > 0)
                    {
                        GVListEmployeess.Columns[9].Visible = true;
                    }
                    else
                    {
                        GVListEmployeess.Columns[9].Visible = false;

                    }
                    Label lblTotaltotaldays = GVListEmployeess.FooterRow.FindControl("lblTotaltotaldays") as Label;
                    lblTotaltotaldays.Text = Math.Round(totaldays).ToString();

                    if (totaldays > 0)
                    {
                        GVListEmployeess.Columns[10].Visible = true;
                    }
                    else
                    {
                        GVListEmployeess.Columns[10].Visible = false;

                    }

                    Label lblTotalSrate = GVListEmployeess.FooterRow.FindControl("lblTotalSrate") as Label;
                    lblTotalSrate.Text = Math.Round(totalsalaryrate).ToString();


                    Label lblTotabasic = GVListEmployeess.FooterRow.FindControl("lblTotabasic") as Label;
                    lblTotabasic.Text = Math.Round(totalbasic).ToString();
                    if (totalbasic > 0)
                    {
                        GVListEmployeess.Columns[12].Visible = true;
                    }
                    else
                    {
                        GVListEmployeess.Columns[12].Visible = false;

                    }
                    Label lblTotalnewbasic = GVListEmployeess.FooterRow.FindControl("lblTotalnewbasic") as Label;
                    lblTotalnewbasic.Text = Math.Round(totalnewbasic).ToString();
                    if (totalnewbasic > 0)
                    {
                        GVListEmployeess.Columns[13].Visible = true;
                    }
                    else
                    {
                        GVListEmployeess.Columns[13].Visible = false;

                    }
                    Label lblTotalArrearbasic = GVListEmployeess.FooterRow.FindControl("lblTotalArrearBasic") as Label;
                    lblTotalArrearbasic.Text = Math.Round(totalArrearbasic).ToString();
                    if (totalArrearbasic > 0)
                    {
                        GVListEmployeess.Columns[14].Visible = true;
                    }
                    else
                    {
                        GVListEmployeess.Columns[14].Visible = false;

                    }

                    Label lblTotalDA = GVListEmployeess.FooterRow.FindControl("lblTotalDA") as Label;
                    lblTotalDA.Text = Math.Round(totalda).ToString();



                    if ((totalda + totalnewda) > 0)
                    {
                        GVListEmployeess.Columns[15].Visible = true;
                        GVListEmployeess.Columns[16].Visible = true;
                        GVListEmployeess.Columns[17].Visible = true;
                    }
                    else
                    {
                        GVListEmployeess.Columns[15].Visible = false;
                        GVListEmployeess.Columns[16].Visible = false;
                        GVListEmployeess.Columns[17].Visible = false;
                    }
                    Label lblTotalHRA = GVListEmployeess.FooterRow.FindControl("lblTotalhra") as Label;
                    lblTotalHRA.Text = Math.Round(totalHRA).ToString();

                    if ((totalHRA + totalnewHRA) > 0)
                    {
                        GVListEmployeess.Columns[18].Visible = true;
                        GVListEmployeess.Columns[19].Visible = true;
                        GVListEmployeess.Columns[20].Visible = true;
                    }
                    else
                    {
                        GVListEmployeess.Columns[18].Visible = false;
                        GVListEmployeess.Columns[19].Visible = false;
                        GVListEmployeess.Columns[20].Visible = false;
                    }

                    Label lblTotalCCA = GVListEmployeess.FooterRow.FindControl("lblTotalcca") as Label;
                    lblTotalCCA.Text = Math.Round(totalCCA).ToString();

                    if ((totalCCA + totalnewCCA) > 0)
                    {
                        GVListEmployeess.Columns[21].Visible = true;
                        GVListEmployeess.Columns[22].Visible = true;
                        GVListEmployeess.Columns[23].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[21].Visible = false;
                        GVListEmployeess.Columns[22].Visible = false;
                        GVListEmployeess.Columns[23].Visible = false;

                    }


                    Label lblTotalConveyance = GVListEmployeess.FooterRow.FindControl("lblTotalConv") as Label;
                    lblTotalConveyance.Text = Math.Round(totalCONV).ToString();

                    if ((totalCONV + totalnewCONV) > 0)
                    {
                        GVListEmployeess.Columns[24].Visible = true;
                        GVListEmployeess.Columns[25].Visible = true;
                        GVListEmployeess.Columns[26].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[24].Visible = false;
                        GVListEmployeess.Columns[25].Visible = false;
                        GVListEmployeess.Columns[26].Visible = false;

                    }

                    Label lblTotalIncentivs = GVListEmployeess.FooterRow.FindControl("lblTotalIncentivs") as Label;
                    lblTotalIncentivs.Text = Math.Round(totalIncetivs).ToString();

                    if ((totalIncetivs + totalnewIncentivs) > 0)
                    {
                        GVListEmployeess.Columns[27].Visible = true;
                        GVListEmployeess.Columns[28].Visible = true;
                        GVListEmployeess.Columns[29].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[27].Visible = false;
                        GVListEmployeess.Columns[28].Visible = false;
                        GVListEmployeess.Columns[29].Visible = false;

                    }




                    Label lblTotalOA = GVListEmployeess.FooterRow.FindControl("lblTotalOA") as Label;
                    lblTotalOA.Text = Math.Round(totalOA).ToString();

                    if ((totalOA + totalnewOA) > 0)
                    {
                        GVListEmployeess.Columns[30].Visible = true;
                        GVListEmployeess.Columns[31].Visible = true;
                        GVListEmployeess.Columns[32].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[30].Visible = false;
                        GVListEmployeess.Columns[31].Visible = false;
                        GVListEmployeess.Columns[32].Visible = false;

                    }
                    Label lblTotalLeaveEncashAmt = GVListEmployeess.FooterRow.FindControl("lblTotalLeaveEncashAmt") as Label;
                    lblTotalLeaveEncashAmt.Text = Math.Round(totalLeaveEncashAmt).ToString();

                    if ((totalLeaveEncashAmt + totalnewLeaveEncashAmt) > 0)
                    {
                        GVListEmployeess.Columns[33].Visible = true;
                        GVListEmployeess.Columns[34].Visible = true;
                        GVListEmployeess.Columns[35].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[33].Visible = false;
                        GVListEmployeess.Columns[34].Visible = false;
                        GVListEmployeess.Columns[35].Visible = false;

                    }

                    Label lblTotalGratuity = GVListEmployeess.FooterRow.FindControl("lblTotalGratuity") as Label;
                    lblTotalGratuity.Text = Math.Round(totalGratuity).ToString();

                    if ((totalGratuity + totalnewGratuity) > 0)
                    {
                        GVListEmployeess.Columns[36].Visible = true;
                        GVListEmployeess.Columns[37].Visible = true;
                        GVListEmployeess.Columns[38].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[36].Visible = false;
                        GVListEmployeess.Columns[37].Visible = false;
                        GVListEmployeess.Columns[38].Visible = false;

                    }
                    Label lblTotalNfhs = GVListEmployeess.FooterRow.FindControl("lblTotalNfhs") as Label;
                    lblTotalNfhs.Text = Math.Round(totalnfhs).ToString();

                    if ((totalnfhs + totalnewnfhs) > 0)
                    {
                        GVListEmployeess.Columns[39].Visible = true;
                        GVListEmployeess.Columns[40].Visible = true;
                        GVListEmployeess.Columns[41].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[39].Visible = false;
                        GVListEmployeess.Columns[40].Visible = false;
                        GVListEmployeess.Columns[41].Visible = false;
                    }
                    Label lblTotalBonus = GVListEmployeess.FooterRow.FindControl("lblTotalBonus") as Label;
                    lblTotalBonus.Text = Math.Round(totalbonus).ToString();

                    if ((totalbonus + totalnewbonus) > 0)
                    {
                        GVListEmployeess.Columns[42].Visible = true;
                        GVListEmployeess.Columns[43].Visible = true;
                        GVListEmployeess.Columns[44].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[42].Visible = false;
                        GVListEmployeess.Columns[43].Visible = false;
                        GVListEmployeess.Columns[44].Visible = false;

                    }
                    //
                    Label lblTotalAttBonus = GVListEmployeess.FooterRow.FindControl("lblTotalAttBonus") as Label;
                    lblTotalAttBonus.Text = Math.Round(totalattbonus).ToString();

                    if ((totalattbonus + totalnewattbonus) > 0)
                    {
                        GVListEmployeess.Columns[45].Visible = true;
                        GVListEmployeess.Columns[46].Visible = true;
                        GVListEmployeess.Columns[47].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[45].Visible = false;
                        GVListEmployeess.Columns[46].Visible = false;
                        GVListEmployeess.Columns[47].Visible = false;

                    }


                    Label lblTotalPLAmount = GVListEmployeess.FooterRow.FindControl("lblTotalPLAmount") as Label;
                    lblTotalPLAmount.Text = Math.Round(totalPLAmount).ToString();


                    if ((totalPLAmount + totalnewPLAmount) > 0)
                    {
                        GVListEmployeess.Columns[48].Visible = true;
                        GVListEmployeess.Columns[49].Visible = true;
                        GVListEmployeess.Columns[50].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[48].Visible = false;
                        GVListEmployeess.Columns[49].Visible = false;
                        GVListEmployeess.Columns[50].Visible = false;

                    }


                    Label lblTotalProfAllw = GVListEmployeess.FooterRow.FindControl("lblTotalProfAllw") as Label;
                    lblTotalProfAllw.Text = Math.Round(totalProfAllow).ToString();


                    if ((totalProfAllow + totalnewProfAllow) > 0)
                    {
                        GVListEmployeess.Columns[51].Visible = true;
                        GVListEmployeess.Columns[52].Visible = true;
                        GVListEmployeess.Columns[53].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[51].Visible = false;
                        GVListEmployeess.Columns[52].Visible = false;
                        GVListEmployeess.Columns[53].Visible = false;

                    }
                    //
                    Label lblTotalArrears = GVListEmployeess.FooterRow.FindControl("lblTotalArrears") as Label;
                    lblTotalArrears.Text = Math.Round(totalArrears).ToString();


                    if ((totalArrears + totalnewArrears) > 0)
                    {
                        GVListEmployeess.Columns[54].Visible = true;
                        GVListEmployeess.Columns[55].Visible = true;
                        GVListEmployeess.Columns[56].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[54].Visible = false;
                        GVListEmployeess.Columns[55].Visible = false;
                        GVListEmployeess.Columns[56].Visible = false;

                    }


                    //
                    Label lblTotalRC = GVListEmployeess.FooterRow.FindControl("lblTotalRC") as Label;
                    lblTotalRC.Text = Math.Round(totalRC).ToString();


                    if ((totalRC + totalnewRC) > 0)
                    {
                        GVListEmployeess.Columns[57].Visible = true;
                        GVListEmployeess.Columns[58].Visible = true;
                        GVListEmployeess.Columns[59].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[57].Visible = false;
                        GVListEmployeess.Columns[58].Visible = false;
                        GVListEmployeess.Columns[59].Visible = false;

                    }
                    //
                    Label lblTotalCS = GVListEmployeess.FooterRow.FindControl("lblTotalCS") as Label;
                    lblTotalCS.Text = Math.Round(totalCS).ToString();


                    if ((totalCS + totalnewCS) > 0)
                    {
                        GVListEmployeess.Columns[60].Visible = true;
                        GVListEmployeess.Columns[61].Visible = true;
                        GVListEmployeess.Columns[62].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[60].Visible = false;
                        GVListEmployeess.Columns[61].Visible = false;
                        GVListEmployeess.Columns[62].Visible = false;

                    }

                    Label lblTotalFoodAllw = GVListEmployeess.FooterRow.FindControl("lblTotalFoodAllw") as Label;
                    lblTotalFoodAllw.Text = Math.Round(totalFoodAllw).ToString();


                    if ((totalFoodAllw + totalnewFoodAllw) > 0)
                    {
                        GVListEmployeess.Columns[63].Visible = true;
                        GVListEmployeess.Columns[64].Visible = true;
                        GVListEmployeess.Columns[65].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[63].Visible = false;
                        GVListEmployeess.Columns[64].Visible = false;
                        GVListEmployeess.Columns[65].Visible = false;

                    }

                    Label lblTotalMedicalAllw = GVListEmployeess.FooterRow.FindControl("lblTotalMedicalAllw") as Label;
                    lblTotalMedicalAllw.Text = Math.Round(totalMedicalAllw).ToString();


                    if ((totalMedicalAllw + totalnewMedicalAllw) > 0)
                    {
                        GVListEmployeess.Columns[66].Visible = true;
                        GVListEmployeess.Columns[67].Visible = true;
                        GVListEmployeess.Columns[68].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[66].Visible = false;
                        GVListEmployeess.Columns[67].Visible = false;
                        GVListEmployeess.Columns[68].Visible = false;

                    }

                    Label lblTotalTravelAllw = GVListEmployeess.FooterRow.FindControl("lblTotalTravelAllw") as Label;
                    lblTotalTravelAllw.Text = Math.Round(totalTravelAllw).ToString();


                    if ((totalTravelAllw + totalnewTravelAllw) > 0)
                    {
                        GVListEmployeess.Columns[69].Visible = true;
                        GVListEmployeess.Columns[70].Visible = true;
                        GVListEmployeess.Columns[71].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[69].Visible = false;
                        GVListEmployeess.Columns[70].Visible = false;
                        GVListEmployeess.Columns[71].Visible = false;

                    }

                    Label lblTotalMobileAllw = GVListEmployeess.FooterRow.FindControl("lblTotalMobileAllw") as Label;
                    lblTotalMobileAllw.Text = Math.Round(totalMobileAllw).ToString();


                    if ((totalMobileAllw + totalnewMobileAllw) > 0)
                    {
                        GVListEmployeess.Columns[72].Visible = true;
                        GVListEmployeess.Columns[73].Visible = true;
                        GVListEmployeess.Columns[74].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[72].Visible = false;
                        GVListEmployeess.Columns[73].Visible = false;
                        GVListEmployeess.Columns[74].Visible = false;

                    }

                    Label lblTotalServiceWeightage = GVListEmployeess.FooterRow.FindControl("lblTotalServiceWeightage") as Label;
                    lblTotalServiceWeightage.Text = Math.Round(totalServiceWeightage).ToString();


                    if ((totalServiceWeightage + totalnewServiceWeightage) > 0)
                    {
                        GVListEmployeess.Columns[75].Visible = true;
                        GVListEmployeess.Columns[76].Visible = true;
                        GVListEmployeess.Columns[77].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[75].Visible = false;
                        GVListEmployeess.Columns[76].Visible = false;
                        GVListEmployeess.Columns[77].Visible = false;

                    }

                    Label lblTotalNightShiftAllw = GVListEmployeess.FooterRow.FindControl("lblTotalNightShiftAllw") as Label;
                    lblTotalNightShiftAllw.Text = Math.Round(totalNightShiftAllw).ToString();


                    if ((totalNightShiftAllw + totalnewNightShiftAllw) > 0)
                    {
                        GVListEmployeess.Columns[78].Visible = true;
                        GVListEmployeess.Columns[79].Visible = true;
                        GVListEmployeess.Columns[80].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[78].Visible = false;
                        GVListEmployeess.Columns[79].Visible = false;
                        GVListEmployeess.Columns[80].Visible = false;

                    }

                    Label lblTotalSplAllw = GVListEmployeess.FooterRow.FindControl("lblTotalSplAllw") as Label;
                    lblTotalSplAllw.Text = Math.Round(totalSpecialAllow).ToString();


                    if ((totalSpecialAllow + totalnewSpecialAllow) > 0)
                    {
                        GVListEmployeess.Columns[81].Visible = true;
                        GVListEmployeess.Columns[82].Visible = true;
                        GVListEmployeess.Columns[83].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[81].Visible = false;
                        GVListEmployeess.Columns[82].Visible = false;
                        GVListEmployeess.Columns[83].Visible = false;

                    }

                    Label lblTotalOTHrsAmt = GVListEmployeess.FooterRow.FindControl("lblTotalOTHrsAmt") as Label;
                    lblTotalOTHrsAmt.Text = Math.Round(totalOTHrsAmt).ToString();


                    if ((totalOTHrsAmt + totalnewOTHrsAmt) > 0)
                    {
                        GVListEmployeess.Columns[84].Visible = true;
                        GVListEmployeess.Columns[85].Visible = true;
                        GVListEmployeess.Columns[86].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[84].Visible = false;
                        GVListEmployeess.Columns[85].Visible = false;
                        GVListEmployeess.Columns[86].Visible = false;
                    }

                    Label lblTotalWOAmount = GVListEmployeess.FooterRow.FindControl("lblTotalWOAmount") as Label;
                    lblTotalWOAmount.Text = Math.Round(totalWOAmt).ToString();


                    if ((totalWOAmt + totalnewWOAmt) > 0)
                    {
                        GVListEmployeess.Columns[87].Visible = true;
                        GVListEmployeess.Columns[88].Visible = true;
                        GVListEmployeess.Columns[89].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[87].Visible = false;
                        GVListEmployeess.Columns[88].Visible = false;
                        GVListEmployeess.Columns[89].Visible = false;
                    }

                    Label lblTotalOTamt = GVListEmployeess.FooterRow.FindControl("lblTotalOTamt") as Label;
                    lblTotalOTamt.Text = Math.Round(totalOTamt).ToString();


                    if ((totalOTamt + totalnewOTamt) > 0)
                    {
                        GVListEmployeess.Columns[90].Visible = true;
                        GVListEmployeess.Columns[91].Visible = true;
                        GVListEmployeess.Columns[92].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[90].Visible = false;
                        GVListEmployeess.Columns[91].Visible = false;
                        GVListEmployeess.Columns[92].Visible = false;

                    }
                    //
                    Label lblTotalNHsamt = GVListEmployeess.FooterRow.FindControl("lblTotalNHsamt") as Label;
                    lblTotalNHsamt.Text = Math.Round(totalNHsAmt).ToString();


                    if ((totalNHsAmt + totalnewNHsAmt) > 0)
                    {
                        GVListEmployeess.Columns[93].Visible = true;
                        GVListEmployeess.Columns[94].Visible = true;
                        GVListEmployeess.Columns[95].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[93].Visible = false;
                        GVListEmployeess.Columns[94].Visible = false;
                        GVListEmployeess.Columns[95].Visible = false;

                    }

                    Label lblTotalNpotsamt = GVListEmployeess.FooterRow.FindControl("lblTotalNpotsamt") as Label;
                    lblTotalNpotsamt.Text = Math.Round(totalNpotsAmt).ToString();


                    if ((totalNpotsAmt + totalNewNpotsAmt) > 0)
                    {
                        GVListEmployeess.Columns[96].Visible = true;
                        GVListEmployeess.Columns[97].Visible = true;
                        GVListEmployeess.Columns[98].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[96].Visible = false;
                        GVListEmployeess.Columns[97].Visible = false;
                        GVListEmployeess.Columns[98].Visible = false;

                    }

                    Label lblTotalWA = GVListEmployeess.FooterRow.FindControl("lblTotalWA") as Label;
                    lblTotalWA.Text = Math.Round(totalWA).ToString();

                    if ((totalWA + totalnewWA) > 0)
                    {
                        GVListEmployeess.Columns[99].Visible = true;
                        GVListEmployeess.Columns[100].Visible = true;
                        GVListEmployeess.Columns[101].Visible = true;

                    }
                    else
                    {
                        GVListEmployeess.Columns[99].Visible = false;
                        GVListEmployeess.Columns[100].Visible = false;
                        GVListEmployeess.Columns[101].Visible = false;

                    }


                    

                    Label lblTotalArrearPT = GVListEmployeess.FooterRow.FindControl("lblTotalArrearPT") as Label;
                    lblTotalArrearPT.Text = Math.Round(totalarrearproftax).ToString();
                    if (totalarrearproftax > 0)
                    {
                        GVListEmployeess.Columns[109].Visible = true;
                    }
                    else
                    {
                        GVListEmployeess.Columns[109].Visible = false;

                    }

                }
            }
            else
            {
                ClearData();
                ScriptManager.RegisterStartupScript(this, GetType(), "showlalert", "alert('There Is No Paysheet Details For The Selected client');", true);

            }


        }

        protected void ClearData()
        {
            //labelDays.Visible = false;
            //txtNoofdays.Visible = false;
            btnGeneratePayment.Visible = false;
            btnCalculate.Visible = false;
            GVListEmployeess.DataSource = null;
            GVListEmployeess.DataBind();

        }



        float Basic = 0;
        float DA = 0;
        float HRA = 0;
        float CCA = 0;
        float Conv = 0;
        float WA = 0;
        float OA = 0;
        float LA = 0;
        float Gratuity = 0;
        float NFHS = 0;
        float Bonus = 0;


        string Designation = "";
        decimal GetTextboxValue(string textboxText)
        {
            return Convert.ToDecimal(string.IsNullOrEmpty(textboxText) ? "0" : textboxText);
        }


        protected void btnCalculate_Click(object sender, EventArgs e)
        {

            DateTime DtLastDay = DateTime.Now;
            DtLastDay = DateTime.Parse(txtmonth.Text, CultureInfo.GetCultureInfo("en-gb"));

            var ContractID = "";

            #region  Begin Get Contract Id Based on The Last Day
            Hashtable HtGetContractID = new Hashtable();
            var SPNameForGetContractID = "GetContractIDBasedOnthMonth";
            HtGetContractID.Add("@clientid", ddlClientId.SelectedValue);
            HtGetContractID.Add("@LastDay", DtLastDay);
            DataTable DTContractID = config.ExecuteAdaptorWithParams(SPNameForGetContractID, HtGetContractID).Result;
            if (DTContractID.Rows.Count > 0)
            {
                ContractID = DTContractID.Rows[0]["contractid"].ToString();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "", "alert('Contract Details Are Not Avaialable For This Client.');", true);
                return;
            }
            #endregion  End Get Contract Id Based on The Last Day





            #region for Contracts Details


            string query = "select PF,esi,pffrom,esifrom,pfonot,esionot,esilimit,pflimit from Contracts where ClientId='" + ddlClientId.SelectedValue + "' and contractid='" + ContractID + "'";
            DataTable dtquery = config.ExecuteReaderWithQueryAsync(query).Result;

            decimal pf = 100, esi = 100, PFEmployee = 12, ESIEmployee = 1, PFEmployer = 14, ESIEmployer = 3, esilimit = 0, pflimit = 0;
            int pffrom = 0, esifrom = 0, pfon = 0, esion = 0;
            if (dtquery.Rows.Count > 0)
            {
                pf = Convert.ToDecimal(dtquery.Rows[0]["PF"].ToString());
                esi = Convert.ToDecimal(dtquery.Rows[0]["esi"].ToString());
                pffrom = Convert.ToInt32(dtquery.Rows[0]["pffrom"].ToString());
                esifrom = Convert.ToInt32(dtquery.Rows[0]["esifrom"].ToString());
                pfon = Convert.ToInt32(dtquery.Rows[0]["PFonOT"].ToString());
                esion = Convert.ToInt32(dtquery.Rows[0]["ESIonOT"].ToString());
                esilimit = Convert.ToDecimal(dtquery.Rows[0]["esilimit"].ToString());
                pflimit = Convert.ToDecimal(dtquery.Rows[0]["pflimit"].ToString());
            }


            #endregion for Contracts Details

            #region for tbloptions

            query = "select PFEmployee,ESIEmployee,PFEmployer,ESIEmployer from TblOptions where '" + DtLastDay + "' between fromdate and todate ";
            DataTable dttbloptions = config.ExecuteReaderWithQueryAsync(query).Result;
            if (dttbloptions.Rows.Count > 0)
            {
                PFEmployee = Convert.ToDecimal(dttbloptions.Rows[0]["PFEmployee"].ToString());
                ESIEmployee = Convert.ToDecimal(dttbloptions.Rows[0]["ESIEmployee"].ToString());
                PFEmployer = Convert.ToDecimal(dttbloptions.Rows[0]["PFEmployer"].ToString());
                ESIEmployer = Convert.ToDecimal(dttbloptions.Rows[0]["ESIEmployer"].ToString());

            }

            #endregion for tbloptions

            #region for ProfTax

            query = "select * from proftax ";
            DataTable dtproftax = config.ExecuteReaderWithQueryAsync(query).Result;


            #endregion for ProfTax


            if (txtNoofdays.Text == "")
            {
                txtNoofdays.Text = "31";
            }

            decimal noofdays = Convert.ToDecimal(txtNoofdays.Text);
            decimal TotalPFWages = 0;
            decimal TotalPFWagesE = 0;
            decimal PFOnDuties = 0;
            decimal PFOnOT = 0;
            decimal PFOnNHs = 0;
            decimal TotalPFEmpr = 0;
            decimal TotalEsiWages = 0;
            decimal TotalESIWagesE = 0;
            decimal ESIOnDuties = 0;
            decimal ESIOnOTs = 0;
            decimal ESIOnNhs = 0;
            decimal TotalEsiEmpr = 0;
            decimal totalesiV = 0;
            decimal ProfTaxRangeFrom = 0;
            decimal ProfTaxRangeTill = 0;
            decimal ProfTaxAmount = 0;
            decimal ProfTax = 0;

            for (int k = 0; k < GVListEmployeess.Rows.Count; k++)
            {
                Label lblnodays = GVListEmployeess.Rows[k].FindControl("lblnodays") as Label;

                CheckBox chkindividual = GVListEmployeess.Rows[k].FindControl("chkindividual") as CheckBox;


                Label lblots = GVListEmployeess.Rows[k].FindControl("lblots") as Label;
                if (lblots.Text == "")
                {
                    lblots.Text = "0";
                }

                Label lblnhs = GVListEmployeess.Rows[k].FindControl("lblnhs") as Label;
                if (lblnhs.Text == "")
                {
                    lblnhs.Text = "0";
                }

                Label lblsrate = GVListEmployeess.Rows[k].FindControl("lblsrate") as Label;


                Label lblbasic = GVListEmployeess.Rows[k].FindControl("lblbasic") as Label;
                TextBox txtnewbasics = GVListEmployeess.Rows[k].FindControl("txtnewbasics") as TextBox;

                if (chkindividual.Checked == false)
                {
                    //basic
                    txtnewbasics.Text = lblbasic.Text;
                }
                else
                {
                    txtnewbasics.Text = txtnewbasics.Text;

                }

                TextBox txtArrearBasic = GVListEmployeess.Rows[k].FindControl("txtArrearBasic") as TextBox;
                if (txtnewbasics.Text != "" && lblbasic.Text != "")
                {
                    txtArrearBasic.Text = Math.Round(Convert.ToDecimal(txtnewbasics.Text) - Convert.ToDecimal(lblbasic.Text)).ToString();

                }
                else
                {
                    txtArrearBasic.Text = "0";
                }

                if (txtnewbasics.Text == "")
                {
                    txtnewbasics.Text = "0";
                }

                //DA
                Label lblDAs = GVListEmployeess.Rows[k].FindControl("lblDAs") as Label;
                TextBox txtnewdas = GVListEmployeess.Rows[k].FindControl("txtnewdas") as TextBox;
                if (chkindividual.Checked == false)
                {
                    //basic
                    txtnewdas.Text = lblDAs.Text;
                }
                else
                {
                    txtnewdas.Text = txtnewdas.Text;
                }

                TextBox txtArreardas = GVListEmployeess.Rows[k].FindControl("txtArreardas") as TextBox;
                if (txtnewdas.Text != "" && lblDAs.Text != "")
                {
                    //txtArreardas.Text = Math.Round(((Convert.ToDecimal(txtnewdas.Text) - Convert.ToDecimal(lblDAs.Text)) / (noofdays)) * (Convert.ToDecimal(lblnodays.Text))).ToString();
                    txtArreardas.Text = Math.Round(Convert.ToDecimal(txtnewdas.Text) - Convert.ToDecimal(lblDAs.Text)).ToString();

                }
                else
                {
                    txtArreardas.Text = "0";
                }

                if (txtnewdas.Text == "")
                {
                    txtnewdas.Text = "0";
                }

                //HRA
                Label lblhra = GVListEmployeess.Rows[k].FindControl("lblhra") as Label;
                TextBox txtNewHRA = GVListEmployeess.Rows[k].FindControl("txtNewHRA") as TextBox;
                if (chkindividual.Checked == false)
                {
                    //basic
                    txtNewHRA.Text = lblhra.Text;
                }
                else
                {
                    txtNewHRA.Text = txtNewHRA.Text;
                }


                TextBox txtArrearHRA = GVListEmployeess.Rows[k].FindControl("txtArrearHRA") as TextBox;
                if (txtNewHRA.Text != "" && lblhra.Text != "")
                {
                    txtArrearHRA.Text = Math.Round(Convert.ToDecimal(txtNewHRA.Text) - Convert.ToDecimal(lblhra.Text)).ToString();
                }
                else
                {
                    txtArrearHRA.Text = "0";
                }

                if (txtNewHRA.Text == "")
                {
                    txtNewHRA.Text = "0";
                }

                //CCA
                Label lblcca = GVListEmployeess.Rows[k].FindControl("lblcca") as Label;
                TextBox txtnewCCAs = GVListEmployeess.Rows[k].FindControl("txtnewCCA") as TextBox;
                if (chkindividual.Checked == false)
                {
                    //basic
                    txtnewCCAs.Text = lblcca.Text;
                }
                else
                {
                    txtnewCCAs.Text = txtnewCCAs.Text;
                }


                TextBox txtArrearCCA = GVListEmployeess.Rows[k].FindControl("txtArrearCCA") as TextBox;
                if (txtnewCCAs.Text != "" && lblcca.Text != "")
                {
                    txtArrearCCA.Text = Math.Round(Convert.ToDecimal(txtnewCCAs.Text) - Convert.ToDecimal(lblcca.Text)).ToString();

                }
                else
                {
                    txtArrearCCA.Text = "0";
                }

                if (txtnewCCAs.Text == "")
                {
                    txtnewCCAs.Text = "0";
                }

                //conv
                Label lblConv = GVListEmployeess.Rows[k].FindControl("lblConv") as Label;
                TextBox txtNewConv = GVListEmployeess.Rows[k].FindControl("txtNewConv") as TextBox;
                if (chkindividual.Checked == false)
                {
                    //basic
                    txtNewConv.Text = lblConv.Text;
                }
                else
                {
                    txtNewConv.Text = txtNewConv.Text;
                }


                TextBox txtArrearConv = GVListEmployeess.Rows[k].FindControl("txtArrearConv") as TextBox;
                if (txtNewConv.Text != "" && lblConv.Text != "")
                {
                    txtArrearConv.Text = Math.Round(Convert.ToDecimal(txtNewConv.Text) - Convert.ToDecimal(lblConv.Text)).ToString();


                }
                else
                {
                    txtArrearConv.Text = "0";
                }

                if (txtNewConv.Text == "")
                {
                    txtNewConv.Text = "0";
                }

                //wa
                Label lblWA = GVListEmployeess.Rows[k].FindControl("lblWA") as Label;
                TextBox txtNewWA = GVListEmployeess.Rows[k].FindControl("txtNewWA") as TextBox;
                if (chkindividual.Checked == false)
                {

                    txtNewWA.Text = lblWA.Text;
                }
                else
                {
                    txtNewWA.Text = txtNewWA.Text;
                }


                TextBox txtArrearWA = GVListEmployeess.Rows[k].FindControl("txtArrearWA") as TextBox;
                if (txtNewWA.Text != "" && lblWA.Text != "")
                {
                    txtArrearWA.Text = Math.Round(Convert.ToDecimal(txtNewWA.Text) - Convert.ToDecimal(lblWA.Text)).ToString();

                }
                else
                {
                    txtArrearWA.Text = "0";
                }

                if (txtNewWA.Text == "")
                {
                    txtNewWA.Text = "0";
                }

                //Incentivs
                Label lblIncentivs = GVListEmployeess.Rows[k].FindControl("lblIncentivs") as Label;
                TextBox txtNewIncentivs = GVListEmployeess.Rows[k].FindControl("txtNewIncentivs") as TextBox;
                if (chkindividual.Checked == false)
                {

                    txtNewIncentivs.Text = lblIncentivs.Text;
                }
                else
                {
                    txtNewIncentivs.Text = txtNewIncentivs.Text;
                }


                TextBox txtArrearIncentivs = GVListEmployeess.Rows[k].FindControl("txtArrearIncentivs") as TextBox;
                if (txtNewIncentivs.Text != "" && lblIncentivs.Text != "")
                {
                    txtArrearIncentivs.Text = Math.Round(Convert.ToDecimal(txtNewIncentivs.Text) - Convert.ToDecimal(lblIncentivs.Text)).ToString();

                }
                else
                {
                    txtArrearIncentivs.Text = "0";
                }

                if (txtNewIncentivs.Text == "")
                {
                    txtNewIncentivs.Text = "0";
                }

                //OA
                Label lblOA = GVListEmployeess.Rows[k].FindControl("lblOA") as Label;
                TextBox txtNewOA = GVListEmployeess.Rows[k].FindControl("txtNewOA") as TextBox;
                if (chkindividual.Checked == false)
                {
                    txtNewOA.Text = lblOA.Text;
                }
                else
                {
                    txtNewOA.Text = txtNewOA.Text;
                }


                TextBox txtArrearOA = GVListEmployeess.Rows[k].FindControl("txtArrearOA") as TextBox;
                if (txtNewOA.Text != "" && lblOA.Text != "")
                {
                    txtArrearOA.Text = Math.Round(Convert.ToDecimal(txtNewOA.Text) - Convert.ToDecimal(lblOA.Text)).ToString();

                }
                else
                {
                    txtArrearOA.Text = "0";
                }


                if (txtNewOA.Text == "")
                {
                    txtNewOA.Text = "0";
                }

                //LA
                Label lblLeaveEncashAmt = GVListEmployeess.Rows[k].FindControl("lblLeaveEncashAmt") as Label;
                TextBox txtNewLeaveEncashAmt = GVListEmployeess.Rows[k].FindControl("txtNewLeaveEncashAmt") as TextBox;
                if (chkindividual.Checked == false)
                {
                    txtNewLeaveEncashAmt.Text = lblLeaveEncashAmt.Text;
                }
                else
                {
                    txtNewLeaveEncashAmt.Text = txtNewLeaveEncashAmt.Text;
                }


                TextBox txtArrearLeaveEncashAmt = GVListEmployeess.Rows[k].FindControl("txtArrearLeaveEncashAmt") as TextBox;
                if (txtNewLeaveEncashAmt.Text != "" && lblLeaveEncashAmt.Text != "")
                {

                    txtArrearLeaveEncashAmt.Text = Math.Round(Convert.ToDecimal(txtNewLeaveEncashAmt.Text) - Convert.ToDecimal(lblLeaveEncashAmt.Text)).ToString();
                }
                else
                {
                    txtArrearLeaveEncashAmt.Text = "0";
                }

                if (txtNewLeaveEncashAmt.Text == "")
                {
                    txtNewLeaveEncashAmt.Text = "0";
                }

                //Gratuity
                Label lblGratuity = GVListEmployeess.Rows[k].FindControl("lblGratuity") as Label;
                TextBox txtNewGratuity = GVListEmployeess.Rows[k].FindControl("txtNewGratuity") as TextBox;
                if (chkindividual.Checked == false)
                {
                    txtNewGratuity.Text = lblGratuity.Text;
                }
                else
                {
                    txtNewGratuity.Text = txtNewGratuity.Text;
                }




                TextBox txtArrearGratuity = GVListEmployeess.Rows[k].FindControl("txtArrearGratuity") as TextBox;
                if (txtNewGratuity.Text != "" && lblGratuity.Text != "")
                {
                    //txtArrearGratuity.Text = Math.Round(((Convert.ToDecimal(txtNewGratuity.Text) - Convert.ToDecimal(lblGratuity.Text)) / (noofdays)) * (Convert.ToDecimal(lblnodays.Text))).ToString();
                    txtArrearGratuity.Text = Math.Round(Convert.ToDecimal(txtNewGratuity.Text) - Convert.ToDecimal(lblGratuity.Text)).ToString();
                }
                else
                {
                    txtArrearGratuity.Text = "0";
                }

                if (txtNewGratuity.Text == "")
                {
                    txtNewGratuity.Text = "0";
                }

                //NFHS
                Label lblNfhs = GVListEmployeess.Rows[k].FindControl("lblNfhs") as Label;
                TextBox txtNewNfhs = GVListEmployeess.Rows[k].FindControl("txtNewNfhs") as TextBox;
                if (chkindividual.Checked == false)
                {
                    txtNewNfhs.Text = lblNfhs.Text;
                }
                else
                {
                    txtNewNfhs.Text = txtNewNfhs.Text;
                }



                TextBox txtArrearNfhs = GVListEmployeess.Rows[k].FindControl("txtArrearNfhs") as TextBox;
                if (txtNewNfhs.Text != "" && lblNfhs.Text != "")
                {
                    // txtArrearNfhs.Text = Math.Round(((Convert.ToDecimal(txtNewNfhs.Text) - Convert.ToDecimal(lblNfhs.Text)) / (noofdays)) * (Convert.ToDecimal(lblnodays.Text))).ToString();

                    txtArrearNfhs.Text = Math.Round(Convert.ToDecimal(txtNewNfhs.Text) - Convert.ToDecimal(lblNfhs.Text)).ToString();
                }
                else
                {
                    txtArrearNfhs.Text = "0";
                }

                if (txtNewNfhs.Text == "")
                {
                    txtNewNfhs.Text = "0";
                }

                //Bonus
                Label lblBonus = GVListEmployeess.Rows[k].FindControl("lblBonus") as Label;
                TextBox txtNewBonus = GVListEmployeess.Rows[k].FindControl("txtNewBonus") as TextBox;
                if (chkindividual.Checked == false)
                {
                    txtNewBonus.Text = lblBonus.Text;
                }
                else
                {
                    txtNewBonus.Text = txtNewBonus.Text;
                }

                TextBox txtArrearBonus = GVListEmployeess.Rows[k].FindControl("txtArrearBonus") as TextBox;
                if (txtNewBonus.Text != "" && lblBonus.Text != "")
                {

                    //txtArrearBonus.Text = Math.Round(((Convert.ToDecimal(txtNewBonus.Text) - Convert.ToDecimal(lblBonus.Text)) / (noofdays)) * (Convert.ToDecimal(lblnodays.Text))).ToString();
                    txtArrearBonus.Text = Math.Round(Convert.ToDecimal(txtNewBonus.Text) - Convert.ToDecimal(lblBonus.Text)).ToString();
                }
                else
                {
                    txtArrearBonus.Text = "0";
                }

                if (txtNewBonus.Text == "")
                {
                    txtNewBonus.Text = "0";
                }

                //att Bonus
                Label lblAttBonus = GVListEmployeess.Rows[k].FindControl("lblAttBonus") as Label;
                TextBox txtNewAttBonus = GVListEmployeess.Rows[k].FindControl("txtNewAttBonus") as TextBox;
                if (chkindividual.Checked == false)
                {
                    txtNewAttBonus.Text = lblAttBonus.Text;
                }
                else
                {
                    txtNewAttBonus.Text = txtNewAttBonus.Text;
                }

                TextBox txtArrearattBonus = GVListEmployeess.Rows[k].FindControl("txtArrearattBonus") as TextBox;
                if (txtNewAttBonus.Text != "" && lblAttBonus.Text != "")
                {

                    //txtArrearBonus.Text = Math.Round(((Convert.ToDecimal(txtNewBonus.Text) - Convert.ToDecimal(lblBonus.Text)) / (noofdays)) * (Convert.ToDecimal(lblnodays.Text))).ToString();
                    txtArrearattBonus.Text = Math.Round(Convert.ToDecimal(txtNewAttBonus.Text) - Convert.ToDecimal(lblAttBonus.Text)).ToString();
                }
                else
                {
                    txtArrearattBonus.Text = "0";
                }

                if (txtNewAttBonus.Text == "")
                {
                    txtNewAttBonus.Text = "0";
                }

                //SplAllw
                Label lblSplAllw = GVListEmployeess.Rows[k].FindControl("lblSplAllw") as Label;
                TextBox txtNewSplAllw = GVListEmployeess.Rows[k].FindControl("txtNewSplAllw") as TextBox;
                if (chkindividual.Checked == false)
                {
                    txtNewSplAllw.Text = lblSplAllw.Text;
                }
                else
                {
                    txtNewSplAllw.Text = txtNewSplAllw.Text;
                }

                TextBox txtArrearSplAllw = GVListEmployeess.Rows[k].FindControl("txtArrearSplAllw") as TextBox;
                if (txtNewSplAllw.Text != "" && lblSplAllw.Text != "")
                {

                    //txtArrearSplAllw.Text = Math.Round(((Convert.ToDecimal(txtNewSplAllw.Text) - Convert.ToDecimal(lblSplAllw.Text)) / (noofdays)) * (Convert.ToDecimal(lblnodays.Text))).ToString();
                    txtArrearSplAllw.Text = Math.Round(Convert.ToDecimal(txtNewSplAllw.Text) - Convert.ToDecimal(lblSplAllw.Text)).ToString();

                }
                else
                {
                    txtArrearSplAllw.Text = "0";
                }

                if (txtNewSplAllw.Text == "")
                {
                    txtNewSplAllw.Text = "0";
                }

                //ProfAllw
                Label lblProfAllw = GVListEmployeess.Rows[k].FindControl("lblProfAllw") as Label;
                TextBox txtNewProfAllw = GVListEmployeess.Rows[k].FindControl("txtNewProfAllw") as TextBox;
                if (chkindividual.Checked == false)
                {
                    txtNewProfAllw.Text = lblProfAllw.Text;
                }
                else
                {
                    txtNewProfAllw.Text = txtNewProfAllw.Text;
                }

                TextBox txtArrearProfAllw = GVListEmployeess.Rows[k].FindControl("txtArrearProfAllw") as TextBox;
                if (txtNewProfAllw.Text != "" && lblProfAllw.Text != "")
                {

                    //txtArrearMobileAllw.Text = Math.Round(((Convert.ToDecimal(txtNewMobileAllw.Text) - Convert.ToDecimal(lblMobileAllw.Text)) / (noofdays)) * (Convert.ToDecimal(lblnodays.Text))).ToString();
                    txtArrearProfAllw.Text = Math.Round(Convert.ToDecimal(txtNewProfAllw.Text) - Convert.ToDecimal(lblProfAllw.Text)).ToString();

                }
                else
                {
                    txtArrearProfAllw.Text = "0";
                }

                if (txtNewProfAllw.Text == "")
                {
                    txtNewProfAllw.Text = "0";
                }

                // UniformAllw
                //Label lblUniformAllw = GVListEmployeess.Rows[k].FindControl("lblUniformAllw") as Label;
                //TextBox txtNewUniformAllw = GVListEmployeess.Rows[k].FindControl("txtNewUniformAllw") as TextBox;
                //if (chkindividual.Checked == false)
                //{
                //    txtNewUniformAllw.Text = lblUniformAllw.Text;
                //}
                //else
                //{
                //    txtNewUniformAllw.Text = txtNewUniformAllw.Text;
                //}

                //TextBox txtArrearUniformAllw = GVListEmployeess.Rows[k].FindControl("txtArrearUniformAllw") as TextBox;
                //if (txtNewUniformAllw.Text != "" && lblUniformAllw.Text != "")
                //{

                //    //txtArrearMobileAllw.Text = Math.Round(((Convert.ToDecimal(txtNewMobileAllw.Text) - Convert.ToDecimal(lblMobileAllw.Text)) / (noofdays)) * (Convert.ToDecimal(lblnodays.Text))).ToString();
                //    txtArrearUniformAllw.Text = Math.Round(Convert.ToDecimal(txtNewUniformAllw.Text) - Convert.ToDecimal(lblUniformAllw.Text)).ToString();

                //}
                //else
                //{
                //    txtArrearUniformAllw.Text = "0";
                //}

                //if (txtNewUniformAllw.Text == "")
                //{
                //    txtNewUniformAllw.Text = "0";
                //}


                //rc
                Label lblRC = GVListEmployeess.Rows[k].FindControl("lblRC") as Label;
                TextBox txtNewRC = GVListEmployeess.Rows[k].FindControl("txtNewRC") as TextBox;
                if (chkindividual.Checked == false)
                {
                    txtNewRC.Text = lblRC.Text;
                }
                else
                {
                    txtNewRC.Text = txtNewRC.Text;
                }

                TextBox txtArrearRC = GVListEmployeess.Rows[k].FindControl("txtArrearRC") as TextBox;
                if (txtNewRC.Text != "" && lblRC.Text != "")
                {

                    //txtArrearConvAllw.Text = Math.Round(((Convert.ToDecimal(txtNewConvAllw.Text) - Convert.ToDecimal(lblConvAllw.Text)) / (noofdays)) * (Convert.ToDecimal(lblnodays.Text))).ToString();
                    txtArrearRC.Text = Math.Round(Convert.ToDecimal(txtNewRC.Text) - Convert.ToDecimal(lblRC.Text)).ToString();

                }
                else
                {
                    txtArrearRC.Text = "0";
                }

                if (txtNewRC.Text == "")
                {
                    txtNewRC.Text = "0";
                }

                //cs
                Label lblCS = GVListEmployeess.Rows[k].FindControl("lblCS") as Label;
                TextBox txtNewCS = GVListEmployeess.Rows[k].FindControl("txtNewCS") as TextBox;
                if (chkindividual.Checked == false)
                {
                    txtNewCS.Text = lblCS.Text;
                }
                else
                {
                    txtNewCS.Text = txtNewCS.Text;
                }

                TextBox txtArrearCS = GVListEmployeess.Rows[k].FindControl("txtArrearCS") as TextBox;
                if (txtNewCS.Text != "" && lblCS.Text != "")
                {

                    //txtArrearConvAllw.Text = Math.Round(((Convert.ToDecimal(txtNewConvAllw.Text) - Convert.ToDecimal(lblConvAllw.Text)) / (noofdays)) * (Convert.ToDecimal(lblnodays.Text))).ToString();
                    txtArrearCS.Text = Math.Round(Convert.ToDecimal(txtNewCS.Text) - Convert.ToDecimal(lblCS.Text)).ToString();

                }
                else
                {
                    txtArrearCS.Text = "0";
                }

                if (txtNewCS.Text == "")
                {
                    txtNewCS.Text = "0";
                }

                //Food Allw
                Label lblFoodAllw = GVListEmployeess.Rows[k].FindControl("lblFoodAllw") as Label;
                TextBox txtNewFoodAllw = GVListEmployeess.Rows[k].FindControl("txtNewFoodAllw") as TextBox;
                if (chkindividual.Checked == false)
                {
                    txtNewFoodAllw.Text = lblFoodAllw.Text;
                }
                else
                {
                    txtNewFoodAllw.Text = txtNewFoodAllw.Text;
                }

                TextBox txtArrearFoodAllw = GVListEmployeess.Rows[k].FindControl("txtArrearFoodAllw") as TextBox;
                if (txtNewFoodAllw.Text != "" && lblFoodAllw.Text != "")
                {

                    //txtArrearConvAllw.Text = Math.Round(((Convert.ToDecimal(txtNewConvAllw.Text) - Convert.ToDecimal(lblConvAllw.Text)) / (noofdays)) * (Convert.ToDecimal(lblnodays.Text))).ToString();
                    txtArrearFoodAllw.Text = Math.Round(Convert.ToDecimal(txtNewFoodAllw.Text) - Convert.ToDecimal(lblFoodAllw.Text)).ToString();

                }
                else
                {
                    txtArrearFoodAllw.Text = "0";
                }

                if (txtNewFoodAllw.Text == "")
                {
                    txtNewFoodAllw.Text = "0";
                }

                //Medical Allw
                Label lblMedicalAllw = GVListEmployeess.Rows[k].FindControl("lblMedicalAllw") as Label;
                TextBox txtNewMedicalAllw = GVListEmployeess.Rows[k].FindControl("txtNewMedicalAllw") as TextBox;
                if (chkindividual.Checked == false)
                {
                    txtNewMedicalAllw.Text = lblMedicalAllw.Text;
                }
                else
                {
                    txtNewMedicalAllw.Text = txtNewMedicalAllw.Text;
                }

                TextBox txtArrearMedicalAllw = GVListEmployeess.Rows[k].FindControl("txtArrearMedicalAllw") as TextBox;
                if (txtNewMedicalAllw.Text != "" && lblMedicalAllw.Text != "")
                {
                    txtArrearMedicalAllw.Text = Math.Round(Convert.ToDecimal(txtNewMedicalAllw.Text) - Convert.ToDecimal(lblMedicalAllw.Text)).ToString();

                }
                else
                {
                    txtArrearMedicalAllw.Text = "0";
                }

                if (txtNewMedicalAllw.Text == "")
                {
                    txtNewMedicalAllw.Text = "0";
                }

                //Travel Allw
                Label lblTravelAllw = GVListEmployeess.Rows[k].FindControl("lblTravelAllw") as Label;
                TextBox txtNewTravelAllw = GVListEmployeess.Rows[k].FindControl("txtNewTravelAllw") as TextBox;
                if (chkindividual.Checked == false)
                {
                    txtNewTravelAllw.Text = lblTravelAllw.Text;
                }
                else
                {
                    txtNewTravelAllw.Text = txtNewTravelAllw.Text;
                }

                TextBox txtArrearTravelAllw = GVListEmployeess.Rows[k].FindControl("txtArrearTravelAllw") as TextBox;
                if (txtNewTravelAllw.Text != "" && lblTravelAllw.Text != "")
                {
                    txtArrearTravelAllw.Text = Math.Round(Convert.ToDecimal(txtNewTravelAllw.Text) - Convert.ToDecimal(lblTravelAllw.Text)).ToString();

                }
                else
                {
                    txtArrearTravelAllw.Text = "0";
                }

                if (txtNewTravelAllw.Text == "")
                {
                    txtNewTravelAllw.Text = "0";
                }


                //Mobile Allw
                Label lblMobileAllw = GVListEmployeess.Rows[k].FindControl("lblMobileAllw") as Label;
                TextBox txtNewMobileAllw = GVListEmployeess.Rows[k].FindControl("txtNewMobileAllw") as TextBox;
                if (chkindividual.Checked == false)
                {
                    txtNewMobileAllw.Text = lblMobileAllw.Text;
                }
                else
                {
                    txtNewMobileAllw.Text = txtNewMobileAllw.Text;
                }

                TextBox txtArrearMobileAllw = GVListEmployeess.Rows[k].FindControl("txtArrearMobileAllw") as TextBox;
                if (txtNewMobileAllw.Text != "" && lblMobileAllw.Text != "")
                {
                    txtArrearMobileAllw.Text = Math.Round(Convert.ToDecimal(txtNewMobileAllw.Text) - Convert.ToDecimal(lblMobileAllw.Text)).ToString();

                }
                else
                {
                    txtArrearMobileAllw.Text = "0";
                }

                if (txtNewMobileAllw.Text == "")
                {
                    txtNewMobileAllw.Text = "0";
                }

                //Night Shift Allw
                Label lblNightShiftAllw = GVListEmployeess.Rows[k].FindControl("lblNightShiftAllw") as Label;
                TextBox txtNewNightShiftAllw = GVListEmployeess.Rows[k].FindControl("txtNewNightShiftAllw") as TextBox;
                if (chkindividual.Checked == false)
                {
                    txtNewNightShiftAllw.Text = lblNightShiftAllw.Text;
                }
                else
                {
                    txtNewNightShiftAllw.Text = txtNewMobileAllw.Text;
                }

                TextBox txtArrearNightShiftAllw = GVListEmployeess.Rows[k].FindControl("txtArrearNightShiftAllw") as TextBox;
                if (txtNewNightShiftAllw.Text != "" && lblNightShiftAllw.Text != "")
                {
                    txtArrearNightShiftAllw.Text = Math.Round(Convert.ToDecimal(txtNewNightShiftAllw.Text) - Convert.ToDecimal(lblNightShiftAllw.Text)).ToString();

                }
                else
                {
                    txtArrearNightShiftAllw.Text = "0";
                }

                if (txtNewNightShiftAllw.Text == "")
                {
                    txtNewNightShiftAllw.Text = "0";
                }

                //Service Weightage
                Label lblServiceWeightage = GVListEmployeess.Rows[k].FindControl("lblServiceWeightage") as Label;
                TextBox txtNewServiceWeightage = GVListEmployeess.Rows[k].FindControl("txtNewServiceWeightage") as TextBox;
                if (chkindividual.Checked == false)
                {
                    txtNewServiceWeightage.Text = lblServiceWeightage.Text;
                }
                else
                {
                    txtNewServiceWeightage.Text = txtNewServiceWeightage.Text;
                }

                TextBox txtArrearServiceWeightage = GVListEmployeess.Rows[k].FindControl("txtArrearServiceWeightage") as TextBox;
                if (txtNewServiceWeightage.Text != "" && lblServiceWeightage.Text != "")
                {
                    txtArrearServiceWeightage.Text = Math.Round(Convert.ToDecimal(txtNewServiceWeightage.Text) - Convert.ToDecimal(lblServiceWeightage.Text)).ToString();

                }
                else
                {
                    txtArrearServiceWeightage.Text = "0";
                }

                if (txtNewServiceWeightage.Text == "")
                {
                    txtNewServiceWeightage.Text = "0";
                }

                //PL Amount
                Label lblPLAmount = GVListEmployeess.Rows[k].FindControl("lblPLAmount") as Label;
                TextBox txtNewPLAmount = GVListEmployeess.Rows[k].FindControl("txtNewPLAmount") as TextBox;
                if (chkindividual.Checked == false)
                {
                    txtNewPLAmount.Text = lblPLAmount.Text;
                }
                else
                {
                    txtNewPLAmount.Text = txtNewPLAmount.Text;
                }

                TextBox txtArrearPLAmount = GVListEmployeess.Rows[k].FindControl("txtArrearPLAmount") as TextBox;
                if (txtNewPLAmount.Text != "" && lblPLAmount.Text != "")
                {
                    txtArrearPLAmount.Text = Math.Round(Convert.ToDecimal(txtNewPLAmount.Text) - Convert.ToDecimal(lblPLAmount.Text)).ToString();

                }
                else
                {
                    txtArrearPLAmount.Text = "0";
                }

                if (txtNewPLAmount.Text == "")
                {
                    txtNewPLAmount.Text = "0";
                }

                //Arrears
                Label lblArrears = GVListEmployeess.Rows[k].FindControl("lblArrears") as Label;
                TextBox txtNewArrears = GVListEmployeess.Rows[k].FindControl("txtNewArrears") as TextBox;
                if (chkindividual.Checked == false)
                {
                    txtNewArrears.Text = lblArrears.Text;
                }
                else
                {
                    txtNewArrears.Text = txtNewArrears.Text;
                }

                TextBox txtArrearArrears = GVListEmployeess.Rows[k].FindControl("txtArrearArrears") as TextBox;
                if (txtNewArrears.Text != "" && lblPLAmount.Text != "")
                {
                    txtArrearArrears.Text = Math.Round(Convert.ToDecimal(txtNewArrears.Text) - Convert.ToDecimal(lblArrears.Text)).ToString();

                }
                else
                {
                    txtArrearArrears.Text = "0";
                }

                if (txtNewArrears.Text == "")
                {
                    txtNewArrears.Text = "0";
                }

                //Npots amt
                Label lblNpotsAmt = GVListEmployeess.Rows[k].FindControl("lblNpotsAmt") as Label;
                TextBox txtNewNpotsAmt = GVListEmployeess.Rows[k].FindControl("txtNewNpotsAmt") as TextBox;
                if (chkindividual.Checked == false)
                {
                    txtNewNpotsAmt.Text = lblNpotsAmt.Text;
                }
                else
                {
                    txtNewNpotsAmt.Text = txtNewNpotsAmt.Text;
                }

                Label lblArrearNpotsAmt = GVListEmployeess.Rows[k].FindControl("lblArrearNpotsAmt") as Label;
                if (txtNewNpotsAmt.Text != "" && lblPLAmount.Text != "")
                {
                    lblArrearNpotsAmt.Text = Math.Round(Convert.ToDecimal(txtNewNpotsAmt.Text) - Convert.ToDecimal(lblNpotsAmt.Text)).ToString();

                }
                else
                {
                    lblArrearNpotsAmt.Text = "0";
                }

                if (txtNewNpotsAmt.Text == "")
                {
                    txtNewNpotsAmt.Text = "0";
                }


                //OTHrs Amount
                Label lblOTHrsAmt = GVListEmployeess.Rows[k].FindControl("lblOTHrsAmt") as Label;
                TextBox txtNewOThrsAmt = GVListEmployeess.Rows[k].FindControl("txtNewOThrsAmt") as TextBox;
                if (chkindividual.Checked == false)
                {
                    txtNewOThrsAmt.Text = lblOTHrsAmt.Text;
                }
                else
                {
                    txtNewOThrsAmt.Text = txtNewOThrsAmt.Text;
                }

                TextBox txtArrearOTHrsAmt = GVListEmployeess.Rows[k].FindControl("txtArrearOTHrsAmt") as TextBox;
                if (txtNewOThrsAmt.Text != "" && lblOTHrsAmt.Text != "")
                {
                    txtArrearOTHrsAmt.Text = Math.Round(Convert.ToDecimal(txtNewOThrsAmt.Text) - Convert.ToDecimal(lblOTHrsAmt.Text)).ToString();

                }
                else
                {
                    txtArrearOTHrsAmt.Text = "0";
                }

                if (txtNewOThrsAmt.Text == "")
                {
                    txtNewOThrsAmt.Text = "0";
                }

                Label lblOTAmt = GVListEmployeess.Rows[k].FindControl("lblOTAmt") as Label;
                TextBox txtNewOTAmt = GVListEmployeess.Rows[k].FindControl("txtNewOTAmt") as TextBox;
                if (chkindividual.Checked == false)
                {
                    txtNewOTAmt.Text = lblOTAmt.Text;
                }
                else
                {
                    txtNewOTAmt.Text = txtNewOTAmt.Text;
                }

                Label lblArrearOTAmt = GVListEmployeess.Rows[k].FindControl("lblArrearOTAmt") as Label;
                if (txtNewOTAmt.Text != "" && lblOTAmt.Text != "")
                {

                    //txtArrearMobileAllw.Text = Math.Round(((Convert.ToDecimal(txtNewMobileAllw.Text) - Convert.ToDecimal(lblMobileAllw.Text)) / (noofdays)) * (Convert.ToDecimal(lblnodays.Text))).ToString();
                    lblArrearOTAmt.Text = Math.Round(Convert.ToDecimal(txtNewOTAmt.Text) - Convert.ToDecimal(lblOTAmt.Text)).ToString();

                }
                else
                {
                    lblArrearOTAmt.Text = "0";
                }

                if (txtNewOTAmt.Text == "")
                {
                    txtNewOTAmt.Text = "0";
                }

                Label lblWOAmount = GVListEmployeess.Rows[k].FindControl("lblWOAmount") as Label;
                TextBox txtNewWOAmount = GVListEmployeess.Rows[k].FindControl("txtNewWOAmount") as TextBox;
                if (chkindividual.Checked == false)
                {
                    txtNewWOAmount.Text = lblWOAmount.Text;
                }
                else
                {
                    txtNewWOAmount.Text = txtNewWOAmount.Text;
                }

                TextBox txtArrearWOAmount = GVListEmployeess.Rows[k].FindControl("txtArrearWOAmount") as TextBox;
                if (txtNewWOAmount.Text != "" && lblWOAmount.Text != "")
                {
                    txtArrearWOAmount.Text = Math.Round(Convert.ToDecimal(txtNewWOAmount.Text) - Convert.ToDecimal(lblWOAmount.Text)).ToString();

                }
                else
                {
                    txtArrearWOAmount.Text = "0";
                }

                if (txtNewWOAmount.Text == "")
                {
                    txtNewWOAmount.Text = "0";
                }

                Label lblNHsAmt = GVListEmployeess.Rows[k].FindControl("lblNHsAmt") as Label;
                TextBox txtNewNHsAmt = GVListEmployeess.Rows[k].FindControl("txtNewNHsAmt") as TextBox;
                if (chkindividual.Checked == false)
                {
                    txtNewNHsAmt.Text = lblNHsAmt.Text;
                }
                else
                {
                    txtNewNHsAmt.Text = txtNewNHsAmt.Text;
                }

                Label lblArrearNHsAmt = GVListEmployeess.Rows[k].FindControl("lblArrearNHsAmt") as Label;
                if (txtNewNHsAmt.Text != "" && lblNHsAmt.Text != "")
                {

                    //txtArrearMobileAllw.Text = Math.Round(((Convert.ToDecimal(txtNewMobileAllw.Text) - Convert.ToDecimal(lblMobileAllw.Text)) / (noofdays)) * (Convert.ToDecimal(lblnodays.Text))).ToString();
                    lblArrearNHsAmt.Text = Math.Round(Convert.ToDecimal(txtNewNHsAmt.Text) - Convert.ToDecimal(lblNHsAmt.Text)).ToString();

                }
                else
                {
                    lblArrearNHsAmt.Text = "0";
                }

                if (txtNewNHsAmt.Text == "")
                {
                    txtNewNHsAmt.Text = "0";
                }

                TextBox txtPaysheetGross = GVListEmployeess.Rows[k].FindControl("txtPaysheetGross") as TextBox;


                TextBox txtArrearGross = GVListEmployeess.Rows[k].FindControl("txtArrearGross") as TextBox;
                txtArrearGross.Text = (Convert.ToDecimal(txtArrearBasic.Text) + Convert.ToDecimal(txtArreardas.Text) + Convert.ToDecimal(txtArrearHRA.Text) +
                Convert.ToDecimal(txtArrearCCA.Text) + Convert.ToDecimal(txtArrearConv.Text) + Convert.ToDecimal(txtArrearGratuity.Text) +
                Convert.ToDecimal(txtArrearBonus.Text) + Convert.ToDecimal(txtArrearWA.Text) + Convert.ToDecimal(txtArrearLeaveEncashAmt.Text) +
                Convert.ToDecimal(txtArrearOA.Text) + Convert.ToDecimal(txtArrearNfhs.Text) + Convert.ToDecimal(txtArrearSplAllw.Text) +
                Convert.ToDecimal(txtArrearProfAllw.Text) + Convert.ToDecimal(txtArrearRC.Text) +
                Convert.ToDecimal(txtArrearCS.Text) + Convert.ToDecimal(txtArrearFoodAllw.Text) + Convert.ToDecimal(txtArrearMedicalAllw.Text) +
                Convert.ToDecimal(txtArrearTravelAllw.Text) + Convert.ToDecimal(txtArrearMobileAllw.Text) + Convert.ToDecimal(txtArrearOTHrsAmt.Text) +
                Convert.ToDecimal(lblArrearOTAmt.Text) + Convert.ToDecimal(lblArrearNHsAmt.Text) + Convert.ToDecimal(txtArrearWOAmount.Text) +
                Convert.ToDecimal(txtArrearServiceWeightage.Text) + Convert.ToDecimal(txtArrearNightShiftAllw.Text) + Convert.ToDecimal(txtArrearPLAmount.Text)
                + Convert.ToDecimal(txtArrearIncentivs.Text) + Convert.ToDecimal(txtArrearArrears.Text) + Convert.ToDecimal(txtArrearOTHrsAmt.Text) + Convert.ToDecimal(lblArrearNpotsAmt.Text)).ToString();


                if (txtArrearGross.Text == "")
                {
                    txtArrearGross.Text = "0";
                }

                Label lblPFDeduct = GVListEmployeess.Rows[k].FindControl("lblPFDeduct") as Label;

                TextBox txtNewpf = GVListEmployeess.Rows[k].FindControl("txtNewpf") as TextBox;
                Label lblnewPFEmpr = GVListEmployeess.Rows[k].FindControl("lblnewPFEmpr") as Label;
                Label lblnewPFWages = GVListEmployeess.Rows[k].FindControl("lblnewPFWages") as Label;

                TextBox txtpf = GVListEmployeess.Rows[k].FindControl("txtpf") as TextBox;
                Label lblPFEmpr = GVListEmployeess.Rows[k].FindControl("lblPFEmpr") as Label;
                Label lblPFWages = GVListEmployeess.Rows[k].FindControl("lblPFWages") as Label;


                Label lblesicheck = GVListEmployeess.Rows[k].FindControl("lblesicheck") as Label;
                Label lblpfcheck = GVListEmployeess.Rows[k].FindControl("lblpfcheck") as Label;

                TextBox txtArrearpf = GVListEmployeess.Rows[k].FindControl("txtArrearpf") as TextBox;
                Label lblArrearPFEmpr = GVListEmployeess.Rows[k].FindControl("lblArrearPFEmpr") as Label;
                Label lblArrearPFWages = GVListEmployeess.Rows[k].FindControl("lblArrearPFWages") as Label;
                Label lblPFPayRate = GVListEmployeess.Rows[k].FindControl("lblPFPayRate") as Label;

                if (lblPFDeduct.Text == "True" && chkindividual.Checked == true)
                {

                    decimal tempfwages = 0;
                    if (lblPFPayRate.Text == "")
                    {
                        lblPFPayRate.Text = "0";
                    }

                    if (lblPFPayRate.Text != "0")
                    {
                        tempfwages = Convert.ToDecimal(lblPFPayRate.Text);
                        txtNewpf.Text = Math.Round(tempfwages * (pf / 100) * (PFEmployee / 100), 0).ToString();
                        lblnewPFWages.Text = tempfwages.ToString();
                        lblnewPFEmpr.Text = Math.Round(tempfwages * (pf / 100) * (PFEmployer / 100), 0).ToString();
                    }
                    else
                    {

                        if (pffrom == 0)
                        {
                            tempfwages = (Convert.ToDecimal(txtnewbasics.Text) + Convert.ToDecimal(txtnewdas.Text));

                            if (pflimit > 0)
                            {
                                if (tempfwages > pflimit)
                                {
                                    tempfwages = pflimit;
                                }
                            }

                            txtNewpf.Text = Math.Round(tempfwages * (pf / 100) * (PFEmployee / 100), 0).ToString();
                            lblnewPFWages.Text = tempfwages.ToString();
                            lblnewPFEmpr.Text = Math.Round(tempfwages * (pf / 100) * (PFEmployer / 100), 0).ToString();


                        }
                        else if (pffrom == 1)
                        {
                            tempfwages = (Convert.ToDecimal(txtnewbasics.Text));

                            if (pflimit > 0)
                            {
                                if (tempfwages > pflimit)
                                {
                                    tempfwages = pflimit;
                                }
                            }

                            txtNewpf.Text = Math.Round(tempfwages * (pf / 100) * (PFEmployee / 100), 0).ToString();
                            lblnewPFWages.Text = tempfwages.ToString();
                            lblnewPFEmpr.Text = Math.Round(tempfwages * (pf / 100) * (PFEmployer / 100), 0).ToString();

                        }
                        else if (pffrom == 2)
                        {
                            tempfwages = (Convert.ToDecimal(txtnewbasics.Text) + Convert.ToDecimal(txtnewdas.Text) + Convert.ToDecimal(txtNewServiceWeightage.Text));

                            if (pflimit > 0)
                            {
                                if (tempfwages > pflimit)
                                {
                                    tempfwages = pflimit;
                                }
                            }

                            txtNewpf.Text = Math.Round(tempfwages * (pf / 100) * (PFEmployee / 100), 0).ToString();
                            lblnewPFWages.Text = tempfwages.ToString();
                            lblnewPFEmpr.Text = Math.Round(tempfwages * (pf / 100) * (PFEmployer / 100), 0).ToString();

                        }
                        else if (pffrom == 3)
                        {
                            tempfwages = (Convert.ToDecimal(txtnewbasics.Text) + Convert.ToDecimal(txtnewdas.Text) + Convert.ToDecimal(txtNewHRA.Text) + Convert.ToDecimal(txtNewWA.Text));

                            if (pflimit > 0)
                            {
                                if (tempfwages > pflimit)
                                {
                                    tempfwages = pflimit;
                                }
                            }

                            txtNewpf.Text = Math.Round(tempfwages * (pf / 100) * (PFEmployee / 100), 0).ToString();
                            lblnewPFWages.Text = tempfwages.ToString();
                            lblnewPFEmpr.Text = Math.Round(tempfwages * (pf / 100) * (PFEmployer / 100), 0).ToString();

                        }
                        else if (pffrom == 4)
                        {
                            tempfwages = Convert.ToDecimal(txtPaysheetGross.Text);

                            if (pflimit > 0)
                            {
                                if (tempfwages > pflimit)
                                {
                                    tempfwages = pflimit;
                                }
                            }

                            txtNewpf.Text = Math.Round(tempfwages * (pf / 100) * (PFEmployee / 100), 0).ToString();
                            lblnewPFWages.Text = tempfwages.ToString();
                            lblnewPFEmpr.Text = Math.Round(tempfwages * (pf / 100) * (PFEmployer / 100), 0).ToString();

                        }

                    }



                    txtArrearpf.Text = (Convert.ToDecimal(txtNewpf.Text) - Convert.ToDecimal(txtpf.Text)).ToString();
                    lblArrearPFEmpr.Text = (Convert.ToDecimal(lblnewPFEmpr.Text) - Convert.ToDecimal(lblPFEmpr.Text)).ToString();
                    lblArrearPFWages.Text = (Convert.ToDecimal(lblnewPFWages.Text) - Convert.ToDecimal(lblPFWages.Text)).ToString();

                    TotalPFWagesE = TotalPFWagesE + Math.Round(Convert.ToDecimal(txtArrearpf.Text) * 100 / PFEmployee, 0);
                    TotalPFWages = TotalPFWagesE;
                    PFOnDuties = Convert.ToDecimal(txtArrearpf.Text);
                    TotalPFEmpr = Math.Round(TotalPFWages * (pf / 100) * (PFEmployer / 100), 0);


                }
                else
                {
                    TotalPFWages = 0;
                    TotalPFWagesE = 0;
                    PFOnDuties = 0;
                    PFOnOT = 0;
                    PFOnNHs = 0;
                    TotalPFEmpr = 0;
                    txtNewpf.Text = "0";

                    txtArrearpf.Text = (Convert.ToDecimal(txtNewpf.Text) - Convert.ToDecimal(txtpf.Text)).ToString();
                    lblArrearPFEmpr.Text = (Convert.ToDecimal(lblnewPFEmpr.Text) - Convert.ToDecimal(lblPFEmpr.Text)).ToString();
                    lblArrearPFWages.Text = (Convert.ToDecimal(lblnewPFWages.Text) - Convert.ToDecimal(lblPFWages.Text)).ToString();


                }

                Label lblESIDeduct = GVListEmployeess.Rows[k].FindControl("lblESIDeduct") as Label;
                TextBox txtNewESI = GVListEmployeess.Rows[k].FindControl("txtNewESI") as TextBox;
                TextBox txtESI = GVListEmployeess.Rows[k].FindControl("txtESI") as TextBox;
                TextBox txtnewgross = GVListEmployeess.Rows[k].FindControl("txtnewgross") as TextBox;
                TextBox txtArrearesi = GVListEmployeess.Rows[k].FindControl("txtArrearesi") as TextBox;
                Label lblmonthdayss = GVListEmployeess.Rows[k].FindControl("lblmonthdayss") as Label;
                Label lblOTPercent = GVListEmployeess.Rows[k].FindControl("lblOTPercent") as Label;
                Label lblotrate = GVListEmployeess.Rows[k].FindControl("lblotrate") as Label;

                Label lblArrearESIEmpr = GVListEmployeess.Rows[k].FindControl("lblArrearESIEmpr") as Label;
                Label lblArrearESIWages = GVListEmployeess.Rows[k].FindControl("lblArrearESIWages") as Label;

                Label lblESIEmpr = GVListEmployeess.Rows[k].FindControl("lblESIEmpr") as Label;
                Label lblESIWages = GVListEmployeess.Rows[k].FindControl("lblESIWages") as Label;

                Label lblNewESIEmpr = GVListEmployeess.Rows[k].FindControl("lblNewESIEmpr") as Label;
                Label lblNewESIWages = GVListEmployeess.Rows[k].FindControl("lblNewESIWages") as Label;


                decimal newgross = (GetTextboxValue(txtnewbasics.Text) + GetTextboxValue(txtnewdas.Text) + GetTextboxValue(txtNewHRA.Text) +
                    GetTextboxValue(txtnewCCAs.Text) + GetTextboxValue(txtNewConv.Text) + GetTextboxValue(txtNewGratuity.Text) + GetTextboxValue(txtNewBonus.Text) +
                    GetTextboxValue(txtNewWA.Text) + GetTextboxValue(txtNewLeaveEncashAmt.Text) + GetTextboxValue(txtNewOA.Text) + GetTextboxValue(txtNewNfhs.Text) +
                    GetTextboxValue(txtNewSplAllw.Text) +
                    GetTextboxValue(txtNewServiceWeightage.Text) + GetTextboxValue(txtNewRC.Text) +
                    GetTextboxValue(txtNewCS.Text) + GetTextboxValue(txtNewOTAmt.Text) + GetTextboxValue(txtNewNHsAmt.Text) + GetTextboxValue(txtNewFoodAllw.Text) +
                    +GetTextboxValue(txtNewMedicalAllw.Text) + GetTextboxValue(txtNewTravelAllw.Text) + GetTextboxValue(txtNewNightShiftAllw.Text) +
                    GetTextboxValue(txtArrearOTHrsAmt.Text) + GetTextboxValue(txtNewWOAmount.Text));

                txtnewgross.Text = newgross.ToString();


                if (lblESIDeduct.Text == "True" && chkindividual.Checked == true)
                {
                    if (esifrom == 0)//gross-wa
                    {
                        if (lblotrate.Text == "0")
                        {
                            txtNewESI.Text = Math.Round((newgross - Convert.ToDecimal(txtNewWA.Text) - (Convert.ToDecimal(txtNewWA.Text) / Convert.ToDecimal(lblmonthdayss.Text) * (Convert.ToDecimal(lblots.Text) * (Convert.ToDecimal(lblOTPercent.Text) / 100)))) * (esi / 100) * (ESIEmployee / 100), 0).ToString();
                            lblNewESIEmpr.Text = Math.Round((newgross - Convert.ToDecimal(txtNewWA.Text) - (Convert.ToDecimal(txtNewWA.Text) / Convert.ToDecimal(lblmonthdayss.Text) * (Convert.ToDecimal(lblots.Text) * (Convert.ToDecimal(lblOTPercent.Text) / 100)))) * (esi / 100) * (ESIEmployer / 100), 0).ToString();
                        }
                        else
                        {
                            txtNewESI.Text = Math.Round((newgross - Convert.ToDecimal(txtNewWA.Text)) * (esi / 100) * (ESIEmployee / 100), 0).ToString();
                            lblNewESIEmpr.Text = Math.Round((newgross - Convert.ToDecimal(txtNewWA.Text)) * (esi / 100) * (ESIEmployer / 100), 0).ToString();

                        }


                        lblNewESIWages.Text = (newgross - Convert.ToDecimal(txtNewWA.Text)).ToString();


                    }
                    else if (esifrom == 1)//gross-la
                    {
                        txtNewESI.Text = Math.Round((newgross - Convert.ToDecimal(txtNewLeaveEncashAmt.Text)) * (esi / 100) * (ESIEmployee / 100), 0).ToString();
                        lblNewESIEmpr.Text = Math.Round((newgross - Convert.ToDecimal(txtNewLeaveEncashAmt.Text)) * (esi / 100) * (ESIEmployer / 100), 0).ToString();
                        lblNewESIWages.Text = (newgross - Convert.ToDecimal(txtNewLeaveEncashAmt.Text)).ToString();
                    }
                    else if (esifrom == 2)//basic
                    {
                        txtNewESI.Text = Math.Round(Convert.ToDecimal(txtnewbasics.Text) * (esi / 100) * (ESIEmployee / 100), 0).ToString();
                        lblNewESIEmpr.Text = Math.Round(Convert.ToDecimal(txtnewbasics.Text) * (esi / 100) * (ESIEmployer / 100), 0).ToString();
                        lblNewESIWages.Text = txtnewbasics.Text;
                    }

                    else if (esifrom == 3)//Gross+Inc
                    {
                        txtNewESI.Text = Math.Round((Convert.ToDecimal(txtNewIncentivs.Text) + newgross) * (esi / 100) * (ESIEmployee / 100), 0).ToString();
                        lblNewESIEmpr.Text = Math.Round((Convert.ToDecimal(txtNewIncentivs.Text) + newgross) * (esi / 100) * (ESIEmployer / 100), 0).ToString();
                        lblNewESIWages.Text = (Convert.ToDecimal(txtNewIncentivs.Text) + newgross).ToString();
                    }

                    else if (esifrom == 4)//gross+WA+Bonus+LA+Gratuity
                    {
                        txtNewESI.Text = Math.Round((newgross + Convert.ToDecimal(txtNewBonus.Text) + Convert.ToDecimal(txtNewWA.Text) + Convert.ToDecimal(txtNewLeaveEncashAmt.Text) + Convert.ToDecimal(txtNewGratuity.Text)) * (esi / 100) * (ESIEmployee / 100), 0).ToString();
                        lblNewESIEmpr.Text = Math.Round((newgross + Convert.ToDecimal(txtNewBonus.Text) + Convert.ToDecimal(txtNewWA.Text) + Convert.ToDecimal(txtNewLeaveEncashAmt.Text) + Convert.ToDecimal(txtNewGratuity.Text)) * (esi / 100) * (ESIEmployer / 100), 0).ToString();
                        lblNewESIWages.Text = (newgross + Convert.ToDecimal(txtNewBonus.Text) + Convert.ToDecimal(txtNewWA.Text) + Convert.ToDecimal(txtNewLeaveEncashAmt.Text) + Convert.ToDecimal(txtNewGratuity.Text)).ToString();

                    }

                    else if (esifrom == 5)//basic+da+cca
                    {
                        txtNewESI.Text = Math.Round((Convert.ToDecimal(txtnewbasics.Text) + Convert.ToDecimal(txtnewdas.Text) + Convert.ToDecimal(txtnewCCAs.Text)) * (esi / 100) * (ESIEmployee / 100), 0).ToString();
                        lblNewESIEmpr.Text = Math.Round((Convert.ToDecimal(txtnewbasics.Text) + Convert.ToDecimal(txtnewdas.Text) + Convert.ToDecimal(txtnewCCAs.Text)) * (esi / 100) * (ESIEmployer / 100), 0).ToString();
                        lblNewESIWages.Text = (Convert.ToDecimal(txtnewbasics.Text) + Convert.ToDecimal(txtnewdas.Text) + Convert.ToDecimal(txtnewCCAs.Text)).ToString();

                    }
                    else if (esifrom == 6)//gross-bonus
                    {
                        txtNewESI.Text = Math.Round((newgross - Convert.ToDecimal(txtNewBonus.Text)) * (esi / 100) * (ESIEmployee / 100), 0).ToString();
                        lblNewESIEmpr.Text = Math.Round((newgross - Convert.ToDecimal(txtNewBonus.Text)) * (esi / 100) * (ESIEmployer / 100), 0).ToString();
                        lblNewESIWages.Text = (newgross - Convert.ToDecimal(txtNewBonus.Text)).ToString();

                    }
                    else if (esifrom == 7)//basic+da
                    {
                        txtNewESI.Text = Math.Round((Convert.ToDecimal(txtnewbasics.Text) + Convert.ToDecimal(txtnewdas.Text)) * (esi / 100) * (ESIEmployee / 100), 0).ToString();
                        lblNewESIEmpr.Text = Math.Round((Convert.ToDecimal(txtnewbasics.Text) + Convert.ToDecimal(txtnewdas.Text)) * (esi / 100) * (ESIEmployer / 100), 0).ToString();
                        lblNewESIWages.Text = (Convert.ToDecimal(txtnewbasics.Text) + Convert.ToDecimal(txtnewdas.Text)).ToString();

                    }
                    else if (esifrom == 8)//gross
                    {
                        txtNewESI.Text = Math.Round((newgross) * (esi / 100) * (ESIEmployee / 100), 0).ToString();
                        lblNewESIEmpr.Text = Math.Round((newgross) * (esi / 100) * (ESIEmployer / 100), 0).ToString();
                        lblNewESIWages.Text = (newgross).ToString();
                    }
                    else if (esifrom == 9)//gross-bonus-la
                    {
                        txtNewESI.Text = Math.Round((newgross - Convert.ToDecimal(txtNewBonus.Text) - Convert.ToDecimal(txtNewLeaveEncashAmt.Text)) * (esi / 100) * (ESIEmployee / 100), 0).ToString();
                        lblNewESIEmpr.Text = Math.Round((newgross - Convert.ToDecimal(txtNewBonus.Text) - Convert.ToDecimal(txtNewLeaveEncashAmt.Text)) * (esi / 100) * (ESIEmployer / 100), 0).ToString();
                        lblNewESIWages.Text = (newgross - Convert.ToDecimal(txtNewBonus.Text) - Convert.ToDecimal(txtNewLeaveEncashAmt.Text)).ToString();
                    }


                    else if (esifrom == 10)//Gross-OA
                    {
                        txtNewESI.Text = Math.Round((newgross - Convert.ToDecimal(txtNewOA.Text)) * (esi / 100) * (ESIEmployee / 100), 0).ToString();
                        lblNewESIEmpr.Text = Math.Round((newgross - Convert.ToDecimal(txtNewOA.Text)) * (esi / 100) * (ESIEmployer / 100), 0).ToString();
                        lblNewESIWages.Text = (newgross - Convert.ToDecimal(txtNewOA.Text)).ToString();

                    }

                    txtArrearesi.Text = (Convert.ToDecimal(txtNewESI.Text) - Convert.ToDecimal(txtESI.Text)).ToString();
                    lblArrearESIWages.Text = (Convert.ToDecimal(lblNewESIWages.Text) - Convert.ToDecimal(lblESIWages.Text)).ToString();
                    lblArrearESIEmpr.Text = (Convert.ToDecimal(lblNewESIEmpr.Text) - Convert.ToDecimal(lblESIEmpr.Text)).ToString();



                    if (esilimit == 0)
                    {

                        TotalEsiWages = Math.Round(Convert.ToDecimal(txtArrearesi.Text) * 100 / ESIEmployee, 0);
                        ESIOnDuties = Convert.ToDecimal(txtArrearesi.Text);
                        TotalEsiEmpr = (TotalEsiWages * (esi / 100) * (ESIEmployer / 100));


                        TotalESIWagesE = TotalESIWagesE + TotalEsiWages;
                        totalesiV = Math.Ceiling(ESIOnDuties + ESIOnOTs + ESIOnNhs);

                        if (Convert.ToDecimal(lblsrate.Text) > 21000)
                        {
                            TotalEsiEmpr = 0;
                            TotalESIWagesE = 0;
                            totalesiV = 0;
                            ESIOnOTs = 0;
                            ESIOnDuties = 0;
                            ESIOnNhs = 0;
                        }

                    }

                    if (esilimit != 0)
                    {
                        if (Convert.ToDecimal(lblsrate.Text) >= esilimit)
                        {
                            TotalESIWagesE = 0;
                            ESIOnOTs = 0;
                            ESIOnDuties = 0;
                            ESIOnNhs = 0;
                            TotalEsiEmpr = 0;
                            totalesiV = 0;
                        }
                        else
                        {
                            TotalESIWagesE = TotalESIWagesE + TotalEsiWages;
                            totalesiV = Math.Ceiling(ESIOnDuties + ESIOnOTs + ESIOnNhs);
                            TotalEsiEmpr = TotalEsiEmpr;
                        }

                    }



                }
                else
                {
                    TotalEsiWages = 0;
                    TotalESIWagesE = 0;
                    ESIOnDuties = 0;
                    ESIOnOTs = 0;
                    ESIOnNhs = 0;
                    TotalEsiEmpr = 0;
                    totalesiV = 0;
                    txtNewESI.Text = "0";

                    txtArrearesi.Text = (Convert.ToDecimal(txtNewESI.Text) - Convert.ToDecimal(txtESI.Text)).ToString();
                    lblArrearESIWages.Text = (Convert.ToDecimal(lblNewESIWages.Text) - Convert.ToDecimal(lblESIWages.Text)).ToString();
                    lblArrearESIEmpr.Text = (Convert.ToDecimal(lblNewESIEmpr.Text) - Convert.ToDecimal(lblESIEmpr.Text)).ToString();


                }


                for (int l = 0; l < dtproftax.Rows.Count; l++)
                {
                    ProfTaxRangeFrom = Convert.ToDecimal(dtproftax.Rows[l]["RangeFrom"].ToString());
                    ProfTaxRangeTill = Convert.ToDecimal(dtproftax.Rows[l]["RangeTill"].ToString());
                    ProfTaxAmount = Convert.ToDecimal(dtproftax.Rows[l]["ProfTax"].ToString());

                    if ((Convert.ToDecimal(txtArrearGross.Text)) > ProfTaxRangeFrom && (Convert.ToDecimal(txtArrearGross.Text)) <= ProfTaxRangeTill)
                    {
                        ProfTax = ProfTaxAmount;
                    }

                }

                TextBox txtArrearPT = GVListEmployeess.Rows[k].FindControl("txtArrearPT") as TextBox;
                txtArrearPT.Text = (ProfTax).ToString();

                TextBox txtArrearTotalDed = GVListEmployeess.Rows[k].FindControl("txtArrearTotalDed") as TextBox;
                txtArrearTotalDed.Text = (PFOnDuties + totalesiV + ProfTax).ToString();

                //Net
                Label lblArrearNet = GVListEmployeess.Rows[k].FindControl("lblArrearNet") as Label;
                lblArrearNet.Text = ((Convert.ToDecimal(txtArrearGross.Text) + Convert.ToDecimal(lblArrearOTAmt.Text) + Convert.ToDecimal(lblArrearNHsAmt.Text)) - Convert.ToDecimal(txtArrearTotalDed.Text)).ToString();

            }


        }


        public void NonGeneratedPaysheetList()
        {
            string daten = string.Empty;
            if (txtmonth.Text.Trim().Length > 0)
            {
                daten = DateTime.Parse(txtmonth.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
            }

            string month = DateTime.Parse(daten).Month.ToString();
            string Year = DateTime.Parse(daten).Year.ToString().Substring(2, 2);

            string qry = "select Timings from emppaysheetArrear where clientid='" + ddlClientId.SelectedValue + "' and month='" + month + Year + "'";
            DataTable dts = config.ExecuteReaderWithQueryAsync(qry).Result;
            string date = "";

            if (dts.Rows.Count > 0)
            {
                date = dts.Rows[0]["Timings"].ToString();
                PnlNonGeneratedPaysheet.Visible = true;

                lblPaysheetGeneratedTime.Text = "<b>Arrear paysheet generated on  " + date + "</b>";

            }
            else
            {
                lblPaysheetGeneratedTime.Text = "";
                PnlNonGeneratedPaysheet.Visible = false;
            }
        }

        protected void btnGeneratePayment_Click(object sender, EventArgs e)
        {
            try
            {


                Hashtable HtArrearWages = new Hashtable();
                string SPName = "";
                var IRecordStatus = 0;

                string date = string.Empty;
                if (txtmonth.Text.Trim().Length > 0)
                {
                    date = DateTime.Parse(txtmonth.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
                }

                string month = DateTime.Parse(date).Month.ToString();
                string monthn = DateTime.Parse(date).Month.ToString("00");
                string Year = DateTime.Parse(date).Year.ToString().Substring(2, 2);


                DateTime DtLastDay = DateTime.Now;
                DtLastDay = DateTime.Parse(txtmonth.Text, CultureInfo.GetCultureInfo("en-gb"));

                var ContractID = "";

                #region  Begin Get Contract Id Based on The Last Day
                Hashtable HtGetContractID = new Hashtable();
                var SPNameForGetContractID = "GetContractIDBasedOnthMonth";
                HtGetContractID.Add("@clientid", ddlClientId.SelectedValue);
                HtGetContractID.Add("@LastDay", DtLastDay);
                DataTable DTContractID = config.ExecuteAdaptorAsyncWithParams(SPNameForGetContractID, HtGetContractID).Result;
                if (DTContractID.Rows.Count > 0)
                {
                    ContractID = DTContractID.Rows[0]["contractid"].ToString();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "", "alert('Contract Details Are Not Avaialable For This Client.');", true);
                    return;
                }
                #endregion  End Get Contract Id Based on The Last Day

                string query = "select PF,esi,pffrom,esifrom,pfonot,esionot,esilimit from Contracts where ClientId='" + ddlClientId.SelectedValue + "' and contractid='" + ContractID + "'";
                DataTable dtquery = config.ExecuteReaderWithQueryAsync(query).Result;



                decimal pf = 100, esi = 100, PFEmployee = 12, ESIEmployee = 1, PFEmployer = 14, ESIEmployer = 3, esilimit = 0;
                int pffrom = 0, esifrom = 0, pfon = 0, esion = 0;
                if (dtquery.Rows.Count > 0)
                {
                    pf = Convert.ToDecimal(dtquery.Rows[0]["PF"].ToString());
                    esi = Convert.ToDecimal(dtquery.Rows[0]["esi"].ToString());
                    pffrom = Convert.ToInt32(dtquery.Rows[0]["pffrom"].ToString());
                    esifrom = Convert.ToInt32(dtquery.Rows[0]["esifrom"].ToString());
                    pfon = Convert.ToInt32(dtquery.Rows[0]["PFonOT"].ToString());
                    esion = Convert.ToInt32(dtquery.Rows[0]["ESIonOT"].ToString());
                    esilimit = Convert.ToDecimal(dtquery.Rows[0]["esilimit"].ToString());
                }

                query = "select PFEmployee,ESIEmployee,PFEmployer,ESIEmployer from TblOptions where '" + DtLastDay + "' between fromdate and todate ";
                DataTable dttbloptions = config.ExecuteReaderWithQueryAsync(query).Result;
                if (dttbloptions.Rows.Count > 0)
                {
                    PFEmployee = Convert.ToDecimal(dttbloptions.Rows[0]["PFEmployee"].ToString());
                    ESIEmployee = Convert.ToDecimal(dttbloptions.Rows[0]["ESIEmployee"].ToString());
                    PFEmployer = Convert.ToDecimal(dttbloptions.Rows[0]["PFEmployer"].ToString());
                    ESIEmployer = Convert.ToDecimal(dttbloptions.Rows[0]["ESIEmployer"].ToString());

                }



                int ArrearWagesCount = 0;
                decimal TotalPFWages = 0;
                decimal TotalPFWagesE = 0;
                decimal PFOnDuties = 0;
                decimal PFOnOT = 0;
                decimal PFOnNHs = 0;
                decimal TotalPFEmpr = 0;
                decimal TotalEsiWages = 0;
                decimal TotalESIWagesE = 0;
                decimal ESIOnDuties = 0;
                decimal ESIOnOTs = 0;
                decimal ESIOnNhs = 0;
                decimal totalesiV = 0;
                decimal TotalEsiEmpr = 0;
                decimal SrvCharges = 0;

                for (int k = 0; k < GVListEmployeess.Rows.Count; k++)
                {
                    HtArrearWages.Clear();



                    CheckBox chkindividual = GVListEmployeess.Rows[k].FindControl("chkindividual") as CheckBox;
                    Label lblEMPid = GVListEmployeess.Rows[k].FindControl("lblEMPid") as Label;
                    Label lbldesignid = GVListEmployeess.Rows[k].FindControl("lbldesignid") as Label;
                    Label lblsrate = GVListEmployeess.Rows[k].FindControl("lblsrate") as Label;
                    Label lblnodays = GVListEmployeess.Rows[k].FindControl("lblnodays") as Label;
                    Label lblots = GVListEmployeess.Rows[k].FindControl("lblots") as Label;
                    if (lblots.Text == "")
                    {
                        lblots.Text = "0";
                    }
                    Label lblWOs = GVListEmployeess.Rows[k].FindControl("lblWOs") as Label;
                    if (lblWOs.Text == "")
                    {
                        lblWOs.Text = "0";
                    }
                    Label lblnhs = GVListEmployeess.Rows[k].FindControl("lblnhs") as Label;
                    if (lblnhs.Text == "")
                    {
                        lblnhs.Text = "0";
                    }
                    TextBox txtArrearBasic = GVListEmployeess.Rows[k].FindControl("txtArrearBasic") as TextBox;
                    if (txtArrearBasic.Text == "")
                    {
                        txtArrearBasic.Text = "0";
                    }
                    TextBox txtArreardas = GVListEmployeess.Rows[k].FindControl("txtArreardas") as TextBox;
                    if (txtArreardas.Text == "")
                    {
                        txtArreardas.Text = "0";
                    }
                    TextBox txtArrearHRA = GVListEmployeess.Rows[k].FindControl("txtArrearHRA") as TextBox;
                    if (txtArrearHRA.Text == "")
                    {
                        txtArrearHRA.Text = "0";
                    }
                    TextBox txtArrearCCA = GVListEmployeess.Rows[k].FindControl("txtArrearCCA") as TextBox;
                    if (txtArrearCCA.Text == "")
                    {
                        txtArrearCCA.Text = "0";
                    }
                    TextBox txtArrearConv = GVListEmployeess.Rows[k].FindControl("txtArrearConv") as TextBox;
                    if (txtArrearConv.Text == "")
                    {
                        txtArrearConv.Text = "0";
                    }
                    TextBox txtArrearWA = GVListEmployeess.Rows[k].FindControl("txtArrearWA") as TextBox;
                    if (txtArrearWA.Text == "")
                    {
                        txtArrearWA.Text = "0";
                    }
                    TextBox txtArrearOA = GVListEmployeess.Rows[k].FindControl("txtArrearOA") as TextBox;
                    if (txtArrearOA.Text == "")
                    {
                        txtArrearOA.Text = "0";
                    }
                    TextBox txtArrearLeaveEncashAmt = GVListEmployeess.Rows[k].FindControl("txtArrearLeaveEncashAmt") as TextBox;
                    if (txtArrearLeaveEncashAmt.Text == "")
                    {
                        txtArrearLeaveEncashAmt.Text = "0";
                    }
                    TextBox txtArrearGratuity = GVListEmployeess.Rows[k].FindControl("txtArrearGratuity") as TextBox;
                    if (txtArrearGratuity.Text == "")
                    {
                        txtArrearGratuity.Text = "0";
                    }
                    TextBox txtArrearNfhs = GVListEmployeess.Rows[k].FindControl("txtArrearNfhs") as TextBox;
                    if (txtArrearNfhs.Text == "")
                    {
                        txtArrearNfhs.Text = "0";
                    }
                    TextBox txtArrearBonus = GVListEmployeess.Rows[k].FindControl("txtArrearBonus") as TextBox;
                    if (txtArrearBonus.Text == "")
                    {
                        txtArrearBonus.Text = "0";
                    }
                    TextBox txtArrearattBonus = GVListEmployeess.Rows[k].FindControl("txtArrearattBonus") as TextBox;
                    if (txtArrearattBonus.Text == "")
                    {
                        txtArrearattBonus.Text = "0";
                    }

                    TextBox txtArrearSplAllw = GVListEmployeess.Rows[k].FindControl("txtArrearSplAllw") as TextBox;
                    if (txtArrearSplAllw.Text == "")
                    {
                        txtArrearSplAllw.Text = "0";
                    }
                    TextBox txtArrearProfAllw = GVListEmployeess.Rows[k].FindControl("txtArrearProfAllw") as TextBox;
                    if (txtArrearProfAllw.Text == "")
                    {
                        txtArrearProfAllw.Text = "0";
                    }


                    TextBox txtArrearRC = GVListEmployeess.Rows[k].FindControl("txtArrearRC") as TextBox;
                    if (txtArrearRC.Text == "")
                    {
                        txtArrearRC.Text = "0";
                    }
                    TextBox txtArrearCS = GVListEmployeess.Rows[k].FindControl("txtArrearCS") as TextBox;
                    if (txtArrearCS.Text == "")
                    {
                        txtArrearCS.Text = "0";
                    }

                    TextBox txtArrearFoodAllw = GVListEmployeess.Rows[k].FindControl("txtArrearFoodAllw") as TextBox;
                    if (txtArrearFoodAllw.Text == "")
                    {
                        txtArrearFoodAllw.Text = "0";
                    }

                    TextBox txtArrearMedicalAllw = GVListEmployeess.Rows[k].FindControl("txtArrearMedicalAllw") as TextBox;
                    if (txtArrearMedicalAllw.Text == "")
                    {
                        txtArrearMedicalAllw.Text = "0";
                    }

                    TextBox txtArrearTravelAllw = GVListEmployeess.Rows[k].FindControl("txtArrearTravelAllw") as TextBox;
                    if (txtArrearTravelAllw.Text == "")
                    {
                        txtArrearTravelAllw.Text = "0";
                    }

                    TextBox txtArrearMobileAllw = GVListEmployeess.Rows[k].FindControl("txtArrearMobileAllw") as TextBox;
                    if (txtArrearMobileAllw.Text == "")
                    {
                        txtArrearMobileAllw.Text = "0";
                    }

                    TextBox txtArrearServiceWeightage = GVListEmployeess.Rows[k].FindControl("txtArrearServiceWeightage") as TextBox;
                    if (txtArrearServiceWeightage.Text == "")
                    {
                        txtArrearServiceWeightage.Text = "0";
                    }

                    TextBox txtArrearNightShiftAllw = GVListEmployeess.Rows[k].FindControl("txtArrearNightShiftAllw") as TextBox;
                    if (txtArrearNightShiftAllw.Text == "")
                    {
                        txtArrearNightShiftAllw.Text = "0";
                    }

                    TextBox txtArrearOTHrsAmt = GVListEmployeess.Rows[k].FindControl("txtArrearOTHrsAmt") as TextBox;
                    if (txtArrearOTHrsAmt.Text == "")
                    {
                        txtArrearOTHrsAmt.Text = "0";
                    }

                    TextBox txtArrearWOAmount = GVListEmployeess.Rows[k].FindControl("txtArrearWOAmount") as TextBox;
                    if (txtArrearWOAmount.Text == "")
                    {
                        txtArrearWOAmount.Text = "0";
                    }



                    TextBox txtPaysheetGross = GVListEmployeess.Rows[k].FindControl("txtPaysheetGross") as TextBox;
                    if (txtPaysheetGross.Text == "")
                    {
                        txtPaysheetGross.Text = "0";
                    }
                    TextBox txtArrearGross = GVListEmployeess.Rows[k].FindControl("txtArrearGross") as TextBox;
                    if (txtArrearGross.Text == "")
                    {
                        txtArrearGross.Text = "0";
                    }
                    TextBox txtArrearpf = GVListEmployeess.Rows[k].FindControl("txtArrearpf") as TextBox;
                    if (txtArrearpf.Text == "")
                    {
                        txtArrearpf.Text = "0";
                    }

                    Label lblArrearPFWages = GVListEmployeess.Rows[k].FindControl("lblArrearPFWages") as Label;
                    if (lblArrearPFWages.Text == "")
                    {
                        lblArrearPFWages.Text = "0";
                    }

                    Label lblArrearPFEmpr = GVListEmployeess.Rows[k].FindControl("lblArrearPFEmpr") as Label;
                    if (lblArrearPFEmpr.Text == "")
                    {
                        lblArrearPFEmpr.Text = "0";
                    }

                    TextBox txtArrearesi = GVListEmployeess.Rows[k].FindControl("txtArrearesi") as TextBox;
                    if (txtArrearesi.Text == "")
                    {
                        txtArrearesi.Text = "0";
                    }

                    Label lblArrearESIEmpr = GVListEmployeess.Rows[k].FindControl("lblArrearESIEmpr") as Label;
                    if (lblArrearESIEmpr.Text == "")
                    {
                        lblArrearESIEmpr.Text = "0";
                    }

                    Label lblArrearESIWages = GVListEmployeess.Rows[k].FindControl("lblArrearESIWages") as Label;
                    if (lblArrearESIWages.Text == "")
                    {
                        lblArrearESIWages.Text = "0";
                    }

                    TextBox txtArrearPT = GVListEmployeess.Rows[k].FindControl("txtArrearPT") as TextBox;
                    if (txtArrearPT.Text == "")
                    {
                        txtArrearPT.Text = "0";
                    }
                    Label lblArrearOTAmt = GVListEmployeess.Rows[k].FindControl("lblArrearOTAmt") as Label;
                    if (lblArrearOTAmt.Text == "")
                    {
                        lblArrearOTAmt.Text = "0";
                    }
                    Label lblArrearNHsAmt = GVListEmployeess.Rows[k].FindControl("lblArrearNHsAmt") as Label;
                    if (lblArrearNHsAmt.Text == "")
                    {
                        lblArrearNHsAmt.Text = "0";
                    }
                    TextBox txtArrearTotalDed = GVListEmployeess.Rows[k].FindControl("txtArrearTotalDed") as TextBox;
                    if (txtArrearTotalDed.Text == "")
                    {
                        txtArrearTotalDed.Text = "0";
                    }
                    Label lblArrearNet = GVListEmployeess.Rows[k].FindControl("lblArrearNet") as Label;
                    if (lblArrearNet.Text == "")
                    {
                        lblArrearNet.Text = "0";
                    }

                    TextBox txtArrearIncentivs = GVListEmployeess.Rows[k].FindControl("txtArrearIncentivs") as TextBox;
                    if (txtArrearIncentivs.Text == "")
                    {
                        txtArrearIncentivs.Text = "0";
                    }

                    TextBox txtArrearPLAmount = GVListEmployeess.Rows[k].FindControl("txtArrearPLAmount") as TextBox;
                    if (txtArrearPLAmount.Text == "")
                    {
                        txtArrearPLAmount.Text = "0";
                    }

                    TextBox txtArrearArrears = GVListEmployeess.Rows[k].FindControl("txtArrearArrears") as TextBox;
                    if (txtArrearArrears.Text == "")
                    {
                        txtArrearArrears.Text = "0";
                    }

                    Label lblArrearNpotsAmt = GVListEmployeess.Rows[k].FindControl("lblArrearNpotsAmt") as Label;
                    if (lblArrearNpotsAmt.Text == "")
                    {
                        lblArrearNpotsAmt.Text = "0";
                    }

                    //if (float.Parse(lblArrearNet.Text) > 0)
                    if (chkindividual.Checked)
                    {

                        SPName = "IMGenratePaysheetArreara";
                        HtArrearWages.Add("@ContractId", ContractID);
                        HtArrearWages.Add("@Clientid", ddlClientId.SelectedValue);
                        HtArrearWages.Add("@month", month + Year);
                        HtArrearWages.Add("@testrecord", ArrearWagesCount);
                        HtArrearWages.Add("@monthnew", Year + monthn);
                        HtArrearWages.Add("@empid", lblEMPid.Text);
                        HtArrearWages.Add("@Desgn", lbldesignid.Text);
                        HtArrearWages.Add("@NoOfDuties", lblnodays.Text);
                        HtArrearWages.Add("@wo", lblWOs.Text);
                        HtArrearWages.Add("@ots", lblots.Text);
                        HtArrearWages.Add("@nhs", lblnhs.Text);
                        HtArrearWages.Add("@basic", txtArrearBasic.Text);
                        HtArrearWages.Add("@DA", txtArreardas.Text);
                        HtArrearWages.Add("@HRA", txtArrearHRA.Text);
                        HtArrearWages.Add("@CCA", txtArrearCCA.Text);
                        HtArrearWages.Add("@Conveyance", txtArrearConv.Text);
                        HtArrearWages.Add("@LeaveEncashAmt", txtArrearLeaveEncashAmt.Text);
                        HtArrearWages.Add("@Bonus", txtArrearBonus.Text);
                        HtArrearWages.Add("@AttBonus", txtArrearattBonus.Text);
                        HtArrearWages.Add("@SpecialAllowance", txtArrearSplAllw.Text);
                        HtArrearWages.Add("@ProfAllowance", txtArrearProfAllw.Text);
                        HtArrearWages.Add("@Incentivs", txtArrearIncentivs.Text);
                        HtArrearWages.Add("@RC", txtArrearRC.Text);
                        HtArrearWages.Add("@CS", txtArrearCS.Text);
                        HtArrearWages.Add("@WashAllowance", txtArrearWA.Text);
                        HtArrearWages.Add("@OtherAllowance", txtArrearOA.Text);
                        HtArrearWages.Add("@Nfhs", txtArrearNfhs.Text);
                        HtArrearWages.Add("@gross", txtArrearGross.Text);
                        HtArrearWages.Add("@otamt", lblArrearOTAmt.Text);
                        HtArrearWages.Add("@NHsAmt", lblArrearNHsAmt.Text);
                        HtArrearWages.Add("@TempGross", lblsrate.Text);
                        HtArrearWages.Add("@PF", txtArrearpf.Text);
                        HtArrearWages.Add("@PFEmpr", TotalPFEmpr);
                        HtArrearWages.Add("@PFWAGES", TotalPFWagesE);
                        HtArrearWages.Add("@ESIWAGES", TotalESIWagesE);
                        HtArrearWages.Add("@ESIEmpr", TotalEsiEmpr);
                        HtArrearWages.Add("@ESI", txtArrearesi.Text);
                        HtArrearWages.Add("@ProfTax", txtArrearPT.Text);
                        HtArrearWages.Add("@TotalDeductions", txtArrearTotalDed.Text);
                        HtArrearWages.Add("@ActualAmount", lblArrearNet.Text);
                        HtArrearWages.Add("@FoodAllowance", txtArrearFoodAllw.Text);
                        HtArrearWages.Add("@MedicalAllw", txtArrearMedicalAllw.Text);
                        HtArrearWages.Add("@TravelAllw", txtArrearTravelAllw.Text);
                        HtArrearWages.Add("@MobileAllw", txtArrearMobileAllw.Text);
                        HtArrearWages.Add("@NightAllw", txtArrearNightShiftAllw.Text);
                        HtArrearWages.Add("@ServiceWeightage", txtArrearServiceWeightage.Text);
                        HtArrearWages.Add("@OTHrsAmt", txtArrearOTHrsAmt.Text);
                        HtArrearWages.Add("@WOAmt", txtArrearWOAmount.Text);
                        HtArrearWages.Add("@PLAmount", txtArrearPLAmount.Text);
                        HtArrearWages.Add("@Arrears", txtArrearArrears.Text);
                        HtArrearWages.Add("@Npotsamt", lblArrearNpotsAmt.Text);
                        IRecordStatus = config.ExecuteNonQueryParamsAsync(SPName, HtArrearWages).Result;
                        if (IRecordStatus != 0)
                        {
                            ArrearWagesCount++;
                        }
                    }


                }

                NonGeneratedPaysheetList();
            }
            catch (Exception ex)
            {

            }

        }
        protected void txtmonth_TextChanged(object sender, EventArgs e)
        {
            if (txtmonth.Text != "")
            {


                string date = string.Empty;

                if (txtmonth.Text.Trim().Length > 0)
                {
                    date = DateTime.Parse(txtmonth.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
                }

                int month = DateTime.Parse(date).Month;
                int Year = DateTime.Parse(date).Year;

                int noofdays = GlobalData.Instance.GetNoOfDaysOfThisMonth(Year, month);

                txtNoofdays.Text = noofdays.ToString();
                NonGeneratedPaysheetList();
            }
            else
            {
                txtNoofdays.Text = "";
            }
        }

    }
}