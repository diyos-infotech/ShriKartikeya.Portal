using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using ShriKartikeya.Portal.DAL;

namespace ShriKartikeya.Portal
{
    public partial class TDSDemo : System.Web.UI.Page
    {
        AppConfiguration config = new AppConfiguration();

        DropDownList bind_dropdownlistOthersections;
        DropDownList bind_dropdownlisttaxes;

        protected void Page_Load(object sender, EventArgs e)

        {

            int i = 0;
            try
            {

                if (!IsPostBack)
                {
                    Session["HRAAmts"] = 0;
                    Session["LTAAmts"] = 0;
                    Session["TaxDeds"] = 0;
                    Session["Othersections"] = 0;
                    Session["RentTypes"] = 0;
                    FinancialYears();

                    DataTable dtDummyTable = new DataTable();
                    dtDummyTable.Columns.Add(new DataColumn("IndexCol"));
                    for (int j = 0; j < 2; j++)
                        dtDummyTable.Rows.Add(j + 1);

                    gvLTA.DataSource = dtDummyTable;

                    gvLTA.DataBind();
                    DisplayDefaultRow();
                    Enable5Rows();


                    HRAdisplaydata();
                    HRADisplayDefaultRow();
                    HRAEnable5Rows();

                    Taxdisplaydata();
                    TaxDisplayDefaultRow();
                    TaxEnable5Rows();

                    Othersecdisplaydata();
                    OthersecDisplayDefaultRow();
                    OthersecEnable5Rows();


                    RentTypesdisplaydata();
                    RentTypesDisplayDefaultRow();
                    RentTypesEnable5Rows();

                }
            }

            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Show alert", "alert('Your Session Expired');", true);
                Response.Redirect("~/Login.aspx");
            }
        }

        public void FinancialYears()
        {
            string spname = "TDS_GetFinancialYears";
            DataTable dtFY = config.ExecuteReaderWithSPAsync(spname).Result;
            if (dtFY.Rows.Count > 0)
            {
                lblfinancialyearDates.Text = dtFY.Rows[0]["financialyearDates"].ToString();
                lblFyear.Text = dtFY.Rows[0]["financialyear"].ToString();
            }
        }


        private void RentTypesdisplaydata()
        {

            string Qry = "select * from TDS_HRARentTypes order By ID ";
            DataTable dt = config.ExecuteAdaptorAsyncWithQueryParams(Qry).Result;

            gvIncomeHouse.DataSource = dt;
            gvIncomeHouse.DataBind();


            foreach (GridViewRow grdRow in gvIncomeHouse.Rows)
            {
                bind_dropdownlisttaxes = (DropDownList)(gvIncomeHouse.Rows[grdRow.RowIndex].Cells[0].FindControl("ddlrenttypes"));
                bind_dropdownlisttaxes.Items.Clear();

                if (dt.Rows.Count > 0)
                {
                    bind_dropdownlisttaxes.DataValueField = "ID";
                    bind_dropdownlisttaxes.DataTextField = "RentTypes";
                    bind_dropdownlisttaxes.DataSource = dt;
                    bind_dropdownlisttaxes.DataBind();

                }
                bind_dropdownlisttaxes.Items.Insert(0, "--Select--");
                bind_dropdownlisttaxes.SelectedIndex = 0;

            }

        }

        protected void RentTypesEnable5Rows()
        {
            btnadddesgnre_Click1(this, null);
            btnadddesgnre_Click1(this, null);
        }

        protected void RentTypesDisplayDefaultRow()
        {
            for (int i = 0; i < gvIncomeHouse.Rows.Count; i++)
            {
                if (i < 1)
                {
                    Session["RentTypes"] = Convert.ToInt16(Session["RentTypes"]) + 1;
                    gvIncomeHouse.Rows[i].Visible = true;
                    RentTypesDefaultRowData(i);
                }
                else
                    gvIncomeHouse.Rows[i].Visible = false;
            }
            Session["RentTypes"] = 1;
            int check = int.Parse(Session["RentTypes"].ToString());
        }

        protected void RentTypesDefaultRowData(int row)
        {
            string Cddldesignation = ((DropDownList)gvIncomeHouse.Rows[row].FindControl("ddlrenttypes")).Text;
            DropDownList ddlrenttypes = gvIncomeHouse.Rows[row].FindControl("ddlrenttypes") as DropDownList;
            ddlrenttypes.SelectedIndex = 0;

            TextBox txtselflayout = (TextBox)gvIncomeHouse.Rows[row].FindControl("txtselflayout");
            txtselflayout.Text = "";

            TextBox txtgrossannualreport = (TextBox)gvIncomeHouse.Rows[row].FindControl("txtgrossannualreport");
            txtgrossannualreport.Text = "";

            TextBox txtMunicipaltax = (TextBox)gvIncomeHouse.Rows[row].FindControl("txtMunicipaltax");
            txtMunicipaltax.Text = "";

            TextBox txtnetannaul = (TextBox)gvIncomeHouse.Rows[row].FindControl("txtnetannaul");
            txtnetannaul.Text = "";

            TextBox txtlessstd = (TextBox)gvIncomeHouse.Rows[row].FindControl("txtlessstd");
            txtlessstd.Text = "";

            TextBox txtInterest = (TextBox)gvIncomeHouse.Rows[row].FindControl("txtInterest");
            txtInterest.Text = "";

            TextBox txtincome = (TextBox)gvIncomeHouse.Rows[row].FindControl("txtincome");
            txtincome.Text = "";

        }

        protected void btnadddesgnre_Click1(object sender, EventArgs e)
        {
            int designCount = Convert.ToInt16(Session["RentTypes"]);
            if (designCount < gvIncomeHouse.Rows.Count)
            {
                gvIncomeHouse.Rows[designCount].Visible = true;
                RentTypesDefaultRowData(designCount);

                string Qry = "select * from TDS_HRARentTypes order By ID ";
                DataTable dt = config.ExecuteAdaptorAsyncWithQueryParams(Qry).Result;
                DropDownList ddlrenttypes = gvIncomeHouse.Rows[designCount].FindControl("ddlrenttypes") as DropDownList;
                ddlrenttypes.Items.Clear();



                if (dt.Rows.Count > 0)
                {
                    ddlrenttypes.DataValueField = "ID";
                    ddlrenttypes.DataTextField = "RentTypes";
                    ddlrenttypes.DataSource = dt;
                    ddlrenttypes.DataBind();

                }
                ddlrenttypes.Items.Insert(0, "--Select--");
                ddlrenttypes.SelectedIndex = 0;

                designCount = designCount + 1;
                Session["RentTypes"] = designCount;
            }
            else
            {
            }
        }




        private void Othersecdisplaydata()
        {

            string Qry = "select * from TDS_Othersections order By ID ";
            DataTable dt = config.ExecuteAdaptorAsyncWithQueryParams(Qry).Result;

            GVOtherSections.DataSource = dt;
            GVOtherSections.DataBind();


            foreach (GridViewRow grdRow in GVOtherSections.Rows)
            {
                bind_dropdownlistOthersections = (DropDownList)(GVOtherSections.Rows[grdRow.RowIndex].Cells[0].FindControl("ddlsection"));
                bind_dropdownlistOthersections.Items.Clear();

                if (dt.Rows.Count > 0)
                {
                    bind_dropdownlistOthersections.DataValueField = "ID";
                    bind_dropdownlistOthersections.DataTextField = "Othersections";
                    bind_dropdownlistOthersections.DataSource = dt;
                    bind_dropdownlistOthersections.DataBind();

                }
                bind_dropdownlistOthersections.Items.Insert(0, "--Select--");
                bind_dropdownlistOthersections.SelectedIndex = 0;

            }

        }

        protected void OthersecEnable5Rows()
        {
            btnadddesgn3_Click1(this, null);
            btnadddesgn3_Click1(this, null);
        }

        protected void OthersecDisplayDefaultRow()
        {
            for (int i = 0; i < GVOtherSections.Rows.Count; i++)
            {
                if (i < 1)
                {
                    Session["Othersections"] = Convert.ToInt16(Session["Othersections"]) + 1;
                    GVOtherSections.Rows[i].Visible = true;
                    OtherDefaultRowData(i);
                }
                else
                    GVOtherSections.Rows[i].Visible = false;
            }
            Session["Othersections"] = 1;
            int check = int.Parse(Session["Othersections"].ToString());
        }

        protected void OtherDefaultRowData(int row)
        {
            string Cddldesignation = ((DropDownList)GVOtherSections.Rows[row].FindControl("ddlsection")).Text;
            DropDownList ddlsection = GVOtherSections.Rows[row].FindControl("ddlsection") as DropDownList;
            ddlsection.SelectedIndex = 0;

            TextBox txtothersecamount = (TextBox)GVOtherSections.Rows[row].FindControl("txtothersecamount");
            txtothersecamount.Text = "";

            TextBox txtothersProof = (TextBox)GVOtherSections.Rows[row].FindControl("txtothersProof");
            txtothersProof.Text = "";

        }

