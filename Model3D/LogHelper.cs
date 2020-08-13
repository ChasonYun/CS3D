using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Public
{
    public class LogHelper
    {
        #region 单例模式
        private static readonly LogHelper instance = null;
        static LogHelper()
        {
            instance = new LogHelper();
        }
        private LogHelper()
        {
        }
        public static LogHelper Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        #region enum
        /// <summary>
        /// 日志等级
        /// </summary>
        public enum LogLevel
        {
            SysInfo,
            Info,
            Exception
        }

        /// <summary>
        /// 日志类型
        /// </summary>
        public enum LogType
        {
            SysInfoLog,
            InfoLog,
            ExceptionLog
        }
        #endregion

        #region object
        /// <summary>
        /// 日志接口
        /// </summary>
        private log4net.ILog m_Log;
        private object obj = new object();
        #endregion


        /// <summary>
        /// 异常日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logLevel"></param>
        public void Exception(string message, LogLevel logLevel = LogLevel.Exception)
        {
            lock (obj)
            {
                m_Log = log4net.LogManager.GetLogger(LogType.ExceptionLog.ToString());
                WriteLog(logLevel, message);
            }
        }

        /// <summary>
        /// 系统日志
        /// </summary>
        /// <param name="message">输出的消息</param>
        public void SysInfo(string message, LogLevel logLevel = LogLevel.SysInfo)
        {
            lock (obj)
            {
                m_Log = log4net.LogManager.GetLogger(LogType.SysInfoLog.ToString());
                WriteLog(logLevel, message);
            }
        }

        /// <summary>
        /// 信息日志
        /// </summary>
        /// <param name="message">输出的消息</param>
        public void Info(string message, LogLevel logLevel = LogLevel.Info)
        {
            lock (obj)
            {
                m_Log = log4net.LogManager.GetLogger(LogType.InfoLog.ToString());
                WriteLog(logLevel, message);
            }
        }

        /// <summary>
        /// 记录系统日志
        /// </summary>
        /// <param name="logLevel">日志级别</param>
        /// <param name="message">输出的消息</param>
        private void WriteLog(LogLevel logLevel, string message)
        {
            switch (logLevel)
            {
                case LogLevel.Info:
                    m_Log.Info(message);
                    break;
                case LogLevel.Exception:
                    m_Log.Error(message);
                    break;
                case LogLevel.SysInfo:
                    m_Log.Info(message);
                    break;
            }

        }
    }
}
