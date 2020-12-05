using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tool;

namespace BLL.AutomaticBetting
{
    /// <summary>
    /// 南宫
    /// </summary>
    public class NGAutomaticBetting
    {

        /// <summary>
        /// 自动投注
        /// </summary>
        /// <param name="content">投注内容</param>
        /// <param name="amount">投注金额</param>
        /// <param name="room">投注房间</param>
        /// <returns></returns>
        public static string Betting(List<string> content, int amount, string room, out string currentAmount)
        {
            currentAmount = string.Empty;
            HttpPostData("http://m.ng103.com/index/login.html", "");//请求网站首页获取cookie

            //获取token
            var result = HttpPostData("http://m.ng103.com/index/calltoken.html", "");
            if (result.Contains("远程服务器返回错误"))
            {
                return "投注网站服务器问题导致 投注失败";
            }
            var token = string.Empty;
            if (!string.IsNullOrWhiteSpace(result))
            {
                var index = result.IndexOf(">");
                var strIndex = result.IndexOf("value=\\") + "value=\\".Length + 1;
                token = result.Substring(strIndex, result.Length - index + "value=\\".Length - 1);
            }
            else
            {
                return "获取验证码token失败";
            }
            var accountAndPwd = ConfigurationManager.AppSettings["NGAccountAndPwd"]; //获取配置的账号密码
            if (string.IsNullOrWhiteSpace(accountAndPwd))
            {
                return "没有获取账号密码，请检查是否配置账号密码";
            }
            var username = accountAndPwd.Split('|')[0];
            var password = accountAndPwd.Split('|')[1];
            var login = HttpPostData("http://m.ng103.com/index/dologin.html", $"username={username}&password={password}&verify=&token={token}"); //登录
            var success = login.Replace("{", "").Replace("}", "").Split(',')[3].Split(':')[1].Contains("success");

            bool isBetting;
            if (success)
            {
                var temp = HttpPostData(MatchingRoom(room), MatchingContent(content, amount));
                isBetting = temp.Contains("success");
            }
            else
            {
                return "登录失败";
            }

            if (isBetting)
            {
                var html = HttpGetData("http://m.ng103.com/user/accmoney.html", string.Empty); //登录
                if (html.Contains("请填写验证码"))
                {
                    HttpPostData("http://m.ng103.com/index/login.html", "");//请求网站首页获取cookie
                    result = HttpPostData("http://m.ng103.com/index/calltoken.html", "");
                    if (result.Contains("远程服务器返回错误"))
                    {
                        return "投注网站服务器问题导致 投注失败";
                    }
                    token = string.Empty;
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        var index = result.IndexOf(">");
                        var strIndex = result.IndexOf("value=\\") + "value=\\".Length + 1;
                        token = result.Substring(strIndex, result.Length - index + "value=\\".Length - 1);
                    }
                    login = HttpPostData("http://m.ng103.com/index/dologin.html", $"username={username}&password={password}&verify=&token={token}"); //登录
                    html = HttpGetData("http://m.ng103.com/user/accmoney.html", string.Empty); //登录
                }
                var str = "mw_money";
                var tempHtml = html.Substring(html.IndexOf(str), html.Length - html.IndexOf(str));
                currentAmount = tempHtml.Substring(tempHtml.IndexOf(">") + 1, tempHtml.IndexOf("<") - tempHtml.IndexOf(">") - 1);
                return string.Empty;
            }
            return "投注失败";
        }

