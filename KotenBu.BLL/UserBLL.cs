using KotenBu.DAL;
using KotenBu.Model;
using MateralTools.MEncryption;
using MateralTools.MEnum;
using MateralTools.MResult;
using MateralTools.MVerify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KotenBu.BLL
{
    /// <summary>
    /// 用户业务类
    /// </summary>
    public sealed class UserBLL : BaseBLL<UserDAL, T_User, V_User>
    {
        #region 成员
        /// <summary>
        /// Token业务对象
        /// </summary>
        private readonly TokenBLL _tokenBLL = new TokenBLL();
        /// <summary>
        /// 权限数据对象
        /// </summary>
        private readonly PermissionsDAL _permissionsDAL = new PermissionsDAL();
        /// <summary>
        /// 权限业务对象
        /// </summary>
        private readonly PermissionsBLL _permissionsBLL = new PermissionsBLL();
        /// <summary>
        /// 默认密码
        /// </summary>
        private const string DEFUALTPASSWORD = "123456";
        #endregion
        #region 公共方法
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName">登录用户名</param>
        /// <param name="password">密码</param>
        /// <returns>登录结果 用户ID,Token值</returns>
        /// <exception cref="ApplicationException"></exception>
        public LoginOutModel Login(string userName, string password)
        {
            List<T_User> listM = _dal.GetUserInfoByLoginUserName(userName);
            password = EncryptionManager.MD5Encode_32(password);
            foreach (T_User item in listM)
            {
                if (item.Password == password)
                {
                    T_Token tokenM = _tokenBLL.GetNewToken(item.ID, TokenTypeEnum.Login);
                    if (tokenM != null)
                    {
                        LoginOutModel resM = new LoginOutModel
                        {
                            ID = item.ID,
                            Token = tokenM.Token
                        };
                        return resM;
                    }
                    else
                    {
                        throw new ApplicationException("获取Token失败！");
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 删除一个用户对象
        /// </summary>
        /// <param name="ID">用户ID</param>
        /// <exception cref="ArgumentException"></exception>
        public void Delete(Guid userID)
        {
            T_User userM = _dal.GetDBModelInfoByID(userID);
            if (userM != null)
            {
                userM.IfDelete = true;
                _dal.SaveChange();
            }
            else
            {
                throw new ArgumentException("用户不存在。");
            }
        }
        /// <summary>
        /// 修改一个用户对象
        /// </summary>
        /// <param name="model">用户对象</param>
        /// <exception cref="ArgumentException"></exception>
        public void Update(T_User model)
        {
            T_User userM = _dal.GetDBModelInfoByID(model.ID);
            if (userM != null)
            {
                string msg = "";
                if (VerificationUpdate(model, ref msg))
                {
                    userM.NickName = model.NickName;
                    userM.UserName = model.UserName;
                    userM.Mobile = model.Mobile;
                    userM.Email = model.Email;
                    userM.TrueName = model.TrueName;
                    userM.Sex = model.Sex;
                    userM.IfEnable = model.IfEnable;
                    _dal.SaveChange();
                }
                else
                {
                    throw new ArgumentException(msg);
                }
            }
            else
            {
                throw new ArgumentException("用户不存在");
            }
        }
        /// <summary>
        /// 修改我的信息
        /// </summary>
        /// <param name="model">要修改的对象</param>
        /// <param name="LoginUserID">登录用户唯一标识</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ApplicationException"></exception>
        public void UpdateMyInfo(T_User model, Guid LoginUserID)
        {
            if (model.ID == LoginUserID)
            {
                T_User userM = _dal.GetDBModelInfoByID(model.ID);
                if (userM != null)
                {
                    string msg = "";
                    if (VerificationUpdate(model, ref msg))
                    {
                        userM.NickName = model.NickName;
                        userM.UserName = model.UserName;
                        userM.Mobile = model.Mobile;
                        userM.Email = model.Email;
                        userM.TrueName = model.TrueName;
                        userM.Sex = model.Sex;
                        _dal.SaveChange();
                    }
                    else
                    {
                        throw new ArgumentException(msg);
                    }
                }
                else
                {
                    throw new ArgumentException("用户不存在");
                }
            }
            else
            {
                throw new ApplicationException("只能修改自己的信息");
            }
        }
        /// <summary>
        /// 添加一个用户对象(后台添加)
        /// </summary>
        /// <param name="model">用户对象</param>
        /// <exception cref="ArgumentException"></exception>
        public void Add(T_User model)
        {
            DateTime dt = DateTime.Now;
            model.IfDelete = false;
            model.CreateTime = dt;
            model.Password = EncryptionManager.MD5Encode_32(DEFUALTPASSWORD);
            model.RegisterSources = (byte)RegisterSourcesEnum.Background;
            string msg = "";
            if (VerificationAdd(model, ref msg))
            {
                _dal.Insert(model);
            }
            else
            {
                throw new ArgumentException(msg);
            }
        }
        /// <summary>
        /// 添加一个用户对象(微信添加)
        /// </summary>
        /// <param name="model">用户对象</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void WeChatAdd(T_User model)
        {
            DateTime dt = DateTime.Now;
            model.IfDelete = false;
            model.CreateTime = dt;
            model.Password = EncryptionManager.MD5Encode_32(DEFUALTPASSWORD);
            model.RegisterSources = (byte)RegisterSourcesEnum.WeChat;
            if (!string.IsNullOrEmpty(model.WeChatOpenID))
            {
                V_User userM = _dal.GetUserViewInfoByWeChatOpenID(model.WeChatOpenID);
                if (userM == null)
                {
                    string msg = "";
                    if (VerificationAdd(model, ref msg))
                    {
                        _dal.Insert(model);
                    }
                    else
                    {
                        throw new ArgumentException(msg);
                    }
                }
                else
                {
                    throw new ArgumentNullException("微信OpenID已存在");
                }
            }
            else
            {
                throw new ArgumentNullException("微信OpenID不可为空");
            }
        }
        /// <summary>
        /// 更改密码
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="oldPassword">新密码</param>
        /// <param name="password">新密码</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ApplicationException"></exception>
        public void EditMyPassword(Guid userID, string oldPassword, string password, Guid LoginUserID)
        {
            if (userID == LoginUserID)
            {
                T_User userM = _dal.GetDBModelInfoByID(userID);
                if (userM != null)
                {
                    if (userM.Password == EncryptionManager.MD5Encode_32(oldPassword))
                    {
                        userM.Password = EncryptionManager.MD5Encode_32(password);
                        _dal.SaveChange();
                    }
                    else
                    {
                        throw new ArgumentException("旧密码错误");
                    }
                }
                else
                {
                    throw new ArgumentException("该用户不存在");
                }
            }
            else
            {
                throw new ApplicationException("只能修改自己的密码");
            }
        }
        /// <summary>
        /// 更改密码
        /// </summary>
        /// <param name="Mobile">手机号码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns>修改结果</returns>
        /// <exception cref="ArgumentException"></exception>
        public void MobileEditPassword(string Mobile, string newPassword)
        {
            T_User userM = _dal.GetDBModelInfoByID(Mobile);
            if (userM != null)
            {
                userM.Password = EncryptionManager.MD5Encode_32(newPassword);
                _dal.SaveChange();
            }
            else
            {
                throw new ArgumentException("该手机号码未注册");
            }
        }
        /// <summary>
        /// 根据用户ID获得权限组信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        public PermissionsGroupModel GetMenuPermissionsInfoByUserID(Guid userID)
        {
            T_User userM = _dal.GetDBModelInfoByID(userID);
            Guid[] userGroupIds = (from m in userM.T_UserGroup
                                   where !m.IfDelete && m.IfEnable
                                   select m.ID).ToArray();
            PermissionsGroupModel permissionsMs = _permissionsBLL.GetHasEnablePermissionsInfoByUserGroupID(userGroupIds, PermissionsTypesEnum.Menu);
            return permissionsMs;
        }
        /// <summary>
        /// 用户是否拥有对应权限
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="permissionCode">权限代码</param>
        /// <returns>检测结果</returns>
        public bool HasPermissions(Guid userID, string permissionCode)
        {
            V_Permissions perM = _permissionsDAL.GetPermissionsInfoByCodeAndEnable(permissionCode, true);
            T_User userM = _dal.GetDBModelInfoByID(userID);
            List<T_Permissions> funPermissions;
            foreach (T_UserGroup userGroupM in userM.T_UserGroup)
            {
                funPermissions = (from m in userGroupM.T_Permissions
                                  where m.Type == (byte)PermissionsTypesEnum.Function && !m.IfDelete && m.IfEnable
                                  select m).ToList();
                if ((from m in funPermissions
                     where m.ID == perM.ID
                     select m).Count() > 0)
                {
                    return true;
                }
            }
            return false;
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
        public List<V_User> GetUserInfoByWhere(string userName, string mobile, string email, string trueName, string nickName, bool? ifEnable, MPagingModel pageM)
        {
            return _dal.GetUserViewInfoByWhere(userName, mobile, email, trueName, nickName, ifEnable, pageM);
        }
        /// <summary>
        /// 根据用户唯一标识获取用户组信息
        /// </summary>
        /// <param name="userID">用户唯一标识</param>
        /// <returns>用户组信息</returns>
        public List<V_UserGroup> GetUserGroupInfoByUserID(Guid userID)
        {
            T_User userM = _dal.GetDBModelInfoByID(userID);
            Guid[] userGroupIDs = (from m in userM.T_UserGroup
                                   where !m.IfDelete && m.IfEnable
                                   select m.ID).ToArray();
            List<V_UserGroup> listM = _dal.GetUserGroupViewInfoByIDs(userGroupIDs);
            return listM;
        }
        /// <summary>
        /// 添加一个用户组
        /// </summary>
        /// <param name="userID">用户唯一标识</param>
        /// <param name="userGroupID">用户组唯一标识</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ApplicationException"></exception>
        public void AddUserGroup(Guid userID, Guid userGroupID)
        {
            T_User userM = _dal.GetDBModelInfoByID(userID);
            if (userM != null)
            {
                T_UserGroup userGroupM = _dal.GetUserGroupInfoByID(userGroupID);
                if (userGroupM != null)
                {
                    if (userM.T_UserGroup.Where(m => m.ID == userGroupID).FirstOrDefault() == null)
                    {
                        userM.T_UserGroup.Add(userGroupM);
                        _dal.SaveChange();
                    }
                    else
                    {
                        throw new ApplicationException("已拥有该用户组");
                    }
                }
                else
                {
                    throw new ArgumentException("用户组不存在");
                }
            }
            else
            {
                throw new ArgumentException("用户不存在");
            }
        }
        /// <summary>
        /// 移除一个用户组
        /// </summary>
        /// <param name="userID">用户唯一标识</param>
        /// <param name="userGroupID">用户组唯一标识</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ApplicationException"></exception>
        public void RemoveUserGroup(Guid userID, Guid userGroupID)
        {
            T_User userM = _dal.GetDBModelInfoByID(userID);
            if (userM != null)
            {
                T_UserGroup userGroupM = userM.T_UserGroup.Where(m => m.ID == userGroupID).FirstOrDefault();
                if (userGroupM != null)
                {
                    userM.T_UserGroup.Remove(userGroupM);
                    _dal.SaveChange();
                }
                else
                {
                    throw new ApplicationException("该用户无此用户组");
                }
            }
            else
            {
                throw new ArgumentException("用户不存在");
            }
        }
        /// <summary>
        /// 根据手机号或真实姓名或用户名获得用户信息
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="trueName">真实姓名</param>
        /// <param name="userName">用户名</param>
        /// <returns>用户信息</returns>
        public List<V_User> GetUserViewInfoByMobileOrTrueNameOrUserNameOrEmail(string mobile, string trueName, string userName, string email)
        {
            List<V_User> listM = new List<V_User>();
            V_User tempM = _dal.GetUserViewInfoByMobile(mobile);
            if (tempM != null)
            {
                listM.Add(tempM);
            }
            tempM = _dal.GetUserViewInfoByUserName(userName);
            if (tempM != null)
            {
                listM.Add(tempM);
            }
            tempM = _dal.GetUserViewInfoByEmail(email);
            if (tempM != null)
            {
                listM.Add(tempM);
            }
            listM.AddRange(_dal.GetUserViewInfoByTrueName(trueName));
            return listM.Distinct().ToList();
        }
        #endregion
        #region 私有方法
        /// <summary>
        /// 验证模型
        /// </summary>
        /// <param name="model">要验证的模型</param>
        /// <param name="msg">提示信息</param>
        /// <returns>验证结果</returns>
        protected override bool Verification(T_User model, ref string msg)
        {
            if (!string.IsNullOrEmpty(model.Email))
            {
                if (!VerifyManager.IsEMail(model.Email))
                {
                    msg += "邮箱格式不正确，";
                }
            }
            if (!string.IsNullOrEmpty(model.Mobile))
            {
                if (!VerifyManager.IsPhoneNumber(model.Mobile))
                {
                    msg += "手机号码格式不正确，";
                }
            }
            return base.Verification(model, ref msg);
        }
        /// <summary>
        /// 验证添加
        /// </summary>
        /// <param name="model"></param>
        /// <param name="msg"></param>
        /// <returns>验证结果</returns>
        private bool VerificationAdd(T_User model, ref string msg)
        {
            if (string.IsNullOrEmpty(model.UserName) && string.IsNullOrEmpty(model.Email) && string.IsNullOrEmpty(model.Mobile))
            {
                msg += "用户名、邮箱、手机号码必须填写一个，";
            }
            else
            {
                V_User temp;
                if (!string.IsNullOrEmpty(model.UserName))
                {
                    temp = _dal.GetUserViewInfoByUserName(model.UserName);
                    if (temp != null)
                    {
                        msg += "用户名已被占用，";
                    }
                }
                if (!string.IsNullOrEmpty(model.Email))
                {
                    temp = _dal.GetUserViewInfoByEmail(model.Email);
                    if (temp != null)
                    {
                        msg += "邮箱已被占用，";
                    }
                }
                if (!string.IsNullOrEmpty(model.Mobile))
                {
                    temp = _dal.GetUserViewInfoByMobile(model.Mobile);
                    if (temp != null)
                    {
                        msg += "手机号码已被占用，";
                    }
                }
            }
            return Verification(model, ref msg);
        }
        /// <summary>
        /// 验证修改
        /// </summary>
        /// <param name="model">要验证的模型</param>
        /// <param name="msg">提示信息</param>
        /// <returns>验证结果</returns>
        private bool VerificationUpdate(T_User model, ref string msg)
        {

            if (string.IsNullOrEmpty(model.UserName) && string.IsNullOrEmpty(model.Email) && string.IsNullOrEmpty(model.Mobile))
            {
                msg += "用户名、邮箱、手机号码必须填写一个，";
            }
            else
            {
                V_User temp;
                if (!string.IsNullOrEmpty(model.UserName))
                {
                    temp = _dal.GetUserViewInfoByUserName(model.UserName);
                    if (temp != null && temp.ID != model.ID)
                    {
                        msg += "用户名已被占用，";
                    }
                }
                if (!string.IsNullOrEmpty(model.Email))
                {
                    temp = _dal.GetUserViewInfoByEmail(model.Email);
                    if (temp != null && temp.ID != model.ID)
                    {
                        msg += "邮箱已被占用，";
                    }
                }
                if (!string.IsNullOrEmpty(model.Mobile))
                {
                    temp = _dal.GetUserViewInfoByMobile(model.Mobile);
                    if (temp != null && temp.ID != model.ID)
                    {
                        msg += "手机号码已被占用，";
                    }
                }
            }
            return Verification(model, ref msg);
        }
        #endregion
    }
}
