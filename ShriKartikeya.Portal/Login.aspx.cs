﻿using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using KLTS.Data;
using ShriKartikeya.Portal.DAL;

namespace ShriKartikeya.Portal
{
    public partial class Login : System.Web.UI.Page
    {
        AppConfiguration config = new AppConfiguration();
        GridViewExportUtil gve = new GridViewExportUtil();
        MenuBAL BalObj = new MenuBAL();
        string ipaddress;
        protected void Page_Load(object sender, EventArgs e)
        {
           

            lblerror.Text = "";
            if (!IsPostBack)
            {

                Session["UserId"] = string.Empty;
                Session["AccessLevel"] = string.Empty;

                Session["EmpIDPrefix"] = string.Empty;
                Session["CmpIDPrefix"] = string.Empty;
                Session["BillnoWithoutST"] = string.Empty;
                Session["BillnoWithST"] = string.Empty;
                Session["IP"] = string.Empty;
                Session["BillprefixWithST"] = string.Empty;
                Session["BillprefixWithoutST"] = string.Empty;
                Session["Emp_Id"] = string.Empty;
                Session["BranchID"] = string.Empty;

                lblcname.Text = SqlHelper.Instance.GetCompanyname();

                Response.Redirect("signin.aspx");
            }
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            try
            {
                var UserName = txtUserName.Text.Trim();
                var password = txtPassword.Text.Trim();
                string IPAddress = "";

                string qry = "select ip from logindetails where username='" + UserName + "'";
                DataTable dtqry = config.ExecuteAdaptorAsyncWithQueryParams(qry).Result;

                if (dtqry.Rows.Count > 0)
                {
                    IPAddress = dtqry.Rows[0]["ip"].ToString();
                }

                if (IPAddress == ipaddress || IPAddress == "")
                {

                    LoginFunction(UserName, password);
                }
                else
                {
                    Response.Redirect("Error.aspx");
                }
            }

            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "show alert", "alert('Invalid UserName/Password.Your session expiered');", true);
            }



        }

        protected void LoginFunction(string UserName, string password)
        {
            Session["uname"] = UserName;
            Session["pwd"] = password;
            int chksession = BalObj.ChecKSession(UserName, Session["SessionId"].ToString(), "C");

            if (chksession == 1)
            {
                #region Begin Code For  Variable Decalration as on  [01-10-2013]

                var SPName = string.Empty;
                SPName = "CheckCredentials";
                Hashtable HTSpParameters = new Hashtable();
                HTSpParameters.Add("@UserName", UserName);
                HTSpParameters.Add("@password", password);
                DataTable DtCheckCredentials = config.ExecuteAdaptorAsyncWithParams(SPName, HTSpParameters).Result;

                #endregion  End  Code For SP PArameters / Calling Stored Procedure as on [01-10-2013]

                if (DtCheckCredentials.Rows.Count > 0)
                {
                    Session["UserId"] = DtCheckCredentials.Rows[0]["username"].ToString();
                    Session["AccessLevel"] = DtCheckCredentials.Rows[0]["previligeid"].ToString();
                    Session["IP"] = DtCheckCredentials.Rows[0]["IP"].ToString();
                    Session["homepage"] = DtCheckCredentials.Rows[0]["PATH"].ToString();
                    Session["Emp_Id"] = DtCheckCredentials.Rows[0]["Emp_Id"].ToString();
                    Session["BranchID"] = DtCheckCredentials.Rows[0]["BranchID"].ToString();

                    Response.Redirect(Session["homepage"].ToString());

                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "open", "alert();", true);

                }

            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "open", "show();", true);

            }
        }

        protected void ButtonYes_Click(object sender, EventArgs e)
        {
            Response.Write(Session["SessionId"]);
            int c = BalObj.ChecKSession(Session["uname"].ToString(), Session["SessionId"].ToString(), "I");
            LoginFunction(Session["uname"].ToString(), Session["pwd"].ToString());
        }

        protected void ButtonNo_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/login.aspx");
        }
    }
}