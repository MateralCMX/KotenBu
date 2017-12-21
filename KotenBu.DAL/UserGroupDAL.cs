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
    /// 用户组数据操作类
    /// </summary>
    public sealed class UserGroupDAL : BaseDAL<T_UserGroup, V_UserGroup>
    {
        /// <summary>
        /// 根据用户组代码和启用状态获得用户组信息
        /// </summary>
        /// <param name="code">用户组代码</param>
        /// <param name="ifEnable">启用标识</param>
        /// <returns>用户组信息</returns>
        public V_UserGroup GetUserGroupInfoByCodeAndEnable(string code, bool? ifEnable)
        {
            Expression<Func<V_UserGroup, bool>> expression = m => m.Code == code;
            if (ifEnable != null)
            {
                expression = LinqManager.And(expression, m => m.IfEnable == ifEnable.Value);
            }
            V_UserGroup resM = _DB.V_UserGroup.Where(expression.Compile()).FirstOrDefault();
            return resM;
        }
        /// <summary>
        /// 根据条件获得用户组信息
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="code">代码</param>
        /// <param name="ifEnable">启用标识</param>
        /// <param name="pageM">分页对象</param>
        /// <returns>用户组信息</returns>
        public List<V_UserGroup> GetUserGroupInfoByWhere(string name, string code, bool? ifEnable, MPagingModel pageM)
        {
            Expression<Func<V_UserGroup, bool>> expression = m => true;
            if (!string.IsNullOrEmpty(name))
            {
                expression = LinqManager.And(expression, m => m.Name.Contains(name));
            }
            if (!string.IsNullOrEmpty(code))
            {
                expression = LinqManager.And(expression, m => m.Code == code);
            }
            if (ifEnable != null)
            {
                expression = LinqManager.And(expression, m => m.IfEnable == ifEnable.Value);
            }
            pageM.DataCount = _DB.V_UserGroup.Count(expression.Compile());
            List<V_UserGroup> listM = null;
            if (pageM.DataCount > 0)
            {
                listM = _DB.V_UserGroup.Where(expression.Compile()).Skip((pageM.PagingIndex - 1) * pageM.PagingSize).Take(pageM.PagingSize).OrderBy(m => m.CreateTime).ToList();
            }
            return listM;
        }
        /// <summary>
        /// 根据唯一标识获得权限信息
        /// </summary>
        /// <param name="ID">唯一标识</param>
        /// <returns>权限信息</returns>
        public T_Permissions GetPermissionsInfoByID(Guid ID)
        {
            return _DB.T_Permissions.Where(m => m.ID == ID).FirstOrDefault();
        }
        /// <summary>
        /// 根据名称获得用户组信息
        /// </summary>
        /// <param name="Name">名称</param>
        /// <returns>用户组信息</returns>
        public List<V_UserGroup> GetUserGroupInfoByName(string name)
        {
            List<V_UserGroup> listM = (from m in _DB.V_UserGroup
                                       where m.Name.Contains(name)
                                       select m).ToList();
            return listM;
        }
    }
}
