using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Public;
using System.Data;

namespace CS3D
{
    public class MysqlDBHandler
    {
        private MysqlDBHandler()
        {
            MySqlCon.HintEvent += new MySqlCon.HintEventHandler(GetHint);
        }
        #region 单例
        private static MysqlDBHandler instance;
        public static MysqlDBHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    return instance = new MysqlDBHandler();
                }
                return instance;
            }
        }
        #endregion
        /// <summary>
        /// 异常信息记录
        /// </summary>
        /// <param name="value"></param>
        private void GetHint(string value)
        {
            LogHelper.Instance.Exception(value);
        }

        /// <summary>
        /// 检测mysql连接状态
        /// </summary>
        /// <returns></returns>
        private bool CheckMysqlConnState()
        {
            return MySqlCon.checkSqlCon();
        }

        /// <summary>
        /// 查询异常信息
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="type"></param>
        public DataTable QueryFaultInfo(DateTime beginTime, DateTime endTime, int type)
        {
            string sql = "";
            try
            {
                DataTable dt = new DataTable();
                DataSet ds = new DataSet();
                sql = string.Format("select *from alarmInfo where Time BETWEEN '{0}' AND '{1}'", beginTime, endTime);
                if (type > -1)
                {
                    sql += string.Format("and Type='{0}'", type);
                }
                ds = MySqlCon.Query(sql);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            dt = ds.Tables[0];
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Exception(ex.ToString() + " :" + sql);
                return null;
            }
        }

        public DataTable GetStoreHouseState()
        {
            try
            {
                string sql = string.Format("select *from `db_storehouse`");
                return MySqlCon.Query(sql).Tables[0];
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Exception(ex.ToString());
                return null;
            }
          
        }

        /// <summary>
        /// 根据货架号拿到一个仓库记录
        /// </summary>
        /// <param name="shelfNo"></param>
        /// <returns></returns>
        public DataTable GetOneCellState(string shelfNo)
        {
            try
            {
                string sql = string.Format("select *from `db_storehouse` where ShelfNo={0}",shelfNo);
                return MySqlCon.Query(sql).Tables[0];
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Exception(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 获取有货的记录
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllCellHaveThing()
        {
            try
            {
                string sql = string.Format("select *from `db_storehouse` where ShelfState=1");
                return MySqlCon.Query(sql).Tables[0];
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Exception(ex.ToString());
                return null;
            }
        }


        /// <summary>
        /// 获取出入库任务清单
        /// </summary>
        /// <param name="dt_beginTime"></param>
        /// <param name="dt_endTime"></param>
        /// <param name="barcode"></param>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        public DataTable GetProInOutInfo(DateTime dt_beginTime, DateTime dt_endTime, string ProId, string exceptions)
        {
            string sqlstr = "";
            try
            {
                DataTable dt = new DataTable();
                DataSet ds = new DataSet();
                sqlstr = string.Format("select * from `proinoutlist` where Time BETWEEN '{0}' and '{1}' and ProId like '%{2}%' and Exception like '%{3}%'",
                    dt_beginTime, dt_endTime, ProId, exceptions);
                ds = MySqlCon.Query(sqlstr);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            dt = ds.Tables[0].Copy();
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Exception(ex.ToString() + " :" + sqlstr);
                return null;
            }
        }

        /// <summary>
        /// 获取指定时间的任务数量
        /// </summary>
        /// <param name="shorttime"></param>
        /// <returns></returns>
        public DataTable GetTotalTaskNum(string shorttime)
        {
            string sqlstr = "";
            try
            {
                DataTable dt = new DataTable();
                DataSet ds = new DataSet();
                sqlstr = string.Format("select * from `db_tasksnum` where `ExecutionTime` = '{0}'", shorttime);

                ds = MySqlCon.Query(sqlstr);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            dt = ds.Tables[0].Copy();
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Exception(ex.ToString() + " :" + sqlstr);
                return null;
            }
        }
    }
}
