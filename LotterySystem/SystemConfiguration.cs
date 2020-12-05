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
    public partial class SystemConfiguration : Form
    {
        public SystemConfiguration()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 保存系统配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (!Verification())
            {
                return;
            }

            var dbAdd = dbAddress.Text;
            var name = dbName.Text;
            var acc = dbAcc.Text;
            var pwd = dbPwd.Text;
            var testAcc = testDbAcc.Text;
            var testPwd = testDbPwd.Text;
            var DefaultNumber = this.DefaultNumber.Text;
            var beijingTime = this.beijingTime.Text;
            var jianadaTime = this.jianadaTime.Text;
            var beijingBeginDormancyTime = this.beijingBeginDormancyTime.Text;
            var beijingEndDormancyTime = this.beijingEndDormancyTime.Text;
            var jianadaBeginDormancyTime = this.jianadaBeginDormancyTime.Text;
            var jianadaEndDormancyTime = this.jianadaEndDormancyTime.Text;
            var ProgramPath = this.ProgramPath.Text;
            var LotterySystemPath = this.LotterySystemPath.Text;
            var content = $@"dbAddress:{dbAdd}
dbName:{name}
dbAcc:{acc}
dbPwd:{pwd}
testDbAcc:{testAcc}
testDbPwd:{testPwd}
DefaultNumber:{DefaultNumber}
beijingTime:{beijingTime}
jianadaTime:{jianadaTime}
beijingBeginDormancyTime:{beijingBeginDormancyTime}
beijingEndDormancyTime:{beijingEndDormancyTime}
jianadaBeginDormancyTime:{jianadaBeginDormancyTime}
jianadaEndDormancyTime:{jianadaEndDormancyTime}
ProgramPath:{ProgramPath}
LotterySystemPath:{LotterySystemPath}";

            var path = Environment.CurrentDirectory;
            var result = Tool.Helper.WriteFile(path, "LotteryConfigurationFile.ini", content);
            result = Tool.Helper.WriteFile(Tool.Helper.ReadConfiguration(Tool.ConfigurationType.ProgramPath), "LotteryConfigurationFile.ini", content);
            if (result)
            {
                MessageBox.Show("保存成功");
            }
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
            var testAcc = testDbAcc.Text;
            var testPwd = testDbPwd.Text;
            var DefaultNumber = this.DefaultNumber.Text;
            var beijingTime = this.beijingTime.Text;
            var jianadaTime = this.jianadaTime.Text;
            var beijingBeginDormancyTime = this.beijingBeginDormancyTime.Text;
            var beijingEndDormancyTime = this.beijingEndDormancyTime.Text;
            var jianadaBeginDormancyTime = this.jianadaBeginDormancyTime.Text;
            var jianadaEndDormancyTime = this.jianadaEndDormancyTime.Text;
            var ProgramPath = this.ProgramPath.Text;
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
            if (string.IsNullOrWhiteSpace(DefaultNumber))
            {
                MessageBox.Show("列表默认查询条数不能为空");
                return false;
            }
            if (string.IsNullOrWhiteSpace(beijingTime))
            {
                MessageBox.Show("北京获取数据的时间间隔不能为空");
                return false;
            }
            if (string.IsNullOrWhiteSpace(jianadaTime))
            {
                MessageBox.Show("加拿大获取数据的时间间隔不能空");
                return false;
            }
            if (string.IsNullOrWhiteSpace(beijingBeginDormancyTime))
            {
                MessageBox.Show("北京休眠开始时间不能为空");
                return false;
            }
            if (string.IsNullOrWhiteSpace(beijingEndDormancyTime))
            {
                MessageBox.Show("北京休眠分钟数不能为空");
                return false;
            }
            if (string.IsNullOrWhiteSpace(jianadaBeginDormancyTime))
            {
                MessageBox.Show("加拿大休眠开始时间不能为空");
                return false;
            }
            if (string.IsNullOrWhiteSpace(jianadaEndDormancyTime))
            {
                MessageBox.Show("加拿大休眠结束时间不能为空");
                return false;
            }
            if (string.IsNullOrWhiteSpace(ProgramPath))
            {
                MessageBox.Show("跑数据程序路径不能为空");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 读取系统设置
        /// </summary>
        public void GetConfig()
        {

            //var path = Tool.Helper.ReadConfiguration(Tool.ConfigurationType.LotterySystemPath) + "//LotteryConfigurationFile.ini";
            var path = Environment.CurrentDirectory + "//LotteryConfigurationFile.ini";
            var list = Tool.Helper.ReadTheLocalFile(path);

            if (string.IsNullOrWhiteSpace(list))
            {
                return;
            }

            dbAddress.Text = list.Split('\n')[0].Split(':')[1];
            dbName.Text = list.Split('\n')[1].Split(':')[1];
            dbAcc.Text = list.Split('\n')[2].Split(':')[1];
            dbPwd.Text = list.Split('\n')[3].Split(':')[1];
            testDbAcc.Text = list.Split('\n')[4].Split(':')[1];
            testDbPwd.Text = list.Split('\n')[5].Split(':')[1];
            DefaultNumber.Text = list.Split('\n')[6].Split(':')[1];
            beijingTime.Text = list.Split('\n')[7].Split(':')[1];
            jianadaTime.Text = list.Split('\n')[8].Split(':')[1];
            beijingBeginDormancyTime.Text = list.Split('\n')[9].Split(':')[1] + ":" + list.Split('\n')[9].Split(':')[2];
            beijingEndDormancyTime.Text = list.Split('\n')[10].Split(':')[1];
            jianadaBeginDormancyTime.Text = list.Split('\n')[11].Split(':')[1] + ":" + list.Split('\n')[11].Split(':')[2];
            jianadaEndDormancyTime.Text = list.Split('\n')[12].Split(':')[1] + ":" + list.Split('\n')[12].Split(':')[2];
            ProgramPath.Text = Tool.Helper.ReadConfiguration(Tool.ConfigurationType.ProgramPath);
            LotterySystemPath.Text = Tool.Helper.ReadConfiguration(Tool.ConfigurationType.LotterySystemPath);
            if (!Verification())
            {
                return;
            }
        }

        private void SystemConfiguration_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1.from4 = null;
        }

        private void SystemConfiguration_Shown(object sender, EventArgs e)
        {
            GetConfig();
        }
    }
}
