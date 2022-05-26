using System;
using System.Web.UI;
using KLTS.Data;
using System.Web.UI.HtmlControls;
using System.Data;
using System.Globalization;
using System.Collections;
using ShriKartikeya.Portal.DAL;
using System.Web.UI.WebControls;

namespace ShriKartikeya.Portal.Module_Reports
{
    public partial class BonusReportNew : System.Web.UI.Page
    {
        AppConfiguration config = new AppConfiguration();
        GridViewExportUtil gve = new GridViewExportUtil();
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
                        HtmlControl emplink = (HtmlControl)this.Master.Master.FindControl("ContentPlaceHolder1").FindControl("sli1");
                        if (emplink != null)
                        {
                            emplink.Attributes["class"] = "current";
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }

        }

        protected void GetWebConfigdata()
        {

            EmpIDPrefix = Session["EmpIDPrefix"].ToString();
            CmpIDPrefix = Session["CmpIDPrefix"].ToString();
            BranchID = Session["BranchID"].ToString();
        }


        protected void lbtn_Export_Click(object sender, EventArgs e)
        {
            string filename = "Quarterly&YearlyReport.xls";
            gve.Export(filename, GVListEmployees);
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {

            if (ddlType.SelectedIndex == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "show alert", "alert('Please Select Type');", true);
                return;
            }

            if (ddlComponent.SelectedIndex == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "show alert", "alert('Please Select Component');", true);
                return;
            }

