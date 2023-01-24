using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using MotoStore.Database;
using MotoStore.Views.Windows;

namespace MotoStore.Views.Pages.IOPagePages
{
    /// <summary>
    /// Interaction logic for IOSanPhamPage.xaml
    /// </summary>
    public partial class IOSanPhamPage : Page
    {
        internal ObservableCollection<Tuple<MatHang, string>> matHangs;
        static string luachon = "0";

        public IOSanPhamPage()
        {
            InitializeComponent();
            DataContext = this;
            Refresh();              
        }

        public void Refresh()
        {
            MainDatabase mdb = new();
            matHangs = new();
            foreach (var xe in mdb.MatHangs.ToList())
            {
                if (xe.DaXoa)
                    continue;
                //matHangs.Add(new(xe, $"/Products Images/{xe.MaMh}.png"));
                matHangs.Add(new(xe, "D:\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Products Images\\" + xe.MaMh + ".png"));
                BitmapImageFromFile("D:\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Products Images\\" + xe.MaMh + ".png");
            }            
            ListViewProduct.ItemsSource = matHangs;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewProduct.ItemsSource);
            //biến view kiểu CollectionView dùng để gom nhóm, tìm kiếm, filter, điều hướng dữ liệu, gán nó bằng ItemsSource ở trên
            
            view.Filter = Filter;
            GC.Collect();
        }

        public static BitmapSource BitmapImageFromFile(string filepath)
        {
            var bi = new BitmapImage();

            using (var fs = new FileStream(filepath, FileMode.Open))
            {
                bi.BeginInit();
                bi.StreamSource = fs;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.EndInit();
            }

            bi.Freeze(); //Important to freeze it, otherwise it will still have minor leaks

            //Cop từ https://stackoverflow.com/questions/28364439/how-to-dispose-bitmapimage-cache
            return bi;
        }

        private void btnAddNewPageSP_Click(object sender, RoutedEventArgs e)
        {
            IOAddSPPage ioAddSP = new();
            NavigationService.Navigate(ioAddSP);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(ListViewProduct.SelectedItem is Tuple<MatHang, string> mathang)
            {
                WindowInformation WI = new WindowInformation(mathang,this);
                WI.ShowDialog();
            }
        }

        private bool Filter(object item)
        {
            if(string.IsNullOrWhiteSpace(txtTimKiem.Text))
                return true; //Text rỗng hoặc chứa khoảng trắng thì hiện toàn bộ item(Sản Phẩm)
            if(item is Tuple< MatHang,string> mh)
            {
                if (mh.Item1.MaMh.Contains(txtTimKiem.Text, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (mh.Item1.TenMh.Contains(txtTimKiem.Text, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (mh.Item1.Mau.Contains(txtTimKiem.Text, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private void txtTimKiem_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewProduct.ItemsSource).Refresh();
        }

        private void subItemQuayLai_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
            subItemLocTheo.Header = "Lọc Theo:";
            luachon = "0";
        }

        private void txtTu_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTu.Text))
                MessageBox.Show("Vui lòng điền đầy đủ");
        }

        private void txtDen_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTu.Text))
                MessageBox.Show("Vui lòng điền đầy đủ");
        }

        private void btnTim_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Tuple<MatHang, string>> ListItems = new();
            if (string.IsNullOrWhiteSpace(txtTu.Text) || string.IsNullOrWhiteSpace(txtDen.Text))
                MessageBox.Show("Vui lòng điền đầy đủ khoảng trống", "Nhắc Nhở", MessageBoxButton.OK, MessageBoxImage.Information);
            else if (long.Parse(txtTu.Text) >= long.Parse(txtDen.Text))
                MessageBox.Show("Giá trị trong ô Từ không được phép lớn hơn hoặc bằng giá trị trong ô Đến");
            else
            {
                switch (luachon)
                {
                    case "PK":
                        foreach (var xe in matHangs.ToList())
                        {
                            if (xe.Item1.SoPhanKhoi >= int.Parse(txtTu.Text) && xe.Item1.SoPhanKhoi <= int.Parse(txtDen.Text))
                                ListItems.Add(new(xe.Item1, xe.Item2));
                        }
                        ListViewProduct.ItemsSource = ListItems;
                        CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewProduct.ItemsSource);
                        view.Filter = Filter;
                        break;
                    case "Gia":
                        foreach (var xe in matHangs.ToList())
                        {
                            if (xe.Item1.GiaBanMh.Value >= int.Parse(txtTu.Text) && xe.Item1.GiaBanMh.Value <= int.Parse(txtDen.Text))
                                ListItems.Add(new(xe.Item1, xe.Item2));
                        }
                        ListViewProduct.ItemsSource = ListItems;
                        CollectionView view1 = (CollectionView)CollectionViewSource.GetDefaultView(ListViewProduct.ItemsSource);
                        view1.Filter = Filter;
                        break;
                    case "0":
                        MessageBox.Show("Vui Lòng Chọn Điều Kiện Lọc!");
                        break;
                }
            }
        }

        private new void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = MouseWheelEvent,
                Source = sender
            };
            var parent = ((Control)sender).Parent as UIElement;
            parent?.RaiseEvent(eventArg);
        }

        private void ListViewProduct_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void ListViewProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is ListView lv)
            {
                if (lv.SelectedItems.Count > 1)
                {
                    e.Handled = true;
                }
                else if (lv.SelectedItem is Tuple<MatHang, string> mathang)
                {
                    WindowInformation WI = new WindowInformation(mathang,this);
                    WI.ShowDialog();
                    lv.SelectedItem = null;
                }
            }
        }

        private void btnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void subItemGia_Click(object sender, RoutedEventArgs e)
        {
            luachon = "Gia";
            subItemGia.IsChecked = false;
            subItemLocTheo.Header = "Giá";
        }

        private void subItemPK_Click(object sender, RoutedEventArgs e)
        {
            luachon = "PK";
            subItemPK.IsChecked = false;
            subItemLocTheo.Header = "Phân Khối";
        }

        private void txtTu_TextChanged(object sender, TextChangedEventArgs e)
        {
           /* if(luachon=="Gia")
            {
                if (!string.IsNullOrWhiteSpace(txtTu.Text))
                    txtTu.Text = string.Format("{0:#.00}", Convert.ToDecimal(txtTu.Text) / 100);
            } */
        }

        private void txtDen_TextChanged(object sender, TextChangedEventArgs e)
        {
           /* if (luachon == "Gia")
            {
                if (!string.IsNullOrWhiteSpace(txtDen.Text))
                    txtDen.Text = string.Format("{0:#.00}", Convert.ToDecimal(txtDen.Text) / 100);
            } */
        }
    }
}
