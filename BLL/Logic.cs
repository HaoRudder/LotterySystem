using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class Logic
    {
        public static string connString = Tool.Helper.ReadConfiguration(Tool.ConfigurationType.Localhost);

        /// <summary>
        /// 分页查询列表
        /// </summary>
        /// <param name="index"></param>
        /// <param name="indexSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static DataTable QueryLotteryList(string table, int index, int indexSize, out int total)
        {
            var where = string.Empty;

            var dt = DAL.DbHelper.GetList($"{table}", @" time 开奖时间,number 开奖期号,concat( one, '+' , two,'+',three,'=',sum) 开奖结果,
(case when `大小` = '大' then '大' else '' end)大,
(case when `大小` = '小' then '小' else '' end)小,
(case when `单双` = '单' then '单' else '' end )单,
(case when `单双` = '双' then '双' else '' end )双,
(case when `大小` = '大' and `单双` = '单' then '大单' else '' end )大单,
(case when `大小` = '大' and `单双` = '双' then '大双' else '' end )大双,
(case when `大小` = '小' and `单双` = '单' then '小单' else '' end )小单,
(case when `大小` = '小' and `单双` = '双' then '小双' else '' end )小双 ", "id", index, indexSize, where, "time desc", out total);

            return dt;
        }

        /// <summary>
        /// 根据开奖期号获取大于该期号的最新一条数据
        /// </summary>
        /// <returns></returns>
        public static DataTable QueryNewLottery(string table, long numer)
        {
            var sql = $"select * from {table}  where number > {numer}";
            var ds = DAL.DbHelper.MySqlQueryBySqlstring(sql);
            if (ds.Tables.Count == 0)
            {
                return null;
            }
            return ds?.Tables[0];
        }

        /// <summary>
        /// 获取最新一条数据
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static DataTable GetNewOneInfo(string table)
        {
            var sql = $"select * from {table} ORDER BY time DESC LIMIT 0,1;";
            var ds = DAL.DbHelper.MySqlQueryBySqlstring(sql);
            return ds?.Tables[0];
        }

        /// <summary>
        /// 获取大于该时间最新一条数据
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static DataTable GetNewOneInfoByGreaterThanTime(string table, DateTime time)
        {
            var sql = $"select * from {table} where time > '{time.ToString()}' LIMIT 0,1;";
            var ds = DAL.DbHelper.MySqlQueryBySqlstring(sql);
            return ds?.Tables[0];
        }

        /// <summary>
        /// 获取彩票分析数据
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<string, int>> AnalysisData(string table, DateTime? begin, DateTime? end, int type = 0)
        {
            var DefaultNumber = Tool.Helper.ReadConfiguration(Tool.ConfigurationType.DefaultNumber);

            var where = " where 1=1";
            if (begin != null && begin != DateTime.MinValue)
            {
                where += $" and unix_timestamp(time) >= unix_timestamp('{begin}')";
            }
            if (end != null && end != DateTime.MinValue)
            {
                where += $" and unix_timestamp(time) <= unix_timestamp('{end}')";
            }

            var sql = $@"select  time 开奖时间,number 期号,one,two,three,sum,
                                (case when `大小` = '大' then '大' else '' end)大,
                                (case when `大小` = '小' then '小' else '' end)小,
                                (case when `单双` = '单' then '单' else '' end )单,
                                (case when `单双` = '双' then '双' else '' end )双,
                                (case when `大小` = '大' and `单双` = '单' then '大单' else '' end )大单,
                                (case when `大小` = '小' and `单双` = '单' then '小单' else '' end )小单,
                                (case when `大小` = '小' and `单双` = '双' then '小双' else '' end )小双,
                                (case when `大小` = '大' and `单双` = '双' then '大双' else '' end )大双,
                                (case when `极值` = '极大' then '极大' else '' end )极大,
                                (case when `极值` = '极小' then '极小' else '' end )极小,
                                (case when `特殊` = '对子' then '对子' else '' end )对子,
                                (case when `特殊` = '顺子' then '顺子' else '' end )顺子,
                                (case when `特殊` = '豹子' then '豹子' else '' end )豹子
                                from {table} {where} order by time DESC,id desc limit 0,{DefaultNumber}";
            var ds = DAL.DbHelper.MySqlQueryBySqlstring(sql);
            if (ds.Tables.Count <= 0 || ds.Tables[0] == null)
            {
                return null;
            }
            var dt = ds.Tables[0];

            var dic = new Dictionary<string, Dictionary<string, int>>();
            switch (type)
            {
                case 0:
                    // 筛选出今天的数据 （ 为了节省性能，就用上面的查询结果中去按时间条件筛选，在内存中处理）
                    var rows = dt.Select($" CONVERT(开奖时间,System.DateTime) >= '{(begin != null ? begin.ToString() : DateTime.Now.ToString("yyyy-MM-dd 00:00:00"))}'  and CONVERT(开奖时间,System.DateTime) <= '{(end != null ? end.ToString() : DateTime.Now.ToString("yyyy-MM-dd 23:59:59"))}'");
                    var newList = new DataTable();
                    if (rows.Count() > 0)
                    {
                        newList = rows[0].Table.Clone(); // 复制DataRow的表结构
                    }

                    foreach (DataRow row in rows)
                    {
                        newList.ImportRow(row); // 将DataRow添加到DataTable中
                    }

                    dic = new Dictionary<string, Dictionary<string, int>>
                    {
                        {"连续未开数据", HandleContinuityUnopenedData(dt)},
                        {"连续最高次数", HandleHighest(newList)},
                        {"总开奖次数", HandleTotalLottery(newList)}
                    };
                    break;
                case 1: dic.Add("连续未开数据", HandleContinuityUnopenedData(dt)); break;
                case 2: dic.Add("连续最高次数", HandleHighest(dt)); break;
                case 3: dic.Add("总开奖次数", HandleTotalLottery(dt)); break;
            }
            return dic;
        }

        #region 连续未开数据
        /// <summary>
        /// 处理连续未开数据集
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static Dictionary<string, int> HandleContinuityUnopenedData(DataTable dt)
        {
            var dic = new Dictionary<string, int>();

            //循环枚举来获取所有枚举类型的未开次数
            foreach (int myCode in Enum.GetValues(typeof(Type)))
            {
                string strName = Enum.GetName(typeof(Type), myCode);//获取名称
                int number = 0;
                if (myCode > 9)
                {
                    if (myCode == 52)
                    {

                    }
                    GetSpecialNumberByEnum(dt, strName, out number);
                }
                else
                {
                    GetNumberByEnum(dt, strName, out number);
                }
                dic.Add(strName, number);
            }
            return dic;
        }

        /// <summary>
        /// 根据枚举名称获取该类型的未开次数（常规数据）
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="name"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int GetNumberByEnum(DataTable dt, string name, out int number)
        {
            number = 0;
            var tempNumber = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                //如果该行该列值没有则加1
                tempNumber = number;
                if (!Judge(string.IsNullOrWhiteSpace(row[name].ToString()), tempNumber, out number)) return number;
            }
            return number;
        }

        /// <summary>
        /// 根据枚举获取特殊值 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="name"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int GetSpecialNumberByEnum(DataTable dt, string name, out int number)
        {
            number = 0;
            var tempNumber = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                tempNumber = number;
                var row = dt.Rows[i];

                var one = Convert.ToInt32(row["one"]);
                var two = Convert.ToInt32(row["two"]);
                var three = Convert.ToInt32(row["three"]);
                var sum = Convert.ToInt32(row["sum"]);
                var tema = false;
                if (name.IndexOf("特码") > -1)
                {
                    //这是代表是特码几
                    var no = Convert.ToInt32(name.Substring(2, name.Length - 2));
                    tema = !(sum == no);
                    if (!Judge((tema), tempNumber, out number))
                    {
                        return number;
                    }
                    else
                    {
                        continue;
                    }
                }

                var da = row["大"].ToString();
                var xiao = row["小"].ToString();
                var dadan = row["大单"].ToString();
                var xiaodan = row["小单"].ToString();
                var dashuang = row["大双"].ToString();
                var xiaoshuang = row["小双"].ToString();
                var duizi = row["对子"].ToString();
                var shunzi = row["顺子"].ToString();
                var baozi = row["豹子"].ToString();
                switch (name)
                {
                    case "对子": if (!Judge(string.IsNullOrWhiteSpace(duizi), tempNumber, out number)) return number; break;

                    case "豹子": if (!Judge(string.IsNullOrWhiteSpace(baozi), tempNumber, out number)) return number; break;

                    case "顺子": if (!Judge(string.IsNullOrWhiteSpace(shunzi), tempNumber, out number)) return number; break;

                    case "对顺豹1314": if (!Judge(string.IsNullOrWhiteSpace(duizi) && string.IsNullOrWhiteSpace(shunzi) && string.IsNullOrWhiteSpace(baozi) && (sum != 13 && sum != 14), tempNumber, out number)) return number; break;

                    case "对子1314": if (!Judge((string.IsNullOrWhiteSpace(duizi) && (sum != 13 && sum != 14)), tempNumber, out number)) return number; break;

                    case "对顺豹": if (!Judge(string.IsNullOrWhiteSpace(duizi) && string.IsNullOrWhiteSpace(shunzi) && string.IsNullOrWhiteSpace(baozi), tempNumber, out number)) return number; break;

                    case "大小单": if (!Judge(!(!string.IsNullOrWhiteSpace(da) || !string.IsNullOrWhiteSpace(xiaodan)), tempNumber, out number)) return number; break;

                    case "大小双": if (!Judge(!(!string.IsNullOrWhiteSpace(da) || !string.IsNullOrWhiteSpace(xiaoshuang)), tempNumber, out number)) return number; break;

                    case "小大单": if (!Judge(!(!string.IsNullOrWhiteSpace(xiao) || !string.IsNullOrWhiteSpace(dadan)), tempNumber, out number)) return number; break;

                    case "小大双": if (!Judge(!(!string.IsNullOrWhiteSpace(xiao) || !string.IsNullOrWhiteSpace(dashuang)), tempNumber, out number)) return number; break;

                    case "大单小双": if (!Judge(!(!string.IsNullOrWhiteSpace(dadan) || !string.IsNullOrWhiteSpace(xiaoshuang)), tempNumber, out number)) return number; break;

                    case "小单大双": if (!Judge(!(!string.IsNullOrWhiteSpace(xiaodan) || !string.IsNullOrWhiteSpace(dashuang)), tempNumber, out number)) return number; break;

                    case "对顺1314": if (!Judge(string.IsNullOrWhiteSpace(duizi) && string.IsNullOrWhiteSpace(shunzi) && (sum != 13 && sum != 14), tempNumber, out number)) return number; break;

                    case "包含09": if (!Judge(one != 0 && one != 9 && two != 0 && two != 9 && three != 0 && three != 9, tempNumber, out number)) return number; break;

                    case "包含0": if (!Judge(one != 0 && two != 0 && three != 0, tempNumber, out number)) return number; break;

                    case "包含9": if (!Judge(one != 9 && two != 9 && three != 9, tempNumber, out number)) return number; break;
                }
            }
            return number;
        }
        #endregion

        #region 最高数据

        /// <summary>
        /// 处理最高数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Dictionary<string, int> HandleHighest(DataTable dt)
        {
            var dic = new Dictionary<string, int>();

            //循环枚举来获取所有枚举类型的未开次数
            foreach (int myCode in Enum.GetValues(typeof(Type)))
            {
                string strName = Enum.GetName(typeof(Type), myCode);//获取名称
                int number = 0;
                List<string> strList = new List<string> { "大单", "小单", "大双", "小双", "对顺豹1314", "对顺豹", "特码13", "特码14", "对顺1314", "包含09", "包含0", "包含9" };
                if (strName == "包含09")
                {

                }
                if (strList.Contains(strName))
                {
                    GetZGNumberByEnum(dt, strName, out number);
                    dic.Add(strName, number);
                }
            }
            return dic;
        }

        /// <summary>
        /// 根据枚举名称获取该类型的连续最高（最高数据）
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="name"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int GetZGNumberByEnum(DataTable dt, string name, out int number)
        {
            number = 0;
            var tempNum = 0;
            var expression = false;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];

                var one = Convert.ToInt32(row["one"]);
                var two = Convert.ToInt32(row["two"]);
                var three = Convert.ToInt32(row["three"]);
                var sum = Convert.ToInt32(row["sum"]);
                var duizi = row["对子"].ToString();
                var shunzi = row["顺子"].ToString();
                var baozi = row["豹子"].ToString();
                switch (name)
                {
                    case "对顺豹1314": expression = (!string.IsNullOrWhiteSpace(duizi) || !string.IsNullOrWhiteSpace(shunzi) || !string.IsNullOrWhiteSpace(baozi) || (sum == 13 || sum == 14)); break;

                    case "对顺豹": expression = (!string.IsNullOrWhiteSpace(duizi) || !string.IsNullOrWhiteSpace(shunzi) || !string.IsNullOrWhiteSpace(baozi)); break;

                    case "特码13": expression = (sum == 13); break;

                    case "特码14": expression = (sum == 14); break;

                    case "对顺1314": expression = (!string.IsNullOrWhiteSpace(duizi) || !string.IsNullOrWhiteSpace(shunzi) || (sum == 13 || sum == 14)); break;

                    case "包含09": expression = (one == 0 || one == 9 || two == 0 || two == 9 || three == 0 || three == 9); break;

                    case "包含0": expression = (one == 0 || two == 0 || three == 0 ); break;

                    case "包含9": expression = ( one == 9 || two == 9 ||  three == 9); break;

                    default: expression = (!string.IsNullOrWhiteSpace(row[name].ToString())); break;
                }

                if (expression)
                {
                    tempNum++;

                    if (tempNum > number)
                    {
                        number = tempNum;
                    }

                }
                else
                {
                    tempNum = 0;
                }
            }
            return number;
        }

        #endregion

        #region 总开奖数据
        /// <summary>
        /// 处理总开奖
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Dictionary<string, int> HandleTotalLottery(DataTable dt)
        {

            var dic = new Dictionary<string, int>();
            try
            {

                //循环枚举来获取所有枚举类型的未开次数
                foreach (int myCode in Enum.GetValues(typeof(Type)))
                {
                    var number = 0;
                    string strName = Enum.GetName(typeof(Type), myCode);//获取名称

                    if ((myCode >= 13 && myCode <= 21) || myCode >= 51)
                    {
                        if (myCode == 52)
                        {

                        }

                        switch (strName)
                        {
                            case "对顺豹1314": number = dt.Rows.Count > 0 ? dt.Select($" 对子 <> '' or 豹子 <>'' or 顺子 <> '' or sum = 14  or sum = 13 ").Count() : 0; break;

                            case "对子1314": number = dt.Rows.Count > 0 ? dt.Select($" 对子 <> '' or  sum = 14  or sum = 13 ").Count() : 0; break;

                            case "对顺豹": number = dt.Rows.Count > 0 ? dt.Select($" 对子 <> '' or  豹子 <>'' or  顺子 <> '' ").Count() : 0; break;

                            case "大小单": number = dt.Rows.Count > 0 ? dt.Select($" 大 <> '' or 小单 <> '' ").Count() : 0; break;

                            case "大小双": number = dt.Rows.Count > 0 ? dt.Select($" 大 <> '' or 小双 <> '' ").Count() : 0; break;

                            case "小大单": number = dt.Rows.Count > 0 ? dt.Select($" 小 <> '' or  大单 <> '' ").Count() : 0; break;

                            case "小大双": number = dt.Rows.Count > 0 ? dt.Select($" 小 <> '' or  大双 <> '' ").Count() : 0; break;

                            case "大单小双": number = dt.Rows.Count > 0 ? dt.Select($" 大单 <> '' or  小双 <> '' ").Count() : 0; break;

                            case "小单大双": number = dt.Rows.Count > 0 ? dt.Select($" 小单 <> '' or  大双 <> '' ").Count() : 0; break;

                            case "包含09": number = dt.Rows.Count > 0 ? dt.Select($"one=0 or one =9 or two=0 or two=9 or three=0 or three=9 ").Count() : 0; break;

                            case "包含0": number = dt.Rows.Count > 0 ? dt.Select($"one=0 or two=0 or three=0").Count() : 0; break;

                            case "包含9": number = dt.Rows.Count > 0 ? dt.Select($"one =9 or two=9 or three=9 ").Count() : 0; break;
                        }
                    }
                    else if (myCode >= 22 && myCode <= 49)
                    {
                        if (strName.IndexOf("特码") > -1)
                        {
                            //这是代表是特码几
                            var no = Convert.ToInt32(strName.Substring(2, strName.Length - 2));

                            number = dt.Rows.Count > 0 ? dt.Select($" sum = {no} ").Count() : 0;
                        }
                    }
                    else if (myCode == 50)
                    {
                        number = dt.Rows.Count > 0 ? dt.Select($" 对子 <> '' or 顺子 <> '' or sum = 14  or sum = 13 ").Count() : 0;
                    }
                    else
                    {
                        number = dt.Rows.Count > 0 ? dt.Select($" {strName} <> '' ").Count() : 0;
                    }
                    dic.Add(strName, number);
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return dic;
        }
        #endregion

        /// <summary>
        /// 判断是否满足条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool Judge(bool expression, int tempNumber, out int number)
        {
            //因为每次定义out 需要赋值，但是每次进方法，number都会被赋值，所以需要等量代换
            number = tempNumber;
            if (expression)
            {
                number++;
            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取未开统计数据
        /// </summary>
        /// <param name="table"></param>
        /// <param name="name"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static DataTable GetStatisticsData(string table, string name, DateTime begin, DateTime end)
        {
            var sql = $@"select  time 开奖时间,one,two,three,sum,
                                (case when `大小` = '大' then '大' else '' end)大,
                                (case when `大小` = '小' then '小' else '' end)小,
                                (case when `单双` = '单' then '单' else '' end )单,
                                (case when `单双` = '双' then '双' else '' end )双,
                                (case when `大小` = '大' and `单双` = '单' then '大单' else '' end )大单,
                                (case when `大小` = '小' and `单双` = '单' then '小单' else '' end )小单,
                                (case when `大小` = '小' and `单双` = '双' then '小双' else '' end )小双,
                                (case when `大小` = '大' and `单双` = '双' then '大双' else '' end )大双,
                                (case when `极值` = '极大' then '极大' else '' end )极大,
                                (case when `极值` = '极小' then '极小' else '' end )极小,
                                (case when `特殊` = '对子' then '对子' else '' end )对子,
                                (case when `特殊` = '顺子' then '顺子' else '' end )顺子,
                                (case when `特殊` = '豹子' then '豹子' else '' end )豹子
                                from {table} where time >= '{begin}' and time <= '{end}' order by time DESC";
            var ds = DAL.DbHelper.MySqlQueryBySqlstring(sql, connString);

            if (ds.Tables.Count <= 0)
            {
                return null;
            }

            DataTable dt = new DataTable();

            //1、添加列
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("开始时间", typeof(string));
            dt.Columns.Add("结束时间", typeof(string));
            dt.Columns.Add("未开期数", typeof(string));
            var num = 0;
            var bTime = new DateTime();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                bool expression;

                var row = ds.Tables[0].Rows[i];

                if (Tool.Helper.IsNumberic(name))
                {
                    expression = Convert.ToInt32(row["sum"]) != Convert.ToInt32(name);
                }
                else if (name == "大|小单")
                {
                    expression = string.IsNullOrWhiteSpace(row["大"].ToString()) && string.IsNullOrWhiteSpace(row["小单"].ToString());
                }
                else if (name == "小|大单")
                {
                    expression = string.IsNullOrWhiteSpace(row["小"].ToString()) && string.IsNullOrWhiteSpace(row["大单"].ToString());
                }
                else if (name == "小单 大双")
                {
                    expression = string.IsNullOrWhiteSpace(row["小单"].ToString()) && string.IsNullOrWhiteSpace(row["大双"].ToString());
                }
                else if (name == "大|小双")
                {
                    expression = string.IsNullOrWhiteSpace(row["大"].ToString()) && string.IsNullOrWhiteSpace(row["小双"].ToString());
                }
                else if (name == "小|大双")
                {
                    expression = string.IsNullOrWhiteSpace(row["小"].ToString()) && string.IsNullOrWhiteSpace(row["大双"].ToString());
                }
                else if (name == "小单 大双")
                {
                    expression = string.IsNullOrWhiteSpace(row["小单"].ToString()) && string.IsNullOrWhiteSpace(row["大双"].ToString());
                }
                else if (name == "大单 小双")
                {
                    expression = string.IsNullOrWhiteSpace(row["大单"].ToString()) && string.IsNullOrWhiteSpace(row["小双"].ToString());
                }
                else if (name == "对顺豹1314")
                {
                    var asd = row["sum"].ToString();
                    expression = string.IsNullOrWhiteSpace(row["对子"].ToString()) && string.IsNullOrWhiteSpace(row["顺子"].ToString()) && string.IsNullOrWhiteSpace(row["豹子"].ToString()) && row["sum"].ToString() != "13" && row["sum"].ToString() != "14";
                }
                else if (name == "对子1314")
                {
                    expression = string.IsNullOrWhiteSpace(row["对子"].ToString()) && row["sum"].ToString() != "13" && row["sum"].ToString() != "14";
                }
                else if (name == "对顺豹")
                {
                    expression = string.IsNullOrWhiteSpace(row["对子"].ToString()) && string.IsNullOrWhiteSpace(row["顺子"].ToString()) && string.IsNullOrWhiteSpace(row["豹子"].ToString());
                }
                else if (name == "对顺1314")
                {
                    expression = string.IsNullOrWhiteSpace(row["对子"].ToString()) && string.IsNullOrWhiteSpace(row["顺子"].ToString()) && row["sum"].ToString() != "13" && row["sum"].ToString() != "14";
                }
                else if (name == "包含09")
                {
                    expression = row["one"].ToString()!="0" && row["one"].ToString() !="9" && row["two"].ToString() != "0" && row["two"].ToString() != "9" && row["three"].ToString() != "0" && row["three"].ToString() != "9";
                }
                else if (name == "包含0")
                {
                    expression = row["one"].ToString() != "0" && row["two"].ToString() != "0" && row["three"].ToString() != "0";
                }
                else if (name == "包含9")
                {
                    expression =row["one"].ToString() != "9" && row["two"].ToString() != "9" && row["three"].ToString() != "9";
                }
                else
                {
                    expression = string.IsNullOrWhiteSpace(row[name].ToString());
                }

                var time = row["开奖时间"];
                DateTime eTime;
                DataRow dr = dt.NewRow();
                
                //这儿代表已经在数据里未开还没有结束，但是已经没有数据了，所以这里强制赋值结束时间
                if (expression && bTime != DateTime.MinValue && i == ds.Tables[0].Rows.Count - 1)
                {
                    eTime = Convert.ToDateTime(ds.Tables[0].Rows[i == ds.Tables[0].Rows.Count - 1 ? i : i + 1]["开奖时间"]);
                    dr["ID"] = dt.Rows.Count + 1;
                    dr["开始时间"] = bTime;
                    dr["结束时间"] = eTime;
                    dr["未开期数"] = num;
                    dt.Rows.Add(dr);
                }

                if (expression)
                {
                    if (bTime == DateTime.MinValue)
                    {
                        bTime = Convert.ToDateTime(ds.Tables[0].Rows[i == 0 ? 0 : i - 1]["开奖时间"]);
                    }
                    num++;
                }
                else
                {
                    if (num == 0)
                    {
                        bTime = Convert.ToDateTime(time);
                        eTime = Convert.ToDateTime(ds.Tables[0].Rows[i == ds.Tables[0].Rows.Count - 1 ? i : i + 1]["开奖时间"]);
                        dr["ID"] = dt.Rows.Count + 1;
                        dr["开始时间"] = bTime;
                        dr["结束时间"] = eTime;
                        dr["未开期数"] = num;
                        dt.Rows.Add(dr);
                        bTime = new DateTime();
                        continue;
                    }
                    eTime = Convert.ToDateTime(time);
                    dr["ID"] = dt.Rows.Count + 1;
                    dr["开始时间"] = bTime;
                    dr["结束时间"] = eTime;
                    dr["未开期数"] = num;
                    dt.Rows.Add(dr);
                    num = 0;
                    bTime = new DateTime();
                    i--;
                }
            }
            return dt;
        }

        /// <summary>
        /// 获取连续最高统计数据
        /// </summary>
        /// <param name="table"></param>
        /// <param name="name"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static DataTable GetHighestContinuousStatisticsData(string table, string name, DateTime begin, DateTime end)
        {
            var sql = $@"select  time 开奖时间,one,two,three,sum,
                                (case when `大小` = '大' then '大' else '' end)大,
                                (case when `大小` = '小' then '小' else '' end)小,
                                (case when `单双` = '单' then '单' else '' end )单,
                                (case when `单双` = '双' then '双' else '' end )双,
                                (case when `大小` = '大' and `单双` = '单' then '大单' else '' end )大单,
                                (case when `大小` = '小' and `单双` = '单' then '小单' else '' end )小单,
                                (case when `大小` = '小' and `单双` = '双' then '小双' else '' end )小双,
                                (case when `大小` = '大' and `单双` = '双' then '大双' else '' end )大双,
                                (case when `极值` = '极大' then '极大' else '' end )极大,
                                (case when `极值` = '极小' then '极小' else '' end )极小,
                                (case when `特殊` = '对子' then '对子' else '' end )对子,
                                (case when `特殊` = '顺子' then '顺子' else '' end )顺子,
                                (case when `特殊` = '豹子' then '豹子' else '' end )豹子
                                from {table} where time >= '{begin}' and time <= '{end}' order by time DESC";
            var ds = DAL.DbHelper.MySqlQueryBySqlstring(sql, connString);

            if (ds.Tables.Count <= 0)
            {
                return null;
            }

            DataTable dt = new DataTable();

            //1、添加列
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("开始时间", typeof(string));
            dt.Columns.Add("结束时间", typeof(string));
            dt.Columns.Add("连开期数", typeof(string));
            var num = 0;
            var bTime = new DateTime();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                bool expression;

                var row = ds.Tables[0].Rows[i];
                if (name == "小单")
                {
                    expression = !string.IsNullOrWhiteSpace(row["小单"].ToString());
                }
                else if (name == "大单")
                {
                    expression = !string.IsNullOrWhiteSpace(row["大单"].ToString());
                }
                else if (name == "大双")
                {
                    expression = !string.IsNullOrWhiteSpace(row["大双"].ToString());
                }
                else if (name == "小双")
                {
                    expression = !string.IsNullOrWhiteSpace(row["小双"].ToString());
                }
                else if (name == "对顺豹1314")
                {
                    expression = !string.IsNullOrWhiteSpace(row["对子"].ToString()) || !string.IsNullOrWhiteSpace(row["顺子"].ToString()) || !string.IsNullOrWhiteSpace(row["豹子"].ToString()) || row["sum"].ToString() == "13" || row["sum"].ToString() == "14";
                }
                else if (name == "对顺豹子")
                {
                    expression = !string.IsNullOrWhiteSpace(row["对子"].ToString()) || !string.IsNullOrWhiteSpace(row["顺子"].ToString()) || !string.IsNullOrWhiteSpace(row["豹子"].ToString());
                }
                else if (name == "对顺1314")
                {
                    expression = !string.IsNullOrWhiteSpace(row["对子"].ToString()) || !string.IsNullOrWhiteSpace(row["顺子"].ToString()) || row["sum"].ToString() == "13" || row["sum"].ToString() == "14";
                }
                else if (name == "包含09")
                {
                    expression = row["one"].ToString() == "0" || row["one"].ToString() == "9"|| row["two"].ToString() == "0" || row["two"].ToString() == "9"|| row["three"].ToString() == "0" || row["three"].ToString() == "9";
                }
                else if (name == "包含0")
                {
                    expression = row["one"].ToString() == "0" || row["two"].ToString() == "0" || row["three"].ToString() == "0";
                }
                else if (name == "包含9")
                {
                    expression = row["one"].ToString() == "9" || row["two"].ToString() == "9" || row["three"].ToString() == "9";
                }
                else
                {
                    expression = Convert.ToInt32(row["sum"]) == Convert.ToInt32(name.Substring(2, name.Length - 2));
                }

                var time = row["开奖时间"];
                DateTime eTime;
                DataRow dr = dt.NewRow();


                //这儿代表已经在数据里未开还没有结束，但是已经没有数据了，所以这里强制赋值结束时间
                if (expression && bTime != DateTime.MinValue && i == ds.Tables[0].Rows.Count - 1)
                {
                    eTime = Convert.ToDateTime(ds.Tables[0].Rows[i == ds.Tables[0].Rows.Count - 1 ? i : i + 1]["开奖时间"]);
                    dr["ID"] = dt.Rows.Count + 1;
                    dr["开始时间"] = bTime;
                    dr["结束时间"] = eTime;
                    dr["连开期数"] = num;
                    dt.Rows.Add(dr);
                }

                if (expression)
                {
                    if (bTime == DateTime.MinValue)
                    {
                        bTime = Convert.ToDateTime(time);
                    }
                    num++;
                }
                else
                {
                    if (bTime == DateTime.MinValue)
                    {
                        continue;
                    }
                    eTime = Convert.ToDateTime(time);
                    dr["ID"] = dt.Rows.Count + 1;
                    dr["开始时间"] = bTime;
                    dr["结束时间"] = eTime;
                    dr["连开期数"] = num;
                    dt.Rows.Add(dr);
                    num = 0;
                    bTime = new DateTime();
                    i--;
                }
            }
            return dt;
        }

        /// <summary>
        /// 获取详情记录数据
        /// </summary>
        /// <param name="table"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="type">ture：大于等于，false：大于</param>
        /// <param name="isNum"></param>
        /// <returns></returns>
        public static DataTable GetRecordDetails(string table, DateTime begin, DateTime end, bool type, bool isNum)
        {
            var sql = string.Empty;
            if (type && isNum)
            {
                sql = $@"select (@i:=@i+1) as 序号,number 期号,time 开奖时间,concat( one, '+' , two,'+',three,'=',sum) 开奖结果,`组合`,`特殊` from {table},(select   @i:=0)   as   it  where  time <='{begin}' and time >='{end}' ORDER BY time asc";
            }
            else
            {
                sql = $@"select (@i:=@i+1) as 序号,number 期号,time 开奖时间,concat( one, '+' , two,'+',three,'=',sum) 开奖结果,`组合`,`特殊` from {table},(select   @i:=0)   as   it  where  time <='{begin}' and time >'{end}' ORDER BY time asc";
            }
            var ds = DAL.DbHelper.MySqlQueryBySqlstring(sql, connString);
            return ds?.Tables[0];
        }

        /// <summary>
        /// 获取总开数据详情
        /// </summary>
        /// <param name="table"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="type">true：查询常规数据，false：查询特码数据</param>
        /// <param name="groupBy">分组规则，按小时，按天，按月 分组</param>
        /// <returns></returns>
        public static DataTable GetTotalNumberDetails(string table, DateTime begin, DateTime end, bool type, string groupBy)
        {

            var sql = string.Empty;
            if (type)
            {
                sql =
                       $@"SELECT 
{groupBy} 开奖时间,
SUM(大小='大') AS '大',
SUM(大小='小') AS '小',
SUM(单双='单') AS '单',
SUM(单双='双') AS '双',
SUM(组合='大单') AS '大单',
SUM(组合='小单') AS '小单',
SUM(组合='大双') AS '大双',
SUM(组合='小双') AS '小双',
SUM(大小='大' or 组合='小单') AS '大小单',
SUM(大小='大' or 组合='小双') AS '大小双',
SUM(大小='小' or 组合='大单') AS '小大单',
SUM(大小='小' or 组合='大双') AS '小大双',
SUM(组合='大单' or 组合='小双') AS '大单小双',
SUM(组合='小单' or 组合='大双') AS '小单大双',
SUM(极值='极大') AS '极大',
SUM(极值='极小') AS '极小',
SUM(特殊='对子') AS '对子',
SUM(特殊='顺子') AS '顺子',
SUM(特殊='豹子') AS '豹子',
SUM(特殊='豹子' OR 特殊='对子' or 特殊='顺子' or sum=13 or sum = 14 ) AS '对顺豹1314',
SUM(特殊='对子' or sum=13 or sum = 14 ) AS '对1314',
SUM(特殊='豹子' OR 特殊='对子' or 特殊='顺子') AS '对顺豹',
SUM(特殊='对子' or 特殊='顺子' or sum=13 or sum = 14 ) AS '对顺1314',
SUM(one=0 or one=9 or two=0 or two=9 or three=0 or three=9 ) AS '包含09',
SUM(one=0 or two=0 or three=0 ) AS '包含0',
SUM(one=9 or two=9 or three=9 ) AS '包含9'
FROM {table} 
where TIME >= '{begin}' and time <= '{end}'
GROUP BY {groupBy}";
            }
            else
            {
                sql = $@"SELECT
date_format(time,'%Y-%m-%d') 开奖时间,
SUM(sum='0') AS '特码0',
SUM(sum='1') AS '特码1',
SUM(sum='2') AS '特码2',
SUM(sum='3') AS '特码3',
SUM(sum='4') AS '特码4',
SUM(sum='5') AS '特码5',
SUM(sum='6') AS '特码6',
SUM(sum='7') AS '特码7',
SUM(sum='8') AS '特码8',
SUM(sum='9') AS '特码9',
SUM(sum='10') AS '特码10',
SUM(sum='11') AS '特码11',
SUM(sum='12') AS '特码12',
SUM(sum='13') AS '特码13',
SUM(sum='14') AS '特码14',
SUM(sum='15') AS '特码15',
SUM(sum='16') AS '特码16',
SUM(sum='17') AS '特码17',
SUM(sum='18') AS '特码18',
SUM(sum='19') AS '特码19',
SUM(sum='20') AS '特码20',
SUM(sum='21') AS '特码21',
SUM(sum='22') AS '特码22',
SUM(sum='23') AS '特码23',
SUM(sum='24') AS '特码24',
SUM(sum='25') AS '特码25',
SUM(sum='26') AS '特码26',
SUM(sum='27') AS '特码27'
FROM {table} 
where TIME >= '{begin}' and time <= '{end}'
GROUP BY {groupBy}";
            }

            var ds = DAL.DbHelper.MySqlQueryBySqlstring(sql, connString);
            if (ds.Tables.Count == 0 || ds == null)
            {
                return null;
            }
            var dt = ds.Tables[0];
            return dt;
        }

        /// <summary>
        /// 模拟投注
        /// </summary>
        /// <param name="table"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="ruleList"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DataTable SimulatedDataReporting(string table, DateTime begin, DateTime end, List<string> ruleList, out string str)
        {
            str = string.Empty;
            var sql = $@"select  time 开奖时间,
                                number 期号,
                                组合,
                                特殊,
                                concat( one, '+' , two,'+',three,'=',sum) 开奖数字,
                                one,two,three,sum,
                                (case when `大小` = '大' then '大' else '' end)大,
                                (case when `大小` = '小' then '小' else '' end)小,
                                (case when `单双` = '单' then '单' else '' end )单,
                                (case when `单双` = '双' then '双' else '' end )双,
                                (case when `大小` = '大' and `单双` = '单' then '大单' else '' end )大单,
                                (case when `大小` = '小' and `单双` = '单' then '小单' else '' end )小单,
                                (case when `大小` = '小' and `单双` = '双' then '小双' else '' end )小双,
                                (case when `大小` = '大' and `单双` = '双' then '大双' else '' end )大双,
                                (case when `极值` = '极大' then '极大' else '' end )极大,
                                (case when `极值` = '极小' then '极小' else '' end )极小,
                                (case when `特殊` = '对子' then '对子' else '' end )对子,
                                (case when `特殊` = '顺子' then '顺子' else '' end )顺子,
                                (case when `特殊` = '豹子' then '豹子' else '' end )豹子
                                from {table} where time >= '{begin}' and time <= '{end}' order by time asc";
            var ds = DAL.DbHelper.MySqlQueryBySqlstring(sql, connString);

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            DataTable dt = new DataTable();
            //1、添加列
            dt.Columns.Add("序号", typeof(string));
            dt.Columns.Add("开奖时间", typeof(string));
            dt.Columns.Add("期号", typeof(string));
            dt.Columns.Add("开奖数字", typeof(string));
            dt.Columns.Add("属性", typeof(string));
            dt.Columns.Add("下注内容", typeof(string));
            dt.Columns.Add("盈亏金额", typeof(string));
            dt.Columns.Add("当前金额", typeof(string));
            dt.Columns.Add("标注", typeof(string));

            decimal totalMoney = 0;
            decimal totalNumber = 0;
            decimal profitAndLossMoeny = 0;
            decimal winningAmount = 0;
            decimal amountOfLoss = 0;
            var dic = new Dictionary<string, DataTable>();
            foreach (var item in ruleList)
            {
                //规则集合
                var list = item.Replace("模拟投注-", "").Split(',');
                var num = 0;

                var continuitynoPrizeCount = 0; //连续不中次数
                var noPrize = 0; //未中次数
                var WinningCount = 0; //中奖次数
                decimal betsMoney = 0;//下注金额
                var LastTimeIsWinning = true;//最后一次是否中奖
                var isPay = false; //是否已经下过转向注
                var isWinning = true; //是否中奖
                decimal currentMoney = 0;//当前金额
                decimal stopAmount = 0;//停止金额
                bool isNoMoreBet = false; //是否停止下注
                DateTime LastStageStopTime = new DateTime(); //上一个阶段的停止时间
                int firstWin = 0;
                int firstNoPrize = 0;

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var row = ds.Tables[0].Rows[i];

                    if (!string.IsNullOrWhiteSpace(list[10]) || !string.IsNullOrWhiteSpace(list[12]))
                    {
                        if (i == 0)
                        {
                            LastStageStopTime = Convert.ToDateTime(row["开奖时间"]).AddHours(Convert.ToInt32(!string.IsNullOrWhiteSpace(list[10]) ? list[10].Split('|')[2] : list[12].Split('|')[2]));
                        }
                        if (!string.IsNullOrWhiteSpace(list[10].Split('|')[0]))
                        {
                            if (stopAmount <= Convert.ToDecimal("-" + list[10].Split('|')[1]))
                            {
                                isNoMoreBet = true;
                                if (Convert.ToDateTime(row["开奖时间"]) >= LastStageStopTime)
                                {
                                    isNoMoreBet = false;
                                    LastStageStopTime = LastStageStopTime.AddHours(Convert.ToInt32(list[10].Split('|')[2]));
                                    stopAmount = 0;
                                    noPrize = 0; //未中次数
                                    WinningCount = 0; //中奖次数
                                }
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(list[12].Split('|')[0]))
                        {
                            if (stopAmount >= Convert.ToDecimal(list[12].Split('|')[1]))
                            {
                                isNoMoreBet = true;
                                if (Convert.ToDateTime(row["开奖时间"]) >= LastStageStopTime)
                                {
                                    isNoMoreBet = false;
                                    LastStageStopTime = LastStageStopTime.AddHours(Convert.ToInt32(list[12].Split('|')[2]));
                                    stopAmount = 0;
                                    noPrize = 0; //未中次数
                                    WinningCount = 0; //中奖次数
                                }
                            }
                        }
                    }

                    var dr = dt.NewRow();

                    dr["开奖时间"] = row["开奖时间"];
                    dr["开奖数字"] = row["开奖数字"];
                    dr["属性"] = row["组合"] + " " + row["特殊"];
                    dr["当前金额"] = 0;
                    dr["期号"] = row["期号"];
                    var dadan = row["大单"].ToString();
                    var xiaodan = row["小单"].ToString();
                    var dashuang = row["大双"].ToString();
                    var xiaoshuang = row["小双"].ToString();
                    var duizi = row["对子"].ToString();
                    var shunzi = row["顺子"].ToString();
                    var baozi = row["豹子"].ToString();

                    var continuous = list[1] == "连续下注"; //是否是连续下注
                    var content = string.Empty;
                    //判断是连续下注才进此判断
                    if (continuous)
                    {
                        if (continuous && i == 0)
                        {
                            continuous = false;
                        }
                        else
                        {
                            content = ds.Tables[0].Rows[i - 1][list[0]].ToString();
                            if (string.IsNullOrWhiteSpace(content))
                            {
                                continuous = false;
                            }
                        }
                    }

                    if ((continuous && !isNoMoreBet) || (!isNoMoreBet && (!string.IsNullOrWhiteSpace(list[2]) && num == Convert.ToInt32(list[2]))))
                    {
                        decimal profitAndLoss = 0; //盈亏金额
                        var betsContent = list[3];
                        if (!continuous && continuitynoPrizeCount >= Convert.ToInt32(list[4]))
                        {
                            if (list[6] == "立即恢复")
                            {
                                if (isPay)
                                {
                                    noPrize = 0;
                                    WinningCount = 0;
                                    continuitynoPrizeCount = 0;
                                    betsContent = list[3];
                                    isPay = false;
                                }
                                else
                                {
                                    betsContent = list[5];
                                    isPay = true;
                                }
                            }
                            else
                            {
                                //转向投注
                                if (!LastTimeIsWinning && isPay)
                                {
                                    noPrize = 0;
                                    WinningCount = 0;
                                    continuitynoPrizeCount = 0;
                                    betsContent = list[3];
                                    isPay = false;
                                }
                                else
                                {
                                    betsContent = list[5];
                                    isPay = true;
                                }
                            }
                        }

                        if (continuous)
                        {
                            betsContent = content;
                        }

                        switch (betsContent)
                        {
                            case "大单小双": isWinning = !string.IsNullOrWhiteSpace(dadan) || !string.IsNullOrWhiteSpace(xiaoshuang); break;
                            case "小单大双": isWinning = !string.IsNullOrWhiteSpace(xiaodan) || !string.IsNullOrWhiteSpace(dashuang); break;
                            case "对顺豹1314": isWinning = !string.IsNullOrWhiteSpace(duizi) || !string.IsNullOrWhiteSpace(shunzi) || !string.IsNullOrWhiteSpace(baozi) || row["sum"].ToString() == "13" || row["sum"].ToString() == "14"; break;
                            default:
                                isWinning = !string.IsNullOrWhiteSpace(row[betsContent].ToString()); break;
                        }

                        //判断倍投模式
                        if (!string.IsNullOrWhiteSpace(list[7]) && !string.IsNullOrWhiteSpace(list[8]))
                        {
                            if (LastTimeIsWinning)
                            {
                                if (WinningCount == list[7].Split('|').ToList().Count)
                                {
                                    WinningCount = 0;

                                }
                                betsMoney = Convert.ToDecimal(list[7].Split('|')[WinningCount]);

                                //判断是不是第一次中奖，并赋值
                                if (firstWin == 0)
                                {
                                    firstWin = 1;
                                }
                                WinningCount++;
                            }
                            else
                            {
                                if (noPrize == list[8].Split('|').ToList().Count)
                                {
                                    noPrize = 0;
                                }
                                betsMoney = Convert.ToInt32(list[8].Split('|')[noPrize]);

                                //判断是不是第一次未中奖，并赋值
                                if (firstNoPrize == 0)
                                {
                                    firstNoPrize = 1;
                                }
                                noPrize++;
                            }
                        }
                        else
                        {
                            var isTrue = string.IsNullOrWhiteSpace(list[7]);
                            //获取有值的那个倍投
                            var moneyList = !isTrue ? list[7] : list[8];
                            var count = !isTrue ? WinningCount : noPrize;
                            if (count > moneyList.Split('|').ToList().Count - 1)
                            {
                                count = 0;
                            }
                            betsMoney = Convert.ToDecimal(moneyList.Split('|')[count]);
                            if (firstWin == 0)
                            {
                                firstWin = 1;
                            }

                            if (!isTrue)
                            {
                                WinningCount++;
                            }
                            else
                            {
                                noPrize++;
                            }
                        }

                        //var zxczxc = BLL.AutomaticBetting.NGAutomaticBetting.Betting(betsContent, Convert.ToInt32(betsMoney), "加拿大28，2.0倍");

                        dr["下注内容"] = betsContent + "," + betsMoney;
                        if (!string.IsNullOrWhiteSpace(dr["下注内容"].ToString()))
                        {
                            dr["序号"] = "方案" + (ruleList.IndexOf(item) + 1);
                        }

                        totalMoney += betsMoney;
                        totalNumber++;

                        if (isWinning)
                        {
                            profitAndLoss = profitAndLoss + betsMoney * Convert.ToDecimal(Tool.Helper.ReadOddsSettings(list[9], Tool.Helper.ConvertToAllSpell(betsContent)));

                            var tempStr = Tool.Helper.ReadOddsSettings(list[9]);
                            var asd = tempStr.Substring(tempStr.IndexOf("-") + 1, tempStr.Length - tempStr.IndexOf("-") - 1).Split(',').ToList();

                            foreach (var zxc in asd)
                            {
                                var wer = zxc.Split(':');
                                switch (wer[0])
                                {
                                    case "baozitongsha":
                                        if (wer[1] == "Checked" && row["特殊"].ToString().Contains("豹子"))
                                        {
                                            profitAndLoss = profitAndLoss - betsMoney;
                                        }
                                        break;
                                    case "baozihuiben":
                                        if (wer[1] == "Checked" && row["特殊"].ToString().Contains("豹子"))
                                        {
                                            profitAndLoss = 0;
                                        }
                                        break;
                                    case "duizihuiben":
                                        if (wer[1] == "Checked" && row["特殊"].ToString().Contains("对子"))
                                        {
                                            profitAndLoss = 0;
                                        }
                                        break;
                                    case "shunzihuiben":
                                        if (wer[1] == "Checked" && row["特殊"].ToString().Contains("顺子"))
                                        {
                                            profitAndLoss = 0;
                                        }
                                        break;
                                    case "linjiuhuiben":
                                        if (wer[1] == "Checked" && Convert.ToInt32(row["sum"]) >= 0 && Convert.ToInt32(row["sum"]) <= 9)
                                        {
                                            profitAndLoss = 0;
                                        }
                                        break;
                                }
                            }
                            if (row["期号"].ToString() == "2373670")
                            {

                            }

                            if (row["sum"].ToString() == "13" || row["sum"].ToString() == "14")
                            {
                                var typeStr = string.Empty;
                                if (betsMoney < Convert.ToInt32(Tool.Helper.ReadOddsSettings(list[9], $"fenshu")))
                                {
                                    typeStr = "top";
                                }
                                else
                                {
                                    typeStr = "down";
                                }
                                if (betsContent == "大单" || betsContent == "小单" || betsContent == "大双" || betsContent == "小双")
                                {
                                    profitAndLoss = betsMoney * Convert.ToDecimal(Convert.ToDecimal(Tool.Helper.ReadOddsSettings(list[9], $"{typeStr}zuhe")));
                                }
                                if (betsContent == "大" || betsContent == "小" || betsContent == "单" || betsContent == "双")
                                {
                                    profitAndLoss = betsMoney * Convert.ToDecimal(Convert.ToDecimal(Tool.Helper.ReadOddsSettings(list[9], $"{typeStr}sixiang")));
                                }
                                if (betsContent.Contains("特码"))
                                {
                                    profitAndLoss = betsMoney * Convert.ToDecimal(Convert.ToDecimal(Tool.Helper.ReadOddsSettings(list[9], $"{typeStr}tema")));
                                }
                                if (betsContent == "duizi")
                                {
                                    profitAndLoss = betsMoney * Convert.ToDecimal(Convert.ToDecimal(Tool.Helper.ReadOddsSettings(list[9], $"{typeStr}duiz")));
                                }
                            }
                            dr["盈亏金额"] = profitAndLoss;
                            dr["标注"] = "中奖";
                            noPrize = 0;
                            if (betsContent != list[5])
                            {
                                continuitynoPrizeCount = 0;
                            }
                            winningAmount += betsMoney * Convert.ToDecimal(Tool.Helper.ReadOddsSettings(list[9], Tool.Helper.ConvertToAllSpell(betsContent)));
                        }
                        else
                        {
                            profitAndLoss = profitAndLoss - betsMoney;
                            dr["盈亏金额"] = profitAndLoss;
                            WinningCount = 0;
                            continuitynoPrizeCount++;
                            amountOfLoss += betsMoney;
                        }
                        LastTimeIsWinning = isWinning;
                        profitAndLossMoeny += profitAndLoss;
                        currentMoney = currentMoney + profitAndLoss;
                        stopAmount += profitAndLoss;
                        dr["当前金额"] = currentMoney;
                        dt.Rows.Add(dr);
                        if (!LastTimeIsWinning && list[6] == "亏损立即下注")
                        {
                            //判断是否投完所有档位后停止
                            if (string.IsNullOrWhiteSpace(list[11]))
                            {
                                //投完停止
                                if (noPrize <= list[8].Split('|').Length)
                                {
                                    num = Convert.ToInt32(list[2]);
                                }
                                else if (firstNoPrize == 1) //证明是第一次没中
                                {
                                    num = Convert.ToInt32(list[2]);
                                }
                                else
                                {
                                    num = 0;
                                }
                            }
                            else
                            {
                                //投完继续
                                if (noPrize < list[8].Split('|').Length)
                                {
                                    num = Convert.ToInt32(list[2]);
                                }
                                else if (firstNoPrize == 1) //证明是第一次没中
                                {
                                    num = Convert.ToInt32(list[2]);
                                }
                                else
                                {
                                    num = 0;
                                }
                            }
                            //num = Convert.ToInt32(list[2]);
                        }
                        else if (LastTimeIsWinning && list[6] == "盈利立即下注")
                        {
                            if (string.IsNullOrWhiteSpace(list[11]))
                            {
                                //投完停止
                                if (WinningCount <= list[7].Split('|').Length)
                                {
                                    num = Convert.ToInt32(list[2]);
                                }
                                else if (firstWin == 1) //证明是第一次中
                                {
                                    num = Convert.ToInt32(list[2]);
                                }
                                else
                                {
                                    num = 0;
                                }
                            }
                            else
                            {
                                //投完继续
                                if (WinningCount < list[7].Split('|').Length)
                                {
                                    num = Convert.ToInt32(list[2]);
                                }
                                else if (firstWin == 1) //证明是第一次中
                                {
                                    num = Convert.ToInt32(list[2]);
                                }
                                else
                                {
                                    num = 0;
                                }
                            }
                        }
                        else
                        {
                            num = 0;
                        }
                        continue;
                    }
                    dt.Rows.Add(dr);
                    bool isEstablish;

                    if (list[1].Contains("未开"))
                    {
                        switch (list[0])
                        {
                            case "大单小双": isEstablish = string.IsNullOrWhiteSpace(dadan) && string.IsNullOrWhiteSpace(xiaoshuang); break;
                            case "小单大双": isEstablish = string.IsNullOrWhiteSpace(xiaodan) && string.IsNullOrWhiteSpace(dashuang); break;
                            case "对顺豹1314": isEstablish = string.IsNullOrWhiteSpace(duizi) && string.IsNullOrWhiteSpace(shunzi) && string.IsNullOrWhiteSpace(baozi) && row["sum"].ToString() != "13" && row["sum"].ToString() != "14"; break;
                            default:
                                isEstablish = string.IsNullOrWhiteSpace(row[list[0]].ToString()); break;
                        }
                    }
                    else
                    {
                        switch (list[0])
                        {
                            case "大单小双": isEstablish = !string.IsNullOrWhiteSpace(dadan) || !string.IsNullOrWhiteSpace(xiaoshuang); break;
                            case "小单大双": isEstablish = !string.IsNullOrWhiteSpace(xiaodan) || !string.IsNullOrWhiteSpace(dashuang); break;
                            case "对顺豹1314": isEstablish = !string.IsNullOrWhiteSpace(duizi) || !string.IsNullOrWhiteSpace(shunzi) || !string.IsNullOrWhiteSpace(baozi) || row["sum"].ToString() == "13" || row["sum"].ToString() == "14"; break;
                            default:
                                isEstablish = !string.IsNullOrWhiteSpace(row[list[0]].ToString()); break;
                        }
                    }
                    if (list[1].Contains("未开"))
                    {
                        if (isEstablish)
                        {
                            num++;
                        }
                        else
                        {
                            num = 0;
                        }
                    }
                    else
                    {
                        if (isEstablish)
                        {
                            num++;
                        }
                        else
                        {
                            num = 0;
                        }
                    }

                }
                var newDt = dt.Copy();
                dic.Add("dt" + ruleList.IndexOf(item), newDt);
                dt.Rows.Clear();
            }
            if (dic.Count == 1)
            {
                str = $@"投注金额：{totalMoney}，投注次数：{totalNumber}，总盈亏：{profitAndLossMoeny}，中奖金额：{winningAmount}，亏损金额：{amountOfLoss}";
                return dic["dt0"];
            }
            decimal currentAmount = 0;
            for (int i = 0; i < dic["dt0"].Rows.Count; i++)
            {
                var asd = dic["dt0"].Rows[i]["期号"];
                dt.ImportRow(dic["dt" + 0].Rows[i]);
                var isContain = dt.Rows[dt.Rows.Count - 1]["期号"].ToString() == (string)dic["dt" + 0].Rows[i == 0 ? 0 : i]["期号"];

                if (isContain)
                {
                    dt.Rows[dt.Rows.Count - 1]["序号"] = dt.Rows[dt.Rows.Count - 1]["序号"] + " " + dic["dt" + 1].Rows[i]["序号"];
                    if (!string.IsNullOrWhiteSpace(dt.Rows[dt.Rows.Count - 1]["下注内容"].ToString()) || !string.IsNullOrWhiteSpace(dic["dt" + 1].Rows[i]["下注内容"].ToString()))
                    {
                        var one = dt.Rows[dt.Rows.Count - 1]["盈亏金额"].ToString();
                        var two = dic["dt" + 1].Rows[i]["盈亏金额"].ToString();
                        currentAmount += Convert.ToDecimal(string.IsNullOrWhiteSpace(one) ? "0" : one) + Convert.ToDecimal(string.IsNullOrWhiteSpace(two) ? "0" : two);

                        dt.Rows[dt.Rows.Count - 1]["下注内容"] = dt.Rows[dt.Rows.Count - 1]["下注内容"] + " " + dic["dt" + 1].Rows[i]["下注内容"];
                        dt.Rows[dt.Rows.Count - 1]["盈亏金额"] = dt.Rows[dt.Rows.Count - 1]["盈亏金额"] + " " + dic["dt" + 1].Rows[i]["盈亏金额"];
                        dt.Rows[dt.Rows.Count - 1]["当前金额"] = currentAmount;
                    }

                    if (string.IsNullOrWhiteSpace(dt.Rows[dt.Rows.Count - 1]["标注"].ToString()) && string.IsNullOrWhiteSpace(dic["dt" + 1].Rows[i]["标注"].ToString()))
                    {
                        continue;
                    }
                    dt.Rows[dt.Rows.Count - 1]["标注"] = dt.Rows[dt.Rows.Count - 1]["标注"] + "|" + dic["dt" + 1].Rows[i]["标注"];
                }
            }
            str = $@"投注金额：{totalMoney}，投注次数：{totalNumber}，总盈亏：{currentAmount}，中奖金额：{winningAmount}，亏损金额：{amountOfLoss}";
            return dt;
        }


        public static DataTable _DtInfo = new DataTable
        {
            Columns = {
                { "序号",typeof(string)},
                { "开奖时间", typeof(string)},
                { "期号", typeof(string)},
                { "开奖数字", typeof(string)},
                { "属性", typeof(string)},
                { "下注内容", typeof(string)},
                { "盈亏金额", typeof(string)},
                { "当前金额", typeof(string)},
                { "标注", typeof(string)}
            }
        };
        public static string timerNumber = Tool.Helper.GetAutomaticBettingConfigureByName("IsTrueBet") == "True" ? string.Empty : Tool.Helper.GetAutomaticBettingConfigureByName("BeginNumber");

        /// <summary>
        /// 获取自动投注数据
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAutomaticBettingInfo(string table, DateTime timerBegin)
        {
            var dt = new DataTable();
            if (string.IsNullOrWhiteSpace(timerNumber))
            {
                var numberDt = GetNewOneInfoByGreaterThanTime(table, timerBegin);
                if (numberDt != null && numberDt.Rows.Count > 0)
                {
                    timerNumber = numberDt.Rows[0]["number"].ToString();
                }
            }
            else
            {
                if (_DtInfo.Rows.Count > 0)
                {
                    timerNumber = (Convert.ToInt32(_DtInfo.Rows[_DtInfo.Rows.Count - 1]["期号"]) + 1).ToString();
                }
            }
            var sql = $@"select  time 开奖时间,
                                number 期号,
                                CONCAT(`组合`,'|',`特殊`,'|',`极值`) as 属性,
                                concat( one, '+' , two,'+',three,'=',sum) 开奖数字
                                from {table} where number >= '{(string.IsNullOrWhiteSpace(timerNumber) ? "999999999" : timerNumber)}'  order by time asc";
            var ds = DAL.DbHelper.MySqlQueryBySqlstring(sql, connString);

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                dt = _DtInfo.Copy();
                return dt;
            }

            foreach (DataRow item in ds.Tables[0].Rows)
            {
                if (_DtInfo.Select($" 期号 = '{ds.Tables[0].Rows[ds.Tables[0].Rows.IndexOf(item)]["期号"]}' ").Length > 0)
                {
                    continue;
                }
                var dr = _DtInfo.NewRow();
                dr["序号"] = "";
                dr["开奖时间"] = item["开奖时间"];
                dr["期号"] = item["期号"];
                dr["开奖数字"] = item["开奖数字"];
                dr["属性"] = item["属性"];
                dr["下注内容"] = "";
                dr["盈亏金额"] = "";
                dr["当前金额"] = "";
                dr["标注"] = "";
                _DtInfo.Rows.Add(dr);
                timerNumber = item["期号"].ToString();
            }
            dt = _DtInfo.Copy();
            return dt;
        }

        public static List<BetsModel> listModel = new List<BetsModel>();
        public static string _Msg = string.Empty;

        static DateTime timeSlot = DateTime.Now; //时间段
        /// <summary>
        /// 自动投注计算
        /// </summary>
        /// <param name="table"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="ruleList"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DataTable AutomaticBettingCalculation(string table, List<string> ruleList, DateTime timerBegin, out string str)
        {
            str = string.Empty;
            var dt = GetAutomaticBettingInfo(table, timerBegin);
            if (dt.Rows.Count == 0)
            {
                return dt;
            }

            decimal totalBetMoeny = 0; //总投注金额
            decimal totalWinningMoeny = 0;//总中奖金额
            decimal totalLossMoeny = 0;//总亏损金额
            decimal currentMoeny = 0;//当前金额

            var ruleConditionList = new List<RuleCondition>();
            var isTrueBet = Tool.Helper.GetAutomaticBettingConfigureByName("IsTrueBet") == "True"; //获取模拟真实投注的开始期号

            foreach (var item in ruleList)
            {
                var list = item.Replace("自动投注-", "").Split(',');//规则集合
                ruleConditionList.Add(new RuleCondition
                {
                    RuleName = "规则" + (ruleList.IndexOf(item) + 1),
                    DoubleThrowType = !string.IsNullOrWhiteSpace(list[7]),//倍投类型，true为盈利倍投，false为亏损倍投
                    DoubleThrow = (!string.IsNullOrWhiteSpace(list[7]) ? list[7] : list[8]).Split('|'),//倍投金额集合
                    BetContent = list[3],//下注内容
                    ImmediateBetType = string.IsNullOrWhiteSpace(list[6]) ? 0 : list[6].Contains("盈利") ? 1 : list[6].Contains("亏损") ? 2 : 0,
                    ToTurnToContent = list[5],
                    ContinuousFailure = 0,
                    ContinuousPeriod = list[1] == "ABA下注" ? 1 : Convert.ToInt32(list[2]),
                    ConformBetNum = 0,
                    IsconformBetTrue = false
                });
            }

            var number = string.Empty;
            decimal deficitOnly = 0;
            decimal surplusOnly = 0;
            int hour = 0;
            if (!string.IsNullOrWhiteSpace(ruleList[0].Split(',')[10]))
            {
                deficitOnly = Convert.ToDecimal("-" + ruleList[0].Split(',')[10].Split('|')[1]);//只亏金额
                hour = Convert.ToInt32(ruleList[0].Split(',')[10].Split('|')[2]); //暂停时间（小时）
            }
            if (!string.IsNullOrWhiteSpace(ruleList[0].Split(',')[12]))
            {
                surplusOnly = Convert.ToDecimal(ruleList[0].Split(',')[12].Split('|')[1]);//只盈金额
                hour = Convert.ToInt32(ruleList[0].Split(',')[12].Split('|')[2]);//暂停时间（小时）
            }

            var LastStageStopTime = DateTime.Now;
            decimal currentMoenyTemp = 0;
            var isBet = true;//是否下注

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                var shuxing = row["属性"].ToString().Split('|').ToList();
                var shuzi = row["开奖数字"].ToString();

                var isJudge = false;

                if (row["期号"].ToString() == "2400000")
                {

                }

                if (hour != 0)
                {
                    if (i == 0)
                    {
                        LastStageStopTime = Convert.ToDateTime(row["开奖时间"]).AddHours(hour);
                    }

                    if (deficitOnly != 0)
                    {
                        if (currentMoenyTemp <= deficitOnly)
                        {
                            if (Convert.ToDateTime(row["开奖时间"]) <= LastStageStopTime)
                            {
                                isBet = false;
                            }
                        }
                    }
                    if (surplusOnly != 0)
                    {
                        if (currentMoenyTemp >= surplusOnly)
                        {
                            if (Convert.ToDateTime(row["开奖时间"]) <= LastStageStopTime)
                            {
                                isBet = false;
                            }
                        }
                    }

                    if (Convert.ToDateTime(row["开奖时间"]) > LastStageStopTime)
                    {
                        LastStageStopTime = LastStageStopTime.AddHours(hour);
                        currentMoenyTemp = 0;
                        for (int k = 0; k < ruleConditionList.Count; k++)
                        {
                            ruleConditionList[k].DoubleThrowNum = 0;
                            ruleConditionList[k].Num = 0;
                            isBet = true;
                        }
                    }
                }

                var num = 0;

                var listTemp = listModel.FindAll(x => x.Number == row["期号"].ToString());
                for (int j = 0; j < listTemp.Count; j++)
                {
                    dt.Rows[i]["序号"] = dt.Rows[i]["序号"] + listTemp[j].BettingRules + "|";
                    dt.Rows[i]["下注内容"] = dt.Rows[i]["下注内容"] + (!listTemp[j].IsSuccessfulBet ? "下注失败" : listTemp[j].Content + "," + listTemp[j].Modeny + " ");
                    listModel[listModel.IndexOf(listTemp[j])].IsAssignment = true;
                    totalBetMoeny += Convert.ToInt32(listTemp[j].Modeny);
                }
                dt.Rows[i]["序号"] = dt.Rows[i]["序号"].ToString().TrimEnd('|');


                foreach (var item in ruleList)
                {
                    var list = item.Replace("自动投注-", "").Split(',');//规则集合
                    var isWinning = true; //是否中奖
                    var betsContent = list[0]; //满足规则的下注内容

                    var index = ruleList.IndexOf(item);

                    var winningContent = string.Empty;//中奖内容

                    //计算盈亏金额
                    if (ruleConditionList[index].Number == row["期号"].ToString())
                    {
                        var model = listModel.FirstOrDefault(x => x.Number == ruleConditionList[index].Number && x.BettingRules.Contains("规则" + (index + 1)));
                        if (model != null)
                        {
                            decimal moeny = 0; //按单个规则合计判断倍投的金额
                            foreach (var betsItem in model.Content.Split('|').ToList())
                            {
                                //计算盈亏
                                if (betsItem.Contains("特码"))
                                {
                                    var content = betsItem.Substring(2, 2);
                                    isWinning = shuzi.Split('+')[2].Contains(content);
                                }
                                else
                                {
                                    isWinning = betsItem.Length == 1 ? shuxing[0].Contains(betsItem) : shuxing.Contains(betsItem);
                                }
                                var isUsed = false;
                                for (int j = 0; j < list[13].Replace("[", "").Replace("]", "").Split('&').Length; j++)
                                {
                                    var profitAndLoss = Convert.ToDecimal(string.IsNullOrWhiteSpace(dt.Rows[i]["盈亏金额"].ToString()) ? 0 : dt.Rows[i]["盈亏金额"]);
                                    if (model.IsSuccessfulBet)
                                    {
                                        if (isWinning)
                                        {
                                            var betsMoney = Convert.ToDecimal(model.Modeny);
                                            var tempStr = Tool.Helper.ReadOddsSettings(list[9]);
                                            var asd = tempStr.Substring(tempStr.IndexOf("-") + 1, tempStr.Length - tempStr.IndexOf("-") - 1).Split(',').ToList();
                                            var sum = Convert.ToInt32(row["开奖数字"].ToString().Split('=')[1]);
                                            var tempMoeny = Convert.ToDecimal(model.Modeny) * Convert.ToDecimal(Tool.Helper.ReadOddsSettings(list[9], Tool.Helper.ConvertToAllSpell(betsItem)));

                                            foreach (var zxc in asd)
                                            {
                                                var wer = zxc.Split(':');
                                                switch (wer[0])
                                                {
                                                    case "baozitongsha":
                                                        if (wer[1] == "Checked" && row["属性"].ToString().Contains("豹子"))
                                                        {
                                                            tempMoeny = profitAndLoss - betsMoney;
                                                        }
                                                        break;
                                                    case "baozihuiben":
                                                        if (wer[1] == "Checked" && row["属性"].ToString().Contains("豹子") && !betsItem.Contains("对子") && !betsItem.Contains("顺字") && !betsItem.Contains("豹子") && !betsItem.Contains("特码"))
                                                        {
                                                            tempMoeny = 0;
                                                        }
                                                        break;
                                                    case "duizihuiben":
                                                        if (wer[1] == "Checked" && row["属性"].ToString().Contains("对子") && !betsItem.Contains("对子") && !betsItem.Contains("顺子") && !betsItem.Contains("豹子") && !betsItem.Contains("特码"))
                                                        {
                                                            tempMoeny = 0;
                                                        }
                                                        break;
                                                    case "shunzihuiben":
                                                        if (wer[1] == "Checked" && row["属性"].ToString().Contains("顺子") && !betsItem.Contains("对子") && !betsItem.Contains("顺子") && !betsItem.Contains("豹子") && !betsItem.Contains("特码"))
                                                        {
                                                            tempMoeny = 0;
                                                        }
                                                        break;
                                                    case "linjiuhuiben":
                                                        if (wer[1] == "Checked" && row["开奖数字"].ToString().Split('+').ToList().Contains("0") && row["开奖数字"].ToString().Split('+').ToList().Contains("9"))
                                                        {
                                                            tempMoeny = 0;
                                                        }
                                                        break;
                                                }
                                            }

                                            if (sum == 13 || sum == 14)
                                            {
                                                var typeStr = string.Empty;
                                                if (betsMoney < Convert.ToInt32(Tool.Helper.ReadOddsSettings(list[9], $"fenshu")))
                                                {
                                                    typeStr = "top";
                                                }
                                                else
                                                {
                                                    typeStr = "down";
                                                }
                                                if (betsItem == "大单" || betsItem == "小单" || betsItem == "大双" || betsItem == "小双")
                                                {
                                                    tempMoeny = betsMoney * Convert.ToDecimal(Convert.ToDecimal(Tool.Helper.ReadOddsSettings(list[9], $"{typeStr}zuhe")));
                                                }
                                                if (betsItem == "大" || betsItem == "小" || betsItem == "单" || betsItem == "双")
                                                {
                                                    tempMoeny = betsMoney * Convert.ToDecimal(Convert.ToDecimal(Tool.Helper.ReadOddsSettings(list[9], $"{typeStr}sixiang")));
                                                }
                                                if (betsItem.Contains("特码"))
                                                {
                                                    tempMoeny = betsMoney * Convert.ToDecimal(Convert.ToDecimal(Tool.Helper.ReadOddsSettings(list[9], $"{typeStr}tema")));
                                                }
                                                if (betsItem == "对子")
                                                {
                                                    tempMoeny = betsMoney * Convert.ToDecimal(Convert.ToDecimal(Tool.Helper.ReadOddsSettings(list[9], $"{typeStr}duizi")));
                                                }
                                            }

                                            moeny += tempMoeny;
                                            totalWinningMoeny += tempMoeny;
                                            dt.Rows[i]["盈亏金额"] = profitAndLoss + tempMoeny;
                                            if (!isUsed)
                                            {
                                                dt.Rows[i]["标注"] = dt.Rows[i]["标注"] + "中奖|";
                                            }
                                        }
                                        else
                                        {
                                            moeny -= Convert.ToDecimal(model.Modeny);
                                            dt.Rows[i]["盈亏金额"] = profitAndLoss - Convert.ToDecimal(model.Modeny);
                                            if (!isUsed)
                                            {
                                                dt.Rows[i]["标注"] = dt.Rows[i]["标注"] + "|";
                                            }
                                            totalLossMoeny += Convert.ToDecimal(model.Modeny);
                                        }
                                    }
                                    isUsed = true;
                                }
                            }

                            //倍投
                            if ("规则" + (ruleList.IndexOf(item) + 1) == ruleConditionList[index].RuleName)
                            {
                                if (moeny >= 0)
                                {
                                    if (moeny > 0)
                                    {
                                        ruleConditionList[index].DoubleThrowNum = ruleConditionList[index].DoubleThrowType ? ruleConditionList[index].DoubleThrowNum + 1 : 0;
                                    }
                                    else
                                    {
                                        ruleConditionList[index].DoubleThrowNum = 0;
                                    }
                                    ruleConditionList[index].ContinuousFailure = 0;
                                }
                                else
                                {
                                    ruleConditionList[index].DoubleThrowNum = ruleConditionList[index].DoubleThrowType ? 0 : ruleConditionList[index].DoubleThrowNum + 1;
                                    ruleConditionList[index].ContinuousFailure = ruleConditionList[index].ContinuousFailure + 1;
                                }
                                if (list[11].Contains("停止档位"))
                                {
                                    ruleConditionList[index].DoubleThrowNum = 0;
                                    ruleConditionList[index].isStopBetting = true;
                                }
                                else
                                {
                                    ruleConditionList[index].DoubleThrowNum = ruleConditionList[index].DoubleThrowNum >= ruleConditionList[index].DoubleThrow.Length ? 0 : ruleConditionList[index].DoubleThrowNum;
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(dt.Rows[i]["盈亏金额"].ToString()) && !isJudge && index + 1 == ruleList.Count)
                    {
                        currentMoeny = currentMoeny + Convert.ToDecimal(string.IsNullOrWhiteSpace(dt.Rows[i]["盈亏金额"].ToString()) ? 0 : dt.Rows[i]["盈亏金额"]);//计算当前金额
                        dt.Rows[i]["当前金额"] = currentMoeny;
                        currentMoenyTemp = currentMoenyTemp + Convert.ToDecimal(string.IsNullOrWhiteSpace(dt.Rows[i]["盈亏金额"].ToString()) ? 0 : dt.Rows[i]["盈亏金额"]);//计算只盈亏的当前金额
                    }
                    if (isBet) //是否下注
                    {
                        var isNotOpen = false;

                        //判断是否立即倍投
                        var zxc = row["期号"].ToString();
                        if (i != 0 && !isJudge && !string.IsNullOrWhiteSpace(dt.Rows[i]["盈亏金额"].ToString()) && index + 1 == ruleList.Count)
                        {
                            var ruleNameList = dt.Rows[i]["序号"].ToString().Split('|').ToList();

                            foreach (var immediatelyTemp in ruleConditionList)
                            {
                                if (immediatelyTemp.isStopBetting)
                                {
                                    continue;
                                }
                                var immediatelyIndex = ruleConditionList.IndexOf(immediatelyTemp);
                                var count = ruleNameList.FindAll(x => x.Contains(immediatelyTemp.RuleName)).Count;

                                if (count > 0)
                                {
                                    if (immediatelyTemp.ImmediateBetType == 0)
                                    {
                                        isJudge = true;
                                        continue;
                                    }
                                    if (immediatelyTemp.ImmediateBetType == 1)
                                    {
                                        ruleConditionList[immediatelyIndex].IsImmediateBet = Convert.ToDecimal(dt.Rows[i]["盈亏金额"].ToString()) > 0;
                                    }
                                    else if (immediatelyTemp.ImmediateBetType == 2)
                                    {
                                        ruleConditionList[immediatelyIndex].IsImmediateBet = Convert.ToDecimal(dt.Rows[i]["盈亏金额"].ToString()) < 0;
                                    }
                                    isJudge = true;
                                }
                            }
                        }

                        if (!ruleConditionList[index].IsImmediateBet)
                        {
                            var model = ruleConditionList[index];
                            if (list[1] == "ABA下注")
                            {
                                ruleConditionList[index].Num = -999;
                                var betsItem = betsContent.Split('|').ToList()[model.ConformBetNum];

                                //判断是否中奖
                                if (betsItem.Contains("特码"))
                                {
                                    var content = betsItem.Substring(2, 2);
                                    isWinning = shuzi.Split('+')[2].Contains(content);
                                }
                                else
                                {
                                    isWinning = betsItem.Length == 1 ? shuxing[0].Contains(betsItem) || (i != 0 && dt.Rows[i - 1]["属性"].ToString().Split(',')[0].Contains(betsItem)) : shuxing.Contains(betsItem);
                                }
                                if (i != 0 && dt.Rows[i - 1]["属性"].ToString().Split(',')[0].Contains(betsItem) && shuxing[0].Contains(betsContent.Split('|').ToList()[model.ConformBetNum == betsContent.Split('|').ToList().Count - 1 ? betsContent.Split('|').ToList().Count - 1 : model.ConformBetNum + 1]))
                                {
                                    model.ConformBetNum = 2;
                                }
                                else
                                {
                                    if (isWinning)
                                    {
                                        model.ConformBetNum++;
                                        model.IsconformBetTrue = true;
                                        if (model.ConformBetNum == betsContent.Split('|').ToList().Count && model.IsconformBetTrue)
                                        {
                                            ruleConditionList[index].IsImmediateBet = true;
                                            model.ConformBetNum = 0;
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        model.ConformBetNum = 0;
                                    }
                                }
                            }
                            else
                            {
                                //判断连开或未开
                                foreach (var betsItem in betsContent.Split('|').ToList())
                                {
                                    //判断是否中奖
                                    if (betsItem.Contains("特码"))
                                    {
                                        var content = betsItem.Substring(2, 2);
                                        isWinning = shuzi.Split('+')[2].Contains(content);
                                    }
                                    else
                                    {
                                        isWinning = betsItem.Length == 1 ? shuxing[0].Contains(betsItem) : shuxing.Contains(betsItem);
                                    }

                                    if (list[1] == "连续未开")
                                    {
                                        if (!isWinning)
                                        {
                                            if (betsContent.Split('|').ToList().IndexOf(betsItem) == betsContent.Split('|').ToList().Count - 1)
                                            {
                                                isNotOpen = true;
                                            }
                                            if (isNotOpen)
                                            {
                                                num = ruleConditionList.FirstOrDefault(x => x.RuleName == "规则" + (ruleList.IndexOf(item) + 1)).Num + 1;
                                                ruleConditionList.FirstOrDefault(x => x.RuleName == "规则" + (ruleList.IndexOf(item) + 1)).Num = num;
                                                break;
                                            }
                                            continue;
                                        }
                                        else
                                        {
                                            ruleConditionList.FirstOrDefault(x => x.RuleName == "规则" + (ruleList.IndexOf(item) + 1)).Num = 0;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (isWinning)
                                        {
                                            num = ruleConditionList.FirstOrDefault(x => x.RuleName == "规则" + (ruleList.IndexOf(item) + 1)).Num + 1;
                                            ruleConditionList.FirstOrDefault(x => x.RuleName == "规则" + (ruleList.IndexOf(item) + 1)).Num = num;
                                            break;
                                        }
                                        else
                                        {
                                            if (betsContent.Split('|').ToList().IndexOf(betsItem) == betsContent.Split('|').ToList().Count - 1)
                                            {
                                                ruleConditionList.FirstOrDefault(x => x.RuleName == "规则" + (ruleList.IndexOf(item) + 1)).Num = 0;
                                            }
                                            continue;
                                        }
                                    }
                                    if (ruleConditionList.FirstOrDefault(x => x.RuleName == "规则" + (ruleList.IndexOf(item) + 1)).Num == Convert.ToInt32(list[2]))
                                    {
                                        ruleConditionList.FirstOrDefault(x => x.RuleName == "规则" + (ruleList.IndexOf(item) + 1)).isStopBetting = false;
                                    }

                                    ruleConditionList.FirstOrDefault(x => x.RuleName == "规则" + (ruleList.IndexOf(item) + 1)).Num = 0;
                                }
                            }
                        }
                        foreach (var ruleConditionTemp in ruleConditionList)
                        {
                            var ruleConditionIndex = ruleConditionList.IndexOf(ruleConditionTemp);
                            if (ruleConditionTemp.Num == ruleConditionTemp.ContinuousPeriod || ruleConditionTemp.IsImmediateBet)//满足连开或未开
                            {
                                ruleConditionList.FirstOrDefault(x => x.RuleName == ruleConditionTemp.RuleName).Number = (Convert.ToInt32(row["期号"]) + 1).ToString();
                                if (listModel.FindAll(x => x.Number == (Convert.ToInt32(row["期号"]) + 1).ToString() && x.BettingRules.Contains("规则" + (ruleConditionIndex + 1))).Count == 0)
                                {
                                    if (deficitOnly != 0)
                                    {
                                        if (currentMoenyTemp <= deficitOnly)
                                        {
                                            ruleConditionList.FirstOrDefault(x => x.RuleName == "规则" + (ruleConditionIndex + 1)).IsImmediateBet = false;//清零连开次数，
                                            continue;
                                        }
                                    }
                                    if (surplusOnly != 0)
                                    {
                                        if (currentMoenyTemp >= surplusOnly)
                                        {
                                            ruleConditionList.FirstOrDefault(x => x.RuleName == "规则" + (ruleConditionIndex + 1)).IsImmediateBet = false;//清零连开次数，
                                            continue;
                                        }
                                    }

                                    string content;
                                    if (ruleConditionTemp.ContinuousFailure == Convert.ToInt32(list[4]))
                                    {
                                        content = ruleConditionTemp.ToTurnToContent;
                                        ruleConditionTemp.ContinuousFailure = -1;
                                    }
                                    else
                                    {
                                        content = ruleConditionTemp.BetContent;
                                    }

                                    var isSuccessfulBet = true;//是否下注成功，用于判断盈亏显示
                                    //下注
                                    foreach (var temp in list[14].Replace("[", "").Replace("]", "").Split('&'))
                                    {
                                        var result = string.Empty;
                                        var contentList = content.Split('|').ToList();
                                        var amountOfBet = Convert.ToInt32(ruleConditionList[ruleConditionIndex].DoubleThrow[ruleConditionList[ruleConditionIndex].DoubleThrowNum]);
                                        var currentAmount = string.Empty;

                                        for (int n = 0; n < 5; n++)
                                        {
                                            result = "";
                                            if (isTrueBet)
                                            {
                                                result = TrueBet.Betting(contentList, amountOfBet, temp.Split('|')[0], temp.Split('|')[1], out currentAmount); //投注
                                            }
                                            if (string.IsNullOrWhiteSpace(result))
                                            {
                                                break;
                                            }
                                        }
                                        isSuccessfulBet = string.IsNullOrWhiteSpace(result);//是否下注成功
                                        var vbn = string.Empty;
                                        foreach (var con in contentList)
                                        {
                                            vbn += con + " " + amountOfBet + ",";
                                        }
                                        vbn = vbn.TrimEnd(',');
                                        if (string.IsNullOrWhiteSpace(result))
                                        {
                                            _Msg += $"第 {ruleConditionList[index].Number} 期 网站:{temp.Split('|')[0]},房间:{temp.Split('|')[1]} 下注内容:{vbn},下注成功 当前余额:{currentAmount}" + "\n";
                                        }
                                        else
                                        {
                                            _Msg += $"第 {ruleConditionList[index].Number} 期 网站:{temp.Split('|')[0]},房间:{temp.Split('|')[1]} 下注内容:{vbn},{result} 当前余额:{currentAmount}" + "\n";
                                        }

                                        //foreach (var betItem in content.Split('|'))
                                        //{
                                        //    var result = string.Empty;
                                        //    for (int n = 0; n < 5; n++)
                                        //    {
                                        //        result = "";
                                        //        if (isTrueBet)
                                        //        {
                                        //            result = TrueBet.Betting(betItem, Convert.ToInt32(ruleConditionList[ruleConditionIndex].DoubleThrow[ruleConditionList[ruleConditionIndex].DoubleThrowNum]), temp.Split('|')[0], temp.Split('|')[1]); //投注
                                        //        }
                                        //        if (string.IsNullOrWhiteSpace(result))
                                        //        {
                                        //            break;
                                        //        }
                                        //    }
                                        //    isSuccessfulBet = string.IsNullOrWhiteSpace(result);//是否下注成功

                                        //    if (string.IsNullOrWhiteSpace(result))
                                        //    {
                                        //        _Msg += $"第 {ruleConditionList[index].Number} 期，{temp.Split('|')[0]}，{temp.Split('|')[1]} 房间 {betItem} 下注成功" + "\n";
                                        //    }
                                        //    else
                                        //    {
                                        //        _Msg += $"第 {ruleConditionList[index].Number} 期，{temp.Split('|')[0]}，{temp.Split('|')[1]} 房间 {betItem} {result}" + "\n"; ;
                                        //    }
                                        //}
                                    }

                                    listModel.Add(new BetsModel
                                    {
                                        BettingRules = "规则" + (ruleConditionIndex + 1),
                                        Content = content,
                                        Number = (Convert.ToInt32(row["期号"]) + 1).ToString(),
                                        Modeny = ruleConditionList[ruleConditionIndex].DoubleThrow[ruleConditionList[ruleConditionIndex].DoubleThrowNum],
                                        IsAssignment = false,
                                        IsSuccessfulBet = isSuccessfulBet
                                    });
                                }
                                ruleConditionList.FirstOrDefault(x => x.RuleName == "规则" + (ruleConditionIndex + 1)).Num = -1;//清零连开次数，
                                ruleConditionList.FirstOrDefault(x => x.RuleName == "规则" + (ruleConditionIndex + 1)).IsImmediateBet = false;//清零连开次数，
                            }
                        }
                    }
                }
                dt.Rows[i]["标注"] = string.IsNullOrWhiteSpace(dt.Rows[i]["标注"].ToString()) ? "" : dt.Rows[i]["标注"].ToString().Substring(0, dt.Rows[i]["标注"].ToString().Length - 1);
            }

            str = $"投注金额：{totalBetMoeny}，投注次数：{listModel.Count}，总盈亏：{currentMoeny}，中奖金额：{totalWinningMoeny}，亏损金额：{totalLossMoeny}";
            return dt;
        }

        public static DataTable ManualBet(string table, List<ManualBetModel> model)
        {
            var sql =
                $@"select time 开奖时间,
                                number 期号,
                                组合,
                                特殊,
                                concat(one, '+', two, '+', three, '=', sum) 开奖数字,
                                one,two,three,sum,
                                (case when `大小` = '大' then '大' else '' end)大,
                                (case when `大小` = '小' then '小' else '' end)小,
                                (case when `单双` = '单' then '单' else '' end )单,
                                (case when `单双` = '双' then '双' else '' end )双,
                                (case when `大小` = '大' and `单双` = '单' then '大单' else '' end )大单,
                                (case when `大小` = '小' and `单双` = '单' then '小单' else '' end )小单,
                                (case when `大小` = '小' and `单双` = '双' then '小双' else '' end )小双,
                                (case when `大小` = '大' and `单双` = '双' then '大双' else '' end )大双,
                                (case when `极值` = '极大' then '极大' else '' end )极大,
                                (case when `极值` = '极小' then '极小' else '' end )极小,
                                (case when `特殊` = '对子' then '对子' else '' end )对子,
                                (case when `特殊` = '顺子' then '顺子' else '' end )顺子,
                                (case when `特殊` = '豹子' then '豹子' else '' end )豹子
                                from {table} where number = '{model[0].Number}'";
            //var sql = $@"select * from {table} where number = '{model.Number}'";

            var ds = DAL.DbHelper.MySqlQueryBySqlstring(sql);
            if (ds == null && ds.Tables.Count <= 0)
            {
                return new DataTable();
            }
            var newDt = new DataTable
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
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var dt = ds.Tables[0];

                foreach (var temp in model)
                {
                    for (int i = 0; i < temp.ContentList.Count; i++)
                    {
                        var item = temp.ContentList[i];
                        var result = JudgingWinning(dt, item, temp.Modeny, temp.OddsName);

                        var dr = newDt.NewRow();
                        dr["编号"] = i == 0 ? temp.ID : temp.ID + i;
                        dr["类型"] = temp.OddsName;
                        dr["期数"] = temp.Number;
                        dr["投注内容"] = item;
                        dr["投注金额"] = temp.Modeny;
                        dr["盈亏"] = result.Money.ToString();
                        dr["时间"] = DateTime.Now;
                        newDt.Rows.Add(dr);
                    }
                }

            }
            return newDt;
        }

        private static WinningModel JudgingWinning(DataTable dt, string content, string betMoney, string oddsName)
        {
            var result = new WinningModel
            {
                Content = content,
                IsWinning = false,
                Money = -Convert.ToInt32(betMoney)
            };
            var tempContent = string.Empty;
            if (content.Contains("特码"))
            {
                if (dt.Rows[0]["sum"].ToString() == content.Replace("特码", string.Empty))
                {
                    tempContent = content;
                }
            }
            else
            {
                tempContent = dt.Rows[0][content].ToString();
            }

            if (!string.IsNullOrWhiteSpace(tempContent))
            {
                result.IsWinning = true;
                result.Content = content;
                var sumList = new List<string>
                {
                    dt.Rows[0]["one"].ToString(),
                    dt.Rows[0]["two"].ToString(),
                    dt.Rows[0]["three"].ToString(),
                };
                result.Money = Convert.ToDecimal(CalculateWinningAmount(content, Convert.ToDecimal(betMoney), oddsName, sumList, tempContent, dt.Rows[0]["特殊"].ToString()).ToString("f2"));
            }
            return result;
        }

        private static decimal CalculateWinningAmount(string content, decimal betMoney, string oddsName, List<string> sumList, string lotteryContent,string teshuContent)
        {
            var tempStr = Tool.Helper.ReadOddsSettings(oddsName, "", 1);
            if (string.IsNullOrWhiteSpace(tempStr))
            {
                return 0;
            }
            var list = tempStr.Substring(tempStr.IndexOf("-") + 1, tempStr.Length - tempStr.IndexOf("-") - 1).Split(',').ToList();

            var odds = Tool.Helper.ReadOddsSettings(oddsName, Tool.Helper.ConvertToAllSpell(content), 1);
            decimal profitAndLoss = betMoney * Convert.ToDecimal(odds);

            var sum = sumList.Sum(item => Convert.ToInt32(item));
            if (sum == 13 || sum == 14)
            {
                var typeStr = string.Empty;
                if (betMoney < Convert.ToInt32(Tool.Helper.ReadOddsSettings(oddsName, "fenshu", 1)))
                {
                    typeStr = "top";
                }
                else
                {
                    typeStr = "down";
                }
                if (lotteryContent == "大单" || lotteryContent == "小单" || lotteryContent == "大双" || lotteryContent == "小双")
                {
                    profitAndLoss = betMoney * Convert.ToDecimal(Convert.ToDecimal(Tool.Helper.ReadOddsSettings(oddsName, $"{typeStr}zuhe", 1)));
                }
                if (lotteryContent == "大" || lotteryContent == "小" || lotteryContent == "单" || lotteryContent == "双")
                {
                    profitAndLoss = betMoney * Convert.ToDecimal(Convert.ToDecimal(Tool.Helper.ReadOddsSettings(oddsName, $"{typeStr}sixiang", 1)));
                }
                if (lotteryContent.Contains("特码"))
                {
                    profitAndLoss = betMoney * Convert.ToDecimal(Convert.ToDecimal(Tool.Helper.ReadOddsSettings(oddsName, $"{typeStr}tema", 1)));
                }
                if (lotteryContent == "对子")
                {
                    profitAndLoss = betMoney * Convert.ToDecimal(Convert.ToDecimal(Tool.Helper.ReadOddsSettings(oddsName, $"{typeStr}duizi", 1)));
                }
            }

            var isJu = false;
            foreach (var zxc in list)
            {
                var wer = zxc.Split(':');
                switch (wer[0])
                {
                    case "baozitongsha":
                        if (wer[1] == "Checked" && teshuContent == "豹子" && !lotteryContent.Contains("特码") && !lotteryContent.Contains("极"))
                        {
                            profitAndLoss = -betMoney;
                            isJu = true;
                        }
                        break;
                    case "baozihuiben":
                        if (wer[1] == "Checked" && teshuContent == "豹子" && !lotteryContent.Contains("特码") && !lotteryContent.Contains("极"))
                        {
                            profitAndLoss = betMoney;
                            isJu = true;
                        }
                        break;
                    case "duizihuiben":
                        if (wer[1] == "Checked" && teshuContent == "对子" && !lotteryContent.Contains("特码") && !lotteryContent.Contains("极"))
                        {
                            profitAndLoss = betMoney;
                            isJu = true;
                        }
                        break;
                    case "shunzihuiben":
                        if (wer[1] == "Checked" && teshuContent == "顺子" && !lotteryContent.Contains("特码") && !lotteryContent.Contains("极"))
                        {
                            profitAndLoss = betMoney;
                            isJu = true;
                        }
                        break;
                    case "linjiuhuiben":
                        if (wer[1] == "Checked" && sumList.Any(x => x == "0" || x == "9") && !lotteryContent.Contains("特码") && !lotteryContent.Contains("极"))
                        {
                            profitAndLoss = betMoney;
                            isJu = true;
                        }
                        break;
                }
                if (isJu)
                {
                    break;
                }
                if (list.IndexOf(zxc) > 5)
                {
                    break;
                }
            }
            
            return profitAndLoss;
        }

        public static bool AddReportData(DataTable dt, string name)
        {
            try
            {
                var sql = "INSERT INTO reportdata (类型,期数,投注内容,投注金额,盈亏,时间,彩种) VALUES ";
                var val = string.Empty;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var row = dt.Rows[i];
                    val += $"('{row["类型"]}','{row["期数"]}','{row["投注内容"]}','{row["投注金额"]}','{row["盈亏"]}','{row["时间"]}','{name}'),";
                }
                sql = sql + val.Substring(0, val.Length - 1);
                var result = DAL.DbHelper.MysqlExecuteSql(sql, connString);
                if (result > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public static DataTable GetReportDataPast(string typeName, string begin, string end, string colorVariety)
        {
            var where = new List<string>();
            if (typeName != "全部")
            {
                where.Add($"类型='{typeName}'");
            }
            if (!string.IsNullOrWhiteSpace(begin))
            {
                where.Add($"时间>='{Convert.ToDateTime(begin)}'");
            }
            if (!string.IsNullOrWhiteSpace(end))
            {
                where.Add($"时间<='{Convert.ToDateTime(end)}'");
            }
            if (colorVariety != "全部")
            {
                where.Add($"彩种='{colorVariety}'");
            }

            var sql = $"select * from reportdata where {string.Join(" and ", where)}";
            var ds = DAL.DbHelper.MySqlQueryBySqlstring(sql, connString);

            if (ds.Tables.Count == 0)
            {
                return null;
            }
            return ds?.Tables[0];
        }
    }

    public class WinningModel
    {
        /// <summary>
        /// 是否中奖
        /// </summary>
        public bool IsWinning { get; set; }

        /// <summary>
        /// 中奖内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 中奖金额
        /// </summary>
        public decimal Money { get; set; }
    }

    /// <summary>
    /// 手动下注
    /// </summary>
    public class ManualBetModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 下注内容
        /// </summary>
        public List<string> ContentList { get; set; }

        /// <summary>
        /// 期号
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 下注金额
        /// </summary>
        public string Modeny { get; set; }

        /// <summary>
        /// 下注金额
        /// </summary>
        public string OddsName { get; set; }
    }

    public class BetsModel
    {
        /// <summary>
        /// 下注规则
        /// </summary>
        public string BettingRules { get; set; }

        /// <summary>
        /// 下注内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 期号
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 下注金额
        /// </summary>
        public string Modeny { get; set; }

        /// <summary>
        /// 是否赋过值
        /// </summary>
        public bool IsAssignment { get; set; }

        /// <summary>
        /// 是否下注成功，用于判断盈亏显示
        /// </summary>
        public bool IsSuccessfulBet { get; set; }
    }

    public class RuleCondition
    {
        /// <summary>
        /// 规则名称
        /// </summary>
        public string RuleName { get; set; }

        /// <summary>
        /// 连续次数
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 倍投类型（true：盈利，false：亏损）
        /// </summary>
        public bool DoubleThrowType { get; set; }

        /// <summary>
        /// 倍投金额集合
        /// </summary>
        public string[] DoubleThrow { get; set; }

        /// <summary>
        /// 下注内容
        /// </summary>
        public string BetContent { get; set; }

        /// <summary>
        /// 倍投次数
        /// </summary>
        public int DoubleThrowNum { get; set; }

        /// <summary>
        /// 立即投注类型（0：无立即投注，1：盈利立即投注，2：亏损立即投注）
        /// </summary>
        public int ImmediateBetType { get; set; }

        /// <summary>
        /// 是否立即投注
        /// </summary>
        public bool IsImmediateBet { get; set; }

        /// <summary>
        /// 期号
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 是否停止投注
        /// </summary>
        public bool isStopBetting { get; set; }

        /// <summary>
        /// 转向投注内容
        /// </summary>
        public string ToTurnToContent { get; set; }

        /// <summary>
        /// 连续未中期数
        /// </summary>
        public int ContinuousFailure { get; set; }

        /// <summary>
        /// 规则的连续开出或未开
        /// </summary>
        public int ContinuousPeriod { get; set; }

        /// <summary>
        /// ABA下注的判断下标
        /// </summary>
        public int ConformBetNum { get; set; }

        /// <summary>
        /// 是否全部满足
        /// </summary>
        public bool IsconformBetTrue { get; set; }
    }

    public enum Type
    {
        大 = 0,
        小 = 1,
        单 = 2,
        双 = 3,
        大单 = 4,
        小单 = 5,
        大双 = 6,
        小双 = 7,
        极小 = 8,
        极大 = 9,
        对子 = 10,
        豹子 = 11,
        顺子 = 12,
        对顺豹1314 = 13,
        对子1314 = 14,
        对顺豹 = 15,
        大小单 = 16,
        大小双 = 17,
        小大单 = 18,
        小大双 = 19,
        大单小双 = 20,
        小单大双 = 21,
        特码0 = 22,
        特码2 = 23,
        特码3 = 24,
        特码4 = 25,
        特码5 = 26,
        特码6 = 27,
        特码7 = 28,
        特码8 = 29,
        特码9 = 30,
        特码10 = 31,
        特码11 = 32,
        特码12 = 33,
        特码13 = 34,
        特码14 = 35,
        特码15 = 36,
        特码16 = 37,
        特码17 = 38,
        特码18 = 39,
        特码19 = 40,
        特码20 = 41,
        特码21 = 42,
        特码22 = 43,
        特码23 = 44,
        特码24 = 45,
        特码25 = 46,
        特码26 = 47,
        特码27 = 48,
        特码1 = 49,
        对顺1314 = 50,
        包含09 = 51,
        包含0 = 52,
        包含9 = 53,
    }
}
