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

        private void NewAnalogData_Load(object sender, EventArgs e)
        {
            begin.Text = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            end.Text = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
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
            if (e.RowIndex == -1)
            {
                return;
            }
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
            var idList = new List<int>();

            for (int i = 0; i < this.list.Rows.Count; i++)
            {
                var cb = (DataGridViewCheckBoxCell)this.list.Rows[i].Cells[0];

                bool flag = Convert.ToBoolean(cb.Value);
                if (flag == true)
                {
                    var val = this.list.Rows[i].Cells[1].Value;
                    idList.Add(Convert.ToInt32(val));
                }
            }

            if (idList.Count == 0)
            {
                MessageBox.Show("请至少选择一条规则");
                return;
            }

            var list = new BLL.SimulationBusiness().GetAnalogDataList(idList, "canada28", begin, end);
            dataGridView1.DataSource = list;
            dataGridView1.Columns[0].HeaderCell.Value = "方案";
            dataGridView1.Columns[0].Width = 80;
            dataGridView1.Columns[1].HeaderCell.Value = "开奖时间";
            dataGridView1.Columns[1].Width = 120;
            dataGridView1.Columns[2].HeaderCell.Value = "期号";
            dataGridView1.Columns[2].Width = 70;
            dataGridView1.Columns[3].HeaderCell.Value = "开奖数字";
            dataGridView1.Columns[3].Width = 70;
            dataGridView1.Columns[4].HeaderCell.Value = "属性";
            dataGridView1.Columns[5].HeaderCell.Value = "下注内容";
            dataGridView1.Columns[5].Width = 180;
            dataGridView1.Columns[6].HeaderCell.Value = "盈亏金额";
            dataGridView1.Columns[6].Width = 80;
            dataGridView1.Columns[7].HeaderCell.Value = "当前金额";
            dataGridView1.Columns[7].Width = 80;
            dataGridView1.Columns[8].HeaderCell.Value = "标注";
            dataGridView1.Columns[8].Width = 100;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var selectIndex = 0;
            if (dataGridView1.CurrentRow != null)
            {
                selectIndex = dataGridView1.CurrentRow.Index;
            }

            for (int i = selectIndex + 1; i < dataGridView1.Rows.Count; i++)
            {
                if (i == dataGridView1.Rows.Count - 1)
                {
                    i = 0;
                }
                var row = dataGridView1.Rows[i];
                if (!string.IsNullOrWhiteSpace(row.Cells["xiazhuneirong"].Value?.ToString()))
                {
                    dataGridView1.Rows[i].Selected = true;
                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[0];
                    return;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var beginTime = Convert.ToDateTime(begin.Text);
            var endTime = Convert.ToDateTime(end.Text);

            begin.Text = beginTime.AddDays(-1).ToString();
            end.Text = endTime.AddDays(-1).ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var beginTime = Convert.ToDateTime(begin.Text);
            var endTime = Convert.ToDateTime(end.Text);

            begin.Text = beginTime.AddMonths(-1).ToString();
            end.Text = endTime.AddMonths(-1).ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var beginTime = Convert.ToDateTime(begin.Text);
            var endTime = Convert.ToDateTime(end.Text);

            begin.Text = beginTime.AddDays(1).ToString();
            end.Text = endTime.AddDays(1).ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var beginTime = Convert.ToDateTime(begin.Text);
            var endTime = Convert.ToDateTime(end.Text);

            begin.Text = beginTime.AddMonths(1).ToString();
            end.Text = endTime.AddMonths(1).ToString();
        }
    }
}
