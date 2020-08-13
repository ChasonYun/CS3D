using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;

namespace CS3D
{
    public class MySqlCon
    {
        #region 提示信息
        public delegate void HintEventHandler(string msg);
        /// <summary>
        /// 提示信息
        /// </summary>
        /// <param name="state"></param>
        public static event HintEventHandler HintEvent;
        #endregion

        #region variable
        public static string mysqlStr = "";
        static string mySqlXmlPath = "/Config/MySqlParam/";
        static string DataBase = string.Empty;
        static string DataSource = string.Empty;
        static string UserId = string.Empty;
        static string Passsword = string.Empty;
        #endregion

        /// <summary>
        /// 建立mysql数据库链接
        /// </summary>
        /// <returns></returns>
        public static MySqlConnection getMySqlCon()
        {
            try
            {
                if (string.IsNullOrEmpty(DataBase))
                    DataBase = XmlHelper.Instance.GetXMLInformation(mySqlXmlPath + "DataBase");
                if (string.IsNullOrEmpty(DataSource))
                    DataSource = XmlHelper.Instance.GetXMLInformation(mySqlXmlPath + "DataSource");
                if (string.IsNullOrEmpty(UserId))
                    UserId = XmlHelper.Instance.GetXMLInformation(mySqlXmlPath + "UserId");
                if (string.IsNullOrEmpty(Passsword))
                {
                    Passsword = XmlHelper.Instance.GetXMLInformation(mySqlXmlPath + "Passsword");
                    // Passsword=MD5Handler.Instance.MD5Decrypt(Passsword);
                }

                String mysqlStr = "Database=" + DataBase + ";Data Source=" + DataSource + ";User Id=" + UserId + ";Password=" + Passsword + ";pooling=true;CharSet=utf8;port=3306";
                MySqlConnection mysql = new MySqlConnection(mysqlStr);
                return mysql;
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库连接失败：" + ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 建立执行命令语句对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="mysql"></param>
        /// <returns></returns>
        public static MySqlCommand getSqlCommand(String sql, MySqlConnection mysql)
        {
            MySqlCommand mySqlCommand = new MySqlCommand(sql, mysql);
            mySqlCommand.CommandTimeout = 2500;
            return mySqlCommand;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="sqlstr"></param>
        /// <returns></returns>
        public static bool SqlCommand(string sqlstr)
        {
            //MySqlDataReader reader = null;
            MySqlConnection mysqlCon = getMySqlCon();
            MySqlCommand mySqlCommand = getSqlCommand(sqlstr, mysqlCon);
            try
            {
                // mysqlCon.BeginTransaction ();
                mysqlCon.Open();
                //reader = mySqlCommand.ExecuteReader();
                //while (reader.Read())
                //{ }
                //if (reader.HasRows == false)
                //{
                //    MessageBox.Show("信息不存在");
                //}
                mySqlCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                String message = ex.Message;
                if (HintEvent != null) HintEvent(string.Format("SqlCommand has exception:'{0}',sql='{1}'", ex.ToString(), sqlstr));
                return false;
            }
            finally
            {
                //if (reader != null)
                //    reader.Close();
                if (mysqlCon.State == System.Data.ConnectionState.Open || mysqlCon.State == System.Data.ConnectionState.Broken)
                {
                    mysqlCon.Close();
                    mysqlCon.Dispose();

                }

            }
        }

        /// <summary>
        /// 创建mysql数据库
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public static bool CreateDataBase(string dbName)
        {
            if (string.IsNullOrEmpty(DataSource))
                DataSource = XmlHelper.Instance.GetXMLInformation(mySqlXmlPath + "DataSource");
            if (string.IsNullOrEmpty(UserId))
                UserId = XmlHelper.Instance.GetXMLInformation(mySqlXmlPath + "UserId");
            if (string.IsNullOrEmpty(Passsword))
            {
                Passsword = XmlHelper.Instance.GetXMLInformation(mySqlXmlPath + "Passsword");
                // Passsword = MD5Handler.Instance.MD5Decrypt(Passsword);
            }
            string sql = string.Format("CREATE DATABASE IF NOT EXISTS {0} DEFAULT CHARSET utf8 COLLATE utf8_general_ci;", dbName);
            String mysqlStr = "Data Source=" + DataSource + ";Persist Security Info=yes;" + "User Id=" + UserId + ";Password=" + Passsword + ";pooling=true;CharSet=utf8;port=3306";
            MySqlConnection conn = new MySqlConnection(mysqlStr);
            MySqlCommand cmd = new MySqlCommand(sql, conn);

            //防止第二次启动时再次新建数据库
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch (Exception)
            {
                conn.Close();
                return false;
            }
            //防止第二次启动时再次新建数据库
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString)
        {
            DataSet ds = null;
            using (MySqlConnection mysql = getMySqlCon())
            {
                try
                {
                    mysql.Open();
                    MySqlDataAdapter command = new MySqlDataAdapter(SQLString, mysql);
                    ds = new DataSet();
                    command.Fill(ds);
                }
                catch (Exception ex)
                {
                    //throw new Exception(ex.Message);
                    Console.WriteLine(ex.ToString());
                    if (HintEvent != null) HintEvent("Query data has exception:" + ex.ToString());
                }
                finally
                {
                    mysql.Close();
                }
            }
            return ds;
        }

        /// <summary>
        /// 判断能否连接上mySql
        /// </summary>
        /// <returns></returns>
        public static bool checkSqlCon()
        {
            MySqlConnection mysqlCon = getMySqlCon();
            try
            {
                mysqlCon.Open();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            finally
            {
                mysqlCon.Close();
            }
        }
    }
}
