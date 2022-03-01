using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Globalization;
using ShriKartikeya.Portal.DAL;

namespace ShriKartikeya.Portal
{
    public partial class GetDaywise_Android_Attendance_Employee_Wise : System.Web.UI.Page
    {
        GridViewExportUtil GVUtil = new GridViewExportUtil();
        AppConfiguration config = new AppConfiguration();
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
                    LoadBranches();

                    txtmonth.Visible = true;

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

        protected void LoadEMPNames()
        {
            var Branch = "";
            if (ddlBranch.SelectedIndex == 1)
            {
                Branch = "%";
            }
            else
            {
                Branch = ddlBranch.SelectedValue;
            }
            string querybranch = "select empid,empfname from empdetails where branch like'" + Branch + "' order by empid";
            DataTable dtbranch = config.ExecuteAdaptorAsyncWithQueryParams(querybranch).Result;
            if (dtbranch.Rows.Count > 0)
            {
                ddlEName.DataValueField = "empid";
                ddlEName.DataTextField = "empfname";
                ddlEName.DataSource = dtbranch;
                ddlEName.DataBind();
            }
            ddlEName.Items.Insert(0, "--Select--");
            ddlEName.Items.Insert(1, "ALL");


        }
        protected void LoadBranches()
        {
            string querybranch = "select * from branchdetails order by branchid";
            DataTable dtbranch = config.ExecuteAdaptorAsyncWithQueryParams(querybranch).Result;
            if (dtbranch.Rows.Count > 0)
            {
                ddlBranch.DataValueField = "branchid";
                ddlBranch.DataTextField = "branchname";
                ddlBranch.DataSource = dtbranch;
                ddlBranch.DataBind();
            }
            ddlBranch.Items.Insert(0, "--Select--");
            ddlBranch.Items.Insert(1, "ALL");
        }

        protected void LoadEMPList()
        {
            var Branch = "";
            if (ddlBranch.SelectedIndex == 1)
            {
                Branch = "%";
            }
            else
            {
                Branch = ddlBranch.SelectedValue;
            }
            string querybranch = "select empid,empfname from empdetails where branch like'" + Branch + "' order by empid";
            DataTable dtbranch = config.ExecuteAdaptorAsyncWithQueryParams(querybranch).Result;
            if (dtbranch.Rows.Count > 0)
            {
                ddlEmpID.DataValueField = "empid";
                ddlEmpID.DataTextField = "empid";
                ddlEmpID.DataSource = dtbranch;
                ddlEmpID.DataBind();
            }
            ddlEmpID.Items.Insert(0, "--Select--");

        }

        protected void ddlEName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearData();
            if (ddlEName.SelectedIndex > 0)
            {
                txtmonth.Text = "";
                ddlEmpID.SelectedValue = ddlEName.SelectedValue;

            }
            else
            {
                ddlEmpID.SelectedIndex = 0;
            }
        }

        protected void ddlEmpID_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearData();

            if (ddlEmpID.SelectedIndex > 0)
            {
                txtmonth.Text = "";
                ddlEName.SelectedValue = ddlEmpID.SelectedValue;
            }
            else
            {
                ddlEName.SelectedIndex = 0;
            }
        }

        protected void btnsearch_Click(object sender, EventArgs e)
        {
            GvDayWiseAttendance.DataSource = null;
            GvDayWiseAttendance.DataBind();


            string empid = "%";
            var Branch = "";
            if (ddlBranch.SelectedIndex == 1)
            {
                Branch = "%";
            }
            else
            {
                Branch = ddlBranch.SelectedValue;
            }

            empid = ddlEmpID.SelectedValue;



            string month = "";
            string Year = "";
            string AttMonth = "";



            if (txtmonth.Text != "")
            {
                string date = DateTime.Parse(txtmonth.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
                month = DateTime.Parse(date).Month.ToString();
                Year = DateTime.Parse(date).Year.ToString();

                AttMonth = month + Year.Substring(2, 2);
            }



            string spname = "";
            DataTable dtBP = null;
            Hashtable HashtableBP = new Hashtable();

            spname = "GetDaywise_Android_Attendance_EmployeeWise";
            HashtableBP.Add("@Empid", empid);
            HashtableBP.Add("@Month", AttMonth);
            HashtableBP.Add("@Branch", Branch);


            dtBP = config.ExecuteAdaptorAsyncWithParams(spname, HashtableBP).Result;
            if (dtBP.Rows.Count > 0)
            {

                GvDayWiseAttendance.DataSource = dtBP;
                GvDayWiseAttendance.DataBind();
                lbtn_Export.Visible = true;
            }
            else
            {
                GvDayWiseAttendance.DataSource = null;
                GvDayWiseAttendance.DataBind();
                lbtn_Export.Visible = false;
            }

        }

        protected void ClearData()
        {
            GvDayWiseAttendance.DataSource = null;
            GvDayWiseAttendance.DataBind();
            lbtn_Export.Visible = false;
        }

        protected void lbtn_Export_Click(object sender, EventArgs e)
        {
            GVUtil.Export("Get_Android_Attendance Employee Wise.xls", this.GvDayWiseAttendance);

        }


        protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            GvDayWiseAttendance.DataSource = null;
            GvDayWiseAttendance.DataBind();
            LoadEMPList();
            LoadEMPNames();
        }
    }
}