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
            MainDatabase db = new MainDatabase();
            List<Product> products = new List<Product>();
            List<MatHang> matHang = db.MatHangs.ToList();
            string AnhXE;
            foreach (MatHang matHang1 in matHang)
            {
                AnhXE = "/Views/Pages/IO_Images/" + matHang1.MaMh.ToString() + ".png"; //Ảnh của từng xe(gán theo mã xe có sẵn )
                products.Add(new Product(matHang1.TenMh, matHang1.GiaBanMh, AnhXE));
            }
            return products;
        }
        private void btnAddNewPageSP_Click(object sender, RoutedEventArgs e)
        {
            IOAddSPPage ioAddSP = new IOAddSPPage();
            NavigationService.Navigate(ioAddSP);

        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Add new Window to show details
            Window window= new Window();
            window.Height = 600;
            window.Width = 500;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.Title = "ITEMS";
            Border myBorder = new Border();
            myBorder.Background = Brushes.LightBlue;
            myBorder.BorderBrush = Brushes.Black;
            myBorder.Padding = new Thickness(20);
           
            myBorder.BorderThickness = new Thickness(2);

            StackPanel myStackPanel = new StackPanel();
            myStackPanel.Background = Brushes.White;
            myStackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            myStackPanel.VerticalAlignment = VerticalAlignment.Top;
            myStackPanel.Width = 500;
            myStackPanel.Height = 600;

            TextBlock myTextBlock = new TextBlock();
            myTextBlock.Margin = new Thickness(10, 0, 10, 0);
            myTextBlock.FontSize = 20;
            myTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            myTextBlock.Text = "Chi Tiết Sản Phẩm";

            
            //Image Sản Phẩm
            Image image = new Image();
            //image.Source= new BitmapImage(new Uri())
          
            //Button 1
            Button myButton1 = new Button();
            myButton1.HorizontalAlignment = HorizontalAlignment.Center;
            myButton1.Margin = new Thickness(20);
            myButton1.Content = "Button 1";
            //Button 2
            Button myButton2 = new Button();
            myButton2.HorizontalAlignment = HorizontalAlignment.Center;
            myButton2.Margin = new Thickness(10);
            myButton2.Content = "Button 2";
            //Button 3
            Button myButton3 = new Button();
            myButton3.HorizontalAlignment = HorizontalAlignment.Center;
            myButton3.VerticalAlignment = VerticalAlignment.Bottom;
            myButton3.Margin = new Thickness(0);
            myButton3.Content = "Quay lại";
            

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
