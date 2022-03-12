using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Collections;
using System.Data;
using System.Globalization;
using ShriKartikeya.Portal.DAL;

namespace ShriKarthikeya.Portal
{
    public partial class Dashboard : System.Web.UI.Page
    {
        AppConfiguration conf = new AppConfiguration();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                LoadBirthdays();
                LoadAllBirthdays();
                LoadData();
                LoadDepartment();
                BindPaysheetSummarydata();
                BindPaysheetAllwDedSummarydata();
            }
        }

        private void BindPaysheetSummarydata()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Wage Details");
            dt.Columns.Add("Amount");
            dt.Rows.Add();
            gvpaysheet.DataSource = dt;
            gvpaysheet.DataBind();
            gvpaysheet.Rows[0].Visible = false;
        }

        //private void BindBiodata()
        //{
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("Text");
        //    dt.Columns.Add("Value");
        //    dt.Rows.Add();
        //    gvBiodata.DataSource = dt;
        //    gvBiodata.DataBind();
        //    gvBiodata.Rows[0].Visible = false;
        //}

        //private void BindDepCount()
        //{
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("Department");
        //    dt.Rows.Add();
        //    gvDepwiseEmp.DataSource = dt;
        //    gvDepwiseEmp.DataBind();
        //    gvDepwiseEmp.Rows[0].Visible = false;
        //}

        private void BindPaysheetAllwDedSummarydata()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Salary Summary");
            dt.Columns.Add("Current Month");
            dt.Columns.Add("Previous Month");
            dt.Columns.Add("(%)");
            dt.Rows.Add();
            gvsalsummary.DataSource = dt;
            gvsalsummary.DataBind();
            gvsalsummary.Rows[0].Visible = false;
        }

        private void LoadBirthdays()
        {
            string SPName = "SPForDashboard";
            Hashtable ht = new Hashtable();
            ht.Add("@Option", "BirthdayList");
            ht.Add("@Type", "Partial");
            DataTable dt = conf.ExecuteAdaptorAsyncWithParams(SPName, ht).Result;
            if (dt.Rows.Count > 0)
            {
                RepeatInformation.DataSource = dt;
                RepeatInformation.DataBind();

                lblbirthdaytext.InnerHtml = dt.Rows[0]["TotalCount"].ToString() + " Birthdays Today";

                if (int.Parse(dt.Rows[0]["TotalCount"].ToString()) > 2)
                {
                    btnViewmore1.Visible = true;
                }
                else
                {
                    btnViewmore1.Visible = false;

                }
            }
            else
            {

                lblbirthdaytext.InnerHtml = "Birthdays";
                btnViewmore1.Visible = false;

            }
        }

        private void LoadAllBirthdays()
        {
            string SPName = "SPForDashboard";
            Hashtable ht = new Hashtable();
            ht.Add("@Option", "BirthdayList");
            ht.Add("@Type", "All");
            DataTable dt = conf.ExecuteAdaptorAsyncWithParams(SPName, ht).Result;
            if (dt.Rows.Count > 0)
            {
                RepeatAllBirthdayInfo.DataSource = dt;
                RepeatAllBirthdayInfo.DataBind();

            }
        }


        public string GetMonth()
        {
            string date = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");

            if (txtMonth.Text.Trim().Length > 0)
            {
                date = DateTime.Parse(txtMonth.Text.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
            }
            string monthnew = string.Empty;

            string month = DateTime.Parse(date).Month.ToString();
            string Year = DateTime.Parse(date).Year.ToString().Substring(2, 2);

            return month + Year;
        }

       

       

        public void LoadDepartment()
        {
            string SPName = "SPForDashboard";
            Hashtable ht = new Hashtable();
            ht.Add("@Option", "LoadDepartment");
            DataTable dt = conf.ExecuteAdaptorAsyncWithParams(SPName, ht).Result;
            if (dt.Rows.Count > 0)
            {
                ddlDepartment.DataTextField = "DeptName";
                ddlDepartment.DataValueField = "DeptId";
                ddlDepartment.DataSource = dt;
                ddlDepartment.DataBind();
            }

            ddlDepartment.Items.Insert(0, "Select Department");
        }

        public void LoadData()
        {
            var Department = "%";

            if (ddlDepartment.SelectedIndex > 0)
            {
                Department = ddlDepartment.SelectedValue;
            }

           

            string month = GetMonth();

            string SPName = "SPForDashboard";
            Hashtable ht = new Hashtable();
            ht.Add("@Option", "SalarySummary");
            ht.Add("@Department", Department);
            ht.Add("@Month", month);
            DataTable dt = conf.ExecuteAdaptorAsyncWithParams(SPName, ht).Result;
            if (dt.Rows.Count > 0)
            {
                gvsalsummary.DataSource = dt;
                gvsalsummary.DataBind();
            }
        }

       

        [WebMethod]
        public static Biodata[] GetBiodata(string Empid)
        {
            AppConfiguration config = new AppConfiguration();
            List<Biodata> details = new List<Biodata>();

            try
            {

                string SPName = "SPForDashboard";
                Hashtable ht = new Hashtable();
                ht.Add("@Option", "EmpProfile");
                ht.Add("@Empid", Empid);
                DataTable dt = config.ExecuteAdaptorAsyncWithParams(SPName, ht).Result;


                foreach (DataRow dtrow in dt.Rows)
                {
                    Biodata psum = new Biodata();
                    psum.text = dtrow["Text"].ToString();
                    psum.value = dtrow["Value"].ToString();
                    details.Add(psum);
                }

                return details.ToArray();
            }
            catch(Exception ex)
            {
                return details.ToArray();
            }

        }

        [WebMethod]
        public static EmployeeDetails[] GetEmployeeDetails(string Empid)
        {
            AppConfiguration config = new AppConfiguration();
            List<EmployeeDetails> details = new List<EmployeeDetails>();

            string SPName = "SPForDashboard";
            Hashtable ht = new Hashtable();
            ht.Add("@Option", "EmployeeDetails");
            ht.Add("@Empid", Empid);
            DataTable dt = config.ExecuteAdaptorAsyncWithParams(SPName, ht).Result;

            foreach (DataRow dtrow in dt.Rows)
            {
                EmployeeDetails psum = new EmployeeDetails();
                psum.Name = dtrow["Name"].ToString();
                psum.Design = dtrow["Design"].ToString();
                psum.Image = dtrow["Image"].ToString();
                details.Add(psum);
            }

            return details.ToArray();

        }

        [WebMethod]
        public static DepEmpCountData[] GetDepEmpCountData()
        {
            AppConfiguration config = new AppConfiguration();

            List<DepEmpCountData> details = new List<DepEmpCountData>();

            string SPName = "SPForDashboard";
            Hashtable ht = new Hashtable();
            ht.Add("@Option", "DeptEmpCount");
            DataTable dt = config.ExecuteAdaptorAsyncWithParams(SPName, ht).Result;


            foreach (DataRow dtrow in dt.Rows)
            {
                DepEmpCountData psum = new DepEmpCountData();
                psum.Deptempcount = dtrow["Department"].ToString();
                details.Add(psum);
            }

            return details.ToArray();

        }



        [WebMethod]
        public static MaleFemaleCount[] GetMaleFemaleCount(string Department)
        {
            AppConfiguration config = new AppConfiguration();

            List<MaleFemaleCount> details = new List<MaleFemaleCount>();

            string SPName = "SPForDashboard";
            Hashtable ht = new Hashtable();
            ht.Add("@Option", "GenderEmpCount");
            ht.Add("@Department", Department);
            DataTable dt = config.ExecuteAdaptorAsyncWithParams(SPName, ht).Result;


            foreach (DataRow dtrow in dt.Rows)
            {
                MaleFemaleCount psum = new MaleFemaleCount();
                psum.Malecount = dtrow["MaleCount"].ToString();
                psum.Femalecount = dtrow["FemaleCount"].ToString();
                details.Add(psum);
            }

            return details.ToArray();

        }


        [WebMethod]
        public static PocketfameAttData[] GetPocketfameAttData(string month, string Department)
        {
            AppConfiguration config = new AppConfiguration();

            string date = string.Empty;
            string Year = "";
            string day = "";
            string monthval = "";
            string monthvalue = "";

            if (month.Trim().Length > 0)
            {
                date = DateTime.Parse(month.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
                monthval = DateTime.Parse(date).Month.ToString("00");
                Year = DateTime.Parse(date).Year.ToString();
                day = DateTime.Parse(date).Day.ToString("00");
                monthvalue = Year + '-' + monthval + '-' + day;

            }


            List<PocketfameAttData> details = new List<PocketfameAttData>();

            string SPName = "SPForDashboard";
            Hashtable ht = new Hashtable();
            ht.Add("@Option", "PocketFameAtt");
            ht.Add("@Department", Department);
            ht.Add("@date", monthvalue);
            DataTable dt = config.ExecuteAdaptorAsyncWithParams(SPName, ht).Result;


            foreach (DataRow dtrow in dt.Rows)
            {
                PocketfameAttData psum = new PocketfameAttData();
                psum.Total = dtrow["Total"].ToString();
                psum.Present = dtrow["Present"].ToString();
                psum.Absent = dtrow["Absent"].ToString();
                psum.NoSingleAttn = dtrow["NoSingleAttn"].ToString();
                details.Add(psum);
            }

            return details.ToArray();

        }


        [WebMethod]
        public static PaysheetSummary[] GetPaysheetSummary(string month, string Department,string Empid)
        {
            AppConfiguration config = new AppConfiguration();

            string date = string.Empty;
            string Year = "";
            string monthval = "";
            string monthvalue = "";

            if (month.Trim().Length > 0)
            {
                date = DateTime.Parse(month.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
                monthval = DateTime.Parse(date).Month.ToString();
                Year = DateTime.Parse(date).Year.ToString();
                monthvalue = monthval + Year.Substring(2, 2);
            }

            List<PaysheetSummary> details = new List<PaysheetSummary>();

            string SPName = "SPForDashboard";
            Hashtable ht = new Hashtable();
            ht.Add("@Option", "PaysheetSummary");
            ht.Add("@Department", Department);
            ht.Add("@Month", monthvalue);
            ht.Add("@Empid", Empid);

            DataTable dt = config.ExecuteAdaptorAsyncWithParams(SPName, ht).Result;


            foreach (DataRow dtrow in dt.Rows)
            {
                PaysheetSummary psum = new PaysheetSummary();
                psum.WageDetails = dtrow["WageDetails"].ToString();
                psum.Amount = decimal.Parse(dtrow["Amount"].ToString());
                details.Add(psum);
            }

            return details.ToArray();

        }


        [WebMethod]
        public static PaysheetAllwDedSummary[] GetPaysheetAllwDedSummary(string month,  string Department, string Empid)
        {
            AppConfiguration config = new AppConfiguration();

            string date = string.Empty;
            string Year = "";
            string monthval = "";
            string monthvalue = "";

            if (month.Trim().Length > 0)
            {
                date = DateTime.Parse(month.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
                monthval = DateTime.Parse(date).Month.ToString();
                Year = DateTime.Parse(date).Year.ToString();
                monthvalue = monthval + Year.Substring(2, 2);
            }

            List<PaysheetAllwDedSummary> details = new List<PaysheetAllwDedSummary>();

            string SPName = "SPForDashboard";
            Hashtable ht = new Hashtable();
            ht.Add("@Option", "SalarySummary");
            ht.Add("@Department", Department);
            ht.Add("@Month", monthvalue);
            ht.Add("@Empid", Empid);

            DataTable dt = config.ExecuteAdaptorAsyncWithParams(SPName, ht).Result;


            foreach (DataRow dtrow in dt.Rows)
            {
                PaysheetAllwDedSummary psum = new PaysheetAllwDedSummary();
                psum.SalarySummary = dtrow["SalarySummary"].ToString();
                psum.CurrentMonth = decimal.Parse(dtrow["CurrentMonth"].ToString());
                psum.PreviousMonth = decimal.Parse(dtrow["PreviousMonth"].ToString());
                psum.Per = decimal.Parse(dtrow["Per"].ToString());
                details.Add(psum);
            }

            return details.ToArray();

        }


        [WebMethod]
        public static List<SalarySpecifications> graphSalarySpecification(string month,  string Department, string Empid)
        {
            AppConfiguration config = new AppConfiguration();

            string monthValue = "";
            decimal NetPayAmt = 0;

            string date = string.Empty;
            string monthval = "";
            string Year = "";
            string monthvalue = "";

            if (month.Trim().Length > 0)
            {
                date = DateTime.Parse(month.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
                monthval = DateTime.Parse(date).Month.ToString();
                Year = DateTime.Parse(date).Year.ToString();
                monthvalue = monthval + Year.Substring(2, 2);
            }


            string SPName = "SPForDashboard";
            Hashtable ht = new Hashtable();
            ht.Add("@Option", "NetSalaries");
            ht.Add("@Department", Department);
            ht.Add("@month", monthvalue);
            ht.Add("@Empid", Empid);

            DataTable dt = config.ExecuteAdaptorAsyncWithParams(SPName, ht).Result;


            List<SalarySpecifications> Netsalaries = new List<SalarySpecifications>();
            foreach (DataRow dr in dt.Rows)
            {
                monthValue = (dr["monthname"].ToString());

                if (dr["Netpay"].ToString() != null && dr["Netpay"].ToString() != "")
                {
                    NetPayAmt = decimal.Parse(dr["Netpay"].ToString());
                }

                Netsalaries.Add(new SalarySpecifications(monthValue, NetPayAmt));
            }


            return Netsalaries;


        }

        [WebMethod]
        public static List<PaymentModeEmpCount> GetPaymodeEmpCount(string month, string Department)
        {
            AppConfiguration config = new AppConfiguration();

            string date = string.Empty;
            string monthval = "";
            string Year = "";
            string monthvalue = "";

            if (month.Trim().Length > 0)
            {
                date = DateTime.Parse(month.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
                monthval = DateTime.Parse(date).Month.ToString();
                Year = DateTime.Parse(date).Year.ToString();
                monthvalue = monthval + Year.Substring(2, 2);
            }


            string Paymentmode = "";
            int Ecount = 0;

            string SPName = "SPForDashboard";
            Hashtable ht = new Hashtable();
            ht.Add("@Option", "EmpPaymentModeCount");
            ht.Add("@month", monthvalue);
            ht.Add("@Department", Department);
            DataTable dt = config.ExecuteAdaptorAsyncWithParams(SPName, ht).Result;


            List<PaymentModeEmpCount> lstempcount = new List<PaymentModeEmpCount>();

            foreach (DataRow dr in dt.Rows)
            {
                Paymentmode = (dr["mode"].ToString());

                if (dr["val"].ToString() != null && dr["val"].ToString() != "")
                {
                    Ecount = int.Parse(dr["val"].ToString());
                }

                lstempcount.Add(new PaymentModeEmpCount(Paymentmode, Ecount));
            }


            return lstempcount;

        }

        [WebMethod]
        public static List<TotalEmpCount> TotalEmpCount( string Department)
        {
            AppConfiguration config = new AppConfiguration();


            string EmpLabel = "";
            int Ecount = 0;

            string SPName = "SPForDashboard";
            Hashtable ht = new Hashtable();
            ht.Add("@Option", "TotalEmpCount");
            ht.Add("@Department", Department);
            DataTable dt = config.ExecuteAdaptorAsyncWithParams(SPName, ht).Result;
            List<TotalEmpCount> lstempcount = new List<TotalEmpCount>();

            foreach (DataRow dr in dt.Rows)
            {
                EmpLabel = dr["Emplabel"].ToString();

                if (dr["EmpCount"].ToString() != null && dr["EmpCount"].ToString() != "")
                {
                    Ecount = int.Parse(dr["EmpCount"].ToString());
                }

                lstempcount.Add(new TotalEmpCount(EmpLabel, Ecount));
            }


            return lstempcount;

        }

        [WebMethod]
        public static List<HiredVsLeft> TotalHiredVsLeft(string month, string Department)
        {
            AppConfiguration config = new AppConfiguration();

            string date = string.Empty;
            string monthval = "";
            string Year = "";
            string monthvalue = "";

            if (month.Trim().Length > 0)
            {
                date = DateTime.Parse(month.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
                monthval = DateTime.Parse(date).Month.ToString();
                Year = DateTime.Parse(date).Year.ToString();
                monthvalue = monthval + Year.Substring(2, 2);
            }

            string EmpLabel = "";
            int value = 0;

            string SPName = "SPForDashboard";
            Hashtable ht = new Hashtable();
            ht.Add("@Option", "HiredVsLeft");
            ht.Add("@Department", Department);
            ht.Add("@Month", monthvalue);
            DataTable dt = config.ExecuteAdaptorAsyncWithParams(SPName, ht).Result;
            List<HiredVsLeft> lstempcount = new List<HiredVsLeft>();

            foreach (DataRow dr in dt.Rows)
            {
                EmpLabel = dr["Emplabel"].ToString();

                if (dr["Value"].ToString() != null && dr["Value"].ToString() != "")
                {
                    value = int.Parse(dr["Value"].ToString());
                }



                lstempcount.Add(new HiredVsLeft(EmpLabel, value));
            }


            return lstempcount;

        }

        [WebMethod]
        public static List<AgeLimitCount> TotalAgeLimitCount(string Department)
        {
            AppConfiguration config = new AppConfiguration();


            string Range = "";
            int Count = 0;

            string SPName = "SPForDashboard";
            Hashtable ht = new Hashtable();
            ht.Add("@Option", "AgeLimitCount");
            ht.Add("@Department", Department);
            DataTable dt = config.ExecuteAdaptorAsyncWithParams(SPName, ht).Result;
            List<AgeLimitCount> lstempcount = new List<AgeLimitCount>();

            foreach (DataRow dr in dt.Rows)
            {
                Range = dr["Range"].ToString();

                if (dr["Count"].ToString() != null && dr["Count"].ToString() != "")
                {
                    Count = int.Parse(dr["Count"].ToString());
                }



                lstempcount.Add(new AgeLimitCount(Range, Count));
            }


            return lstempcount;

        }

        [WebMethod]
        public static List<SalaryLimitCount> TotalSalaryLimitCount(string month, string Department)
        {
            AppConfiguration config = new AppConfiguration();

            string date = string.Empty;
            string monthval = "";
            string Year = "";
            string monthvalue = "";

            if (month.Trim().Length > 0)
            {
                date = DateTime.Parse(month.Trim(), CultureInfo.GetCultureInfo("en-gb")).ToString();
                monthval = DateTime.Parse(date).Month.ToString();
                Year = DateTime.Parse(date).Year.ToString();
                monthvalue = monthval + Year.Substring(2, 2);
            }

            string Range = "";
            decimal Count = 0;

            string SPName = "SPForDashboard";
            Hashtable ht = new Hashtable();
            ht.Add("@Option", "SalaryLimitCount");
            ht.Add("@Department", Department);
            ht.Add("@month", monthvalue);
            DataTable dt = config.ExecuteAdaptorAsyncWithParams(SPName, ht).Result;
            List<SalaryLimitCount> lstempcount = new List<SalaryLimitCount>();

            foreach (DataRow dr in dt.Rows)
            {
                Range = dr["Range"].ToString();

                if (dr["Count"].ToString() != null && dr["Count"].ToString() != "")
                {
                    Count = decimal.Parse(dr["Count"].ToString());
                }

                lstempcount.Add(new SalaryLimitCount(Range, Count));
            }


            return lstempcount;

        }

    }
}

public class EmpBioDataForm
{
    public string StrText { get; set; }
    public string StrValue { get; set; }

}

public class EmployeeDetails
{
    public string Name { get; set; }
    public string Design { get; set; }
    public string Image { get; set; }

}

public class DepEmpCountData
{
    public string Deptempcount { get; set; }
}

public class PocketfameAttData
{
    public string Total { get; set; }
    public string Present { get; set; }
    public string Absent { get; set; }
    public string NoSingleAttn { get; set; }

}

public class MaleFemaleCount
{
    public string Malecount { get; set; }
    public string Femalecount { get; set; }
}



public class PaysheetAllwDedSummary
{
    public string SalarySummary { get; set; }
    public decimal CurrentMonth { get; set; }
    public decimal PreviousMonth { get; set; }
    public decimal Per { get; set; }

}

public class Biodata
{
    public string text { get; set; }
    public string value { get; set; }
}

public class PaysheetSummary
{
    public string WageDetails { get; set; }
    public decimal Amount { get; set; }
}
public class SalaryLimitCount
{
    public SalaryLimitCount(string Rangen, decimal countn)
    {
        Range = Rangen;
        count = countn;
    }
    public decimal count { get; set; }
    public string Range { get; set; }
}

public class AgeLimitCount
{
    public AgeLimitCount(string Rangen, int countn)
    {
        Range = Rangen;
        count = countn;
    }
    public int count { get; set; }
    public string Range { get; set; }
}

public class HiredVsLeft
{
    public HiredVsLeft(string EmpLabel, int value)
    {
        Val = value;
        EmployeeLabel = EmpLabel;
    }
    public int Val { get; set; }
    public string EmployeeLabel { get; set; }

}
public class TotalEmpCount
{
    public TotalEmpCount(string EmpLabel, int empcountn)
    {
        EmpCount = empcountn;
        EmployeeLabel = EmpLabel;
    }
    public int EmpCount { get; set; }
    public string EmployeeLabel { get; set; }

}

public class PaymentModeEmpCount
{
    public PaymentModeEmpCount(string paymoden, int empcountn)
    {
        Paymode = paymoden;
        Empcount = empcountn;
    }
    public int Empcount { get; set; }
    public string Paymode { get; set; }
}

public class DepEmpCount
{
    public DepEmpCount(string Deptn, int empcountn)
    {
        Department = Deptn;
        Empcount = empcountn;
    }
    public int Empcount { get; set; }
    public string Department { get; set; }
}

public class SalarySpecifications
{
    public SalarySpecifications(string monthnames, decimal Netamt)
    {
        MonthnameValue = monthnames;
        NetPayAmt = Netamt;
    }

    public decimal NetPayAmt { get; set; }
    public string MonthnameValue { get; set; }

}