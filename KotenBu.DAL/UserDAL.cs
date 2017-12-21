using KotenBu.Model;
using MateralTools.MLinq;
using MateralTools.MResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KotenBu.DAL
{
    /// <summary>
    /// 用户数据操作类
    /// </summary>
    public sealed class UserDAL : BaseDAL<T_User, V_User>
    {
        /// <summary>
        /// 根据登录名获得用户信息
        /// </summary>
        /// <param name="userName">登录名</param>
        /// <returns>用户信息</returns>
        public List<T_User> GetUserInfoByLoginUserName(string userName)
        {
            List<T_User> resM = _DB.T_User.Where(m => m.UserName == userName || m.Email == userName || m.Mobile == userName).ToList();
            return resM;
        }
        /// <summary>
        /// 根据用户名获得用户信息
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>用户信息</returns>
        public V_User GetUserViewInfoByUserName(string userName)
        {
            V_User resM = _DB.V_User.Where(m => m.UserName == userName).FirstOrDefault();
            return resM;
        }
        /// <summary>
        /// 根据真实姓名获得用户信息
        /// </summary>
        /// <param name="trueName">真实姓名</param>
        /// <returns>用户信息</returns>
        public List<V_User> GetUserViewInfoByTrueName(string trueName)
        {
            List<V_User> listM = (from m in _DB.V_User
                                  where m.TrueName.Contains(trueName)
                                  orderby m.CreateTime
                                  select m).ToList();
            return listM;
        }
        /// <summary>
        /// 根据邮箱获得用户信息
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <returns>用户信息</returns>
        public V_User GetUserViewInfoByEmail(string email)
        {
            V_User resM = _DB.V_User.Where(m => m.Email == email).FirstOrDefault();
            return resM;
        }
        /// <summary>
        /// 根据手机号码获得用户信息
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <returns>用户信息</returns>
        public V_User GetUserViewInfoByMobile(string mobile)
        {
            V_User resM = _DB.V_User.Where(m => m.Mobile == mobile).FirstOrDefault();
            return resM;
        }
        /// <summary>
        /// 根据OpenID获得用户信息
        /// </summary>
        /// <param name="openID">OpenID</param>
        /// <returns>用户信息</returns>
        public V_User GetUserViewInfoByWeChatOpenID(string openID)
        {
            V_User resM = _DB.V_User.Where(m => m.WeChatOpenID == openID).FirstOrDefault();
            return resM;
        }
        /// <summary>
        /// 根据条件获得用户信息
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="mobile">手机</param>
        /// <param name="email">邮箱</param>
        /// <param name="trueName">真实姓名</param>
        /// <param name="nickName">昵称</param>
        /// <param name="ifEnable">启用标识</param>
        /// <param name="pageM">分页对象</param>
        /// <returns>用户信息</returns>
        public List<V_User> GetUserViewInfoByWhere(string userName, string mobile, string email, string trueName, string nickName, bool? ifEnable, MPagingModel pageM)
        {
            Expression<Func<V_User, bool>> expression = m => true;

            if (!string.IsNullOrEmpty(userName))
            {
                expression = LinqManager.And(expression, m => m.UserName == userName);
            }
            if (!string.IsNullOrEmpty(mobile))
            {
                expression = LinqManager.And(expression, m => m.Mobile == mobile);
            }
            if (!string.IsNullOrEmpty(email))
            {
                expression = LinqManager.And(expression, m => m.Email == email);
            }
            if (!string.IsNullOrEmpty(trueName))
            {
                expression = LinqManager.And(expression, m => m.TrueName.Contains(trueName));
            }
            if (!string.IsNullOrEmpty(nickName))
            {
                expression = LinqManager.And(expression, m => m.NickName.Contains(nickName));
            }
            if (ifEnable != null)
            {
                expression = LinqManager.And(expression, m => m.IfEnable == ifEnable.Value);
            }
            pageM.DataCount = _DB.V_User.Count(expression.Compile());
            List<V_User> listM = null;
            if (pageM.DataCount > 0)
            {
                listM = _DB.V_User.Where(expression.Compile()).Skip((pageM.PagingIndex - 1) * pageM.PagingSize).Take(pageM.PagingSize).OrderBy(m => m.CreateTime).ToList();
            }
            return listM;
        }
        /// <summary>
        /// 根据唯一标识获得用户组信息
        /// </summary>
        /// <param name="id">唯一标识</param>
        /// <returns>用户组信息</returns>
        public T_UserGroup GetUserGroupInfoByID(Guid id)
        {
            return _DB.T_UserGroup.Where(m => m.ID == id).FirstOrDefault();
        }
        /// <summary>
        /// 根据唯一标识组获得用户组信息
        /// </summary>
        /// <param name="ids">唯一标识</param>
        /// <returns>用户组信息</returns>
        public List<V_UserGroup> GetUserGroupViewInfoByIDs(Guid[] ids)
        {
            return _DB.V_UserGroup.Where(m => ids.Contains(m.ID)).ToList();
        }
        /// <summary>
        /// 根据用户唯一标识组获得用户信息
        /// </summary>
        /// <param name="ids">唯一标识组</param>
        /// <returns>用户信息</returns>
        public List<V_User> GetUserViewInfoByIDs(Guid[] ids)
        {
            List<V_User> listM = (from m in _DB.V_User
                                  where ids.Contains(m.ID)
                                  select m).ToList();
            return listM;
        }
    }
}
