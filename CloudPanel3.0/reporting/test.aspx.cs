using CloudPanel.Modules.Reporting;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CloudPanel.reporting
{
    public partial class test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ExchangePlansByCompany();
        }

        private void ExchangePlansByCompany()
        {
            try
            {
                ReportViewer1.LocalReport.ReportPath = @"reporting/ExchangePlanPerCompany.rdlc";

                ReportDataSource rds = new ReportDataSource("ReportData", GetReportData.GetExchangePlanPerCompany());

                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(rds);
                ReportViewer1.LocalReport.Refresh();

                Export("ExchangePlans_" + DateTime.Now.ToShortDateString() + ".pdf", ReportViewer1.LocalReport.Render("PDF"));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Exports to Excel
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="bytes"></param>
        private void Export(string filename, byte[] bytes)
        {
            string attachment = "attachment; filename=" + filename;

            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.ms-excel";

            Response.BinaryWrite(bytes);

            Response.End();
        }
    }
}