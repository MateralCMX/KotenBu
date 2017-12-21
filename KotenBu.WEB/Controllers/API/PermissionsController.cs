using KotenBu.BLL;
using KotenBu.Model;
using MateralTools.MEnum;
using MateralTools.MResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KotenBu.WEB.Controllers.API
{
    /// <summary>
    /// 权限API接口
    /// </summary>
    [RoutePrefix("api/Permissions")]
    public class PermissionsController : ApiBaseController<PermissionsBLL>
    {
        /// <summary>
        /// 获得所有权限类型信息
        /// </summary>
        /// <returns>权限信息</returns>
        [HttpGet]
        [Route("GetAllPermissionsType")]
        public MResultModel GetAllPermissionsType()
        {
            List<EnumModel> listM = EnumManager.GetAllEnum(typeof(PermissionsTypesEnum));
            return MResultModel<List<EnumModel>>.GetSuccessResultM(listM, "查询成功");
        }
        /// <summary>
        /// 根据类型获得权限信息
        /// </summary>
        /// <param name="Type">权限类型</param>
        /// <returns>权限信息</returns>
        [HttpGet]
        [Route("GetPermissionsInfoByType")]
        public MResultModel GetPermissionsInfoByType(PermissionsTypesEnum Type)
        {
            List<PermissionsModel> listM = _bll.GetPermissionsInfoByType(Type);
            return MResultModel<List<PermissionsModel>>.GetSuccessResultM(listM, "查询成功");
        }
        /// <summary>
        /// 根据唯一标识获得权限信息
        /// </summary>
        /// <param name="ID">唯一标识</param>
        /// <returns>权限信息</returns>
        [HttpGet]
        [Route("GetPermissionsInfoByID")]
        public MResultModel GetPermissionsInfoByID(Guid ID)
        {
            V_Permissions resM = _bll.GetDBModelViewInfoByID(ID);
            return MResultModel<V_Permissions>.GetSuccessResultM(resM, "查询成功");
        }
        /// <summary>
        /// 根据唯一标识获得权限信息
        /// </summary>
        /// <param name="UserGroupID">唯一标识</param>
        /// <returns>权限信息</returns>
        [HttpGet]
        [Route("GetEnablePermissionsInfoByUserGroupID")]
        public MResultModel GetEnablePermissionsInfoByUserGroupID(Guid UserGroupID)
        {
            try
            {
                List<PermissionsGroupModel> resM = _bll.GetEnablePermissionsInfoByUserGroupID(UserGroupID);
                return MResultModel<List<PermissionsGroupModel>>.GetSuccessResultM(resM, "查询成功");
            }
            catch(ArgumentException ex)
            {
                return MResultModel.GetFailResultM(ex.Message);
            }
        }
        /// <summary>
        /// 添加权限
        /// </summary>
        /// <param name="model">操作对象</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        [Route("AddPermissions")]
        [PermissionsCode(ApplicationManager.Permissions_PermissionsOperation)]
        public MResultModel AddPermissions(EditPermissionsInModel model)
        {
            try
            {
                _bll.Add(model);
                return MResultModel.GetSuccessResultM("添加成功");
            }
            catch(ArgumentException ex)
            {
                return MResultModel.GetFailResultM(ex.Message);
            }
        }
        /// <summary>
        /// 修改权限
        /// </summary>
        /// <param name="model">操作对象</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        [Route("EditPermissions")]
        [PermissionsCode(ApplicationManager.Permissions_PermissionsOperation)]
        public MResultModel EditPermissions(EditPermissionsInModel model)
        {
            try
            {
                _bll.Update(model);
                return MResultModel.GetSuccessResultM("修改成功");
            }
            catch (ArgumentException ex)
            {
                return MResultModel.GetFailResultM(ex.Message);
            }
        }
        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="model">操作对象</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        [Route("DeletePermissions")]
        [PermissionsCode(ApplicationManager.Permissions_PermissionsOperation)]
        public MResultModel DeletePermissions(DeleteInModel model)
        {
            try
            {
                _bll.Delete(model.ID);
                return MResultModel.GetSuccessResultM("删除成功");
            }
            catch (ArgumentException ex)
            {
                return MResultModel.GetFailResultM(ex.Message);
            }
        }
        /// <summary>
        /// 更改位序
        /// </summary>
        /// <param name="model">操作对象</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        [Route("ChangeRank")]
        [PermissionsCode(ApplicationManager.Permissions_PermissionsOperation)]
        public MResultModel ChangeRank(ChangeRankInModel model)
        {
            try
            {
                _bll.ChangeRank(model.ID, model.TargetID);
                return MResultModel.GetSuccessResultM("更改成功");
            }
            catch (ArgumentException ex)
            {
                return MResultModel.GetFailResultM(ex.Message);
            }
        }
    }
}
