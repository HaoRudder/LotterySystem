using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GetTheLatestData
{
    public partial class Form1 : Form
    {
        string _dbAddress;
        string _dbName;
        string _dbAcc;
        string _dbPwd;
        int _updateInterval;
        string _tableList;
        string _testDbAcc;
        string _testDbPwd;
        string _testDBName;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GetconfigurationFile();
            HandleInfo("程序已开启...");
            update_Click(null, null);
            comboBox1.SelectedIndex = 0;

            //// 注意判断关闭事件reason来源于窗体按钮，否则用菜单退出时无法退出!
            //this.WindowState = FormWindowState.Minimized;
            //this.notifyIcon1.Visible = true;
            ////任务栏区显示图标
            //this.ShowInTaskbar = false;
            //this.Hide();

            //if (this.WindowState == FormWindowState.Minimized)
            //{
            //    this.Visible = false;
            //    notifyIcon1.Visible = true;
            //    int tipShowMilliseconds = 1000;
            //    string tipTitle = "数据采集器提醒";
            //    string tipContent = "程序已启动";
            //    ToolTipIcon tipType = ToolTipIcon.Info;
            //    notifyIcon1.ShowBalloonTip(tipShowMilliseconds, tipTitle, tipContent, tipType);
            //}
            //else
            //{
            //    notifyIcon1.Visible = false;
            //}
        }

        /// <summary>
        /// 获取最新的数据
        /// </summary>
        public void GetTheLatestData(bool isByTime = false)
        {
            timer1.Stop();
            Task.Run(() =>
            {
                try
                {
                    var tableList = _tableList.Split(',');

                    var newData = new DataSet();
                    var dbDonn =
                        $"Data Source = {_dbAddress}; Initial Catalog = {_dbName}; User ID = {_dbAcc}; Password = {_dbPwd};PORT= 33060 ;Character Set=utf8;Allow User Variables=True";
                    var Localhost =
                        $"Data Source =localhost; Initial Catalog = {_testDBName}; User ID = {_testDbAcc}; Password = {_testDbPwd};Character Set=utf8; Allow User Variables=True";
                    foreach (var item in tableList)
                    {
                        var sql = string.Empty;
                        if (isByTime)
                        {
                            var timeList = Tool.Helper.AcquisitionByTimePeriodTimeList(Convert.ToDateTime(begin.Text),
                                Convert.ToDateTime(end.Text));
                            for (int i = 0; i < timeList.Count; i++)
                            {
                                var where = " where 1=1 ";
                                if (begin != null)
                                {
                                    where +=
                                        $" and create_time >='{Convert.ToDateTime(Convert.ToDateTime(timeList[i]).ToString("yyyy-MM-dd 00:00:00"))}'";
                                }
                                if (end != null)
                                {
                                    where +=
                                        $" and create_time <='{Convert.ToDateTime(Convert.ToDateTime(timeList[i]).ToString("yyyy-MM-dd 23:59:59"))}'";
                                }
                                sql = $"select * from {item} " + where;
                                newData = DAL.DbHelper.MySqlQueryBySqlstring(sql, dbDonn);
                                if (newData.Tables.Count <= 0 || newData.Tables[0].Rows.Count == 0)
                                {
                                    continue;
                                }
                                sql = $"DELETE from {item} " + where;

                                //Data Source = localhost; Initial Catalog = cccc; User ID = root; Password = yanghao1996; Character Set = utf8; Allow User Variables = True
                                var del = DAL.DbHelper.MysqlExecuteSql(sql, Localhost);
                                HandleInfo($"已删除 {item} {timeList[i]} 数据，共删除 {del}条...");

                                var insertStr = string.Empty;
                                insertStr +=
                                    $@"INSERT INTO  {item}(id, qishu, create_time,one,two,three,sum,daxiao,danshuang,teshu,zuhe,jizhi,count_down,count_down2)VALUES";
                                for (int j = 0; j < newData.Tables[0].Rows.Count; j++)
                                {
                                    var row = newData.Tables[0].Rows[j];
                                    insertStr +=
                                        $"({row["id"]}, {row["qishu"]},'{row["create_time"]}',{row["one"]},{row["two"]},{row["three"]},{row["sum"]},'{row["daxiao"]}','{row["danshuang"]}','{row["teshu"]}','{row["zuhe"]}','{row["jizhi"]}',{(!string.IsNullOrWhiteSpace(row["count_down"].ToString()) ? string.Format("'{0}'", row["count_down"]) : "null")},{(!string.IsNullOrWhiteSpace(row["count_down2"].ToString()) ? string.Format("'{0}'", row["count_down2"]) : "null")}),\n";
                                }
                                insertStr = insertStr.TrimEnd().TrimEnd(',');
                                HandleInfo($"正往本地 {item} 表更新中...");
                                var result = DAL.DbHelper.MysqlExecuteSql(insertStr, Localhost);
                                HandleInfo($"更新完成，共{result}条...");
                            }
                        }
                        else
                        {
                            sql = $"select * from {item}  ORDER BY create_time DESC  LIMIT 0,1";
                            HandleInfo($"正在查询本地{item}最新期号...");
                            var ds = DAL.DbHelper.MySqlQueryBySqlstring(sql, Localhost);
                            var newSql = string.Empty;
                            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                            {
                                //newSql = $"select * from {item} where number >{ds.Tables[0].Rows[0][0]}";
                                HandleInfo($"本地 {item} 没有数据，请先补全数据...");
                                Invoke(new MethodInvoker(delegate { timer1.Start(); }));
                                return;
                                //var oldestTime = Convert.ToDateTime(DAL.DbHelper.MySqlQueryBySqlstring($" select* from {item} ORDER BY time asc LIMIT 0,1", dbDonn).Tables[0].Rows[0]["time"]);
                                //var timeList = Tool.Helper.AcquisitionByTimePeriodTimeList(oldestTime, DateTime.Now);
                            }


                            var timeList =
                                Tool.Helper.AcquisitionByTimePeriodTimeList(
                                    Convert.ToDateTime(ds.Tables[0].Rows[0]["create_time"]), DateTime.Now);
                            for (int i = 0; i < timeList.Count; i++)
                            {
                                if (i == 0)
                                {
                                    newData =
                                        DAL.DbHelper.MySqlQueryBySqlstring(
                                            $"select * from {item} where create_time >'{Convert.ToDateTime(ds.Tables[0].Rows[0]["create_time"]).AddSeconds(1)}' and create_time <= '{timeList[i].ToString("yyyy-MM-dd 23:59:59")}'",
                                            dbDonn);
                                    if (newData.Tables.Count == 0)
                                    {
                                        HandleInfo($"没有获取到服务器 {item} 的数据");
                                        continue;
                                    }
                                    if (newData.Tables[0].Rows.Count == 0)
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    newData =
                                        DAL.DbHelper.MySqlQueryBySqlstring(
                                            $"select * from {item} where create_time >='{timeList[i].ToString("yyyy-MM-dd 00:00:00")}' and create_time <= '{timeList[i].ToString("yyyy-MM-dd 23:59:59")}'",
                                            dbDonn);
                                }
                                if (newData.Tables[0].Rows.Count == 0)
                                {
                                    continue;
                                }

                                var insertStr = string.Empty;
                                insertStr +=
                                    $@"INSERT INTO  {item}(id, qishu, create_time,one,two,three,sum,daxiao,danshuang,teshu,zuhe,jizhi,count_down,count_down2)VALUES";
                                for (int j = 0; j < newData.Tables[0].Rows.Count; j++)
                                {
                                    var row = newData.Tables[0].Rows[j];

                                    object count_down = DBNull.Value;
                                    object count_down2 = DBNull.Value;

                                    if (!string.IsNullOrWhiteSpace(row["count_down"].ToString()))
                                    {
                                        count_down = row["count_down"];
                                    }
                                    if (!string.IsNullOrWhiteSpace(row["count_down2"].ToString()))
                                    {
                                        count_down = row["count_down2"];
                                    }

                                    insertStr +=
                                        $"({row["id"]}, {row["qishu"]},'{row["create_time"]}',{row["one"]},{row["two"]},{row["three"]},{row["sum"]},'{row["daxiao"]}','{row["danshuang"]}','{row["teshu"]}','{row["zuhe"]}','{row["jizhi"]}',{(!string.IsNullOrWhiteSpace(row["count_down"].ToString()) ? string.Format("'{0}'", row["count_down"]) : "null")},{(!string.IsNullOrWhiteSpace(row["count_down2"].ToString()) ? string.Format("'{0}'", row["count_down2"]) : "null")}),\n";
                                }
                                insertStr = insertStr.TrimEnd().TrimEnd(',');
                                HandleInfo($"正往本地 {item} 表更新中...");
                                var result = DAL.DbHelper.MysqlExecuteSql(insertStr, Localhost);
                                HandleInfo($"更新完成，共{result}条...");

                            }
                        }
                    }
                    if (!isByTime && update.Text == "开始更新" ? false : true)
                    {
                        Invoke(new MethodInvoker(delegate { timer1.Start(); }));
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            });
        }

        private void update_Click(object sender, EventArgs e)
        {
            if (!Verification())
            {
                return;
            }
            var startOrStop = update.Text == "开始更新" ? true : false;
            if (!StartTimer(startOrStop))
            {
                MessageBox.Show("定时器启动失败");
                return;
            }
            if (startOrStop)
            {
                update.Text = "暂停更新";
            }
            else
            {
                update.Text = "开始更新";
            }

            DisableOrEnable(!startOrStop);
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
                        textBox2.Text += @"   " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + content +
                                         "\r\n\r\n";
                    }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dbAdd = dbAddress.Text;
            var name = dbName.Text;
            var acc = dbAcc.Text;
            var pwd = dbPwd.Text;
            var updateIn = updateInterval.Text;
            var list = tableList.Text;
            var testAcc = testDbAcc.Text;
            var testPwd = testDbPwd.Text;
            var testDBName = this.testDBName.Text;

            if (!Verification())
            {
                return;
            }
            var content =
                $@"dbAddress:{dbAdd}
dbName:{name}
dbAcc:{acc}
dbPwd:{pwd}
updateInterval:{updateIn}
tableList:{list}
testDbAcc:{testAcc}
testDbPwd:{testPwd}
testDBName:{testDBName}";

            var path = Tool.Helper.ReadConfiguration(Tool.ConfigurationType.LotterySystemPath);
            var result = Tool.Helper.WriteFile(path, "ConfigurationFile.ini", content);
            if (result)
            {
                _dbAddress = dbAdd;
                _dbName = name;
                _dbAcc = acc;
                _dbPwd = pwd;
                _updateInterval = Convert.ToInt32(updateIn);
                _tableList = list;
                _testDbAcc = testAcc;
                _testDbPwd = testPwd;
                _testDBName = testDBName;
                MessageBox.Show("保存成功");
                return;
            }
            MessageBox.Show("保存失败");
            GetconfigurationFile();
        }

        /// <summary>
        /// 获取配置文件
        /// </summary>
        public void GetconfigurationFile()
        {
            var path = Tool.Helper.ReadConfiguration(Tool.ConfigurationType.LotterySystemPath) +
                       "\\ConfigurationFile.ini";
            var content = Tool.Helper.ReadTheLocalFile(path).Split('\n').ToList();
            if (string.IsNullOrWhiteSpace(content[0]))
            {
                return;
            }
            dbAddress.Text = content[0].Split(':')[1];
            dbName.Text = content[1].Split(':')[1];
            dbAcc.Text = content[2].Split(':')[1];
            dbPwd.Text = content[3].Split(':')[1];
            updateInterval.Text = content[4].Split(':')[1];
            tableList.Text = content[5].Split(':')[1];
            testDbAcc.Text = content[6].Split(':')[1];
            testDbPwd.Text = content[7].Split(':')[1];
            testDBName.Text = content[8].Split(':')[1];

            _dbAddress = content[0].Split(':')[1];
            _dbName = content[1].Split(':')[1];
            _dbAcc = content[2].Split(':')[1];
            _dbPwd = content[3].Split(':')[1];
            _updateInterval = Convert.ToInt32(content[4].Split(':')[1]);
            _tableList = content[5].Split(':')[1];
            _testDbAcc = content[6].Split(':')[1];
            _testDbPwd = content[7].Split(':')[1];
            _testDBName = content[8].Split(':')[1];
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox2.SelectionStart = textBox2.Text.Length;
            textBox2.SelectionLength = 0;
            textBox2.ScrollToCaret();
        }

        /// <summary>
        /// 启动定时器
        /// </summary>
        public bool StartTimer(bool startOrStop)
        {
            if (_updateInterval == 0)
            {
                return false;
            }

            timer1.Interval = _updateInterval * 1000;
            if (startOrStop)
            {
                timer1.Start();
                timer1.Enabled = true;
                HandleInfo($"程序将在{_updateInterval}秒后执行...");
            }
            else
            {
                timer1.Stop();
                timer1.Enabled = false;
                HandleInfo($"程序已停止...");
            }
            return true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            GetTheLatestData();
            ClearContent();
            ClearMemory();
        }

        private void updateBytime_Click(object sender, EventArgs e)
        {
            var begin = Convert.ToDateTime(this.begin.Text);
            var end = Convert.ToDateTime(this.end.Text);
            DisableOrEnable(false);
            update.Enabled = false;
            GetTheLatestData(true);
            DisableOrEnable(true);
            update.Enabled = true;
        }

        public void DisableOrEnable(bool isDisable)
        {
            dbAddress.Enabled = isDisable;
            dbName.Enabled = isDisable;
            dbAcc.Enabled = isDisable;
            dbPwd.Enabled = isDisable;
            updateInterval.Enabled = isDisable;
            tableList.Enabled = isDisable;
            testDbAcc.Enabled = isDisable;
            testDbPwd.Enabled = isDisable;
            begin.Enabled = isDisable;
            end.Enabled = isDisable;
            button1.Enabled = isDisable;
            updateBytime.Enabled = isDisable;
            button2.Enabled = isDisable;
            button3.Enabled = isDisable;
            comboBox1.Enabled = isDisable;
            testDBName.Enabled = isDisable;
        }

        /// <summary>
        /// 非空验证
        /// </summary>
        /// <returns></returns>
        public bool Verification()
        {
            var dbAdd = dbAddress.Text;
            var name = dbName.Text;
            var acc = dbAcc.Text;
            var pwd = dbPwd.Text;
            var updateIn = updateInterval.Text;
            var list = tableList.Text;
            var testAcc = testDbAcc.Text;
            var testPwd = testDbPwd.Text;
            var testDBName = this.testDBName.Text;

            if (string.IsNullOrWhiteSpace(dbAdd))
            {
                MessageBox.Show("服务器地址不能为空");
                return false;
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("数据库名称不能为空");
                return false;
            }
            if (string.IsNullOrWhiteSpace(acc))
            {
                MessageBox.Show("数据库账号不能为空");
                return false;
            }
            if (string.IsNullOrWhiteSpace(pwd))
            {
                MessageBox.Show("数据库密码不能为空");
                return false;
            }
            if (string.IsNullOrWhiteSpace(testAcc))
            {
                MessageBox.Show("测试库账号不能为空");
                return false;
            }
            if (string.IsNullOrWhiteSpace(testPwd))
            {
                MessageBox.Show("测试库密码不能为空");
                return false;
            }
            if (string.IsNullOrWhiteSpace(updateIn))
            {
                MessageBox.Show("更新间隔时间不能为空");
                return false;
            }
            if (string.IsNullOrWhiteSpace(list))
            {
                MessageBox.Show("更新的表集合不能为空");
                return false;
            }
            if (string.IsNullOrWhiteSpace(testDBName))
            {
                MessageBox.Show("本地数据库名称不能为空");
                return false;
            }
            if (Convert.ToDecimal(updateIn) < 1)
            {
                MessageBox.Show("时间间隔不能小于1");
                return false;
            }
            return true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 注意判断关闭事件reason来源于窗体按钮，否则用菜单退出时无法退出!
            if (e.CloseReason == CloseReason.UserClosing)
            {
                //取消"关闭窗口"事件
                e.Cancel = true; // 取消关闭窗体 

                //使关闭时窗口向右下角缩小的效果
                this.WindowState = FormWindowState.Minimized;
                this.notifyIcon1.Visible = true;
                this.Hide();
                return;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.Visible)
            {
                this.WindowState = FormWindowState.Minimized;
                this.notifyIcon1.Visible = true;
                this.Hide();
            }
            else
            {
                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (
                MessageBox.Show("你确定要退出？", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {

                this.notifyIcon1.Visible = false;
                //托盘区图标隐藏
                notifyIcon1.Visible = false;
                this.Close();
                this.Dispose();
                System.Environment.Exit(System.Environment.ExitCode);

            }
        }

        /// <summary>
        /// 清楚文本框里的内容
        /// </summary>
        public void ClearContent()
        {
            var list = textBox2.Text.Split('\n').ToList();

            if (list.Count() > 500)
            {
                textBox2.Text = "";
            }
        }

        #region 内存回收

        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);

        /// <summary>
        /// 释放内存
        /// </summary>
        public static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Form1.SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }
        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            var dbDonn = $"Data Source = {_dbAddress}; Initial Catalog = {_dbName}; User ID = {_dbAcc}; Password = {_dbPwd}; PORT= 33060 ;Character Set=utf8;";
            var Localhost = $"Data Source =localhost; Initial Catalog =  {_testDBName}; User ID = {_testDbAcc}; Password = {_testDbPwd};Character Set=utf8; Allow User Variables=True";

            var str = string.Empty;

            foreach (var item in _tableList.Split(','))
            {
                var sql = $"select qishu from {item} ORDER BY create_time desc LIMIT 0,1;select COUNT(1) from {item};";
                var server = DAL.DbHelper.MySqlQueryBySqlstring(sql, dbDonn);
                var localhost = DAL.DbHelper.MySqlQueryBySqlstring(sql, Localhost);
                if (server.Tables.Count == 0 || server.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show($"服务器 {item} 表数据为空");
                }
                else
                {
                    str += $"服务器 {item} 最新期号：{server.Tables[0].Rows[0][0]}" + "\n";
                }
                if (localhost.Tables.Count == 0 || localhost.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show($"本地 {item} 表数据为空");
                }
                else
                {
                    str += $"本地 {item} 最新期号：{localhost.Tables[0].Rows[0][0]}" + "\n";
                }

                if (server.Tables.Count == 0 || server.Tables[1].Rows.Count == 0)
                {
                    MessageBox.Show($"服务器 {item} 表数据为空");
                }
                else
                {
                    str += $"服务器 {item} 总条数：{server.Tables[1].Rows[0][0]}" + "\n";
                }
                if (localhost.Tables.Count == 0 || localhost.Tables[1].Rows.Count == 0)
                {
                    MessageBox.Show($"本地 {item} 表数据为空");
                }
                else
                {
                    str += $"本地 {item} 总条数：{localhost.Tables[1].Rows[0][0]}" + "\n";
                }
            }
            MessageBox.Show(str);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DisableOrEnable(false);
            update.Enabled = false;
            var begin = Convert.ToDateTime(this.begin.Text);
            var end = Convert.ToDateTime(this.end.Text);
            var table = string.Empty;
            switch (comboBox1.Text)
            {
                case "加拿大": table = "canada28"; break;
                case "北京": table = "beijing28"; break;
            }
            Task.Run(() =>
            {
                HandleInfo("正在查询截断期号信息...");
                var localhost = $"Data Source =localhost; Initial Catalog = {_testDBName}; User ID = {_testDbAcc}; Password = {_testDbPwd};Character Set=utf8; Allow User Variables=True";
                var sql = $"select create_time,qishu from {table} where create_time >= '{begin.ToString("yyyy-MM-dd 00:00:00")}' and create_time <= '{end.ToString("yyyy-MM-dd 23:59:59")}' ORDER BY qishu asc";
                var dt = DAL.DbHelper.MySqlQueryBySqlstring(sql, localhost);
                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    if (i == dt.Tables[0].Rows.Count - 1)
                    {
                        continue;
                    }
                    var row = dt.Tables[0].Rows[i];
                    var num = Convert.ToInt32(dt.Tables[0].Rows[i]["qishu"]) + 1;
                    var nextNum = Convert.ToInt32(dt.Tables[0].Rows[i + 1]["qishu"]);
                    if (num != nextNum)
                    {
                        HandleInfo($" '{row["qishu"] }' ，该期号截断，开奖时间为：{row["create_time"]}");
                    }
                }
                Invoke(new MethodInvoker(delegate { HandleInfo("截断期号信息查询结束..."); DisableOrEnable(true); update.Enabled = true; }));
            });
        }

        private void comboBox1_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(comboBox1, "该下拉框是针对查询截断期号功能使用的");//ttMsg为ToolTip控件,txtLoginName为文本框
        }
    }
}
