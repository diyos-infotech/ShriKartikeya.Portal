using KLTS.Data;
using ShriKartikeya.Portal.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ShriKartikeya.Portal.Module_Reports
{
    public partial class PFZeroReport : System.Web.UI.Page
    {
        AppConfiguration config = new AppConfiguration();
        GridViewExportUtil gve = new GridViewExportUtil();
        string EmpIDPrefix = "";
        string CmpIDPrefix = "";
        string BranchID = "";
        //code changes done by dhanalakshmi on 15-06-2022 ref:009307
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
                        HtmlControl emplink = (HtmlControl)this.Master.Master.FindControl("ContentPlaceHolder1").FindControl("sli1");
                        if (emplink != null)
                        {
                            emplink.Attributes["class"] = "current";
                        }
                    }
                    //LoadStatenames();
                }
            }
            catch (Exception ex)
            {

            }

        }

        protected void GetWebConfigdata()
        {
            BranchID = Session["BranchID"].ToString();
            EmpIDPrefix = Session["EmpIDPrefix"].ToString();
            CmpIDPrefix = Session["CmpIDPrefix"].ToString();
        }

        

        protected void btnsearch_Click(object sender, EventArgs e)
        {
            LblResult.Text = "";
            GVListEmployees.DataSource = null;
            GVListEmployees.DataBind();



            if (txtmonth.Text.Trim().Length == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showlalert", "alert('Please Select Month');", true);

                return;
            }
            string date = string.Empty;

            if (txtmonth.Text.Trim().Length > 0)
            {
                date = DateTime.Parse(txtmonth.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
            }

            string month = DateTime.Parse(date).Month.ToString();
            string Year = DateTime.Parse(date).Year.ToString();
            string sqlqry = string.Empty;

            var SPName = "";
            Hashtable HTPaysheet = new Hashtable();
            SPName = "PFZeroReport";

            


            var Type = 0;

            
            DataTable dtBranch = GlobalData.Instance.LoadBranchOnUserID(BranchID);
            HTPaysheet.Add("@month", month + Year.Substring(2, 2));
            //HTPaysheet.Add("@PTState", PTState);
            //HTPaysheet.Add("@Branch", dtBranch);
            //HTPaysheet.Add("@type", Type);

            DataTable dt = config.ExecuteAdaptorAsyncWithParams(SPName, HTPaysheet).Result;

            if (dt.Rows.Count > 0)
            {
               
                    GVListEmployees.DataSource = dt;
                    GVListEmployees.DataBind();
                    lbtn_Export.Visible = true;
                    lbtn_Export_Text.Visible = true;



            }
            else
            {
                GVListEmployees.DataSource = null;
                GVListEmployees.DataBind();
               
                ScriptManager.RegisterStartupScript(this, GetType(), "showlalert", "alert('No records found');", true);

            }

        }

        protected void ClearData()
        {
            LblResult.Text = "";
            GVListEmployees.DataSource = null;
            GVListEmployees.DataBind();           
            lbtn_Export.Visible = false;
        }

        protected void lbtn_Export_Click(object sender, EventArgs e)
        {
           
                gve.Export("PTReport.xls", this.GVListEmployees);
            
            
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
            return month;



            #endregion
        }

        protected void lbtn_Export_Text_Click(object sender, EventArgs e)
        {

            

            if (txtmonth.Text.Trim().Length == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showlalert", "alert('Please Select Month');", true);

                return;
            }
            string date = string.Empty;

            if (txtmonth.Text.Trim().Length > 0)
            {
                date = DateTime.Parse(txtmonth.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
            }

            string month = DateTime.Parse(date).Month.ToString();
            string Year = DateTime.Parse(date).Year.ToString();
            string sqlqry = string.Empty;
            //for (int index = 0; index < GVListOfClients.Columns.Count; index++)
            //{
            //    sBuilder.Append(GVListOfClients.Columns[index].HeaderText + ',');
            //}
            //sBuilder.Append("\r\n");
            DataTable DtListOfEmployees = new DataTable();
            Hashtable HtsearchEmp = new Hashtable();
            string sp = "";
            
            sp = "PFZeroText";
            HtsearchEmp.Add("@month", month + Year.Substring(2, 2));

            DtListOfEmployees = config.ExecuteAdaptorAsyncWithParams(sp, HtsearchEmp).Result;
        
            if (DtListOfEmployees.Rows.Count > 0)
            {
                GVListOfClients.Visible = false;
                GVListOfClients.DataSource = DtListOfEmployees;
                GVListOfClients.DataBind();
            }
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=PFZERO.txt");
            Response.Charset = "";
            Response.ContentType = "application/text";
            StringBuilder sBuilder = new System.Text.StringBuilder();
            string txtFile = "";
            foreach (GridViewRow row in GVListOfClients.Rows)
            {
                foreach (TableCell cell in row.Cells)
                {
                    txtFile += cell.Text;
                }
                txtFile += "\r\n";
            }
            Response.Output.Write(txtFile.ToString());
            Response.Flush();
            Response.End();
        }

      





    }
}