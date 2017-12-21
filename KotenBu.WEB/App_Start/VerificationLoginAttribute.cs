using KotenBu.BLL;
using KotenBu.Model;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace KotenBu.WEB
{
    /// <summary>
    /// 验证登录过滤器
    /// </summary>
    public class VerificationLoginAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public VerificationLoginAttribute()
        {
        }
        /// <summary>
        /// 执行Action之前触发
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.Request.Method != HttpMethod.Get)
            {
                if (actionContext.ControllerContext.Controller.GetType().GetCustomAttributes(typeof(NotVerificationLoginAttribute), false).Length == 0)
                {
                    string MeName = actionContext.ControllerContext.Request.RequestUri.Segments.Last();
                    if (actionContext.ControllerContext.Controller.GetType().GetMethod(MeName).GetCustomAttributes(typeof(NotVerificationLoginAttribute), false).Length == 0)
                    {
                        object obj = actionContext.ActionArguments.ToArray()[0].Value;
                        Type objType = obj.GetType();
                        Type iVerificationLoginType = objType.GetInterface(nameof(IVerificationLoginModel));
                        if (iVerificationLoginType != null)
                        {
                            IVerificationLoginModel queryM = (IVerificationLoginModel)obj;
                            if (queryM.LoginUserID != null && queryM.LoginUserToken != null)
                            {
                                TokenBLL tokenBLL = new TokenBLL();
                                bool resM = tokenBLL.TokenValid(queryM.LoginUserID, queryM.LoginUserToken, TokenTypeEnum.Login);
                                if (resM)
                                {
                                    #region 验证权限
                                    UserBLL userBLL = new UserBLL();
                                    object[] rightsCodeAttrs = actionContext.ControllerContext.Controller.GetType().GetCustomAttributes(typeof(PermissionsCodeAttribute), false);
                                    if (rightsCodeAttrs.Length > 0)
                                    {
                                        if (rightsCodeAttrs[0] is PermissionsCodeAttribute permissionsAttr)
                                        {
                                            if (userBLL.HasPermissions(queryM.LoginUserID, permissionsAttr.Code))
                                            {
                                                base.OnActionExecuting(actionContext);
                                            }
                                            else
                                            {
                                                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        rightsCodeAttrs = actionContext.ControllerContext.Controller.GetType().GetMethod(MeName).GetCustomAttributes(typeof(PermissionsCodeAttribute), false);
                                        if (rightsCodeAttrs.Length > 0)
                                        {
                                            if (rightsCodeAttrs[0] is PermissionsCodeAttribute permissionsAttr)
                                            {
                                                if (userBLL.HasPermissions(queryM.LoginUserID, permissionsAttr.Code))
                                                {
                                                    base.OnActionExecuting(actionContext);
                                                }
                                                else
                                                {
                                                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            base.OnActionExecuting(actionContext);
                                        }
                                    }
                                    #endregion
                                }
                                else//Token过期或用户不存在
                                {
                                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                                }
                            }
                            else//不包含所需参数400
                            {
                                actionContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                            }
                        }
                        else//不实现登录验证接口
                        {
                            actionContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// 不进行登录验证
    /// </summary>
    public class NotVerificationLoginAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public NotVerificationLoginAttribute() { }
    }
}