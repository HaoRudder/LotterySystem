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
        public int dadan { get; set; }
        public int xiaodan { get; set; }
        public int dashuang { get; set; }
        public int xiaoshuang { get; set; }
        public int duizi { get; set; }
        public int shunzi { get; set; }
        public int baozi { get; set; }
        public int jishu { get; set; }
        public int da { get; set; }
        public int xiao { get; set; }
        public int dan { get; set; }
        public int shuang { get; set; }
        public int tema0 { get; set; }
        public int tema1 { get; set; }
        public int tema2 { get; set; }
        public int tema3 { get; set; }
        public int tema4 { get; set; }
        public int tema5 { get; set; }
        public int tema6 { get; set; }
        public int tema7 { get; set; }
        public int tema8 { get; set; }
        public int tema9 { get; set; }
        public int tema10 { get; set; }
        public int tema11 { get; set; }
        public int tema12 { get; set; }
        public int tema13 { get; set; }
        public int tema14 { get; set; }
        public int tema15 { get; set; }
        public int tema16 { get; set; }
        public int tema17 { get; set; }
        public int tema18 { get; set; }
        public int tema19 { get; set; }
        public int tema20 { get; set; }
        public int tema21 { get; set; }
        public int tema22 { get; set; }
        public int tema23 { get; set; }
        public int tema24 { get; set; }
        public int tema25 { get; set; }
        public int tema26 { get; set; }
        public int tema27 { get; set; }
        public int topzuhe { get; set; }
        public int topsixiang { get; set; }
        public int toptema { get; set; }
        public int topduizi { get; set; }
        public int downzuhe { get; set; }
        public int downsixiang { get; set; }
        public int downtema { get; set; }
        public int downduizi { get; set; }
        public int fenshu { get; set; }
        public int dadanxiaoshuang { get; set; }
        public int xiaodandashuang { get; set; }
        public int duishunbao1314 { get; set; }
    }
}
