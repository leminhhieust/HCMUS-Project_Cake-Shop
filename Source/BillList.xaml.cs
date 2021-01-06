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

namespace CakeShop
{
    /// <summary>
    /// Interaction logic for BillList.xaml
    /// </summary>
    public partial class BillList : Page
    {
        public BillList()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            check.Visibility = Visibility.Hidden;
            lvBill.ItemsSource = BillLists.Intance.Data;
        }

        private void lvBill_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Bill bill = lvBill.SelectedItem as Bill;
            UpdateBtn.IsEnabled = true;
            CancelBtn.IsEnabled = true;

            if (bill != null)
            {
                if (bill.Status == "Chưa hoàn thành")
                {
                    CompleteBtn.IsEnabled = true;
                }
                else
                {
                    CompleteBtn.IsEnabled = false;
                }
            }
            
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            #region Alert
            var MessageBoxBtn = MessageBox.Show("Do you really want to cancel this bill?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (MessageBoxBtn == MessageBoxResult.No)
            {
                return;
            }
            #endregion

            #region Delete Database
            Bill currentBill = lvBill.SelectedItem as Bill;
            var billList = Database.Intance.Data.Root.Element("BillLists").Elements();
            foreach (var bill in billList)
            {
                if (int.Parse(bill.Element("Id").Value) == currentBill.Id)
                {
                    bill.Remove();
                }
            }
            #endregion

            #region Update Database
            BillLists.Intance.Update();
            lvBill.ItemsSource = BillLists.Intance.Data;
            WriteDownDatabase();
            #endregion
        }

        private void CompleteBtn_Click(object sender, RoutedEventArgs e)
        {
            #region Alert
            var MessageBoxBtn = MessageBox.Show("Do you really want to complete this bill?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (MessageBoxBtn == MessageBoxResult.No)
            {
                return;
            }
            #endregion

            #region Update Status
            Bill currentBill = lvBill.SelectedItem as Bill;
            var billList = Database.Intance.Data.Root.Element("BillLists").Elements();
            CompleteBtn.IsEnabled = false;

            foreach (var bill in billList)
            {
                if (int.Parse(bill.Element("Id").Value) == currentBill.Id)
                {
                    bill.Element("Status").Value = "Đã hoàn thành";
                }
            }

            #endregion

            #region Update Database
            BillLists.Intance.Update();
            lvBill.ItemsSource = BillLists.Intance.Data;
            WriteDownDatabase();
            #endregion
        }

        private void WriteDownDatabase()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            path += @"\Data\Database.xml";

            File.WriteAllText(path, Database.Intance.Data.ToString());
        }

        private void CreateUpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            Button currentBtn = sender as Button;
            var addBill = new AddBill();
            if (currentBtn.Name == "CreateBtn")
            {
                Flag.Intance.OnAdd = true;
                Flag.Intance.OnUpdate = false;
            }
            else if (currentBtn.Name == "UpdateBtn")
            {
                Flag.Intance.OnUpdate = true;
                Flag.Intance.OnAdd = false;
                addBill.currentBill = lvBill.SelectedItem as Bill;
            }

            addBill.ShowDialog();
            while (Flag.Intance.OnAdd)
            {
                //wait
            }
            lvBill.ItemsSource = BillLists.Intance.Data;
        }

        private void StatusCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem status = (ComboBoxItem)StatusCb.SelectedItem;
            string content = status.Content.ToString();
            ObservableCollection<Bill> newlist = new ObservableCollection<Bill>();
 
            if(content == "Đã hoàn thành")
            {
                foreach(var bill in BillLists.Intance.Data)
                {
                    if(bill.Status == "Đã hoàn thành")
                    {
                        newlist.Add(bill);
                    }
                }
            }
            else if(content == "Chưa hoàn thành")
            {
                foreach (var bill in BillLists.Intance.Data)
                {
                    if (bill.Status == "Chưa hoàn thành")
                    {
                        newlist.Add(bill);
                    }
                }
            }
            else
            {
                newlist = BillLists.Intance.Data;
            }

            if(newlist.Count == 0)
            {
                check.Visibility = Visibility.Visible;
            }
            else
            {
                check.Visibility = Visibility.Hidden;
            }

            lvBill.ItemsSource = newlist;
        }
    }
}