        protected void btnadddesgn3_Click1(object sender, EventArgs e)
        {
            int designCount = Convert.ToInt16(Session["Othersections"]);
            if (designCount < GVOtherSections.Rows.Count)
            {
                GVOtherSections.Rows[designCount].Visible = true;
                OtherDefaultRowData(designCount);

                string Qry = "select * from TDS_Othersections order By ID ";
                DataTable dt = config.ExecuteAdaptorAsyncWithQueryParams(Qry).Result;
                DropDownList ddldrow = GVOtherSections.Rows[designCount].FindControl("ddlsection") as DropDownList;
                ddldrow.Items.Clear();



                if (dt.Rows.Count > 0)
                {
                    ddldrow.DataValueField = "ID";
                    ddldrow.DataTextField = "Othersections";
                    ddldrow.DataSource = dt;
                    ddldrow.DataBind();

                }
                ddldrow.Items.Insert(0, "--Select--");
                ddldrow.SelectedIndex = 0;

                designCount = designCount + 1;
                Session["Othersections"] = designCount;
            }
            else
            {
            }
        }


        private void Taxdisplaydata()
        {

            string Qry = "select * from TDS_TaxSavingDeductions order By ID ";
            DataTable dt = config.ExecuteAdaptorAsyncWithQueryParams(Qry).Result;

            GvTaxdeds.DataSource = dt;
            GvTaxdeds.DataBind();


            foreach (GridViewRow grdRow in GvTaxdeds.Rows)
            {
                bind_dropdownlisttaxes = (DropDownList)(GvTaxdeds.Rows[grdRow.RowIndex].Cells[0].FindControl("ddltaxDed"));
                bind_dropdownlisttaxes.Items.Clear();

                if (dt.Rows.Count > 0)
                {
                    bind_dropdownlisttaxes.DataValueField = "ID";
                    bind_dropdownlisttaxes.DataTextField = "TaxSavingDeductions";
                    bind_dropdownlisttaxes.DataSource = dt;
                    bind_dropdownlisttaxes.DataBind();

                }
                bind_dropdownlisttaxes.Items.Insert(0, "--Select--");
                bind_dropdownlisttaxes.SelectedIndex = 0;

            }

        }

        protected void TaxEnable5Rows()
        {
            btnadddesgn2_Click1(this, null);
            btnadddesgn2_Click1(this, null);
        }

        protected void TaxDisplayDefaultRow()
        {
            for (int i = 0; i < GvTaxdeds.Rows.Count; i++)
            {
                if (i < 1)
                {
                    Session["TaxDeds"] = Convert.ToInt16(Session["TaxDeds"]) + 1;
                    GvTaxdeds.Rows[i].Visible = true;
                    TaxDefaultRowData(i);
                }
                else
                    GvTaxdeds.Rows[i].Visible = false;
            }
            Session["TaxDeds"] = 1;
            int check = int.Parse(Session["TaxDeds"].ToString());
        }

        protected void TaxDefaultRowData(int row)
        {
            string Cddldesignation = ((DropDownList)GvTaxdeds.Rows[row].FindControl("ddltaxDed")).Text;
            DropDownList ddltaxDed = GvTaxdeds.Rows[row].FindControl("ddltaxDed") as DropDownList;
            ddltaxDed.SelectedIndex = 0;

            TextBox txttaxdedamount = (TextBox)GvTaxdeds.Rows[row].FindControl("txttaxdedamount");
            txttaxdedamount.Text = "";

            TextBox txtltaExpenses = (TextBox)GvTaxdeds.Rows[row].FindControl("txtltaExpenses");
            txtltaExpenses.Text = "";
        }

        protected void btnadddesgn2_Click1(object sender, EventArgs e)
        {
            int TaxDedsdesignCount = Convert.ToInt16(Session["TaxDeds"]);
            if (TaxDedsdesignCount < GvTaxdeds.Rows.Count)
            {
                GvTaxdeds.Rows[TaxDedsdesignCount].Visible = true;
                TaxDefaultRowData(TaxDedsdesignCount);

                string Qry = "select * from TDS_TaxSavingDeductions order By ID ";
                DataTable dt = config.ExecuteAdaptorAsyncWithQueryParams(Qry).Result;
                DropDownList ddldrow = GvTaxdeds.Rows[TaxDedsdesignCount].FindControl("ddltaxDed") as DropDownList;
                ddldrow.Items.Clear();



                if (dt.Rows.Count > 0)
                {
                    ddldrow.DataValueField = "ID";
                    ddldrow.DataTextField = "TaxSavingDeductions";
                    ddldrow.DataSource = dt;
                    ddldrow.DataBind();

                }
                ddldrow.Items.Insert(0, "--Select--");
                ddldrow.SelectedIndex = 0;

                TaxDedsdesignCount = TaxDedsdesignCount + 1;
                Session["TaxDeds"] = TaxDedsdesignCount;
            }
            else
            {
            }
        }



        private void HRAdisplaydata()
        {

            string Qry = "select * from TDS_TaxSavingDeductions order By ID ";
            DataTable dt = config.ExecuteAdaptorAsyncWithQueryParams(Qry).Result;

            GVHRA.DataSource = dt;
            GVHRA.DataBind();
        }

        protected void HRAEnable5Rows()
        {
            btnadddesgn_Click(this, null);
            btnadddesgn_Click(this, null);
        }

        protected void HRADisplayDefaultRow()
        {
            for (int i = 0; i < GVHRA.Rows.Count; i++)
            {
                if (i < 1)
                {
                    Session["HRAAmts"] = Convert.ToInt16(Session["HRAAmts"]) + 1;
                    GVHRA.Rows[i].Visible = true;
                    HRADefaultRowData(i);
                }
                else
                    GVHRA.Rows[i].Visible = false;
            }
            Session["HRAAmts"] = 1;
            int check = int.Parse(Session["HRAAmts"].ToString());
        }

        protected void HRADefaultRowData(int row)
        {

            TextBox txtRentpaid = (TextBox)GVHRA.Rows[row].FindControl("txtRentpaid");
            txtRentpaid.Text = "";

            TextBox txtrentProof = (TextBox)GVHRA.Rows[row].FindControl("txtrentProof");
            txtrentProof.Text = "";

            TextBox txtLandlord = (TextBox)GVHRA.Rows[row].FindControl("txtLandlord");
            txtLandlord.Text = "";

            TextBox txtPAN = (TextBox)GVHRA.Rows[row].FindControl("txtPAN");
            txtPAN.Text = "";

            TextBox txtAddressofLandlord = (TextBox)GVHRA.Rows[row].FindControl("txtAddressofLandlord");
            txtAddressofLandlord.Text = "";
        }

        protected void btnadddesgn_Click(object sender, EventArgs e)
        {
            int TaxDedsdesignCount = Convert.ToInt16(Session["HRAAmts"]);
            if (TaxDedsdesignCount < GVHRA.Rows.Count)
            {
                GVHRA.Rows[TaxDedsdesignCount].Visible = true;
                HRADefaultRowData(TaxDedsdesignCount);

                TaxDedsdesignCount = TaxDedsdesignCount + 1;
                Session["HRAAmts"] = TaxDedsdesignCount;
            }
            else
            {
            }
        }




        protected void Enable5Rows()
        {
            btnadddesgn_Click1(this, null);
            btnadddesgn_Click1(this, null);
        }

        protected void btnadddesgn_Click1(object sender, EventArgs e)
        {
            int designCount = Convert.ToInt16(Session["LTAAmts"]);
            if (designCount < gvLTA.Rows.Count)
            {
                gvLTA.Rows[designCount].Visible = true;
                DefaultRowData(designCount);


                designCount = designCount + 1;
                Session["LTAAmts"] = designCount;
            }
            else
            {
            }
        }

        protected void DisplayDefaultRow()
        {
            for (int i = 0; i < gvLTA.Rows.Count; i++)
            {
                if (i < 1)
                {
                    Session["LTAAmts"] = Convert.ToInt16(Session["LTAAmts"]) + 1;
                    gvLTA.Rows[i].Visible = true;
                    DefaultRowData(i);
                }
                else
                    gvLTA.Rows[i].Visible = false;
            }
            Session["LTAAmts"] = 1;
            int check = int.Parse(Session["LTAAmts"].ToString());
        }

        protected void DefaultRowData(int row)
        {
            TextBox txtLTAAmount = (TextBox)gvLTA.Rows[row].FindControl("txtLTAAmount");
            txtLTAAmount.Text = "";

            TextBox txtltaExpenses = (TextBox)gvLTA.Rows[row].FindControl("txtltaExpenses");
            txtltaExpenses.Text = "";

        }

