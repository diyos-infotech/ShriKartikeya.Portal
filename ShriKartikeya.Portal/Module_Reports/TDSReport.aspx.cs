using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using KLTS.Data;
using System.Globalization;
using System.Collections;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using OfficeOpenXml;
using ShriKartikeya.Portal.DAL;

namespace ShriKartikeya.Portal
{
    public partial class TDSReport : System.Web.UI.Page
    {
        AppConfiguration Config = new AppConfiguration();
        GridViewExportUtil GVUtill = new GridViewExportUtil();
        DataTable dt;
        string EmpIDPrefix = ""; string Branch = "";
        string CmpIDPrefix = "";
        string Elength = "";
        string Clength = "";
        string Fontstyle = "";
        string CFontstyle = "";
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
            Branch = Session["BranchID"].ToString(); EmpIDPrefix = Session["EmpIDPrefix"].ToString();
            CmpIDPrefix = Session["CmpIDPrefix"].ToString();
            BranchID = Session["BranchID"].ToString();
        }

        protected void lbtn_Export_Click(object sender, EventArgs e)
        {
            GVUtill.Export("TDSREPORT.xls", this.GVListEmployees);
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            try
            {
                string date = string.Empty;
                if (txtmonth.Text.Trim().Length == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "Show alert", "alert('Please select Month');", true);
                    return;
                }

                if (txtmonth.Text.Trim().Length > 0)
                {
                    date = DateTime.Parse(txtmonth.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
                }

                string month = DateTime.Parse(date).Month.ToString();
                string Year = DateTime.Parse(date).Year.ToString();
                string sqlqry = string.Empty;
                var SPName = "";
                Hashtable HTPaysheet = new Hashtable();
                SPName = "TDSReport";

                HTPaysheet.Add("@month", month + Year.Substring(2, 2));
                HTPaysheet.Add("@branch", BranchID);
                dt = Config.ExecuteAdaptorAsyncWithParams(SPName, HTPaysheet).Result;

                if (dt.Rows.Count > 0)
                {
                    GVListEmployees.DataSource = dt;
                    GVListEmployees.DataBind();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}