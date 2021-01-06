using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Xml.Linq;

namespace CakeShop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    #region Assign essential classes
    public class Cake
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public string Image { get; set; }
        public BitmapImage BMPImg { get; set; }
        public float PurchasePrice { get; set; }
        public float SellPrice { get; set; }

        public Cake(XElement cake)
        {
            Id = int.Parse(cake.Element("Id").Value);
            Name = cake.Element("Name").Value;
            Type = cake.Element("Type").Value;
            DateAdded = DateTime.Parse(cake.Element("DateAdded").Value).Date;
            Description = cake.Element("Description").Value;
            Image = cake.Element("Image").Value;
            PurchasePrice = float.Parse(cake.Element("PurchasePrice").Value);
            SellPrice = float.Parse(cake.Element("SellPrice").Value);

            BMPImg = new BitmapImage();
            BMPImg.BeginInit();
            BMPImg.CacheOption = BitmapCacheOption.OnLoad;
            BMPImg.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + $@"Images\{Image}", UriKind.RelativeOrAbsolute);
            BMPImg.EndInit();
        }

        public Cake()
        {
            Id = -1;
            Name = "";
            Type = "";
            Description = "";
            DateAdded = new DateTime().Date;
            Image = "";
            BMPImg = new BitmapImage();
            PurchasePrice = 0;
            SellPrice = 0;
        }
    }

    public class CakeList
    {
        private static CakeList _instance = null;
        public ObservableCollection<Cake> Data { get; set; }

        public static CakeList Intance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CakeList();
                }
                return _instance;
            }
        }

        private CakeList()
        {
            Data = new ObservableCollection<Cake>();
            var DB = Database.Intance;
            var CakeList = DB.Data.Element("root").Element("CakeList").Elements();

            foreach (XElement cake in CakeList)
            {
                var cake_res = new Cake(cake);
                Data.Add(cake_res);
            }
        }

        public void Update()
        {
            Data.Clear();

            var DB = Database.Intance;
            var CakeList = DB.Data.Element("root").Element("CakeList").Elements();

            foreach (XElement cake in CakeList)
            {
                var cake_res = new Cake(cake);
                Data.Add(cake_res);
            }
        }
    }

    public class Bill
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Cake { get; set; }
        public int Quantity { get; set; }
        public float TotalPrice { get; set; }
        public DateTime DateCreate { get; set; }
        public string Status { get; set; }

        public Bill(XElement bill)
        {
            Id = int.Parse(bill.Element("Id").Value);
            Name = bill.Element("Name").Value;
            Phone = bill.Element("Phone").Value;
            Cake = bill.Element("Cake").Value;
            Quantity = int.Parse(bill.Element("Quantity").Value);
            TotalPrice = float.Parse(bill.Element("TotalPrice").Value);
            DateCreate = DateTime.Parse(bill.Element("DateCreate").Value).Date;
            Status = bill.Element("Status").Value;
        }

        public Bill()
        {
            Id = -1;
            Name = "";
            Phone = "";
            Cake = "";
            Quantity = 0;
            TotalPrice = 0;
            DateCreate = new DateTime().Date;
            Status = "Chưa hoàn thành";
        }
    }

    public class BillLists
    {
        private static BillLists _instance = null;
        public ObservableCollection<Bill> Data { get; set; }

        public static BillLists Intance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BillLists();
                }
                return _instance;
            }
        }

        private BillLists()
        {
            Data = new ObservableCollection<Bill>();
            var DB = Database.Intance;
            var billList = DB.Data.Element("root").Element("BillLists").Elements();

            foreach (XElement bill in billList)
            {
                var bill_res = new Bill(bill);
                Data.Add(bill_res);
            }
        }

        public void Update()
        {
            Data.Clear();

            var DB = Database.Intance;
            var billList = DB.Data.Element("root").Element("BillLists").Elements();

            foreach (XElement bill in billList)
            {
                var bill_res = new Bill(bill);
                Data.Add(bill_res);
            }
        }
    }

    public class Database
    {
        private static Database _instance = null;
        public XDocument Data { get; set; }

        public static Database Intance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Database();
                }
                return _instance;
            }
        }

        private Database()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string XMLpath = path + @"\Data\Database.xml";

            string XMLtext = File.ReadAllText(XMLpath);
            this.Data = new XDocument(XDocument.Parse(XMLtext));
        }
    }

    public class Flag
    {
        private static Flag _instance = null;
        public static Flag Intance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Flag();
                }
                return _instance;
            }
        }

        public bool OnAdd { get; set; }
        public bool OnUpdate { get; set; }
        public bool OnDetail { get; set; }

        private Flag()
        {
            OnUpdate = false;
            OnAdd = false;
            OnDetail = false;
        }
    }
    #endregion

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadIcon();
            LoadBackground();
            view.Navigate(HomeSingleton.Intance);
        }

        private void LoadIcon()
        {
            var BMPImg = new BitmapImage();
            BMPImg.BeginInit();
            BMPImg.CacheOption = BitmapCacheOption.OnLoad;
            BMPImg.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + $@"Images\Icon.png", UriKind.RelativeOrAbsolute);
            BMPImg.EndInit();
            icon.Source = BMPImg;
        }

        private void LoadBackground()
        {
            var BMPImg = new BitmapImage();
            BMPImg.BeginInit();
            BMPImg.CacheOption = BitmapCacheOption.OnLoad;
            BMPImg.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + $@"Images\Main.jpg", UriKind.RelativeOrAbsolute);
            BMPImg.EndInit();
            bg.ImageSource = BMPImg;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void InforButton_Click(object sender, RoutedEventArgs e)
        {
            var infor = InforSingleton.Intance;
            infor.ShowDialog();
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            path += "\\Data\\Database.xml";
            var isShow = bool.Parse(Database.Intance.Data.Element("root").Element("AppSetting").Element("ShowSplashScreen").Value);
            if (isShow)
            {
                var mb = MessageBox.Show("Do you want to turn off Splash Screen?", "Splash screen Setting", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (mb == MessageBoxResult.No)
                {
                    return;
                }
                Database.Intance.Data.Element("root").Element("AppSetting").Element("ShowSplashScreen").Value = "false";
            }
            else
            {
                var mb = MessageBox.Show("Do you want to turn on Splash Screen?", "Splash screen Setting", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (mb == MessageBoxResult.No)
                {
                    return;
                }
                Database.Intance.Data.Element("root").Element("AppSetting").Element("ShowSplashScreen").Value = "true";
            }
            File.WriteAllText(path, Database.Intance.Data.ToString());
        }

        private void HomePage_Click(object sender, RoutedEventArgs e)
        {
            var homePage = HomeSingleton.Intance;
            view.Navigate(homePage);
        }

        private void BillList_Click(object sender, RoutedEventArgs e)
        {
            var billListPage = BillListSingleton.Intance;
            view.Navigate(billListPage);
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            var statisticPage = StatisticSingleton.Intance;
            view.Navigate(statisticPage);
        }

        #region Assign Singleton Classes
        public class HomeSingleton
        {
            private static Home _instance = null;
            public static Home Intance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new Home();
                    }
                    return _instance;
                }
            }

            private HomeSingleton()
            {
                //do nothing
            }
        }

        public class BillListSingleton
        {
            private static BillList _instance = null;
            public static BillList Intance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new BillList();
                    }
                    return _instance;
                }
            }

            private BillListSingleton()
            {
                //do nothing
            }
        }

        public class StatisticSingleton
        {
            private static Statistic _instance = null;
            public static Statistic Intance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new Statistic();
                    }
                    return _instance;
                }
            }

            private StatisticSingleton()
            {
                //do nothing
            }
        }

        public class InforSingleton
        {
            private static Infor _instance = null;
            public static Infor Intance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new Infor();
                    }
                    return _instance;
                }
            }

            private InforSingleton()
            {
                //do nothing
            }
        }
        #endregion
    }
}
