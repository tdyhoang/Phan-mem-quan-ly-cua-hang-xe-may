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
using MotoStore.Database;

namespace MotoStore.Views.Pages.IOPagePages
{
    /// <summary>
    /// Interaction logic for IOSanPhamPage.xaml
    /// </summary>
    public partial class IOSanPhamPage : Page
    {
        public TextBox tbmssv, tbtensv, tbdiemtb;
        public ComboBox cbxKhoa;
        public IOSanPhamPage()
        {
            InitializeComponent();
            this.DataContext = this;
            var products = GetProducts();
            if (products.Count > 0)
                ListViewProduct.ItemsSource = products;
        }
        private List<Product> GetProducts()
        {
            MainDatabase db = new();
            List<Product> products = new();
            List<MatHang> matHang = db.MatHangs.ToList();
            string AnhXE;
            foreach (MatHang matHang1 in matHang)
            {
                AnhXE = "/Views/Pages/IO_Images/" + matHang1.MaMh.ToString() + ".png"; //Ảnh của từng xe(gán theo mã xe có sẵn )
                products.Add(new(matHang1.TenMh, matHang1.GiaBanMh, AnhXE));
            }
            return products;
        }
        private void btnAddNewPageSP_Click(object sender, RoutedEventArgs e)
        {
            IOAddSPPage ioAddSP = new();
            NavigationService.Navigate(ioAddSP);

        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Add new Window to show details
            Window window = new()
            {
                Height = 600,
                Width = 500,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Title = "ITEMS"
            };
            Border myBorder = new()
            {
                Background = Brushes.LightBlue,
                BorderBrush = Brushes.Black,
                Padding = new(20),

                BorderThickness = new(2)
            };

            StackPanel myStackPanel = new()
            {
                Background = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 500,
                Height = 600
            };

            TextBlock myTextBlock = new()
            {
                Margin = new(10, 0, 10, 0),
                FontSize = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                Text = "Chi Tiết Sản Phẩm"
            };


            //Image Sản Phẩm
            Image image = new();
            //image.Source= new BitmapImage(new())

            //Button 1
            Button myButton1 = new()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new(20),
                Content = "Button 1"
            };
            //Button 2
            Button myButton2 = new()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new(10),
                Content = "Button 2"
            };
            //Button 3
            Button myButton3 = new()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new(0),
                Content = "Quay lại"
            };


            // Add child elements to the parent StackPanel.
            myStackPanel.Children.Add(myTextBlock);
            myStackPanel.Children.Add(myButton1);
            myStackPanel.Children.Add(myButton2);
            myStackPanel.Children.Add(myButton3);

            // Add the StackPanel as the lone Child of the Border.
            myBorder.Child = myStackPanel;

            // Add the Border as the Content of the Parent Window Object.
            window.Content = myBorder;
            window.Show();



        }
    }
}
