using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KotenBu.BLL
{
    /// <summary>
    /// 业务处理类父类
    /// </summary>
    /// <typeparam name="TDAL">对应的数据操作类</typeparam>
    /// <typeparam name="TModel">对应的数据模型</typeparam>
    public class BaseBLL<TDAL, TModel, VModel> : BaseBLL<TDAL, TModel>
    {
        /// <summary>
        /// 根据唯一标识获得视图信息
        /// </summary>
        /// <param name="id">唯一标识</param>
        /// <returns></returns>
        public virtual VModel GetDBModelViewInfoByID(object id)
        {
            MethodInfo method = (typeof(TDAL)).GetMethod("GetDBModelViewInfoByID");
            if (method != null)
            {
                return (VModel)method.Invoke(_dal, new object[] { id });
            }
            else
            {
                throw new ApplicationException("未实现该方法，需重写");
            }
        }
    }
    /// <summary>
    /// 业务处理类父类
    /// </summary>
    /// <typeparam name="TDAL">对应的数据操作类</typeparam>
    /// <typeparam name="TModel">对应的数据模型</typeparam>
    public class BaseBLL<TDAL, TModel>
    {
        /// <summary>
        /// 数据操作对象
        /// </summary>
        public readonly TDAL _dal;
        /// <summary>
        /// 构造方法
        /// </summary>
        public BaseBLL()
        {
            ConstructorInfo[] constructors = typeof(TDAL).GetConstructors();
            ConstructorInfo constructor = (from m in constructors
                                           where m.GetParameters().Length == 0
                                           select m).FirstOrDefault();
            if (constructor != null)
            {
                _dal = (TDAL)constructor.Invoke(new object[0]);
            }
        }
        /// <summary>
        /// 验证模型
        /// </summary>
        /// <param name="model">要验证的模型</param>
        /// <returns>验证结果</returns>
        protected bool Verification(TModel model)
        {
            string msg = string.Empty;
            return Verification(model, ref msg);
        }
        /// <summary>
        /// 验证模型
        /// </summary>
        /// <param name="model">要验证的模型</param>
        /// <param name="msg">提示信息</param>
        /// <returns>验证结果</returns>
        protected virtual bool Verification(TModel model, ref string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                msg = "验证失败：" + msg.Substring(0, msg.Length - 1) + "。";
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 根据唯一标识获得信息
        /// </summary>
        /// <param name="id">唯一标识</param>
        /// <returns></returns>
        public virtual TModel GetDBModelInfoByID(object id)
        {
            MethodInfo method = (typeof(TDAL)).GetMethod("GetDBModelInfoByID");
            if (method != null)
            {
                return (TModel)method.Invoke(_dal, new object[] { id });
            }
            else
            {
                throw new ApplicationException("未实现该方法，需重写");
            }
        }
    }
}
