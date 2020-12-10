using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class RulesBusiness
    {
        public bool AddRules(Ruleinfo model)
        {
            if (model.ID > 0)
            {
                Add(model);
            }
            else
            {
                Update(model);
            }

        }

        public bool Add(Ruleinfo model)
        {
            var sql = $@"insert into ruleinfo(
            OddsID,
            OnlineBetID,
            OpenContent,
            JudgeCondition,
            JudgeNumber,
            BetContent,
            NoWinBetNumber,
            NoWinBetConent,
            StopProfit,
            StopLoss,
            StopBetHours,
            intervalBetHours,
            IsTurnBet,
            ProfitMultiple,
            LossMultiple,
            IsProfitBetNow,
            IsLossBetNow,
            BetGearStop,
            CreationTime)
            values
            (
            {model.OddsID}, --(int)赔率ID，对应json文件
            {model.OnlineBetID}, --(int)线上投注信息ID
            '{model.BetContent}', --(varchar)开奖内容
            '{model.JudgeCondition}', --(varchar)判断投注条件
            {model.JudgeNumber}, --(int)判断投注期数
            '{model.BetContent}', --(varchar)投注内容，可多选，用 | 隔开
            {model.NoWinBetNumber}, --(int)未中奖期数
            '{model.BetContent}', --(varchar)未中奖后投注内容
            {model.StopProfit}, --(int)止盈金额
            {model.StopLoss}, --(int)止损金额
            {model.StopBetHours}, --(int)停止投注的小时数
            {model.intervalBetHours}, --(int)间隔投注的小时数
            {model.IsTurnBet}, --(bit)是否转向投注直到未中奖后停止，配合未中奖投注内容一起使用
            '{model.ProfitMultiple}', --(varchar)盈利倍投
            '{model.LossMultiple}', --(varchar)亏损倍投
            {model.IsProfitBetNow}, --(bit)盈利后立即下注
            {model.IsLossBetNow}, --(bit)亏损后立即下注
            {model.BetGearStop}, --(bit)下注完倍投档位后立即停止
            now() --(timestamp)创建时间
                )";
            
            return false;
        }

        public bool Update(Ruleinfo model)
        {
            return false;
        }
    }


    /// <summary>
    /// 规则表
    /// </summary> 
    public class Ruleinfo
    {
        /// <summary>
        ///ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 赔率ID，对应json文件
        /// </summary>
        public int OddsID { get; set; }

        /// <summary>
        /// 线上投注信息ID
        /// </summary>
        public int OnlineBetID { get; set; }

        /// <summary>
        /// 开奖内容
        /// </summary>
        public string OpenContent { get; set; }

        /// <summary>
        /// 判断投注条件
        /// </summary>
        public string JudgeCondition { get; set; }

        /// <summary>
        /// 判断投注期数
        /// </summary>
        public int JudgeNumber { get; set; }

        /// <summary>
        /// 投注内容，可多选，用|隔开
        /// </summary>
        public string BetContent { get; set; }

        /// <summary>
        /// 未中奖期数
        /// </summary>
        public int NoWinBetNumber { get; set; }

        /// <summary>
        /// 未中奖后投注内容
        /// </summary>
        public string NoWinBetConent { get; set; }

        /// <summary>
        /// 止盈金额
        /// </summary>
        public int StopProfit { get; set; }

        /// <summary>
        /// 止损金额
        /// </summary>
        public int StopLoss { get; set; }

        /// <summary>
        /// 停止投注的小时数
        /// </summary>
        public int StopBetHours { get; set; }

        /// <summary>
        /// 间隔投注的小时数
        /// </summary>
        public int intervalBetHours { get; set; }

        /// <summary>
        /// 是否转向投注直到未中奖后停止，配合未中奖投注内容一起使用
        /// </summary>
        public int IsTurnBet { get; set; }

        /// <summary>
        /// 盈利倍投
        /// </summary>
        public string ProfitMultiple { get; set; }

        /// <summary>
        /// 亏损倍投
        /// </summary>
        public string LossMultiple { get; set; }

        /// <summary>
        /// 盈利后立即下注
        /// </summary>
        public int IsProfitBetNow { get; set; }

        /// <summary>
        /// 亏损后立即下注
        /// </summary>
        public int IsLossBetNow { get; set; }

        /// <summary>
        /// 下注完倍投档位后立即停止
        /// </summary>
        public int BetGearStop { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

    }
}
