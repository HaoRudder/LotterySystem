using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LotterySystem
{
    public partial class NewAnalogData : Form
    {
        public static OddsSetting from6;
        public static AddRules from7;

        public NewAnalogData()
        {
            InitializeComponent();
        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {

        }

        private void NewAnalogData_Load(object sender, EventArgs e)
        {
            comboBox10.SelectedIndex = 0;
            GetList();
        }

        public void GetList()
        {
            var dt = new BLL.RulesBusiness().GetRuleinfoList();
            this.list.DataSource = dt;

            AddCheckBox();
        }

        public void AddCheckBox()
        {
            if (list.Columns[0].Name == "cb_check")
            {
                return;
            }
            DataGridViewCheckBoxColumn columncb = new DataGridViewCheckBoxColumn();
            columncb.HeaderText = "选择";
            columncb.Name = "cb_check";
            columncb.TrueValue = true;
            columncb.FalseValue = false;
            columncb.DataPropertyName = "IsChecked";
            list.Columns.Insert(0, columncb);    //添加的checkbox在第一列

            list.Columns[0].Width = 50;
        }

        private void 赔率设置ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (from6 == null)
            {
                from6 = new OddsSetting();
                from6.Assignment(0);
                from6.Show();
            }
            else
            {
                from6.Activate();
            }
        }

        private void 添加规则ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (from7 == null)
            {
                from7 = new AddRules();
                from7.Show();
            }
            else
            {
                from7.Activate();
            }
        }

        private void NewAnalogData_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1.from8 = null;
        }

        private void 刷新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetList();
        }

        private void list_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //checkbox 勾上
            if ((bool)list.Rows[e.RowIndex].Cells[0].EditedFormattedValue == true)
            {
                this.list.Rows[e.RowIndex].Cells[0].Value = false;
            }
            else
            {
                this.list.Rows[e.RowIndex].Cells[0].Value = true;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            var begin = Convert.ToDateTime(this.begin.Text);
            var end = Convert.ToDateTime(this.end.Text);
            var list = new BLL.SimulationBusiness().GetDataList("canada28", begin, end);

            dataGridView1.DataSource = list;
        }
    }
}
