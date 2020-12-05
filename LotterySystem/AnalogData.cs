using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Configuration;

namespace LotterySystem
{
    public partial class AnalogData : Form
    {
        public static OddsSetting from6;
        public static AddRules from7;
        private static DataTable _Dt = new DataTable();
        static string _table;
        public AnalogData()
        {
            InitializeComponent();
        }



        /// <summary>
        /// 传递赋值
        /// </summary>
        /// <param name="table"></param>
        public void Assignment(string table)
        {
            _table = table;
        }

        private void 赔率设置ToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void AnalogData_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (button2.Text == "查询中...")
            {
                MessageBox.Show("请等待当前线程执行完成再关闭");
                e.Cancel = true;
                return;
            }
            if (button2.Text == "停止")
            {
                MessageBox.Show("请先停止后再关闭");
                e.Cancel = true;
                return;
            }
            var beginNumber = Tool.Helper.GetAutomaticBettingConfigureByName("BeginNumber"); //获取模拟真实投注的开始期号
            var isTrueBet = Tool.Helper.GetAutomaticBettingConfigureByName("IsTrueBet") == "True"; //获取模拟真实投注的开始期号
            Form1.from5 = null;
            BLL.Logic.listModel = new List<BLL.BetsModel>();
            BLL.Logic._Msg = string.Empty;
            BLL.Logic._DtInfo.Clear();
            BLL.Logic.timerNumber = isTrueBet ? string.Empty : beginNumber;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var content = Tool.Helper.ReadRuleList();


            //var zxc = "baozitongsha:Unchecked,baozihuiben:Checked,duizihuiben:Checked,shunzihuiben:Checked,linjiuhuiben:Checked,dadan:4.25,xiaodan:4.65,dashuang:4.65,xiaoshuang:4.25,duizi:3,shunzi:15,baozi:2.96,jishu:12,da:2,xiao:2,dan:2,shuang:2,tema0:12,tema1:12,tema2:12,tema3:12,tema4:12,tema5:12,tema6:12,tema7:12,tema8:12,tema9:12," +
            //          "tema10:12,tema11:12,tema12:12,tema13:12,tema14:12,tema15:12,tema16:12,tema17:12,tema18:12,tema19:12,tema20:12,tema21:12,tema22:12,tema23:12,tema24:12,tema25:12,tema26:12,tema27:12,topzuhe:0,topsixiang:2,toptema:12,topduizi:3,downzuhe:0,downsixiang:0,downtema:12,downduizi:3,fenshu:1000,dadanxiaoshuang:1.2," +
            //          "xiaodandashuang:1.2,duishunbao1314:7";

            //var asd = "";
            //foreach (var item in zxc.Split(','))
            //{
            //    var qwe = "";
            //    foreach (var temp in item.Split(':'))
            //    {
            //        qwe += '"' + temp + '"' + ':';
            //    }

            //    qwe= qwe.Substring(0, qwe.Length - 1);
            //    asd += qwe + ',';
            //}

            //asd = "{"+asd.Substring(0, asd.Length - 1)+"}";
            


            if (comboBox2.SelectedIndex != 2 && string.IsNullOrWhiteSpace(textBox1.Text) && comboBox2.SelectedIndex != 3)
            {
                MessageBox.Show("开始投注期数不能为空");
                textBox1.Focus();
                return;
            }
            if (comboBox2.SelectedIndex != 2 && string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("连续不中期数不能为空");
                textBox2.Focus();
                return;
            }
            if (checkBox8.CheckState == CheckState.Checked && checkBox10.CheckState == CheckState.Unchecked)
            {

                if (string.IsNullOrWhiteSpace(textBox4.Text))
                {
                    MessageBox.Show("盈利倍投不能为空");
                    textBox4.Focus();
                    return;
                }
            }
            else
            {
                textBox4.Text = "";
            }
            if (checkBox9.CheckState == CheckState.Checked && checkBox10.CheckState == CheckState.Unchecked)
            {
                if (string.IsNullOrWhiteSpace(textBox5.Text))
                {
                    MessageBox.Show("亏损倍投不能为空");
                    textBox5.Focus();
                    return;
                }
            }
            else
            {
                textBox5.Text = "";
            }

            if (checkBox4.CheckState == CheckState.Checked)
            {
                if (string.IsNullOrWhiteSpace(textBox6.Text))
                {
                    MessageBox.Show("请填写停止下注金额");
                    return;
                }
            }

            if (checkBox5.CheckState == CheckState.Checked)
            {
                if (string.IsNullOrWhiteSpace(textBox8.Text))
                {
                    MessageBox.Show("请填写停止下注金额");
                    return;
                }
            }

