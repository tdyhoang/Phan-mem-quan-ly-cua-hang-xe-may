using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using MotoStore.Database;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace MotoStore.Views.Pages
{
    /// <summary>
    /// Interaction logic for ReportPage.xaml
    /// </summary>
    public partial class ReportPage : Page
    {
        static private int SoLgXeBanChay;
        static private int SoLgBanXeNVNgSuat;

        //4 dòng dưới phục vụ cho việc tìm NHỮNG mặt hàng bán chạy (cùng số lượng bán ra và số lượng bán ra cao nhất)
        private List<string> ListMaMH;
        private List<int> ListSoLgBanRaTungXe;
        private List<string> ListMaMHBanChay;
        private List<string> ListTenMHBanChay;

        //4 dòng dưới phục vụ cho việc tìm NHỮNG Nhân Viên năng suất
        private List<string> ListMaNV;
        private List<int> ListSoLgBanRaTungNV;
        private List<string> ListMaNVNgSuat;
        private List<string> ListTenNVNgSuat;

        //4 dòng dưới phục vụ cho việc tìm NHỮNG KH VIP
        private List<string> ListMaKH;
        private List<decimal> ListSoTienBoRaTungKH;
        private List<string> ListMaKHVip;
        private List<string> ListTenKHVip;

        static private int index = 0; //của MH Bán chạy
        static private int indexNV = 0; //của NV NgSuat
        static private int indexKH = 0; //Của Khách VIP

        public ReportPage()
        {
            InitializeComponent();
        }

        private void Refresh()
        {
            MainDatabase mdb = new();
            /*Tìm Tên Xe Bán Chạy Nhất trên SQL:
            SELECT TENMH
            FROM MATHANG 
            WHERE MaMH = (SELECT TOP(1) MaMH FROM HOADON Group by MaMH Order by SUM(SoLuong) DESC)*/
            index = 0;
            indexNV = 0;
            indexKH = 0;
            ListMaMH = new();
            ListSoLgBanRaTungXe = new();
            ListMaMHBanChay = new();
            ListTenMHBanChay = new();
            index = 0;
            indexNV = 0;
            indexKH = 0;

            var XeBanChay = mdb.HoaDons.GroupBy(u => u.MaMh).Select(u => new { Tong = u.Sum(u => u.SoLuong), IdXe = u.Key }).OrderByDescending(u => u.Tong).FirstOrDefault();
            SoLgXeBanChay = XeBanChay.Tong; //Tìm số lượng bán ra của sp bán chạy nhất

            foreach (var xe in mdb.MatHangs.ToList())
                ListMaMH.Add(xe.MaMh); //Tạo List Mã MH, add từng mặt hàng vào List

            for (int i = 0; i < ListMaMH.Count; i++)
            {
                ListSoLgBanRaTungXe.Add(0);
                foreach (var xe in mdb.HoaDons.ToList())
                {
                    if (xe.MaMh == ListMaMH[i]) //listmamh[i] tương ứng với listsolgbanra[i]
                        ListSoLgBanRaTungXe[i] += xe.SoLuong;
                }
                //Tìm số lượng bán ra mỗi xe
            }
            for (int i = 0; i < ListMaMH.Count; i++)
            {
                if (ListSoLgBanRaTungXe[i] == SoLgXeBanChay)
                    ListMaMHBanChay.Add(ListMaMH[i]);
                //Tìm Mã các xe bán chạy
            }
            foreach(var xe in mdb.MatHangs.ToList())
            {
                for (int i = 0; i < ListMaMHBanChay.Count; i++)
                {
                    if (xe.MaMh == ListMaMHBanChay[i]) 
                        ListTenMHBanChay.Add(xe.TenMh);
                }
                //Tìm Tên các xe bán chạy
            }
            if (ListMaMHBanChay.Count > 0) //Có MH bán chạy
                txtblThgTinMHBanChay.Text = $"{ListTenMHBanChay[0]}\nMã Mặt Hàng:\n{ListMaMHBanChay[0]}";

            ListMaNV = new();
            ListSoLgBanRaTungNV = new();
            ListMaNVNgSuat = new();
            ListTenNVNgSuat = new();

            var NVNgSuat = mdb.HoaDons.GroupBy(u => u.MaNv).Select(u => new { Tong = u.Sum(u => u.SoLuong), IdNv = u.Key }).OrderByDescending(u => u.Tong).FirstOrDefault();
            SoLgBanXeNVNgSuat = NVNgSuat.Tong;
            foreach (var nv in mdb.NhanViens.ToList())
                ListMaNV.Add(nv.MaNv);
            for (int i=0;i<ListMaNV.Count;i++)
            {
                ListSoLgBanRaTungNV.Add(0);
                foreach (var nv in mdb.HoaDons.ToList())
                    if (nv.MaNv == ListMaNV[i])
                        ListSoLgBanRaTungNV[i] += nv.SoLuong;
            }
            for (int i = 0; i < ListMaNV.Count; i++)
            {
                if (ListSoLgBanRaTungNV[i] == SoLgBanXeNVNgSuat)
                    ListMaNVNgSuat.Add(ListMaNV[i]);
            }
            foreach (var nv in mdb.NhanViens.ToList())
            {
                for (int i = 0; i < ListMaNVNgSuat.Count; i++)
                {
                    if (nv.MaNv == ListMaNVNgSuat[i])
                        ListTenNVNgSuat.Add(nv.HoTenNv);
                }
                //Tìm Tên các xe bán chạy
            }
            if(ListMaNVNgSuat.Count>0)
                txtblThgTinNVNgSuat.Text= $"{ListTenNVNgSuat[0]}\nMã Nhân Viên:\n{ListMaNVNgSuat[0]}";

            ListMaKH = new();
            ListSoTienBoRaTungKH = new();
            ListMaKHVip = new();
            ListTenKHVip = new();

            var KhVIP = mdb.HoaDons.GroupBy(u => u.MaKh).Select(u => new { Tong = u.Sum(u => u.ThanhTien), IdKhach = u.Key }).OrderByDescending(u => u.Tong).FirstOrDefault();
            decimal sotienKHVipChi = KhVIP.Tong;

            foreach (var kh in mdb.KhachHangs.ToList())
                ListMaKH.Add(kh.MaKh);
            for (int i = 0; i < ListMaKH.Count; i++)
            {
                ListSoTienBoRaTungKH.Add(0);
                foreach (var kh in mdb.HoaDons.ToList())
                {
                    if (kh.MaKh == ListMaKH[i])
                        ListSoTienBoRaTungKH[i] += kh.ThanhTien;
                }
            }
            for (int i = 0; i < ListMaKH.Count; i++)
            {
                if (ListSoTienBoRaTungKH[i] == sotienKHVipChi)
                    ListMaKHVip.Add(ListMaKH[i]);
            }
            foreach (var kh in mdb.KhachHangs.ToList())
            {
                for (int i = 0; i < ListMaKHVip.Count; i++)
                {
                    if (kh.MaKh == ListMaKHVip[i])
                        ListTenKHVip.Add(kh.HoTenKh);
                }
                //Tìm Tên các xe bán chạy
            }

            if (ListMaKHVip.Count > 0)
                txtblThgTinKHVIP.Text = $"{ListTenKHVip[0]}\nMã Khách Hàng: {ListMaKHVip[0]}";

            string StartDate = "1/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
            string EndDate;
            if (DateTime.Now.Month == 12)
            {
                int namsau = DateTime.Now.Year + 1;
                EndDate = "1/" + 1 + "/" + namsau;
            }
            else
            {
                int thangsau = DateTime.Now.Month + 1;
                EndDate = "1/" + thangsau + "/" + DateTime.Now.Year;
            }

            int soxebandc;
            soxebandc = mdb.HoaDons.Where(u => u.NgayLapHd >= DateTime.ParseExact(StartDate, "d/M/yyyy", CultureInfo.InvariantCulture) && u.NgayLapHd < DateTime.ParseExact(EndDate, "d/M/yyyy", CultureInfo.InvariantCulture)).Select(u => u.SoLuong).Sum();
            txtblThgTinSoXeBanDc.Text = soxebandc.ToString();

            decimal sotien;
            sotien = mdb.HoaDons.Where(u => u.NgayLapHd >= DateTime.ParseExact(StartDate, "d/M/yyyy", CultureInfo.InvariantCulture) && u.NgayLapHd < DateTime.ParseExact(EndDate, "d/M/yyyy", CultureInfo.InvariantCulture)).Select(u => u.ThanhTien).Sum();
            txtblDoanhThu.Text = string.Format("{0:C}", sotien);

            int prevMonth = DateTime.Now.AddMonths(-1).Month;
            int Year = DateTime.Now.Year;
            decimal DTThgTrc = 0;
            if (prevMonth == 12)
                Year -= 1;
            string StartDateThangTrc = "1/" + prevMonth + "/" + Year;
            string EndDateThangTrc = "1/" + DateTime.Now.Month + "/" + DateTime.Now.Year;

            DTThgTrc = mdb.HoaDons.Where(u => u.NgayLapHd >= DateTime.ParseExact(StartDateThangTrc, "d/M/yyyy", CultureInfo.InvariantCulture) && u.NgayLapHd < DateTime.ParseExact(EndDateThangTrc, "d/M/yyyy", CultureInfo.InvariantCulture)).Select(u => u.ThanhTien).Sum();
            decimal Diff = DTThgTrc - sotien;
            if (Diff > 0)
            {
                txtblThgTinMHBanE.Text = "Giảm\n" + string.Format("{0:C}", Diff);
                AnhMHBanE.ImageSource = new BitmapImage(new("pack://application:,,,/Views/Pages/Images/IconLowSales.png"));
            }
            else if (Diff == 0)
            {
                txtblThgTinMHBanE.Text = "Giữ Nguyên";
                AnhMHBanE.ImageSource = new BitmapImage(new("pack://application:,,,/Views/Pages/Images/stb-removebg-preview.png"));
            }
            else
            {
                txtblThgTinMHBanE.Text = "Tăng\n" + string.Format("{0:C}", Math.Abs(Diff));
                AnhMHBanE.ImageSource = new BitmapImage(new("pack://application:,,,/Views/Pages/Images/highSaleIcon.png"));
            }
        }

        private void btnBieuDo_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageBieuDo());
        }

        private void border6ThgTin_MouseMove(object sender, MouseEventArgs e)
        {
            border6ThgTin.Opacity = 1;
        }

        private void border6ThgTin_MouseLeave(object sender, MouseEventArgs e)
        {
            border6ThgTin.Opacity = 0.9;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
            //Tự động cập nhật lại dữ liệu mỗi lần load lại trang
        }

        private void btnNextMHBanChay_Click(object sender, RoutedEventArgs e)
        {
            if(ListMaMHBanChay.Count>0) //Nếu có tồn tại sản phẩm bán chạy, 0 thì 0 hiện gì cả
            {
                index++;
                if (index < ListMaMHBanChay.Count)
                    txtblThgTinMHBanChay.Text = $"{ListTenMHBanChay[index]}\nMã Mặt Hàng:\n{ListMaMHBanChay[index]}";
                else
                {
                    index = 0;
                    txtblThgTinMHBanChay.Text = $"{ListTenMHBanChay[index]}\nMã Mặt Hàng:\n{ListMaMHBanChay[index]}";
                }
            }
        }

        private void btnNextNV_Click(object sender, RoutedEventArgs e)
        {
            if(ListMaNVNgSuat.Count>0)
            {
                indexNV++;
                if (indexNV < ListMaNVNgSuat.Count)
                    txtblThgTinNVNgSuat.Text = $"{ListTenNVNgSuat[indexNV]}\nMã Nhân Viên:\n{ListMaNVNgSuat[indexNV]}";
                else
                {
                    indexNV = 0;
                    txtblThgTinNVNgSuat.Text = $"{ListTenNVNgSuat[indexNV]}\nMã Nhân Viên:\n{ListMaNVNgSuat[indexNV]}";
                }
            }
        }

        private void btnNextKH_Click(object sender, RoutedEventArgs e)
        {
            if(ListMaKHVip.Count>0)
            {
                indexKH++;
                if (indexKH < ListMaKHVip.Count)
                    txtblThgTinKHVIP.Text = $"{ListTenKHVip[indexKH]}\nMã Khách Hàng: {ListMaKHVip[indexKH]}";
                else
                {
                    indexKH = 0;
                    txtblThgTinKHVIP.Text = $"{ListTenKHVip[indexKH]}\nMã Khách Hàng: {ListMaKHVip[indexKH]}";
                }
            }
        }
    }
}
