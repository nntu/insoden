using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace insoden
{
    public partial class Inbclt : Form
    {
        private IList<DatainNoCo> datains;

        public Inbclt(IList<DatainNoCo> aList)
        {
            datains = aList;
            InitializeComponent();
        }

        public Inbclt()
        {
            InitializeComponent();
        }

        private void inbclt_Load(object sender, EventArgs e)
        {
         
            noco1.SetDataSource(datains);             
            crystalReportViewer1.ReportSource = noco1;             
            crystalReportViewer1.Refresh();
        }
    }
}