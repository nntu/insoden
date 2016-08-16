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
    public partial class frmInTBNoDenHan : Form
    {
        List<InTBNoDenHan> _InTBNoDenHan;
        ThongTinNganHang _ttThongtin;
        public frmInTBNoDenHan(List<InTBNoDenHan> InTBNoDenHan, ThongTinNganHang ttThongtinnganhang)
        {
            _InTBNoDenHan = InTBNoDenHan;
            _ttThongtin = ttThongtinnganhang;
            InitializeComponent();
        }
        public frmInTBNoDenHan()
        {
            InitializeComponent();
        }
        private void frmInTBNoDenHan_Load(object sender, EventArgs e)
        {
            tbtkdenhan1.SetDataSource(_InTBNoDenHan);
            tbtkdenhan1.Refresh();
            crystalReportViewer1.ReportSource = tbtkdenhan1;

            tbtkdenhan1.SetParameterValue("HoTen", string.Format("{0}{1}", _ttThongtin.cfname1, _ttThongtin.cfname2));
            tbtkdenhan1.SetParameterValue("DiaChi", string.Format("{0}{1}", _ttThongtin.addr1, _ttThongtin.addr2));
            tbtkdenhan1.SetParameterValue("TenNH_vi", string.Format("{0}", _ttThongtin.tencn_vi));
            tbtkdenhan1.SetParameterValue("TenNH_en", string.Format("{0}", _ttThongtin.tencn_en));
            tbtkdenhan1.SetParameterValue("DiaChiNH", string.Format("{0}", _ttThongtin.diachi));
            tbtkdenhan1.SetParameterValue("NgayDC", _ttThongtin.ngaycuoiky);
            tbtkdenhan1.SetParameterValue("DienThoai", _ttThongtin.dt);
            tbtkdenhan1.SetParameterValue("PhongLH", _ttThongtin.noinhan);

            tbtkdenhan1.SetParameterValue("tennguoiky", _ttThongtin.tennguoiky);
            tbtkdenhan1.SetParameterValue("chucdanh", _ttThongtin.chucdanh);

            tbtkdenhan1.SetParameterValue("chuky", AppDomain.CurrentDomain.BaseDirectory + _ttThongtin.tenfilechuky);
        }
    }
}
