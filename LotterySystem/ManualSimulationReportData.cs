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
    public partial class ManualSimulationReportData : Form
    {

        private static DataTable _Statistics;
        public ManualSimulationReportData()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Query();
        }
        public void Query()
        {
            var dt = BLL.Logic.GetReportDataPast(comboBox2.Text, dateTimePicker2.Text, dateTimePicker1.Text, comboBox3.Text);
            dataGridView1.DataSource = dt;
            HandleDataGridView(dataGridView1);
            _Statistics.Clear();
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    var item = dt.Rows[i];
            //    if (item[5].ToString() == "等待开奖")
            //    {
            //        dt.Rows.Remove(item);
            //        i--;
            //    }
            //}
            if (dt != null && dt.Rows.Count > 0)
            {
                var groupBy = dt.Rows.Cast<DataRow>().GroupBy(r => r.ItemArray[1]);
                var enumerable = groupBy as IGrouping<object, DataRow>[] ?? groupBy.ToArray();

                for (int i = 0; i < enumerable.Length; i++)
                {
                    var item = enumerable[i];
                    var grContent = item.GroupBy(x => x.ItemArray[3]);
                    var list = groupBy as IGrouping<object, DataRow>[] ?? grContent.ToArray();

                    for (int j = 0; j < list.Length; j++)
                    {
                        var dr = _Statistics.NewRow();
                        //类型，内容，投注金额，中奖金额，盈亏金额，投注次数，中奖次数，中奖比率
                        var temp = list[j];
                        dr["类型"] = item.Key;
                        dr["内容"] = temp.Key;
                        dr["投注金额"] = temp.Sum(x => Convert.ToDecimal(x.ItemArray[4])).ToString("f2");
                        dr["中奖金额"] = temp.Sum(x => Convert.ToDecimal(x.ItemArray[5]) > 0 ? Convert.ToDecimal(x.ItemArray[5]) : 0).ToString("f2");
                        //dr["盈亏金额"] = Convert.ToInt32(dr["中奖金额"]) - Convert.ToInt32(dr["投注金额"]);
                        dr["盈亏金额"] = temp.Sum(x => Convert.ToDecimal(x.ItemArray[5])).ToString("f2"); /*(Convert.ToDecimal(dr["中奖金额"]) - Convert.ToDecimal(dr["投注金额"])).ToString("f2");*/
                        dr["中奖次数"] = temp.Count(zxc => Convert.ToDecimal(zxc[5]) > 0);
                        dr["投注次数"] = temp.GroupBy(x => x.ItemArray[2]).Count();
                        dr["中奖比率"] = Convert.ToDouble((Convert.ToDecimal(dr["中奖次数"]) / temp.Count()).ToString("f2")) * 100 + "%";
                        _Statistics?.Rows.Add(dr);
                    }
                }
            }
            dataGridView2.DataSource = _Statistics;
            HandleDataGridView(dataGridView2);
        }

        public void HandleDataGridView(DataGridView item)
        {
            for (int j = 0; j < 2; j++)
            {
                dataGridView1.Columns[0].Frozen = false;//固定列

                var width = 0;
                for (int i = 0; i < item.Columns.Count; i++)
                {
                    //将每一列都调整为自动适应模式
                    item.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.AllCells);
                    //记录整个DataGridView的宽度
                    width += item.Columns[i].Width;
                }
                item.AutoSizeColumnsMode = width > item.Size.Width ? DataGridViewAutoSizeColumnsMode.DisplayedCells : DataGridViewAutoSizeColumnsMode.Fill;

                dataGridView1.Columns[0].Frozen = true;//固定列
            }
        }

        private void ManualSimulationReportData_Load(object sender, EventArgs e)
        {
            _Statistics = new DataTable
            {
                //类型，内容，投注金额，中奖金额，盈亏金额，投注次数，中奖次数，中奖比率
                Columns = {
                    { "类型",typeof(string)},
                    { "内容",typeof(string)},
                    { "投注金额", typeof(string)},
                    { "中奖金额", typeof(string)},
                    { "盈亏金额", typeof(string)},
                    { "投注次数", typeof(string)},
                    { "中奖次数", typeof(string)},
                    { "中奖比率", typeof(string)},
                }
            };
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            dateTimePicker2.Text = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            dateTimePicker1.Text = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");

            Query();
        }

        private void ManualSimulationReportData_FormClosing(object sender, FormClosingEventArgs e)
        {
            ManualSimulation.from7 = null;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var where = comboBox1.Text == "全部" ? string.Empty : "类型=" + "'" + comboBox1.Text + "'";
            var dt = _Statistics.Select(where);
            var newTable = _Statistics.Clone();
            for (int index = 0; index < dt.Length; index++)
            {
                DataRow t = dt[index];
                newTable.ImportRow(t);
            }
            dataGridView2.DataSource = newTable;
        }
    }
}
