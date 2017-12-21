using KotenBu.Model;
using MateralTools.MLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KotenBu.DAL
{
    /// <summary>
    /// 权限数据操作类
    /// </summary>
    public sealed class PermissionsDAL : BaseDAL<T_Permissions, V_Permissions>
    {
        /// <summary>
        /// 根据权限代码和启用状态获得权限信息
        /// </summary>
        /// <param name="code">权限代码</param>
        /// <param name="ifEnable">启用标识</param>
        /// <returns>权限信息</returns>
        public V_Permissions GetPermissionsInfoByCodeAndEnable(string code, bool? ifEnable)
        {
            Expression<Func<V_Permissions, bool>> expression = m => m.Code == code;
            if (ifEnable != null)
            {
                expression = LinqManager.And(expression, m => m.IfEnable == ifEnable.Value);
            }
            V_Permissions resM = _DB.V_Permissions.Where(expression.Compile()).FirstOrDefault();
            return resM;
        }
        /// <summary>
        /// 根据类型和启用标识获得顶层权限信息
        /// </summary>
        /// <param name="type">权限类型</param>
        /// <param name="ifEnable">启用标识</param>
        /// <returns>所有权限信息</returns>
        public List<T_Permissions> GetPermissionsInfoByTypeAndEnable(PermissionsTypesEnum type, bool? ifEnable)
        {
            Expression<Func<T_Permissions, bool>> expression = m => m.Type == (byte)type && !m.IfDelete && m.FK_ParentID == null;
            if (ifEnable != null)
            {
                expression = LinqManager.And(expression, m => m.IfEnable == ifEnable.Value);
            }
            List<T_Permissions> listM = _DB.T_Permissions.Where(expression.Compile()).OrderByDescending(m => m.Ranks).ToList();
            return listM;
        }
        /// <summary>
        /// 获得最大的Rank值
        /// </summary>
        /// <returns>Rank值</returns>
        public int GetMaxRank()
        {
            int Rank = 0;
            if (_DB.T_Permissions.Count() > 0)
            {
                Rank = _DB.T_Permissions.Max(m => m.Ranks);
            }
            return Rank;
        }
    }
}
