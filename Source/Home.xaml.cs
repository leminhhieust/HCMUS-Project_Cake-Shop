using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        class PageCount : INotifyPropertyChanged
        {
            private int _currentPage; // Backup field
            private int _totalPage; // Backup field
            private int _recipePerPage; // Backup field
            public event PropertyChangedEventHandler PropertyChanged;

            public int CurrentPage
            {
                get { return _currentPage; }
                set
                {
                    _currentPage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentPage"));
                }
            }

            public int TotalPage
            {
                get { return _totalPage; }
                set
                {
                    _totalPage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TotalPage"));
                }
            }

            public int RecipePerPage
            {
                get { return _recipePerPage; }
                set
                {
                    _recipePerPage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RecipePerPage"));
                }
            }
        }

        class Type
        {
            public string Name { get; set; }
            public int Quantity { get; set; }
        }

        private PageCount PageCountInstance { get; set; }
        private ObservableCollection<Cake> CurrentData { get; set; }

        public Home()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PageCountInstance = new PageCount();
            CurrentData = new ObservableCollection<Cake>();
            CurrentData = CakeList.Intance.Data;

            LoadSettingPage();
            LoadTypeList();
            DataContext = PageCountInstance;
            TypeListView.ItemsSource = LoadTypeList();
            dataListView.ItemsSource = LoadCakeList(PageCountInstance.CurrentPage, PageCountInstance.RecipePerPage);
        }

        ObservableCollection<Cake> LoadCakeList(int page, int recipeNum)
        {
            CheckButton(page);
            ObservableCollection<Cake> ResultList = new ObservableCollection<Cake>(CurrentData.Skip((page - 1) * recipeNum).Take(recipeNum).ToList());
            return ResultList;
         
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            PageCountInstance.CurrentPage++;
            dataListView.ItemsSource = LoadCakeList(PageCountInstance.CurrentPage, PageCountInstance.RecipePerPage);
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            PageCountInstance.CurrentPage--;
            dataListView.ItemsSource = LoadCakeList(PageCountInstance.CurrentPage, PageCountInstance.RecipePerPage);
        }

        private void CheckButton(int page)
        {
            if (page == 1)
            {
                Prev.Visibility = Visibility.Hidden;
            }
            else
            {
                Prev.Visibility = Visibility.Visible;
            }
            if (page == PageCountInstance.TotalPage)
            {
                Next.Visibility = Visibility.Hidden;
            }
            else
            {
                Next.Visibility = Visibility.Visible;
            }
        }

        private void LoadSettingPage()
        {
            PageCountInstance.CurrentPage = 1;
            PageCountInstance.RecipePerPage = 6;
            PageCountInstance.TotalPage = CurrentData.Count / PageCountInstance.RecipePerPage + (CurrentData.Count % PageCountInstance.RecipePerPage == 0 ? 0 : 1);
        }

        ObservableCollection<Type> LoadTypeList()
        {
            List<string> typenames = new List<string>();

            List<Type> types = new List<Type>();

            foreach (var cake in CakeList.Intance.Data)
            {
                typenames.Add(cake.Type);
            }

            typenames = typenames.Distinct().ToList();

            types.Add(new Type { Name = "Tất cả", Quantity = CakeList.Intance.Data.Count});

            foreach (var typename in typenames)
            {
                Type temp = new Type();
                int count = 0;
                foreach (var cake in CakeList.Intance.Data)
                {
                    if (cake.Type == typename)
                    {
                        count++;
                    }
                }
                temp.Name = typename;
                temp.Quantity = count;
                types.Add(temp);
            }
            return new ObservableCollection<Type>(types);
        }

        private void TypeBtn_Click(object sender, RoutedEventArgs e)
        {
            var type = TypeListView.SelectedItem as Type;
            if(type.Name == "Tất cả")
            {
                CurrentData = CakeList.Intance.Data;
            }
            else
            {
                CurrentData = new ObservableCollection<Cake>();
                foreach (var cake in CakeList.Intance.Data)
                {
                    if (cake.Type == type.Name)
                    {
                        CurrentData.Add(cake);
                    }
                }
            }
            dataListView.ItemsSource = LoadCakeList(PageCountInstance.CurrentPage, PageCountInstance.RecipePerPage);
            LoadSettingPage();
            CheckButton(1);
        }

        private void AddUpdateCake_Click(object sender, RoutedEventArgs e)
        {
            Button currentBtn = sender as Button;
            var addScreen = new Add();
            if (currentBtn.Name == "AddBtn")
            {
                Flag.Intance.OnAdd = true;
                Flag.Intance.OnUpdate = false;
            }
            else if (currentBtn.Name == "UpdateBtn")
            {
                Flag.Intance.OnUpdate = true;
                Flag.Intance.OnAdd = false;
                addScreen.currentCake = dataListView.SelectedItem as Cake;
            }

            addScreen.ShowDialog();
            while (Flag.Intance.OnAdd)
            {
                //wait
            }
            CakeList.Intance.Update();
            dataListView.ItemsSource = LoadCakeList(PageCountInstance.CurrentPage, PageCountInstance.RecipePerPage);
        }

        private void DetailBtn_Click(object sender, RoutedEventArgs e)
        {
            Flag.Intance.OnDetail = true;
            var detailScreen = new Detail();
            detailScreen.currentCake = dataListView.SelectedItem as Cake;
            detailScreen.ShowDialog();
            while (Flag.Intance.OnDetail)
            {
                //wait
            }
            TypeListView.ItemsSource = LoadTypeList();
            dataListView.ItemsSource = LoadCakeList(PageCountInstance.CurrentPage, PageCountInstance.RecipePerPage);
        }
    }
}
