using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using KLTS.Data;
using System.Collections.Generic;
using System.Globalization;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
using System.Data.OleDb;
using ShriKartikeya.Portal.DAL;

namespace ShriKartikeya.Portal
{
    public partial class ZeroAttendanceReport : System.Web.UI.Page
    {
        AppConfiguration config = new AppConfiguration();
        GridViewExportUtil gve = new GridViewExportUtil();
        string EmpIDPrefix = "";
        string CmpIDPrefix = "";
        string Elength = "";
        string Clength = "";
        string Zone = "";

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
            EmpIDPrefix = Session["EmpIDPrefix"].ToString();
            CmpIDPrefix = Session["CmpIDPrefix"].ToString();
        }


        public void GetSampleExport()
        {
            string date = string.Empty;
            if (txtmonth.Text.Trim().Length > 0)
            {
                date = DateTime.Parse(txtmonth.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
            }

            string month = DateTime.Parse(date).Month.ToString("00");
            string Year = DateTime.Parse(date).Year.ToString();

            string datev = Year + "-" + month + "-" + GlobalData.Instance.GetNoOfDaysForThisMonthNew(int.Parse(Year), int.Parse(month));

            string qry = "select distinct (isnull(ed.EmpId,'')+'-'+isnull(ed.OldEmpid,'')) as EmpId,isnull(c.clientName,'') as ClientName,(ed.EmpFName+' '+ed.EmpMName+' '+ed.EmpLName) as Name,'' as Dateofleaving from EmpDetails ed  left join Clients c on c.clientid=ed.UnitId where ed.EmpId not in (select EmpId from EmpAttendance where MONTH = '" + month + Year.Substring(2, 2) + "') and ed.Empstatus=1 and EmpDtofJoining<=cast('" + datev + "' as date) and ed.EmpId not like ('%NYA%') and EmpId like'%" + EmpIDPrefix + "%'";
            DataTable dt = config.ExecuteAdaptorAsyncWithQueryParams(qry).Result;

            if (dt.Rows.Count > 0)
            {
                gvlistofemp.DataSource = dt;
                gvlistofemp.DataBind();

            }
            else
            {
                gvlistofemp.DataSource = null;
                gvlistofemp.DataBind();
            }
        }

        protected void lnkImportfromexcel_Click(object sender, EventArgs e)
        {
            gve.Export("Sampleempdetails.xls", this.gvlistofemp);
        }
        DataTable dt = new DataTable();
        protected void btnsave_Click(object sender, EventArgs e)
        {
            try
            {

                int result = 0;
                string ExcelSheetname = "";
                string FileName = FileUploadEmpDetails.FileName;
                string path = Path.Combine(Server.MapPath("~/ImportDocuments"), Guid.NewGuid().ToString() + Path.GetExtension(FileUploadEmpDetails.PostedFile.FileName));
                FileUploadEmpDetails.PostedFile.SaveAs(path);

                OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=Excel 12.0;");
                con.Open();
                dt = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                ExcelSheetname = dt.Rows[0]["TABLE_NAME"].ToString();

                OleDbCommand cmd = new OleDbCommand("Select [IDNO],[Emp Name],[ClientName],[Date of leaving] from [" + ExcelSheetname + "]", con);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                DataTable ds = new DataTable();
                da.Fill(ds);


                using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["KLTSConnectionString"].ConnectionString))
                {
                    sqlcon.Open();

                    string IDNO = ""; string Dateofleaving = "";

                    #region Begin Getmax Id from DB
                    int ExcelNo = 0;
                    string selectquery = "select max(cast(Excel_Number as int )) as Id from empdetails ";
                    DataTable dtExcelID = config.ExecuteAdaptorAsyncWithQueryParams(selectquery).Result;

                    if (dtExcelID.Rows.Count > 0)
                    {
                        if (String.IsNullOrEmpty(dtExcelID.Rows[0]["Id"].ToString()) == false)
                        {
                            ExcelNo = Convert.ToInt32(dtExcelID.Rows[0]["Id"].ToString()) + 1;
                        }
                        else
                        {
                            ExcelNo = int.Parse("1");
                        }
                    }
                    #endregion End Getmax Id from DB

                    for (int i = 0; i < ds.Rows.Count; i++)
                    {
                        IDNO = ds.Rows[i]["IDNO"].ToString();
                        var EMPID = IDNO.Substring(0, 9);
                        Dateofleaving = ds.Rows[i]["Date of leaving"].ToString();

                        string Dateofleaving1 = "01/01/1900";

                        if (Dateofleaving.Length > 0)
                        {
                            string db1 = Convert.ToDateTime(Dateofleaving).ToString("dd/MM/yyyy");
                            Dateofleaving1 = Timings.Instance.CheckDateFormat(db1);
                        }
                        else
                        {
                            Dateofleaving1 = "01/01/1900";
                        }


                        string UpdatQry = "";
                        if (Dateofleaving.Length > 0)
                        {
                            UpdatQry = "update EmpDetails set EmpDtofLeaving='" + Dateofleaving1 + "', Empstatus=0  where empid='" + EMPID + "'";
                        }
                        else
                        {
                            UpdatQry = "update EmpDetails set EmpDtofLeaving='" + Dateofleaving1 + "' where empid='" + EMPID + "'";
                        }
                        result = config.ExecuteNonQueryWithQueryAsync(UpdatQry).Result;
                        if (result > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "showlalert", "alert('Employee Details added Successfuly');", true);
                        }

                    }
                }

            }
            catch (Exception Ex)
            {

            }
        }

        protected void txtmonth_TextChanged(object sender, EventArgs e)
        {
            GetSampleExport();
        }

        protected void gvlistofemp_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[0].Attributes.Add("class", "text");
            }
        }
    }
}