        // public class Program
        //{

        public void Main(string Empid, string FYear)
        {
            AppConfiguration config1 = new AppConfiguration();


            //Employee income
            string QryIncome = "select * from TDS_EmpIncome where Empid='" + Empid + "' and FinancialYear='" + FYear + "'";
            DataTable dtIncome = config1.ExecuteAdaptorAsyncWithQueryParams(QryIncome).Result;
            double income = 0;
            if (dtIncome.Rows.Count > 0)
            {
                income = double.Parse(dtIncome.Rows[0]["Amount"].ToString());
            }


            //aggregate of all exclusions of the emp
            string QryExclusionAmounts = "select isnull(sum(isnull(ApprovedAmount,0)),0) ApprovedAmount from TDS_EmpApprovedExclusionAmounts where Empid='" + Empid + "' and FinancialYear='" + FYear + "'";
            DataTable dtExclusionAmounts = config1.ExecuteAdaptorAsyncWithQueryParams(QryExclusionAmounts).Result;
            double taxExclusions = 0;
            if (dtExclusionAmounts.Rows.Count > 0)
            {
                taxExclusions = double.Parse(dtExclusionAmounts.Rows[0]["ApprovedAmount"].ToString());
            }

            //standard deduction
            double stdDeduction = 0.0;

            //aggregate of all extra Income of emp
            string QryEmpOtherIncome = "select * from TDS_EmpOtherIncome where Empid='" + Empid + "' and FinancialYear='" + FYear + "'";
            DataTable dtEmpOtherIncome = config1.ExecuteAdaptorAsyncWithQueryParams(QryEmpOtherIncome).Result;
            double extraIncome = 0;
            if (dtEmpOtherIncome.Rows.Count > 0)
            {
                extraIncome = double.Parse(dtEmpOtherIncome.Rows[0]["Amount"].ToString());
            }




            string TdsFormulaName = "New Regime";

            string TdsFormulaId = "2";

            if (txtpan.Text.Trim() == "" || txtpan.Text.Trim() == "0")
            {
                TdsFormulaId = "5";
            }

            string NewQry = "select * from TDS_Slab where TdsFormulaId='" + TdsFormulaId + "' order by TdsSlabId";
            DataTable Newdt = config1.ExecuteAdaptorAsyncWithQueryParams(NewQry).Result;



            string GetCessPerQry = "select TaxLessAmount,exclusionsAllowed,Cess from TDS_Formula where TdsFormulaId='" + TdsFormulaId + "' order by TdsFormulaId";
            DataTable dtCessPer = config1.ExecuteAdaptorAsyncWithQueryParams(GetCessPerQry).Result;

            double cess = double.Parse(dtCessPer.Rows[0]["Cess"].ToString());
            double TaxLessAmount = double.Parse(dtCessPer.Rows[0]["TaxLessAmount"].ToString());




            //fetch from global db
            List<TaxSlab> taxSlabs = new List<TaxSlab>(Newdt.Rows.Count);
            for (int m = 0; m < Newdt.Rows.Count; m++)
            {
                double MinAmt = 0;
                double MaxAmt = 0;
                double Per = 0;
                MinAmt = double.Parse(Newdt.Rows[m]["MinAmount"].ToString());
                MaxAmt = double.Parse(Newdt.Rows[m]["MaxAmount"].ToString());
                Per = double.Parse(Newdt.Rows[m]["percentage"].ToString());

                taxSlabs.Add(new TaxSlab(MinAmt, MaxAmt, Per));
            }


            //get totalTax
            double taxableIncome = income + extraIncome;
            double TotalIncome = taxableIncome;
            double NewtotalTax = calcTotalTax(taxableIncome, taxSlabs, cess);
            double NewTaxOnAmt = GetTaxAmount(taxableIncome, taxSlabs, cess);
            double NewCessAmt = GetCessAmount(taxableIncome, taxSlabs, cess);


            double paidTax = 0.0;

            int currentMonth = 0;
            double NewmonthFinal = calcMonthly(currentMonth, paidTax, NewtotalTax);

            if (TotalIncome <= TaxLessAmount)
            {
                NewtotalTax = 0;
                NewTaxOnAmt = 0;
                NewCessAmt = 0;
                NewmonthFinal = 0;
            }


            TdsFormulaId = "1";
            if (double.Parse(txtage.Text) >= 60 && double.Parse(txtage.Text) <= 80)
            {
                TdsFormulaId = "3";
            }
            else if (double.Parse(txtage.Text) > 80)
            {
                TdsFormulaId = "4";
            }
            if (txtpan.Text.Trim() == "" || txtpan.Text.Trim() == "0")
            {
                TdsFormulaId = "5";
            }

            string OldQry = "select * from TDS_Slab where TdsFormulaId='" + TdsFormulaId + "' order by TdsSlabId";
            DataTable Olddt = config1.ExecuteAdaptorAsyncWithQueryParams(OldQry).Result;


            string GetoldCessPerQry = "select TaxLessAmount,exclusionsAllowed,Cess,StandardDed from TDS_Formula where TdsFormulaId='" + TdsFormulaId + "' order by TdsFormulaId";
            DataTable dtoldCessPer = config1.ExecuteAdaptorAsyncWithQueryParams(GetoldCessPerQry).Result;

            cess = double.Parse(dtoldCessPer.Rows[0]["Cess"].ToString());
            TaxLessAmount = double.Parse(dtoldCessPer.Rows[0]["TaxLessAmount"].ToString());
            string exclusionsAllowed = (dtoldCessPer.Rows[0]["exclusionsAllowed"].ToString());
            stdDeduction = double.Parse(dtoldCessPer.Rows[0]["StandardDed"].ToString());




            List<TaxSlab> taxSlabsOld = new List<TaxSlab>(Olddt.Rows.Count);
            for (int l = 0; l < Olddt.Rows.Count; l++)
            {
                double MinAmt = 0;
                double MaxAmt = 0;
                double Per = 0;
                MinAmt = double.Parse(Olddt.Rows[l]["MinAmount"].ToString());
                MaxAmt = double.Parse(Olddt.Rows[l]["MaxAmount"].ToString());
                Per = double.Parse(Olddt.Rows[l]["percentage"].ToString());

                taxSlabsOld.Add(new TaxSlab(MinAmt, MaxAmt, Per));
            }




            // aggregate of the paid taxes from April to current month from db

            double OldtaxableIncome = income + extraIncome;

            if (stdDeduction != 0)
            {
                taxExclusions += stdDeduction;
            }
            if (exclusionsAllowed != "0")
            {
                OldtaxableIncome -= taxExclusions;
            }
            double OldtotalTax = calcTotalTax(OldtaxableIncome, taxSlabsOld, cess);
            double OldTaxOnAmt = GetTaxAmount(OldtaxableIncome, taxSlabsOld, cess);
            double OldCessAmt = GetCessAmount(OldtaxableIncome, taxSlabsOld, cess);


            double OldmonthFinal = calcMonthly(currentMonth, paidTax, OldtotalTax);


            if (TotalIncome <= TaxLessAmount)
            {
                OldtotalTax = 0;
                OldTaxOnAmt = 0;
                OldCessAmt = 0;
                OldmonthFinal = 0;
            }

            //NewtotalTax = ROUND(NewtotalTax * 10.0) / 10;
            //OldtotalTax = ROUND(NewtotalTax * 10.0) / 10;

            string TDS_EmpYearlyData = "Insert into TDS_PrvEmpYearlyData(EmpId,FinancialYear,NewYearlyAmount,NewMonthlyAmount,OldYearlyAmount,OldMonthlyAmount,TotalIncome,TotalDed,OldTaxAmount,NewTaxAmount,OldCessAmount,NewCessAmount) " +
                  " values ('" + Empid + "','" + lblFyear.Text + "','" + NewtotalTax + "','" + NewmonthFinal + "','" + OldtotalTax + "','" + OldmonthFinal + "','" + TotalIncome + "','" + taxExclusions + "','" + OldTaxOnAmt + "','" + NewTaxOnAmt + "','" + OldCessAmt + "','" + NewCessAmt + "')";
            int dtTDS_EmpYearlyData = config1.ExecuteNonQueryWithQueryAsync(TDS_EmpYearlyData).Result;
            if (dtTDS_EmpYearlyData > 0)
            {
                lblPreFinancialYear.Text = FYear;
                lblPreEmpid.Text = Empid;
                lblPreEmpname.Text = txtname.Text;

                if (txtpan.Text.Trim() == "" || txtpan.Text.Trim() == "0")
                {
                    lblpanno.Text = "NA";
                }
                else
                {
                    lblpanno.Text = txtpan.Text;
                }

                lblage.Text = txtage.Text;


                string SpGetData = "TDS_GetPrvEmpYearlyData";
                Hashtable ht = new Hashtable();
                ht.Add("@Empid", Empid);
                ht.Add("@FinancialYear", FYear);
                DataTable dtgetprvamts = config.ExecuteAdaptorAsyncWithParams(SpGetData, ht).Result;
                if (dtgetprvamts.Rows.Count > 0)
                {
                    GvTDSPreAmounts.DataSource = dtgetprvamts;
                    GvTDSPreAmounts.DataBind();
                }

                divOldscheme.Visible = false;
                divNewscheme.Visible = false;

                if (OldtotalTax < NewtotalTax)
                {
                    divOldscheme.Visible = true;
                }
                else
                {
                    divNewscheme.Visible = true;
                }

                if (OldtotalTax == NewtotalTax)
                {
                    divOldscheme.Visible = false;
                    divNewscheme.Visible = false;
                }

                ModalPopupExtender1.Show();

            }


        }

