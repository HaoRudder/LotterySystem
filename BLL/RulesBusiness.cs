using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace BLL
{
    public class RulesBusiness
    {
        private static string _connString = string.Empty;
        //private static string testConnString = string.Empty;
        public RulesBusiness()
        {
            var path = Environment.CurrentDirectory + "//ConfigurationFile.ini";
            var list = Tool.Helper.ReadTheLocalFile(path).Split('\n');
            //_connString = $"Data Source ={list[0].Split(':')[1]}; Initial Catalog = {list[1].Split(':')[1]}; User ID ={list[3].Split(':')[1]}; Password ={list[3].Split(':')[1]};PORT= 33060 ;Character Set=utf8; Allow User Variables=True";
            //_connString = $"Data Source =localhost; Initial Catalog = {list[8].Split(':')[1]}; User ID ={list[6].Split(':')[1]}; Password ={list[7].Split(':')[1]};Character Set=utf8; Allow User Variables=True";
            _connString = Tool.Helper.GetConfigValue("Lochost");
        }

        public Ruleinfo GetRuleinfo(int id)
        {
            var data = new Ruleinfo();
            try
            {
                var sql = $@"select  ID,OddsID,
            RuleType,
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
            CreationTime from ruleinfo where id={id}";

                var ds = DAL.DbHelper.MySqlQueryBySqlstring(sql, _connString);

                if (ds != null && ds.Tables.Count > 0)
                {
                    data = Tool.Helper.ToList<Ruleinfo>(ds.Tables[0]).FirstOrDefault(x => x.ID == id);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return data;
        }

        public DataTable GetRuleinfoList()
        {
            var data = new DataTable();
            try
            {
                var sql = @"select  
                ID '规则编号',
	(CASE WHEN RuleType = 1 THEN '断开后投注'
               WHEN RuleType = 0 THEN '默认'END) '规则类型',
			    IF(OddsID = 1,'规则1','规则2') as '赔率类型',
                OpenContent '开奖内容',
                JudgeCondition '判断投注条件',
                JudgeNumber '判断期数',
                BetContent '投注内容',
                NoWinBetNumber '不中期数',
                NoWinBetConent '不中后投注内容',
                StopProfit '止盈',
                StopLoss '只亏',
                intervalBetHours '间隔投注小时数',
                IF(IsTurnBet=0,'否','是') '是否转向投注',
                ProfitMultiple '盈利倍投',
                LossMultiple '亏损倍投',
                IF(IsProfitBetNow =0,'否','是') '盈利后立即下注',
                IF(IsLossBetNow =0,'否','是') '亏损后立即下注',
                IF(BetGearStop =0,'否','是') '倍投完后停止',
			    IF(CrackAfterBet  =0,'否','是') '断开后投注',
                CreationTime '创建时间'      
                from ruleinfo order by id desc";

                var ds = DAL.DbHelper.MySqlQueryBySqlstring(sql, _connString);

                if (ds != null && ds.Tables.Count > 0)
                {
                    //data = Tool.Helper.ToList<Ruleinfo>(ds.Tables[0]);
                    data = ds.Tables[0];
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return data;
        }

        public bool AddRules(Ruleinfo model)
        {
            bool reuslt;
            if (model.ID > 0)
            {
                reuslt = Update(model);
            }
            else
            {
                reuslt = Add(model);
            }
            return reuslt;
        }

        private bool Add(Ruleinfo model)
        {
            try
            {
                var sql = $@"insert into ruleinfo(
            OddsID,
            RuleType,
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
            CrackAfterBet,
            CreationTime)
            values
            (
            {model.OddsID}, 
            {model.RuleType},
            {model.OnlineBetID}, 
            '{model.OpenContent}', 
            '{model.JudgeCondition}', 
            {model.JudgeNumber}, 
            '{model.BetContent}',
            {model.NoWinBetNumber}, 
            '{model.NoWinBetConent}', 
            {model.StopProfit}, 
            {model.StopLoss},
            {model.StopBetHours}, 
            {model.intervalBetHours}, 
            {model.IsTurnBet}, 
            '{model.ProfitMultiple}', 
            '{model.LossMultiple}',
            {model.IsProfitBetNow},
            {model.IsLossBetNow},
            {model.BetGearStop}, 
            {model.CrackAfterBet},
            now()
                )";

                var result = DAL.DbHelper.MysqlExecuteSql(sql, _connString);

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

        private bool Update(Ruleinfo model)
        {
            try
            {
                var sql = $@"UPDATE ruleinfo SET
            OddsID = {model.OddsID},
            OnlineBetID = {model.OnlineBetID}, 
            OpenContent= '{model.OpenContent}',
            JudgeCondition = '{model.JudgeCondition}',
            JudgeNumber = {model.JudgeNumber},
            BetContent = '{model.BetContent}',
            NoWinBetNumber = {model.NoWinBetNumber},
            NoWinBetConent = '{model.NoWinBetConent}',
            StopProfit = {model.StopProfit},
            StopLoss = {model.StopLoss},
            StopBetHours = {model.StopBetHours},
            intervalBetHours = {model.intervalBetHours},
            IsTurnBet = {model.IsTurnBet},
            ProfitMultiple = '{model.ProfitMultiple}',
            LossMultiple = '{model.LossMultiple}',
            IsProfitBetNow = {model.IsProfitBetNow},
            IsLossBetNow = {model.IsLossBetNow},
            BetGearStop = {model.BetGearStop},
            CrackAfterBet = {model.CrackAfterBet},
            RuleType ={model.RuleType}
            WHERE ID = {model.ID}";

                var result = DAL.DbHelper.MysqlExecuteSql(sql, _connString);

                if (result > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
            }
            return true;
        }

        public bool DelRuleInfo(int id)
        {
            try
            {
                var sql = $"delete from ruleinfo where id = {id}";
                var result = DAL.DbHelper.MysqlExecuteSql(sql, _connString);

                if (result > 0)
                {
                    return true;
                }
            }
            catch (Exception e)
            {

            }
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

        public int RuleType { get; set; }

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
        /// 断开后投注
        /// </summary>
        public int CrackAfterBet { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 盈利倍投档位（扩展类型）
        /// </summary>
        public int ProfitMultipleLevel { get; set; }

        private int lossMultipleLevel = -1;

        /// <summary>
        /// 亏损倍投档位（扩展字段）
        /// </summary>
        public int LossMultipleLevel
        {
            get { return lossMultipleLevel; }
            set { lossMultipleLevel = value; }
        }
    }
}
