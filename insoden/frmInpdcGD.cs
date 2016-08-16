using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace insoden
{
    public partial class frmInpdcGD : Form
    {
        private IList<incif> lCif;
        private ThongTinNganHang ttThongtin;

        public frmInpdcGD(IList<incif> aCif, ThongTinNganHang ttThongtinnganhang)
        {
            lCif = aCif;
            ttThongtin = ttThongtinnganhang;

            InitializeComponent();
        }

        public frmInpdcGD()
        {
            InitializeComponent();
        }

        private void InpdcGD_Load(object sender, EventArgs e)
        {
            //   crystalReportViewer1.ParameterFieldInfo.Clear();
            pdcgiamdoc1.SetDataSource(lCif);
            pdcgiamdoc1.Refresh();
            crystalReportViewer1.ReportSource = pdcgiamdoc1;

            pdcgiamdoc1.SetParameterValue("HoTen", string.Format("{0}{1}", ttThongtin.cfname1, ttThongtin.cfname2));
            pdcgiamdoc1.SetParameterValue("DiaChi", string.Format("{0}{1}", ttThongtin.addr1, ttThongtin.addr2));
            pdcgiamdoc1.SetParameterValue("TenNH_vi", string.Format("{0}", ttThongtin.tencn_vi));
            pdcgiamdoc1.SetParameterValue("TenNH_en", string.Format("{0}", ttThongtin.tencn_en));
            pdcgiamdoc1.SetParameterValue("DiaChiNH", string.Format("{0}", ttThongtin.diachi));
            pdcgiamdoc1.SetParameterValue("NgayDC", ttThongtin.ngaycuoiky);
            pdcgiamdoc1.SetParameterValue("DienThoai", ttThongtin.dt);
            pdcgiamdoc1.SetParameterValue("PhongLH", ttThongtin.noinhan);
            pdcgiamdoc1.SetParameterValue("tennguoiky", ttThongtin.tennguoiky);
            pdcgiamdoc1.SetParameterValue("chucdanh", ttThongtin.chucdanh);

            pdcgiamdoc1.SetParameterValue("chuky", AppDomain.CurrentDomain.BaseDirectory + ttThongtin.tenfilechuky);
        }
    }
}