        private int ROUND(double v)
        {
            throw new NotImplementedException();
        }

        public static double calcMonthly(int month, double paidAmount, double totalAmount)
        {
            //returns the amount to be paid on the currrent month
            //month - current month
            //paidAmount - amount paid till last month
            //totalAmount - total TDS of taxableIncome
            double newFinal = totalAmount - paidAmount;
            int remainingMonths = 12 - month;
            double monthFinal = newFinal / remainingMonths;
            return Math.Round(monthFinal, 2);
        }


        public static double calcTotalTax(double taxableInc, List<TaxSlab> taxSlabs, double cess)
        {
            //returns total tax amount (taxes + cess)
            //taxableInc - final taxable income inclusing other incomes without Exclusions
            //taxSlabs - taxSlabs from globaldb
            //cess - add and include all the cess (will be applied to tax)
            double taxAmount = 0, tempTaxable = taxableInc;

            for (int i = 0; i < taxSlabs.Count; i++)
            {
                TaxSlab taxSlab = taxSlabs[i];
                if (taxableInc > 0)
                {
                    double slabVal = taxSlab.maxAmount - taxSlab.minAmount;
                    if (taxableInc < slabVal)
                    {
                        slabVal = taxableInc;
                        taxableInc = 0;
                    }
                    else
                    {
                        taxableInc -= slabVal;
                    }
                    double slabTax = slabVal * (taxSlab.percentage / 100);
                    taxAmount = taxAmount + slabTax;
                    //Console.WriteLine("Slab Value Rs."+slabTax);
                }
            }
            Console.WriteLine("Before cess Value Rs." + taxAmount);
            //calculate cess to taxAmount
            double cessAmount = (taxAmount * cess) / 100;
            double finalTaxAmount = taxAmount + cessAmount;
            Console.WriteLine("After cess Value Rs." + finalTaxAmount);
            return finalTaxAmount;
        }

        public static double GetTaxAmount(double taxableInc, List<TaxSlab> taxSlabs, double cess)
        {
            //returns total tax amount (taxes + cess)
            //taxableInc - final taxable income inclusing other incomes without Exclusions
            //taxSlabs - taxSlabs from globaldb
            //cess - add and include all the cess (will be applied to tax)
            double taxAmount = 0, tempTaxable = taxableInc;

            for (int i = 0; i < taxSlabs.Count; i++)
            {
                TaxSlab taxSlab = taxSlabs[i];
                if (taxableInc > 0)
                {
                    double slabVal = taxSlab.maxAmount - taxSlab.minAmount;
                    if (taxableInc < slabVal)
                    {
                        slabVal = taxableInc;
                        taxableInc = 0;
                    }
                    else
                    {
                        taxableInc -= slabVal;
                    }
                    double slabTax = slabVal * (taxSlab.percentage / 100);
                    taxAmount = taxAmount + slabTax;
                    //Console.WriteLine("Slab Value Rs."+slabTax);
                }
            }
            Console.WriteLine("Before cess Value Rs." + taxAmount);
            //calculate cess to taxAmount
            double cessAmount = (taxAmount * cess) / 100;
            double finalTaxAmount = taxAmount + cessAmount;
            Console.WriteLine("After cess Value Rs." + finalTaxAmount);
            return taxAmount;
        }

        public static double GetCessAmount(double taxableInc, List<TaxSlab> taxSlabs, double cess)
        {
            //returns total tax amount (taxes + cess)
            //taxableInc - final taxable income inclusing other incomes without Exclusions
            //taxSlabs - taxSlabs from globaldb
            //cess - add and include all the cess (will be applied to tax)
            double taxAmount = 0, tempTaxable = taxableInc;

            for (int i = 0; i < taxSlabs.Count; i++)
            {
                TaxSlab taxSlab = taxSlabs[i];
                if (taxableInc > 0)
                {
                    double slabVal = taxSlab.maxAmount - taxSlab.minAmount;
                    if (taxableInc < slabVal)
                    {
                        slabVal = taxableInc;
                        taxableInc = 0;
                    }
                    else
                    {
                        taxableInc -= slabVal;
                    }
                    double slabTax = slabVal * (taxSlab.percentage / 100);
                    taxAmount = taxAmount + slabTax;
                    //Console.WriteLine("Slab Value Rs."+slabTax);
                }
            }
            Console.WriteLine("Before cess Value Rs." + taxAmount);
            //calculate cess to taxAmount
            double cessAmount = (taxAmount * cess) / 100;
            double finalTaxAmount = taxAmount + cessAmount;
            Console.WriteLine("After cess Value Rs." + finalTaxAmount);
            return cessAmount;
        }

        //}

        public class TaxSlab
        {
            // class for taxSlabs can be fetched and inserted from db
            public double minAmount { get; set; }
            public double maxAmount { get; set; }
            public double percentage { get; set; }

            public TaxSlab(double minAmount, double maxAmount, double percentage)
            {
                this.minAmount = minAmount;
                this.maxAmount = maxAmount;
                this.percentage = percentage;
            }
        }

        public class Exclusion
        {
            // class for tax Exclusions (HRA, etc) can be fetched and inserted from db	
            public String name { get; set; }
            public double maxAmount { get; set; }

            public Exclusion(String name, double maxAmount)
            {
                this.name = name;
                this.maxAmount = maxAmount;
            }
        }

        public class EmpExclusion
        {
            // class for tax Exclusions (HRA, etc) can be fetched and inserted from db	
            public double amount { get; set; }

            public EmpExclusion(double amount)
            {
                this.amount = amount;
            }
        }

        public class EmpOtherIncome
        {
            // class for tax Exclusions (HRA, etc) can be fetched and inserted from db	
            public double amount { get; set; }

            public EmpOtherIncome(double amount)
            {
                this.amount = amount;
            }
        }

