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
            tbnovaydenhan1.SetDataSource(_InTBNoDenHan);
            tbnovaydenhan1.Refresh();
            crystalReportViewer1.ReportSource = tbnovaydenhan1;

            tbnovaydenhan1.SetParameterValue("HoTen", string.Format("{0}{1}", _ttThongtin.cfname1, _ttThongtin.cfname2));
            tbnovaydenhan1.SetParameterValue("DiaChi", string.Format("{0}{1}", _ttThongtin.addr1, _ttThongtin.addr2));
            tbnovaydenhan1.SetParameterValue("TenNH_vi", string.Format("{0}", _ttThongtin.tencn_vi));
            tbnovaydenhan1.SetParameterValue("TenNH_en", string.Format("{0}", _ttThongtin.tencn_en));
            tbnovaydenhan1.SetParameterValue("DiaChiNH", string.Format("{0}", _ttThongtin.diachi));
            tbnovaydenhan1.SetParameterValue("NgayDC", DateTime.Now);
            tbnovaydenhan1.SetParameterValue("DienThoai", _ttThongtin.dt);
            tbnovaydenhan1.SetParameterValue("PhongLH", _ttThongtin.noinhan);

            tbnovaydenhan1.SetParameterValue("tennguoiky", _ttThongtin.tennguoiky);
            tbnovaydenhan1.SetParameterValue("chucdanh", _ttThongtin.chucdanh);

            tbnovaydenhan1.SetParameterValue("chuky", AppDomain.CurrentDomain.BaseDirectory + _ttThongtin.tenfilechuky);
        }
    }
}
