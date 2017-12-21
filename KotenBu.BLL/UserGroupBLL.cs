using KotenBu.DAL;
using KotenBu.Model;
using MateralTools.MEncryption;
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
    /// 用户组业务类
    /// </summary>
    public sealed class UserGroupBLL : BaseBLL<UserGroupDAL, T_UserGroup, V_UserGroup>
    {
        #region 成员
        private readonly PermissionsBLL _permissionsBLL = new PermissionsBLL();
        #endregion
        #region 公共方法
        /// <summary>
        /// 删除一个用户组对象
        /// </summary>
        /// <param name="ID">用户组ID</param>
        /// <exception cref="ArgumentException"></exception>
        public void Delete(Guid id)
        {
            T_UserGroup userM = _dal.GetDBModelInfoByID(id);
            if (userM != null)
            {
                userM.IfDelete = true;
                _dal.SaveChange();
            }
            else
            {
                throw new ArgumentException("用户组不存在。");
            }
        }
        /// <summary>
        /// 修改一个用户组对象
        /// </summary>
        /// <param name="model">用户组对象</param>
        /// <exception cref="ArgumentException"></exception>
        public void Update(T_UserGroup model)
        {
            T_UserGroup userM = _dal.GetDBModelInfoByID(model.ID);
            if (userM != null)
            {
                string msg = "";
                if (VerificationUpdate(model, ref msg))
                {
                    userM.Code = model.Code;
                    userM.IfEnable = model.IfEnable;
                    userM.Introduction = model.Introduction;
                    userM.Name = model.Name;
                    _dal.SaveChange();
                }
                else
                {
                    throw new ArgumentException(msg);
                }
            }
            else
            {
                throw new ArgumentException("用户组不存在");
            }
        }
        /// <summary>
        /// 添加一个用户组对象
        /// </summary>
        /// <param name="model">用户组对象</param>
        /// <exception cref="ArgumentException"></exception>
        public void Add(T_UserGroup model)
        {
            DateTime dt = DateTime.Now;
            model.IfDelete = false;
            model.CreateTime = dt;
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
        /// 根据条件获得用户组信息
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="code">代码</param>
        /// <param name="ifEnable">启用标识</param>
        /// <param name="pageM">分页对象</param>
        /// <returns>用户组信息</returns>
        public List<V_UserGroup> GetUserGroupInfoByWhere(string name, string code, bool? ifEnable, MPagingModel pageM)
        {
            List<V_UserGroup> listM = _dal.GetUserGroupInfoByWhere(name, code, ifEnable, pageM);
            return listM;
        }
        /// <summary>
        /// 保存权限
        /// </summary>
        /// <param name="userGroupID">用户组唯一标识</param>
        /// <param name="permissionsIDs">用户组唯一标识</param>
        /// <exception cref="ArgumentException"></exception>
        public void SavePermissions(Guid userGroupID,Guid[] permissionsIDs)
        {
            T_UserGroup userGroupM = _dal.GetDBModelInfoByID(userGroupID);
            Guid[] hasIDs = _permissionsBLL.GetHasPermissionsIDsByUserGroupID(userGroupID);
            Guid[] AddIDs = (from m in permissionsIDs
                             where !hasIDs.Contains(m)
                             select m).ToArray();
            T_Permissions perM;
            foreach (Guid item in AddIDs)
            {
                perM = _dal.GetPermissionsInfoByID(item);
                if (perM != null)
                {
                    userGroupM.T_Permissions.Add(perM);
                }
                else
                {
                    throw new ArgumentException("该权限不存在");
                }
            }
            Guid[] DeleteIDs = (from m in hasIDs
                                where !permissionsIDs.Contains(m)
                                select m).ToArray();
            foreach (Guid item in DeleteIDs)
            {
                perM = (from m in userGroupM.T_Permissions
                                      where m.ID == item
                                      select m).FirstOrDefault();
                if (perM != null)
                {
                    userGroupM.T_Permissions.Remove(perM);
                }
                else
                {
                    throw new ArgumentException("该权限不存在");
                }
            }
            _dal.SaveChange();
        }
        /// <summary>
        /// 根据名称获得用户组信息
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>用户组信息</returns>
        public List<V_UserGroup> GetUserGroupInfoByName(string name)
        {
            List<V_UserGroup> listM = _dal.GetUserGroupInfoByName(name);
            return listM;
        }
        #endregion
        #region 私有方法
        /// <summary>
        /// 验证模型
        /// </summary>
        /// <param name="model">要验证的模型</param>
        /// <param name="msg">提示信息</param>
        /// <returns>验证结果</returns>
        protected override bool Verification(T_UserGroup model, ref string msg)
        {
            if (string.IsNullOrEmpty(model.Code))
            {
                msg += "用户组代码不能为空，";
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                msg += "用户组名称不能为空，";
            }
            return base.Verification(model, ref msg);
        }
        /// <summary>
        /// 验证添加
        /// </summary>
        /// <param name="model"></param>
        /// <param name="msg"></param>
        /// <returns>验证结果</returns>
        private bool VerificationAdd(T_UserGroup model, ref string msg)
        {
            V_UserGroup temp;
            if (!string.IsNullOrEmpty(model.Code))
            {
                temp = _dal.GetUserGroupInfoByCodeAndEnable(model.Code, null);
                if (temp != null)
                {
                    msg += "用户组代码已被占用，";
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
        private bool VerificationUpdate(T_UserGroup model, ref string msg)
        {
            V_UserGroup temp;
            if (!string.IsNullOrEmpty(model.Code))
            {
                temp = _dal.GetUserGroupInfoByCodeAndEnable(model.Code, null);
                if (temp != null && temp.ID != model.ID)
                {
                    msg += "用户组代码已被占用，";
                }
            }
            return Verification(model, ref msg);
        }
        #endregion
    }
}
