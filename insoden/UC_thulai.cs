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
using System.IO;
using OfficeOpenXml;

namespace insoden
{
    public partial class UC_thulai : DevExpress.XtraEditors.XtraUserControl
    {
        List<LayTKThanhToanTheoCIF_Result> _ciftk;
        List<ThuLai> _inthulai;
        List<TinhThuLaiTheoCIF_Result> _tp;
        public static string _clickOnceLocation;

        public void SetLocaltion(string clickOnceLocation) {
            _clickOnceLocation = clickOnceLocation;
        }
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
            var pathToFile = Path.Combine(_clickOnceLocation, @"thulai.xlsx");
            SaveFileDialog SaveFileExcel = new SaveFileDialog();
            SaveFileExcel.FileName = "thulai_";
            SaveFileExcel.Filter = @"Excel File (*.xlsx)|*.xlsx";
            if (SaveFileExcel.ShowDialog() == DialogResult.OK)
            {
                var temp = new FileInfo(pathToFile);
                using (var pck = new ExcelPackage(temp))
                {
                    ExcelWorksheet wsList = pck.Workbook.Worksheets[1];
                    const int startRow = 13;

                    int row = startRow;
                    

                    for (int i = 0; i < GV_TD_Pyctl_ThuLai.DataRowCount; i++)
                    {
 
                        wsList.InsertRow(row, 1);
                        wsList.Cells[row, 2].Value = GV_TD_Pyctl_ThuLai.GetRowCellValue(i,"tkvay").ToString();

                        wsList.Cells[row, 3].Value = Convert.ToDecimal(GV_TD_Pyctl_ThuLai.GetRowCellValue(i, "LaiCongDon"));
                        wsList.Cells[row, 3].Style.Numberformat.Format = "#,##0.00;-#,##0.00";
                        wsList.Cells[row, 4].Value = GV_TD_Pyctl_ThuLai.GetRowCellValue(i, "LaiTraCham");
                        wsList.Cells[row, 4].Style.Numberformat.Format = "#,##0.00;-#,##0.00";
                        wsList.Cells[row, 5].Value = "";

                        var ptt = GV_TD_Pyctl_ThuLai.GetRowCellValue(i, "PhuongThucTra") ;
                        wsList.Cells[row, 6].Value = ptt.ToString();
                        wsList.Cells[row, 7].Value = GV_TD_Pyctl_ThuLai.GetRowCellValue(i, "GhiChu").ToString();

                        wsList.Cells[row,5].Formula = string.Format("Sum(C{0}:D{0})", row - 1);

                    }
                    var fi = new FileInfo(SaveFileExcel.FileName);

                    if (fi.Exists)
                    {
                        fi.Delete();
                    }
                    pck.SaveAs(fi);
                }
              insoden.MainForm.OpenExplorer(SaveFileExcel.FileName);
            }
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
                    GhiChu = @"Ngày tính lãi " + i.denngay.Value.ToShortDateString(),
                    LoaiTien = i.tiente

                });
            }
            GC_TD_Pyctl_ThuLai.DataSource = new BindingSource(_inthulai, "");
            GV_TD_Pyctl_ThuLai.OptionsView.BestFitMaxRowCount = -1;
            GV_TD_Pyctl_ThuLai.BestFitColumns();
        }
    }
}
