using MateralTools.MEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KotenBu.Model
{
    /// <summary>
    /// 用户组修改模型
    /// </summary>
    public sealed class EditUserGroupInModel : T_UserGroup, IVerificationLoginModel
    {
        /// <summary>
        /// 登录用户唯一标识
        /// </summary>
        public Guid LoginUserID { get; set; }
        /// <summary>
        /// 登录用户Token
        /// </summary>
        public string LoginUserToken { get; set; }
    }
    /// <summary>
    /// 保存权限输入模型
    /// </summary>
    public sealed class SavePermissionsInModel : IVerificationLoginModel
    {
        /// <summary>
        /// 用户组ID
        /// </summary>
        public Guid UserGroupID { get; set; }
        /// <summary>
        /// 要保存的权限ID
        /// </summary>
        public Guid[] PermissionsIDs { get; set; }
        /// <summary>
        /// 登录用户唯一标识
        /// </summary>
        public Guid LoginUserID { get; set; }
        /// <summary>
        /// 登录用户Token
        /// </summary>
        public string LoginUserToken { get; set; }
    }

}
