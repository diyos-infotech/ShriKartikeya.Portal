using ShriKartikeya.Portal.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ShriKartikeya.Portal.Module_Reports
{
    public partial class Salerystructure : System.Web.UI.Page
    {
        AppConfiguration config = new AppConfiguration();
        GridViewExportUtil gve = new GridViewExportUtil();
        string EmpIDPrefix = "";
        string CmpIDPrefix = "";
        string Elength = "";
        string Clength = "";
        string Fontstyle = "";
        string CFontstyle = "";
        string BranchID = "";
        string Branch = "";
        string Emp_id = "";
        DataTable dt;
        // page created by dhanalakshmi on 22-06-2022 ref:009307
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
                    LoadClientList();
                    LoadClientNames();

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

        protected void LoadClientNames()
        {
            string qry = "select Clientid,Clientname from clients where clientid like '%" + CmpIDPrefix + "%' and  Paysheet=1  order by clientname";
            DataTable DtClientids = config.ExecuteReaderWithQueryAsync(qry).Result;
            if (DtClientids.Rows.Count > 0)
            {
                ddlcname.DataValueField = "Clientid";
                ddlcname.DataTextField = "clientname";
                ddlcname.DataSource = DtClientids;
                ddlcname.DataBind();
            }
            ddlcname.Items.Insert(0, "-Select-");
            ddlcname.Items.Insert(1, "ALL");

        }

        protected void LoadClientList()
        {
            string qry = "select Clientid from clients where clientid like '%" + CmpIDPrefix + "%' and  Paysheet=1  order by clientid";
            DataTable DtClientNames = config.ExecuteReaderWithQueryAsync(qry).Result;
            if (DtClientNames.Rows.Count > 0)
            {
                ddlclientid.DataValueField = "Clientid";
                ddlclientid.DataTextField = "Clientid";
                ddlclientid.DataSource = DtClientNames;
                ddlclientid.DataBind();
            }
            ddlclientid.Items.Insert(0, "-Select-");
            ddlclientid.Items.Insert(1, "ALL");
        }

        protected void ddlcname_OnSelectedIndexChanged(object sender, EventArgs e)
        {

            if (ddlcname.SelectedIndex > 0)
            {
                //txtmonth.Text = "";
                ddlclientid.SelectedValue = ddlcname.SelectedValue;

            }
            else
            {
                ddlclientid.SelectedIndex = 0;
            }
        }

        protected void ddlclientid_SelectedIndexChanged(object sender, EventArgs e)
        {


            if (ddlclientid.SelectedIndex > 0)
            {
                //txtmonth.Text = "";
                ddlcname.SelectedValue = ddlclientid.SelectedValue;
            }
            else
            {
                ddlcname.SelectedIndex = 0;
            }
        }



        protected void lbtn_Export_Click(object sender, EventArgs e)
        {

            try
            {
                if (ddlclientid.SelectedIndex == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showlalert", "alert('Please Select Client Id/Name');", true);

                    return;
                }

                //if (txtmonth.Text.Trim().Length == 0)
                //{
                //    ScriptManager.RegisterStartupScript(this, GetType(), "showlalert", "alert('Please Select Month');", true);

                //    return;
                //}

                //string date = string.Empty;

                //if (txtmonth.Text.Trim().Length > 0)
                //{
                //    date = DateTime.Parse(txtmonth.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
                //}

                //string month = DateTime.Parse(date).Month.ToString();
                //string Year = DateTime.Parse(date).Year.ToString().Substring(2, 2);
                //string sqlqry = string.Empty;

                string clientid = ddlclientid.SelectedValue;

                if (ddlclientid.SelectedIndex == 1)
                {
                    clientid = "%";
                }

                var SPName = "IndividualSalaryStructure";
                Hashtable ht = new Hashtable();

                //ht.Add("@month", month + Year);
                ht.Add("@clientid", clientid);
                ht.Add("@ClientIDPrefix", CmpIDPrefix);
                DataTable dtSalStructure = config.ExecuteAdaptorAsyncWithParams(SPName, ht).Result;
                if (dtSalStructure.Rows.Count > 0)
                {

                    GVListEmployees.DataSource = dtSalStructure;
                    GVListEmployees.DataBind();
                    gve.Export("Salerystructure.xls", this.GVListEmployees);
                }
            }
            catch (Exception ex)
            {
            }

        }




    }
}