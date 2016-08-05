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
            var doctoprint = new System.Drawing.Printing.PrintDocument();
            int rawKind = 0;
            for (int i = 0; i <= doctoprint.PrinterSettings.PaperSizes.Count - 1; i++)
            {
                if (doctoprint.PrinterSettings.PaperSizes[i].PaperName == "giaylientuc") // "LXP : Your Page Size"
                {
                    rawKind = Convert.ToInt32(doctoprint.PrinterSettings.PaperSizes[i].GetType().GetField("kind",
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic).GetValue(doctoprint.PrinterSettings.PaperSizes[i]));
                    break;
                }
            }
            noco1.SetDataSource(datains);
            //noco1.PrintOptions.PaperOrientation = PaperOrientation.Landscape;
            //   noco1.PrintOptions.PaperSize = (CrystalDecisions.Shared.PaperSize)rawKind;
            crystalReportViewer1.ReportSource = noco1;

            crystalReportViewer1.Refresh();
        }
    }
}