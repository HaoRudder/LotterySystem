using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using static System.Int32;

namespace Tool
{
    public static class Helper
    {
        /// <summary>
        /// 读取本地文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static string ReadTheLocalFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    var sr = new StreamReader(path, Encoding.UTF8);
                    string line;
                    var str = string.Empty;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            str += line + "\n";
                        }
                    }
                    sr.Close();
                    return str;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="path">需要写入文件的地址</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="content">文件内容</param>
        /// <param name="isAppend">是否追加</param>
        public static bool WriteFile(string path, string fileName, string content, bool isAppend = false)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var sw = new StreamWriter(path + "\\" + fileName, isAppend, Encoding.UTF8);
                sw.WriteLine(content);
                sw.Close();//写入
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 从字符串中提取数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetNumber(string str)
        {
            return System.Text.RegularExpressions.Regex.Replace(str, @"[^0-9]+", "");
        }

        /// <summary>
        /// 汉字转全拼
        /// </summary>
        /// <param name="strChinese"></param>
        /// <param name="isBigOrSmall">大写或者小写，默认小写</param>
        /// <returns></returns>
        public static string ConvertToAllSpell(string strChinese, bool isBigOrSmall = false)
        {
            try
            {
                if (strChinese.Length != 0)
                {
                    StringBuilder fullSpell = new StringBuilder();
                    for (int i = 0; i < strChinese.Length; i++)
                    {
                        var chr = strChinese[i];
                        fullSpell.Append(GetSpell(chr));
                    }

                    var str = string.Empty;
                    if (isBigOrSmall)
                    {
                        str = fullSpell.ToString().ToUpper();
                    }

                    return fullSpell.ToString();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("全拼转化出错！" + e.Message);
            }

            return string.Empty;
        }

        /// <summary>
        /// 汉字转首字母
        /// </summary>
        /// <param name="strChinese"></param>
        /// <param name="isBigOrSmall">大写或者小写，默认小写</param>
        /// <returns></returns>
        public static string GetFirstSpell(string strChinese, bool isBigOrSmall = false)
        {
            //NPinyin.Pinyin.GetInitials(strChinese)  有Bug  洺无法识别
            //return NPinyin.Pinyin.GetInitials(strChinese);

            try
            {
                if (strChinese.Length != 0)
                {
                    StringBuilder fullSpell = new StringBuilder();
                    for (int i = 0; i < strChinese.Length; i++)
                    {
                        var chr = strChinese[i];
                        fullSpell.Append(GetSpell(chr)[0]);
                    }

                    var str = string.Empty;
                    if (isBigOrSmall)
                    {
                        str = fullSpell.ToString().ToUpper();
                    }

                    return fullSpell.ToString();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("首字母转化出错！" + e.Message);
            }

            return string.Empty;
        }

        private static string GetSpell(char chr)
        {
            var coverchr = NPinyin.Pinyin.GetPinyin(chr);
            return coverchr;

        }

        /// <summary>
        /// 判断字符串是否可以转化为数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumberic(string str)
        {
            double vsNum;
            return double.TryParse(str, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out vsNum); ;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path"></param>
        public static bool OpenFile(string path)
        {
            try
            {
                Process.Start(path);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 根据配置类型获取配置信息
        /// </summary>
        /// <returns></returns>
        public static string ReadConfiguration(ConfigurationType type)
        {
            var path = Environment.CurrentDirectory + "//LotteryConfigurationFile.ini";
            var list = ReadTheLocalFile(path);

            if (string.IsNullOrWhiteSpace(list))
            {
                return string.Empty;
            }

            try
            {
                switch (type)
                {
                    case ConfigurationType.dbAdd:
                        return list.Split('\n')[0].Split(':')[1];
                    case ConfigurationType.dbName:
                        return list.Split('\n')[1].Split(':')[1];
                    case ConfigurationType.acc:
                        return list.Split('\n')[2].Split(':')[1];
                    case ConfigurationType.pwd:
                        return list.Split('\n')[3].Split(':')[1];
                    case ConfigurationType.testAcc:
                        return list.Split('\n')[4].Split(':')[1];
                    case ConfigurationType.testPwd:
                        return list.Split('\n')[5].Split(':')[1];
                    case ConfigurationType.DefaultNumber:
                        return list.Split('\n')[6].Split(':')[1];
                    case ConfigurationType.beijingTime:
                        return list.Split('\n')[7].Split(':')[1];
                    case ConfigurationType.jianadaTime:
                        return list.Split('\n')[8].Split(':')[1];
                    case ConfigurationType.beijingBeginDormancyTime:
                        return list.Split('\n')[9].Split(':')[1] + ":" + list.Split('\n')[9].Split(':')[2];
                    case ConfigurationType.beijingEndDormancyTime:
                        return list.Split('\n')[10].Split(':')[1];
                    case ConfigurationType.jianadaBeginDormancyTime:
                        return list.Split('\n')[11].Split(':')[1] + ":" + list.Split('\n')[11].Split(':')[2];
                    case ConfigurationType.jianadaEndDormancyTime:
                        return list.Split('\n')[12].Split(':')[1] + ":" + list.Split('\n')[12].Split(':')[2];
                    case ConfigurationType.AutomaticExportEntities:
                        return $"Data Source = {list.Split('\n')[0].Split(':')[1]}; Initial Catalog = {list.Split('\n')[1].Split(':')[1]}; User ID = { list.Split('\n')[2].Split(':')[1]}; Password = { list.Split('\n')[3].Split(':')[1]};Character Set=utf8;";
                    case ConfigurationType.Localhost:
                        return $"Data Source =localhost; Initial Catalog = cccc; User ID = { list.Split('\n')[4].Split(':')[1]}; Password = { list.Split('\n')[5].Split(':')[1]};Character Set=utf8; Allow User Variables=True";
                    case ConfigurationType.ProgramPath:
                        return list.Split('\n')[13].Substring(list.Split('\n')[13].LastIndexOf(':') - 1, list.Split('\n')[13].Length - list.Split('\n')[13].LastIndexOf(':') + 1);
                    case ConfigurationType.LotterySystemPath:
                        return list.Split('\n')[14].Substring(list.Split('\n')[14].LastIndexOf(':') - 1, list.Split('\n')[14].Length - list.Split('\n')[14].LastIndexOf(':') + 1);
                    default:
                        return string.Empty;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 根据控件名称获取提醒配置信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetReminderConfigurationByName(string name)
        {
            var path = Environment.CurrentDirectory + "//ReminderConfiguration.ini";
            var list = ReadTheLocalFile(path).Trim().Split('\n');
            var str = list.FirstOrDefault(x => x.Split(':')[0] == name);
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }
            return str.Split(':')[1];
        }

        /// <summary>
        /// 判断配置文件是否为空
        /// </summary>
        /// <returns></returns>
        public static bool JudgementConfigurationIsNull()
        {
            var path = Tool.Helper.ReadConfiguration(Tool.ConfigurationType.LotterySystemPath) + "//LotteryConfigurationFile.ini";
            var list = ReadTheLocalFile(path);

            if (string.IsNullOrWhiteSpace(list))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 根据时间段获取时间集合
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static List<DateTime> AcquisitionByTimePeriodTimeList(DateTime begin, DateTime end)
        {
            List<DateTime> list = new List<DateTime>();
            if (begin.ToString("yyyy-MM-dd") == end.ToString("yyyy-MM-dd"))
            {
                return new List<DateTime> { begin };
            }
            for (var time = begin; time <= end;)
            {
                list.Add(time);
                time = time.AddDays(1);
                time = Convert.ToDateTime(time.ToString("yyyy-MM-dd 00:00:00"));
            }
            return list;
        }

        /// <summary>
        /// 获取赔率
        /// </summary>
        /// <param name="oddsName">赔率名称</param>
        /// <param name="typeName">类型名称</param>
        public static string ReadOddsSettings(string oddsName = "", string typeName = "", int type = 0)
        {
            var path = string.Empty;
            if (type == 1)
            {
                path = Environment.CurrentDirectory + "//OddsSettingManualSimulation.ini";
            }
            else
            {
                path = Environment.CurrentDirectory + "//OddsSetting.ini";
            }

            var list = ReadTheLocalFile(path).Trim();
            if (string.IsNullOrWhiteSpace(oddsName))
            {
                return list;
            }
            var str = list.Split('\n').FirstOrDefault(x => x.Contains(oddsName));
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }
            if (!string.IsNullOrWhiteSpace(typeName))
            {
                if (typeName == "jixiao" || typeName == "jida")
                {
                    typeName = "jishu";
                }
                var temp = str.Replace(oddsName + "-", "").Split(',').ToList().FirstOrDefault(x => x.Split(':')[0] == typeName).Split(':')[1];
                return string.IsNullOrWhiteSpace(temp) ? "1" : temp;
            }
            return str;
        }

        /// <summary>
        /// 获取模拟设置
        /// </summary>
        /// <param name="oddsName">赔率名称</param>
        /// <param name="typeName">类型名称</param>
        public static Dictionary<string, string> ReadSimulationSettings()
        {
            var path = Environment.CurrentDirectory + "//SimulationSettings.ini";

            var list = ReadTheLocalFile(path).Trim();
            var strList = list.Split(',');
            var dic = strList.ToDictionary(item => item.Split(':')[0], item => item.Split(':')[1]);
            return dic;
        }

        /// <summary>
        /// 读取规则
        /// </summary>
        /// <returns></returns>
        public static string ReadRuleList()
        {
            var path = Environment.CurrentDirectory + "//RuleContent.ini";
            var list = ReadTheLocalFile(path).Trim();
            if (string.IsNullOrWhiteSpace(list))
            {
                return string.Empty;
            }
            return list;
        }

        /// <summary>
        /// 杀死进程
        /// </summary>
        /// <param name="processNameList">进程名称集合</param>
        public static void KillProcess(List<string> processNameList)
        {
            foreach (var item in processNameList)
            {
                var process = Process.GetProcessesByName(item);
                foreach (var p in process)
                {
                    if (!p.HasExited) // 如果程序没有关闭，结束程序
                    {
                        p.Kill();
                        p.WaitForExit();
                    }
                }
            }
        }

        /// <summary>  
        /// 获取Json的Model  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="json"></param>  
        /// <returns></returns>  
        public static T DeserializeObject<T>(string json)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            T obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }

        /// <summary>
        /// Json 字符串 转换为 DataTable数据集合
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static DataTable JsonToDataTable(this string strJson)
        {
            //转换json格式
            strJson = strJson.Replace(",\"", "*\"").Replace("\":", "\"#").ToString();
            //取出表名 
            var rg = new Regex(@"(?<={)[^:]+(?=:\[)", RegexOptions.IgnoreCase);
            string strName = rg.Match(strJson).Value;
            DataTable tb = null;
            //去除表名 
            strJson = strJson.Substring(strJson.IndexOf("[") + 1);
            strJson = strJson.Substring(0, strJson.IndexOf("]"));

            //获取数据 
            rg = new Regex(@"(?<={)[^}]+(?=})");
            MatchCollection mc = rg.Matches(strJson);
            for (int i = 0; i < mc.Count; i++)
            {
                string strRow = mc[i].Value;
                string[] strRows = strRow.Split('*');

                //创建表 
                if (tb == null)
                {
                    tb = new DataTable();
                    tb.TableName = strName;
                    foreach (string str in strRows)
                    {
                        var dc = new DataColumn();
                        string[] strCell = str.Split('#');

                        if (strCell[0].Substring(0, 1) == "\"")
                        {
                            int a = strCell[0].Length;
                            dc.ColumnName = strCell[0].Substring(1, a - 2);
                        }
                        else
                        {
                            dc.ColumnName = strCell[0];
                        }
                        tb.Columns.Add(dc);
                    }
                    tb.AcceptChanges();
                }

                //增加内容 
                DataRow dr = tb.NewRow();
                for (int r = 0; r < strRows.Length; r++)
                {
                    dr[r] = strRows[r].Split('#')[1].Trim().Replace("，", ",").Replace("：", ":").Replace("\"", "");
                }
                tb.Rows.Add(dr);
                tb.AcceptChanges();
            }

            return tb;
        }

        /// <summary>
        /// 修改AppSettings中配置
        /// </summary>
        /// <param name="key">key值</param>
        /// <param name="value">相应值</param>
        public static bool SetConfigValue(string key, string value)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings[key] != null)
                    config.AppSettings.Settings[key].Value = value;
                else
                    config.AppSettings.Settings.Add(key, value);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 根据主键名称获取自动投注配置信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAutomaticBettingConfigureByName(string key)
        {
            var str = ReadTheLocalFile(Environment.CurrentDirectory + "/AutomaticBettingConfigure.ini").Trim();

            if (key == "BeginNumber")
            {
                return str.Split(',')[0].Split(':')[1];
            }
            if (key == "IsTrueBet")
            {
                return str.Split(',')[1].Split(':')[1];
            }
            return string.Empty;
        }
    }

    public enum ConfigurationType
    {
        /// <summary>
        /// 正式服务器地址
        /// </summary>
        dbAdd = 1,

        /// <summary>
        /// 正式数据库名称
        /// </summary>
        dbName = 2,

        /// <summary>
        /// 正式数据库账号
        /// </summary>
        acc = 3,

        /// <summary>
        /// 正式数据库密码
        /// </summary>
        pwd = 4,

        /// <summary>
        /// 测试库账号
        /// </summary>
        testAcc = 5,

        /// <summary>
        /// 测试库密码
        /// </summary>
        testPwd = 6,

        /// <summary>
        /// 列表查询的默认条数
        /// </summary>
        DefaultNumber = 7,

        /// <summary>
        /// 北京获取数据的时间间隔
        /// </summary>
        beijingTime = 8,

        /// <summary>
        /// 加拿大获取数据的时间间隔
        /// </summary>
        jianadaTime = 9,

        /// <summary>
        /// 北京休眠开始时间
        /// </summary>
        beijingBeginDormancyTime = 10,

        /// <summary>
        /// 北京休眠分钟数
        /// </summary>
        beijingEndDormancyTime = 11,

        /// <summary>
        /// 加拿大休眠开始时间
        /// </summary>
        jianadaBeginDormancyTime = 12,

        /// <summary>
        /// 加拿大休眠结束时间
        /// </summary>
        jianadaEndDormancyTime = 13,

        /// <summary>
        /// 正式服务器的连接字符串
        /// </summary>
        AutomaticExportEntities = 14,

        /// <summary>
        /// 测试服务器的连接字符串
        /// </summary>
        Localhost = 15,

        /// <summary>
        /// 跑数据程序的路径
        /// </summary>
        ProgramPath = 16,

        /// <summary>
        /// 系统路径
        /// </summary>
        LotterySystemPath = 17
    }
}
