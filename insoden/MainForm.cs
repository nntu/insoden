using DevExpress.XtraEditors;
using DevExpress.XtraReports.UI;
using insoden.Model;
using Ionic.Zip;
using Lex.Db;
using NLog;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.Deployment.Application;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace insoden
{
    public partial class MainForm : XtraForm
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public string Data2Print;
        public string DiaChi;
        public string TenChiNhanh;
        public string TencnEn;
        public string TencnVi;
        private const int PrinterPort = 8989;
        private readonly dlgocEntities _db;
        private readonly List<ClBc810> _dsbc810 = new List<ClBc810>();
        Config _cf;
        public static string _clickOnceLocation;
        private bdsuEntities _dbbdsu = new bdsuEntities();
        private List<DatainNoCo> _dbin;
        private ObjectResult<DoanhSoTientheoCif_Result> _doanhso;
        private List<ClBc833> _dsbc833 = new List<ClBc833>();
        private List<X1PCMS> _dsX1Pcms;
        private ObjectResult<ThuNoThuLai_Result> _giainganthuno;
        List<InTBDuNovaLaiVay> _InTBDuNovaLaiVay;
        List<InTBNoDenHan> _InTBNoDenHan;
        private List<incif> _lcif;
        private List<GLHIST> _listLsGl = new List<GLHIST>();

        DbInstance _localdb;
        private NetworkStream _ls;
        private List<GLHIST> _lsglhist;
        private DateTime? _ngaydl_loanmonth;
        private ObjectResult<TinhPhiTheoCif_Result> _phi;
        private bool _printed;
        private string _printerIp = "10.141.2.35";
        private ObjectResult<SaoKeCif_Result> _saoke;
        private TcpClient _tcpcl;
        private string _tempdir;

        // List<tk> ltk;
        private ThongTinNganHang _ttnghang;
        private List<dcdienluc> dchieu = new List<dcdienluc>();
        private List<ClBc833> dsbc833 = new List<ClBc833>();
        public MainForm()
        {
            _dbin = new List<DatainNoCo>();
            _db = new dlgocEntities();
            _dbbdsu = new bdsuEntities();
            _db.Database.CommandTimeout = 300000;
            InitializeComponent();

        }

        private delegate void UpdateLabelTextDelegate(string newText);

        public void SendData()
        {
            try
            {
                _tcpcl = new TcpClient();
                _tcpcl.Connect(_printerIp, PrinterPort);
                _ls = _tcpcl.GetStream();
                var bytes = Encoding.ASCII.GetBytes(Data2Print);
                _ls.Write(bytes, 0, bytes.Length);
                bytes = new byte[0x101];
                int count = _ls.Read(bytes, 0, bytes.Length);
                if (String.CompareOrdinal(Encoding.ASCII.GetString(bytes, 0, count).Substring(0, 4), "0000") != 0)
                {
                    MessageBox.Show(@"Lỗi in", @"IN so", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                _printed = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void b_noco_tk_inlientuc_Click(object sender, EventArgs e)
        {
            var datanoco = (IList<DatainNoCo>)dgv_noco_tk.DataSource;
            if (datanoco == null)
            {
                MessageBox.Show(@"Chưa có số liệu");
            }
            else
            {
                var finbc = new Inbclt((IList<DatainNoCo>)dgv_noco_tk.DataSource);

                finbc.Show();
            }
        }

        private void bt_atm_pht_laydl_Click(object sender, EventArgs e)
        {
            if (dtp_atm_pht_nd.Value > dtp_atm_pht_nc.Value)
            {
                MessageBox.Show(@"Ngày đầu > ngày cuối");
            }
            else
            {
                _dsX1Pcms = (from p in _db.X1PCMS
                             where p.DATADATE >= dtp_atm_pht_nd.Value && p.DATADATE <= dtp_atm_pht_nc.Value
                             select p).Distinct().ToList();

                gc_atm_phathanhthe.DataSource = new BindingSource(_dsX1Pcms, "");
                gv_atm_pht.BestFitColumns();
            }
        }

        private void bt_atm_pht_xuatfile_Click(object sender, EventArgs e)
        {
            if (_dsX1Pcms == null || _dsX1Pcms.Count == 0)
            {
                MessageBox.Show(@"Chưa có DL");
            }
            else
            {
                if (SaveFileExcel.ShowDialog() == DialogResult.OK)
                {
                    using (var pck = new ExcelPackage())
                    {
                        var fi = new FileInfo(SaveFileExcel.FileName);

                        if (fi.Exists)
                        {
                            fi.Delete();
                        }
                        var wsList = pck.Workbook.Worksheets.Add("bcATM");
                        wsList.Cells["A1"].LoadFromCollection(_dsX1Pcms, true);
                        wsList.Column(1).Style.Numberformat.Format = "dd/MM/yyyy hh:mm:ss";
                        wsList.Column(17).Style.Numberformat.Format = "dd/MM/yyyy hh:mm:ss";
                        for (int i = 1; i <= wsList.Dimension.End.Column; i++)
                        {
                            wsList.Column(i).AutoFit();
                        }

                        pck.SaveAs(fi);
                    }
                    OpenExplorer(SaveFileExcel.FileName);
                }
            }
        }

        private void Bt_atmXoa_Click(object sender, EventArgs e)
        {

            IQueryable<tbXoaATM> ds = from p in _dbbdsu.tbXoaATMs
                                      where p.ngayxoa >= dtp_atmxoa_ngaydau.Value && p.ngayxoa <= dtp_atmxoa_ngaycuoi.Value
                                      select p;

            dgw_theatmxoa.DataSource = ds.ToList();
        }

        private void bt_dkmayin_Click(object sender, EventArgs e)
        {
            if (tb_dkMayin.Text != "")
            {

                string machineName = Environment.MachineName;
                //      dbbds.tbPrinters.Attach(new tbPrinter() { WorkStation = machineName, printerService = tb_dkMayin.Text.Trim() });
                tbPrinter tbprinter = _dbbdsu.tbPrinters.FirstOrDefault(c => c.WorkStation.Contains(machineName.ToUpper()));
                if (tbprinter == null)
                {
                    _dbbdsu.tbPrinters.Add(new tbPrinter { WorkStation = machineName, printerService = tb_dkMayin.Text.Trim() });

                }
                else
                {
                    tbprinter.printerService = tb_dkMayin.Text.Trim();

                }
                _dbbdsu.SaveChanges();
                MessageBox.Show(@"Đã Thêm / cập nhật");

            }
            else
            {
                MessageBox.Show(@"Chưa Nhập địa chỉ Server printer");
            }
        }

        private void bt_exportexcel_Click(object sender, EventArgs e)
        {
            if (_dbin == null)
            {
                MessageBox.Show(@"Chưa có số liệu");
            }
            else
            {
                if (SaveFileExcel.ShowDialog() == DialogResult.OK)
                {
                    var fi = new FileInfo(SaveFileExcel.FileName);
                    if (fi.Exists)
                    {
                        fi.Delete();
                    }
                    using (var pck = new ExcelPackage())
                    {
                        var wsList = pck.Workbook.Worksheets.Add("saoke");
                        wsList.Cells["A1"].LoadFromCollection(_dbin, true);
                        /*
                        for (int i = 1; i <= wsList.Dimension.End.Column; i++)
                        {
                            if (i == 14 || i == 16)
                            {
                            }

                            wsList.Column(i).AutoFit();
                        }
                        */
                        wsList.Cells.AutoFitColumns();
                        wsList.Column(15).Style.Numberformat.Format = "dd/MM/yyyy hh:mm:ss";
                        wsList.Column(17).Style.Numberformat.Format = "dd/MM/yyyy hh:mm:ss";
                        wsList.Column(8).Style.Numberformat.Format = "#,##0.0";
                        wsList.Column(9).Style.Numberformat.Format = "#,##0.0";

                        pck.SaveAs(fi);
                    }
                    OpenExplorer(SaveFileExcel.FileName);
                }
            }
        }

        private void bt_exportexcel_saoke_Click(object sender, EventArgs e)
        {
            var pathToFile = Path.Combine(_clickOnceLocation, @"saoketk.xlsx");
            if (_dbin == null || _dbin.Count == 0)
            {
                MessageBox.Show(@"Chưa có DL");
            }
            else
            {
                SaveFileExcel.FileName = "saoke_";
                if (SaveFileExcel.ShowDialog() == DialogResult.OK)
                {
                    var temp = new FileInfo(pathToFile);
                    using (var pck = new ExcelPackage(temp))
                    {
                        ExcelWorksheet wsList = pck.Workbook.Worksheets[1];

                        const int startRow = 11;

                        int row = startRow;

                        foreach (var dd in _dbin)
                        {
                            decimal ghico = 0;
                            decimal ghino = 0;

                            // we have our total formula on row 7, so push them down so we can insert more data
                            //  if (row > startRow) wsList.InsertRow(1,row);

                            if (dd.loai.ToUpper().Equals("CO"))
                            {
                                ghico = dd.sotien;
                            }
                            else
                            {
                                ghino = dd.sotien;
                            }
                            wsList.InsertRow(row, 1);
                            wsList.Cells[row, 1].Value = dd.ngaygiaodich;
                            wsList.Cells[row, 1].Style.Numberformat.Format = "dd/MM/yyyy hh:mm:ss";
                            wsList.Cells[row, 1].Style.WrapText = true;

                            wsList.Cells[row, 2].Value = dd.trancd;

                            wsList.Cells[row, 3].Value = dd.loaitien;

                            wsList.Cells[row, 4].Value = ghino;
                            wsList.Cells[row, 4].Style.Numberformat.Format = "#,##0.00;-#,##0.00";

                            wsList.Cells[row, 5].Value = ghico;
                            wsList.Cells[row, 5].Style.Numberformat.Format = "#,##0.00;-#,##0.00";

                            wsList.Cells[row, 6].Value = dd.sodu;
                            wsList.Cells[row, 6].Style.Numberformat.Format = "#,##0.00;-#,##0.00";

                            wsList.Cells[row, 7].Value = dd.ghichu.Trim();
                            wsList.Cells[row, 7].Style.WrapText = true;

                            wsList.Cells[row, 8].Value = dd.truser.Trim();
                            wsList.Cells["D4"].Value = string.Format(@"Số tài khoản : {0}", dd.tk);
                            wsList.Cells["D5"].Value = string.Format(@"Tên tài khoản : {0}", dd.tentk);
                            wsList.Cells["D6"].Value = string.Format(@"Mã khách hàng: {0}", dd.cifno);
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

                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                        wsList.Cells["D3"].Value = string.Format(@"Từ ngày {0} đến ngày {1} ", dtp_tungay.Value, dtp_denngay.Value);

                        //for (int i = 1; i <= wsList.Dimension.End.Column; i++)
                        //{
                        //    wsList.Column(i).AutoFit();
                        //}
                        var fi = new FileInfo(SaveFileExcel.FileName);

                        if (fi.Exists)
                        {
                            fi.Delete();
                        }
                        pck.SaveAs(fi);
                    }
                    OpenExplorer(SaveFileExcel.FileName);
                }
            }
        }

        private void bt_gl_erp_laytt_Click(object sender, EventArgs e)
        {
            string tk = "";
            if (cb_gl_lsgl_tk.SelectedItem != null)
            {
                tk = cb_gl_lsgl_tk.SelectedItem.ToString();
            }
            else
            {
                tk = cb_gl_lsgl_tk.Text;
            }
            if (tk.Trim() == "")
            {
                MessageBox.Show(@"Chưa chọn TK GL");
            }
            else
            {
                //  IQueryable<GLHIST_ERP> listgl;

                var gl = Lib.ExtractNumber(tk.Trim());
                var glacc = Convert.ToDecimal(gl.Trim());
                var ngaybd = (DateTime)de_gl_erp_datdau.EditValue;
                var ngaykt = (DateTime)de_gl_erp_ketthuc.EditValue;
                var tiente = (string)cb_gl_erp_loaitien.SelectedItem;
                var bds = Convert.ToInt32(cb_gl_erp_bds.SelectedItem);
                if (tiente == null)
                {
                    tiente = "VND";
                }
                var lsgl = _db.tracuulsgl(glacc, ngaybd, ngaykt, bds, tiente.Trim());

                //       _listLsGl = listgl.ToList();
                gc_gl_lsglerp.DataSource = new BindingSource(lsgl, "");
                gv_gl_lsglerp.BestFitColumns();
            }
        }

        private void bt_gl_erp_xuatexcel_Click(object sender, EventArgs e)
        {
            if (SaveFileExcel.ShowDialog() == DialogResult.OK)
            {
                gv_gl_lsglerp.OptionsPrint.AutoWidth = false;
                gv_gl_lsglerp.BestFitColumns();
                gv_gl_lsglerp.ExportToXlsx(SaveFileExcel.FileName);
            }
            OpenExplorer(SaveFileExcel.FileName);
        }

        private void bt_gl_LayTonQuy_Click(object sender, EventArgs e)
        {
            var tungay = (DateTime)de_ql_tq_tungay.EditValue;
            var denngay = (DateTime)de_ql_tq_dengay.EditValue;

            var datagltq = _db.TonQuy(tungay, denngay);

            RW_gc_gl_tonquy.DataSource = new BindingSource(datagltq, "");
        }

        private void bt_gl_TQ_XuatExlcel_Click(object sender, EventArgs e)
        {
            SaveFileExcel.FileName = "tonquy_";
            if (SaveFileExcel.ShowDialog() == DialogResult.OK)
            {
                var fi = new FileInfo(SaveFileExcel.FileName);

                if (fi.Exists)
                {
                    fi.Delete();
                }
                RW_gc_gl_tonquy.ExportToXlsx(SaveFileExcel.FileName);
            }
            OpenExplorer(SaveFileExcel.FileName);
        }

        private void bt_gl_trc_laytt_Click(object sender, EventArgs e)
        {
            var tk = cb_gl_tracuugl_tk.SelectedItem.ToString();
            if (tk == "")
            {
                MessageBox.Show(@"Chưa nhập tk GL");
            }
            else
            {
                var gl = Lib.ExtractNumber(tk.Trim());
                var tkgl = Convert.ToDecimal(gl);
                var ngaybd = (DateTime)de_gl_trc_nbd.EditValue;
                var ngaykt = (DateTime)de_gl_trc_nkt.EditValue;

                var tracuugl = _db.TraCuuGLERP(tkgl, ngaybd, ngaykt);

                gridControl2.DataSource = new BindingSource(tracuugl, "");
                gridView2.BestFitColumns();
            }
        }


        private void bt_in_Click(object sender, EventArgs e)
        {
            if (_lcif == null)
            {
                MessageBox.Show(@"Chưa có số liệu");
            }
            else
            {
                if (cb_ipdc_ck.Checked)
                {
                    var finbc = new frmInpdcGD(_lcif, _ttnghang);
                    finbc.Show();
                    //XRPDC rep = new XRPDC();
                    //rep.DataSource = _lcif;
                    //rep.RequestParameters = false;
                    //rep.ods_incif.DataSource = _lcif;
                    //rep.ods_ttnganhang.DataSource = _ttnghang;
                    //rep.Parameters["NgayDC"].Value = string.Format("Ngày {0} tháng {1} năm {2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);


                    //using (ReportPrintTool printTool = new ReportPrintTool(rep))
                    //{
                    //    // Invoke the Ribbon Print Preview form modally, 
                    //    // and load the report document into it.
                    //    printTool.ShowRibbonPreviewDialog();

                    //    // Invoke the Ribbon Print Preview form
                    //    // with the specified look and feel setting.

                    //}

                }
                else
                {
                    var finbc = new Inpdc(_lcif, _ttnghang);
                    finbc.Show();
                }
            }
        }

        private void bt_in_tk_Click(object sender, EventArgs e)
        {
            var data = (IList<DatainNoCo>)dgv_noco_tk.DataSource;
            if (data == null)
            {
                MessageBox.Show(@"Chưa có số liệu");
            }
            else
            {
                var finbc = new Inbc((IList<DatainNoCo>)dgv_noco_tk.DataSource);

                finbc.Show();
                //XRInNCA5 rep = new XRInNCA5();
                //rep.DataSource = (IList<DatainNoCo>)dgv_noco_tk.DataSource;
                //using (ReportPrintTool printTool = new ReportPrintTool(rep))
                //{

                //    // Invoke the Ribbon Print Preview form modally, 
                //    // and load the report document into it.
                //    printTool.ShowRibbonPreviewDialog();

                //    // Invoke the Ribbon Print Preview form
                //    // with the specified look and feel setting.

                //}

            }
        }

        private void bt_insosec_Click(object sender, EventArgs e)
        {
            if (tb_tenKh.Text.Trim().Length == 0)
            {
                MessageBox.Show(@"Chưa có tên Kh");
            }
            else
            {
                string branchname = tb_tenchinhanh.Text.Trim();
                string address = tb_diachi.Text.Trim();
                string fullname = tb_tenKh.Text.Trim();
                string soCa = mtb_tk.Text;
                string str3;
                if ((branchname + " " + address).Length > 0x4d)
                {
                    str3 = (branchname + "," + address).Substring(0, 0x4d);
                }
                else
                {
                    str3 = branchname + "," + address;
                }

                string str = soCa + " tai " + str3;
                string str2 = "~n 1 ";
                str2 = (str2 + " ~V 13 ~H 40 ~t " + fullname) + " ~V 14 ~H 45 ~t " + Lib.Split_ALong_Line(str, 40);

                int num = Convert.ToInt32(mtb_solanin.Text.Trim());
                lb_solanin.Text = mtb_solanin.Text.Trim();

                Data2Print = str2;

                if (!_printed)
                {
                    new Thread(() =>
                    {
                        for (int i = 1; i <= num; i++)
                        {
                            UpdateLabelText((num - i).ToString(CultureInfo.InvariantCulture));
                            SendData();
                        }
                    }).Start();

                    _printed = true;
                }
            }
        }

        private void bt_laythontin_Click(object sender, EventArgs e)
        {
            LayThongTinTheoTk();
        }

        private void bt_locdl_tk_Click(object sender, EventArgs e)
        {
            List<DatainNoCo> data = _dbin;

            if (data == null || data.Count == 0)
            {
                MessageBox.Show(@"Chưa có số liệu");
            }
            else
            {
                if (clb_Locdl.GetItemChecked(0))
                {
                    dgv_noco_tk.DataSource = null;
                    tb_somon.Text = data.Count.ToString(CultureInfo.InvariantCulture);
                    dgv_noco_tk.DataSource = data;
                }
                else
                {
                    List<DatainNoCo> temp = data.ToList();

                    for (int i = 1; i < clb_Locdl.Items.Count; i++)
                    {
                        if (!clb_Locdl.GetItemChecked(i))
                        {
                            string t = clb_Locdl.Items[i].ToString();
                            string loc = t.Split('|')[0];
                            temp.RemoveAll(x => x.auxtrc.Trim() == loc);
                        }
                    }

                    dgv_noco_tk.DataSource = null;
                    tb_somon.Text = temp.Count.ToString(CultureInfo.InvariantCulture);
                    dgv_noco_tk.DataSource = temp;
                    dgv_noco_tk.Refresh();
                }
            }
        }

        private void bt_lsgl_laydl_Click(object sender, EventArgs e)
        {
            if (tb_lsgl_tkgl.Text.Trim() == "")
            {
                MessageBox.Show(@"Chưa nhập TK GL");
            }
            else
            {
                IQueryable<GLHIST> listgl;
                decimal glacc = Convert.ToDecimal(tb_lsgl_tkgl.Text.Trim());
                var ngaybd = (DateTime)dtp_lsgl_ngaybd.EditValue;
                var ngaykt = (DateTime)dtp_lsgl_ngaykt.EditValue;
                var tiente = (string)cb_gl_tiente.SelectedItem;
                var bds = (string)cb_lsgl_bds.SelectedItem;

                listgl = from p in _db.GLHISTs
                         where p.GTACCT == glacc && (p.DataDate >= ngaybd.Date && p.DataDate <= ngaykt.Date)
                         where p.TRCTYP == tiente
                         where p.BRANCH == bds
                         orderby p.TRSEQ descending
                         orderby p.DataDate

                         select p;

                _listLsGl = listgl.ToList();
                GL_gc_lsgl.DataSource = new BindingSource(_listLsGl, "");
                GL_gv_lsgl.BestFitColumns();
            }
        }

        private void bt_lsgl_xuat_Click(object sender, EventArgs e)
        {
            var pathToFile = Path.Combine(_clickOnceLocation, @"gl_70217.xlsx");

            if (_listLsGl == null || _listLsGl.Count == 0)
            {
                MessageBox.Show(@"Chưa có DL");
            }
            else
            {
                SaveFileExcel.FileName = "gl_";
                if (SaveFileExcel.ShowDialog() == DialogResult.OK)
                {
                    var temp = new FileInfo(pathToFile);
                    using (var pck = new ExcelPackage(temp))
                    {
                        decimal glacc = Convert.ToDecimal(tb_lsgl_tkgl.Text.Trim());

                        var r = _db.GLMASTs.GroupBy(c => c.ACCTGL, c => c.TITLE, (key, g) => new { gl = key, title = g.FirstOrDefault() }).FirstOrDefault(c => c.gl == glacc);

                        ExcelWorksheet wsList = pck.Workbook.Worksheets[1];

                        const int startRow = 13;

                        int row = startRow;
                        wsList.Cells[6, 3].Value = dtp_lsgl_ngaybd.EditValue;
                        wsList.Cells[6, 6].Value = dtp_lsgl_ngaykt.EditValue;
                        wsList.Cells["C7"].Value = tb_lsgl_tkgl.EditValue;
                        wsList.Cells["C9"].Value = cb_gl_tiente.SelectedItem;
                        if (r != null) wsList.Cells["F7"].Value = r.title;

                        foreach (var glhist in _listLsGl)
                        {
                            decimal ghico = 0;
                            decimal ghino = 0;

                            // we have our total formula on row 7, so push them down so we can insert more data
                            //  if (row > startRow) wsList.InsertRow(1,row);

                            if (glhist.TRDORC.Equals("C"))
                            {
                                ghico = -glhist.TRAMT;
                            }
                            else
                            {
                                ghino = glhist.TRAMT;
                            }
                            wsList.InsertRow(row, 1);
                            wsList.Cells[row, 1].Value = glhist.DataDate;
                            wsList.Cells[row, 2].Value = glhist.LMDATE;
                            wsList.Cells[row, 3].Value = ghino;

                            wsList.Cells[row, 4].Value = ghico;
                            wsList.Cells[row, 5].Value = "M";
                            wsList.Cells[row, 6].Value = glhist.TRSEQ;
                            wsList.Cells[row, 7].Value = (glhist.TRDESC + glhist.TREFTH).Trim();
                            row++;
                        }
                        var sumno = "C" + row;
                        var sumco = "D" + row;
                        wsList.Cells[sumno].Formula = string.Format("Sum(C12:C{0})", row - 1);
                        wsList.Cells[sumco].Formula = string.Format("Sum(D12:D{0})", row - 1);

                        wsList.Column(1).Style.Numberformat.Format = "dd/MM/yyyy";
                        wsList.Column(2).Style.Numberformat.Format = "dd/MM/yyyy";
                        wsList.Column(3).Style.Numberformat.Format = "#,##0.00;-#,##0.00";
                        wsList.Column(4).Style.Numberformat.Format = "#,##0.00;-#,##0.00";

                        wsList.Cells[6, 3].Style.Numberformat.Format = "dd/MM/yyyy";
                        wsList.Cells[6, 6].Style.Numberformat.Format = "dd/MM/yyyy";

                        for (int i = 1; i <= wsList.Dimension.End.Column; i++)
                        {
                            wsList.Column(i).AutoFit();
                        }
                        var fi = new FileInfo(SaveFileExcel.FileName);

                        if (fi.Exists)
                        {
                            fi.Delete();
                        }
                        pck.SaveAs(fi);
                    }
                    OpenExplorer(SaveFileExcel.FileName);
                }
            }
        }

        private void bt_n_ttnt_laydl_Click(object sender, EventArgs e)
        {
            var ngoaite = _db.MuaBanNgoaiTe(dtp_n_ttnt_ngay.Value).ToList();

            var tygia = _db.laytygiangay(dtp_n_ttnt_ngay.Value);
            if (ngoaite.Count > 0)
            {
                gc_nguon_ttnt.DataSource = new BindingSource(ngoaite, "");
                gv_nguon_ttnt.OptionsView.ColumnAutoWidth = false;
                gv_nguon_ttnt.OptionsView.BestFitMaxRowCount = -1;
                gv_nguon_ttnt.BestFitColumns();

                gc_nguon_ttnt_tygia.DataSource = new BindingSource(tygia, "");
                gv_nguon_ttnt_tygia.OptionsView.ColumnAutoWidth = false;
                gv_nguon_ttnt_tygia.OptionsView.BestFitMaxRowCount = -1;
                gv_nguon_ttnt_tygia.BestFitColumns();
                var sumqdusd = ngoaite.Sum(p => p.qdusd);
                textBox1.Text = $"{sumqdusd:0,0.00}";
            }
            else
            {
                MessageBox.Show(@"Chưa có dl");
            }
        }

        private void bt_phieudoichieu_Click(object sender, EventArgs e)
        {
            NameValueCollection config;

            if (cb_bds_pdc.SelectedItem.ToString() == "741")
            {
                config = (NameValueCollection)ConfigurationManager.GetSection("customAppSettingsGroup/pdc_cantho");
            }
            else
            {
                config = (NameValueCollection)ConfigurationManager.GetSection("customAppSettingsGroup/pdc_tranoc");
            }

            TenChiNhanh = config["TenChiNhanh"];
            DiaChi = config["DiaChi"];
            _ttnghang = new ThongTinNganHang
            {
                tencn_vi = config["tencn_vi"],
                tencn_en = config["tencn_en"],
                diachi = config["diachi"],
                tinh = config["tinh"],
                fax = config["fax"],
                dt = config["dt"],
                tennguoiky = config["tennguoiky"],
                chucdanh = config["chucdanh"],
                noinhan = config["noinhan"],
                tenfilechuky = config["tenfilechuky"]

                //ngaycuoiky = ConfigurationManager.AppSettings["ngaycuoiky"]
            };
            string check = Check_checkbox();
            int bds = Convert.ToInt32(cb_bds_pdc.SelectedItem);
            if (tb_cif_pdc.Text == "")
            {
                MessageBox.Show(@"Chưa nhập Cif");
            }
            else
            {
                List<PhieuDoiChieu_Result> da =
                    _db.PhieuDoiChieu(Convert.ToDecimal(tb_cif_pdc.Text), ipdc_date.Value, bds, check).ToList();
                _lcif = new List<incif>();

                foreach (PhieuDoiChieu_Result i in da)
                {
                    string acctno = i.tk.ToString();
                    string cfname1 = i.acname;

                    string currtyp = i.loaitt;
                    string acctyp = i.loaitk;
                    string addr1 = i.addr1;
                    string cfindi = i.cif.ToString();
                    double cbal = Convert.ToDouble(i.sodu);

                    var prov = new MaskedTextProvider("###-##-##-######-#");
                    prov.Set(acctno);
                    string formattk = prov.ToDisplayString();

                    _lcif.Add(new incif
                    {
                        acctno = formattk,
                        cbal = cbal,
                        currtyp = currtyp,
                        acctyp = acctyp,
                        cfindi = cfindi
                    });
                    _ttnghang.cifno = tb_cif_pdc.Text;
                    _ttnghang.cfname1 = cfname1;
                    //  ttnghang.ngaycuoiky = String.Format(format: "ngày {0:dd} tháng {0:MM} năm {0:yyy}", arg0: ipdc_date.Value);
                    _ttnghang.ngaycuoiky = ipdc_date.Value;
                    _ttnghang.addr1 = addr1;
                }
                dgv_ipdc.DataSource = _lcif;
            }
        }

        private void bt_rw_bx810_loadfile_Click(object sender, EventArgs e)
        {
            var fd = new OpenFileDialog();
            if (fd.ShowDialog() == DialogResult.OK) // Test result.
            {
                string filereport = Path.GetFileName(fd.FileName);

                if (filereport != null)
                {
                    string bds = filereport.Substring(filereport.IndexOf("_", StringComparison.Ordinal) + 1, 3);
                    string mainstring = "BIDVBITCKT4" + filereport;

                    var password = new string(mainstring.ToCharArray().Reverse().ToArray());
                    using (var zip = ZipFile.Read(fd.FileName))
                    {
                        string bc = "ISW810P";
                        var a = zip[bc + bds];
                        if (a == null)
                        {
                            MessageBox.Show(@"Khong co bc 810");
                        }
                        else
                        {
                            a.ExtractWithPassword(_tempdir, ExtractExistingFileAction.OverwriteSilently, password);
                            using (var sr = new StreamReader(_tempdir + "\\" + bc + bds))
                            {
                                string line;
                                // Read and display lines from the file until the end of
                                // the file is reached.

                                while ((line = sr.ReadLine()) != null)
                                {
                                    if (line.Length == 85)
                                    {
                                        string stt = line.Substring(4, 7).Trim();
                                        string sothe = line.Substring(12, 20).Trim();
                                        string hoten = line.Substring(33, 40).Trim();
                                        string ngaythang = line.Substring(73).Trim().Replace("  ", " ");
                                        if (ngaythang.Length == 7) { ngaythang = "0" + ngaythang; }
                                        DateTime r = DateTime.ParseExact(ngaythang, "dd/MM/yy", CultureInfo.InvariantCulture);

                                        _dsbc810.Add(new ClBc810
                                        {
                                            Stt = Convert.ToInt32(stt),
                                            SoThe = sothe,
                                            HoTen = hoten,
                                            Ngaystr = ngaythang,
                                            NgayMo = r
                                        });
                                    }
                                }
                            }

                            RW_gc_bc810.DataSource = new BindingSource(_dsbc810, "");
                            RW_gv_bc810.BestFitColumns();
                        }
                    }
                }
            }
        }

        private void bt_rw_bx833_loadfile_Click(object sender, EventArgs e)
        {
            //try
            //{
            var formats = new[] { @"dd/MM/yy hh:mm:ss", @"dd/MM/yy HH:mm:ss", @"d/MM/yy HH:mm:ss", @"d/MM/yy H:mm:ss", @"d/MM/yy hh:mm:ss", @"d/MM/yy h:mm:ss", @"d/MM/yy hh:mm:ss", @"d/MM/yy mm:ss", @"dd/MM/yy ss", @"dd/MM/yy" };
            dsbc833.Clear();
            var fd = new OpenFileDialog();
            if (fd.ShowDialog() == DialogResult.OK) // Test result.
            {
                string filereport = Path.GetFileName(fd.FileName);
                var ngaybc = filereport.Substring(2, 6);

                var reportdate = DateTime.ParseExact(ngaybc.Trim(), new string[] { "yyMMdd" },
                                          CultureInfo.InvariantCulture,
                                             DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal);
                if (filereport != null)
                {
                    string bds = filereport.Substring(filereport.IndexOf("_", StringComparison.Ordinal) + 1, 3);
                    string mainstring = "BIDVBITCKT4" + filereport;

                    var password = new string(mainstring.ToCharArray().Reverse().ToArray());

                    using (ZipFile zip = ZipFile.Read(fd.FileName))
                    {
                        string bc = "ISW833P";
                        ZipEntry a = zip[bc + bds];
                        if (a == null)
                        {
                            MessageBox.Show(@"Khong co bc 833");
                        }
                        else
                        {
                            a.ExtractWithPassword(_tempdir, ExtractExistingFileAction.OverwriteSilently, password);
                            using (var sr = new StreamReader(_tempdir + "\\" + bc + bds))
                            {
                                String line;
                                // Read and display lines from the file until the end of
                                // the file is reached.
                                string status = "";
                                while ((line = sr.ReadLine()) != null)
                                {
                                    if (reportdate.CompareTo(new DateTime(2017, 10, 28)) < 0)
                                    {
                                        if (line.IndexOf("CARD STATUS", StringComparison.Ordinal) != -1)
                                        {
                                            if (line.Length < 50)
                                            {
                                                status = line.Substring(line.IndexOf(":", StringComparison.Ordinal),
                                                    line.Length - line.IndexOf(":", StringComparison.Ordinal));
                                                status = status.Replace(":", "").Trim();
                                            }
                                        }

                                        if (line.Length == 76)
                                        {
                                            string stt = line.Substring(4, 7).Trim();
                                            string sothe = line.Substring(12, 20).Trim();
                                            string hoten = line.Substring(33, 25).Trim();
                                            string ngaythang = line.Substring(57).Trim().Replace("  ", " ").Replace("   ", " ");

                                            DateTime dateValue = DateTime.ParseExact(ngaythang.Trim(), formats,
                                            CultureInfo.InvariantCulture,
                                               DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal);
                                            double songayqh = 0;
                                            var ngayhientai = dateTimePicker1.Value;
                                            if (status.Trim() == "RECV")
                                            {
                                                songayqh = ngayhientai.Subtract(dateValue).TotalDays;
                                            }
                                            dsbc833.Add(new ClBc833
                                            {
                                                Stt = Convert.ToInt32(stt),
                                                SoThe = sothe,
                                                HoTen = hoten,
                                                Ngaystr = ngaythang,
                                                TrangThai = status,
                                                NgayMo = dateValue,
                                                SoNgayQuaHan = songayqh,
                                                QuaHan90ngay = songayqh >= 90 ? "Y" : ""
                                            });
                                        }
                                    }
                                    else
                                    {
                                        if (line.Length >= 77 && line.Length <= 119)
                                        {
                                            if (line.IndexOf("BIDV BANK", StringComparison.Ordinal) == -1 && line.IndexOf("NO  CARD", StringComparison.Ordinal) == -1 && line.IndexOf("CHECKED BY", StringComparison.Ordinal) == -1)
                                            {
                                                string stt = line.Substring(4, 7).Trim();
                                                string sothe = line.Substring(12, 20).Trim();
                                                string hoten = line.Substring(33, 25).Trim();
                                                string ngaythang = line.Substring(57, 10).Trim().Replace("  ", " ").Replace("   ", " ");
                                                if (line.Length == 77) { status = line.Substring(70, 7).Trim(); }
                                                else
                                                {
                                                    status = line.Substring(70, 10).Trim();
                                                }
                                                DateTime dateValue = DateTime.ParseExact(ngaythang.Trim(), new string[] { "d/MM/yy", "dd/MM/yy" },
                                                    CultureInfo.InvariantCulture,
                                                       DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal);
                                                string am = "";
                                                string teller = "";
                                                string super = "";
                                                double songayqh = 0;
                                                var ngayhientai = dateTimePicker1.Value;
                                                if (line.Length > 107)
                                                {

                                                    am = line.Substring(107).Trim();
                                                }
                                                if (line.Length >= 80)
                                                    teller = line.Substring(80, 15).Trim();

                                                if (line.Length >= 95)
                                                {
                                                    if (line.Length - 95 >= 10)
                                                    {
                                                        super = line.Substring(95, 10).Trim();
                                                    }
                                                    else
                                                    {
                                                        super = line.Substring(95, line.Length - 95).Trim();
                                                    }
                                                }
                                                if (status.Trim() == "RECV")
                                                {
                                                    songayqh = ngayhientai.Subtract(dateValue).TotalDays;
                                                }
                                                dsbc833.Add(new ClBc833
                                                {
                                                    Stt = Convert.ToInt32(stt),
                                                    SoThe = sothe,
                                                    HoTen = hoten,
                                                    Ngaystr = ngaythang,
                                                    TrangThai = status,
                                                    NgayMo = dateValue,
                                                    SoNgayQuaHan = songayqh,
                                                    QuaHan90ngay = songayqh >= 90 ? "Y" : "",
                                                    am = am,
                                                    Teller = teller,
                                                    Supervisor = super

                                                });
                                            }
                                        }
                                    }
                                }
                            }

                            var temp = (from p in dsbc833

                                        join d in _db.X1PCMS
                                            on new
                                            {
                                                p.SoThe,
                                                p.TrangThai
                                            }
                                            equals
                                            new
                                            {
                                                SoThe = d.CARDD?.Trim() ?? string.Empty,
                                                TrangThai = d.CDSTAT?.Trim() ?? string.Empty
                                            }
                                            into g
                                        from su in g.DefaultIfEmpty()

                                        join c in _dbbdsu.tbsothes
                                            on new
                                            {
                                                p.SoThe,
                                                p.TrangThai
                                            }
                                            equals
                                            new
                                            {
                                                SoThe = c.masothe,
                                                TrangThai = c.trangthai
                                            }
                                            into gj
                                        from subpet in gj.DefaultIfEmpty()

                                        select new ClBc833
                                        {
                                            Stt = p.Stt,
                                            SoThe = p.SoThe,
                                            HoTen = p.HoTen,
                                            Ngaystr = p.Ngaystr,
                                            TrangThai = p.TrangThai,
                                            NgayMo = p.NgayMo,
                                            NguoiMo = (su == null ? subpet == null ? string.Empty : subpet.usertacdong : su.OPER),
                                            Teller = p.Teller,
                                            Supervisor = p.Supervisor,
                                            SoNgayQuaHan = p.SoNgayQuaHan,
                                            am = p.am,
                                            QuaHan90ngay = p.QuaHan90ngay

                                        }).ToList();
                            _dsbc833 = temp;
                            RW_gc_bc833.DataSource = new BindingSource(_dsbc833, "");
                            RW_gv_bc833.BestFitColumns();
                        }
                    }
                }
            }
            //}
            //catch (Exception ex)
            //{

            //    logger.Error(ex);

            //}
        }

        private void bt_rw_bx833_xuatexcel_Click(object sender, EventArgs e)
        {
            if (_dsbc833.Count == 0)
            {
                MessageBox.Show(@"Chưa có DL");
            }
            else
            {
                if (SaveFileExcel.ShowDialog() == DialogResult.OK)
                {
                    using (var pck = new ExcelPackage())
                    {
                        var fi = new FileInfo(SaveFileExcel.FileName);

                        if (fi.Exists)
                        {
                            fi.Delete();
                        }
                        ExcelWorksheet wsList = pck.Workbook.Worksheets.Add("bc833");
                        wsList.Cells["A1"].LoadFromCollection(_dsbc833, true);
                        wsList.Column(5).Style.Numberformat.Format = "dd/MM/yyyy hh:mm:ss";
                        for (int i = 1; i <= wsList.Dimension.End.Column; i++)
                        {
                            wsList.Column(i).AutoFit();
                        }
                        wsList = pck.Workbook.Worksheets.Add("bc833 1");
                        wsList.Cells["A1"].LoadFromCollection(dsbc833, true);
                        pck.SaveAs(fi);
                    }
                    OpenExplorer(SaveFileExcel.FileName);
                }
            }
        }

        private void bt_saokeds_Click(object sender, EventArgs e)
        {
            var cif = mtb_sk_cif.Text;
            if (cif == "")
            {
                MessageBox.Show(@"Chưa nhập cif");
            }
            else
            {
                _doanhso = _db.DoanhSoTientheoCif(dtp_sk_ngaydau.Value, dtp_sk_ngaycuoi.Value, Convert.ToDecimal(cif));

                gc_skds_doanhso.DataSource = new BindingSource(_doanhso, "");

                _giainganthuno = _db.ThuNoThuLai(dtp_sk_ngaydau.Value,
                    dtp_sk_ngaycuoi.Value, Convert.ToInt32(cif));
                gc_skds_thunothulai.DataSource = new BindingSource(_giainganthuno, "");

                _saoke = _db.SaoKeCif(dtp_sk_ngaydau.Value, dtp_sk_ngaycuoi.Value,
                    Convert.ToInt32(cif));

                gc_skds_saoke.DataSource = new BindingSource(_saoke, "");
                _phi = _db.TinhPhiTheoCif(dtp_sk_ngaydau.Value, dtp_sk_ngaycuoi.Value,
                    Convert.ToInt32(cif));

                gc_skds_thuphi.DataSource = new BindingSource(_phi, "");

                gv_skds_doanhso.OptionsView.ColumnAutoWidth = false;
                gv_skds_doanhso.OptionsView.BestFitMaxRowCount = -1;
                gv_skds_doanhso.BestFitColumns();

                gv_skds_thuphi.OptionsView.ColumnAutoWidth = false;
                gv_skds_thuphi.OptionsView.BestFitMaxRowCount = -1;
                gv_skds_thuphi.BestFitColumns();

                gv_skds_saoke.OptionsView.ColumnAutoWidth = false;
                gv_skds_saoke.OptionsView.BestFitMaxRowCount = -1;
                gv_skds_saoke.BestFitColumns();
            }
        }

        private void bt_sk_pos_ltt_Click(object sender, EventArgs e)
        {
            var phipos = _db.phiPos(dtk_sk_pos_bd.Value, dtk_sk_pos_kt.Value);
            gc_sk_phipos.DataSource = new BindingSource(phipos, "");

            gv_sk_phipos.OptionsView.ColumnAutoWidth = false;
            gv_sk_phipos.OptionsView.BestFitMaxRowCount = -1;
            gv_sk_phipos.BestFitColumns();
        }

        private void bt_SK_xuatxls_Click(object sender, EventArgs e)
        {
            if (_saoke == null)
            {
                MessageBox.Show(@"Chưa có dữ liệu");
            }
            else
            {
                if (SaveFileExcel.ShowDialog() == DialogResult.OK)
                {
                    using (var pck = new ExcelPackage())
                    {
                        var fi = new FileInfo(SaveFileExcel.FileName);

                        if (fi.Exists)
                        {
                            fi.Delete();
                        }
                        MemoryStream stream = new MemoryStream();
                        gv_skds_saoke.OptionsPrint.AutoWidth = false;

                        gv_skds_saoke.BestFitColumns();
                        gc_skds_saoke.ExportToXlsx(stream);

                        pck.Workbook.Worksheets.Add("saoke", new ExcelPackage(stream).Workbook.Worksheets[1]);
                        stream.SetLength(0);
                        gv_skds_doanhso.OptionsPrint.AutoWidth = false;

                        gv_skds_doanhso.BestFitColumns();
                        gc_skds_doanhso.ExportToXlsx(stream);
                        pck.Workbook.Worksheets.Add("saokeds", new ExcelPackage(stream).Workbook.Worksheets[1]);
                        stream.SetLength(0);
                        gv_skds_thunothulai.OptionsPrint.AutoWidth = false;

                        gv_skds_thunothulai.BestFitColumns();

                        gc_skds_thunothulai.ExportToXlsx(stream);
                        pck.Workbook.Worksheets.Add("thunothulai", new ExcelPackage(stream).Workbook.Worksheets[1]);
                        stream.SetLength(0);
                        gv_skds_thuphi.OptionsPrint.AutoWidth = false;
                        gv_skds_thuphi.BestFitColumns();
                        gc_skds_thuphi.ExportToXlsx(stream);
                        pck.Workbook.Worksheets.Add("thuphi", new ExcelPackage(stream).Workbook.Worksheets[1]);

                        pck.SaveAs(fi);
                    }
                    OpenExplorer(SaveFileExcel.FileName);
                }
            }
        }



        private void bt_tbdn_laysl_Click(object sender, EventArgs e)
        {
            var cifstring = cb_td_tbdn_tk.Text.ToString();
            var cifno = Lib.ExtractNumber(cifstring);

            if (cifno.Trim() == "")
            {
                MessageBox.Show("Chưa nhập cif");
            }
            else
            {
                var cif = Convert.ToDecimal(cifno);

                var tbdn = _db.ThongBaoDuNo(cif, dtp_td_tbdn.Value).AsEnumerable().Cast<ThongBaoDuNo_Result>().ToList();

                _InTBDuNovaLaiVay = new List<InTBDuNovaLaiVay>();

                if (tbdn.Count == 0)
                {
                    MessageBox.Show("Sai Cif hoặc cif không có tk vay");
                }
                else
                {
                    var ciflocal = _localdb.Table<CifInfo>().FirstOrDefault(c => c.cifno == cif);

                    if (ciflocal == null)
                    {
                        _localdb.Table<CifInfo>().Save(new CifInfo()
                        {
                            cifno = tbdn.ToList()[0].socif,
                            acname = tbdn.ToList()[0].khachhang
                        });

                    }

                    foreach (var i in tbdn)
                    {
                        _InTBDuNovaLaiVay.Add(new InTBDuNovaLaiVay()
                        {
                            Socif = i.socif,
                            DuNo = (decimal)i.duno,
                            LaiCongDon = i.laicd ?? 0,
                            LaiPhaiTra = i.laicd ?? 0 + i.laiphat ?? 0,
                            TaiKhoan = string.Format(@"{0:###-##-##-######-#}", i.taikhoan),
                            LaiPhat = i.laiphat ?? 0,
                            LoaiVay = i.loaisaoke,
                            LoaiTien = i.tiente
                            ,
                            TenKhachHang = i.khachhang,
                            NgayDL = i.ngaydl.Value
                        });

                    }
                }
                GC_TD_TBDN.DataSource = new BindingSource(_InTBDuNovaLaiVay, "");
                GV_TD_TBDN.OptionsView.ColumnAutoWidth = false;
                GV_TD_TBDN.BestFitColumns();
            }
        }

        private void bt_TC_SK_laydl_Click(object sender, EventArgs e)
        {
            if (mtb_TC_SK_sotk.Text != null)
            {
                var tk = Convert.ToDecimal(mtb_TC_SK_sotk.Text.Replace("-", ""));
                var thauchi = _db.saokethauchi(dtp_TC_SK_ngaydau.Value, dtp_TC_SK_ngaycuoi.Value, tk);
                GC_ThauChi_SaoKe.DataSource = new BindingSource(thauchi, "");
            }
            GV_ThauChi_SaoKe.OptionsView.ColumnAutoWidth = false;
            GV_ThauChi_SaoKe.BestFitColumns();
        }

        private void bt_tc_tcmpa_laysdtc_Click(object sender, EventArgs e)
        {
            var thauchi = _db.thauchi_mpa(dtp_tcmpa_tc_ngay.Value);
            gc_tcmpa.DataSource = new BindingSource(thauchi, "");
            gv_tcmpa.OptionsView.ColumnAutoWidth = false;
            gv_tcmpa.BestFitColumns();
        }

        private void bt_tc_tcmpa_xuatexcel_Click(object sender, EventArgs e)
        {
            if (SaveFileExcel.ShowDialog() == DialogResult.OK)
            {
                gv_tcmpa.OptionsPrint.AutoWidth = false;
                gv_tcmpa.BestFitColumns();
                gv_tcmpa.ExportToXlsx(SaveFileExcel.FileName);
            }
            OpenExplorer(SaveFileExcel.FileName);
        }

        private void bt_td_pyctl_thulai_Click(object sender, EventArgs e)
        {


            var cifstring = cb_td_pyctl_tk.Text;

            if (cifstring.Trim() == "")
            {
                MessageBox.Show("Chưa nhập cif");
            }
            else
            {
                var cifno = Lib.ExtractNumber(cifstring);
                var cif = Convert.ToDecimal(cifno);
                var tinhthulai = _db.TinhThuLaiTheoCIF(dtp_td_pyctl_ngaytl.Value, cif).AsEnumerable().Cast<TinhThuLaiTheoCIF_Result>().ToList();
                if (tinhthulai.Count == 0)
                {
                    MessageBox.Show("Sai Cif hoặc cif không có tk vay");
                }
                else
                {
                    var ciflocal = _localdb.Table<CifInfo>().FirstOrDefault(c => c.cifno == cif);

                    if (ciflocal == null)
                    {
                        _localdb.Table<CifInfo>().Save(new CifInfo()
                        {
                            cifno = tinhthulai.ToList()[0].socif,
                            acname = tinhthulai.ToList()[0].khachhang
                        });

                    }
                    var tktt = _db.LayTKThanhToanTheoCIF(cif).AsEnumerable().Cast<LayTKThanhToanTheoCIF_Result>().ToList();

                    panelControl1.Controls.Clear();
                    UC_ThuLai tl = new UC_ThuLai(tinhthulai, tktt, cb_td_pyctl_noigui.Text);
                    tl.Dock = DockStyle.Fill;
                    tl.SetLocaltion(_clickOnceLocation);
                    panelControl1.Controls.Add(tl);
                }

            }
        }

        private void bt_td_tbdn_in_Click(object sender, EventArgs e)
        {
            ThongTin config;
            switch (cb_td_tbdnuno.SelectedIndex)
            {
                case 0:
                    config = _cf.HSC;
                    break;
                case 1:
                    config = _cf.TraNoc;
                    break;
                case 2:
                    config = _cf.NinhKieu;
                    break;
                case 3:
                    config = _cf.ThotNot;
                    break;
                default:
                    config = _cf.HSC;
                    break;

            }


            TenChiNhanh = config.TenChiNhanh;
            DiaChi = config.DiaChi;
            _ttnghang = new ThongTinNganHang
            {
                tencn_vi = config.tencn_vi,
                tencn_en = config.tencn_en,
                diachi = config.diachi,
                tinh = config.tinh,
                fax = config.fax,
                dt = config.dt,
                tennguoiky = config.tennguoiky,
                chucdanh = config.chucdanh,
                noinhan = config.noinhan,
                tenfilechuky = config.tenfilechuky

                //ngaycuoiky = ConfigurationManager.AppSettings["ngaycuoiky"]
            };
            if (_InTBDuNovaLaiVay.Count == 0)
            {
                MessageBox.Show("Chưa có thông tin");

            }
            else
            {
                frmInDuNovaLaivay frm = new frmInDuNovaLaivay(_InTBDuNovaLaiVay, _ttnghang);
                frm.ShowDialog();
            }
        }

        private void bt_td_tbndh_in_Click(object sender, EventArgs e)
        {
            ThongTin config;
            switch (cb_td_tbndh_noi.SelectedIndex)
            {
                case 0:
                    config = _cf.HSC;
                    break;
                case 1:
                    config = _cf.TraNoc;
                    break;
                case 2:
                    config = _cf.NinhKieu;
                    break;
                case 3:
                    config = _cf.ThotNot;
                    break;
                default:
                    config = _cf.HSC;
                    break;

            }
            TenChiNhanh = config.TenChiNhanh;
            DiaChi = config.DiaChi;
            _ttnghang = new ThongTinNganHang
            {
                tencn_vi = config.tencn_vi,
                tencn_en = config.tencn_en,
                diachi = config.diachi,
                tinh = config.tinh,
                fax = config.fax,
                dt = config.dt,
                tennguoiky = config.tennguoiky,
                chucdanh = config.chucdanh,
                noinhan = config.noinhan,
                tenfilechuky = config.tenfilechuky

                //ngaycuoiky = ConfigurationManager.AppSettings["ngaycuoiky"]
            };
            if (_InTBNoDenHan.Count == 0)
            {
                MessageBox.Show("Chưa có thông tin");

            }
            else
            {
                frmInTBNoDenHan frm = new frmInTBNoDenHan(_InTBNoDenHan, _ttnghang);
                frm.ShowDialog();
            }
        }

        private void bt_td_tl_laytt_Click(object sender, EventArgs e)
        {
            var tungay = dtp_td_tl_tungay.Value;
            var denngay = dtp_td_tl_denngay.Value;

            var trlai = _db.TraLai(_ngaydl_loanmonth, tungay, denngay);
            gc_td_tralai.DataSource = new BindingSource(trlai, "");

            gv_td_tralai.OptionsView.ColumnAutoWidth = false;
            gv_td_tralai.OptionsView.BestFitMaxRowCount = -1;
            gv_td_tralai.BestFitColumns();
        }

        private void bt_td_tl_xuatexel_Click(object sender, EventArgs e)
        {
            SaveFileExcel.FileName = "saoke_";
            if (SaveFileExcel.ShowDialog() == DialogResult.OK)
            {
                var fi = new FileInfo(SaveFileExcel.FileName);

                if (fi.Exists)
                {
                    fi.Delete();
                }
                gv_td_tralai.OptionsPrint.AutoWidth = false;
                gv_td_tralai.BestFitColumns();
                gc_td_tralai.ExportToXlsx(SaveFileExcel.FileName);
            }
            OpenExplorer(SaveFileExcel.FileName);
        }

        private void bt_thaythongtincif_Click(object sender, EventArgs e)
        {
            LayThongTinTheoCif();
        }

        private void bt_tracuucif_Click(object sender, EventArgs e)
        {

            var noidung = maskedTextBox2.Text.Trim();
            if (noidung == "")
            {
                MessageBox.Show(@"Chưa nhập nội dùng tìm kiếm");
            }
            else
            {
                switch (cb_tratk_tim.SelectedIndex)
                {
                    case 0:
                        var dstk = from p in _dbbdsu.tb_ql_CIF
                                   join c in _dbbdsu.depts on p.dept_unhap equals c.deptcode into deptnhap
                                   join d in _dbbdsu.depts on p.dept_uduyet equals d.deptcode into deptduyet
                                   from subpet in deptnhap.DefaultIfEmpty()
                                   from subpet1 in deptduyet.DefaultIfEmpty()
                                   where p.accno == noidung
                                   select new
                                   {
                                       CIF = p.cif,
                                       TenKH = p.TenKh,
                                       taikhoan = p.accno,
                                       UserIDMo = p.User_mo,
                                       p.TenUserMo,
                                       phongusermo = subpet.TenPhong,
                                       UserIDDuyet = p.User_duyet,
                                       p.TenUserDuyet,
                                       PhongUserDuyet = subpet1.TenPhong,
                                       Ngay = p.NgayMo,
                                       p.JRNLSEQNO,
                                       p.EJSEQNO
                                   };
                        dgw_tracuutk.DataSource = dstk.ToList();
                        break;

                    case 1:
                        int cif = Convert.ToInt32(noidung);
                        var dscif = from p in _dbbdsu.tb_ql_CIF
                                    join c in _dbbdsu.depts on p.dept_unhap equals c.deptcode into deptnhap
                                    join d in _dbbdsu.depts on p.dept_uduyet equals d.deptcode into deptduyet
                                    from subpet in deptnhap.DefaultIfEmpty()
                                    from subpet1 in deptduyet.DefaultIfEmpty()
                                    where p.cif == cif
                                    select new
                                    {
                                        CIF = p.cif,
                                        TenKH = p.TenKh,
                                        taikhoan = p.accno,
                                        UserIDMo = p.User_mo,
                                        p.TenUserMo,
                                        phongusermo = subpet.TenPhong,
                                        UserIDDuyet = p.User_duyet,

                                        p.TenUserDuyet,
                                        PhongUserDuyet = subpet1.TenPhong,
                                        Ngay = p.NgayMo,
                                        p.JRNLSEQNO,
                                        p.EJSEQNO
                                    };
                        dgw_tracuutk.DataSource = dscif.ToList();
                        break;

                    case 2:
                        var dsten = from p in _dbbdsu.tb_ql_CIF
                                    join c in _dbbdsu.depts on p.dept_unhap equals c.deptcode into deptnhap
                                    join d in _dbbdsu.depts on p.dept_uduyet equals d.deptcode into deptduyet
                                    from subpet in deptnhap.DefaultIfEmpty()
                                    from subpet1 in deptduyet.DefaultIfEmpty()
                                    where p.TenKh.Contains(noidung)
                                    select new
                                    {
                                        CIF = p.cif,
                                        TenKH = p.TenKh,
                                        taikhoan = p.accno,
                                        UserIDMo = p.User_mo,
                                        p.TenUserMo,
                                        phongusermo = subpet.TenPhong,
                                        UserIDDuyet = p.User_duyet,
                                        p.TenUserDuyet,
                                        PhongUserDuyet = subpet1.TenPhong,
                                        Ngay = p.NgayMo,
                                        p.JRNLSEQNO,
                                        p.EJSEQNO
                                    };
                        dgw_tracuutk.DataSource = dsten.ToList();
                        break;
                }
            }
        }

        private void bt_tracuutheatm_Click(object sender, EventArgs e)
        {

            if (maskedTextBox3.Text == "")
            {
                MessageBox.Show(@"Chưa nhập số liệu");
            }
            else
            {
                var dssothe = new List<tbsothe>();
                switch (cb_ATM_tracuthe.SelectedIndex)
                {
                    case 0:
                        // tìm theo mã số thẻ
                        dssothe = _dbbdsu.tbsothes.Where(p => p.masothe.Contains(maskedTextBox3.Text)).ToList();
                        break;

                    case 1:
                        // tìm theo cif
                        var cif = Convert.ToInt32(maskedTextBox3.Text);
                        dssothe = _dbbdsu.tbsothes.Where(p => p.cif.Value == cif).ToList();
                        break;

                    case 2:
                        // tìm theo tên
                        dssothe = _dbbdsu.tbsothes.Where(p => p.Hoten.Contains(maskedTextBox3.Text)).ToList();
                        break;

                    case 3:
                        // tìm theo tên
                        dssothe = _dbbdsu.tbsothes.Where(p => p.cmnd.Contains(maskedTextBox3.Text)).ToList();
                        break;
                }

                //var ds = dbbds.tbsothes.Where(p => p.masothe.Contains(maskedTextBox3.Text)).Select(p => new
                //{
                //    MaSoThe = p.masothe,
                //    HoTen = p.Hoten,
                //    CMND = p.cmnd,
                //    p.DiaChi,
                //    CIF = p.cif,
                //    Ngay = p.ngay,
                //    TrangThai = p.trangthai,
                //    NgayBC = p.ngaybc,
                //    NguoiTacDong = p.usertacdong,
                //    SoTaiKhoan = p.sotk,
                //});

                dgw_tracuutheatm.DataSource = dssothe;
            }
        }

        private void bt_uncheckclb_locdl_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clb_Locdl.Items.Count; i++)
            {
                clb_Locdl.SetItemCheckState(i, CheckState.Unchecked);
            }
        }

        private void bt_xembc_cif_Click(object sender, EventArgs e)
        {
            var data = (IList<DatainNoCo>)dgv_noco_cif.DataSource;
            if (data == null)
            {
                MessageBox.Show(@"Chưa có số liệu");
            }
            else
            {
                var finbc = new Inbc(data);
                finbc.Show();
            }
        }

        private void bt_xuaexcel_atmxoa_Click(object sender, EventArgs e)
        {
            var data = (IList<tbXoaATM>)dgw_theatmxoa.DataSource;
            if (data == null)
            {
                MessageBox.Show(@"Chưa có dữ liệu");
            }
            else
            {
                if (SaveFileExcel.ShowDialog() == DialogResult.OK)
                {
                    using (var pck = new ExcelPackage())
                    {
                        var fi = new FileInfo(SaveFileExcel.FileName);

                        if (fi.Exists)
                        {
                            fi.Delete();
                        }
                        ExcelWorksheet wsList = pck.Workbook.Worksheets.Add("Thẻ ATM");
                        wsList.Cells["A1"].LoadFromCollection(data, true);

                        for (int i = 1; i <= wsList.Dimension.End.Column; i++)
                        {
                            if (i == 3 || i == 6)
                            {
                                wsList.Column(i).Style.Numberformat.Format = "dd/MM/yyyy hh:mm:ss";
                            }
                            wsList.Column(i).AutoFit();
                        }

                        pck.SaveAs(fi);
                    }
                }
                OpenExplorer(SaveFileExcel.FileName);
            }
        }

        private void bt_xuatexcel_cif_Click(object sender, EventArgs e)
        {
            var data = (IList<DatainNoCo>)dgv_noco_cif.DataSource;
            if (data == null)
            {
                MessageBox.Show(@"Chưa có số liệu");
            }
            else
            {
                if (SaveFileExcel.ShowDialog() == DialogResult.OK)
                {
                    var fi = new FileInfo(SaveFileExcel.FileName);

                    if (fi.Exists)
                    {
                        fi.Delete();
                    }
                    using (var pck = new ExcelPackage())
                    {
                        ExcelWorksheet wsList = pck.Workbook.Worksheets.Add("saoke");
                        wsList.Cells["A1"].LoadFromCollection(data, true);

                        wsList.Cells.AutoFitColumns();
                        wsList.Column(15).Style.Numberformat.Format = "dd/MM/yyyy hh:mm:ss";
                        wsList.Column(17).Style.Numberformat.Format = "dd/MM/yyyy hh:mm:ss";
                        wsList.Column(8).Style.Numberformat.Format = "#,##0.0";
                        wsList.Column(9).Style.Numberformat.Format = "#,##0.0";

                        pck.SaveAs(fi);
                    }
                    OpenExplorer(SaveFileExcel.FileName);
                }
            }
        }



        private void button10_Click(object sender, EventArgs e)
        {
            if (SaveFileExcel.ShowDialog() == DialogResult.OK)
            {
                gridView2.OptionsPrint.AutoWidth = false;
                gridView2.BestFitColumns();
                gridView2.ExportToXlsx(SaveFileExcel.FileName);
            }
            OpenExplorer(SaveFileExcel.FileName);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (SaveFileExcel.ShowDialog() == DialogResult.OK)
            {
                GV_ThauChi_SaoKe.OptionsPrint.AutoWidth = false;
                GV_ThauChi_SaoKe.BestFitColumns();
                GV_ThauChi_SaoKe.ExportToXlsx(SaveFileExcel.FileName);
            }
            OpenExplorer(SaveFileExcel.FileName);
        }

        private void button14_Click(object sender, EventArgs e)
        {


            var cifstring = cb_td_tbndh_tk.Text;

            if (cifstring.Trim() == "")
            {
                MessageBox.Show("Chưa nhập cif");
            }
            else
            {
                var cifno = Lib.ExtractNumber(cifstring);
                var cif = Convert.ToDecimal(cifno);
                var ds = _db.ThongBaoNoDenHan(cif, dtp_td_tbdnldh.Value).AsEnumerable().Cast<ThongBaoNoDenHan_Result>().ToList();

                _InTBNoDenHan = new List<InTBNoDenHan>();

                if (ds.Count == 0)
                {
                    MessageBox.Show("Sai Cif hoặc cif không có tk vay");
                }
                else
                {
                    var ciflocal = _localdb.Table<CifInfo>().FirstOrDefault(c => c.cifno == cif);

                    if (ciflocal == null)
                    {
                        _localdb.Table<CifInfo>().Save(new CifInfo()
                        {
                            cifno = ds.ToList()[0].socif,
                            acname = ds.ToList()[0].khachhang
                        });

                    }

                    foreach (var i in ds)
                    {
                        _InTBNoDenHan.Add(new InTBNoDenHan()
                        {

                            Socif = i.socif,
                            DuNo = i.DUNO ?? 0,
                            Gocdh = i.gocdh ?? 0,
                            Kyhangoc = i.kyhangoc.Value,
                            LoaiTien = i.tiente,
                            LoaiVay = i.loaisaoke,
                            TaiKhoan = string.Format(@"{0:###-##-##-######-#}", i.taikhoan),
                            TenKhachHang = i.khachhang,
                            quanhe = i.quanhe,
                            thang = i.thang.Value,
                            Datadate = i.datadate.Value

                        });

                    }
                }
                GC_TD_TBNDH.DataSource = new BindingSource(_InTBNoDenHan, "");
                GV_TD_TBNDH.OptionsView.ColumnAutoWidth = false;
                GV_TD_TBNDH.BestFitColumns();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var pathToFile = Path.Combine(_clickOnceLocation, @"MAU.xlsx");
            //    logger.Info(_pathToFile);
            if (dchieu == null || dchieu.Count == 0)
            {
                MessageBox.Show(@"Chưa có DL");
            }
            else
            {
                SaveFileExcel.FileName = "dluc_";
                if (SaveFileExcel.ShowDialog() == DialogResult.OK)
                {
                    var temp = new FileInfo(pathToFile);
                    using (var pck = new ExcelPackage(temp))
                    {
                        ExcelWorksheet wsList = pck.Workbook.Worksheets[1];

                        const int startRow = 6;

                        int row = startRow;
                        int ii = 1;
                        foreach (var dd in dchieu)
                        {
                            wsList.InsertRow(row, 1);
                            wsList.Cells[row, 1].Value = ii;
                            wsList.Cells[row, 2].Value = dd.NgayGD;
                            wsList.Cells[row, 2].Style.Numberformat.Format = "dd/MM/yyyy";
                            wsList.Cells[row, 3].Value = dd.MaGd;

                            wsList.Cells[row, 4].Value = dd.LoaiTien;

                            wsList.Cells[row, 5].Value = dd.PSNo;
                            wsList.Cells[row, 5].Style.Numberformat.Format = "#,##0.00;-#,##0.00";

                            wsList.Cells[row, 6].Value = dd.PSCo;
                            wsList.Cells[row, 6].Style.Numberformat.Format = "#,##0.00;-#,##0.00";

                            wsList.Cells[row, 7].Value = dd.SoDu;
                            wsList.Cells[row, 7].Style.Numberformat.Format = "#,##0.00;-#,##0.00";

                            wsList.Cells[row, 8].Value = dd.GhiChu;
                            wsList.Cells[row, 8].Style.WrapText = true;
                            row++;
                            ii++;
                        }
                        string local = "A" + startRow + ":H" + row;
                        //     logger.Info(local);
                        var cell = wsList.Cells[local];
                        var border = cell.Style.Border;
                        border.BorderAround(ExcelBorderStyle.Thin);
                        border.Bottom.Style = ExcelBorderStyle.Thin;
                        border.Top.Style = ExcelBorderStyle.Thin;
                        border.Left.Style = ExcelBorderStyle.Thin;
                        border.Right.Style = ExcelBorderStyle.Thin;

                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        wsList.Cells["A4"].Value = string.Format(@"Ngày: {0:dd/MM/yyyy}", DateTime.Now);
                        wsList.Cells["E" + row].Formula = "Sum(E" + startRow + ":E" + (row - 1) + ")";
                        wsList.Cells["E" + row].Style.Numberformat.Format = "#,##0.00;-#,##0.00";
                        wsList.Cells["F" + row].Formula = "Sum(F" + startRow + ":F" + (row - 1) + ")";
                        wsList.Cells["F" + row].Style.Numberformat.Format = "#,##0.00;-#,##0.00";
                        //  wsList.Cells["J" + row].Formula = "Sum(J" + startRow + ":J" + (row - 1) + ")";
                        //  wsList.Cells["J" + row].Style.Numberformat.Format = "#,##0.00;-#,##0.00";
                        //    for (int i = 1; i <= wsList.Dimension.End.Column; i++)
                        //    {
                        //        wsList.Column(i).AutoFit();
                        //   }

                        /*  var wsList1 = pck.Workbook.Worksheets.Add("full");

                        var ds = from p in _db.DDDHIS_DATE
                                 where p.DATADATE >= dtp_saoke_dienluc_nd.Value
                                 where p.DATADATE <= dtp_saoke_dienluc_nc.Value
                                 where p.TRACCT == 74110000191552
                                 where p.DORC == "C"
                                 //where p.TRUSER != @"DD4400"
                                 //    where p.TRUSER != @"DD2625"
                                 select p;
                        wsList1.Cells["A1"].LoadFromCollection(ds.ToList(), true);
                        */

                        var fi = new FileInfo(SaveFileExcel.FileName);

                        if (fi.Exists)
                        {
                            fi.Delete();
                        }
                        pck.SaveAs(fi);
                    }
                }
                // OpenExplorer(SaveFileExcel.FileName);
                OpenExplorer(SaveFileExcel.FileName);
            }
        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            var sotk = mtb_tk.Text.Replace("-", "").Trim();
            if (string.IsNullOrEmpty(sotk))
            {
                MessageBox.Show(@"Chưa nhập Số Tài khoản");
            }
            else if (sotk.Length != 14)
            {
                MessageBox.Show(@"Sai Số Tài khoản");
            }
            else
            {


                var brach = sotk.Substring(0, 3);
                var thongtin = _dbbdsu.tblBranches.FirstOrDefault(c => c.BRANCHCODE.Contains(brach));
                if (thongtin != null)
                {
                    tb_tenchinhanh.Text = thongtin.BRANCHNAME.ToUpper().Trim();
                    tb_diachi.Text = thongtin.ADDRESS.ToUpper().Trim();
                }
                else
                {
                    tb_tenchinhanh.Text = "";
                    tb_diachi.Text = "";
                }
                //var tk = db.DDMAST_DATEs.FirstOrDefault(c => c.ACCTNO == Convert.ToDecimal(sotk));
                var stk = Convert.ToDecimal(sotk);
                DDMAST_DATE tk = (from p in _db.DDMAST_DATE
                                  orderby p.DATADATE descending
                                  where p.ACCTNO == stk
                                  select p).FirstOrDefault();
                tb_tenKh.Text = tk != null ? tk.ACNAME : "";
            }
        }

        private void button2_Click_3(object sender, EventArgs e)
        {
            //var ttk = dataGridView1.SelectedRows[0].Cells[1].Value;
            var ttk = dgv_ipdc.SelectedCells[0].Value;

            var removecif = _lcif.Single(c => c.acctno == ttk.ToString());
            _lcif.Remove(removecif);

            dgv_ipdc.DataSource = null;
            dgv_ipdc.DataSource = _lcif;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (_dsbc810.Count == 0)
            {
                MessageBox.Show(@"Chưa có DL");
            }
            else
            {
                if (SaveFileExcel.ShowDialog() == DialogResult.OK)
                {
                    using (var pck = new ExcelPackage())
                    {
                        var fi = new FileInfo(SaveFileExcel.FileName);

                        if (fi.Exists)
                        {
                            fi.Delete();
                        }
                        var wsList = pck.Workbook.Worksheets.Add("bc810");
                        wsList.Cells["A1"].LoadFromCollection(_dsbc810, true);
                        wsList.Column(4).Style.Numberformat.Format = "dd/MM/yyyy";
                        for (int i = 1; i <= wsList.Dimension.End.Column; i++)
                        {
                            wsList.Column(i).AutoFit();
                        }

                        pck.SaveAs(fi);
                    }
                    OpenExplorer(SaveFileExcel.FileName);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //    sktv.Clear();
            //    var tk = Convert.ToDecimal(mtb_td_sk_tk.Text.Replace("-", ""));
            //    var lnhist = from p in _db.LNDHIS_DATE
            //                 where p.DATADATE <= dtp_td_skvay_nc.Value
            //                 where p.DATADATE >= dtp_td_skvay_nd.Value
            //                 where p.LHACCT == tk
            //                 select p;
            //    foreach (var i in lnhist)
            //    {
            //        sktv.Add(new saokeTienVay()
            //        {
            //            CN = i.ACCT_BRN,
            //            DienGiai = i.LHEFTH,

            //        });

            //    }

            //    GC_TD_saoketv.DataSource = new BindingSource(sktv, "");
            var tk = Convert.ToDecimal(mtb_td_sk_tk.Text.Replace("-", ""));
            var lnhist = _db.SaoKeTienVay(dtp_td_skvay_nd.Value, dtp_td_skvay_nc.Value, tk);
            GC_TD_saoketv.DataSource = new BindingSource(lnhist, "");
            GV_TD_saoketv.OptionsView.ColumnAutoWidth = false;
            GV_TD_saoketv.BestFitColumns();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SaveFileExcel.FileName = "saoketv_";
            if (SaveFileExcel.ShowDialog() == DialogResult.OK)
            {
                var fi = new FileInfo(SaveFileExcel.FileName);

                if (fi.Exists)
                {
                    fi.Delete();
                }
                GV_TD_saoketv.OptionsPrint.AutoWidth = false;
                GV_TD_saoketv.BestFitColumns();

                GC_TD_saoketv.ExportToXlsx(SaveFileExcel.FileName);
            }
            OpenExplorer(SaveFileExcel.FileName);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SaveFileExcel.FileName = "tralai_";
            if (SaveFileExcel.ShowDialog() == DialogResult.OK)
            {
                var fi = new FileInfo(SaveFileExcel.FileName);

                if (fi.Exists)
                {
                    fi.Delete();
                }
                gv_td_tragoc.OptionsPrint.AutoWidth = false;
                gv_td_tragoc.BestFitColumns();
                gc_td_tragoc.ExportToXlsx(SaveFileExcel.FileName);
            }
            OpenExplorer(SaveFileExcel.FileName);
        }

        private void button6_Click_2(object sender, EventArgs e)
        {
            SaveFileExcel.FileName = "saokepos_";
            if (SaveFileExcel.ShowDialog() == DialogResult.OK)
            {
                var fi = new FileInfo(SaveFileExcel.FileName);

                if (fi.Exists)
                {
                    fi.Delete();
                }
                gv_sk_phipos.OptionsPrint.AutoWidth = false;
                gv_sk_phipos.BestFitColumns();
                gv_sk_phipos.ExportToXlsx(SaveFileExcel.FileName);
            }
            OpenExplorer(SaveFileExcel.FileName);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var tungay = dtp_td_tg_tungay.Value;
            var denngay = dtp_td_tg_denngay.Value;

            var trlai = _db.TraGoc(_ngaydl_loanmonth, tungay, denngay);
            gc_td_tragoc.DataSource = new BindingSource(trlai, "");

            gv_td_tragoc.OptionsView.ColumnAutoWidth = false;
            gv_td_tragoc.OptionsView.BestFitMaxRowCount = -1;
            gv_td_tragoc.BestFitColumns();
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            if (SaveFileExcel.ShowDialog() == DialogResult.OK)
            {
                gv_nguon_ttnt.OptionsPrint.AutoWidth = false;
                gv_nguon_ttnt.BestFitColumns();
                gv_nguon_ttnt.ExportToXlsx(SaveFileExcel.FileName);
            }
            OpenExplorer(SaveFileExcel.FileName);
        }




        private void cb_td_pyctl_tk_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.ComboBox comboBox = (System.Windows.Forms.ComboBox)sender;

            // Save the selected employee's name, because we will remove
            // the employee's name from the list.
            var tenkh = (string)comboBox.SelectedItem;
            if (tenkh != "")
            {
                textBox2.Text = tenkh.Substring(tenkh.IndexOf("-", StringComparison.Ordinal) + 1);
            }

        }

        private string Check_checkbox()
        {
            var loaitk = cb_ca.Checked ? "D,S" : "";
            if (cb_fd.Checked)
            {
                if (loaitk != "")
                {
                    loaitk += ",T";
                }
                else
                {
                    loaitk = "T";
                }
            }
            if (cb_ln.Checked)
            {
                if (loaitk != "")
                {
                    loaitk += ",L";
                }
                else
                {
                    loaitk = "L";
                }
            }
            if (!cb_tf.Checked) return loaitk;
            if (loaitk != "")
            {
                loaitk += ",B";
            }
            else
            {
                loaitk = "B";
            }

            return loaitk;
        }

        private string Check_dl(string tablename, DateTime ngaydau, DateTime ngaycuoi)
        {
            IQueryable<tb_checkdulieu> t =
                         _db.tb_checkdulieu.Where(
                             a => a.tablename.ToUpper() == tablename.ToUpper() && a.datadate >= ngaydau && a.datadate <= ngaycuoi);

            return (from dateTime in Lib.GetDateRange(ngaydau, ngaycuoi) let ee = t.FirstOrDefault(a => a.datadate == dateTime) where ee == null select dateTime).Aggregate("", (current, dateTime) => current + ("Chưa có dữ liệu ngày " + dateTime.ToShortDateString() + "\n"));
        }

        private void clb_Locdl_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.Index == 0 && e.NewValue == CheckState.Checked)
            {
                for (int i = 1; i < clb_Locdl.Items.Count; i++)
                {
                    clb_Locdl.SetItemCheckState(i, CheckState.Checked);
                }
            }
            if (e.NewValue == CheckState.Unchecked && e.Index != 0)
            {
                clb_Locdl.SetItemCheckState(0, CheckState.Unchecked);
            }
        }

        private void ComboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            switch (cb_tratk_tim.SelectedIndex)
            {
                case 0:
                    maskedTextBox2.Mask = @"000-00-00-000000-0";
                    maskedTextBox2.Text = @"   -  -  -      -";
                    break;

                case 1:
                    maskedTextBox2.Mask = "";
                    maskedTextBox2.Text = "";
                    break;

                case 2:
                    maskedTextBox2.Mask = "";
                    maskedTextBox2.Text = "";
                    break;

                default:
                    maskedTextBox2.Mask = "";
                    maskedTextBox2.Text = "";
                    break;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

            _cf = Config.Load();

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                Version myVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                Text = Text +
                       $" - Version: v{myVersion.Major}.{myVersion.Minor}.{myVersion.Build}.{myVersion.Revision}";
                //  logger.Info($" - Version: v{myVersion.Major}.{myVersion.Minor}.{myVersion.Build}.{myVersion.Revision}");
            }
            Assembly assemblyInfo = Assembly.GetExecutingAssembly();
            //Location is where the assembly is run from

            //CodeBase is the location of the ClickOnce deployment files
            var uriCodeBase = new Uri(assemblyInfo.CodeBase);

            _clickOnceLocation = Path.GetDirectoryName(uriCodeBase.LocalPath);
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(uriCodeBase.LocalPath);

            logger.Info(fileVersionInfo.FileVersion);


            DateTime now = DateTime.Today.AddDays(-1);
            ipdc_date.Value = dtp_tungay.Value = dtp_denngay.Value = now;
            dtp_lsgl_ngaybd.EditValue = dtp_lsgl_ngaykt.EditValue = now;
            de_gl_erp_datdau.EditValue = de_gl_erp_ketthuc.EditValue = now;
            de_gl_trc_nbd.EditValue = de_gl_trc_nkt.EditValue = now;
            dtp_gl_ngay.DateTime = dtp_tcmpa_tc_ngay.Value = dtp__ngaycuoi_cif.Value =
                    dtp__ngaydau_cif.Value = dtp_atmxoa_ngaydau.Value = dtp_atmxoa_ngaycuoi.Value = now;
            dtp_td_pyctl_ngaytl.Value = dtp_td_tbdn.Value = dtp_td_tbdnldh.Value = now;

            cb_tratk_tim.SelectedIndex = 0;
            cb_ATM_tracuthe.SelectedIndex = 0;
            cb_lsgl_bds.SelectedIndex = 0;
            cb_gl_tiente.SelectedIndex = 0;
            cb_bds_pdc.SelectedIndex = 0;
            cb_td_tbdnuno.SelectedIndex = 0;
            cb_td_tbndh_noi.SelectedIndex = 0;
            cb_td_pyctl_noigui.SelectedIndex = 0;

            _localdb = new DbInstance(_clickOnceLocation + @"/data");
            _localdb.Map<CifInfo>().Automap(i => i.cifno);
            _localdb.Initialize();

            _tempdir = Application.StartupPath + "/temp";

            var _glconfig = glconfig.Load("gl.json");
            AutoCompleteStringCollection atcgl = new AutoCompleteStringCollection();

            foreach (var i in _glconfig.dstkgl)
            {
                var item = string.Format("{0:0.##} - {1}", i.tk, i.tentk);
                atcgl.Add(item);
                cb_gl_lsgl_tk.Items.Add(item);
                cb_gl_tracuugl_tk.Items.Add(item);


            }

            cb_gl_lsgl_tk.AutoCompleteCustomSource = atcgl;

            cb_gl_tracuugl_tk.AutoCompleteCustomSource = atcgl;



            if (!Directory.Exists(_tempdir))
            {
                Directory.CreateDirectory(_tempdir);
            }
            else
            {
                var filePaths = Directory.GetFiles(_tempdir);
                foreach (var filePath in filePaths)
                    File.Delete(filePath);
            }

            _ngaydl_loanmonth = (from p in _db.tb_checkdulieu
                                 where p.tablename == "loanmonth"
                                 select p.datadate).Max();

            dtp_td_tbdn.Value = _ngaydl_loanmonth.Value;
        }

        private void LayThongTinTheoCif()
        {
            var listma = new List<ListItem> { new ListItem { Id = "0", Name = "in Tất cả" } };

            string loai;

            if (cif_tatca.Checked)
            {
                loai = " ";
            }
            else if (cif_baoco.Checked)
            {
                loai = "C";
            }
            else
            {
                loai = "D";
            }

            string sotk = tb_cif.Text.Trim();
            if (sotk == "")
            {
                MessageBox.Show(@"Chưa nhập Số cif");
            }
            else
            {
                string res = Check_dl("ddmast_date", dtp__ngaydau_cif.Value, dtp__ngaycuoi_cif.Value);
                if (!string.IsNullOrEmpty(res))
                {
                    MessageBox.Show(res);
                }
                else
                {
                    var re = _db.LichSuGiaoDichTheoCIF(Convert.ToDecimal(sotk),
                        dtp__ngaydau_cif.Value, dtp__ngaycuoi_cif.Value, loai);
                    _dbin = new List<DatainNoCo>();
                    foreach (var value in re)
                    {
                        string gio = value.TRTIME.ToString();
                        if (gio.IndexOf(".", StringComparison.Ordinal) != -1)
                        {
                            gio = gio.Remove(gio.IndexOf(".", StringComparison.Ordinal));
                        }
                        if (gio.Length == 5)
                        {
                            gio = "0" + gio;
                        }
                        if (gio.Trim().Length == 1)
                        {
                            gio = "000000";
                        }
                        var t = DateTime.ParseExact(gio, "HHmmss", CultureInfo.CurrentCulture);
                        var ngaygd = new DateTime(value.TRDAT6.Year, value.TRDAT6.Month, value.TRDAT6.Day, t.Hour,
                            t.Minute, t.Second);

                        var ghichu =
                            value.TREFTH.Replace("tREM", "")
                                .Replace("REM       ", "")
                                .Replace("SWEEP", "")
                                .Trim()
                                .Replace("    ", " ");
                        var prov = new MaskedTextProvider("###-##-##-######-#");
                        prov.Set(value.TRACCT.ToString(CultureInfo.InvariantCulture));
                        var formattk = prov.ToDisplayString();
                        var loaigd = value.DORC == "D" ? "No" : "Co";
                        if (value.AUXTRC.Trim() != "" && listma.FirstOrDefault(c => c.Id == value.AUXTRC.Trim()) == null)
                        {
                            listma.Add(new ListItem
                            {
                                Id = value.AUXTRC.Trim(),
                                Name = value.AUXTRC.Trim() + "|" + value.Chuthich
                            });
                        }

                        if (value.cbal != null)
                            _dbin.Add(new DatainNoCo
                            {
                                auxtrc = value.AUXTRC.Trim(),
                                sotien = value.AMT,
                                ngayin = DateTime.Now,
                                seq = Convert.ToInt32(value.SEQ),
                                ngaygiaodich = ngaygd,
                                cngd = Convert.ToInt32(value.TRBR),
                                ghichu = ghichu,
                                loaitien = value.TRCTYP,
                                tentk = value.ACNAME.Trim(),
                                tk = formattk,
                                truser = value.TRUSER,
                                loai = loaigd,
                                chuthich_aux = value.Chuthich,
                                cn_motk = value.TRBR.ToString(),
                                cn_giaodich = value.TRSOBR.ToString(CultureInfo.InvariantCulture),
                                cifno = value.CIFNO.ToString(),
                                sodu = value.cbal.Value
                            });
                    }

                    dgv_noco_cif.DataSource = null;
                    dgv_noco_cif.DataSource = _dbin;
                    var dataGridViewColumn = dgv_noco_cif.Columns["sotien"];
                    if (dataGridViewColumn != null)
                        dataGridViewColumn.DefaultCellStyle.Format = "#,##0;-#,##0;0";
                    var gridViewColumn = dgv_noco_cif.Columns["sodu"];
                    if (gridViewColumn != null)
                        gridViewColumn.DefaultCellStyle.Format = "#,##0;-#,##0;0";
                    lb_cif_trangthai.Text = _dbin.Count.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        private void LayThongTinTheoTk()
        {
            clb_Locdl.Items.Clear();
            string loai;
            clb_Locdl.Sorted = false;
            clb_Locdl.Items.Add("-In Tất cả", CheckState.Checked);
            clb_Locdl.Sorted = true;
            if (rb_all.Checked)
            {
                loai = " ";
            }
            else if (rb_baoco.Checked)
            {
                loai = "C";
            }
            else
            {
                loai = "D";
            }

            var sotk = Convert.ToDecimal(mtb_nc_tk.Text.Replace("-", "").Trim());
            if (string.IsNullOrEmpty(mtb_nc_tk.Text.Replace("-", "").Trim()))
            {
                MessageBox.Show(@"Chưa nhập Số Tài khoản");
            }
            else if (mtb_nc_tk.Text.Replace("-", "").Trim().Length != 14)
            {
                MessageBox.Show(@"Sai Số Tài khoản");
            }
            else
            {
                string res = Check_dl("dddhis_date", dtp_tungay.Value, dtp_denngay.Value);
                if (!string.IsNullOrEmpty(res))
                {
                    MessageBox.Show(res);
                }
                else
                {
                    var re = _db.LichSuGiaoDichTheoTK(sotk, dtp_tungay.Value, dtp_denngay.Value, loai);
                    _dbin = new List<DatainNoCo>();

                    var ngaytruocdo = dtp_tungay.Value.AddDays(-1);

                    var cablngaytruoc = _db.DDMAST_DATE.FirstOrDefault(c => c.ACCTNO == sotk && c.DATADATE == ngaytruocdo);
                    decimal sodu = 0;
                    if (cablngaytruoc != null)
                    {
                        sodu = cablngaytruoc.CBAL;
                    }

                    foreach (var value in re)
                    {
                        string gio = value.TRTIME.ToString();
                        if (gio.IndexOf(".", StringComparison.Ordinal) != -1)
                        {
                            gio = gio.Remove(gio.IndexOf(".", StringComparison.Ordinal));
                        }
                        if (gio.Length == 5)
                        {
                            gio = "0" + gio;
                        }
                        if (gio.Trim().Length == 1)
                        {
                            gio = "000000";
                        }
                        // DateTime t = DateTime.ParseExact(gio, "HHmmss", CultureInfo.CurrentCulture);
                        DateTime t;
                        if (!DateTime.TryParseExact(gio, "HHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None, out t))
                        {
                            t = DateTime.ParseExact("000000", "HHmmss", CultureInfo.CurrentCulture);
                        }

                        var ngaygd = new DateTime(value.TRDAT6.Year, value.TRDAT6.Month, value.TRDAT6.Day, t.Hour,
                            t.Minute, t.Second);

                        string ghichu =
                            value.TREFTH.Replace("tREM", "")
                                .Replace("REM       ", "")
                                .Replace("SWEEP", "")
                                .Trim()
                                .Replace("    ", " ")
                                .Trim();
                        var prov = new MaskedTextProvider("###-##-##-######-#");
                        prov.Set(value.TRACCT.ToString(CultureInfo.InvariantCulture));
                        var formattk = prov.ToDisplayString();
                        string loaigd;
                        loaigd = value.DORC == "D" ? "No" : "Co";

                        if (clb_Locdl.Items.IndexOf(value.AUXTRC.Trim() + "|" + value.Chuthich) == -1)
                        {
                            clb_Locdl.Items.Add(value.AUXTRC.Trim() + "|" + value.Chuthich, true);
                        }

                        if (value.DORC == "D")
                        {
                            sodu = sodu - value.AMT;
                        }
                        else
                        {
                            sodu = sodu + value.AMT;
                        }
                        if (value.cbal != null)
                            _dbin.Add(new DatainNoCo
                            {
                                auxtrc = value.AUXTRC.Trim(),
                                sotien = value.AMT,
                                ngayin = DateTime.Now,
                                seq = Convert.ToInt32(value.SEQ),
                                ngaygiaodich = ngaygd,
                                cngd = Convert.ToInt32(value.TRBR),
                                ghichu = ghichu,
                                loaitien = value.TRCTYP,
                                tentk = value.ACNAME.Trim(),
                                tk = formattk,
                                truser = value.TRUSER,
                                loai = loaigd,
                                chuthich_aux = value.Chuthich,
                                cn_motk = value.TRBR.ToString(),
                                cn_giaodich = value.TRSOBR.ToString(CultureInfo.InvariantCulture),
                                cifno = value.CIFNO.ToString(),
                                sodu = sodu,
                                trancd = value.TRANCD
                            });
                    }

                    dgv_noco_tk.DataSource = _dbin;
                    //  dataview.Columns["sotien"].DefaultCellStyle.Format = "#,##0;-#,##0;0";
                    // dataview.Columns["sodu"].DefaultCellStyle.Format = "#,##0;-#,##0;0";
                    tb_somon.Text = _dbin.Count.ToString(CultureInfo.InvariantCulture);
                    bt_uncheckclb_locdl.Enabled = true;
                    foreach (DataGridViewRow row in dgv_noco_tk.Rows)
                        if (row.Cells[6].Value.ToString() == "No")
                        {
                            row.DefaultCellStyle.BackColor = Color.LightPink;
                        }
                }
            }
        }

        private void maskedTextBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LayThongTinTheoTk();
            }
        }

        public static void OpenExplorer(string dir)
        {
            var result = MessageBox.Show($"Xuất Bc thành công \n File Lưu tại {dir} \n Bạn Có muốn mở file", @"OpenFile", MessageBoxButtons.YesNo,
                                   MessageBoxIcon.Question);

            // If the no button was pressed ...
            if (result == DialogResult.Yes)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = dir,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
        }

        private void tb_cif_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LayThongTinTheoCif();
            }
        }

        private void tb_gl_laydulieu_Click(object sender, EventArgs e)
        {
            if (textEdit1.Text.Trim() == "")
            {
                MessageBox.Show(@"Chưa nhập TK GL");
            }
            else
            {
                var glacc = Convert.ToDecimal(textEdit1.Text.Trim());
                var ngaydl = (DateTime)dtp_gl_ngay.EditValue;
                IQueryable<GLHIST> listgl = from p in _db.GLHISTs
                                            where p.GTACCT == glacc && p.DataDate == ngaydl.Date
                                            select p;
                _lsglhist = listgl.ToList();
                GL_gc_tracuutk.DataSource = new BindingSource(_lsglhist, "");
                GL_gv_tracuutk.BestFitColumns();
            }
        }

        private void tb_gl_xuatExcel_Click(object sender, EventArgs e)
        {
            if (_lsglhist == null || _lsglhist.Count == 0)
            {
                MessageBox.Show(@"Chưa có DL");
            }
            else
            {
                SaveFileExcel.FileName = "gl_";
                if (SaveFileExcel.ShowDialog() == DialogResult.OK)
                {
                    using (var pck = new ExcelPackage())
                    {
                        var fi = new FileInfo(SaveFileExcel.FileName);

                        if (fi.Exists)
                        {
                            fi.Delete();
                        }
                        ExcelWorksheet wsList = pck.Workbook.Worksheets.Add("glhist");
                        wsList.Cells["A1"].LoadFromCollection(_lsglhist, true);
                        wsList.Column(1).Style.Numberformat.Format = "dd/MM/yyyy hh:mm:ss";
                        wsList.Column(2).Style.Numberformat.Format = "dd/MM/yyyy hh:mm:ss";
                        for (var i = 1; i <= wsList.Dimension.End.Column; i++)
                        {
                            wsList.Column(i).AutoFit();
                        }

                        pck.SaveAs(fi);
                    }
                    OpenExplorer(SaveFileExcel.FileName);
                }
            }
        }



        private void tb_in_sec_Enter(object sender, EventArgs e)
        {
            lb_mayinso.Text = @"Đang lấy Thông Tin Máy in";
            tb_LayTTinSosec.Visible = true;
            bt_insosec.Visible = true;
            panel1.Visible = true;
            tb_tenchinhanh.Text = TenChiNhanh;
            tb_diachi.Text = DiaChi;

            tb_dkMayin.Visible = true;

            string machineName = Environment.MachineName;
            var tbprinter = _dbbdsu.tbPrinters.FirstOrDefault(c => c.WorkStation.Contains(machineName.ToUpper()));

            if (tbprinter != null)
            {
                _printerIp = tbprinter.printerService;
                lb_mayinso.Text = $@"Địa chỉ máy: {machineName} - Địa Chỉ Máy In: {_printerIp}";
                tb_LayTTinSosec.Enabled = true;
                bt_insosec.Enabled = true;
                tb_dkMayin.Text = _printerIp;
            }
            else
            {
                MessageBox.Show($"Máy {machineName} Chưa Khai Báo Máy in");
                tb_LayTTinSosec.Enabled = false;
                bt_insosec.Enabled = false;

            }
        }


        private void tb_TinDung_Enter(object sender, EventArgs e)
        {
            var cif = _localdb.Table<CifInfo>();
            AutoCompleteStringCollection cifatc = new AutoCompleteStringCollection();
            foreach (var i in cif)
            {
                var item = string.Format("{0} - {1}", i.cifno, i.acname);
                cb_td_tbdn_tk.Items.Add(item);
                cb_td_tbndh_tk.Items.Add(item);
                cifatc.Add(item);
                cb_td_pyctl_tk.Items.Add(item);
                cb_td_skdn_cif.Items.Add(item);

            }
            cb_td_tbdn_tk.AutoCompleteCustomSource = cifatc;
            cb_td_tbndh_tk.AutoCompleteCustomSource = cifatc;
            cb_td_pyctl_tk.AutoCompleteCustomSource = cifatc;
            cb_td_skdn_cif.AutoCompleteCustomSource = cifatc;
        }

        private void tb_tralai_Enter(object sender, EventArgs e)
        {


        }

        private void tc_td_tragoc_Enter(object sender, EventArgs e)
        {

            if (_ngaydl_loanmonth != null)
            {
                lb_td_tl_ngaydl.Text = _ngaydl_loanmonth.Value.ToShortDateString();
                lb_ngaydl.Text = _ngaydl_loanmonth.Value.ToShortDateString();
                lb_dhgl_ngaydl.Text = _ngaydl_loanmonth.Value.ToShortDateString();
            }
        }
        private void UpdateLabelText(string newText)
        {
            if (lb_solanin.InvokeRequired)
            {
                // this is worker thread
                UpdateLabelTextDelegate del = UpdateLabelText;
                lb_solanin.Invoke(del, newText);
            }
            else
            {
                // this is UI thread
                lb_solanin.Text = newText;
            }
        }

        private void bt_Td_pyctl_thugoclai_Click(object sender, EventArgs e)
        {


            var cifstring = cb_td_pyctl_tk.Text;

            if (cifstring.Trim() == "")
            {
                MessageBox.Show("Chưa nhập cif");
            }
            else
            {
                var cifno = Lib.ExtractNumber(cifstring);
                var cif = Convert.ToDecimal(cifno);
                var tinhthulai = _db.TinhThuLaiGocTheoCIF(dtp_td_pyctl_ngaytl.Value, cif).AsEnumerable().Cast<TinhThuLaiGocTheoCIF_Result>().ToList();
                if (tinhthulai.Count == 0)
                {
                    MessageBox.Show("Sai Cif hoặc cif không có tk vay");
                }
                else
                {
                    var ciflocal = _localdb.Table<CifInfo>().FirstOrDefault(c => c.cifno == cif);

                    if (ciflocal == null)
                    {
                        _localdb.Table<CifInfo>().Save(new CifInfo()
                        {
                            cifno = tinhthulai.ToList()[0].socif,
                            acname = tinhthulai.ToList()[0].khachhang
                        });

                    }
                    var tktt = _db.LayTKThanhToanTheoCIF(cif).AsEnumerable().Cast<LayTKThanhToanTheoCIF_Result>().ToList();

                    panelControl1.Controls.Clear();
                    var tl = new UC_ThuGocLai(tinhthulai, tktt, cb_td_pyctl_noigui.Text);
                    tl.Dock = DockStyle.Fill;
                    tl.SetLocaltion(_clickOnceLocation);
                    panelControl1.Controls.Add(tl);
                }

            }
        }

        private void bt_td_dhgl_laysl_Click(object sender, EventArgs e)
        {
            // var dl = _db.TraGocLai();

            var tungay = dtp_td_dhgl_bd.Value;
            var denngay = dtp_td_dhgl_kt.Value;
            var trlai = _db.TraGocLai(_ngaydl_loanmonth, tungay, denngay);
            GC_td_dhgl.DataSource = new BindingSource(trlai, "");
            GV_td_dhgl.OptionsView.ColumnAutoWidth = false;
            GV_td_dhgl.OptionsView.BestFitMaxRowCount = -1;
            GV_td_dhgl.BestFitColumns();


        }

        private void bt_td_dhgl_excel_Click(object sender, EventArgs e)
        {
            SaveFileExcel.FileName = "dhgl_";
            if (SaveFileExcel.ShowDialog() == DialogResult.OK)
            {
                var fi = new FileInfo(SaveFileExcel.FileName);

                if (fi.Exists)
                {
                    fi.Delete();
                }
                GV_td_dhgl.OptionsPrint.AutoWidth = false;
                GV_td_dhgl.BestFitColumns();
                GV_td_dhgl.ExportToXlsx(SaveFileExcel.FileName);
            }
            OpenExplorer(SaveFileExcel.FileName);
        }

        private void button14_Click_1(object sender, EventArgs e)
        {
            XRInNCLT rep = new XRInNCLT();

            rep.DataSource = (IList<DatainNoCo>)dgv_noco_tk.DataSource;
            using (ReportPrintTool printTool = new ReportPrintTool(rep))
            {

                // Invoke the Ribbon Print Preview form modally, 
                // and load the report document into it.
                printTool.ShowRibbonPreviewDialog();

                // Invoke the Ribbon Print Preview form
                // with the specified look and feel setting.

            }


        }
        

        private void bt_skdn_laytt_Enter(object sender, EventArgs e)
        {
            dtp_skdn_ngaydl.Value = _ngaydl_loanmonth.Value;
        }

        private void bt_skdn_laytt_Click(object sender, EventArgs e)
        {
            var cifstring = cb_td_skdn_cif.Text.ToString();
            var cifno = Lib.ExtractNumber(cifstring);

            if (cifno.Trim() == "")
            {
                MessageBox.Show("Chưa nhập cif");
            }
            else
            {
                var cif = Convert.ToDecimal(cifno);

                var _loan = from s in _db.LOANMONTHs
                            where s.Datadate == dtp_skdn_ngaydl.Value &&  s.CIFNO == cif && s.CBAL >0
                            orderby s.CURTYP, s.GROUP, s.ACCTNO
                            select s;
                List<skdn> listloan = new List<skdn>();

                foreach (var s in _loan)
                {
                    
                    listloan.Add(new skdn
                    {
                        NgayDL = s.Datadate.Value,
                        Socif = s.CIFNO,
                        DuNo = s.CBAL.Value,
                        LaiSuat = s.RATE.Value,
                        LoaiTien = s.CURTYP,
                        NgayDuyet = s.ORGDT6.Value,
                        TaiKhoan = string.Format(@"{0:###-##-##-######-#}", s.ACCTNO),
                        TenKhachHang = s.ACNAME,
                        KyHanGoc = s.MATDT6.Value,
                        TrangThai = s.STATUS.ToString(),
                        phaitra = s.CBAL < s.PMTAMT ? s.CBAL.Value : s.PMTAMT.Value,
                        quanhe = s.ODIND,
                        denngay = s.ODIND6  ,
                        LoaiSaoKe = ((s.GROUP == 13 )|| (s.GROUP == 13)) ? "NH": "TDH",
                        chovay = s.AMTREL.Value,
                        thuno = s.LTDPRN.Value,
                        thoigian = s.TERM.Value
                        
                        
                    });
                }



                GC_TD_SKDN.DataSource = new BindingSource(listloan, "");
                GV_TD_SKDN.BestFitColumns();
            }
        }



        // Function for read data from Excel worksheet into DataTable
        /*
                private DataTable WorksheetToDataTable(ExcelWorksheet ws, bool hasHeader = true)
                {
                    var dt = new DataTable(ws.Name);
                    var totalCols = ws.Dimension.End.Column;
                    var totalRows = ws.Dimension.End.Row;
                    var startRow = hasHeader ? 2 : 1;
                    foreach (var firstRowCell in ws.Cells[1, 1, 1, totalCols])
                    {
                        dt.Columns.Add(hasHeader ? firstRowCell.Text : $"Column {firstRowCell.Start.Column}");
                    }

                    for (var rowNum = startRow; rowNum <= totalRows; rowNum++)
                    {
                        var wsRow = ws.Cells[rowNum, 1, rowNum, totalCols];
                        var dr = dt.NewRow();
                        foreach (var cell in wsRow)
                        {
                            dr[cell.Start.Column - 1] = cell.Text;
                        }

                        dt.Rows.Add(dr);
                    }

                    return dt;
                }
        */

        /*
                private void button6_Click_1(object sender, EventArgs e)
                {
                    using (var openFileDialog1 = new OpenFileDialog())
                    {
                        openFileDialog1.Filter = @"Excel File (*.xlsx)|*.xlsx";
                        openFileDialog1.FilterIndex = 1;
                        if (openFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                using (ExcelPackage pck = new ExcelPackage())
                                {
                                    // Open the Excel file and load it to the ExcelPackage
                                    using (var stream = File.OpenRead(openFileDialog1.FileName))
                                    {
                                        pck.Load(stream);
                                    }

                                    ExcelWorksheet ws = pck.Workbook.Worksheets.First();
                                    int totalCols = ws.Dimension.End.Column;
                                    int totalRows = ws.Dimension.End.Row;
                                    int startRow = 2;
                                    ExcelRange wsRow;
                                    for (int rowNum = startRow; rowNum <= totalRows; rowNum++)
                                    {
                                        wsRow = ws.Cells[rowNum, 1, rowNum, totalCols];
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(@"Import failed. Original error: " + ex.Message);
                            }
                        }
                    }
                }
        */
    }
}