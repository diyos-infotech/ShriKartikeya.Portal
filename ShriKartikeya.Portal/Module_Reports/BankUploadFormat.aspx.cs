using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using KLTS.Data;
using System.Data;
using System.Collections;
using System.Globalization;
using ShriKartikeya.Portal.DAL;
namespace ShriKartikeya.Portal
{
    public partial class BankUploadFormat : System.Web.UI.Page
    {
        //DataTable dt;
        string EmpIDPrefix = "";
        string CmpIDPrefix = "";
        string BranchID = "";
        string Accountno = "";
        AppConfiguration config = new AppConfiguration();
        GridViewExportUtil gve = new GridViewExportUtil();


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
                    //  FillClientList();
                    // FillClientNameList();
                    GetBankNames();
                }

            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Show alert", "alert('Your Session Expired');", true);
                Response.Redirect("~/Login.aspx");
            }
        }

        public void GetEmpDetail()
        {

        }

        protected void GetWebConfigdata()
        {
            CmpIDPrefix = Session["CmpIDPrefix"].ToString();
            BranchID = Session["BranchID"].ToString();

        }

        public string GetMonth()
        {
            string month = "";
            string year = "";
            string DateVal = "";
            DateTime date;


            if (txtmonth.Text != "")
            {
                date = DateTime.ParseExact(txtmonth.Text, "MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                month = DateTime.Parse(txtmonth.Text.Trim(), CultureInfo.GetCultureInfo("en-GB")).Month.ToString();
                year = DateTime.Parse(txtmonth.Text.Trim(), CultureInfo.GetCultureInfo("en-GB")).Year.ToString();

            }

            DateVal = month + year.Substring(2, 2);
            return DateVal;

        }

        public void GetClientsData()
        {
            string date = string.Empty;

            GVListEmployees.DataSource = null;
            GVListEmployees.DataBind();

            GVListClients.DataSource = null;
            GVListClients.DataBind();

            if (txtmonth.Text.Trim().Length == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "show alert", "alert('Please Select The Month');", true);
                return;
            }

            if (txtmonth.Text.Trim().Length > 0)
            {
                date = DateTime.Parse(txtmonth.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
            }

            int cycle = 0;
            cycle = ddlcycles.SelectedIndex;
            string Month = GetMonth();

            string query = "select distinct(eps.clientid),c.ClientName from emppaysheet eps inner join Clients c on c.ClientId = eps.ClientId inner join Contracts cS on cS.ClientId = eps.ClientId where cS.PaySheetDates='" + cycle + "' and  month ='" + Month + "' and eps.clientid like '%" + CmpIDPrefix + "%'";
            DataTable dtClients = config.ExecuteAdaptorAsyncWithQueryParams(query).Result;
            if (dtClients.Rows.Count > 0)
            {
                GVListEmployees.DataSource = dtClients;
                GVListEmployees.DataBind();
            }
            else
            {
                GVListEmployees.DataSource = null;
                GVListEmployees.DataBind();

            }


            lbtn_Export.Visible = true;
        }

        public void GetBankNames()
        {
            ddlOptions.DataSource = null;
            ddlOptions.DataBind();

            string query = "select BankId,Bankname from BankNames where bankid>0";
            DataTable dtbanks = config.ExecuteAdaptorAsyncWithQueryParams(query).Result;
            if (dtbanks.Rows.Count > 0)
            {

                ddlOptions.DataSource = dtbanks;
                ddlOptions.DataBind();
                ddlOptions.DataTextField = "Bankname";
                ddlOptions.DataValueField = "BankId";
                ddlOptions.DataBind();
            }
               ddlOptions.Items.Insert(0, "--Select--");
            
         
        }
        protected void ClearData()
        {
            GVListEmployees.DataSource = null;
            GVListEmployees.DataBind();
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
            //return month;

            return month;

            #endregion
        }

        protected int GetMonth(string NameOfmonth)
        {
            int month = -1;
            var formatInfoinfo = new DateTimeFormatInfo();
            string[] monthName = formatInfoinfo.MonthNames;
            for (int i = 0; i < monthName.Length; i++)
            {
                if (monthName[i].CompareTo(NameOfmonth) == 0)
                {
                    month = i + 1;
                    break;
                }
            }
            return month;
        }

        public string GetMonthName()
        {
            string monthname = string.Empty;
            int payMonth = 0;
            DateTimeFormatInfo mfi = new DateTimeFormatInfo();



            DateTime date = Convert.ToDateTime(txtmonth.Text, CultureInfo.GetCultureInfo("en-gb"));
            monthname = mfi.GetMonthName(date.Month).ToString();
            //payMonth = GetMonth(monthname);

            return monthname;
        }

        protected void lbtn_Export_Click(object sender, EventArgs e)
        {
            DataTable DtListOfEmployees = new DataTable();

            string Month = GetMonth();
            int option = ddlOptions.SelectedIndex;
            var list = new List<string>();

            GVListClients.DataSource = null;
            GVListClients.DataBind();

            if (GVListEmployees.Rows.Count > 0)
            {
                string strQry = "Select * from CompanyInfo ";
                DataTable compInfo = config.ExecuteAdaptorAsyncWithQueryParams(strQry).Result;
                string companyName = "Your Company Name";

                if (compInfo.Rows.Count > 0)
                {
                    companyName = compInfo.Rows[0]["CompanyName"].ToString();
                }


                for (int i = 0; i < GVListEmployees.Rows.Count; i++)
                {
                    CheckBox chkclientid = GVListEmployees.Rows[i].FindControl("chkindividual") as CheckBox;
                    if (chkclientid.Checked == true)
                    {
                        Label lblclientid = GVListEmployees.Rows[i].FindControl("lblclientid") as Label;
                        Label lblclientname = GVListEmployees.Rows[i].FindControl("lblclientname") as Label;

                        if (chkclientid.Checked == true)
                        {
                            list.Add(lblclientid.Text);
                        }
                    }
                }

                string Clientids = string.Join(",", list.ToArray());

                DataTable dtClientList = new DataTable();
                dtClientList.Columns.Add("Clientid");
                if (list.Count != 0)
                {
                    foreach (string s in list)
                    {
                        DataRow row = dtClientList.NewRow();
                        row["Clientid"] = s;
                        dtClientList.Rows.Add(row);
                    }
                }

                Hashtable HtsearchEmp = new Hashtable();
                string sp = "";

                string date = string.Empty;
                sp = "BankUpLoadFormate";
                HtsearchEmp.Add("@month", Month);
                HtsearchEmp.Add("@ClientId", dtClientList);
                HtsearchEmp.Add("@option", option);

                DtListOfEmployees = config.ExecuteAdaptorAsyncWithParams(sp, HtsearchEmp).Result;
                if (DtListOfEmployees.Rows.Count > 0)
                {
                    GVListClients.Visible = false;
                    GVListClients.DataSource = DtListOfEmployees;
                    GVListClients.DataBind();
                    int count = DtListOfEmployees.Columns.Count;

                    //if (ddlOptions.SelectedIndex == 1 || ddlOptions.SelectedIndex == 2 || ddlOptions.SelectedIndex == 3)
                    //{
                    //    string line = "NAME OF A/C HOLDER : " + companyName;
                    //    string line1 = DateTime.Now.ToString("dd/MM/yyyy");
                    //    gve.ExporttoExcelForBankUpload("BankUploadFormat.xls", this.GVListClients, line, line1, count);
                    //}

                    if (ddlOptions.SelectedIndex == 1)
                    {

                        gve.Export("BankUploadFormat.xls", this.GVListClients);

                    }
                    if (ddlOptions.SelectedIndex == 2 )
                    {
                        string strcompanyName = compInfo.Rows[0]["CompanyName"].ToString();
                        string line = strcompanyName;
                        Label lblclientname = GVListEmployees.Rows[0].FindControl("lblclientname") as Label;
                        string line1 = "Salary Register For The Month of " +"-"+ txtmonth.Text+"-"+ lblclientname.Text; 
                        gve.ExporttoExcelForBankUploadbank("BankUploadFormat.xls", this.GVListClients, line, line1, count);
                    }
                   
                }
            }
        }

        protected void GVListClients_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[0].Text == "99999")
                {
                    e.Row.Cells[0].Text = "";
                    e.Row.Font.Bold = true;
                }
                if (ddlOptions.SelectedIndex == 1)
                {
                    e.Row.Cells[3].Attributes.Add("class", "text");
                    e.Row.Cells[4].Attributes.Add("class", "text");
                    e.Row.Cells[5].Attributes.Add("class", "text");
                    e.Row.Cells[6].Attributes.Add("class", "text");
                    e.Row.Cells[13].Attributes.Add("class", "text");
                    e.Row.Cells[15].Attributes.Add("class", "text");
                    e.Row.Cells[14].Attributes.Add("class", "text");
                }
                else
                {
                    e.Row.Cells[3].Attributes.Add("class", "text");
                    e.Row.Cells[4].Attributes.Add("class", "text");
                }

                if (ddlOptions.SelectedIndex == 3 )
                {
                    e.Row.Cells[6].Attributes.Add("class", "text");
                }



            }
        }

        protected void txtmonth_TextChanged(object sender, EventArgs e)
        {
            GetClientsData();
        }

    }
}