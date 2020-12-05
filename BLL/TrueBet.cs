using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class TrueBet
    {

        /// <summary>
        /// 真实投注
        /// </summary>
        /// <param name="content">内容集合</param>
        /// <param name="amount">金额</param>
        /// <param name="website">网站</param>
        /// <param name="room">房间</param>
        /// <returns></returns>
        public static string Betting(List<string> content, int amount, string website, string room,out string currentAmount)
        {
            currentAmount = string.Empty;
            var result = string.Empty;
            if (website.Contains("南宫"))
            {
                result = AutomaticBetting.NGAutomaticBetting.Betting(content, amount, room,out currentAmount);//南宫投注
            }
            return result;
        }

        /// <summary>
        /// 查询余额
        /// </summary>
        /// <param name="website">网站</param>
        /// <returns></returns>
        public static string QueryForTheLatestBalance(string website)
        {
            var result = string.Empty;
            if (website.Contains("南宫"))
            {
                result = AutomaticBetting.NGAutomaticBetting.QueryForTheLatestBalance();//查询余额
            }
            return result;
        }
    }
}
