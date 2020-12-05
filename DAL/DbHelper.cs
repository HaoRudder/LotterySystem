using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DbHelper
    {
        private static readonly string connString = Tool.Helper.ReadConfiguration(Tool.ConfigurationType.AutomaticExportEntities);

        /// <summary>
        /// 根据Sql字符串查询MySql数据并返回数据集
        /// </summary>
        /// <param name="sqlString">Sql字符串</param>
        /// <param name="connString">数据库链接字符串</param>
        /// <returns></returns>
        public static DataSet MySqlQueryBySqlstring(string sqlString,string conn = "")
        {
            var strConn = string.IsNullOrWhiteSpace(conn) ? connString : conn;
            try
            {
                var ds = new DataSet();

                using (var con = new MySqlConnection(strConn))
                {
                    con.Open();

                    var da = new MySqlDataAdapter
                    {
                        SelectCommand = new MySqlCommand
                        {
                            Connection = con,
                            CommandText = sqlString,
                            CommandType = CommandType.Text,
                            CommandTimeout = 1800
                        }
                    };
                    da.Fill(ds, "data");
                    con.Close();
                }
                return ds;
            }
            catch (Exception ex)
            {
                return new DataSet();
            }
           
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="connString">数据库链接字符串</param>
        /// <param name="sqlStr">Sql字符串</param>
        public static int MysqlExecuteSql(string sqlStr, string conn = "")
        {
            var strConn  = string.IsNullOrWhiteSpace(conn) ? connString : conn;
            using (var con = new MySqlConnection(strConn))
            {
                con.Open();
                var command = con.CreateCommand();
                command.CommandText = sqlStr;
                command.CommandTimeout = 600;
                int rows = command.ExecuteNonQuery();
                con.Close();
                return rows;
            }
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="sqlStr"></param>
        /// <returns></returns>
        public static int SqlServerExecuteSql(string sqlStr, string conn = "")
        {
            var strConn =  string.IsNullOrWhiteSpace(conn) ? connString : conn;

            using (MySqlConnection connection = new MySqlConnection(strConn))
            {
                using (MySqlCommand cmd = new MySqlCommand(sqlStr, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (MySqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 分页查询方法
        /// </summary>
        /// <param name="tabName">表名</param>
        /// <param name="filed">查询字段（为空查询所有）</param>
        /// <param name="keyFiled">主键</param>
        /// <param name="pageIndex">当前页数</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="strWhere">查询条件</param>
        /// <param name="orderBy">排序</param>
        /// <param name="total">输出总共多少条</param>
        /// <returns>DataTable</returns>
        public static DataTable GetList(string tabName, string filed, string keyFiled, int pageIndex, int pageSize, string strWhere, string orderBy, out int total, string conn = "")
        {
            var dt = new DataTable();
            total = 0;
            if (string.IsNullOrEmpty(tabName) && pageSize > 0 && string.IsNullOrEmpty(keyFiled))
            {
                dt = null;
            }
            else
            {
                if (string.IsNullOrEmpty(filed)) { filed = " * "; }
                if (pageIndex == 0) { pageIndex = 1; }
                if (string.IsNullOrEmpty(strWhere)) { strWhere += " 1=1 "; }
                if (string.IsNullOrEmpty(orderBy)) { orderBy = keyFiled + " desc"; }

                var start = pageSize * (pageIndex - 1);
                var end = start + pageSize;

                string sql = $"select count({keyFiled}) as total from {tabName} where {strWhere} ";
                try
                {
                    using (var ds = MySqlQueryBySqlstring(sql))
                    {
                        var d1 = ds.Tables[0];
                        total = Convert.ToInt32(d1.Rows[0][0]);
                        if (total > 0)
                        {
                            StringBuilder sbsql = new StringBuilder();
                            sbsql.Append("select " + filed + " from " + tabName + " where " + strWhere + " order by "
                                + orderBy + "," + keyFiled + " desc limit " + start + "," + pageSize + " ");
                            dt = MySqlQueryBySqlstring(sbsql.ToString()).Tables[0];
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return dt;
        }
        
    }
}
