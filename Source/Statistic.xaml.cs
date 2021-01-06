using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;


namespace CakeShop
{
    /// <summary>
    /// Interaction logic for Statistic.xaml
    /// </summary>
    public partial class Statistic : Page
    {
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> FormatterY { get; set; }
        public Func<double, string> FormatterX { get; set; }

        public Statistic()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            #region Load Year combobox
            List<string> year = new List<string>();
            foreach (var bill in BillLists.Intance.Data)
            {
                year.Add(bill.DateCreate.Year.ToString());
            }
            year = year.Distinct().ToList();
            year.Sort();
            yearCb.ItemsSource = year;
            yearCb.Text = year[year.Count - 1];
            #endregion

            #region Load Chart
            LoadPieChart(LoadMonthBill(LoadYearBill()));
            LoadColumnChart(LoadYearBill());
            DataContext = this;
            #endregion
        }

        private void LoadPieChart(List<Bill> bills)
        {
            pieChart.Series.Clear();
            foreach (var cake in CakeList.Intance.Data)
            {
                foreach (var bill in bills)
                {
                    bool isIn = false;
                    if (cake.Name == bill.Cake)
                    { 
                        float revenue = bill.TotalPrice - bill.Quantity * cake.PurchasePrice;
                        foreach(var serie in pieChart.Series)
                        {
                            if(serie.Title == cake.Type)
                            {
                                revenue += float.Parse(serie.Values[0].ToString());
                                serie.Values = new ChartValues<float> { revenue };
                                isIn = true;
                                break;
                            }
                        }
                        if (!isIn)
                        {
                            pieChart.Series.Add(new PieSeries
                            {
                                Title = cake.Type,
                                Values = new ChartValues<float> { revenue }
                            });
                        }
                        
                    }
                }
            }
            
        }

        private void LoadColumnChart(List<Bill> bills)
        {
            List<float> values = new List<float>();
            for(int i = 1; i <= 12; ++i)
            {
                float sum = 0;

                foreach(var bill in bills)
                {
                    if(bill.DateCreate.Month == i)
                    {
                        float purchasePrice = 0;
                        foreach(var cake in CakeList.Intance.Data)
                        {
                            if(cake.Name == bill.Cake)
                            {
                                purchasePrice = cake.PurchasePrice;
                                break;
                            }
                        }
                        sum += bill.TotalPrice - bill.Quantity * purchasePrice;
                    }
                }
                values.Add(sum);
            }

            FormatterY = value => value.ToString("#,## VNĐ");

            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Total Price",
                    Values = new ChartValues<float>(values)
                }
            };
            Labels = new[] { "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12" };

            columnChart.Series = SeriesCollection;
           
        }

        private void Chart_OnDataClick(object sender, ChartPoint chartPoint)
        {
            var chart = chartPoint.ChartView as PieChart;
            foreach (PieSeries series in chart.Series)
            {
                series.PushOut = 0;
            }
            var selectedSeries = chartPoint.SeriesView as PieSeries;
            selectedSeries.PushOut = 15;
        }

        private void monthCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadPieChart(LoadMonthBill(LoadYearBill()));
        }

        private List<Bill> LoadYearBill()
        {
            string year = yearCb.SelectedItem as string;
            List<Bill> bills = new List<Bill>();

            foreach (var bill in BillLists.Intance.Data)
            {
                if (bill.DateCreate.Year.ToString() == year)
                {
                    bills.Add(bill);
                }
            }

            return bills;
        }

        private List<Bill> LoadMonthBill(List<Bill> yearBill)
        {
            ComboBoxItem monthCbi = (ComboBoxItem)monthCb.SelectedItem;
            List<Bill> bills = new List<Bill>();

            if (monthCbi == null)
            {
                bills = yearBill;

            }
            else
            {
                string month = monthCbi.Content.ToString();
                string[] res = month.Split(' ');

                if (res[1] == "cả")
                {
                    bills = yearBill;
                }
                else
                {
                    foreach (var bill in yearBill)
                    {
                        if (bill.DateCreate.Month.ToString() == res[1])
                        {
                            bills.Add(bill);
                        }
                    }
                }
            }
            
            return bills;
        }

        private void yearCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadColumnChart(LoadYearBill());
            LoadPieChart(LoadMonthBill(LoadYearBill()));
        }
    }
}
