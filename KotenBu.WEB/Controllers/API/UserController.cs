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
    /// 用户API接口
    /// </summary>
    [RoutePrefix("api/User")]
    public class UserController : ApiBaseController<UserBLL>
    {
        /// <summary>
        /// 后台用户登录
        /// </summary>
        /// <param name="model">登录模型</param>
        /// <returns>登录结果</returns>
        [HttpPost]
        [Route("AdminLogin")]
        [NotVerificationLogin]
        public MResultModel AdminLogin(LoginInModel model)
        {
            try
            {
                if (VerificationImageCode(model.ValidateCode))
                {
                    LoginOutModel resM = _bll.Login(model.UserName, model.Password);
                    if (resM != null)
                    {
                        if (_bll.HasPermissions(resM.ID, ApplicationManager.Permissions_AdminLogin))
                        {
                            return MResultModel<LoginOutModel>.GetSuccessResultM(resM, "登录成功");
                        }
                        else
                        {
                            return MResultModel.GetFailResultM("登录失败,权限不足");
                        }
                    }
                    else
                    {
                        return MResultModel.GetFailResultM("登录失败,用户名或者密码错误");
                    }
                }
                else
                {
                    return MResultModel.GetFailResultM("登录失败,验证码错误");
                }
            }
            catch (ApplicationException ex)
            {
                return MResultModel.GetFailResultM(ex.Message);
            }
        }
        /// <summary>
        /// 根据用户唯一标识获得用户视图信息
        /// </summary>
        /// <param name="ID">用户唯一标识</param>
        /// <returns>用户信息</returns>
        [HttpGet]
        [Route("GetUserViewInfoByID")]
        public MResultModel GetUserViewInfoByID(Guid ID)
        {
            V_User userM = _bll.GetDBModelViewInfoByID(ID);
            return MResultModel<V_User>.GetSuccessResultM(userM, "查询成功");
        }
        /// <summary>
        /// 根据查询字符串获得用户信息
        /// </summary>
        /// <param name="SearchTxt">查询字符串</param>
        /// <returns>用户信息</returns>
        [HttpGet]
        [Route("GetUserInfoBySearchTxt")]
        public MResultModel GetUserInfoBySearchTxt(string SearchTxt)
        {
            List<V_User> resM = _bll.GetUserViewInfoByMobileOrTrueNameOrUserNameOrEmail(SearchTxt, SearchTxt, SearchTxt, SearchTxt);
            return MResultModel<List<V_User>>.GetSuccessResultM(resM, "查询成功");
        }
        /// <summary>
        /// 根据用户唯一标识获得用户组信息
        /// </summary>
        /// <param name="ID">用户唯一标识</param>
        /// <returns>用户信息</returns>
        [HttpGet]
        [Route("GetUserGroupInfoByUserID")]
        public MResultModel GetUserGroupInfoByUserID(Guid UserID)
        {
            List<V_UserGroup> listM = _bll.GetUserGroupInfoByUserID(UserID);
            return MResultModel<List<V_UserGroup>>.GetSuccessResultM(listM, "查询成功");
        }
        /// <summary>
        /// 获得所有性别信息
        /// </summary>
        /// <returns>性别信息</returns>
        [HttpGet]
        [Route("GetAllSex")]
        public MResultModel GetAllSex()
        {
            List<EnumModel> listM = EnumManager.GetAllEnum(typeof(SexEnum));
            return MResultModel<List<EnumModel>>.GetSuccessResultM(listM, "查询成功");
        }
        /// <summary>
        /// 根据条件获得用户信息
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Mobile">手机</param>
        /// <param name="Email">邮箱</param>
        /// <param name="TrueName">真实姓名</param>
        /// <param name="NickName">昵称</param>
        /// <param name="IfEnable">启用标识</param>
        /// <param name="PageIndex">当前页数</param>
        /// <param name="PageSize">每页显示数量</param>
        /// <returns>用户信息</returns>
        [HttpGet]
        [Route("GetUserInfoByWhere")]
        public MResultModel GetUserInfoByWhere(string UserName, string Mobile, string Email, string TrueName, string NickName, bool? IfEnable, int PageIndex, int PageSize)
        {
            MPagingModel pageM = new MPagingModel(PageIndex, PageSize);
            List<V_User> listM = _bll.GetUserInfoByWhere(UserName, Mobile, Email, TrueName, NickName, IfEnable, pageM);
            return MResultPagingModel<List<V_User>>.GetSuccessResultM(listM, pageM, "查询成功");
        }
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="model">操作对象</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        [Route("AddUser")]
        [PermissionsCode(ApplicationManager.Permissions_UserOperation)]
        public MResultModel AddUser(EditUserInModel model)
        {
            try
            {
                _bll.Add(model);
                return MResultModel.GetSuccessResultM("添加成功");
            }
            catch (ArgumentException ex)
            {
                return MResultModel.GetFailResultM(ex.Message);
            }
        }
        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="model">操作对象</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        [Route("EditUser")]
        [PermissionsCode(ApplicationManager.Permissions_UserOperation)]
        public MResultModel EditUser(EditUserInModel model)
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
        /// 删除用户
        /// </summary>
        /// <param name="model">操作对象</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        [Route("DeleteUser")]
        [PermissionsCode(ApplicationManager.Permissions_UserOperation)]
        public MResultModel DeleteUser(DeleteInModel model)
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
        /// 根据登录获得菜单信息
        /// </summary>
        /// <param name="ID">用户唯一标识</param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetMenuInfoByLoginUser")]
        [PermissionsCode(ApplicationManager.Permissions_AdminLogin)]
        public MResultModel GetMenuInfoByLoginUser(BaseQueryModel model)
        {
            PermissionsGroupModel resM = _bll.GetMenuPermissionsInfoByUserID(model.LoginUserID);
            return MResultModel<PermissionsGroupModel>.GetSuccessResultM(resM, "查询成功");
        }
        /// <summary>
        /// 修改我的信息
        /// </summary>
        /// <param name="model">修改模型</param>
        /// <returns>修改结果</returns>
        [HttpPost]
        [Route("UpdateMyInfo")]
        public MResultModel UpdateMyInfo(EditUserInModel model)
        {
            try
            {
                _bll.UpdateMyInfo(model, model.LoginUserID);
                return MResultModel.GetSuccessResultM("修改成功");
            }
            catch (ArgumentException ex)
            {
                return MResultModel.GetFailResultM(ex.Message);
            }
            catch (ApplicationException ex)
            {
                return MResultModel.GetFailResultM(ex.Message);
            }
        }
        /// <summary>
        /// 修改我的密码
        /// </summary>
        /// <param name="model">修改模型</param>
        /// <returns>修改结果</returns>
        [HttpPost]
        [Route("EditMyPassword")]
        public MResultModel EditMyPassword(EditPasswordInModel model)
        {
            try
            {
                _bll.EditMyPassword(model.ID, model.OldPassword, model.NewPassword, model.LoginUserID);
                return MResultModel.GetSuccessResultM("修改成功");
            }
            catch (ArgumentException ex)
            {
                return MResultModel.GetFailResultM(ex.Message);
            }
            catch (ApplicationException ex)
            {
                return MResultModel.GetFailResultM(ex.Message);
            }
        }
        /// <summary>
        /// 添加一个用户组
        /// </summary>
        /// <param name="model">修改用户组模型</param>
        /// <returns>修改结果</returns>
        [HttpPost]
        [Route("AddUserGroup")]
        [PermissionsCode(ApplicationManager.Permissions_UserOperation)]
        public MResultModel AddUserGroup(EditUserGroupUserInModel model)
        {
            try
            {
                _bll.AddUserGroup(model.UserID, model.UserGroupID);
                return MResultModel.GetSuccessResultM("添加成功");
            }
            catch (ArgumentException ex)
            {
                return MResultModel.GetFailResultM(ex.Message);
            }
            catch (ApplicationException ex)
            {
                return MResultModel.GetFailResultM(ex.Message);
            }
        }
        /// <summary>
        /// 移除一个用户组
        /// </summary>
        /// <param name="model">修改用户组模型</param>
        /// <returns>修改结果</returns>
        [HttpPost]
        [Route("RemoveUserGroup")]
        [PermissionsCode(ApplicationManager.Permissions_UserOperation)]
        public MResultModel RemoveUserGroup(EditUserGroupUserInModel model)
        {
            try
            {
                _bll.RemoveUserGroup(model.UserID, model.UserGroupID);
                return MResultModel.GetSuccessResultM("移除成功");
            }
            catch (ArgumentException ex)
            {
                return MResultModel.GetFailResultM(ex.Message);
            }
            catch (ApplicationException ex)
            {
                return MResultModel.GetFailResultM(ex.Message);
            }
        }
    }
}
