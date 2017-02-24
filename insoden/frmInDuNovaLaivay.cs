using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace insoden
{
    public partial class frmInDuNovaLaivay : Form
    {
        List<InTBDuNovaLaiVay> _indnlai;
        ThongTinNganHang _ttThongtin;
        public frmInDuNovaLaivay(List<InTBDuNovaLaiVay> indnlai, ThongTinNganHang ttThongtinnganhang)
        {
            _indnlai = indnlai;
            _ttThongtin = ttThongtinnganhang;
            InitializeComponent();
        }

        public frmInDuNovaLaivay()
        {
            InitializeComponent();
        }

        private void frmInDuNovaLaivay_Load(object sender, EventArgs e)
        {
            tbdunovalaivay1.SetDataSource(_indnlai);
            tbdunovalaivay1.Refresh();
            crystalReportViewer1.ReportSource = tbdunovalaivay1;

            tbdunovalaivay1.SetParameterValue("HoTen", string.Format("{0}{1}", _ttThongtin.cfname1, _ttThongtin.cfname2));
            tbdunovalaivay1.SetParameterValue("DiaChi", string.Format("{0}{1}", _ttThongtin.addr1, _ttThongtin.addr2));
            tbdunovalaivay1.SetParameterValue("TenNH_vi", string.Format("{0}", _ttThongtin.tencn_vi));
            tbdunovalaivay1.SetParameterValue("TenNH_en", string.Format("{0}", _ttThongtin.tencn_en));
            tbdunovalaivay1.SetParameterValue("DiaChiNH", string.Format("{0}", _ttThongtin.diachi));
            tbdunovalaivay1.SetParameterValue("NgayDC", DateTime.Now);
            tbdunovalaivay1.SetParameterValue("DienThoai", _ttThongtin.dt);
            tbdunovalaivay1.SetParameterValue("PhongLH", _ttThongtin.noinhan);

            tbdunovalaivay1.SetParameterValue("tennguoiky", _ttThongtin.tennguoiky);
            tbdunovalaivay1.SetParameterValue("chucdanh", _ttThongtin.chucdanh);

            tbdunovalaivay1.SetParameterValue("chuky", AppDomain.CurrentDomain.BaseDirectory + _ttThongtin.tenfilechuky);
        }
    }
}
