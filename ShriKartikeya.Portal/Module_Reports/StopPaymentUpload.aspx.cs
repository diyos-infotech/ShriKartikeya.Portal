using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.IO;
using System.Data.OleDb;
using KLTS.Data;
using ShriKartikeya.Portal.DAL;

namespace ShriKartikeya.Portal.Module_Reports
{
    public partial class StopPaymentUpload : System.Web.UI.Page
    {
        string EmpIDPrefix = "";
        string CmpIDPrefix = "";
        AppConfiguration config = new AppConfiguration();
        GridViewExportUtil util = new GridViewExportUtil();

        protected void Page_Load(object sender, EventArgs e)
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




                string sqlemptydata = "select * from tblEmptyforExcel";
                DataTable dtempty = config.ExecuteAdaptorAsyncWithQueryParams(sqlemptydata).Result;
                if (dtempty.Rows.Count > 0)
                {
                    GVStopPayment.DataSource = dtempty;
                    GVStopPayment.DataBind();
                }

                System.IO.DirectoryInfo di = new DirectoryInfo(Server.MapPath("ImportDocuments"));

                foreach (FileInfo file in di.GetFiles())
                {

                    file.Delete();
                }
            }
        }

        protected void GetWebConfigdata()
        {
            if (Session.Keys.Count > 0)
            {
                EmpIDPrefix = Session["EmpIDPrefix"].ToString();
                CmpIDPrefix = Session["CmpIDPrefix"].ToString();
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }

        public void GoToLoginPage()
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "show alert", "alert('Your Session Expired.Please Login');", true);
            Response.Redirect("~/login.aspx");
        }


        protected void txtmonth_TextChanged(object sender, EventArgs e)
        {

            GVStopPayment.DataSource = null;
            GVStopPayment.DataBind();
        }




        protected string GetEmpName(string empId)
        {
            string name = null;

            string sqlQry = "Select EmpFName,EmpMName from EmpDetails where EmpId='" + empId + "'";
            DataTable data = config.ExecuteAdaptorAsyncWithQueryParams(sqlQry).Result;
            if (data.Rows.Count > 0)
            {
                name = data.Rows[0]["EmpFName"].ToString() + " " + data.Rows[0]["EmpMName"].ToString();
            }
            return name;
        }


        public string GetExcelSheetNames()
        {
            string ExcelSheetname = "";
            OleDbConnection con = null;
            DataTable dt = null;
            string filename = Path.Combine(Server.MapPath("~/ImportDocuments"), Guid.NewGuid().ToString() + Path.GetExtension(fileupload1.PostedFile.FileName));
            fileupload1.PostedFile.SaveAs(filename);
            string extn = Path.GetExtension(fileupload1.PostedFile.FileName);
            string conStr = string.Empty;
            if (extn.ToLower() == ".xls")
            {
                conStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filename + ";Extended properties=\"excel 8.0;HDR=Yes;IMEX=2\"";
            }
            else if (extn.ToLower() == ".xlsx")
            {
                conStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filename + ";Extended properties=\"excel 12.0;HDR=Yes;IMEX=2\"";
            }

            con = new OleDbConnection(conStr);
            con.Open();
            dt = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            if (dt == null)
            {
                return null;
            }
            ExcelSheetname = dt.Rows[0]["TABLE_NAME"].ToString();
            return ExcelSheetname;
        }


        protected void btnImport_Click(object sender, EventArgs e)
        {
            int days = 0;
            string date = string.Empty;

            if (txtmonth.Text.Trim().Length > 0)
            {
                date = DateTime.Parse(txtmonth.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
            }

            string month = DateTime.Parse(date).Month.ToString();
            string Year = DateTime.Parse(date).Year.ToString();

            string Month = month + Year.Substring(2, 2);



            if (txtmonth.Text.Trim().Length == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Show alert", "alert('Please Select Month');", true);
                return;
            }

            try
            {
                #region Begin Code for when select Full Attendance as on 31/07/2014 by Venkat
                //

                string filename = Path.Combine(Server.MapPath("~/ImportDocuments"), Guid.NewGuid().ToString() + Path.GetExtension(fileupload1.PostedFile.FileName));
                fileupload1.PostedFile.SaveAs(filename);
                string extn = Path.GetExtension(fileupload1.PostedFile.FileName);
                string constring = "";
                if (extn.ToLower() == ".xls")
                {
                    constring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filename + ";Extended properties=\"excel 12.0;HDR=Yes;IMEX=2\"";
                }
                else if (extn.ToLower() == ".xlsx")
                {
                    constring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filename + ";Extended properties=\"excel 12.0;HDR=Yes;IMEX=2\"";
                }

                string Sheetname = string.Empty;

                string qry = "select [Client Id],[Emp Id],[Stop Payment]" + "  from  [" + GetExcelSheetNames() + "]" + "";


                OleDbConnection con = new OleDbConnection(constring);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();

                }
                OleDbCommand cmd = new OleDbCommand(qry, con);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                da.Dispose();
                con.Close();
                con.Dispose();
                GC.Collect();

                string empid = string.Empty;
                string clientid = string.Empty;
                string stoppayment = string.Empty;
                string Checkempid= string.Empty;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string Remark = string.Empty;
                    clientid = ds.Tables[0].Rows[i]["Client Id"].ToString();
                    empid = ds.Tables[0].Rows[i]["Emp Id"].ToString();
                    stoppayment = ds.Tables[0].Rows[i]["Stop Payment"].ToString().ToUpper();
                    empid = ds.Tables[0].Rows[i]["Emp Id"].ToString();
                    if ( stoppayment.Trim() == "YES")
                    {
                        stoppayment = "1";
                    }
                    if (stoppayment.Trim() == "NO")
                    {
                        stoppayment = "0";
                    }
                    string sqlchkempid = "select empid,clientid,case stoppayment when '0' then 'NO' else 'YES' end as stoppayment from empattendance where clientid='" + clientid + "' and Empid='" + empid + "' and month='" + Month + "'";
                    DataTable dtchkempid = config.ExecuteAdaptorAsyncWithQueryParams(sqlchkempid).Result;
                    if (dtchkempid.Rows.Count > 0)
                    {
                         Checkempid = dtchkempid.Rows[0]["empid"].ToString();
                    }
                    if (Checkempid.Length > 0)
                    {
                        GridView1.DataSource = null;
                        GridView1.DataBind();
                    }
                    else
                    {
                        lblresult.Visible = true;
                        lblresult.Text = "These employees stoppayment is not uploaded because attendance is not available for this month ";
                        GridView1.Visible = true;
                        GridView1.DataSource = ds;
                        GridView1.DataBind();
                        return;
                    }
                    if (stoppayment=="1")
                    {
                        string Query = "update EmpAttendance set stoppayment=1 where clientid='" + clientid + "' and Empid='" + empid + "' and month='" + month + "'";
                        int Result = config.ExecuteNonQueryWithQueryAsync(Query).Result;
                    }
                    if (stoppayment == "0")
                    {
                        string Query = "update EmpAttendance set stoppayment=0 where clientid='" + clientid + "' and Empid='" + empid + "' and month='" + month + "'";
                        int Result = config.ExecuteNonQueryWithQueryAsync(Query).Result;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Show alert", "alert('Please Upload Valid Data');", true);
                lblMessage.Visible = false;

            }
        }

        protected void lnkImportfromexcel_Click(object sender, EventArgs e)
        {

            util.Export("Stop Payment Upload.xls", this.GVStopPayment);
        }
    }
}