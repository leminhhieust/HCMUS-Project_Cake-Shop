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

namespace CakeShop
{
    /// <summary>
    /// Interaction logic for Detail.xaml
    /// </summary>
    public partial class Detail : Window
    {
        public Cake currentCake { get; set; }

        public Detail()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = currentCake;
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            Flag.Intance.OnDetail = false;
            this.Close();
        }

        private void delBtn_Click(object sender, RoutedEventArgs e)
        {
            #region Alert
            var MessageBoxBtn = MessageBox.Show("Do you really want to delete this cake?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (MessageBoxBtn == MessageBoxResult.No)
            {
                return;
            }
            #endregion

            #region Delelte Database
            var cakelist = Database.Intance.Data.Root.Element("CakeList").Elements();
            foreach (var cake in cakelist)
            {
                if (int.Parse(cake.Element("Id").Value) == currentCake.Id)
                {
                    cake.Remove();
                }
            }
            #endregion

            #region Delete Image
            var SystemPath = AppDomain.CurrentDomain.BaseDirectory + $@"Images\";
            File.Delete(SystemPath + currentCake.Image);
            #endregion

            #region Delete bill
            var billlist = Database.Intance.Data.Root.Element("BillLists").Elements();
            foreach (var bill in billlist)
            {
                if (bill.Element("Cake").Value == currentCake.Name)
                {
                    bill.Remove();
                }
            } 
            #endregion

            #region Update Database
            CakeList.Intance.Update();
            BillLists.Intance.Update();
            WriteDownDatabase();
            #endregion

            Flag.Intance.OnDetail = false;
            this.Close();
        }

        private void WriteDownDatabase()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            path += @"\Data\Database.xml";

            File.WriteAllText(path, Database.Intance.Data.ToString());
        }
    }
}
