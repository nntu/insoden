using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
namespace insoden
{
    public partial class UC_thulai : DevExpress.XtraEditors.XtraUserControl
    {
        List<LayTKThanhToanTheoCIF_Result> _ciftk;
        List<ThuLai> _inthulai;
        List<TinhThuLaiTheoCIF_Result> _tp;
        public UC_thulai(List<TinhThuLaiTheoCIF_Result> tp, List< LayTKThanhToanTheoCIF_Result> ciftk)
        {
            _tp = tp;
            _ciftk = ciftk;
            _inthulai = new List<ThuLai>();
            InitializeComponent();
        }
        public UC_thulai( )
        {
            InitializeComponent();
        }

        private void bt_td_pyctl_thulai_xuatexcel_Click(object sender, EventArgs e)
        {

        }

        private void GV_TD_Pyctl_ThuLai_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName == "PhuongThucTra")
            {

                e.RepositoryItem = repositoryItemComboBox1;
            }


        }

        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void UC_thulai_Load(object sender, EventArgs e)
        {
            repositoryItemComboBox1.Items.Clear();
          
            foreach(var i in _ciftk) {
                repositoryItemComboBox1.Items.Add(string.Format(@"{0:###-##-##-######-#}", i.ACCTNO));
            }
            repositoryItemComboBox1.Items.Add("Tien Mat");

            foreach (var i in _tp)
            {
                _inthulai.Add(new ThuLai()
                {
                    Cif = i.socif,
                    TenKh = i.khachhang,
                    LaiCongDon = i.laicongdon ?? 0,
                    LaiTraCham = i.laitracham ?? 0,
                    SoTienPhaiTra = (i.laicongdon ?? 0) + (i.laitracham ?? 0),
                    PhuongThucTra = "Tien Mat",
                    tkvay = string.Format(@"{0:###-##-##-######-#}", i.taikhoan),
                    GhiChu = @"Ngày tính lãi " + i.denngay.Value.ToShortDateString()

                });
            }
            GC_TD_Pyctl_ThuLai.DataSource = new BindingSource(_inthulai, "");
            GV_TD_Pyctl_ThuLai.OptionsView.BestFitMaxRowCount = -1;
            GV_TD_Pyctl_ThuLai.BestFitColumns();
        }
    }
}
