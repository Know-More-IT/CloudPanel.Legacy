using CloudPanel.classes;
using CloudPanel.Modules.Base;
using CloudPanel.Modules.Base.Class;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Sql;
using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Options;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace CloudPanel
{
    public partial class dashboard : System.Web.UI.Page
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetStatistics();

                // Gets the line chart for super admins
                if (Authentication.IsSuperAdmin)
                {
                    GetSuperAdminLineChart();

                    // Only load database chart if Exchange statistics is enabled or not
                    if (Master.ExchangeStatistics)
                        GetSuperAdminDatabaseSizesChart();
                }

                // Get the bar chart for super admins and reseller admins
                if (Authentication.IsSuperAdmin || Authentication.IsResellerAdmin)
                {
                    GetSuperResellerAdminBarChart();
                }
            }
        }


        /// <summary>
        /// Gets the line chart for super admins
        /// </summary>
        private void GetSuperAdminLineChart()
        {
            try
            {
                // Get our users, Exchange, and Citrix values
                List<Dictionary<string, object>> theValuesList = SQLStatistics.GetSuperAdminLineChart();

                // Merge the moneys
                Dictionary<string, object> values = theValuesList[0].Concat(theValuesList[1]).Concat(theValuesList[2]).GroupBy(d => d.Key).ToDictionary(d => d.Key, d => d.First().Value);

                // Create our Chart
                Highcharts lineChart = new Highcharts("lineChart");
                lineChart.SetCredits(new Credits() { Enabled = false });
                lineChart.SetTitle(new DotNet.Highcharts.Options.Title() { Text = Resources.LocalizedText.Dashboard_HistoryChart });
                lineChart.SetLegend(new Legend()
                {
                    Align = DotNet.Highcharts.Enums.HorizontalAligns.Right,
                    Layout = DotNet.Highcharts.Enums.Layouts.Vertical,
                    VerticalAlign = DotNet.Highcharts.Enums.VerticalAligns.Middle,
                    BorderWidth = 0
                });

                // Create our XAXIS
                XAxis axis = new XAxis();
                axis.Categories = values.Keys.ToArray();

                // Our series collection
                List<Series> seriesCollection = new List<Series>();

                // Create our series for Users
                Series seriesUsers = new Series();
                seriesUsers.Data = new DotNet.Highcharts.Helpers.Data(theValuesList[0].Values.ToArray());
                seriesUsers.Name = Resources.LocalizedText.Global_Users;
                seriesUsers.Color = Color.Green;
                seriesCollection.Add(seriesUsers);

                // Create our series for Exchange
                Series seriesExch = new Series();
                seriesExch.Data = new DotNet.Highcharts.Helpers.Data(theValuesList[1].Values.ToArray());
                seriesExch.Name = Resources.LocalizedText.Global_Mailboxes;
                seriesExch.Color = Color.Blue;
                seriesCollection.Add(seriesExch);

                // Create our series for Citrix
                if (Config.CitrixEnabled)
                {
                    Series seriesCitrix = new Series();
                    seriesCitrix.Data = new DotNet.Highcharts.Helpers.Data(theValuesList[2].Values.ToArray());
                    seriesCitrix.Name = Resources.LocalizedText.Global_Citrix;
                    seriesCitrix.Color = Color.Red;
                    seriesCollection.Add(seriesCitrix);
                }
                
                // Add our series and axis to the chart
                lineChart.SetXAxis(axis);
                lineChart.SetSeries(seriesCollection.ToArray());

                // Show the chart
                ltrChart.Text = lineChart.ToHtmlString();
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Warning, ex.Message);
            }
        }

        /// <summary>
        /// Gets the bar chart
        /// </summary>
        private void GetSuperResellerAdminBarChart()
        {
            try
            {
                BaseLargestCustomers largest = null;

                if (Authentication.IsResellerAdmin)
                    largest = SQLStatistics.GetLargestCustomers(true, CPContext.SelectedResellerCode);
                else
                    largest = SQLStatistics.GetLargestCustomers(false, null);

                // Create our Chart
                Highcharts barChart = new Highcharts("barChart")
                    .InitChart(new Chart
                    {
                         DefaultSeriesType =  ChartTypes.Bar
                    })
                    .SetTitle(new Title
                    {
                        Text = Resources.LocalizedText.Dashboard_Top5LargestCustomers
                    })
                    .SetCredits(new Credits
                    {
                        Enabled = false
                    })
                    .SetXAxis(new XAxis
                    {
                        Categories = largest.CompanyNames.ToArray()
                    })
                    .SetYAxis(new YAxis
                    {
                        Title = new YAxisTitle { Text = Resources.LocalizedText.Global_Count },
                        PlotLines = new[]
                        {
                            new YAxisPlotLines
                            {
                                Value = 0,
                                Width = 1
                            }
                        }
                    })
                    .SetLegend(new Legend
                    {
                        Layout = Layouts.Vertical,
                        Align = HorizontalAligns.Right,
                        VerticalAlign = VerticalAligns.Top,
                        BorderWidth = 0
                    })
                    .SetPlotOptions(new PlotOptions()
                    {
                        Bar = new PlotOptionsBar()
                        {
                            DataLabels = new PlotOptionsBarDataLabels()
                            {
                                Enabled = true,
                                Style = "fontWeight: 'bold'"
                            }
                        }
                    })
                    .SetSeries(new[]
                    {
                        new Series { Name = Resources.LocalizedText.Global_Mailboxes, Data = new DotNet.Highcharts.Helpers.Data(largest.MailboxCount.ToArray()) },
                        new Series { Name = Resources.LocalizedText.Global_Users, Data = new DotNet.Highcharts.Helpers.Data(largest.UserCount.ToArray()) },
                    }
              );

              ltrBar.Text = barChart.ToHtmlString();
                
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Warning, ex.Message);
            }
        }

        /// <summary>
        /// Gets the colum chart for the database sizes
        /// </summary>
        private void GetSuperAdminDatabaseSizesChart()
        {
            try
            {
                // Get our users, Exchange, and Citrix values
                List<MailboxDatabase> mdbData = SQLStatistics.GetSuperAdminDbSizeChart();

                // Our database names
                var dbNames = from m in mdbData orderby m.DatabaseSize select m.Identity;

                // Our sizes
                var dbSizes = from m in mdbData orderby m.DatabaseSize select (object)ConvertToGB(m.DatabaseSize);

                // Create our Chart
                Highcharts columnChart = new Highcharts("mdbBarChart")
                    .SetOptions(new DotNet.Highcharts.Helpers.GlobalOptions()
                    {
                        Lang = new DotNet.Highcharts.Helpers.Lang()
                        {
                            DecimalPoint = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator,
                            ThousandsSep = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator
                        }
                    })
                    .InitChart(new Chart
                    {
                        DefaultSeriesType = ChartTypes.Column
                    })
                    .SetTitle(new Title
                    {
                        Text = Resources.LocalizedText.Dashboard_ExchangeDatabases
                    })
                    .SetCredits(new Credits
                    {
                        Enabled = false
                    })
                    .SetXAxis(new XAxis
                    {
                        Categories = dbNames.ToArray()
                    })
                    .SetYAxis(new YAxis
                    {
                        Title = new YAxisTitle { Text = Resources.LocalizedText.Dashboard_SizeInGigabytes },
                        Labels = new YAxisLabels() { Enabled = true },
                        PlotLines = new[]
                        {
                            new YAxisPlotLines
                            {
                                Value = 0,
                                Width = 1
                            }
                        }
                    })
                    .SetLegend(new Legend
                    {
                        Enabled = false
                    })
                    .SetPlotOptions(new PlotOptions()
                    {
                        Column = new PlotOptionsColumn()
                        {
                            DataLabels = new PlotOptionsColumnDataLabels()
                            {
                                Enabled = true,
                                Style = "fontWeight: 'bold'"
                            }
                        }
                    })
                    .SetSeries(new[]
                    {
                        new Series { Name = "GB", Data = new DotNet.Highcharts.Helpers.Data(dbSizes.ToArray()) }
                    }
                );

                ltrDbSize.Text = columnChart.ToHtmlString();
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Warning, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets the SuperAdmin statistics over the entire environment
        /// </summary>
        private void GetStatistics()
        {
            SQLStatistics stats = new SQLStatistics();

            try
            {
                BaseDashboard dashboardStats = null;
                string exchSize = "0MB";

                if (Authentication.IsSuperAdmin)
                {
                    dashboardStats = stats.GetDashboardStatistics(true, false, null);
                    exchSize = stats.GetTotalAssignedExchangeStorage(true, false, false, null);
                }
                else if (Authentication.IsResellerAdmin)
                {
                    dashboardStats = stats.GetDashboardStatistics(false, true, CPContext.SelectedResellerCode);
                    exchSize = stats.GetTotalAssignedExchangeStorage(false, true, false, CPContext.SelectedResellerCode);

                    // Load the reseller admin bar chart
                    GetCompanyAdminBarChart(dashboardStats.UserCount, dashboardStats.MailboxCount, dashboardStats.CitrixCount, dashboardStats.DomainCount, dashboardStats.AcceptedDomainCount);
                
                }
                else
                {
                    dashboardStats = stats.GetDashboardStatistics(false, false, CPContext.SelectedCompanyCode);
                    exchSize = stats.GetTotalAssignedExchangeStorage(false, false, true, CPContext.SelectedCompanyCode);

                    // Load the company admin bar chart
                    GetCompanyAdminBarChart(dashboardStats.UserCount, dashboardStats.MailboxCount, dashboardStats.CitrixCount, dashboardStats.DomainCount, dashboardStats.AcceptedDomainCount);
                }

                lbStatisticsTotalUsers.Text = dashboardStats.UserCount.ToString();
                lbStatisticsTotalMailboxes.Text = dashboardStats.MailboxCount.ToString();
                lbStatisticsTotalCompanies.Text = dashboardStats.CompanyCount.ToString();
                lbStatisticsTotalCitrixUsers.Text = dashboardStats.CitrixCount.ToString();
                lbStatisticsDomains.Text = dashboardStats.DomainCount.ToString();
                lbStatisticsAcceptedDomains.Text = dashboardStats.AcceptedDomainCount.ToString();
                
                // Set total exchange size
                lbStatisticsTotalExchangeStorage.Text = exchSize;
               
            }
            catch (Exception ex)
            {
                // FATAL //
                this.logger.Fatal("Error retrieving dashboard statistics.", ex);

                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Gets the bar chart for Company Administrators
        /// </summary>
        /// <param name="userCount"></param>
        /// <param name="mailboxCount"></param>
        /// <param name="citrixCount"></param>
        public void GetCompanyAdminBarChart(int userCount, int mailboxCount, int citrixCount, int domainCount, int acceptedDomainCount)
        {
            try
            {
                // Create our Chart
                Highcharts barChart = new Highcharts("barChartCompanyAdmin")
                    .InitChart(new Chart
                    {
                        DefaultSeriesType = ChartTypes.Column
                    })
                    .SetTitle(new Title
                    {
                        Text = Resources.LocalizedText.Global_Overview
                    })
                    .SetCredits(new Credits
                    { 
                        Enabled = false
                    })
                    .SetXAxis(new XAxis
                    {
                        Categories = new string[] { Resources.LocalizedText.Global_Users, Resources.LocalizedText.Global_Mailboxes, Resources.LocalizedText.Global_Citrix, Resources.LocalizedText.Global_Domains, Resources.LocalizedText.Global_AcceptedDomains }
                    })
                    .SetYAxis(new YAxis
                    {
                        Title = new YAxisTitle { Text = Resources.LocalizedText.Global_Count },
                        PlotLines = new[]
                        {
                            new YAxisPlotLines
                            {
                                Value = 0,
                                Width = 1
                            }
                        }
                    })
                    .SetLegend(new Legend
                    {
                        Layout = Layouts.Vertical,
                        Align = HorizontalAligns.Right,
                        VerticalAlign = VerticalAligns.Top,
                        BorderWidth = 0
                    })
                    .SetSeries(new[]
                    {
                        new Series { Name = Resources.LocalizedText.Global_Statistics, Data = new DotNet.Highcharts.Helpers.Data(new object[] { userCount, mailboxCount, citrixCount, domainCount, acceptedDomainCount }) }
                    }
              );

                ltrChart.Text = barChart.ToHtmlString();

            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Warning, ex.Message);
            }
        }

        /// <summary>
        /// Gets the bar chart for Reseller Admins
        /// </summary>
        /// <param name="userCount"></param>
        /// <param name="mailboxCount"></param>
        /// <param name="citrixCount"></param>
        public void GetResellerAdminBarChart(int userCount, int mailboxCount, int citrixCount, int domainCount, int acceptedDomainCount)
        {
            try
            {
                // Create our Chart
                Highcharts barChart = new Highcharts("barChartResellerAdmin")
                    .InitChart(new Chart
                    {
                        DefaultSeriesType = ChartTypes.Column
                    })
                    .SetTitle(new Title
                    {
                        Text = Resources.LocalizedText.Global_Overview
                    })
                    .SetCredits(new Credits
                    {
                        Enabled = false
                    })
                    .SetXAxis(new XAxis
                    {
                        Categories = new string[] { Resources.LocalizedText.Global_Users, Resources.LocalizedText.Global_Mailboxes, Resources.LocalizedText.Global_Citrix, Resources.LocalizedText.Global_Domains, Resources.LocalizedText.Global_AcceptedDomains }
                    })
                    .SetYAxis(new YAxis
                    {
                        Title = new YAxisTitle { Text = Resources.LocalizedText.Global_Count },
                        PlotLines = new[]
                        {
                            new YAxisPlotLines
                            {
                                Value = 0,
                                Width = 1
                            }
                        }
                    })
                    .SetLegend(new Legend
                    {
                        Layout = Layouts.Vertical,
                        Align = HorizontalAligns.Right,
                        VerticalAlign = VerticalAligns.Top,
                        BorderWidth = 0
                    })
                    .SetSeries(new[]
                    {
                        new Series { Name = Resources.LocalizedText.Global_Statistics, Data = new DotNet.Highcharts.Helpers.Data(new object[] { userCount, mailboxCount, citrixCount, domainCount, acceptedDomainCount }) }
                    }
              );

                ltrChart.Text = barChart.ToHtmlString();

            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Warning, ex.Message);
            }
        }

        /// <summary>
        /// Converts the size from TB,GB,MB to KB
        /// </summary>
        /// <param name="size"></param>
        /// <param name="sizeType"></param>
        /// <returns></returns>
        private object ConvertToGB(string size)
        {
            string newSize = string.Format(CultureInfo.InvariantCulture, "{0:0.00}", (decimal.Parse(size, CultureInfo.InvariantCulture) / 1024) / 1024);
            return newSize;
        }
    }
}