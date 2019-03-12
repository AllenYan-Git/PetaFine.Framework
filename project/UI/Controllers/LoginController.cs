using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Infrastructure;
using log4net.Config;
using Newtonsoft.Json;
using Bll;
using Entity.Respose;
using Entity.View;

namespace UI.Controllers
{
    public class LoginController : BaseController
    {
        private readonly UserBll _userInfoBll = new UserBll();

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 提交登录申请
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string SubLogin(string account, string password)
        {
            UserInfoView user = _userInfoBll.GetUserInfo(account);
            //string md5Pswd = EncryptHelper.MD5(pswd);
            if (user == null)
            {
                return JsonConvert.SerializeObject(new ExecuteResult(false, "用户名或密码错误"));
            }
            if (user.Password != password)
            {
                return JsonConvert.SerializeObject(new ExecuteResult(false, "用户名或密码错误"));
            }
            //权限
            //user.UserAccessedControls = _userInfoBll.GetAccessedControls(account);
            //服务器设置缓存 用于单点登录
            UserStatusCheck.SetCache(user);
            //设置session
            SessionHelper.SetSession(UserInfoOfSession, user);

            #region 设置cookie

            HttpCookie cookie = CookieHelper.Get(EncryptHelper.MD5(UserInfoOfCookie));
            if (cookie != null)
            {
                CookieHelper.Remove(EncryptHelper.MD5(UserInfoOfCookie));
            }
            //cookie加密
            cookie = CookieHelper.Set(EncryptHelper.MD5(UserInfoOfCookie));
            string n1 = AppSettingsHelper.GetString("UserAccount");
            string p1 = AppSettingsHelper.GetString("UserPassword");
            cookie.Values.Add(EncryptHelper.MD5(n1), user.Account);
            cookie.Values.Add(EncryptHelper.MD5(p1), user.Password);
            CookieHelper.Save(cookie, Convert.ToInt32(AppSettingsHelper.GetString("ExpiresHours")));

            #endregion

            #region 记录登录日志
            XmlConfigurator.Configure();
            log4net.ILog log = log4net.LogManager.GetLogger("loginfo");
            string result = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(result))
            {
                result = Request.ServerVariables["REMOTE_ADDR"];
            }
            if (string.IsNullOrEmpty(result))
            {
                result = Request.UserHostAddress;
            }
            if (string.IsNullOrEmpty(result))
            {
                result = "0.0.0.0";
            };
            string message = user.Account + "通过" + result + "于" + DateTime.Now.ToString() + "成功登录\r\n";
            log.Info(message);
            #endregion

            string url = Request.UrlReferrer.AbsoluteUri;

            if (url.IndexOf("?ReturnUrl") == -1)
            {
                string homepage = "http://" + Request.ServerVariables["HTTP_HOST"] + AppSettingsHelper.GetString("Homepage");
                return JsonConvert.SerializeObject(new SingleExecuteResult<UserInfoView>(true, homepage, user));
            }
            else
            {
                int index = url.IndexOf("?ReturnUrl");
                string urls = url.Substring(index, url.Length - index);
                int index1 = urls.IndexOf('=');
                int index2 = urls.IndexOf('&');
                if (index2 == -1)
                {
                    string urlreturn = urls.Substring(index1 + 1, urls.Length - index1 - 1);
                    return JsonConvert.SerializeObject(new SingleExecuteResult<UserInfoView>(true, urlreturn, user));
                }
                else
                {
                    string urlreturn = urls.Substring(index1 + 1, index2 - index1 - 1);
                    return JsonConvert.SerializeObject(new SingleExecuteResult<UserInfoView>(true, urlreturn, user));
                }


            }
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            if (SessionHelper.GetSession(UserInfoOfSession) != null)
            {
                UserInfoView userinfo = SessionHelper.GetSession(UserInfoOfSession) as UserInfoView;
                UserStatusCheck.ClearServerCredential(userinfo.Id.ToString());
                SessionHelper.RemoveSession(UserInfoOfSession);
            }
            if (CookieHelper.Get(EncryptHelper.MD5(UserInfoOfCookie)) != null)
            {
                CookieHelper.Remove(EncryptHelper.MD5(UserInfoOfCookie));
            }
            UserStatusCheck.ClearLocalCredential();
            string index = AppSettingsHelper.GetString("LoginIndex");
            string loginUrl = "http://" + Request.ServerVariables["HTTP_HOST"] + index;
            return Redirect(loginUrl);
        }
    }
}