            if (checkBox4.CheckState == CheckState.Checked || checkBox5.CheckState == CheckState.Checked)
            {
                if (string.IsNullOrWhiteSpace(textBox7.Text))
                {
                    MessageBox.Show("请填写停止下注时间");
                    return;
                }
            }

            if (comboBox2.SelectedIndex == 3)
            {
                var abaList = comboBox1.Text.ToString().Split('|').ToList();
                if (abaList.Count != 3 || abaList[0] != abaList[2])
                {
                    MessageBox.Show("请输入正确的ABA格式");
                    return;
                }
            }

            var lossStr = textBox4.Text;
            var profitStr = textBox5.Text;

            var str = string.Empty;
            var zyInfo = checkBox4.CheckState == CheckState.Checked ? $"只盈|{textBox6.Text}|{textBox7.Text}" : "";
            var zkInfo = checkBox5.CheckState == CheckState.Checked ? $"只亏|{textBox8.Text}|{textBox7.Text}" : "";
            //拼接只盈只亏的内容
            if (comboBox2.SelectedIndex == 2)
            {
                str = $"{comboBox1.Text},{comboBox2.Text},,,,,,{lossStr},{profitStr},{comboBox6.Text},{zkInfo},,{zyInfo}";
            }
            else
            {
                var tzdw = checkBox7.CheckState == CheckState.Checked ? "停止档位" : "";
                str = (checkBox1.CheckState == CheckState.Unchecked ? "模拟投注-" : "自动投注-") + $"{comboBox1.Text},{comboBox2.Text},{textBox1.Text},{comboBox3.Text},{textBox2.Text},{comboBox4.Text},{(checkBox3.CheckState == CheckState.Checked ? "亏损立即下注" : checkBox6.CheckState == CheckState.Checked ? "盈利立即下注" : checkBox2.CheckState == CheckState.Checked ? "直到未中后恢复" : "立即恢复")},{(checkBox10.CheckState == CheckState.Unchecked ? lossStr : string.Empty)},{(checkBox10.CheckState == CheckState.Unchecked ? profitStr : string.Empty)},{comboBox6.Text},{zkInfo},{tzdw},{zyInfo},{(checkBox10.CheckState == CheckState.Checked ? "通用倍投" : "")}";
            }

            if (content.Split('\n').Any(item => str == item))
            {
                MessageBox.Show("该规则已存在");
                return;
            }

            var newContent = string.Empty;
            var contentList = content.Split('\n').ToList();
            contentList.RemoveAll(string.IsNullOrWhiteSpace);

            newContent = contentList.ToList().Aggregate(newContent, (current, item) => current + (item + "\n"));

            newContent += str;

            var path = Environment.CurrentDirectory;
            var result = Tool.Helper.WriteFile(path, "RuleContent.ini", newContent + "\n");
            list.Rows.Add(false, str);
        }

        private void AnalogData_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
            comboBox6.SelectedIndex = 0;
            comboBox7.SelectedIndex = 0;
            comboBox8.SelectedIndex = 0;
            comboBox10.SelectedIndex = 0;
            comboBox11.SelectedIndex = 0;

            begin.Text = Convert.ToDateTime(begin.Text).ToString("yyyy-MM-dd 00:00:00");
            end.Text = Convert.ToDateTime(end.Text).ToString("yyyy-MM-dd 23:59:59");

            var columncb = new DataGridViewCheckBoxColumn
            {
                HeaderText = "",
                Name = "cb_check",
                TrueValue = true,
                FalseValue = false,
                DataPropertyName = "IsChecked",
                Width = 50
            };
            list.Columns.Insert(0, columncb);    //添加的checkbox在第一列

            var txt = new DataGridViewTextBoxColumn();
            txt.HeaderText = "规则内容";
            txt.Name = "rule";
            txt.Width = 606;
            txt.ReadOnly = true;
            list.Columns.Insert(1, txt);

