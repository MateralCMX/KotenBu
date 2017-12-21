using KotenBu.DAL;
using KotenBu.Model;
using MateralTools.MEncryption;
using MateralTools.MVerify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KotenBu.BLL
{
    /// <summary>
    /// 权限业务类
    /// </summary>
    public sealed class PermissionsBLL : BaseBLL<PermissionsDAL, T_Permissions, V_Permissions>
    {
        #region 成员
        private readonly UserGroupDAL _userGroupDAL = new UserGroupDAL();
        #endregion
        #region 公共方法
        /// <summary>
        /// 删除一个权限对象
        /// </summary>
        /// <param name="ID">权限ID</param>
        /// <exception cref="ArgumentException"></exception>
        public void Delete(Guid id)
        {
            T_Permissions userM = _dal.GetDBModelInfoByID(id);
            if (userM != null)
            {
                userM.IfDelete = true;
                _dal.SaveChange();
            }
            else
            {
                throw new ArgumentException("权限不存在。");
            }
        }
        /// <summary>
        /// 修改一个权限对象
        /// </summary>
        /// <param name="model">权限对象</param>
        /// <exception cref="ArgumentException"></exception>
        public void Update(T_Permissions model)
        {
            T_Permissions userM = _dal.GetDBModelInfoByID(model.ID);
            if (userM != null)
            {
                string msg = "";
                if (VerificationUpdate(model, ref msg))
                {
                    userM.Code = model.Code;
                    userM.Ico = model.Ico;
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
                throw new ArgumentException("权限不存在");
            }
        }
        /// <summary>
        /// 添加一个权限对象
        /// </summary>
        /// <param name="model">权限对象</param>
        /// <exception cref="ArgumentException"></exception>
        public void Add(T_Permissions model)
        {
            DateTime dt = DateTime.Now;
            model.IfDelete = false;
            model.CreateTime = dt;
            model.Ranks = _dal.GetMaxRank() + 1;
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
        /// 更改位序
        /// </summary>
        /// <param name="id">权限唯一标识</param>
        /// <param name="targetID">目标唯一标识</param>
        public void ChangeRank(Guid id, Guid targetID)
        {
            T_Permissions perM = _dal.GetDBModelInfoByID(id);
            if (perM != null)
            {
                T_Permissions targetPerM = _dal.GetDBModelInfoByID(targetID);
                if (targetPerM != null)
                {
                    int tempRank = perM.Ranks;
                    perM.Ranks = targetPerM.Ranks;
                    targetPerM.Ranks = tempRank;
                    _dal.SaveChange();
                }
                else
                {
                    throw new ArgumentException("目标权限不存在");
                }
            }
            else
            {
                throw new ArgumentException("该权限不存在");
            }
        }
        /// <summary>
        /// 根据类型获得权限信息
        /// </summary>
        /// <param name="type">权限类型</param>
        /// <returns></returns>
        public List<PermissionsModel> GetPermissionsInfoByType(PermissionsTypesEnum type)
        {
            List<T_Permissions> perInfos = _dal.GetPermissionsInfoByTypeAndEnable(type, null);
            List<PermissionsModel> listM = PermissionsModel.GetList(perInfos, null);
            return listM;
        }
        /// <summary>
        /// 根据用户组ID获得所有启用的权限信息
        /// </summary>
        /// <param name="userGroupID">用户组ID</param>
        /// <returns>权限信息</returns>
        /// <exception cref="ArgumentException"></exception>
        public List<PermissionsGroupModel> GetEnablePermissionsInfoByUserGroupID(Guid userGroupID)
        {
            Guid[] ids = GetHasPermissionsIDsByUserGroupID(userGroupID);
            List<PermissionsGroupModel> resM = new List<PermissionsGroupModel>();
            foreach (PermissionsTypesEnum type in Enum.GetValues(typeof(PermissionsTypesEnum)))
            {
                resM.Add(GetEnablePermissionsInfoByUserGroupIDAndTypeAndHsID(type, ids, PermissionsModelModeEnum.All));
            }
            return resM;
        }
        /// <summary>
        /// 根据用户组ID和类型获得所拥有启用的权限信息
        /// </summary>
        /// <param name="userGroupID">用户组ID</param>
        /// <param name="type">权限类型</param>
        /// <returns>权限信息</returns>
        /// <exception cref="ArgumentException"></exception>
        public PermissionsGroupModel GetHasEnablePermissionsInfoByUserGroupID(Guid[] userGroupIDs, PermissionsTypesEnum type)
        {
            List<Guid> ids = new List<Guid>();
            foreach (Guid item in userGroupIDs)
            {
                ids.AddRange(GetHasPermissionsIDsByUserGroupID(item));
            }
            return GetEnablePermissionsInfoByUserGroupIDAndTypeAndHsID(type, ids.Distinct().ToArray(), PermissionsModelModeEnum.Has);
        }
        /// <summary>
        /// 根据用户组获得拥有权限的
        /// </summary>
        /// <param name="userGroupID">用户组ID</param>
        /// <returns>拥有的权限ID</returns>
        /// <exception cref="ArgumentException"></exception>
        public Guid[] GetHasPermissionsIDsByUserGroupID(Guid userGroupID)
        {
            T_UserGroup userGroupM = _userGroupDAL.GetDBModelInfoByID(userGroupID);
            if (userGroupM != null)
            {
                Guid[] ids = (from m in userGroupM.T_Permissions
                              where !m.IfDelete && m.IfEnable
                              select m.ID).ToArray();
                return ids;
            }
            else
            {
                throw new ArgumentException("该用户组不存在");
            }
        }
        #endregion
        #region 私有方法
        /// <summary>
        /// 根据类型和拥有权限获得权限组信息
        /// </summary>
        /// <param name="type">权限类型</param>
        /// <param name="hasIDs">拥有权限</param>
        /// <returns>权限信息</returns>
        private PermissionsGroupModel GetEnablePermissionsInfoByUserGroupIDAndTypeAndHsID(PermissionsTypesEnum type, Guid[] hasIDs, PermissionsModelModeEnum mode)
        {
            List<T_Permissions> perInfos = _dal.GetPermissionsInfoByTypeAndEnable(type, true);
            PermissionsGroupModel tempM = new PermissionsGroupModel
            {
                Type = type,
                Items = PermissionsModel.GetList(perInfos, true, hasIDs, mode)
            };
            return tempM;
        }
        /// <summary>
        /// 验证模型
        /// </summary>
        /// <param name="model">要验证的模型</param>
        /// <param name="msg">提示信息</param>
        /// <returns>验证结果</returns>
        protected override bool Verification(T_Permissions model, ref string msg)
        {
            if (string.IsNullOrEmpty(model.Code))
            {
                msg += "权限代码不能为空，";
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                msg += "权限名称不能为空，";
            }
            return base.Verification(model, ref msg);
        }
        /// <summary>
        /// 验证添加
        /// </summary>
        /// <param name="model"></param>
        /// <param name="msg"></param>
        /// <returns>验证结果</returns>
        private bool VerificationAdd(T_Permissions model, ref string msg)
        {
            V_Permissions temp;
            if (!string.IsNullOrEmpty(model.Code))
            {
                temp = _dal.GetPermissionsInfoByCodeAndEnable(model.Code, null);
                if (temp != null)
                {
                    msg += "权限代码已被占用，";
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
        private bool VerificationUpdate(T_Permissions model, ref string msg)
        {
            V_Permissions temp;
            if (!string.IsNullOrEmpty(model.Code))
            {
                temp = _dal.GetPermissionsInfoByCodeAndEnable(model.Code, null);
                if (temp != null && temp.ID != model.ID)
                {
                    msg += "权限代码已被占用，";
                }
            }
            return Verification(model, ref msg);
        }
        #endregion
    }
}
