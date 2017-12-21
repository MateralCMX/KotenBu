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
    /// Token数据操作类
    /// </summary>
    public sealed class TokenDAL : BaseDAL<T_Token>
    {
        /// <summary>
        /// 根据用户唯一编号获得令牌信息
        /// </summary>
        /// <param name="userID">用户唯一标识</param>
        /// <returns>令牌信息</returns>
        public List<T_Token> GetTokenInfoByUserID(Guid userID)
        {
            List<T_Token> resM = _DB.T_Token.Where(m => m.FK_User == userID).ToList();
            return resM;
        }
        /// <summary>
        /// 根据用户唯一编号和令牌类型获得令牌信息
        /// </summary>
        /// <param name="userID">用户唯一标识</param>
        /// <param name="tokenType">Token类型</param>
        /// <returns>令牌信息</returns>
        public T_Token GetTokenInfoByUserIDAndTokenType(Guid userID, TokenTypeEnum tokenType)
        {
            T_Token resM = _DB.T_Token.Where(m => m.FK_User == userID && m.TokenType == (byte)tokenType).FirstOrDefault();
            return resM;
        }
    }
}
