using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace insoden
{
    public partial class Inpdc : Form
    {
        private IList<incif> lCif;
        private ThongTinNganHang ttThongtin;
   //     private bool _chuky;

        public Inpdc(IList<incif> aCif, ThongTinNganHang ttThongtinnganhang)
        {
            lCif = aCif;
            ttThongtin = ttThongtinnganhang;

            InitializeComponent();
        }

        public Inpdc()
        {
            InitializeComponent();
        }

        private void inpdc_Load(object sender, EventArgs e)
        {
            //   crystalReportViewer1.ParameterFieldInfo.Clear();
            pdc1.SetDataSource(lCif);
            pdc1.Refresh();
            crystalReportViewer1.ReportSource = pdc1;

            pdc1.SetParameterValue("HoTen", string.Format("{0}{1}", ttThongtin.cfname1, ttThongtin.cfname2));
            pdc1.SetParameterValue("DiaChi", string.Format("{0}{1}", ttThongtin.addr1, ttThongtin.addr2));
            pdc1.SetParameterValue("TenNH_vi", string.Format("{0}", ttThongtin.tencn_vi));
            pdc1.SetParameterValue("TenNH_en", string.Format("{0}", ttThongtin.tencn_en));
            pdc1.SetParameterValue("DiaChiNH", string.Format("{0}", ttThongtin.diachi));
            pdc1.SetParameterValue("NgayDC", ttThongtin.ngaycuoiky);
            pdc1.SetParameterValue("DienThoai", ttThongtin.dt);
            pdc1.SetParameterValue("PhongLH", ttThongtin.noinhan);

            pdc1.SetParameterValue("tennguoiky", ttThongtin.tennguoiky);
            pdc1.SetParameterValue("chucdanh", ttThongtin.chucdanh);

            pdc1.SetParameterValue("chuky", AppDomain.CurrentDomain.BaseDirectory + ttThongtin.tenfilechuky);

            //  crystalReportViewer1.RefreshReport();
        }

        private void pdc1_InitReport(object sender, EventArgs e)
        {
        }
    }
}