        protected void btnCalculate_Click(object sender, EventArgs e)
        {
            try
            {


                if (txtempid.Text.Trim().Length == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showlalert", "alert('Please fill EmpId');", true);
                    return;
                }

                if (txtincome.Text.Trim().Length == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showlalert", "alert('Please fill Income Amount.');", true);
                    return;
                }

                //if (txtpan.Text.Trim().Length == 0)
                //{
                //    ScriptManager.RegisterStartupScript(this, GetType(), "showlalert", "alert('Please fill PAN No.');", true);
                //    return;
                //}

                if (txtemailid.Text.Trim().Length == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showlalert", "alert('Please fill Email ID');", true);
                    return;
                }

                if (txtfathername.Text.Trim().Length == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showlalert", "alert('Please fill Father's Name');", true);
                    return;
                }

                //if (txtaddress.Text.Trim().Length == 0)
                //{
                //    ScriptManager.RegisterStartupScript(this, GetType(), "showlalert", "alert('Please fill Address ');", true);
                //    return;
                //}

                //if (txtplace.Text.Trim().Length == 0)
                //{
                //    ScriptManager.RegisterStartupScript(this, GetType(), "showlalert", "alert('Please fill Place ');", true);
                //    return;
                //}

                if (txtdate.Text.Trim().Length == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showlalert", "alert('Please fill Date ');", true);
                    return;
                }

                if (txtOrganisation.Text.Trim().Length == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showlalert", "alert('Please fill Name of Organisation');", true);
                    return;
                }

                string DeleteData = "";
                int dtdelete = 0;

                DeleteData = "delete from TDS_EmpIncome where empid='" + txtempid.Text.Trim() + "' and FinancialYear='" + lblFyear.Text + "'";
                dtdelete = config.ExecuteNonQueryWithQueryAsync(DeleteData).Result;

                DeleteData = "delete from TDS_EmpOtherIncome where empid='" + txtempid.Text.Trim() + "' and FinancialYear='" + lblFyear.Text + "'";
                dtdelete = config.ExecuteNonQueryWithQueryAsync(DeleteData).Result;

                DeleteData = "delete from TDS_EmpExclusion where empid='" + txtempid.Text.Trim() + "' and FinancialYear='" + lblFyear.Text + "'";
                dtdelete = config.ExecuteNonQueryWithQueryAsync(DeleteData).Result;

                DeleteData = "delete from TDS_EmpApprovedExclusionAmounts where empid='" + txtempid.Text.Trim() + "' and FinancialYear='" + lblFyear.Text + "'";
                dtdelete = config.ExecuteNonQueryWithQueryAsync(DeleteData).Result;

                DeleteData = "delete from TDS_PrvEmpYearlyData where empid='" + txtempid.Text.Trim() + "' and FinancialYear='" + lblFyear.Text + "'";
                dtdelete = config.ExecuteNonQueryWithQueryAsync(DeleteData).Result;

                #region For Income and OtherIncome


                string TDS_EmpIncome = "Insert into TDS_EmpIncome(EmpId,FinancialYear,Amount) " +
                     " values ('" + txtempid.Text.Trim() + "','" + lblFyear.Text + "','" + txtincome.Text + "')";
                int dtTDS_EmpIncome = config.ExecuteNonQueryWithQueryAsync(TDS_EmpIncome).Result;


                if (txtotherIncome.Text.Trim() != "0")
                {
                    string TDS_EmpOtherIncome = "Insert into TDS_EmpOtherIncome(EmpId,FinancialYear,Amount,Remarks) " +
                   " values ('" + txtempid.Text.Trim() + "','" + lblFyear.Text + "','" + txtotherIncome.Text + "','" + txtOtherincomeRemarks.Text + "')";
                    int dtTDS_EmpOtherIncome = config.ExecuteNonQueryWithQueryAsync(TDS_EmpOtherIncome).Result;
                }



                #endregion

                if (txtFinalHRA.Text.Trim() != "0")
                {
                    string InsertHRANewExc = "Insert into TDS_EmpApprovedExclusionAmounts(EmpId,FinancialYear,TdsExclusionId,ApprovedAmount) " +
                      " values ('" + txtempid.Text.Trim() + "','" + lblFyear.Text + "','1','" + txtFinalHRA.Text + "')";
                    int DtInsertHRANewExc = config.ExecuteNonQueryWithQueryAsync(InsertHRANewExc).Result;
                }

                if (txtincometotal.Text.Trim() != "0")
                {
                    string InsertHRAIntrestloanExc = "Insert into TDS_EmpApprovedExclusionAmounts(EmpId,FinancialYear,TdsExclusionId,ApprovedAmount) " +
                     " values ('" + txtempid.Text.Trim() + "','" + lblFyear.Text + "','1','" + txtincometotal.Text + "')";
                    int DtInsertHRAIntrestloanExc = config.ExecuteNonQueryWithQueryAsync(InsertHRAIntrestloanExc).Result;
                }

                #region For HRA Amounts
                decimal FinalHRAAmount = 0;
                string HRATdsExclusionId = "0";
                decimal HRATdsExclusionMaxAmt = 0;


                for (int k = 0; k < GVHRA.Rows.Count; k++)
                {
                    TextBox txtRentpaid = GVHRA.Rows[k].FindControl("txtRentpaid") as TextBox;
                    TextBox txtrentProof = GVHRA.Rows[k].FindControl("txtrentProof") as TextBox;
                    TextBox txtLandlord = GVHRA.Rows[k].FindControl("txtLandlord") as TextBox;
                    TextBox txtPAN = GVHRA.Rows[k].FindControl("txtPAN") as TextBox;
                    TextBox txtAddressofLandlord = GVHRA.Rows[k].FindControl("txtAddressofLandlord") as TextBox;
                    if (txtRentpaid.Text.Trim() == "")
                    {
                        txtRentpaid.Text = "0";
                    }


                    string HRATDS_ExclusionMaster = "select * from TDS_ExclusionMaster where ExclusionName='HRA'";
                    DataTable dtHRATDS_ExclusionMaster = config.ExecuteAdaptorAsyncWithQueryParams(HRATDS_ExclusionMaster).Result;
                    if (dtHRATDS_ExclusionMaster.Rows.Count > 0)
                    {
                        HRATdsExclusionId = dtHRATDS_ExclusionMaster.Rows[0]["TdsExclusionId"].ToString();
                        HRATdsExclusionMaxAmt = Convert.ToDecimal(dtHRATDS_ExclusionMaster.Rows[0]["MaxAmount"].ToString());
                    }

                    if (txtRentpaid.Text != "0")
                    {
                        string InsertHRAQry = "Insert into TDS_EmpExclusion(EmpId,FinancialYear,TdsExclusionId,AmountUpdated,ProofLocation,PANNo,ParticularsofLandlord,AddressofLandlord) " +
                        " values ('" + txtempid.Text.Trim() + "','" + lblFyear.Text + "','" + HRATdsExclusionId + "','" + txtRentpaid.Text + "','" + txtrentProof.Text + "','" + txtPAN.Text + "','" + txtLandlord.Text + "','" + txtAddressofLandlord.Text + "')";
                        int DtInsertHRAQry = config.ExecuteNonQueryWithQueryAsync(InsertHRAQry).Result;

                        FinalHRAAmount += Convert.ToDecimal(txtRentpaid.Text.Trim());
                    }

                }

                if (FinalHRAAmount > 0)
                {
                    if (HRATdsExclusionMaxAmt > 0)
                    {
                        if (FinalHRAAmount > HRATdsExclusionMaxAmt)
                        {
                            FinalHRAAmount = HRATdsExclusionMaxAmt;
                        }
                    }
                    string InsertHRAApprovedQry = "Insert into TDS_EmpApprovedExclusionAmounts(EmpId,FinancialYear,TdsExclusionId,ApprovedAmount) " +
                       " values ('" + txtempid.Text.Trim() + "','" + lblFyear.Text + "','" + HRATdsExclusionId + "','" + FinalHRAAmount + "')";
                    int DtInsertHRAApprovedQry = config.ExecuteNonQueryWithQueryAsync(InsertHRAApprovedQry).Result;
                }
                #endregion

                #region For LTA Amounts
                decimal FinalLTAAmount = 0;
                string LTATdsExclusionId = "0";
                decimal LTATdsExclusionMaxAmt = 0;
                for (int k = 0; k < gvLTA.Rows.Count; k++)
                {
                    TextBox txtLTAAmount = gvLTA.Rows[k].FindControl("txtLTAAmount") as TextBox;
                    TextBox txtltaExpenses = gvLTA.Rows[k].FindControl("txtltaExpenses") as TextBox;
                    if (txtLTAAmount.Text.Trim() == "")
                    {
                        txtLTAAmount.Text = "0";
                    }


                    string LTATDS_ExclusionMaster = "select * from TDS_ExclusionMaster where ExclusionName='LTA'";
                    DataTable dtLTATDS_ExclusionMaster = config.ExecuteAdaptorAsyncWithQueryParams(LTATDS_ExclusionMaster).Result;
                    if (dtLTATDS_ExclusionMaster.Rows.Count > 0)
                    {
                        LTATdsExclusionId = dtLTATDS_ExclusionMaster.Rows[0]["TdsExclusionId"].ToString();
                        LTATdsExclusionMaxAmt = Convert.ToDecimal(dtLTATDS_ExclusionMaster.Rows[0]["MaxAmount"].ToString());

                    }


                    if (txtLTAAmount.Text != "0")
                    {
                        string InsertLTAQry = "Insert into TDS_EmpExclusion(EmpId,FinancialYear,TdsExclusionId,AmountUpdated,ProofLocation) " +
                        " values ('" + txtempid.Text.Trim() + "','" + lblFyear.Text + "','" + LTATdsExclusionId + "','" + txtLTAAmount.Text + "','" + txtltaExpenses.Text + "')";
                        int DtInsertLTAQry = config.ExecuteNonQueryWithQueryAsync(InsertLTAQry).Result;

                        FinalLTAAmount += Convert.ToDecimal(txtLTAAmount.Text.Trim());
                    }


                }

                if (FinalLTAAmount > 0)
                {
                    if (LTATdsExclusionMaxAmt > 0)
                    {
                        if (FinalLTAAmount > LTATdsExclusionMaxAmt)
                        {
                            FinalLTAAmount = LTATdsExclusionMaxAmt;
                        }
                    }
                    string InsertLTAApprovedQry = "Insert into TDS_EmpApprovedExclusionAmounts(EmpId,FinancialYear,TdsExclusionId,ApprovedAmount) " +
                       " values ('" + txtempid.Text.Trim() + "','" + lblFyear.Text + "','" + LTATdsExclusionId + "','" + FinalLTAAmount + "')";
                    int DtInsertLTAApprovedQry = config.ExecuteNonQueryWithQueryAsync(InsertLTAApprovedQry).Result;
                }


                #endregion

                #region For House Loan Ded
                decimal FinalHLDedAmount = 0;
                string HLDedTdsExclusionId = "0";
                decimal HLDedTdsExclusionMaxAmt = 0;


                string TDS_ExclusionMaster = "select * from TDS_ExclusionMaster where ExclusionName='HLoanDed'";
                DataTable dtTDS_ExclusionMaster = config.ExecuteAdaptorAsyncWithQueryParams(TDS_ExclusionMaster).Result;
                if (dtTDS_ExclusionMaster.Rows.Count > 0)
                {
                    HLDedTdsExclusionId = dtTDS_ExclusionMaster.Rows[0]["TdsExclusionId"].ToString();
                    HLDedTdsExclusionMaxAmt = Convert.ToDecimal(dtTDS_ExclusionMaster.Rows[0]["MaxAmount"].ToString());
                }

                if (txtinterestpayable.Text.Trim() == "")
                {
                    txtinterestpayable.Text = "0";
                }

                if (txtinterestpayable.Text.Trim() != "0")
                {

                    if (int.Parse(txtinterestpayable.Text) > 200000)
                    {
                        txtinterestpayable.Text = "200000";
                    }
                    string InsertHLDedQry = "Insert into TDS_EmpExclusion(EmpId,FinancialYear,TdsExclusionId,AmountUpdated,ProofLocation,HLDed_TypeofLender,HLDed_Name,HLDed_Address,HLDed_PAN) " +
                    " values ('" + txtempid.Text.Trim() + "','" + lblFyear.Text + "','" + HLDedTdsExclusionId + "','" + txtinterestpayable.Text + "','" + txtproofofinterest.Text + "','" + ddltypeofvendor.SelectedValue + "','" + txtLendername.Text + "','" + txtLenderaddress.Text + "','" + txtLenderpan.Text + "')";
                    int DtInsertHLDedQry = config.ExecuteNonQueryWithQueryAsync(InsertHLDedQry).Result;

                    FinalHLDedAmount += Convert.ToDecimal(txtinterestpayable.Text.Trim());
                }


                if (FinalHLDedAmount > 0)
                {
                    if (HLDedTdsExclusionMaxAmt > 0)
                    {
                        if (FinalHLDedAmount > HLDedTdsExclusionMaxAmt)
                        {
                            FinalHLDedAmount = HLDedTdsExclusionMaxAmt;
                        }
                    }
                    string InsertLTAApprovedQry = "Insert into TDS_EmpApprovedExclusionAmounts(EmpId,FinancialYear,TdsExclusionId,ApprovedAmount) " +
                       " values ('" + txtempid.Text.Trim() + "','" + lblFyear.Text + "','" + HLDedTdsExclusionId + "','" + FinalHLDedAmount + "')";
                    int DtInsertLTAApprovedQry = config.ExecuteNonQueryWithQueryAsync(InsertLTAApprovedQry).Result;
                }



                #endregion

                #region For 80C
                decimal Final80C = 0;
                string C80TdsExclusionId = "0";
                decimal C80TdsExclusionMaxAmt = 0;

                for (int k = 0; k < GvTaxdeds.Rows.Count; k++)
                {
                    DropDownList ddltaxDed = GvTaxdeds.Rows[k].FindControl("ddltaxDed") as DropDownList;
                    TextBox txttaxdedamount = GvTaxdeds.Rows[k].FindControl("txttaxdedamount") as TextBox;
                    TextBox txtltaExpenses = GvTaxdeds.Rows[k].FindControl("txtltaExpenses") as TextBox;

                    if (txttaxdedamount.Text.Trim() == "")
                    {
                        txttaxdedamount.Text = "0";
                    }


                    string C80TDS_ExclusionMaster = "select * from TDS_ExclusionMaster where ExclusionName='80C'";
                    DataTable dt80CTDS_ExclusionMaster = config.ExecuteAdaptorAsyncWithQueryParams(C80TDS_ExclusionMaster).Result;
                    if (dt80CTDS_ExclusionMaster.Rows.Count > 0)
                    {
                        C80TdsExclusionId = dt80CTDS_ExclusionMaster.Rows[0]["TdsExclusionId"].ToString();
                        C80TdsExclusionMaxAmt = Convert.ToDecimal(dt80CTDS_ExclusionMaster.Rows[0]["MaxAmount"].ToString());
                    }

                    if (txttaxdedamount.Text != "0")
                    {
                        string InsertC80Qry = "Insert into TDS_EmpExclusion(EmpId,FinancialYear,TdsExclusionId,AmountUpdated,ProofLocation,SectionsType) " +
                        " values ('" + txtempid.Text.Trim() + "','" + lblFyear.Text + "','" + C80TdsExclusionId + "','" + txttaxdedamount.Text + "','" + txtltaExpenses.Text + "','" + ddltaxDed.SelectedValue + "')";
                        int DtInsertC80Qry = config.ExecuteNonQueryWithQueryAsync(InsertC80Qry).Result;

                        Final80C += Convert.ToDecimal(txttaxdedamount.Text.Trim());
                    }
                }
                if (Final80C > 0)
                {
                    if (C80TdsExclusionMaxAmt > 0)
                    {
                        if (Final80C > C80TdsExclusionMaxAmt)
                        {
                            Final80C = C80TdsExclusionMaxAmt;
                        }
                    }
                    string InsertLTAApprovedQry = "Insert into TDS_EmpApprovedExclusionAmounts(EmpId,FinancialYear,TdsExclusionId,ApprovedAmount) " +
                       " values ('" + txtempid.Text.Trim() + "','" + lblFyear.Text + "','" + C80TdsExclusionId + "','" + Final80C + "')";
                    int DtInsertLTAApprovedQry = config.ExecuteNonQueryWithQueryAsync(InsertLTAApprovedQry).Result;
                }

                #endregion

                #region For 80CCC & 80CCD

                //80CCD
                decimal Final80CCCAmount = 0;
                string TdsExclusionId80CCC = "0";
                decimal TdsExclusionMaxAmt80CCC = 0;


                string TDS_ExclusionMaster80CCC = "select * from TDS_ExclusionMaster where ExclusionName='80CCC'";
                DataTable dtTDS_ExclusionMaster80CCC = config.ExecuteAdaptorAsyncWithQueryParams(TDS_ExclusionMaster80CCC).Result;
                if (dtTDS_ExclusionMaster80CCC.Rows.Count > 0)
                {
                    TdsExclusionId80CCC = dtTDS_ExclusionMaster80CCC.Rows[0]["TdsExclusionId"].ToString();
                    TdsExclusionMaxAmt80CCC = Convert.ToDecimal(dtTDS_ExclusionMaster80CCC.Rows[0]["MaxAmount"].ToString());
                }

                if (txt80CCAmt.Text.Trim() == "")
                {
                    txt80CCAmt.Text = "0";
                }

                if (txt80CCAmt.Text.Trim() != "0")
                {
                    string InsertHLDedQry = "Insert into TDS_EmpExclusion(EmpId,FinancialYear,TdsExclusionId,AmountUpdated,ProofLocation) " +
                    " values ('" + txtempid.Text.Trim() + "','" + lblFyear.Text + "','" + TdsExclusionId80CCC + "','" + txt80CCAmt.Text + "','" + txt80CCproof.Text + "')";
                    int DtInsertHLDedQry = config.ExecuteNonQueryWithQueryAsync(InsertHLDedQry).Result;

                    Final80CCCAmount += Convert.ToDecimal(txt80CCAmt.Text.Trim());
                }


                if (Final80CCCAmount > 0)
                {
                    if (TdsExclusionMaxAmt80CCC > 0)
                    {
                        if (Final80CCCAmount > TdsExclusionMaxAmt80CCC)
                        {
                            Final80CCCAmount = TdsExclusionMaxAmt80CCC;
                        }
                    }


                    string Insert80cApprovedQry = "Insert into TDS_EmpApprovedExclusionAmounts(EmpId,FinancialYear,TdsExclusionId,ApprovedAmount) " +
                       " values ('" + txtempid.Text.Trim() + "','" + lblFyear.Text + "','" + TdsExclusionId80CCC + "','" + Final80CCCAmount + "')";
                    int DtInsert80CApprovedQry = config.ExecuteNonQueryWithQueryAsync(Insert80cApprovedQry).Result;
                }


                //80CCD
                decimal Final80CCDAmount = 0;
                string TdsExclusionId80CCD = "0";
                decimal TdsExclusionMaxAmt80CCD = 0;


                string TDS_ExclusionMaster80CCD = "select * from TDS_ExclusionMaster where ExclusionName='80CCD'";
                DataTable dtTDS_ExclusionMaster80CCD = config.ExecuteAdaptorAsyncWithQueryParams(TDS_ExclusionMaster80CCD).Result;
                if (dtTDS_ExclusionMaster80CCD.Rows.Count > 0)
                {
                    TdsExclusionId80CCD = dtTDS_ExclusionMaster80CCD.Rows[0]["TdsExclusionId"].ToString();
                    TdsExclusionMaxAmt80CCD = Convert.ToDecimal(dtTDS_ExclusionMaster80CCD.Rows[0]["MaxAmount"].ToString());
                }

                if (txt80CCDAmt.Text.Trim() == "")
                {
                    txt80CCDAmt.Text = "0";
                }

                if (txt80CCDAmt.Text.Trim() != "0")
                {
                    string InsertHLDedQry = "Insert into TDS_EmpExclusion(EmpId,FinancialYear,TdsExclusionId,AmountUpdated,ProofLocation) " +
                    " values ('" + txtempid.Text.Trim() + "','" + lblFyear.Text + "','" + TdsExclusionId80CCD + "','" + txt80CCDAmt.Text + "','" + txt80CCDproof.Text + "')";
                    int DtInsertHLDedQry = config.ExecuteNonQueryWithQueryAsync(InsertHLDedQry).Result;

                    Final80CCDAmount += Convert.ToDecimal(txt80CCDAmt.Text.Trim());
                }


                if (Final80CCDAmount > 0)
                {
                    if (TdsExclusionMaxAmt80CCD > 0)
                    {
                        if (Final80CCDAmount > TdsExclusionMaxAmt80CCD)
                        {
                            Final80CCDAmount = TdsExclusionMaxAmt80CCD;
                        }
                    }


                    string Insert80cApprovedQry = "Insert into TDS_EmpApprovedExclusionAmounts(EmpId,FinancialYear,TdsExclusionId,ApprovedAmount) " +
                       " values ('" + txtempid.Text.Trim() + "','" + lblFyear.Text + "','" + TdsExclusionId80CCD + "','" + Final80CCDAmount + "')";
                    int DtInsert80CApprovedQry = config.ExecuteNonQueryWithQueryAsync(Insert80cApprovedQry).Result;
                }



                #endregion

                #region For 80C
                decimal FinalOtherSec = 0;
                string OtherSecTdsExclusionId = "0";
                decimal OtherSecTdsExclusionMaxAmt = 0;

                for (int k = 0; k < GVOtherSections.Rows.Count; k++)
                {
                    DropDownList ddlsection = GVOtherSections.Rows[k].FindControl("ddlsection") as DropDownList;
                    TextBox txtothersecamount = GVOtherSections.Rows[k].FindControl("txtothersecamount") as TextBox;
                    TextBox txtothersProof = GVOtherSections.Rows[k].FindControl("txtothersProof") as TextBox;

                    if (txtothersecamount.Text.Trim() == "")
                    {
                        txtothersecamount.Text = "0";
                    }


                    string C80TDS_ExclusionMaster = "select * from TDS_ExclusionMaster where ExclusionName='" + ddlsection.SelectedValue + "'";
                    DataTable dt80CTDS_ExclusionMaster = config.ExecuteAdaptorAsyncWithQueryParams(C80TDS_ExclusionMaster).Result;
                    if (dt80CTDS_ExclusionMaster.Rows.Count > 0)
                    {
                        OtherSecTdsExclusionId = dt80CTDS_ExclusionMaster.Rows[0]["TdsExclusionId"].ToString();
                        OtherSecTdsExclusionMaxAmt = Convert.ToDecimal(dt80CTDS_ExclusionMaster.Rows[0]["MaxAmount"].ToString());
                    }

                    if (txtothersecamount.Text != "0")
                    {
                        string InsertCQry = "Insert into TDS_EmpExclusion(EmpId,FinancialYear,TdsExclusionId,AmountUpdated,ProofLocation,SectionsType) " +
                        " values ('" + txtempid.Text.Trim() + "','" + lblFyear.Text + "','" + OtherSecTdsExclusionId + "','" + txtothersecamount.Text + "','" + txtothersProof.Text + "','" + ddlsection.SelectedValue + "')";
                        int DtInsertC80Qry = config.ExecuteNonQueryWithQueryAsync(InsertCQry).Result;

                        FinalOtherSec = Convert.ToDecimal(txtothersecamount.Text.Trim());

                        if (FinalOtherSec > 0)
                        {
                            if (OtherSecTdsExclusionMaxAmt > 0)
                            {
                                if (FinalOtherSec > OtherSecTdsExclusionMaxAmt)
                                {
                                    FinalOtherSec = OtherSecTdsExclusionMaxAmt;
                                }
                            }


                            string InsertSecApprovedQry = "Insert into TDS_EmpApprovedExclusionAmounts(EmpId,FinancialYear,TdsExclusionId,ApprovedAmount) " +
                               " values ('" + txtempid.Text.Trim() + "','" + lblFyear.Text + "','" + OtherSecTdsExclusionId + "','" + FinalOtherSec + "')";
                            int DtInsertsecApprovedQry = config.ExecuteNonQueryWithQueryAsync(InsertSecApprovedQry).Result;
                        }
                    }
                }


                #endregion

                Main(txtempid.Text.Trim(), lblFyear.Text);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                Response.Write("Error Message : " + ex.Message);
            }
        }

        protected void GetEmpName()
        {
            string Sqlqry = "select (empfname+' '+empmname+' '+emplname) as empname from empdetails where empid='" + txtempid.Text + "' ";
            DataTable dt = config.ExecuteAdaptorAsyncWithQueryParams(Sqlqry).Result;
            if (dt.Rows.Count > 0)
            {
                try
                {
                    txtname.Text = dt.Rows[0]["empname"].ToString();

                }
                catch (Exception ex)
                {
                }
            }
            else
            {
            }


        }

        protected void txtFname_TextChanged(object sender, EventArgs e)
        {
            Getempid();
            ChkEmpID();
            GetData();
        }

        public void ChkEmpID()
        {
            string QryCheck = "select * from TDS_EmpYearlyData where empid='" + txtempid.Text.Trim() + "' and FinancialYear='" + lblFyear.Text + "'";
            DataTable dtCheck = config.ExecuteAdaptorAsyncWithQueryParams(QryCheck).Result;
            if (dtCheck.Rows.Count > 0)
            {
                txtempid.Text = "";
                txtname.Text = "";
                return;
            }
            else
            {
                string DeleteData = "";
                int dtdelete = 0;

                DeleteData = "delete from TDS_EmpIncome where empid='" + txtempid.Text.Trim() + "' and FinancialYear='" + lblFyear.Text + "'";
                dtdelete = config.ExecuteNonQueryWithQueryAsync(DeleteData).Result;

                DeleteData = "delete from TDS_EmpOtherIncome where empid='" + txtempid.Text.Trim() + "' and FinancialYear='" + lblFyear.Text + "'";
                dtdelete = config.ExecuteNonQueryWithQueryAsync(DeleteData).Result;

                DeleteData = "delete from TDS_EmpExclusion where empid='" + txtempid.Text.Trim() + "' and FinancialYear='" + lblFyear.Text + "'";
                dtdelete = config.ExecuteNonQueryWithQueryAsync(DeleteData).Result;

                DeleteData = "delete from TDS_EmpApprovedExclusionAmounts where empid='" + txtempid.Text.Trim() + "' and FinancialYear='" + lblFyear.Text + "'";
                dtdelete = config.ExecuteNonQueryWithQueryAsync(DeleteData).Result;

                DeleteData = "delete from TDS_PrvEmpYearlyData where empid='" + txtempid.Text.Trim() + "' and FinancialYear='" + lblFyear.Text + "'";
                dtdelete = config.ExecuteNonQueryWithQueryAsync(DeleteData).Result;
            }

        }

        protected void txtemplyid_TextChanged(object sender, EventArgs e)
        {
            GetEmpName();
            ChkEmpID();
            GetData();
        }

        protected void Getempid()
        {

            string Sqlqry = "select  empid from empdetails where empfname+' '+empmname+' '+emplname like '%" + txtname.Text + "%' ";
            DataTable dt = config.ExecuteAdaptorAsyncWithQueryParams(Sqlqry).Result;
            if (dt.Rows.Count > 0)
            {
                try
                {
                    txtempid.Text = dt.Rows[0]["empid"].ToString();

                }
                catch (Exception ex)
                {
                }
            }
            else
            {
            }

        }

        public void GetData()
        {
            string query = "select ed.EmpId,epd.PanCardNo,ed.EmailId,EmpPhone,EmpFatherName,prLmark,S.State,D.Design,convert(varchar(10),getdate(),103) Date,'ABC Company' companyname, case convert(nvarchar(20),EmpDtofBirth,103) when '01/01/1900' then '' else convert(nvarchar(20),EmpDtofBirth,103) end DOB,case when CONVERT(varchar(10),EmpDtofBirth,103)='01/01/1900' then  '' else DATEDIFF(hour, EmpDtofBirth, cast(GETDATE() as date))/8766 end Age from EmpDetails ed left join EmpProofDetails epd on epd.EmpId = ed.EmpId " +
                            "  inner join Designations D on D.DesignId = ed.EmpDesgn left join States S on S.StateID = ed.prState where ed.EmpId = '" + txtempid.Text.Trim() + "'";
            DataTable dt = config.ExecuteAdaptorAsyncWithQueryParams(query).Result;
            if (dt.Rows.Count > 0)
            {
                txtpan.Text = dt.Rows[0]["PanCardNo"].ToString();
                txtemailid.Text = dt.Rows[0]["EmailId"].ToString();
                txtDOB.Text = dt.Rows[0]["DOB"].ToString();
                if (dt.Rows[0]["Age"].ToString().Trim() == "0")
                {
                    txtage.Text = "";
                }
                else
                {
                    txtage.Text = dt.Rows[0]["Age"].ToString();
                }

                txtmobile.Text = dt.Rows[0]["EmpPhone"].ToString();
                txtfathername.Text = dt.Rows[0]["EmpFatherName"].ToString();
                txtaddress.Text = dt.Rows[0]["prLmark"].ToString();
                txtplace.Text = dt.Rows[0]["State"].ToString();
                txtdate.Text = dt.Rows[0]["Date"].ToString();
                txtdesignation.Text = dt.Rows[0]["Design"].ToString();
                txtOrganisation.Text = dt.Rows[0]["companyname"].ToString();
                txtemailOrganisation.Text = dt.Rows[0]["EmailId"].ToString();

                string SalQry = "select top(1) Basic*12 Basic,HRA*12 HRA,(Basic+DA+HRA+OtherAllowance)*12 TotalSalary from EmpSalaryStructure where EmpId='" + txtempid.Text + "' order by ID desc";
                DataTable Saldt = config.ExecuteAdaptorAsyncWithQueryParams(SalQry).Result;
                if (dt.Rows.Count > 0)
                {
                    txtincome.Text = Saldt.Rows[0]["TotalSalary"].ToString();
                    txtbasic.Text = Saldt.Rows[0]["Basic"].ToString();
                    txtHra.Text = Saldt.Rows[0]["HRA"].ToString();
                }
            }
            else
            {

            }
        }

        protected void btnTDSSave_Click(object sender, EventArgs e)
        {
            string TDSEmpiD = "";
            string TDSFY = "";

            TDSEmpiD = lblPreEmpid.Text.Trim();
            TDSFY = lblPreFinancialYear.Text.Trim();


            string insertQry = "";
            int DtInsert = 0;

            if (RdboldScheme.Checked == true)
            {
                insertQry = "insert into TDS_EmpYearlyData (EmpId,FinancialYear,YearlyAmount,MonthlyAmount,scheme) " +
               " select EmpId,FinancialYear,OldYearlyAmount,OldMonthlyAmount,'OldScheme' from TDS_PrvEmpYearlyData where empid='" + TDSEmpiD + "' and FinancialYear='" + TDSFY + "'";
            }
            else
            {
                insertQry = "insert into TDS_EmpYearlyData (EmpId,FinancialYear,YearlyAmount,MonthlyAmount,scheme) " +
              " select EmpId,FinancialYear,NewYearlyAmount,NewMonthlyAmount,'NewScheme' from TDS_PrvEmpYearlyData where empid='" + TDSEmpiD + "' and FinancialYear='" + TDSFY + "'";
            }
            DtInsert = config.ExecuteNonQueryWithQueryAsync(insertQry).Result;
            if (DtInsert > 0)
            {
                txtempid.Text = "";
                txtincome.Text = "";
                txtname.Text = "";
                txtpan.Text = "";
                txtemailid.Text = "";
                txtmobile.Text = "";
                txtfathername.Text = "";
                txtaddress.Text = "";
                txtplace.Text = "";
                txtdate.Text = "";
                txtdesignation.Text = "";
                txtOrganisation.Text = "";
                txtemailOrganisation.Text = "";
                txtotherIncome.Text = "0";
                txtOtherincomeRemarks.Text = "";
                txtDOB.Text = "";
                txtage.Text = "";
                txtbasic.Text = "0";
                txtHra.Text = "0";
                txtRentPaid.Text = "0";

                DataTable dtDummyTable = new DataTable();
                dtDummyTable.Columns.Add(new DataColumn("IndexCol"));
                for (int j = 0; j < 2; j++)
                    dtDummyTable.Rows.Add(j + 1);

                gvLTA.DataSource = dtDummyTable;

                gvLTA.DataBind();
                DisplayDefaultRow();
                Enable5Rows();


                HRAdisplaydata();
                HRADisplayDefaultRow();
                HRAEnable5Rows();

                Taxdisplaydata();
                TaxDisplayDefaultRow();
                TaxEnable5Rows();

                Othersecdisplaydata();
                OthersecDisplayDefaultRow();
                OthersecEnable5Rows();
                ModalPopupExtender1.Hide();
                ScriptManager.RegisterStartupScript(this, GetType(), "showlalert", "alert('Details Added successfully');", true);
                return;
            }

        }

        protected void lnkBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("Employees.aspx");
        }

