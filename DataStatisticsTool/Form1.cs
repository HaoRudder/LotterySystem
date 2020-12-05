using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataStatisticsTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GetStatisticalData();
        }

        public void GetStatisticalData()
        {
            var dt= BLL.Logic.GetAllColumnDataByTime("canada",Convert.ToDateTime("2019-1-20 00:00:00"),Convert.ToDateTime("2019-1-20 23:59:59"));
        }
    }
}
