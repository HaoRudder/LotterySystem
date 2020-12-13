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
                            var analogDatalsit = DuanKaiHouTouZhu(ruleinfo, dataList);
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
                foreach (var item in dataList)
                {
                    var model = new AnalogData
                    {
                        id = item.id.ToString(),
                        kaijiangshijian = item.create_time.ToString(),
                        qihao = item.qishu,
                        kaijiangshuzi = item.one + item.two + item.three,
                        shuxing = item.zuhe + item.teshu + item.jizhi,
                        dangqianjine = "0",
                    };
                }
            }
            catch (Exception)
            {

                throw;
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