            if (txtfmonth.Text.Trim().Length == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "show alert", "alert('Please Select Month');", true);
                return;
            }

            if (txtTmonth.Text.Trim().Length == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "show alert", "alert('Please Select Month');", true);
                return;
            }

            DateTime frmdate;
            string FromDate = "";
            string Frmonth = "";
            string FrYear = "";

            if (txtfmonth.Text.Trim().Length > 0)
            {
                frmdate = DateTime.ParseExact(txtfmonth.Text.Trim(), "MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                Frmonth = frmdate.ToString("MM");
                FrYear = frmdate.ToString("yy");
            }

            FromDate = FrYear + Frmonth;

            DateTime tdate;
            string ToDate = "";
            string Tomonth = "";
            string ToYear = "";

            if (txtTmonth.Text.Trim().Length > 0)
            {
                tdate = DateTime.ParseExact(txtTmonth.Text.Trim(), "MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                Tomonth = tdate.ToString("MM");
                ToYear = tdate.ToString("yy");

            }

            ToDate = ToYear + Tomonth;

            Hashtable ht = new Hashtable();
            string SPName = "BonusDetailsReport";
            ht.Add("@fromdate", FromDate);
            ht.Add("@todate", ToDate);
            ht.Add("@Component", ddlComponent.SelectedIndex);
            ht.Add("@Type", ddlType.SelectedValue);



            DataTable dt = config.ExecuteAdaptorAsyncWithParams(SPName, ht).Result;

            int columnscount = dt.Columns.Count;
            if (dt.Rows.Count > 0)
            {
                GVListEmployees.DataSource = dt;
                GVListEmployees.DataBind();
            }
            else
            {
                GVListEmployees.DataSource = null;
                GVListEmployees.DataBind();
            }

        }

        protected void GVListEmployees_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                for (int i = 0; i < e.Row.Controls.Count; i++)
                {
                    var headerCell = e.Row.Controls[i] as DataControlFieldHeaderCell;
                    if (headerCell != null)
                    {
                        //Amount heading
                        if (headerCell.Text.Substring(2, 4) == "01_A")
                        {
                            headerCell.Text = "Jan -" + headerCell.Text.Substring(0, 2) + "_Amount";
                        }
                        if (headerCell.Text.Substring(2, 4) == "02_A")
                        {
                            headerCell.Text = "Feb -" + headerCell.Text.Substring(0, 2) + "_Amount";
                        }
                        if (headerCell.Text.Substring(2, 4) == "03_A")
                        {
                            headerCell.Text = "Mar -" + headerCell.Text.Substring(0, 2) + "_Amount";
                        }
                        if (headerCell.Text.Substring(2, 4) == "04_A")
                        {
                            headerCell.Text = "Apr -" + headerCell.Text.Substring(0, 2) + "_Amount";
                        }
                        if (headerCell.Text.Substring(2, 4) == "05_A")
                        {
                            headerCell.Text = "May -" + headerCell.Text.Substring(0, 2) + "_Amount";
                        }
                        if (headerCell.Text.Substring(2, 4) == "06_A")
                        {
                            headerCell.Text = "Jun -" + headerCell.Text.Substring(0, 2) + "_Amount";
                        }
                        if (headerCell.Text.Substring(2, 4) == "07_A")
                        {
                            headerCell.Text = "Jul -" + headerCell.Text.Substring(0, 2) + "_Amount";
                        }
                        if (headerCell.Text.Substring(2, 4) == "08_A")
                        {
                            headerCell.Text = "Aug -" + headerCell.Text.Substring(0, 2) + "_Amount";
                        }
                        if (headerCell.Text.Substring(2, 4) == "09_A")
                        {
                            headerCell.Text = "Sep -" + headerCell.Text.Substring(0, 2) + "_Amount";
                        }
                        if (headerCell.Text.Substring(2, 4) == "10_A")
                        {
                            headerCell.Text = "Oct -" + headerCell.Text.Substring(0, 2) + "_Amount";
                        }
                        if (headerCell.Text.Substring(2, 4) == "11_A")
                        {
                            headerCell.Text = "Nov -" + headerCell.Text.Substring(0, 2) + "_Amount";
                        }
                        if (headerCell.Text.Substring(2, 4) == "12_A")
                        {
                            headerCell.Text = "Dec -" + headerCell.Text.Substring(0, 2) + "_Amount";
                        }

                        //Duties heading
                        if (headerCell.Text.Substring(2, 4) == "01_D")
                        {
                            headerCell.Text = "Jan -" + headerCell.Text.Substring(0, 2) + "_Duties";
                        }
                        if (headerCell.Text.Substring(2, 4) == "02_D")
                        {
                            headerCell.Text = "Feb -" + headerCell.Text.Substring(0, 2) + "_Duties";
                        }
                        if (headerCell.Text.Substring(2, 4) == "03_D")
                        {
                            headerCell.Text = "Mar -" + headerCell.Text.Substring(0, 2) + "_Duties";
                        }
                        if (headerCell.Text.Substring(2, 4) == "04_D")
                        {
                            headerCell.Text = "Apr -" + headerCell.Text.Substring(0, 2) + "_Duties";
                        }
                        if (headerCell.Text.Substring(2, 4) == "05_D")
                        {
                            headerCell.Text = "May -" + headerCell.Text.Substring(0, 2) + "_Duties";
                        }
                        if (headerCell.Text.Substring(2, 4) == "06_D")
                        {
                            headerCell.Text = "Jun -" + headerCell.Text.Substring(0, 2) + "_Duties";
                        }
                        if (headerCell.Text.Substring(2, 4) == "07_D")
                        {
                            headerCell.Text = "Jul -" + headerCell.Text.Substring(0, 2) + "_Duties";
                        }
                        if (headerCell.Text.Substring(2, 4) == "08_D")
                        {
                            headerCell.Text = "Aug -" + headerCell.Text.Substring(0, 2) + "_Duties";
                        }
                        if (headerCell.Text.Substring(2, 4) == "09_D")
                        {
                            headerCell.Text = "Sep -" + headerCell.Text.Substring(0, 2) + "_Duties";
                        }
                        if (headerCell.Text.Substring(2, 4) == "10_D")
                        {
                            headerCell.Text = "Oct -" + headerCell.Text.Substring(0, 2) + "_Duties";
                        }
                        if (headerCell.Text.Substring(2, 4) == "11_D")
                        {
                            headerCell.Text = "Nov -" + headerCell.Text.Substring(0, 2) + "_Duties";
                        }
                        if (headerCell.Text.Substring(2, 4) == "12_D")
                        {
                            headerCell.Text = "Dec -" + headerCell.Text.Substring(0, 2) + "_Duties";
                        }

                    }
                }
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[5].Attributes.Add("class", "text");
            }
        }

    }
}