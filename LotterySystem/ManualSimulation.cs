using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL;

namespace LotterySystem
{
    public partial class ManualSimulation : Form
    {
        public static OddsSetting from6;
        public static ManualSimulationReportData from7;
        private int _width;
        private static DataTable _Statistics;

        public ManualSimulation()
        {
            InitializeComponent();
        }

        private void ManualSimulation_FormClosing(object sender, FormClosingEventArgs e)
        {

            var result = MessageBox.Show("是否确认关闭？", "关闭", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                e.Cancel = true;//就不退了
                return;
            }
   
            Form1.from6 = null;
            _Model.Clear();
            for (int i = 0; i < _Dt.Rows.Count; i++)
            {
                var item = _Dt.Rows[i];
                if (item[5].ToString() == "等待开奖")
                {
                    _Dt.Rows.Remove(item);
                    i--;
                }
            }
            if (_Dt.Rows.Count > 0)
            {
                Logic.AddReportData(_Dt, comboBox7.Text);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (from6 == null)
            {
                from6 = new OddsSetting();
                from6.Assignment(1);
                from6.Show();
            }
            else
            {
                from6.Activate();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var closingTime = textBox2.Text;
            var updateInterval = textBox3.Text;
            var beijiupdateInterval = textBox4.Text;

            var str = $@"ClosingTime:{closingTime},UpdateInterval:{updateInterval},BeiJiupdateInterval:{beijiupdateInterval}";
            var path = Environment.CurrentDirectory;
            var result = Tool.Helper.WriteFile(path, "SimulationSettings.ini", str);
            if (result)
            {
                MessageBox.Show("保存成功");
                groupBox4.Visible = false;
                return;
            }
            MessageBox.Show("保存失败");
        }

        private void 模拟配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            groupBox4.Visible = true;
            var str = Tool.Helper.ReadSimulationSettings();

            if (str.Count < 3)
            {
                MessageBox.Show("请先完成模拟配置");
                return;
            }
            textBox2.Text = str["ClosingTime"];
            textBox3.Text = str["UpdateInterval"]; ;
            textBox4.Text = str["BeiJiupdateInterval"];
        }

        private void label7_Click(object sender, EventArgs e)
        {
            groupBox4.Visible = false;
        }

        public void GetNewNumber()
        {
            timer2.Stop();
            var dt = Logic.GetNewOneInfo(comboBox7.Text == "加拿大" ? "canada" : "china");
            if (dt != null && dt.Rows.Count > 0)
            {
                label9.Text = dt.Rows[0]["number"].ToString();

                var str = Tool.Helper.ReadSimulationSettings();
                var updateTime = string.Empty;
                switch (comboBox7.Text)
                {
                    case "加拿大":
                        if (string.IsNullOrWhiteSpace(str["UpdateInterval"]))
                        {
                            MessageBox.Show("尚未配置加拿大更新间隔时间");
                            return;
                        }
                        updateTime = str["UpdateInterval"];
                        break;
                    case "北京":
                        if (string.IsNullOrWhiteSpace(str["BeiJiupdateInterval"]))
                        {
                            MessageBox.Show("尚未配置北京更新间隔时间");
                            return;
                        }
                        updateTime = str["BeiJiupdateInterval"];
                        break;
                }

                var time = Convert.ToDateTime(dt.Rows[0]["time"]);
                var sTime = time.AddSeconds(Convert.ToInt32(updateTime)).AddSeconds(3);//延迟三秒
                var temp = sTime - DateTime.Now;
                if (temp.Minutes > 0 && temp.Seconds > 0 && _Model.Count > 0)
                {
                    var result = Logic.ManualBet(comboBox7.Text == "加拿大" ? "canada" : "china", _Model);
                    for (int i = 0; i < result.Rows.Count; i++)
                    {
                        var item = result.Rows[i];
                        var tempDt = _Dt.Select($"编号='{item["编号"]}'");
                        tempDt[0]["盈亏"] = item["盈亏"];
                    }
                    //刷新一下
                    dataGridView1.DataSource = _Dt;

                    _Model.Clear();//清空队列

                    CalculationStatistics();
                }
                label10.Text = temp.Minutes.ToString();
                label13.Text = temp.Seconds.ToString();
                label12.Text = "分";
                label14.Text = "秒";
                timer1.Start();
            }
        }

        private void ManualSimulation_Load(object sender, EventArgs e)
        {
            _Dt = new DataTable
            {
                Columns = {
                    { "编号",typeof(string)},
                    { "类型",typeof(string)},
                    { "期数", typeof(string)},
                    { "投注内容", typeof(string)},
                    { "投注金额", typeof(string)},
                    { "盈亏", typeof(string)},
                    { "时间", typeof(string)},
                }
            };
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
            dataGridView1.DataSource = _Dt;
            comboBox1.SelectedIndex = 0;
            comboBox7.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            dateTimePicker2.Text = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            dateTimePicker1.Text = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
            GetNewNumber();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var s = Convert.ToInt32(label13.Text);
            var m = Convert.ToInt32(label10.Text);
            if (s <= 0 && m <= 0)
            {
                timer1.Stop();
                label10.Text = "正在获取...";
                label12.Text = "";
                label13.Text = "";
                label14.Text = "";
                timer2.Start();

            }
            else
            {
                var time = new TimeSpan(0, m, s - 1);
                label10.Text = time.Minutes.ToString();
                label13.Text = time.Seconds.ToString();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            GetNewNumber();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("请输入投注金额");
                return;
            }

            comboBox3.SelectedIndex = 0;
            groupBox5.Visible = true;
        }

        private void label18_Click(object sender, EventArgs e)
        {
            groupBox5.Visible = false;
        }

        private static List<ManualBetModel> _Model = new List<ManualBetModel>();
        private static DataTable _Dt = new DataTable();
        private void button5_Click(object sender, EventArgs e)
        {
            var str = Tool.Helper.ReadSimulationSettings();
            if (string.IsNullOrWhiteSpace(str["UpdateInterval"]))
            {
                MessageBox.Show("尚未配置封盘时间");
                groupBox5.Visible = false;
                return;
            }
            if (Convert.ToInt32(!Regex.IsMatch(label10.Text, @"^[+-]?\d*[.]?\d*$") ? "0" : label10.Text) * 60 + Convert.ToInt32(string.IsNullOrWhiteSpace(label13.Text) ? "0" : label13.Text) < Convert.ToInt32(str["ClosingTime"]))
            {
                MessageBox.Show("已经封盘");
                groupBox5.Visible = false;
                return;
            }
            var strList = checkedListBox1.CheckedItems.Cast<string>().ToList();
            if (strList.Count <= 0)
            {
                MessageBox.Show("请选择下注内容");
                groupBox5.Visible = false;
                return;
            }
            var number = Convert.ToInt32(label9.Text) + 1;
            _Model.Add(new ManualBetModel
            {
                ID = _Dt.Rows.Count + 1,
                Number = number.ToString(),
                Modeny = textBox1.Text,
                ContentList = strList,
                OddsName = comboBox3.Text
            });

            foreach (var item in strList)
            {
                var newRow = _Dt.NewRow();
                newRow["编号"] = _Dt.Rows.Count + 1;
                newRow["类型"] = comboBox3.Text;
                newRow["期数"] = number;
                newRow["投注内容"] = item;
                newRow["投注金额"] = textBox1.Text;
                newRow["盈亏"] = "等待开奖";
                newRow["时间"] = DateTime.Now;
                _Dt?.Rows.Add(newRow);
            }
            dataGridView1.DataSource = _Dt;
            groupBox5.Visible = false;
            textBox1.Text = "";
            for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
            {
                this.checkedListBox1.SetItemChecked(i, false);
            }
            HandleDataGridView(dataGridView1);
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

        private void button4_Click(object sender, EventArgs e)
        {
            var where = new List<string>();
            if (comboBox2.Text != "全部")
            {
                where.Add($"类型='{comboBox2.Text }'");
            }
            if (!string.IsNullOrWhiteSpace(dateTimePicker2.Text))
            {
                where.Add($"时间>='{Convert.ToDateTime(dateTimePicker2.Text)}'");
            }
            if (!string.IsNullOrWhiteSpace(dateTimePicker1.Text))
            {
                where.Add($"时间<='{Convert.ToDateTime(dateTimePicker1.Text)}'");
            }

            var table = _Dt.Select($"{string.Join(" and ", where)}");
            var newTable = _Dt.Clone();
            foreach (DataRow t in table)
            {
                newTable.ImportRow(t);
            }

            dataGridView1.DataSource = newTable;
        }

        public void CalculationStatistics()
        {
            //var result = Logic.ManualBet(comboBox7.Text == "加拿大" ? "canada" : "china", _Model);
            //for (int i = 0; i < result.Rows.Count; i++)
            //{
            //    var item = result.Rows[i];
            //    var tempDt = _Dt.Select($"编号='{item["编号"]}'");
            //    tempDt[0]["盈亏"] = item["盈亏"];
            //}
            ////刷新一下
            //dataGridView1.DataSource = _Dt;

            //_Model.Clear();//清空队列
            var dt = _Dt.Copy();
            _Statistics.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var item = dt.Rows[i];
                if (item[5].ToString() == "等待开奖")
                {
                    dt.Rows.Remove(item);
                    i--;
                }
            }
            if (dt.Rows.Count <= 0)
            {
                return;
            }
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
                    dr["盈亏金额"] = Convert.ToDecimal(dr["中奖金额"]) - Convert.ToDecimal(dr["投注金额"]);
                    //dr["盈亏金额"] = (temp.Sum(x => Convert.ToDecimal(x.ItemArray[5])) - Convert.ToDecimal(dr["投注金额"])).ToString("f2"); /*(Convert.ToDecimal(dr["中奖金额"]) - Convert.ToDecimal(dr["投注金额"])).ToString("f2");*/
                    dr["中奖次数"] = temp.Count(zxc => Convert.ToDecimal(zxc[5]) > 0);
                    dr["投注次数"] = temp.GroupBy(x => x.ItemArray[2]).Count();
                    dr["中奖比率"] = Convert.ToDouble((Convert.ToDecimal(dr["中奖次数"]) / temp.Count()).ToString("f2")) * 100 + "%";
                    _Statistics?.Rows.Add(dr);
                }
            }
            dataGridView2.DataSource = _Statistics;
            HandleDataGridView(dataGridView2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //这个是计算函数，查询单独写，只操作_Statistics
            CalculationStatistics();
        }

        private void aaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (from7 == null)
            {
                from7 = new ManualSimulationReportData();
                from7.Show();
            }
            else
            {
                from7.Activate();
            }
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

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            var row = _Dt.Select("盈亏='等待开奖'");
            if (row.Any())
            {
                MessageBox.Show("存在等待开奖的数据，必须等待开奖完成后才能切换");
                return;
            }
            GetNewNumber();
            _Dt.Clear();
            _Statistics.Clear();
            _Model.Clear();
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("当前未选中行");
                return;
            }
            var row = dataGridView1.SelectedRows;
            var index = row[0].Index;
            dataGridView1.Rows.RemoveAt(index);
            _Model.RemoveAll(x => x.ID == Convert.ToInt32(row[0].Cells[0].Value));
        }
    }
}
