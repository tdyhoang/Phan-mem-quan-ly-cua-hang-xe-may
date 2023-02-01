using MotoStore.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace MotoStore.Views.Pages.IOPagePages
{
    /// <summary>
    /// Interaction logic for IOPhieuBaoHanhPage.xaml
    /// </summary>
    public partial class IOPhieuBaoHanhPage : Page
    {
        internal ObservableCollection<HoaDon> HoaDons;
        internal List<Control> InputFields;
        public IOPhieuBaoHanhPage()
        {
            InitializeComponent();
            PageInitialization();
        }
        private void PageInitialization()
        {
            InputFields = new()
            {
                cmbMaHD,
                txtThoiGian,
                txtGhiChu
            };
            PageRefresh();
        }
        private void PageRefresh()
        {
            RefreshHoaDon();
            
            foreach (var tbx in InputFields.Where(c => c is TextBox).Cast<TextBox>())
                tbx.Clear();
            txtThoiGian.Text = DateTime.Today.ToString("dd/MM/yyyy");
        }
        public void RefreshHoaDon()
        {
            MainDatabase dtb = new();
            HoaDons = new(dtb.HoaDons);
            cmbMaHD.ItemsSource = HoaDons;
            cmbMaHD.Text = string.Empty;
        }

        private void InputField_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
                tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            if (sender is ComboBox cmb)
                cmb.GetBindingExpression(ComboBox.TextProperty).UpdateSource();
        }
        private void cmbMaHD_DropDownClosed(object sender, EventArgs e)
        {
            if (sender is ComboBox cmb)
            {
                if (cmb.SelectedItem is HoaDon hd)
                    cmb.Text = hd.MaHd;
                cmb.GetBindingExpression(ComboBox.TextProperty).UpdateSource();
            }
        }
        private void ComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender is not ComboBox cmb)
                return;

            CollectionView collection = (CollectionView)CollectionViewSource.GetDefaultView(cmb.ItemsSource);
            if (string.Equals(cmb.Name, "cmbMaHD"))
            {
                collection.Filter = (c) =>
                {
                    if (string.IsNullOrEmpty(cmb.Text))
                        return true;
                    if (((HoaDon)c).MaHd.Contains(cmb.Text, StringComparison.OrdinalIgnoreCase))
                        return true;
                    if (((HoaDon)c).MaKh.Contains(cmb.Text, StringComparison.OrdinalIgnoreCase))
                        return true;
                    if (((HoaDon)c).NgayLapHd.HasValue)
                        return ((HoaDon)c).NgayLapHd.Value.ToString("dd/MM/yyyy").Contains(cmb.Text);
                    return false;
                    
                };
            }
        
        }

        private void StackPanel_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Source is ComboBox)
            {
                if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Return || e.Key == Key.Enter)
                {
                    e.Handled = true;
                }
            }
        }

  
        private void btnAddPBH_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRefreshHD_Click(object sender, RoutedEventArgs e)
        {
            RefreshHoaDon();
        }
    }
}
