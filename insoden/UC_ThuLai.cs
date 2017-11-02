using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace insoden
{
    public partial class UC_ThuLai : DevExpress.XtraEditors.XtraUserControl
    {
        List<LayTKThanhToanTheoCIF_Result> _ciftk;
        List<ThuLai> _inthulai;
        List<TinhThuLaiTheoCIF_Result> _tp;
        string _phongthu = "";
        public static string _clickOnceLocation;

        public void SetLocaltion(string clickOnceLocation) {
            _clickOnceLocation = clickOnceLocation;
        }
        public UC_ThuLai(List<TinhThuLaiTheoCIF_Result> tp, List< LayTKThanhToanTheoCIF_Result> ciftk,string phong)
        {
            _tp = tp;
            _ciftk = ciftk;
            _inthulai = new List<ThuLai>();
            _phongthu = phong;
            InitializeComponent();
        }
        public UC_ThuLai( )
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
                    var loaitien = "VND";
                  
                    for (int i = 0; i < GV_TD_Pyctl_ThuLai.DataRowCount; i++)
                    {
                        wsList.InsertRow(row, 1);                
                        wsList.Cells[row, 2].Value = GV_TD_Pyctl_ThuLai.GetRowCellValue(i,"tkvay").ToString();
                        wsList.Cells[row, 3].Value = Convert.ToDecimal(GV_TD_Pyctl_ThuLai.GetRowCellValue(i, "LaiCongDon"));
                        wsList.Cells[row, 3].Style.Numberformat.Format = "#,##0.00;-#,##0.00";
                        wsList.Cells[row, 4].Value = GV_TD_Pyctl_ThuLai.GetRowCellValue(i, "LaiTraCham");
                        wsList.Cells[row, 4].Style.Numberformat.Format = "#,##0.00;-#,##0.00";                       
                        wsList.Cells[row, 6].Value = GV_TD_Pyctl_ThuLai.GetRowCellValue(i, "PhuongThucTra").ToString();
                        wsList.Cells[row, 7].Value = GV_TD_Pyctl_ThuLai.GetRowCellValue(i, "GhiChu").ToString();
                        wsList.Cells[row, 8].Value = GV_TD_Pyctl_ThuLai.GetRowCellValue(i, "LoaiTien").ToString();
                        wsList.Cells[row, 5].Formula = string.Format("Sum(C{0}:D{0})", row );
                        loaitien = GV_TD_Pyctl_ThuLai.GetRowCellValue(i, "LoaiTien").ToString();
                        row++;
                    }
                    string local = "A" + startRow + ":H" + row;
                    var cell = wsList.Cells[local];
                    var border = cell.Style.Border;
                    border.BorderAround(ExcelBorderStyle.Thin);
                    border.Bottom.Style = ExcelBorderStyle.Thin;
                    border.Top.Style = ExcelBorderStyle.Thin;
                    border.Left.Style = ExcelBorderStyle.Thin;
                    border.Right.Style = ExcelBorderStyle.Thin;

                    wsList.Cells["A10"].Value = wsList.Cells["A10"].Value + " " +  GV_TD_Pyctl_ThuLai.GetRowCellValue(1, "Cif");
                    wsList.Cells["A9"].Value = wsList.Cells["A9"].Value + " " +  GV_TD_Pyctl_ThuLai.GetRowCellValue(1, "TenKh");
                    wsList.Cells["E3"].Value = string.Format(@"Ngày {0} tháng {1} nam {2}",DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                    wsList.Cells["A7"].Value = wsList.Cells["A7"].Value + " " +_phongthu;

                    wsList = pck.Workbook.Worksheets[2];
                    wsList.Cells["A1"].LoadFromCollection(_tp, true);
                    wsList.Column(2).Style.Numberformat.Format = "dd/MM/yyyy";
                    wsList.Column(8).Style.Numberformat.Format = "dd/MM/yyyy";
                    wsList.Column(9).Style.Numberformat.Format = "dd/MM/yyyy";
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
            repositoryItemComboBox1.Items.Add("Ti?n M?t");

            foreach (var i in _tp)
            {   var tk = _ciftk.FirstOrDefault(c => c.DDCTYP == i.tiente && c.BRANCH == 740);
                string ptra = "";
                if (tk != null)
                {
                    ptra = string.Format(@"{0:###-##-##-######-#}", tk.ACCTNO);
                }
                else {
                    ptra = "Ti?n M?t";
                }
                _inthulai.Add(new ThuLai()
                {
                    Cif = i.socif,
                    TenKh = i.khachhang,
                    LaiCongDon = i.laicongdon ?? 0,
                    LaiTraCham = i.laitracham ?? 0,
                    SoTienPhaiTra = (i.laicongdon ?? 0) + (i.laitracham ?? 0),
                    PhuongThucTra =ptra,
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
