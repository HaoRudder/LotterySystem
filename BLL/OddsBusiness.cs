using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    /// <summary>
    /// 赔率设置逻辑代码
    /// </summary>
    public class OddsBusiness
    {
        /// <summary>
        /// 获取赔率信息
        /// </summary>
        /// <returns></returns>
        public static List<OddssInfo> GetOddssInfo()
        {
            var path = Environment.CurrentDirectory + "/OddsInfo.json";
            var data = Tool.Helper.ReadTheLocalFile(path).Trim();
            var list = Tool.Helper.DeserializeObject<List<OddssInfo>>(data);

            return list;
        }

        /// <summary>
        /// 修改赔率信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool ModifyOddssInfo(OddssInfo model)
        {
            var list = GetOddssInfo();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].OddsID == model.OddsID)
                {
                    list[i] = model;
                }
            }
            var json = JsonConvert.SerializeObject(list);
            var isSuccess = Tool.Helper.WriteFile(Environment.CurrentDirectory, "OddsInfo.json", json);
            return isSuccess;
        }
    }

    public class OddssInfo
    {
        public int OddsID { get; set; }
        public string OddsName { get; set; }
        public bool baozitongsha { get; set; }
        public bool baozihuiben { get; set; }
        public bool duizihuiben { get; set; }
        public bool shunzihuiben { get; set; }
        public bool linjiuhuiben { get; set; }
        public decimal dadan { get; set; }
        public decimal xiaodan { get; set; }
        public decimal dashuang { get; set; }
        public decimal xiaoshuang { get; set; }
        public decimal duizi { get; set; }
        public decimal shunzi { get; set; }
        public decimal baozi { get; set; }
        public decimal jishu { get; set; }
        public decimal da { get; set; }
        public decimal xiao { get; set; }
        public decimal dan { get; set; }
        public decimal shuang { get; set; }
        public decimal tema0 { get; set; }
        public decimal tema1 { get; set; }
        public decimal tema2 { get; set; }
        public decimal tema3 { get; set; }
        public decimal tema4 { get; set; }
        public decimal tema5 { get; set; }
        public decimal tema6 { get; set; }
        public decimal tema7 { get; set; }
        public decimal tema8 { get; set; }
        public decimal tema9 { get; set; }
        public decimal tema10 { get; set; }
        public decimal tema11 { get; set; }
        public decimal tema12 { get; set; }
        public decimal tema13 { get; set; }
        public decimal tema14 { get; set; }
        public decimal tema15 { get; set; }
        public decimal tema16 { get; set; }
        public decimal tema17 { get; set; }
        public decimal tema18 { get; set; }
        public decimal tema19 { get; set; }
        public decimal tema20 { get; set; }
        public decimal tema21 { get; set; }
        public decimal tema22 { get; set; }
        public decimal tema23 { get; set; }
        public decimal tema24 { get; set; }
        public decimal tema25 { get; set; }
        public decimal tema26 { get; set; }
        public decimal tema27 { get; set; }
        public decimal topzuhe { get; set; }
        public decimal topsixiang { get; set; }
        public decimal toptema { get; set; }
        public decimal topduizi { get; set; }
        public decimal downzuhe { get; set; }
        public decimal downsixiang { get; set; }
        public decimal downtema { get; set; }
        public decimal downduizi { get; set; }
        public decimal fenshu { get; set; }
        public decimal dadanxiaoshuang { get; set; }
        public decimal xiaodandashuang { get; set; }
        public decimal duishunbao1314 { get; set; }
    }
}
