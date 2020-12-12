using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace LotterySystem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //Timer timer1 = new Timer();
        TimeSpan updateTime = new TimeSpan(0, 0, 0);
        string table = string.Empty;
        int suspend = 0;
        public static UnopenedDetails from2;
        public static TotalNumberDetails from3;
        public static SystemConfiguration from4;
        public static AnalogData from5;
        public static ManualSimulation from6;
        public static NewAnalogData from8;

        private void Form1_Load(object sender, EventArgs e)
        {
            #region 初始化时间为当天
            wkBeginTime.Text = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            wkEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
            zgBeginTime.Text = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            zgEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
            zkBeginTime.Text = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            zkEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
            #endregion

            var path = Tool.Helper.ReadConfiguration(Tool.ConfigurationType.ProgramPath) + "\\GetTheLatestData.exe";
            if (string.IsNullOrWhiteSpace(path))
            {
                MessageBox.Show("打开跑数据工具失败，请检查路径是否设置");
                return;
            }
            var result = Tool.Helper.OpenFile(path);
            if (!result)
            {
                MessageBox.Show("跑数据程序打开失败，请检查路径是否正确");
                return;
            }
            if (Tool.Helper.JudgementConfigurationIsNull())
            {
                MessageBox.Show("系统配置为空，请先配置");
                return;
            }
            if (Tool.Helper.JudgementConfigurationIsNull())
            {
                return;
            }
            comboBox1.SelectedIndex = 0;
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
        /// 查询列表
        /// </summary>
        public void QueryList()
        {
            timer2.Stop();
            if (string.IsNullOrWhiteSpace(this.comboBox2.Text))
            {
                return;
            }
            var index = Convert.ToInt32(Tool.Helper.GetNumber(this.index.Text.Substring(0, this.index.Text.IndexOf("/"))));
            var indexSize = Convert.ToInt32(Tool.Helper.GetNumber(this.comboBox2.Text));
            int total;

            Task.Run(() =>
            {
                var dt = BLL.Logic.QueryLotteryList(table, index, indexSize, out total);
                Invoke(new MethodInvoker(delegate
                {
                    if (dt == null || dt.Rows.Count <= 0)
                    {
                        MessageBox.Show("程序内部出错，请重试");
                        return;
                    }
                    cpList.DataSource = dt;
                    cpList.Columns[0].DefaultCellStyle.Format = "yyyy-MM-dd  HH:mm:ss";

                    #region 获取最新数据 （获取到最新数据一条数据的时间后，加上设置的时间间隔再用当前时间减去这个时间，最后等于出来的时间就是我们最终的倒计时更新时间）

                    var intervalTime = comboBox1.Text.Contains("北京") ? (Convert.ToInt32(Tool.Helper.ReadConfiguration(Tool.ConfigurationType.beijingTime)) / 1000) : Convert.ToInt32(Tool.Helper.ReadConfiguration(Tool.ConfigurationType.jianadaTime)) / 1000;
                    updateTime = Convert.ToDateTime(Convert.ToDateTime(dt.Rows[0]["开奖时间"]).ToString("HH:mm:ss")).AddSeconds(intervalTime) - Convert.ToDateTime(DateTime.Now.ToString("HH:mm:ss"));

                    //默认加载的时候就从这儿获取最新的一条
                    var newDt = BLL.Logic.GetNewOneInfo(table);
                    if (newDt == null)
                    {
                        countDownTime.Text = "没有获取到数据，重新获取";
                        return;
                    }
                    var newRow = newDt.Rows[0];
                    newOne.Text = newRow["one"].ToString();
                    newTwo.Text = newRow["two"].ToString();
                    newThree.Text = newRow["three"].ToString();
                    newSum.Text = newRow["sum"].ToString();
                    zuhe.Text = !string.IsNullOrWhiteSpace(newRow["组合"].ToString()) ? newRow["组合"].ToString() : "";
                    teshu.Text = !string.IsNullOrWhiteSpace(newRow["特殊"].ToString()) ? newRow["特殊"].ToString() : "";
                    jizhi.Text = !string.IsNullOrWhiteSpace(newRow["极值"].ToString()) ? newRow["极值"].ToString() : "";
                    newNumber.Text = "最新期 " + newRow["number"].ToString();

                    this.indexSize.Text = "共" + total + "条";
                    this.index.Text = index + "/" + (total / indexSize);
                    #endregion

                    HandleDgvColor();   //渲染单元格颜色
                    HandleDgv(cpList, dt.Rows.Count > 25);  //渲染dvg
                    GetAnalysisDataByType(null, null, 0);
                    timer2.Start();
                }));
            });
        }

        /// <summary>
        /// 控件绑定值
        /// </summary>
        /// <param name="newStrList">分析数据的键值对</param>
        /// <param name="prefix">控件前缀</param>
        /// <param name="key">名称</param>
        /// <param name="val">值</param>
        public void ControlBinding(List<Control> newStrList, string prefix, string key, string val)
        {
            var pj = prefix + Tool.Helper.ConvertToAllSpell(key);
            var txt = newStrList.FirstOrDefault(x => x.Name == pj);
            VoiceAnnouncements(prefix, key, val);
            txt.Text = val;
        }

        /// <summary>
        /// 语音播报
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void VoiceAnnouncements(string prefix, string key, string val)
        {
            var qz = prefix == "wk" ? "未开" : "连开";
            var py = Tool.Helper.ConvertToAllSpell(key);
            var str = Tool.Helper.GetReminderConfigurationByName(Tool.Helper.ConvertToAllSpell(comboBox1.Text) + prefix + "tx" + py);
            var temp = new List<string>();
            if (string.IsNullOrWhiteSpace(str))
            {
                return;
            }
            temp = str.Split(',').ToList();

            //语音播报
            var voice = new SpeechSynthesizer
            {
                Rate = 2, //设置语速,[-10,10] 
                Volume = 100 //设置音量,[0,100] 
            };
            foreach (var item in temp)
            {
                if (item == val)
                {
                    voice.SpeakAsync($"{key},{val}期{qz}，请注意！"); ; //创建语音实例
                }
            }
        }

        /// <summary>
        /// 处理dgv列表的颜色
        /// </summary>
        public void HandleDgvColor()
        {
            var list = new List<Control> { zuhe, teshu, jizhi }.FindAll(x => !string.IsNullOrWhiteSpace(x.Text));
            foreach (var item in new List<Control> { zuhe, teshu, jizhi })
            {
                if (string.IsNullOrWhiteSpace(item.Text)) { item.Visible = false; } else { item.Visible = true; }

            }
            if (list.Count == 1) list[0].Location = new Point(382, 69);
            if (list.Count == 2) { list[0].Location = new Point(337, 69); list[1].Location = new Point(430, 69); }
            if (list.Count == 3) { list[0].Location = new Point(299, 70); list[1].Location = new Point(382, 69); list[2].Location = new Point(470, 69); }

            //循环行，列
            for (int i = 0; i < cpList.Rows.Count; i++)
            {
                for (int j = 0; j < cpList.ColumnCount; j++)
                {
                    var val = cpList.Rows[i].Cells[j].Value.ToString();

                    //让内容居中
                    cpList.Rows[i].Cells[j].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    //判断颜色
                    if (!string.IsNullOrWhiteSpace(val) && (val == "大" || val == "单" || val == "大单" || val == "小单"))
                    {
                        cpList.Rows[i].Cells[j].Style.BackColor = Color.FromArgb(43, 147, 255);
                    }
                    if (!string.IsNullOrWhiteSpace(val) && (val == "小" || val == "双" || val == "大双" || val == "小双"))
                    {
                        cpList.Rows[i].Cells[j].Style.BackColor = Color.FromArgb(242, 68, 57);
                    }
                }
            }
        }

        /// <summary>
        /// 处理dgv自适应宽度效果
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="dropDown">是否有下拉框</param>
        public void HandleDgv(DataGridView dgv, bool dropDown)
        {
            dgv.Columns[0].Width = 126;
            dgv.Columns[1].Width = 76;
            dgv.Columns[2].Width = 60;

            var width = 0;

            if (dropDown)
            {
                dgv.Columns[1].Width = 76;
                width = dgv.Width - dgv.Columns[0].Width - dgv.Columns[1].Width - dgv.Columns[2].Width - 20;
            }
            else
            {
                width = dgv.Width - dgv.Columns[0].Width - dgv.Columns[1].Width - dgv.Columns[2].Width;
            }
            for (int i = 3; i < dgv.Columns.Count; i++)
            {
                dgv.Columns[i].Width = width / (dgv.Columns.Count - 3);
            }
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                dgv.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            cpList.Columns[0].Frozen = true;
            cpList.Columns[1].Frozen = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var count = this.index.Text.Substring(this.index.Text.IndexOf("/") + 1, this.index.Text.Length - (this.index.Text.IndexOf("/")) - 1);
            var index = Convert.ToInt32(Tool.Helper.GetNumber(this.index.Text.Substring(0, this.index.Text.IndexOf("/")))) - 1;
            if (index <= 0)
            {
                return;
            }
            this.index.Text = index + "/" + count;
            QueryList();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var count = this.index.Text.Substring(this.index.Text.IndexOf("/") + 1, this.index.Text.Length - (this.index.Text.IndexOf("/")) - 1);

            var index = Convert.ToInt32(Tool.Helper.GetNumber(this.index.Text.Substring(0, this.index.Text.IndexOf("/")))) + 1;
            if (index >= Convert.ToInt32(count))
            {
                return;
            }
            this.index.Text = index + "/" + count;
            QueryList();
        }

        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            index.Text = 1 + "/" + 1;
            QueryList();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var count = this.index.Text.Substring(this.index.Text.IndexOf("/") + 1, this.index.Text.Length - (this.index.Text.IndexOf("/")) - 1);
            var index = Convert.ToInt32(string.IsNullOrWhiteSpace(Tool.Helper.GetNumber(jumpText.Text)) ? "1" : Tool.Helper.GetNumber(jumpText.Text));

            if (index > Convert.ToInt32(count))
            {
                index = Convert.ToInt32(count);
            }
            if (index <= 0)
            {
                this.index.Text = 1 + "/" + count;
            }
            else
            {
                this.index.Text = index + "/" + count;
            }
            jumpText.Text = index.ToString();
            QueryList();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (button1.Text == "恢复")
            {
                wkBeginTime.Text = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                wkEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                button1.Text = "查询";
                return;
            }

            button1.Enabled = false;
            var begin = Convert.ToDateTime(wkBeginTime.Text);
            var end = Convert.ToDateTime(wkEndTime.Text);

            if (wkBeginTime.Text != DateTime.Now.ToString("yyyy-MM-dd 00:00:00") || wkEndTime.Text != DateTime.Now.ToString("yyyy-MM-dd 23:59:59"))
            {
                button1.Text = "恢复";
            }
            Task.Run(() =>
            {
                GetAnalysisDataByType(begin, end, 1);
                Invoke(new MethodInvoker(delegate { button1.Enabled = true; }));
            });
        }

        /// <summary>
        /// 根据类型查询分析数据
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="time"></param>
        /// <param name="type">0:默认查询所有（可空参数），1：连续未开，2：最高，3：总开奖</param>
        public void GetAnalysisDataByType(DateTime? begin, DateTime? time, int type)
        {
            //获取分析数据的键值对
            var dic = BLL.Logic.AnalysisData(table, begin, time, type);
            if (dic == null)
            {
                MessageBox.Show("未获取导数据，请重新获取！");
                return;
            }
            //递归循环出页面所有控件
            var strList = new List<Control>();
            foreach (Control ctl in Controls)
            {
                GetControls(ctl, strList);
            }
            //取出需要绑定的控件
            var newStrList = strList.FindAll(x => x.Name.Contains("wk") || x.Name.Contains("zg") || x.Name.Contains("zk"));

            //绑值
            foreach (var item in dic)
            {
                foreach (var temp in item.Value)
                {
                    if (item.Key.Contains("未开"))
                    {
                        Invoke(new MethodInvoker(delegate { ControlBinding(newStrList, "wk", temp.Key, temp.Value.ToString()); }));
                    }
                    else if (item.Key.Contains("最高"))
                    {
                        Invoke(new MethodInvoker(delegate { ControlBinding(newStrList, "zg", temp.Key, temp.Value.ToString()); }));
                    }
                    else if (item.Key.Contains("总开"))
                    {
                        Invoke(new MethodInvoker(delegate { ControlBinding(newStrList, "zk", temp.Key, temp.Value.ToString()); }));
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "恢复")
            {
                zgBeginTime.Text = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                zgEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                button2.Text = "查询";
                return;
            }

            button2.Enabled = false;
            var begin = Convert.ToDateTime(zgBeginTime.Text);
            var end = Convert.ToDateTime(zgEndTime.Text);

            if (zgBeginTime.Text != DateTime.Now.ToString("yyyy-MM-dd 00:00:00") || zgEndTime.Text != DateTime.Now.ToString("yyyy-MM-dd 23:59:59"))
            {
                button2.Text = "恢复";
            }

            Task.Run(() =>
            {
                GetAnalysisDataByType(begin, end, 2);
                Invoke(new MethodInvoker(delegate { button2.Enabled = true; }));
            });

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (button3.Text == "恢复")
            {
                zgBeginTime.Text = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                zgEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                button3.Text = "查询";
                return;
            }

            button3.Enabled = false;
            var begin = Convert.ToDateTime(zgBeginTime.Text);
            var end = Convert.ToDateTime(zgEndTime.Text);

            if (zgBeginTime.Text != DateTime.Now.ToString("yyyy-MM-dd 00:00:00") || zgEndTime.Text != DateTime.Now.ToString("yyyy-MM-dd 23:59:59"))
            {
                button3.Text = "恢复";
            }

            Task.Run(() =>
            {
                GetAnalysisDataByType(begin, end, 3);
                Invoke(new MethodInvoker(delegate { button3.Enabled = true; }));
            });
        }

        /// <summary>
        /// 倒计时刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                return;
            }

            if (countDownTime.Text.Contains("重新获取") && suspend < 3)
            {
                suspend++;
                countDownTime.Text = $"重新获取 {3 - suspend}";
                return;
            }

            var jianadaBegin = Convert.ToDateTime(Tool.Helper.ReadConfiguration(Tool.ConfigurationType.jianadaBeginDormancyTime));
            var jianadaEnd = Convert.ToDateTime(Tool.Helper.ReadConfiguration(Tool.ConfigurationType.jianadaEndDormancyTime));
            var beijingBegin = Convert.ToDateTime(Tool.Helper.ReadConfiguration(Tool.ConfigurationType.beijingBeginDormancyTime));
            var beijingEnd = Convert.ToInt32(Tool.Helper.ReadConfiguration(Tool.ConfigurationType.beijingEndDormancyTime));
            var atime = beijingBegin;
            var tempTime = Convert.ToDateTime(atime.AddMinutes(beijingEnd).ToString("HH:mm:ss")) - Convert.ToDateTime(DateTime.Now.ToString("HH:mm:ss"));
            var time = DateTime.Now.AddSeconds(Convert.ToInt32(tempTime.Hours * 3600) + Convert.ToInt32(tempTime.Minutes * 60));

            if (comboBox1.SelectedItem.ToString().Contains("北京") && (DateTime.Now >= beijingBegin || DateTime.Now <= time))
            {
                countDownTime.Text = $"{comboBox1.Text }现在是休眠期";
                return;
            }
            if (comboBox1.SelectedItem.ToString().Contains("加拿大") && Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:00")) >= jianadaBegin && Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:59")) <= jianadaEnd)
            {
                countDownTime.Text = $"{comboBox1.Text }现在是休眠期";
                return;
            }
            updateTime = updateTime.Subtract(new TimeSpan(0, 0, 1));
            var str = $"{updateTime.Minutes } 分 {updateTime.Seconds} 秒";
            countDownTime.Text = str;

            //必须提前一秒钟给控件赋值，如果在0秒赋值，赋值的动作是一闪而过，直接就执行到查询操作了，会导致主线程卡主，也就是赋值没有效果，虽然我也不晓得为什么会这样！
            if (updateTime < new TimeSpan(0, 0, 1))
            {
                countDownTime.Text = "获取数据中...";
            }

            if (updateTime < new TimeSpan(0, 0, 0))
            {
                while (true)
                {
                    var newInfo = BLL.Logic.QueryNewLottery(table, Convert.ToInt64(cpList.Rows[0].Cells["开奖期号"].Value));
                    if (newInfo == null)
                    {
                        countDownTime.Text = "程序内部出错，请重试";
                        return;
                    }
                    if (newInfo.Rows.Count > 0 && suspend >= 3)
                    {
                        break;
                    }
                    if (newInfo.Rows.Count > 0)
                    {
                        break;
                    }
                    countDownTime.Text = "没有获取到数据，重新获取中";
                    return;
                }
                QueryList();
                //语音播报
                var voice = new SpeechSynthesizer
                {
                    Rate = 2, //设置语速,[-10,10] 
                    Volume = 100 //设置音量,[0,100] 
                }.SpeakAsync("来电啦！"); ; //创建语音实例
                suspend = 0;
            }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            switch (comboBox1.Text)
            {
                case "加拿大28": table = "canada "; break;
                case "北京28": table = "china "; break;
                case "新疆28": table = "`新疆28` "; break;
                case "重庆28": table = "`重庆28` "; break;
            }
            if (comboBox2.SelectedIndex == -1)
            {
                comboBox2.SelectedIndex = 0;
            }
            else
            {
                QueryList();
            }
        }

        /// <summary>
        /// 弹出子窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AcquisitionStatistics(object sender, EventArgs e)
        {
            var name = ((Label)sender).Text;
            var type = string.IsNullOrWhiteSpace(((Control)sender).Parent.Text);
            
            if (from2 == null)
            {
                from2 = new UnopenedDetails();
                from2.GetStatisticsData(table, name, type);
                from2.Show();
            }
            else
            {
                from2.Activate();
            }
        }

        /// <summary>
        /// 总开奖统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TotalNumberJump(object sender, EventArgs e)
        {
            if (from3 == null)
            {
                from3 = new TotalNumberDetails();
                from3.GetStatisticsData(table, true);
                from3.Show();
            }
            else
            {
                from3.Activate();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (from4 == null)
            {
                from4 = new SystemConfiguration();
                from4.Show();
            }
            else
            {
                from4.Activate();
            }
        }

        private void SettingVoiceConfiguration(object sender, EventArgs e)
        {
            var name = ((Label)sender).Name;
            var val = Tool.Helper.GetReminderConfigurationByName(Tool.Helper.ConvertToAllSpell(comboBox1.Text) + name);

            var str = Interaction.InputBox("请输入提醒期数（以英文逗号隔开，例：2,3,4）", "设置", val);
            if (string.IsNullOrWhiteSpace(str))
            {
                return;
            }

            str = str.Trim().TrimEnd(',');
            var temp = $@"{Tool.Helper.ConvertToAllSpell(comboBox1.Text) + name}:{str}";

            var path = Tool.Helper.ReadConfiguration(Tool.ConfigurationType.LotterySystemPath);
            var list = Tool.Helper.ReadTheLocalFile(path + "\\ReminderConfiguration.ini").Trim().Split('\n').ToList();
            list.RemoveAll(x => string.IsNullOrWhiteSpace(x));
            var index = list.ToList().FindIndex(x => x.Split(':')[0] == Tool.Helper.ConvertToAllSpell(comboBox1.Text) + name);
            var content = string.Empty;
            if (index == -1)
            {
                list.Add(temp);
            }
            else
            {
                list[index] = temp;

            }

            content = list.Aggregate(string.Empty, (current, item) => current + (item + "\n"));
            var result = Tool.Helper.WriteFile(path, "ReminderConfiguration.ini", content);
            if (result)
            {
                MessageBox.Show("保存成功");
                return;
            }
            MessageBox.Show("保存失败");
        }

        private void 模拟数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (from5 == null)
            {
                from5 = new AnalogData();
                from5.Assignment(table);
                from5.Show();
            }
            else
            {
                from5.Activate();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (from3 == null)
            {
                from3 = new TotalNumberDetails();
                from3.GetStatisticsData(table, false);
                from3.Show();
            }
            else
            {
                from3.Activate();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //退出时，一起退出数据采集工具
          Tool.Helper.KillProcess(new List<string> { "GetTheLatestData" });
        }

        private void 手动数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (from6 == null)
            {
                from6 = new ManualSimulation();
                //from6.Assignment(table);
                from6.Show();
            }
            else
            {
                from6.Activate();
            }
        }

        private void 新版模拟数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (from8 == null)
            {
                from8 = new NewAnalogData();
                //from6.Assignment(table);
                from8.Show();
            }
            else
            {
                from8.Activate();
            }
        }
    }
}
