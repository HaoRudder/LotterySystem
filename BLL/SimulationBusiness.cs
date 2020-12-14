using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class SimulationBusiness
    {
        private static string _connString = string.Empty;
        //private static string testConnString = string.Empty;
        public SimulationBusiness()
        {
            var path = Environment.CurrentDirectory + "//ConfigurationFile.ini";
            var list = Tool.Helper.ReadTheLocalFile(path).Split('\n');
            //_connString = $"Data Source ={list[0].Split(':')[1]}; Initial Catalog = {list[1].Split(':')[1]}; User ID ={list[3].Split(':')[1]}; Password ={list[3].Split(':')[1]};PORT= 33060 ;Character Set=utf8; Allow User Variables=True";
            _connString = $"Data Source =localhost; Initial Catalog = {list[8].Split(':')[1]}; User ID ={list[6].Split(':')[1]}; Password ={list[7].Split(':')[1]};Character Set=utf8; Allow User Variables=True";
        }

        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <param name="table"></param>
        /// <param name="maxTime"></param>
        /// <param name="minTime"></param>
        /// <returns></returns>
        public List<DataInfo> GetDataList(string table, DateTime minTime, DateTime maxTime)
        {
            var list = new List<DataInfo>();
            try
            {
                var sql = $"SELECT * FROM {table} WHERE create_time>= '{minTime}' AND  create_time <='{maxTime}';";
                var ds = DAL.DbHelper.MySqlQueryBySqlstring(sql, _connString);

                if (ds != null && ds.Tables.Count > 0)
                {
                    list = Tool.Helper.ToList<DataInfo>(ds.Tables[0]);
                }
                return list;
            }
            catch (Exception)
            {
            }
            return list;
        }

        public List<AnalogData> GetAnalogDataList(List<int> ruleIdList, string table, DateTime minTime, DateTime maxTime)
        {
            var list = new List<AnalogData>();
            try
            {
                var dataList = GetDataList(table, minTime, maxTime);

                foreach (var item in ruleIdList)
                {
                    var ruleinfo = new RulesBusiness().GetRuleinfo(item);
                    switch (ruleinfo.RuleType)
                    {
                        case 1://断开后投注
                            list.AddRange(DuanKaiHouTouZhu(ruleinfo, dataList));
                            break;
                    }
                }
            }
            catch (Exception)
            {

            }
            return list;
        }

        /// <summary>
        /// 断开后投注算法
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="dataList"></param>
        /// <returns></returns>
        public List<AnalogData> DuanKaiHouTouZhu(Ruleinfo rule, List<DataInfo> dataList)
        {
            var list = new List<AnalogData>();
            try
            {
                //投注条件算法

                //开始投注
                list = BetAlgorithm(rule, dataList);
            }
            catch (Exception ex)
            {

                throw;
            }
            return list;
        }

        /// <summary>
        /// 投注算法（通用）
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="dataList"></param>
        /// <returns></returns>
        public List<AnalogData> BetAlgorithm(Ruleinfo rule, List<DataInfo> dataList)
        {
            var list = new List<AnalogData>();
            try
            {
                foreach (var item in dataList)
                {
                    //倍投金额
                    var lossMultiple = rule.LossMultiple.Split('|');
                    var profitMultiple = rule.ProfitMultiple.Split('|');
                    var lossMultipleMoney = Convert.ToDecimal(lossMultiple[rule.LossMultipleLevel]);
                    var profitMultipleMoney = Convert.ToDecimal(profitMultiple[rule.ProfitMultipleLevel]);
                    var data = WinOrNot(item, rule.BetContent, profitMultipleMoney, lossMultipleMoney, rule.OddsID);

                    if (data.biaozhu == "中奖")
                    {
                        if (rule.ProfitMultipleLevel >= profitMultiple.Length - 1)
                        {
                            rule.ProfitMultipleLevel = 0;
                        }
                        rule.ProfitMultipleLevel++;
                        rule.LossMultipleLevel = 0;

                    }
                    else
                    {
                        if (rule.LossMultipleLevel >= lossMultiple.Length - 1)
                        {
                            rule.LossMultipleLevel = 0;
                        }
                        else
                        {
                            rule.LossMultipleLevel++;
                        }
                        rule.ProfitMultipleLevel = 0;
                    }

                    var model = new AnalogData
                    {
                        id = item.id.ToString(),
                        kaijiangshijian = item.create_time.ToString(),
                        qihao = item.qishu,
                        kaijiangshuzi = item.one + "+" + item.two + "+" + item.three + "=" + (Convert.ToInt32(item.one) + Convert.ToInt32(item.two) + Convert.ToInt32(item.three)),
                        shuxing = item.zuhe + "|" + item.teshu + "|" + item.jizhi,
                        biaozhu = data.biaozhu,
                        xiazhuneirong = data.xiazhuneirong,
                        yingkuijine = data.yingkuijine
                    };
                    list.Add(model);
                }

                //这里单独计算当前金额
                list = CalculateCurrentAmount(list);
                return list;
            }
            catch (Exception ex)
            {
            }
            return list;
        }

        /// <summary>
        /// 是否中奖并计算赔率
        /// </summary>
        /// <returns></returns>
        public AnalogData WinOrNot(DataInfo dataInfo, string betContent, decimal profitMultipleMoney, decimal lossMultipleMoney, int oddsID)
        {
            var data = new AnalogData();

            //判断是否中奖
            foreach (var item in betContent.Split('|'))
            {
                var openContent = GetWinContent(dataInfo, item);

                if (!string.IsNullOrWhiteSpace(openContent))
                {
                    //计算赔率
                    var oddsInfo = OddsBusiness.GetOddssInfo().FirstOrDefault(x => x.OddsID == oddsID);
                    var pinyin = Tool.Helper.ConvertToAllSpell(openContent);
                    var val = oddsInfo.GetType().GetProperty(pinyin).GetValue(oddsInfo, null);
                    var money = profitMultipleMoney * Convert.ToDecimal(val);

                    data.xiazhuneirong = data.xiazhuneirong + item + "," + profitMultipleMoney + "|";

                    data.yingkuijine = (Convert.ToDecimal(data.yingkuijine) + money).ToString();
                    data.biaozhu = "中奖";
                }
                else
                {
                    data.yingkuijine = (Convert.ToDecimal(data.yingkuijine) + (-lossMultipleMoney)).ToString();
                    data.xiazhuneirong = data.xiazhuneirong + item + "," + lossMultipleMoney + "|";
                }
            }
            data.xiazhuneirong = data.xiazhuneirong.Substring(0, data.xiazhuneirong.Length - 1);
            return data;
        }

        /// <summary>
        /// 获取中奖内容
        /// </summary>
        /// <param name="dataInfo"></param>
        /// <param name="betContent"></param>
        /// <returns></returns>
        public string GetWinContent(DataInfo dataInfo, string betContent)
        {
            var openContent = string.Empty;

            if (betContent.Contains("特码") && dataInfo.sum == betContent)
            {
                openContent = betContent;
            }
            if (dataInfo.daxiao == betContent)
            {
                openContent = betContent;
            }
            if (dataInfo.danshuang == betContent)
            {
                openContent = betContent;
            }
            if (dataInfo.teshu == betContent)
            {
                openContent = betContent;
            }
            if (dataInfo.zuhe == betContent)
            {
                openContent = betContent;
            }
            if (dataInfo.jizhi == betContent)
            {
                openContent = betContent;
            }
            return openContent;
        }

        /// <summary>
        /// 计算当前金额
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<AnalogData> CalculateCurrentAmount(List<AnalogData> list)
        {
            decimal money = 0;
            //这里单独计算当前金额
            for (int i = 0; i < list.Count; i++)
            {
                var row = list[i];

                if (!string.IsNullOrWhiteSpace(row.yingkuijine))
                {
                    row.dangqianjine = (money + Convert.ToDecimal(row.yingkuijine)).ToString();
                    money = Convert.ToDecimal(row.dangqianjine);
                }
            }
            return list;
        }
    }
    public class AnalogData
    {
        public string id { get; set; }
        public string kaijiangshijian { get; set; }
        public string qihao { get; set; }
        public string kaijiangshuzi { get; set; }
        public string shuxing { get; set; }
        public string xiazhuneirong { get; set; }
        public string yingkuijine { get; set; }
        public string dangqianjine { get; set; }
        public string biaozhu { get; set; }
    }

    public class DataInfo
    {
        public int id { get; set; }
        public string qishu { get; set; }
        public DateTime create_time { get; set; }
        public string one { get; set; }
        public string two { get; set; }
        public string three { get; set; }
        public string sum { get; set; }
        public string daxiao { get; set; }
        public string danshuang { get; set; }
        public string teshu { get; set; }
        public string zuhe { get; set; }
        public string jizhi { get; set; }
        public DateTime count_down { get; set; }
        public DateTime count_down2 { get; set; }
    }
}
