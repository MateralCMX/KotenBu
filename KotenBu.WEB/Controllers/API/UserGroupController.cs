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
    /// 用户组API接口
    /// </summary>
    [RoutePrefix("api/UserGroup")]
    public class UserGroupController : ApiBaseController<UserGroupBLL>
    {
        /// <summary>
        /// 根据类型获得用户组信息
        /// </summary>
        /// <param name="Name">名称</param>
        /// <param name="Code">代码</param>
        /// <param name="IfEnable">启用标识</param>
        /// <param name="PageIndex">当前页数</param>
        /// <param name="PageSize">每页显示数量</param>
        /// <returns>用户组信息</returns>
        [HttpGet]
        [Route("GetUserGroupInfoByWhere")]
        public MResultModel GetUserGroupInfoByWhere(string Name, string Code, bool? IfEnable, int PageIndex, int PageSize)
        {
            MPagingModel pageM = new MPagingModel(PageIndex, PageSize);
            List<V_UserGroup> listM = _bll.GetUserGroupInfoByWhere(Name, Code, IfEnable, pageM);
            return MResultPagingModel<List<V_UserGroup>>.GetSuccessResultM(listM, pageM, "查询成功");
        }
        /// <summary>
        /// 根据唯一标识获得用户组信息
        /// </summary>
        /// <param name="ID">唯一标识</param>
        /// <returns>用户组信息</returns>
        [HttpGet]
        [Route("GetUserGroupInfoByID")]
        public MResultModel GetUserGroupInfoByID(Guid ID)
        {
            V_UserGroup resM = _bll.GetDBModelViewInfoByID(ID);
            return MResultModel<V_UserGroup>.GetSuccessResultM(resM, "查询成功");
        }
        /// <summary>
        /// 根据名称获得用户组信息
        /// </summary>
        /// <param name="Name">名称</param>
        /// <returns>用户组信息</returns>
        [HttpGet]
        [Route("GetUserGroupInfoByName")]
        public MResultModel GetUserGroupInfoByName(string Name)
        {
            List<V_UserGroup> resM = _bll.GetUserGroupInfoByName(Name);
            return MResultModel<List<V_UserGroup>>.GetSuccessResultM(resM, "查询成功");
        }
        /// <summary>
        /// 添加用户组
        /// </summary>
        /// <param name="model">操作对象</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        [Route("AddUserGroup")]
        [PermissionsCode(ApplicationManager.Permissions_UserGroupOperation)]
        public MResultModel AddUserGroup(EditUserGroupInModel model)
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
        /// 修改用户组
        /// </summary>
        /// <param name="model">操作对象</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        [Route("EditUserGroup")]
        [PermissionsCode(ApplicationManager.Permissions_UserGroupOperation)]
        public MResultModel EditUserGroup(EditUserGroupInModel model)
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
        /// 删除用户组
        /// </summary>
        /// <param name="model">操作对象</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        [Route("DeleteUserGroup")]
        [PermissionsCode(ApplicationManager.Permissions_UserGroupOperation)]
        public MResultModel DeleteUserGroup(DeleteInModel model)
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
        /// 保存权限信息
        /// </summary>
        /// <param name="model">操作对象</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        [Route("SavePermissions")]
        [PermissionsCode(ApplicationManager.Permissions_UserGroupOperation)]
        public MResultModel SavePermissions(SavePermissionsInModel model)
        {
            try
            {
                _bll.SavePermissions(model.UserGroupID, model.PermissionsIDs);
                return MResultModel.GetSuccessResultM("保存成功");
            }
            catch (ArgumentException ex)
            {
                return MResultModel.GetFailResultM(ex.Message);
            }
        }
    }
}
