using Infrastructure;
using Bll;
using Entity.View;
using System;
using System.Web;
using System.Web.Mvc;

namespace UI.Controllers
{
    public class BaseController : Controller
    {
        //存user信息的session
        protected string UserInfoOfSession => AppSettingsHelper.GetString("userInfoOfSession");

        //存user信息的cookie
        protected string UserInfoOfCookie => AppSettingsHelper.GetString("userInfoOfCookie");

        /// <summary>
        /// 当前登录人员的信息
        /// </summary>
        public UserInfoView CurrentUserInfo
        {
            get
            {
                try
                {
                    //判断session是否为空
                    object obj = SessionHelper.GetSession(UserInfoOfSession);
                    if (obj == null)
                    {
                        //如果Cookie为空，跳转登录
                        //else Cookie有值，解析，验证帐号、密码，写入Session
                        HttpCookie cookie = CookieHelper.Get(EncryptHelper.MD5(UserInfoOfCookie));
                        if (cookie != null)
                        {
                            string n1 = AppSettingsHelper.GetString("UserAccount");
                            string p1 = AppSettingsHelper.GetString("UserPassword");
                            string account = cookie[EncryptHelper.MD5(n1)];
                            string pswd = cookie[EncryptHelper.MD5(p1)];
                            UserBll userInfoBll = new UserBll();
                            UserInfoView user = userInfoBll.GetUserInfo(account);
                            if (user != null && user.Password == pswd)
                            {
                                SessionHelper.SetSession(UserInfoOfSession, user);
                                return (UserInfoView)SessionHelper.GetSession(UserInfoOfSession);
                            }
                        }
                        GotoLogin();
                        return null;

                    }
                    UserInfoView u = (UserInfoView)obj;
                    return u;
                }
                catch
                {

                    //如果Cookie为空，跳转登录
                    //else Cookie有值，解析，验证帐号、密码，写入Session
                    HttpCookie cookie = CookieHelper.Get(EncryptHelper.MD5(UserInfoOfCookie));
                    if (cookie != null)
                    {
                        string n1 = AppSettingsHelper.GetString("UserAccount");
                        string p1 = AppSettingsHelper.GetString("UserPassword");
                        string account = cookie[EncryptHelper.MD5(n1)];
                        string pswd = cookie[EncryptHelper.MD5(p1)];
                        UserBll userInfoBll = new UserBll();
                        UserInfoView user = userInfoBll.GetUserInfo(account);
                        if (user != null && user.Password == pswd)
                        {
                            SessionHelper.SetSession(UserInfoOfSession, user);
                            return (UserInfoView)SessionHelper.GetSession(UserInfoOfSession);
                        }
                    }
                    GotoLogin();
                    return null;

                }
            }
            set { throw new NotImplementedException(); }
        }

        //跳转登录页
        private static void GotoLogin()
        {

            HttpRequest request = System.Web.HttpContext.Current.Request;
            string url = "http://" + request.ServerVariables["HTTP_HOST"] + request.Url.PathAndQuery;
            if (url.IndexOf('&') > 0)
            {
                url = HttpUtility.UrlEncode(url);
            }
            string index = AppSettingsHelper.GetString("LoginIndex");
            string loginUrl = "http://" + request.ServerVariables["HTTP_HOST"] + index;
            string returnUrl = string.Format(loginUrl + "?ReturnUrl={0}", url);
            System.Web.HttpContext.Current.Response.Redirect(returnUrl);
        }
    }
}