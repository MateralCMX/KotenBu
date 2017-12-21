using KotenBu.DAL;
using KotenBu.Model;
using MateralTools.MEncryption;
using MateralTools.MVerify;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KotenBu.BLL
{
    public sealed class TokenBLL : BaseBLL<TokenDAL, T_Token>
    {
        #region 成员
        /// <summary>
        /// Token有效时间[分钟]
        /// </summary>
        private static double TokenOverdue = 1440;
        #endregion
        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public TokenBLL()
        {
            string tokenOverdue = ConfigurationManager.AppSettings["TokenOverdue"];
            if (!string.IsNullOrEmpty(tokenOverdue) && VerifyManager.IsInteger(tokenOverdue))
            {
                TokenOverdue = Convert.ToDouble(tokenOverdue);
            }
        }
        #endregion
        #region 公共方法
        /// <summary>
        /// 获得新的Token
        /// </summary>
        /// <param name="userID">用户唯一标识</param>
        /// <param name="tokenType">Token类型</param>
        /// <returns></returns>
        public T_Token GetNewToken(Guid userID, TokenTypeEnum tokenType)
        {
            T_Token tokenM = _dal.GetTokenInfoByUserIDAndTokenType(userID, tokenType);
            if (tokenM == null)
            {
                tokenM = new T_Token
                {
                    CreateTime = DateTime.Now,
                    FK_User = userID,
                    Token = GetNewToken(),
                    TokenType = (byte)tokenType
                };
                _dal.Insert(tokenM);
            }
            else
            {
                tokenM.Token = GetNewToken();
                tokenM.CreateTime = DateTime.Now;
                _dal.SaveChange();
            }
            return tokenM;
        }
        /// <summary>
        /// Token是否有效
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="token">Token内容</param>
        /// <param name="tokenType">Token类型</param>
        /// <returns>检测结果</returns>
        public bool TokenValid(Guid userID, string token, TokenTypeEnum tokenType)
        {
            bool resM = false;
            T_Token model = _dal.GetTokenInfoByUserIDAndTokenType(userID, tokenType);
            if (model != null)
            {
                if (model.Token.Equals(token))
                {
                    if (model.CreateTime.AddMinutes(TokenOverdue) >= DateTime.Now)
                    {
                        resM = true;
                    }
                }
            }
            else
            {
                throw new ArgumentException($"参数{nameof(userID)}错误。");
            }
            return resM;
        }
        #endregion
        #region 私有方法
        /// <summary>
        /// 获得新的Token
        /// </summary>
        /// <returns>Token值</returns>
        private string GetNewToken()
        {
            string lib = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random rd = new Random();
            int Count = rd.Next(15, 32);
            string token = "";
            for (int i = 0; i < Count; i++)
            {
                token += lib[rd.Next(0, lib.Length)];
            }
            return EncryptionManager.MD5Encode_32(token);
        }
        #endregion
    }
}
