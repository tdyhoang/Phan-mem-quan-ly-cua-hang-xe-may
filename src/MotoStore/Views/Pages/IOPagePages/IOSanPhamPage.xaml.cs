using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;
using MotoStore.Database;
using MotoStore.Views.Windows;
using Wpf.Ui.Controls.Interfaces;

namespace MotoStore.Views.Pages.IOPagePages
{
    /// <summary>
    /// Interaction logic for IOSanPhamPage.xaml
    /// </summary>
    public partial class IOSanPhamPage : Page
    {
        internal ObservableCollection<Tuple<MatHang, string>> matHangs;
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
                matHangs.Add(new(xe, $"/Products Images/{xe.MaMh}.png"));
            }
            ListViewProduct.ItemsSource = matHangs;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewProduct.ItemsSource);
            //biến view kiểu CollectionView dùng để gom nhóm, tìm kiếm, filter, điều hướng dữ liệu, gán nó bằng ItemsSource ở trên
            view.Filter = Filter;
            GC.Collect();
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
        static string luachon;
        private void subItemTuChon_Click(object sender, RoutedEventArgs e)
        {
            lblTu.Visibility = Visibility.Visible;
            txtTu.Visibility = Visibility.Visible;
            txtDen.Visibility = Visibility.Visible;
            lblDen.Visibility = Visibility.Visible;
            btnTim.Visibility = Visibility.Visible;
            if (subItemTuChon.IsChecked)
            {
                luachon = "PK";
                subItemTuChon.IsChecked = false;
            }
            else if (subItemTuChonGia.IsChecked)
            {
                luachon = "Gia";
                subItemTuChonGia.IsChecked = false;
            }
        }

        private void subItemQuayLai_Click(object sender, RoutedEventArgs e)
        {
            lblTu.Visibility = Visibility.Collapsed;
            txtTu.Visibility = Visibility.Collapsed;
            txtDen.Visibility = Visibility.Collapsed;
            lblDen.Visibility = Visibility.Collapsed;
            btnTim.Visibility = Visibility.Collapsed;
            ObservableCollection<Tuple<MatHang, string>> ListItems = new();
            foreach (var xe in matHangs.ToList())
                    ListItems.Add(new(xe.Item1, xe.Item2));
            ListViewProduct.ItemsSource = ListItems;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewProduct.ItemsSource);
            view.Filter = Filter;
            //3 dòng trên dùng để filter sản phẩm
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
                MessageBox.Show("Vui lòng điền đầy đủ khoảng trống", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
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
                        break;
                    case "Gia":

                        foreach (var xe in matHangs.ToList())
                        {
                            if (xe.Item1.GiaBanMh.Value >= int.Parse(txtTu.Text) && xe.Item1.GiaBanMh.Value <= int.Parse(txtDen.Text))
                                ListItems.Add(new(xe.Item1, xe.Item2));
                        }
                        break;
                }
            }
            ListViewProduct.ItemsSource = ListItems;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewProduct.ItemsSource);
            view.Filter = Filter;
        }

        private void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
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
    }
}
