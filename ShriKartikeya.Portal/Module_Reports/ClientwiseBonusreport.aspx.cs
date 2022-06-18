using KLTS.Data;
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
    public partial class ClientwiseBonusreport : System.Web.UI.Page
    {
        string CmpIDPrefix = "";
        string BranchID = "";
        AppConfiguration config = new AppConfiguration();
        GridViewExportUtil gve = new GridViewExportUtil();
        // page created by dhanalakshmi on 15-06-2022 ref:009307
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

       

       
        
        protected void ddlcname_OnSelectedIndexChanged(object sender, EventArgs e)
        {
           
            if (ddlcname.SelectedIndex > 0)
            {
                ddlClientId.SelectedValue = ddlcname.SelectedValue;
            }
            else
            {

                ddlClientId.SelectedIndex = 0;
            }

        }

        protected void ddlClientId_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            if (ddlClientId.SelectedIndex > 0)
            {
                ddlcname.SelectedValue = ddlClientId.SelectedValue;
            }
            else
            {
                ddlcname.SelectedIndex = 0;
            }
        }

        protected void FillClientList()
        {
            DataTable dtBranch = GlobalData.Instance.LoadBranchOnUserID(BranchID);

            DataTable dt = GlobalData.Instance.LoadCIds(CmpIDPrefix, dtBranch);
            if (dt.Rows.Count > 0)
            {
                ddlClientId.DataValueField = "clientid";
                ddlClientId.DataTextField = "clientid";
                ddlClientId.DataSource = dt;
                ddlClientId.DataBind();
            }
            ddlClientId.Items.Insert(0, "--Select--");
            ddlClientId.Items.Insert(1, "ALL");


        }

        protected void FillClientNameList()
        {
            DataTable dtBranch = GlobalData.Instance.LoadBranchOnUserID(BranchID);

            DataTable dt = GlobalData.Instance.LoadCNames(CmpIDPrefix, dtBranch);
            if (dt.Rows.Count > 0)
            {
                ddlcname.DataValueField = "clientid";
                ddlcname.DataTextField = "Clientname";
                ddlcname.DataSource = dt;
                ddlcname.DataBind();
            }
            ddlcname.Items.Insert(0, "--Select--");
            ddlcname.Items.Insert(1, "ALL");


        }

        protected void GetWebConfigdata()
        {
            CmpIDPrefix = Session["CmpIDPrefix"].ToString();
            BranchID = Session["BranchID"].ToString();

        }

        protected void lbtn_Export_Click(object sender, EventArgs e)
        {
            try
            {


                DataTable dt = null;
                string strQry = "Select * from CompanyInfo  where   ClientidPrefix='" + CmpIDPrefix + "'";
                DataTable compInfo = SqlHelper.Instance.GetTableByQuery(strQry);
                string companyName = "Your Company Name";
                string companyAddress = "Your Company Address";

                string date = string.Empty;

                

                if (compInfo.Rows.Count > 0)
                {
                    companyName = compInfo.Rows[0]["CompanyName"].ToString();
                    companyAddress = compInfo.Rows[0]["Address"].ToString();
                }

                DateTime frmdate;
                string FromDate = "";
                string Frmonth = "";
                string FrYear = "";
                string FrMn = "";
                string formatted = "";
                string toatted = "";

                if (Txt_From_Date.Text.Trim().Length > 0)
                {
                    frmdate = DateTime.ParseExact(Txt_From_Date.Text.Trim(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    formatted = frmdate.ToString("yyyy-MM-dd");
                    Frmonth = frmdate.ToString("MM");
                    FrYear = frmdate.ToString("yy");
                    FrMn = frmdate.Month.ToString();
                }

                FromDate = FrYear + Frmonth;


                DateTime tdate;
                string ToDate = "";
                string Tomonth = "";
                string ToYear = "";
                string ToMn = "";
                if (Txt_To_Date.Text.Trim().Length > 0)
                {
                    tdate = DateTime.ParseExact(Txt_To_Date.Text.Trim(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    toatted = tdate.ToString("yyyy-MM-dd");
                    Tomonth = tdate.ToString("MM");
                    ToYear = tdate.ToString("yy");
                    ToMn = tdate.Month.ToString();

                }

                ToDate = ToYear + Tomonth;
                string strQrydate = "SELECT DATEDIFF(MONTH, '" + formatted + "', '" + toatted + "') as Months ";
                DataTable compInfos = SqlHelper.Instance.GetTableByQuery(strQrydate);

                string MOnthdays = compInfos.Rows[0]["Months"].ToString();



                string selectclientaddress = "select * from clients where clientid= '" + ddlClientId.SelectedValue + "'";
                DataTable dtclientaddress = SqlHelper.Instance.GetTableByQuery(selectclientaddress);
                string addressData = "";
                string AddrHno = ""; string AddrColony = ""; string AddrArea = ""; string AddrStreet = ""; string Addrcity = ""; string Addrstate = ""; string Addrpin = "";
                if (dtclientaddress.Rows.Count > 0)
                {



                    AddrHno = dtclientaddress.Rows[0]["ShiptoLine1"].ToString() + " ";
                    AddrHno += dtclientaddress.Rows[0]["ShiptoLine2"].ToString() + " ";
                    AddrHno += dtclientaddress.Rows[0]["ShiptoLine3"].ToString() + " ";
                    AddrHno += dtclientaddress.Rows[0]["ShiptoLine4"].ToString() + " ";
                    AddrHno += dtclientaddress.Rows[0]["ShiptoLine5"].ToString() + " ";
                    AddrHno += dtclientaddress.Rows[0]["ShiptoLine6"].ToString() + " ";


                }

                string selectcontracts = "select typeofwork from Contracts where clientid= '" + ddlClientId.SelectedValue + "'";
                DataTable dtcontraccts = SqlHelper.Instance.GetTableByQuery(selectcontracts);
                string typeofwork = "";
                if (dtcontraccts.Rows.Count > 0)
                {
                    typeofwork = dtcontraccts.Rows[0]["typeofwork"].ToString() + "";
                }
                string line1 = "FORM C-BONUS PAID TO EMPLOYEES FOR THE ACCOUNTING YEAR ENDING ON THE";

                string line2 = "[See Rule 4 (c) of the Payment of Bonus Rules 1975]";

                string line3 = "Name and address of the establishment  : " + companyName + "<br> " + companyAddress + "";

                string line4 = "Name and address of Principal Employer : " + AddrHno;


                //int type = ddlForms.SelectedIndex;
                string Spname = "Bonusreport";
                Hashtable ht = new Hashtable();
                //ht.Add("@type", type);
                ht.Add("@ClientID", ddlClientId.SelectedValue);
                ht.Add("@dateFrom", FromDate);
                ht.Add("@dateTo", ToDate);


                dt = config.ExecuteAdaptorAsyncWithParams(Spname, ht).Result;

                string Filename = ddlcname.SelectedItem + "";
                if (dt.Rows.Count > 0)
                {
                    gve.ExporttoExcelformc(dt, Filename, line1, line2, line3, line4);
                }

            }
            catch (Exception ex)
            {

                return;
            }









     
        }


       



        
    }
}