using Microsoft.Win32;
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
    /// Interaction logic for Add.xaml
    /// </summary>

    public partial class SelectedImage
    {
        public string Path { get; set; }
        public string Name { get; set; }

        public SelectedImage()
        {
            //Do nothing
        }
    }

    public partial class Add : Window
    {
        public SelectedImage CakeImage { get; set; }
        public Cake currentCake { get; set; }

        public Add()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region Check Update or Add
            CakeImage = new SelectedImage();
            if (Flag.Intance.OnAdd)
            {
                currentCake = new Cake();
                currentCake.DateAdded = DateTime.Now.Date;
            }
            else if(Flag.Intance.OnUpdate)
            {
                Title.Text = "CẬP NHẬT SẢN PHẨM";
                var SystemPath = AppDomain.CurrentDomain.BaseDirectory + $@"Images\";

                CakeImage.Name = currentCake.Image;
                CakeImage.Path = SystemPath + CakeImage.Name;
                TypeCb.Text = currentCake.Type;
            }
            #endregion 

            this.DataContext = currentCake;
        }

        private string GetFileName(string path)
        {
            char delim = '\\';
            var tokens = path.Split(delim);

            string result = tokens[tokens.Count() - 1];

            return result;
        }

        private string GetFileExtension(string name)
        {
            char delim = '.';

            var tokens = name.Split(delim);
            string result = "." + tokens[tokens.Count() - 1];

            return result;
        }

        private BitmapImage LoadImage(string Path)
        {
            var BMPImg = new BitmapImage();
            BMPImg.BeginInit();
            BMPImg.CacheOption = BitmapCacheOption.OnLoad;
            BMPImg.UriSource = new Uri(Path, UriKind.RelativeOrAbsolute);
            BMPImg.EndInit();
            return BMPImg;
        }

        private void BrowseImage(object sender, RoutedEventArgs e)
        {
            var openfileDialog = new OpenFileDialog
            {
                Title = "Pick an image"
            };

            if (openfileDialog.ShowDialog() == true)
            {
                CakeImage.Path = openfileDialog.FileName;
                CakeImage.Name = GetFileName(CakeImage.Path);
                currentCake.BMPImg = LoadImage(CakeImage.Path);
                CakeImg.Source = currentCake.BMPImg;
            }
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
            var cakelist = Database.Intance.Data.Root.Element("CakeList").Elements();
            var SystemPath = AppDomain.CurrentDomain.BaseDirectory + $@"Images\";

            #region Check current ID
            int currentID = 0;
            if (Flag.Intance.OnUpdate)
            {
                currentID = currentCake.Id;
            }
            else
            {
                for (int i = 0; i <= CakeList.Intance.Data.Count; ++i)
                {
                    bool isIn = false;

                    foreach (var cake in CakeList.Intance.Data)
                    {
                        if (i == cake.Id)
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
            foreach (var cake in cakelist)
            {
                if (int.Parse(cake.Element("Id").Value) == currentCake.Id)
                {
                    cake.Remove();
                }
            } 
            #endregion

            #region Handling Cake's Image
            if (CakeImage.Name != null)
            {
                if (File.Exists(SystemPath + currentCake.Image) && (SystemPath + currentCake.Image) != CakeImage.Path)
                {
                    File.Delete(SystemPath + currentCake.Image);
                }

                if (!File.Exists(SystemPath + currentCake.Image))
                {
                    var extension = GetFileExtension(CakeImage.Name);
                    CakeImage.Name = $"{currentID}{extension}";
                    var DestinationPath = SystemPath + CakeImage.Name;
                    File.Copy(CakeImage.Path, DestinationPath);
                }
            }
            #endregion

            #region Create new Cake in database
            ComboBoxItem ComboItem = (ComboBoxItem)TypeCb.SelectedItem;
            Database.Intance.Data.Root.Element("CakeList").Add(new XElement("Cake",
                                                        new XElement("Id", currentID),
                                                        new XElement("Name", currentCake.Name),
                                                        new XElement("Type", ComboItem.Content),
                                                        new XElement("Description", currentCake.Description),
                                                        new XElement("DateAdded", DateAdded.Text),
                                                        new XElement("Image", CakeImage.Name),
                                                        new XElement("PurchasePrice", currentCake.PurchasePrice),
                                                        new XElement("SellPrice", currentCake.SellPrice)
                                                          ));
            #endregion

            #region Update database
            CakeList.Intance.Update();
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

    }
}
