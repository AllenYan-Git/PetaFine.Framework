using System.Web;

namespace Infrastructure
{
    public class SessionHelper
    {
        /// <summary>
        /// 根据session名获取session对象
        /// </summary>
        /// <param name="name">session名</param>
        /// <returns>session对象</returns>
        public static object GetSession(string name)
        {
            return HttpContext.Current.Session[name];
        }

        /// <summary>
        /// 设置session
        /// </summary>
        /// <param name="name">session名</param>
        /// <param name="val">session值</param>
        public static void SetSession(string name, object val)
        {
            HttpContext.Current.Session.Remove(name);
            HttpContext.Current.Session.Add(name, val);
        }

        /// <summary>
        /// 删除一个指定的session
        /// </summary>
        /// <param name="name">session名</param>
        public static void RemoveSession(string name)
        {
            HttpContext.Current.Session.Remove(name);
        }

        /// <summary>
        /// 清空所有的session
        /// </summary>
        public static void ClearAll()
        {
            //Session.RemoveAll()通用调用Clear()方法
            HttpContext.Current.Session.Clear();
        }

        /// <summary>
        /// (全局)设置session过期时间
        ///  Timeout属性不能设置为超过 525,600 分钟(1年)的值。 默认值为 20 分钟。 
        ///  <param name="iExpires">调动有效期(分钟)</param>
        /// <remarks>同时可以在web.config中system.web节点中使用sessionState配置timeout属性</remarks>
        /// </summary>
        public static void SetTimeOut(int iExpires)
        {
            HttpContext.Current.Session.Timeout = iExpires;
        }
    }
}
