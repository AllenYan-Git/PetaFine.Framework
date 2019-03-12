using System;
using System.Web;
using System.Web.Mvc;
using Infrastructure;
using Bll;
using Entity.View;

namespace UI
{
    /// <summary>
    /// 登录验证和权限验证
    /// 如果不需要登录验证可在action上添加[AllowAnonymous]特性
    /// 如果不需要权限验证无需加任何特性
    /// 需要权限验证则需要添加[Authorization(PermissonCode="xxxx")]的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AuthorizationAttribute : FilterAttribute, IAuthorizationFilter
    {
        private static string[] _controller = { "login" };

        //存user信息的session
        private string UserInfoOfSession => AppSettingsHelper.GetString("userInfoOfSession");

        //存user信息的session
        private string UserInfoOfCookie => AppSettingsHelper.GetString("userInfoOfCookie");

        public AuthorizationAttribute()
        {
            this.AppCode = "";
        }
        /// <summary>
        /// 登录认证并进行权限验证
        /// </summary>
        /// <param name="filterContext">筛选器上下文</param>
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            HttpRequestBase request = filterContext.RequestContext.HttpContext.Request;
            object objuserinfo = SessionHelper.GetSession(UserInfoOfSession);
            //判断是否需要验证
            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), false) ||
           filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), false))
            {
                return;
            }
            if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower().Equals("login"))
            {
                return;
            }

            //bool result = true;
            //string detail;
            if (objuserinfo == null)
            {
                HttpCookie cookie = CookieHelper.Get(EncryptHelper.MD5(UserInfoOfCookie));
                if (cookie != null)
                {
                    string n1 = AppSettingsHelper.GetString("UserAccount");
                    string p1 = AppSettingsHelper.GetString("UserPassword");
                    string account = cookie[EncryptHelper.MD5(n1)];
                    string pswd = cookie[EncryptHelper.MD5(p1)];
                    UserBll userInfoBll = new UserBll();
                    UserInfoView user = userInfoBll.GetUserInfo(account);
                    
                    if (user != null)
                    {
                        #region 登陆验证

                        if (CheckIsKickedOff(user, filterContext)) //验证是否被踢
                        {
                            return;
                        }
                        if (user.Password == pswd)
                        {
                            filterContext.Controller.ViewBag.CurrentUserInfo = user;
                            SessionHelper.SetSession(UserInfoOfSession, user);
                            return;
                        }

                        #endregion

                        //#region 权限验证

                        //user.UserAccessedControls = userInfoBll.GetAccessedControls(account);
                        //if (!string.IsNullOrEmpty(PermissionCode))
                        //{
                        //    if (!user.UserAccessedControls.Contains(PermissionCode))
                        //    {
                        //        result = false;
                        //    }
                        //    if (result && AppCode.Equals("superadmin") && !user.Account.Equals("admin"))
                        //    {
                        //        result = false;
                        //    }
                        //    if (!result)
                        //    {
                        //        if (filterContext.HttpContext.Request.IsAjaxRequest())
                        //        {
                        //            filterContext.Result = new JsonResult()
                        //            {
                        //                Data = new { Sussess = false, Message = "无权操作" },
                        //                JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        //            };
                        //        }
                        //        else
                        //        {                                 
                        //            filterContext.Result = new RedirectResult("/401.html");
                        //        }
                        //        return;
                        //    }
                        //}

                        //#endregion
                        
                    }
                }

                UserStatusCheck.ClearLocalCredential();
                //跳转到登录页
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult()
                    {
                        Data = new { Success = false, Message = "登录已过期，请重新登录。" },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    GotoLogin(filterContext);
                }
            }
            else
            {
                UserInfoView userInfo = (UserInfoView)objuserinfo;

                #region 登陆验证
                if (CheckIsKickedOff(userInfo, filterContext)) //验证是否被踢
                {
                    return;
                }
                filterContext.Controller.ViewBag.CurrentUserInfo = userInfo;

                #endregion

                //#region 权限验证
               
                //if (!string.IsNullOrEmpty(PermissionCode))
                //{
                //    if (!userInfo.UserAccessedControls.Contains(PermissionCode))
                //    {
                //        result = false;
                //    }
                //    if (result && AppCode.Equals("superadmin") && !userInfo.Account.Equals("admin"))
                //    {
                //        result = false;
                //    }
                //    if (!result)
                //    {
                //        if (filterContext.HttpContext.Request.IsAjaxRequest())
                //        {
                //            filterContext.Result = new JsonResult()
                //            {
                //                Data = new { Sussess = false, Message = "无权操作" },
                //                JsonRequestBehavior = JsonRequestBehavior.AllowGet
                //            };
                //        }
                //        else
                //        {
                //            filterContext.Result = new RedirectResult("/401.html");
                //        }
                //        return;
                //    }
                //}

                //#endregion
            }
        }

        //跳转登录页
        private static void GotoLogin(AuthorizationContext filterContext)
        {
            HttpRequestBase request = filterContext.HttpContext.Request;
            string url = "http://" + request.ServerVariables["HTTP_HOST"] + request.Url.PathAndQuery;
            if (url.IndexOf('&') > 0)
            {
                url = HttpUtility.UrlEncode(url);
            }
            string index = AppSettingsHelper.GetString("LoginIndex");
            string loginUrl = "http://" + request.ServerVariables["HTTP_HOST"] + index;
            string returnUrl = string.Format(loginUrl + "?ReturnUrl={0}", url);
            filterContext.Result = new RedirectResult(returnUrl);
        }
        private static void GotoLogin(AuthorizationContext filterContext, string alert)
        {
            HttpRequestBase request = filterContext.HttpContext.Request;
            string url = "http://" + request.ServerVariables["HTTP_HOST"] + request.Url.PathAndQuery;
            if (url.IndexOf('&') > 0)
            {
                url = HttpUtility.UrlEncode(url);
            }
            string index = AppSettingsHelper.GetString("LoginIndex");
            string loginUrl = "http://" + request.ServerVariables["HTTP_HOST"] + index;
            string returnUrl = string.Format(loginUrl + "?ReturnUrl={0}&alertstr={1}", url, alert);
            filterContext.Result = new RedirectResult(returnUrl);
        }

        /// <summary>
        /// 当前功能的权限编码
        /// </summary>
        public string PermissionCode
        {
            get;
            set;
        }

        /// <summary>
        /// 应用系统的权限代码
        /// </summary>
        public string AppCode
        {
            get;
            set;
        }

        /// <summary>
        /// 验证是否在其它地方登录
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="filterContext"></param>
        private bool CheckIsKickedOff(UserInfoView userInfo, AuthorizationContext filterContext)
        {
            string userKey = userInfo.Id.ToString();
            string tokenKey = UserStatusCheck.Token;
            HttpCookie tokenCookie = CookieHelper.Get(tokenKey);
            if (tokenCookie != null)
            {
                string tokenkey = tokenCookie.Value;
                string detail;
                if (UserStatusCheck.IsKickedOff(tokenkey, userKey, out detail))
                {
                    CacheHelper.Remove(tokenkey);
                    UserStatusCheck.ClearLocalCredential();
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = new JsonResult()
                        {
                            Data = new { Sussess = false, Message = detail },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }
                    else
                    {
                        GotoLogin(filterContext, detail);
                    }

                    return true;
                }
            }
            return false;
        }
    }
}