        protected void btnTDSClose_Click(object sender, EventArgs e)
        {
            ModalPopupExtender1.Hide();
        }

        protected void btnHRACalculate_Click(object sender, EventArgs e)
        {

            double RentPaid = 0;
            double MCDCagainst = 0;


            double ActaulHRA = 0;

            ActaulHRA = double.Parse(txtHra.Text);

            RentPaid = double.Parse(txtRentPaid.Text) - (double.Parse(txtbasic.Text) * 10 / 100);

            if (ddlplaceofstay.SelectedIndex == 1)
            {
                MCDCagainst = (double.Parse(txtbasic.Text) * 50 / 100);
            }
            else if (ddlplaceofstay.SelectedIndex == 2)
            {
                MCDCagainst = (double.Parse(txtbasic.Text) * 40 / 100);
            }

            double[] arr = new double[3] { ActaulHRA, RentPaid, MCDCagainst };

            if (arr.Min() > 0)
            {
                txtFinalHRA.Text = arr.Min().ToString();
            }
            else
            {
                txtFinalHRA.Text = "0";
            }


        }

        protected void btninvcalculate_Click(object sender, EventArgs e)
        {
            double TotalAmount = 0;
            for (int i = 0; i < gvIncomeHouse.Rows.Count; i++)
            {
                TextBox txtgrossannualreport = gvIncomeHouse.Rows[i].FindControl("txtgrossannualreport") as TextBox;
                TextBox txtMunicipaltax = gvIncomeHouse.Rows[i].FindControl("txtMunicipaltax") as TextBox;
                TextBox txtnetannaul = gvIncomeHouse.Rows[i].FindControl("txtnetannaul") as TextBox;
                TextBox txtlessstd = gvIncomeHouse.Rows[i].FindControl("txtlessstd") as TextBox;
                TextBox txtInterest = gvIncomeHouse.Rows[i].FindControl("txtInterest") as TextBox;
                TextBox txtincome = gvIncomeHouse.Rows[i].FindControl("txtincome") as TextBox;

                if (txtgrossannualreport.Text.Trim() == "")
                {
                    txtgrossannualreport.Text = "0";
                }
                if (txtMunicipaltax.Text.Trim() == "")
                {
                    txtMunicipaltax.Text = "0";
                }
                if (txtnetannaul.Text.Trim() == "")
                {
                    txtnetannaul.Text = "0";
                }
                if (txtlessstd.Text.Trim() == "")
                {
                    txtlessstd.Text = "0";
                }
                if (txtInterest.Text.Trim() == "")
                {
                    txtInterest.Text = "0";
                }
                if (txtInterest.Text.Trim() == "")
                {
                    txtincome.Text = "0";
                }

                txtnetannaul.Text = (double.Parse(txtgrossannualreport.Text) - double.Parse(txtMunicipaltax.Text)).ToString();
                if (double.Parse(txtnetannaul.Text) < 0)
                {
                    txtnetannaul.Text = "0";
                }

                txtlessstd.Text = (double.Parse(txtnetannaul.Text) * 30 / 100).ToString();
                if (double.Parse(txtlessstd.Text) < 0)
                {
                    txtlessstd.Text = "0";
                }

                txtincome.Text = (double.Parse(txtnetannaul.Text) - double.Parse(txtlessstd.Text) - double.Parse(txtInterest.Text)).ToString();
                //if (double.Parse(txtincome.Text) < 0)
                //{
                //    txtincome.Text = "0";
                //}


                TotalAmount += (double.Parse(txtincome.Text));
            }

            TotalAmount = TotalAmount - (2 * TotalAmount);


            if (TotalAmount > 200000)
            {
                txtincometotal.Text = "200000";
            }
            else
            {
                txtincometotal.Text = TotalAmount.ToString();
            }

            if (double.Parse(txtincometotal.Text) < 0)
            {
                txtincometotal.Text = "0";
            }


        }
    }
}