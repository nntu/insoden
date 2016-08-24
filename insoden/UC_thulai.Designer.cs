namespace insoden
{
    partial class UC_ThuLai
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.bt_td_pyctl_thulai_xuatexcel = new System.Windows.Forms.Button();
            this.GC_TD_Pyctl_ThuLai = new DevExpress.XtraGrid.GridControl();
            this.thuLaiBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.GV_TD_Pyctl_ThuLai = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colTenKh = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCif = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coltkvay = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSoTienPhaiTra = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colLaiCongDon = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colLaiTraCham = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPhuongThucTra = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.colGhiChu = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colLoaiTien = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GC_TD_Pyctl_ThuLai)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.thuLaiBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GV_TD_Pyctl_ThuLai)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.Horizontal = false;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.panelControl1);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.Controls.Add(this.GC_TD_Pyctl_ThuLai);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(667, 302);
            this.splitContainerControl1.SplitterPosition = 39;
            this.splitContainerControl1.TabIndex = 0;
            this.splitContainerControl1.Text = "splitContainerControl1";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.bt_td_pyctl_thulai_xuatexcel);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(667, 39);
            this.panelControl1.TabIndex = 0;
            this.panelControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.panelControl1_Paint);
            // 
            // bt_td_pyctl_thulai_xuatexcel
            // 
            this.bt_td_pyctl_thulai_xuatexcel.Location = new System.Drawing.Point(114, 6);
            this.bt_td_pyctl_thulai_xuatexcel.Name = "bt_td_pyctl_thulai_xuatexcel";
            this.bt_td_pyctl_thulai_xuatexcel.Size = new System.Drawing.Size(75, 23);
            this.bt_td_pyctl_thulai_xuatexcel.TabIndex = 1;
            this.bt_td_pyctl_thulai_xuatexcel.Text = "Xuất Excel";
            this.bt_td_pyctl_thulai_xuatexcel.UseVisualStyleBackColor = true;
            this.bt_td_pyctl_thulai_xuatexcel.Click += new System.EventHandler(this.bt_td_pyctl_thulai_xuatexcel_Click);
            // 
            // GC_TD_Pyctl_ThuLai
            // 
            this.GC_TD_Pyctl_ThuLai.DataSource = this.thuLaiBindingSource;
            this.GC_TD_Pyctl_ThuLai.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GC_TD_Pyctl_ThuLai.Location = new System.Drawing.Point(0, 0);
            this.GC_TD_Pyctl_ThuLai.MainView = this.GV_TD_Pyctl_ThuLai;
            this.GC_TD_Pyctl_ThuLai.Name = "GC_TD_Pyctl_ThuLai";
            this.GC_TD_Pyctl_ThuLai.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemComboBox1});
            this.GC_TD_Pyctl_ThuLai.Size = new System.Drawing.Size(667, 251);
            this.GC_TD_Pyctl_ThuLai.TabIndex = 0;
            this.GC_TD_Pyctl_ThuLai.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.GV_TD_Pyctl_ThuLai});
            // 
            // thuLaiBindingSource
            // 
            this.thuLaiBindingSource.DataSource = typeof(insoden.ThuLai);
            // 
            // GV_TD_Pyctl_ThuLai
            // 
            this.GV_TD_Pyctl_ThuLai.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colTenKh,
            this.colCif,
            this.coltkvay,
            this.colSoTienPhaiTra,
            this.colLaiCongDon,
            this.colLaiTraCham,
            this.colPhuongThucTra,
            this.colGhiChu,
            this.colLoaiTien});
            this.GV_TD_Pyctl_ThuLai.GridControl = this.GC_TD_Pyctl_ThuLai;
            this.GV_TD_Pyctl_ThuLai.Name = "GV_TD_Pyctl_ThuLai";
            this.GV_TD_Pyctl_ThuLai.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.GV_TD_Pyctl_ThuLai_CustomRowCellEdit);
            // 
            // colTenKh
            // 
            this.colTenKh.FieldName = "TenKh";
            this.colTenKh.Name = "colTenKh";
            this.colTenKh.Visible = true;
            this.colTenKh.VisibleIndex = 0;
            // 
            // colCif
            // 
            this.colCif.FieldName = "Cif";
            this.colCif.Name = "colCif";
            this.colCif.Visible = true;
            this.colCif.VisibleIndex = 1;
            // 
            // coltkvay
            // 
            this.coltkvay.FieldName = "tkvay";
            this.coltkvay.Name = "coltkvay";
            this.coltkvay.Visible = true;
            this.coltkvay.VisibleIndex = 2;
            // 
            // colSoTienPhaiTra
            // 
            this.colSoTienPhaiTra.DisplayFormat.FormatString = "D";
            this.colSoTienPhaiTra.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colSoTienPhaiTra.FieldName = "SoTienPhaiTra";
            this.colSoTienPhaiTra.Name = "colSoTienPhaiTra";
            this.colSoTienPhaiTra.Visible = true;
            this.colSoTienPhaiTra.VisibleIndex = 3;
            // 
            // colLaiCongDon
            // 
            this.colLaiCongDon.FieldName = "LaiCongDon";
            this.colLaiCongDon.Name = "colLaiCongDon";
            this.colLaiCongDon.Visible = true;
            this.colLaiCongDon.VisibleIndex = 4;
            // 
            // colLaiTraCham
            // 
            this.colLaiTraCham.FieldName = "LaiTraCham";
            this.colLaiTraCham.Name = "colLaiTraCham";
            this.colLaiTraCham.Visible = true;
            this.colLaiTraCham.VisibleIndex = 5;
            // 
            // colPhuongThucTra
            // 
            this.colPhuongThucTra.ColumnEdit = this.repositoryItemComboBox1;
            this.colPhuongThucTra.FieldName = "PhuongThucTra";
            this.colPhuongThucTra.Name = "colPhuongThucTra";
            this.colPhuongThucTra.Visible = true;
            this.colPhuongThucTra.VisibleIndex = 6;
            // 
            // repositoryItemComboBox1
            // 
            this.repositoryItemComboBox1.AutoHeight = false;
            this.repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemComboBox1.Name = "repositoryItemComboBox1";
            // 
            // colGhiChu
            // 
            this.colGhiChu.FieldName = "GhiChu";
            this.colGhiChu.Name = "colGhiChu";
            this.colGhiChu.Visible = true;
            this.colGhiChu.VisibleIndex = 7;
            // 
            // colLoaiTien
            // 
            this.colLoaiTien.FieldName = "LoaiTien";
            this.colLoaiTien.Name = "colLoaiTien";
            this.colLoaiTien.Visible = true;
            this.colLoaiTien.VisibleIndex = 8;
            // 
            // UC_ThuLai
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerControl1);
            this.Name = "UC_ThuLai";
            this.Size = new System.Drawing.Size(667, 302);
            this.Load += new System.EventHandler(this.UC_thulai_Load);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GC_TD_Pyctl_ThuLai)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.thuLaiBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GV_TD_Pyctl_ThuLai)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraGrid.GridControl GC_TD_Pyctl_ThuLai;
        private DevExpress.XtraGrid.Views.Grid.GridView GV_TD_Pyctl_ThuLai;
        private System.Windows.Forms.Button bt_td_pyctl_thulai_xuatexcel;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemComboBox1;
        private System.Windows.Forms.BindingSource thuLaiBindingSource;
        private DevExpress.XtraGrid.Columns.GridColumn colTenKh;
        private DevExpress.XtraGrid.Columns.GridColumn colCif;
        private DevExpress.XtraGrid.Columns.GridColumn coltkvay;
        private DevExpress.XtraGrid.Columns.GridColumn colSoTienPhaiTra;
        private DevExpress.XtraGrid.Columns.GridColumn colLaiCongDon;
        private DevExpress.XtraGrid.Columns.GridColumn colLaiTraCham;
        private DevExpress.XtraGrid.Columns.GridColumn colPhuongThucTra;
        private DevExpress.XtraGrid.Columns.GridColumn colGhiChu;
        private DevExpress.XtraGrid.Columns.GridColumn colLoaiTien;
    }
}
