using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for SplashScreen.xaml
    /// </summary>

    public partial class SplashScreen : Window
    {
        System.Timers.Timer timer;
        int count = 0;
        int target = 100;
        public Random _rng = new Random();

        public SplashScreen()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region Load Background
            var BMPImg = new BitmapImage();
            BMPImg.BeginInit();
            BMPImg.CacheOption = BitmapCacheOption.OnLoad;
            BMPImg.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + $@"Images\Splash.jpg", UriKind.RelativeOrAbsolute);
            BMPImg.EndInit();
            bg.ImageSource = BMPImg;
            #endregion

            #region Initiate
            var value = Database.Intance.Data.Element("root").Element("AppSetting").Element("ShowSplashScreen").Value;
            var showSplash = bool.Parse(value);

            if (showSplash == false)
            {
                var screen = new MainWindow();
                this.Close();
                screen.Show();
            }
            else
            {
                timer = new System.Timers.Timer();
                timer.Elapsed += Timer_Elapsed;
                timer.Interval = 50;
                timer.Start();
            }
            #endregion

            #region Load Cake
            int num = CakeList.Intance.Data.Count;
            int indexJourney = _rng.Next(num);
            Cake cake = CakeList.Intance.Data[indexJourney];

            Description.Text = cake.Description;
            Name.Text = cake.Name;
            Image.ImageSource = cake.BMPImg;
            #endregion
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            count ++;
            if (count == target)
            {
                timer.Stop();


                Dispatcher.Invoke(() =>
                {
                    var screen = new MainWindow();
                    screen.Show();

                    this.Close();
                });

            }

            Dispatcher.Invoke(() =>
            {
                progress.Value = count;
            });
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            path += "\\Data\\Database.xml";

            Database.Intance.Data.Element("root").Element("AppSetting").Element("ShowSplashScreen").Value = "false";
            File.WriteAllText(path, Database.Intance.Data.ToString());
        }

        private void CheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            path += "\\Data\\Database.xml";

            Database.Intance.Data.Element("root").Element("AppSetting").Element("ShowSplashScreen").Value = "true";
            File.WriteAllText(path, Database.Intance.Data.ToString());
        }
    }
}
