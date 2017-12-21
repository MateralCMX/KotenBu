using KotenBu.DAL;
using KotenBu.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KotenBu.BLL
{
    /// <summary>
    /// 应用程序日志业务类
    /// </summary>
    public sealed class ApplicationLogBLL : BaseBLL<ApplicationLogDAL, T_ApplicationLog>
    {
        /// <summary>
        /// 应用程序日志数据操作对象
        /// </summary>
        private readonly static ApplicationLogDAL _applicationLogDAL = new ApplicationLogDAL();
        /// <summary>
        /// 写入异常日志
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="contents">内容</param>
        public static void WriteExceptionLog(string title, string contents)
        {
            WriteLog(title, contents, ApplicationLogTypeEnum.Exception);
        }
        /// <summary>
        /// 写入操作日志
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="contents">内容</param>
        public static void WriteOptionsLog(string title, string contents)
        {
            WriteLog(title, contents, ApplicationLogTypeEnum.Options);
        }
        /// <summary>
        /// 写入调试日志
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="contents">内容</param>
        public static void WriteDebugLog(string title, string contents)
        {
            WriteLog(title, contents, ApplicationLogTypeEnum.Debug);
        }
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="contents">内容</param>
        /// <param name="types">类型</param>
        private static void WriteLog(string title, string contents, ApplicationLogTypeEnum types)
        {
            T_ApplicationLog logM = new T_ApplicationLog
            {
                Types = (byte)types,
                CreateTime = DateTime.Now,
                Title = title,
                Contents = contents
            };
            _applicationLogDAL.Insert(logM);
        }
    }
}
