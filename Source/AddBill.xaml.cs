using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Xml.Linq;

namespace CakeShop
{
    /// <summary>
    /// Interaction logic for AddBill.xaml
    /// </summary>
    public partial class AddBill : Window
    {
        public Bill currentBill { get; set; }

        public AddBill()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region Check Update or Add
            if (Flag.Intance.OnAdd)
            {
                currentBill = new Bill();
                currentBill.DateCreate = DateTime.Now.Date;
            }
            else if (Flag.Intance.OnUpdate)
            {
                Title.Text = "CẬP NHẬT ĐƠN HÀNG";
                TypeCb.Text = currentBill.Status;
                CakeCb.SelectedItem = currentBill.Cake;
            }
            #endregion 
            LoadCakeCb();
            this.DataContext = currentBill;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var MessageBoxBtn = MessageBox.Show("If you close this window, all data will be lost.", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (MessageBoxBtn == MessageBoxResult.Cancel)
            {
                return;
            }
            Flag.Intance.OnAdd = false;
            Flag.Intance.OnUpdate = false;
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var billlist = Database.Intance.Data.Root.Element("BillLists").Elements();

            #region Check current ID
            int currentID = 0;
            if (Flag.Intance.OnUpdate)
            {
                currentID = currentBill.Id;
            }
            else
            {
                for (int i = 0; i <= BillLists.Intance.Data.Count; ++i)
                {
                    bool isIn = false;

                    foreach (var bill in BillLists.Intance.Data)
                    {
                        if (i == bill.Id)
                        {
                            isIn = true;
                            break;
                        }
                    }

                    if (!isIn)
                    {
                        currentID = i;
                        break;
                    }
                }
            }
            #endregion

            #region Delete to update
            foreach (var bill in billlist)
            {
                if (int.Parse(bill.Element("Id").Value) == currentBill.Id)
                {
                    bill.Remove();
                }
            }
            #endregion

            #region Create new Bill in database
            ComboBoxItem ComboItem = (ComboBoxItem)TypeCb.SelectedItem;
            string cakeName = CakeCb.SelectedItem as string;
            Database.Intance.Data.Root.Element("BillLists").Add(new XElement("Bill",
                                                        new XElement("Id", currentID),
                                                        new XElement("Name", currentBill.Name),
                                                        new XElement("Phone", currentBill.Phone),
                                                        new XElement("Cake", cakeName),
                                                        new XElement("Quantity", currentBill.Quantity),
                                                        new XElement("TotalPrice", currentBill.TotalPrice),
                                                        new XElement("DateCreate",DateCreate.Text),
                                                        new XElement("Status", ComboItem.Content)
                                                          ));
            #endregion

            #region Update database
            BillLists.Intance.Update();
            WriteDownDatabase();
            #endregion

            #region Alert
            if (Flag.Intance.OnAdd)
            {
                MessageBox.Show("Thêm thành công!");
            }
            else
            {
                MessageBox.Show("Cập nhật thành công!");
            }
            #endregion

            #region Update Flag
            Flag.Intance.OnUpdate = false;
            Flag.Intance.OnAdd = false;
            #endregion

            this.Close();
        }

        private void WriteDownDatabase()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            path += @"\Data\Database.xml";

            File.WriteAllText(path, Database.Intance.Data.ToString());
        }

        private void LoadCakeCb()
        {
            List<string> Cake = new List<string>();

            foreach (var cake in CakeList.Intance.Data)
            {
                Cake.Add(cake.Name);
            }

            CakeCb.ItemsSource = Cake;
        }

        private void CountPrice()
        {
            string cakeName = CakeCb.SelectedItem as string;
            int quantity = 0;
            if (Quantity.Text != "")
            {
                quantity = int.Parse(Quantity.Text);
            }
            float price = 0;

            if (cakeName != null)
            {
                foreach (var cake in CakeList.Intance.Data)
                {
                    if (cake.Name == cakeName)
                    {
                        price = cake.SellPrice;
                    }
                }
            }

            totalprice.Text = (quantity * price).ToString("#,##");
            currentBill.TotalPrice = quantity * price;
        }

        private void Quantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            CountPrice();
        }

        private void CakeCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CountPrice();
        }
    }
}
