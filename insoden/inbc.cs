using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace insoden
{
    public partial class Inbc : Form
    {
        private IList<DatainNoCo> _datains;
        public Inbc(IList<DatainNoCo> aList)
        {
            _datains = aList;
            InitializeComponent();
        }

        public Inbc()
        {
            InitializeComponent();
        }

        private void inbc_Load(object sender, EventArgs e)
        {

            nocoa51.SetDataSource(_datains);

            crystalReportViewer1.ReportSource = nocoa51;
            crystalReportViewer1.Refresh(); 
        }
    }
}