            var content = Tool.Helper.ReadRuleList();
            foreach (var item in content.Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                list.Rows.Add(false, item);
            }
            list.Columns[0].Frozen = true;
            checkBox1.CheckState = CheckState.Unchecked;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (list.SelectedRows.Count == 0)
            {
                return;
            }
            if (MessageBox.Show("是否确认删除该规则？", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                var contentList = Tool.Helper.ReadRuleList().Split('\n').ToList();
                contentList.RemoveAll(x => x == list.Rows[list.SelectedRows[0].Index].Cells[1].Value.ToString());
                list.Rows.RemoveAt(list.SelectedRows[0].Index);
                var str = string.Empty;
                foreach (var item in contentList)
                {
                    str += item + "\n";
                }
                var result = Tool.Helper.WriteFile(Environment.CurrentDirectory, "RuleContent.ini", str + "\n");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (checkBox1.CheckState == CheckState.Checked)
            {
                var beginNumber = textBox10.Text; //获取模拟真实投注的开始期号
                var isTrueBet = comboBox10.Text == "真实投注"; //获取模拟真实投注的开始期号
                if (comboBox10.Text == "模拟投注")
                {
                    if (string.IsNullOrWhiteSpace(beginNumber))
                    {
                        MessageBox.Show("请输入开始期号");
                        return;
                    }
                    Tool.Helper.WriteFile(Environment.CurrentDirectory, "AutomaticBettingConfigure.ini", $"BeginNumber:{beginNumber},IsTrueBet:{isTrueBet.ToString()}");
                    BLL.Logic.timerNumber = isTrueBet ? string.Empty : beginNumber;
                }
                else
                {
                    Tool.Helper.WriteFile(Environment.CurrentDirectory, "AutomaticBettingConfigure.ini", $"BeginNumber:{string.Empty},IsTrueBet:{isTrueBet.ToString()}");
                    BLL.Logic.timerNumber = isTrueBet ? string.Empty : beginNumber;
                }
                if (button2.Text == "停止")
                {
                    this.begin.Enabled = true;
                    this.end.Enabled = true;

                    timer1.Enabled = false;
                    button2.Text = "开始";
                    startAndStop = false;
                    BLL.Logic.listModel = new List<BLL.BetsModel>();
                    BLL.Logic._Msg = string.Empty;
                    BLL.Logic._DtInfo.Clear();
                    BLL.Logic.timerNumber = isTrueBet ? string.Empty : beginNumber;
                    HandleInfo("已停止执行...");
                    return;
                }

                textBox9.Text = string.Empty;
                startAndStop = true;
            }

            var begin = Convert.ToDateTime(this.begin.Text);
            var end = Convert.ToDateTime(this.end.Text);
            var ruleList = new List<string>();
            for (int i = 0; i < list.Rows.Count; i++)
            {
                string _selectValue = list.Rows[i].Cells["cb_check"].EditedFormattedValue.ToString();
                var txt = list.Rows[i].Cells["rule"].Value.ToString();

                if (_selectValue == "True" && txt.Contains("通用倍投"))
                {
                    if (checkBox8.CheckState == CheckState.Checked)
                    {
                        if (string.IsNullOrWhiteSpace(textBox4.Text))
                        {
                            MessageBox.Show("通用倍投，盈利倍投框不能为空");
                            return;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(textBox5.Text))
                        {
                            MessageBox.Show("通用倍投，亏损倍投框不能为空");
                            return;
                        }
                    }
                }

                if (_selectValue == "True")
                {
                    ruleList.Add(txt);
                }
                if (checkBox1.CheckState == CheckState.Checked && _selectValue == "True" && list.Rows[i].Cells[1].Value.ToString().Split(',').Length <= 14)
                {
                    MessageBox.Show($"第{i + 1}条规则中，请添加投注网站以及房间");
                    return;
                }
            }

            for (int i = 0; i < ruleList.FindAll(x => x.Contains("通用倍投")).Count; i++)
            {
                var list = ruleList[i].Replace("自动投注-", "").Split(',');
                if (!string.IsNullOrWhiteSpace(textBox4.Text))
                {
                    list[7] = textBox4.Text;
                }
                else
                {
                    list[8] = textBox5.Text;
                }
                ruleList[i] = string.Join(",", list);
            }

            if (ruleList.Count == 0)
            {
                MessageBox.Show("请选择规则");
                return;
            }

            if (checkBox1.CheckState == CheckState.Unchecked && ruleList.Count > 2)
            {
                MessageBox.Show("最多只能选择两条规则");
                return;
            }
            var monthly = false;
            if (begin.Year != end.Year || begin.Month != end.Month)
            {
                monthly = true;
                comboBox5.SelectedIndex = 1;
            }
            else
            {
                comboBox5.SelectedIndex = 0;
            }
            button2.Text = "查询中...";
            button2.Enabled = false;
            dataGridView1.DataSource = null;
            textBox3.Text = string.Empty;

            if (checkBox1.CheckState == CheckState.Checked)
            {
                begin = DateTime.Now;
                this.begin.Enabled = false;
                this.end.Enabled = false;
                this.begin.Text = begin.ToString();
                this.end.Text = DateTime.Now.AddYears(10).ToString();
            }


            Task.Run(() =>
            {
                string str = string.Empty;
                var dt = new DataTable();
                if (checkBox1.CheckState == CheckState.Unchecked)
                {
                    dt = BLL.Logic.SimulatedDataReporting(_table, begin, end, ruleList, out str);
                    Invoke(new MethodInvoker(delegate
                    {
                        timer1.Stop();
                    }));
                }
                else
                {
                    AutomaticBetting(begin, ruleList);
                    return;
                }
                Invoke(new MethodInvoker(delegate
                {
                    if (dt == null)
                    {
                        MessageBox.Show("没有数据，请检查数据库是否有数据,如有数据，请检查数据库配置是否正确，并重启");
                        Invoke(new MethodInvoker(delegate
                        {
                            button2.Text = "开始";
                            button2.Enabled = true;
                        }));
                        return;
                    }
                    dataGridView1.DataSource = dt;
                    int width;
                    dataGridView1.Columns[0].Width = 80;
                    dataGridView1.Columns[1].Width = 120;
                    dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
                    if (dt.Rows.Count > 17)
                    {
                        width = dataGridView1.Width - dataGridView1.Columns[0].Width - dataGridView1.Columns[1].Width - 20;
                    }
                    else
                    {
                        width = dataGridView1.Width - dataGridView1.Columns[0].Width - dataGridView1.Columns[1].Width - 3;
                    }
                    for (int i = 2; i < dataGridView1.Columns.Count; i++)
                    {
                        dataGridView1.Columns[i].Width = width / (dataGridView1.Columns.Count - 2);
                        dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                    textBox3.Text = str;
                }));


                #region 统计

                //DataTable profitDt = new DataTable();
                //DataTable lossDt = new DataTable();
                ////1、添加列
                //profitDt.Columns.Add("盈利/时间", typeof(string));
                //profitDt.Columns.Add("盈亏", typeof(string));
                //lossDt.Columns.Add("亏损/时间", typeof(string));
                //lossDt.Columns.Add("盈亏", typeof(string));

                //var temp = new List<int>();
                //foreach (var item in ruleList)
                //{
                //    var one = item.Split(',')[7].Split('|').Length;
                //    var two = item.Split(',')[8].Split('|').Length;
                //    temp.Add(one);
                //    temp.Add(two);
                //}
                //for (int i = 0; i < temp.Max(x => x); i++)
                //{
                //    profitDt.Columns.Add($"{i + 1}", typeof(string));
                //    lossDt.Columns.Add($"{i + 1}", typeof(string));
                //}

                //var timeList = new List<string>();
                //for (int i = 0; i < dt.Rows.Count; i++)
                //{
                //    if (monthly)
                //    {
                //        var time = Convert.ToDateTime(dt.Rows[i]["开奖时间"]).ToString("yyyy-MM");
                //        if (!timeList.Contains(time))
                //        {
                //            timeList.Add(time);
                //        }

                //    }
                //    else
                //    {
                //        var time = Convert.ToDateTime(dt.Rows[i]["开奖时间"]).ToString("yyyy-MM-dd");
                //        if (!timeList.Contains(time))
                //        {
                //            timeList.Add(time);
                //        }
                //    }
                //}
                ////var timeList = dt.AsEnumerable().Select(x => x.Field<string>("开奖时间")).ToList().Select(x => monthly ? x.Substring(0, x.LastIndexOf("/")) : x.Substring(0, x.IndexOf(" "))).Distinct();

                //foreach (var item in timeList)
                //{
                //    var profitDr = profitDt.NewRow();
                //    profitDr["盈利/时间"] = item;
                //    var lossDr = lossDt.NewRow();
                //    lossDr["亏损/时间"] = item;
                //    var tempDt = dt.Clone();

                //    DataRow[] tempList = null;
                //    if (monthly)
                //    {
                //        tempList = dt.Select($" CONVERT(开奖时间,System.DateTime) <= '{Convert.ToDateTime(item).ToString($"yyyy/MM/{Convert.ToDateTime(item).AddDays(1 - Convert.ToDateTime(item).Day).AddMonths(1).AddDays(-1).Day} 23:59:59")}' and CONVERT(开奖时间,System.DateTime) >= '{Convert.ToDateTime(item).ToString("yyyy/MM/01 00:00:00")}'");
                //    }
                //    else
                //    {
                //        tempList = dt.Select($" CONVERT(开奖时间,System.DateTime) <= '{Convert.ToDateTime(item).ToString("yyyy/MM/dd 23:59:59")}' and CONVERT(开奖时间,System.DateTime) >= '{Convert.ToDateTime(item).ToString("yyyy/MM/dd 00:00:00")}'");
                //    }
                //    foreach (var r in tempList)
                //    {
                //        tempDt.ImportRow(r);
                //    }
                //    var xcvxcv = tempDt.Clone();
                //    for (int i = 0; i < tempDt.Rows.Count; i++)
                //    {
                //        if (!string.IsNullOrWhiteSpace(tempDt.Rows[i]["下注内容"].ToString()))
                //        {
                //            xcvxcv.ImportRow(tempDt.Rows[i]);
                //        }
                //    }
                //    DataView dv = xcvxcv.DefaultView;
                //    dv.Sort = "期号";
                //    var dtCopy = dv.ToTable();
                //    var rows = dtCopy.Rows;

                //    profitDt.Rows.Add(profitDr);
                //    lossDt.Rows.Add(lossDr);
                //    for (int i = 0; i < rows.Count; i++)
                //    {
                //        var cbv = rows[i]["盈亏金额"].ToString().Split(' ');
                //        for (int z = 0; z < cbv.Length; z++)
                //        {
                //            if (string.IsNullOrWhiteSpace(cbv[z]))
                //            {
                //                continue;
                //            }
                //            profitDr["盈亏"] = Convert.ToDecimal(string.IsNullOrWhiteSpace(profitDr["盈亏"].ToString()) ? "0" : profitDr["盈亏"].ToString()) + Convert.ToDecimal(cbv[z]);
                //            lossDr["盈亏"] = Convert.ToDecimal(string.IsNullOrWhiteSpace(lossDr["盈亏"].ToString()) ? "0" : lossDr["盈亏"].ToString()) + Convert.ToDecimal(cbv[z]);
                //        }

                //        bool isNull = string.IsNullOrWhiteSpace(rows[i == 0 ? 0 : i - 1]["标注"].ToString());
                //        var ykList = rows[i == 0 ? 0 : i - 1]["标注"].ToString().Split('|');
                //        var xzList = rows[i]["下注内容"].ToString().Split(' ');

                //        for (int j = 0; j < xzList.Length; j++)
                //        {
                //            if (string.IsNullOrWhiteSpace(xzList[j]))
                //            {
                //                continue;
                //            }
                //            var asd = xzList[j].Split(',')[1].ToString();
                //            if (i == 0 || (!isNull && ykList[j] == "中奖"))
                //            {
                //                var zxc = ruleList[j].Split(',')[7].Split('|').ToList().IndexOf(asd) + 1;
                //                if (i == 0 && zxc == 0)
                //                {
                //                    zxc = ruleList[j].Split(',')[8].Split('|').ToList().IndexOf(asd) + 1;
                //                }

                //                profitDt.Rows[profitDt.Rows.Count - 1][zxc.ToString()] = string.IsNullOrWhiteSpace(profitDt.Rows[profitDt.Rows.Count - 1][zxc.ToString()].ToString()) ? 1 : Convert.ToInt32(profitDt.Rows[profitDt.Rows.Count - 1][zxc.ToString()]) + 1;
                //            }
                //            else
                //            {
                //                var zxc = ruleList[j].Split(',')[8].Split('|').ToList().IndexOf(asd) + 1;
                //                if (i == 0 && zxc == 0)
                //                {
                //                    zxc = ruleList[j].Split(',')[7].Split('|').ToList().IndexOf(asd) + 1;
                //                }
                //                lossDt.Rows[lossDt.Rows.Count - 1][zxc.ToString()] = string.IsNullOrWhiteSpace(lossDt.Rows[lossDt.Rows.Count - 1][zxc.ToString()].ToString()) ? 1 : Convert.ToInt32(lossDt.Rows[lossDt.Rows.Count - 1][zxc.ToString()]) + 1;
                //            }
                //        }
                //    }
                //}
                #endregion

                Invoke(new MethodInvoker(delegate
                {
                    //dataGridView2.DataSource = profitDt;
                    //dataGridView2.Columns[0].Frozen = true;
                    //dataGridView3.DataSource = lossDt;
                    //dataGridView3.Columns[0].Frozen = true;

                    button2.Text = "开始";
                    button2.Enabled = true;
                }));

            });
        }

        static DateTime timerBegin; static List<string> timerRuleList;
        public void AutomaticBetting(DateTime begin, List<string> ruleList)
        {
            timerBegin = begin;
            timerRuleList = ruleList;

            Invoke(new MethodInvoker(delegate
            {
                timer1.Start();
                timer2.Start();
            }));
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.CheckState == CheckState.Checked)
            {
                checkBox2.CheckState = CheckState.Unchecked;
                checkBox6.CheckState = CheckState.Unchecked;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.CheckState == CheckState.Checked)
            {
                checkBox3.CheckState = CheckState.Unchecked;
                checkBox6.CheckState = CheckState.Unchecked;
            }
        }

        //private void checkBox4_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (checkBox4.CheckState == CheckState.Checked)
        //    {
        //        checkBox5.CheckState = CheckState.Unchecked;
        //    }
        //}

        //private void checkBox5_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (checkBox5.CheckState == CheckState.Checked)
        //    {
        //        checkBox4.CheckState = CheckState.Unchecked;
        //    }
        //}

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
                if (!string.IsNullOrWhiteSpace(row.Cells["下注内容"].Value.ToString()))
                {
                    dataGridView1.Rows[i].Selected = true;
                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[0];
                    return;
                }
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.CheckState == CheckState.Checked)
            {
                checkBox2.CheckState = CheckState.Unchecked;
                checkBox3.CheckState = CheckState.Unchecked;
            }
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox8.CheckState == CheckState.Checked)
            {
                checkBox9.CheckState = CheckState.Unchecked;
            }
            else
            {
                checkBox8.CheckState = CheckState.Checked;
            }
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox9.CheckState == CheckState.Checked)
            {
                checkBox8.CheckState = CheckState.Unchecked;
            }
            else
            {
                checkBox9.CheckState = CheckState.Checked;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var com = (ComboBox)sender;
            var text = com.Text;
            if (text == "自定义投注")
            {
                var str = Interaction.InputBox("请输入自定义投注参数（以竖杠隔开，例：大|小|单|双）", "设置");
                if (string.IsNullOrWhiteSpace(str))
                {
                    com.SelectedIndex = 0;
                }
                else
                {
                    if (com.Items.IndexOf(str) > -1)
                    {
                        MessageBox.Show("该内容已存在");
                        return;
                    }
                    com.Items.Add(new ListViewItem(str, str).Text.TrimEnd('|'));
                    com.SelectedIndex = com.Items.Count - 1;
                }
            }
        }

        private string custom = string.Empty;
        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            var com = (ComboBox)sender;
            for (int i = 0; i < com.Items.Count; i++)
            {
                var index = com.Items.IndexOf(com.Items[i]);
                if (index > 16)
                {
                    custom = com.Items[i].ToString();
                    com.Items.RemoveAt(i);
                }
            }
        }

        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            var com = (ComboBox)sender;
            if (com.Text == "自定义投注")
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(com.Text))
            {
                com.Items.Add(new ListViewItem(custom, custom).Text.TrimEnd('|'));
                com.SelectedIndex = com.Items.Count - 1;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.Text == "ABA下注")
            {
                textBox1.Enabled = false;
            }
            else
            {
                //递归循环出页面所有控件
                var strList = new List<Control>();

                foreach (Control ctl in Controls)
                {
                    if (ctl.Text == "自动投注规则")
                    {
                        GetControls(ctl, strList);
                    }
                }

                strList = strList.FindAll(x => x.GetType().Name.Contains("ComboBox") || x.GetType().Name.Contains("TextBox") || x.GetType().Name.Contains("CheckBox") || x.GetType().Name.Contains("Button"));
                foreach (var item in strList)
                {
                    if (item.Name == "button6" || item.Name == "checkBox1" || item.Name == "button5" || item.Name == "comboBox8" || item.Name == "comboBox7" || item.Name == "textBox7" || item.Name == "textBox8" || item.Name == "textBox6" || item.Name == "checkBox5" || item.Name == "checkBox4" || item.Name == "comboBox1" || item.Name == "comboBox2" || item.Name == "list" || item.Name == "button1" || item.Name == "button3" || item.Name == "checkBox8" || item.Name == "checkBox9" || item.Name == "textBox4" || item.Name == "textBox5")
                    {
                        continue;
                    }

                    if (comboBox2.SelectedIndex == 2)
                    {
                        item.Enabled = false;
                    }
                    else
                    {
                        item.Enabled = true;
                    }
                }
            }
        }
        /// <summary>
        /// 获取所有控件
        /// </summary>
        /// <param name="fatherControl"></param>
        /// <param name="strList"></param>
        private void GetControls(Control fatherControl, List<Control> strList)
        {
            var sonControls = fatherControl.Controls;
            //遍历所有控件
            foreach (Control c in sonControls)
            {
                strList.Add(c);
                GetControls(c, strList);
            }
        }

        /// <summary>
        /// 窗口显示处理信息
        /// </summary>
        /// <param name="content">需要显示的信息</param>
        private void HandleInfo(string content)
        {
            Invoke(
                new MethodInvoker(
                    delegate
                    {
                        textBox9.Text += @"   " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + content +
                                         "\r\n\r\n";
                    }));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            textBox9.SelectionStart = textBox9.Text.Length;
            textBox9.SelectionLength = 0;
            textBox9.ScrollToCaret();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var isCheck = checkBox1.CheckState == CheckState.Checked;
            List<ComboBox> comList = new List<ComboBox> { comboBox1, comboBox3, comboBox4 };
            foreach (var item in comList)
            {
                if (isCheck)
                {
                    item.Items.Add("自定义投注");
                }
                else
                {
                    var index = item.Items.IndexOf("自定义投注");
                    if (index > -1)
                    {
                        item.Items.RemoveAt(index);
                    }
                }
            }

            var content = Tool.Helper.ReadRuleList();
            list.Rows.Clear();
            foreach (var item in content.Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                if (isCheck)
                {
                    if (item.Contains("模拟投注"))
                    {
                        continue;
                    }
                }
                else
                {
                    if (item.Contains("自动投注"))
                    {
                        continue;
                    }
                }
                list.Rows.Add(false, item);
            }

            if (isCheck)
            {
                comboBox10.Visible = true;
                textBox10.Visible = true;
            }
            else
            {
                comboBox10.Visible = false;
                textBox10.Visible = false;
                comboBox10.SelectedIndex = 0;
            }

            comboBox7.SelectedIndex = 0;
            comboBox7.Enabled = isCheck;
            comboBox8.Enabled = isCheck;
            button5.Enabled = isCheck;
            button6.Enabled = isCheck;
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox8.DataSource = null;
            if (comboBox7.Text.Contains("请选择"))
            {
                comboBox8.Items.Add("请选择房间");
            }
            else
            {
                comboBox8.DataSource = MatchingRoom(comboBox7.Text);
            }
            comboBox8.SelectedIndex = 0;
        }

        public List<string> MatchingRoom(string websiteName)
        {
            switch (websiteName)
            {
                case "南宫": return new List<string> { "加拿大1.88倍", "加拿大2.0倍", "加拿大2.5倍", "加拿大2.8倍", "加拿大3.2倍", "北京2倍", "北京2.5倍" };
            }
            return new List<string>();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (comboBox7.SelectedIndex == 0)
            {
                MessageBox.Show("请选择投注网站");
                return;
            }
            var val = list.Rows[list.SelectedRows[0].Index].Cells[1].Value.ToString();
            var beginIndex = val.IndexOf("[");
            var endIndex = val.IndexOf("]");
            var currentVal = $"{comboBox7.Text}|{comboBox8.Text}";
            if (beginIndex > -1 && endIndex > -1)
            {
                if (val.IndexOf(currentVal) > -1)
                {
                    MessageBox.Show("该投注网站和投注房间已存在");
                    return;
                }
                var temp = val.Substring(beginIndex + 1, endIndex - 1 - beginIndex);
                var info = $"{temp}&{currentVal}";
                list.Rows[list.SelectedRows[0].Index].Cells[1].Value = val.Substring(0, beginIndex) + $"[{info}]";
            }
            else
            {
                list.Rows[list.SelectedRows[0].Index].Cells[1].Value = val + $",[{currentVal}]";
            }
            var str = list.Rows[list.SelectedRows[0].Index].Cells[1].Value.ToString();
            var newContent = string.Empty;
            var contentList = Tool.Helper.ReadRuleList().Split('\n').ToList();
            contentList.RemoveAll(string.IsNullOrWhiteSpace);
            contentList.RemoveAll(x => x == val);
            newContent = contentList.ToList().Aggregate(newContent, (current, item) => current + (item + "\n"));

            newContent += str;
            var path = Environment.CurrentDirectory;
            var result = Tool.Helper.WriteFile(path, "RuleContent.ini", newContent + "\n");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (button6.Text == "取消删除")
            {
                comboBox9.Visible = false;
                button6.Text = "删除选中网站";
                return;
            }
            var val = list.Rows[list.SelectedRows[0].Index].Cells[1].Value.ToString();
            var beginIndex = val.IndexOf("[");
            var endIndex = val.IndexOf("]");
            if (beginIndex > -1 && endIndex > -1)
            {
                var temp = val.Substring(beginIndex + 1, endIndex - 1 - beginIndex);
                var tempList = temp.Split('&');
                comboBox9.DataSource = tempList;
                comboBox9.Visible = true;
                button6.Text = "取消删除";
            }
            else
            {
                MessageBox.Show("该规则中没有可以删除的投注网站");
            }
        }

        private void comboBox9_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var val = list.Rows[list.SelectedRows[0].Index].Cells[1].Value.ToString();
            var tempVal = val;
            var beginIndex = val.IndexOf("[");
            var endIndex = val.IndexOf("]");
            if (beginIndex > -1 && endIndex > -1)
            {
                var temp = val.Substring(beginIndex + 1, endIndex - 1 - beginIndex);
                var tempList = temp.Split('&').ToList();
                tempList.RemoveAll(x => x == comboBox9.Text);
                var newVal = tempList.Aggregate(string.Empty, (current, item) => current + (item + "&"));
                newVal = newVal.TrimEnd('&');
                val = val.Substring(0, beginIndex) + (string.IsNullOrWhiteSpace(newVal) ? "" : $"[{newVal}]");
                if (string.IsNullOrWhiteSpace(newVal))
                {
                    val = val.TrimEnd('&');
                    val = val.Substring(0, val.Length - 1);
                }
                list.Rows[list.SelectedRows[0].Index].Cells[1].Value = val;
            }

            var str = list.Rows[list.SelectedRows[0].Index].Cells[1].Value.ToString();
            var newContent = string.Empty;
            var contentList = Tool.Helper.ReadRuleList().Split('\n').ToList();
            contentList.RemoveAll(string.IsNullOrWhiteSpace);
            contentList.RemoveAll(x => x == tempVal);
            newContent = contentList.ToList().Aggregate(newContent, (current, item) => current + (item + "\n"));
            newContent += str;
            var path = Environment.CurrentDirectory;
            var result = Tool.Helper.WriteFile(path, "RuleContent.ini", newContent + "\n");
            comboBox9.DataSource = null;
            comboBox9.Visible = false;
            button6.Text = "删除选中网站";
        }

        private bool startAndStop = true;
        private void timer1_Tick(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                Invoke(new MethodInvoker(delegate
                {
                    timer1.Stop();
                }));

                string str = string.Empty;
                var dt = BLL.Logic.AutomaticBettingCalculation(_table, timerRuleList, timerBegin, out str);
                Invoke(new MethodInvoker(delegate
                {
                    if (startAndStop)
                    {
                        button2.Text = "停止";
                        button2.Enabled = true;
                    }
                    if (dt == null)
                    {
                        MessageBox.Show("没有数据，请检查数据库是否有数据,如有数据，请检查数据库配置是否正确，并重启");
                        Invoke(new MethodInvoker(delegate
                        {
                            button2.Text = "开始";
                            button2.Enabled = true;
                        }));
                        return;
                    }

                    if (dataGridView1.DataSource == null)
                    {
                        dataGridView1.DataSource = dt;
                    }
                    else
                    {
                        var num = 0;
                        foreach (DataRow item in dt.Rows)
                        {
                            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                            {
                                if (dataGridView1.Rows[i].Cells[2].Value.ToString() == item["期号"].ToString())
                                {
                                    num++;
                                    break;
                                }
                                num = 0;
                            }

                            if (num == 0)
                            {
                                ((DataTable)dataGridView1.DataSource).ImportRow(item);
                                for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
                                {
                                    dataGridView1.Rows[dataGridView1.SelectedRows[i].Index].Selected = false;
                                }
                                dataGridView1.Rows[dataGridView1.RowCount - 1].Selected = true;
                                dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
                            }
                        }
                    }

                    int width;
                    dataGridView1.Columns[0].Width = 80;
                    dataGridView1.Columns[1].Width = 120;
                    dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
                    if (dt.Rows.Count > 17)
                    {
                        width = dataGridView1.Width - dataGridView1.Columns[0].Width - dataGridView1.Columns[1].Width - 20;
                    }
                    else
                    {
                        width = dataGridView1.Width - dataGridView1.Columns[0].Width - dataGridView1.Columns[1].Width - 3;
                    }
                    for (int i = 2; i < dataGridView1.Columns.Count; i++)
                    {
                        dataGridView1.Columns[i].Width = width / (dataGridView1.Columns.Count - 2);
                        dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                    textBox3.Text = str;

                    if (startAndStop)
                    {
                        timer1.Start();
                    }
                }));
            });
        }

        /// <summary>
        /// 定时异步获取下注信息并显示在页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Stop();
            var logNumber = Convert.ToInt32(ConfigurationManager.AppSettings["LogNumber"]); //获取保留日志数量
            Task.Run(() =>
            {
                try
                {
                    var strList = BLL.Logic._Msg.Split('\n').ToList();
                    strList.RemoveAll(string.IsNullOrWhiteSpace);
                    if (strList.Count > logNumber)
                    {
                        for (int i = strList.Count; i <= strList.Count; i--)
                        {
                            if (i > logNumber)
                            {
                                strList.RemoveAt(0);
                            }
                        }
                    }
                    foreach (var item in strList)
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            var txt = string.Empty;
                            Invoke(new MethodInvoker(delegate { txt = textBox9.Text; }));
                            var list = txt.Split('\n').ToList();
                            list.RemoveAll(string.IsNullOrWhiteSpace);
                            var isCon = list.FindAll(x => x.Split('	')[1].Contains(item)).Count > 0;

                            if (isCon)
                            {
                                continue;
                            }
                            HandleInfo(item);
                        }
                    }
                    Invoke(new MethodInvoker(delegate { timer2.Start(); }));
                }
                catch (Exception)
                {

                }
            });
        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox10.SelectedIndex == 0)
            {
                textBox10.Text = string.Empty;
                textBox10.Enabled = false;
            }
            else
            {
                textBox10.Enabled = true;
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            var website = comboBox11.Text;
            Task.Run(() =>
            {
                var reuslt = BLL.TrueBet.QueryForTheLatestBalance(website);
                HandleInfo($"网站：{website}，最新余额为：{reuslt}");
            });
        }

        private void 添加规则ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (from7 == null)
            {
                from7 = new AddRules();
                //from7.Assignment(0);
                from7.Show();
            }
            else
            {
                from7.Activate();
            }
        }
    }
}