        static CookieContainer cookie = new CookieContainer();
        /// <summary>
        /// POST请求（带参，带cookie）
        /// </summary>
        /// <param name="request"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private static string HttpPostData(string url, string param)
        {
            try
            {
                //注意提交的编码 这边是需要改变的 这边默认的是Default：系统当前编码
                byte[] postData = Encoding.UTF8.GetBytes(param);

                // 设置提交的相关参数 
                var request = WebRequest.Create(url) as HttpWebRequest;
                request.Accept = "*/*";
                request.KeepAlive = true;
                request.ContentLength = postData.Length;
                request.CookieContainer = cookie;
                request.Host = "m.ng103.com";
                request.Referer = "http://m.ng103.com/game/index.html";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("Accept-Encoding", "gzip,deflate");
                request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
                request.Headers.Add("X-Requested-With", "XMLHttpRequest");
                request.Method = "POST";
                request.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1";

                // 提交请求数据 
                var outputStream = request.GetRequestStream();
                outputStream.Write(postData, 0, postData.Length);
                outputStream.Close();

                var response = request.GetResponse() as HttpWebResponse;
                response.Cookies = cookie.GetCookies(response.ResponseUri);
                var responseStream = response.GetResponseStream();
                var reader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8"));
                var srcString = reader.ReadToEnd();
                var result = srcString;
                reader.Close();

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private static string HttpGetData(string url, string param)
        {
            try
            {
                //注意提交的编码 这边是需要改变的 这边默认的是Default：系统当前编码
                byte[] postData = Encoding.UTF8.GetBytes(param);

                // 设置提交的相关参数 
                var request = WebRequest.Create(url) as HttpWebRequest;
                request.Accept = "text/html, */*; q=0.01";
                request.KeepAlive = true;
                request.ContentLength = postData.Length;
                request.CookieContainer = cookie;
                request.Host = "m.ng103.com";
                request.Referer = "http://m.ng103.com/index/login.html";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("Accept-Encoding", "UTF-8,deflate");
                request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
                request.Headers.Add("X-Requested-With", "XMLHttpRequest");
                request.Method = "GET";
                request.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) CriOS/56.0.2924.75 Mobile/14E5239e Safari/602.1";

                //// 提交请求数据 
                //var outputStream = request.GetRequestStream();
                //outputStream.Write(postData, 0, postData.Length);
                //outputStream.Close();

                var response = request.GetResponse() as HttpWebResponse;
                response.Cookies = cookie.GetCookies(response.ResponseUri);
                var responseStream = response.GetResponseStream();
                var reader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8"));
                var srcString = reader.ReadToEnd();
                var result = srcString;
                reader.Close();

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        /// <summary>
        /// 查询余额
        /// </summary>
        /// <returns></returns>
        public static string QueryForTheLatestBalance()
        {
            try
            {
                var html = HttpGetData("http://m.ng103.com/user/accmoney.html", string.Empty); //登录
                HttpPostData("http://m.ng103.com/index/login.html", "");//请求网站首页获取cookie
                var result = HttpPostData("http://m.ng103.com/index/calltoken.html", "");
                if (result.Contains("远程服务器返回错误"))
                {
                    return "投注网站服务器问题导致 投注失败";
                }
                var token = string.Empty;
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var index = result.IndexOf(">");
                    var strIndex = result.IndexOf("value=\\") + "value=\\".Length + 1;
                    token = result.Substring(strIndex, result.Length - index + "value=\\".Length - 1);
                }

                var accountAndPwd = ConfigurationManager.AppSettings["NGAccountAndPwd"]; //获取配置的账号密码
                if (string.IsNullOrWhiteSpace(accountAndPwd))
                {
                    return "没有获取账号密码，请检查是否配置账号密码";
                }
                var username = accountAndPwd.Split('|')[0];
                var password = accountAndPwd.Split('|')[1];

                var login = HttpPostData("http://m.ng103.com/index/dologin.html", $"username={username}&password={password}&verify=&token={token}"); //登录
                html = HttpGetData("http://m.ng103.com/user/accmoney.html", string.Empty); //登录

                var str = "mw_money";
                var tempHtml = html.Substring(html.IndexOf(str), html.Length - html.IndexOf(str));
                var currentAmount = tempHtml.Substring(tempHtml.IndexOf(">") + 1, tempHtml.IndexOf("<") - tempHtml.IndexOf(">") - 1);
                return currentAmount;
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        #region 匹配参数

        /// <summary>
        /// 匹配下注内容
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="amount">金额</param>
        /// <returns></returns>
        private static string MatchingContent(List<string> content, int amount)
        {
            var parList = "tp100=&tp101=0&tp102=0&tp103=0&tp104=0&tp105=0&tp106=0&tp107=0&tp108=0&tp109=0&tp110=0&tp140=0&tp141=0&tp142=0&tp143=0&tp144=0&tp145=0".Split('&').ToList();
            var str = string.Empty;
            foreach (var item in content)
            {
                switch (item)
                {
                    case "小": parList[parList.IndexOf("tp101=0")] = $"tp101={amount}"; break;
                    case "大": parList[parList.IndexOf("tp102=0")] = $"tp102={amount}"; break;
                    case "单": parList[parList.IndexOf("tp103=0")] = $"tp103={amount}"; break;
                    case "双": parList[parList.IndexOf("tp104=0")] = $"tp104={amount}"; break;
                    case "小单": parList[parList.IndexOf("tp105=0")] = $"tp105={amount}"; break;
                    case "大单": parList[parList.IndexOf("tp106=0")] = $"tp106={amount}"; break;
                    case "小双": parList[parList.IndexOf("tp106=0")] = $"tp107={amount}"; break;
                    case "大双": parList[parList.IndexOf("tp108=0")] = $"tp108={amount}"; break;
                    case "极小": parList[parList.IndexOf("tp109=0")] = $"tp109={amount}"; break;
                    case "极大": parList[parList.IndexOf("tp110=0")] = $"tp110={amount}"; break;
                    case "豹子": parList[parList.IndexOf("tp140=0")] = $"tp140={amount}"; break;
                    case "顺子": parList[parList.IndexOf("tp141=0")] = $"tp141={amount}"; break;
                    case "对子": parList[parList.IndexOf("tp142=0")] = $"tp142={amount}"; break;
                }
            }
            str = parList.Aggregate(string.Empty, (current, item) => current + "&" + item);
            return str.TrimStart('&');
        }

        /// <summary>
        /// 匹配房间
        /// </summary>
        /// <param name="roomName"></param>
        /// <returns></returns>
        private static string MatchingRoom(string roomName)
        {
            var url = string.Empty;

            switch (roomName)
            {
                case "加拿大2.0倍": url = "Canada/game28_buy20"; break;
                case "加拿大1.88倍": url = "Canada/game28_buy188"; break;
                case "加拿大2.5倍": url = "Canada/game28_buy25"; break;
                case "加拿大2.8倍": url = "Canada/game28_buy27"; break;
                case "加拿大3.2倍": url = "Canada/game28_buy32"; break;
                case "北京2倍": url = "Game28/game28_buy20"; break;
                case "北京2.5倍": url = "Game28/game28_buy25"; break;
            }
            return "http://m.ng103.com/" + url;
        }

        #endregion
    }
}
