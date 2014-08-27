using CloudPanel.classes;
using CloudPanel.Modules.Reporting;
using CloudPanel.Modules.Settings;
using log4net;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CloudPanel.reporting
{
    public partial class reports : System.Web.UI.Page
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {

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

        /// <summary>
        /// Runs the Exchange report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRunExchangeReport_Click(object sender, EventArgs e)
        {
            try
            {
                dummyReportViewer.LocalReport.ReportEmbeddedResource = "CloudPanel.reporting.RDLC.ExchangeReport.rdlc";
                dummyReportViewer.LocalReport.EnableExternalImages = true;

                // Get Data
                BindingList<ExchangeDetails> data = GetReportData.Get_ExchangeDetails();
                BindingList<TotalStats> stats = GetReportData.Get_TotalStatistics();

                // Create our data source with the data we retrieved
                ReportDataSource rds = new ReportDataSource("ExchangeDataset", data);
                ReportDataSource rds2 = new ReportDataSource("TotalsDataset", stats);

                // Bind and report our export
                dummyReportViewer.LocalReport.DataSources.Clear();
                dummyReportViewer.LocalReport.DataSources.Add(rds);
                dummyReportViewer.LocalReport.DataSources.Add(rds2);
                dummyReportViewer.LocalReport.Refresh();

                // Export to PDF
                Export("ExchangeReport_" + DateTime.Now.ToShortDateString() + ".pdf", dummyReportViewer.LocalReport.Render("PDF"));

                // Clear
                data = null;
                rds = null;

                // Update notification
                notification1.SetMessage(controls.notification.MessageType.Success, "The Exchange Report should be available for you to download. Please check your browser for any popups about downloading a file.");
            }
            catch (Exception ex)
            {
                // FATAL //
                this.logger.Fatal("Error running the Exchange report.", ex);

                notification1.SetMessage(controls.notification.MessageType.Error, "Error running report: " + ex.ToString());
            }
        }

        /// <summary>
        /// Runs the Citrix report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRunCitrixReport_Click(object sender, EventArgs e)
        {
            try
            {
                dummyReportViewer.LocalReport.ReportEmbeddedResource = "CloudPanel.reporting.RDLC.CitrixReport.rdlc";
                dummyReportViewer.LocalReport.EnableExternalImages = true;

                // Get Data
                BindingList<CitrixDetails> data = GetReportData.Get_CitrixDetails();
                BindingList<TotalStats> stats = GetReportData.Get_TotalStatistics();

                // Create our data source with the data we retrieved
                ReportDataSource rds = new ReportDataSource("CitrixDataset", data);
                ReportDataSource rds2 = new ReportDataSource("TotalsDataset", stats);

                // Bind and report our export
                dummyReportViewer.LocalReport.DataSources.Clear();
                dummyReportViewer.LocalReport.DataSources.Add(rds);
                dummyReportViewer.LocalReport.DataSources.Add(rds2);
                dummyReportViewer.LocalReport.Refresh();

                // Export to PDF
                Export("CitrixReport_" + DateTime.Now.ToShortDateString() + ".pdf", dummyReportViewer.LocalReport.Render("PDF"));

                // Clear
                data = null;
                rds = null;

                // Update notification
                notification1.SetMessage(controls.notification.MessageType.Success, "The Citrix Report should be available for you to download. Please check your browser for any popups about downloading a file.");
            }
            catch (Exception ex)
            {
                // FATAL //
                this.logger.Fatal("Error running the Citrix report.", ex);

                notification1.SetMessage(controls.notification.MessageType.Error, "Error running report: " + ex.ToString());
            }
        }

        private static string ConvertImageToBase64(System.Drawing.Image image, ImageFormat format)
        {
            byte[] imageArray;

            using (System.IO.MemoryStream imageStream = new System.IO.MemoryStream())
            {
                image.Save(imageStream, format);
                imageArray = new byte[imageStream.Length];
                imageStream.Seek(0, System.IO.SeekOrigin.Begin);
                imageStream.Read(imageArray, 0, imageArray.Length);
            }

            return Convert.ToBase64String(imageArray);
        }
    }
}