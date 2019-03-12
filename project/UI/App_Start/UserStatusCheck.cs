using System;
using System.Web;
using Infrastructure;
using Entity.View;

namespace UI
{
    /// <summary>
    /// 用户状态检验
    /// </summary>
    public class UserStatusCheck
    {
        public const string Token = "TOKEN_";
        public const string Userkey = "KEY_";
        /// <summary>
        /// 是否被踢下线
        /// </summary>
        /// <returns></returns>
        public static bool IsKickedOff(string token, string key, out string detail)
        {
            bool kickoff = false;
            detail = "登录已过期，请重新登录。";
            object tkValue = CacheHelper.Get(token);
            if (tkValue == null)
            {
                kickoff = true;
                key = Userkey + key;
                if (CacheHelper.Get(key) != null)
                {
                    detail = "该帐号已在另一地点登录。";
                }
            }
            else if (((string)tkValue).Equals("0"))
            {
                kickoff = true;
                detail = "该帐号已在另一地点登录。";
            }
            return kickoff;
        }

        /// <summary>
        /// 清空服务端缓存
        /// </summary>
        /// <param name="userid"></param>
        public static void ClearServerCredential(string userid)
        {
            userid = Userkey + userid;
            if (CacheHelper.Get(userid) != null)
            {
                string token = CacheHelper.Get(userid).ToString();
                CacheHelper.Remove(userid);
                if (CacheHelper.Get(token) != null)
                {
                    CacheHelper.Remove(token);
                }
            }
        }

        /// <summary>
        /// 清空本地凭证
        /// </summary>
        public static void ClearLocalCredential()
        {
            //清空所有SESSION
            HttpContext.Current.Session.Clear();
            //清空所有COOKIES
            HttpCookie cookie;
            string cookieName;
            int limit = HttpContext.Current.Request.Cookies.Count;
            for (int i = 0; i < limit; i++)
            {
                cookieName = HttpContext.Current.Request.Cookies[i].Name;
                cookie = new HttpCookie(cookieName);
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="userInfo"></param>
        public static void SetCache(UserInfoView userInfo)
        {
            //每个用户随机生成一个独一无二的标识
            string token = Guid.NewGuid().ToString();
            //加入cache
            //用户重复登陆，删除已经登录用户服务端信息
            string userid = Userkey + userInfo.Id.ToString();
            if (CacheHelper.Get(userid) != null)
            {
                string tk = CacheHelper.Get(userid).ToString();
                CacheHelper.Remove(userid);
                CacheHelper.Remove(tk);
                CacheHelper.Set(tk, "0", 20);//标注已被踢出
            }
            CacheHelper.Set(userid, token, Convert.ToInt32(AppSettingsHelper.GetString("ExpiresHours")) * 60);
            CacheHelper.Set(token, userid, Convert.ToInt32(AppSettingsHelper.GetString("ExpiresHours")) * 60);
            //本地也存一份Token Cokie
            CookieHelper.Save(Token, token, Convert.ToInt32(AppSettingsHelper.GetString("ExpiresHours")));
        }
    }
}