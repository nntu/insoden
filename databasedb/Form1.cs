using Lex.Db;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace databasedb
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Assembly assemblyInfo = Assembly.GetExecutingAssembly();
            //Location is where the assembly is run from

            //CodeBase is the location of the ClickOnce deployment files
            var uriCodeBase = new Uri(assemblyInfo.CodeBase);

            var _clickOnceLocation = Path.GetDirectoryName(uriCodeBase.LocalPath);

            var _localdb = new DbInstance(_clickOnceLocation + @"/data");
            _localdb.Map<CifInfo>().Automap(i => i.cifno);
            _localdb.Initialize();

            dlgocEntities dl = new dlgocEntities();
            var ds = dl.getCIFInfo();

            var lm = (from p in dl.LOANMONTHs
                      where p.Datadate == new DateTime(2017, 10, 31)
                      select new
                      {
                          p.CIFNO,
                          p.ACNAME
                      }).Distinct();

            foreach (var i in lm)
            {
                richTextBox1.AppendText(i.CIFNO + "\n");
                _localdb.Save(new CifInfo()
                {
                    cifno = (decimal)i.CIFNO,
                    acname = i.ACNAME
                });
            }
            
           _localdb.Compact();
        }
    }
}